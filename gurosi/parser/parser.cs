namespace Gurosi;

public sealed class Parser {
    private TokenReader _reader;
    private List<Error> _errors;
    private StatementBlock _runBlock;
    private List<Function> _globalFunctions;
    private List<ClassModel> _classes;
    private List<string> _imports;
    private List<string> _shortens;
    private string _moduleName = "undefined";
    private FunctionIndexer _globalIndexer;
    private GenericList _currentGenericContext;

    public Semantic Parse(string filename, Lexical lexical)
    {
        _reader = new TokenReader(lexical);
        _errors = new List<Error>();
        _runBlock = null;
        _imports = new List<string>();
        _shortens = new List<string>();
        _moduleName = "undefined";
        _globalFunctions = new List<Function>();
        _classes = new List<ClassModel>();
        _globalIndexer = new FunctionIndexer();
        _currentGenericContext = new GenericList();

        ApplyMacros();

        while (!_reader.EOF)
        {
            Token token = _reader.Read();

            if (token.Type == TokenType.Run)
            {
                if (_runBlock is not null)
                {
                    AddError(ErrorProvider.Parser.RunBlockDuplicate(), _reader.GetCurrent());
                }
                else
                {
                    _runBlock = ParseStatementBlock(root: true);
                }
            }

            if (token.Type == TokenType.Mine)
            {
                // add import
                _reader.Retr();
                string name = ParseImport();
                if (name is not null)
                {
                    _imports.Add(name);
                }
            }

            if (token.Type == TokenType.Shorten)
            {
                // add shortens
                _reader.Retr();
                string name = ParseShorten();
                if (name is not null)
                {
                    _shortens.Add(name);
                }
            }

            if (token.Type == TokenType.Module)
            {
                string name = ParseModuleName();
                if (name is not null)
                {
                    if (_moduleName != "undefined")
                    {
                        AddError(ErrorProvider.Parser.ModuleNameDuplicate(), _reader.GetCurrent());
                    }
                    else
                    {
                        _moduleName = name;
                    }
                }
            }

            if (token.Type == TokenType.How)
            {
                _reader.Retr();
                Function function = ParseGlobalFunction();
                if (function is not null)
                    _globalFunctions.Add(function);
            }

            if (token.Type == TokenType.Class)
            {
                _reader.Retr();
                ClassModel classModel = ParseClass();
                if (classModel is not null)
                    _classes.Add(classModel);
            }
        }

        if (_moduleName == "undefined")
        {
            _errors.Add(new Error(ErrorProvider.Parser.ModuleNameMissing(), default));
        }

        if (_moduleName == "sys")
        {
            _errors.Add(new Error(ErrorProvider.Parser.ModuleNameNotAvailable(_moduleName), default));
        }

        Error.AttachFileName(filename, _errors);

        return new Semantic(_errors, new SemanticCode() {
            FileName = filename,
            RunBlock = _runBlock,
            ModuleName = _moduleName,
            Imports = _imports,
            Shortens = _shortens.Distinct().ToList(),
            GlobalFunctions = _globalFunctions,
            Classes = _classes
        });
    }

    private void ApplyMacros()
    {
        Dictionary<string, SimpleMacro> simpleMacros = ReadSimpleMacros();

        for (int i = 0; i < _reader.Data.Count; i++)
        {
            if (i > 0 && _reader.Data[i].Type == TokenType.Ident &&
                _reader.Data[i-1].Type != TokenType.Macro &&
                simpleMacros.ContainsKey(_reader.Data[i].Value))
            {
                string name = _reader.Data[i].Value;
                _reader.Data.RemoveAt(i);
                _reader.Data.InsertRange(i, simpleMacros[name].Tokens);
            }
        }

        _reader.Seek(0);

        Dictionary<string, TemplateMacro> templateMacros = ReadTemplateMacros();

        _reader.Seek(0);

        ApplyTemplateMacros(templateMacros);

        _reader.Seek(0);
    }

    private void ApplyTemplateMacros(Dictionary<string, TemplateMacro> templateMacros)
    {
        while (!_reader.EOF)
        {
            Token token = _reader.Read();

            if (token.Type == TokenType.Dollar)
            {
                int start = _reader.Position - 1;
                if (EnsureNext(TokenType.Ident))
                    return;
                Token nameToken = _reader.Read();
                
                if (!templateMacros.ContainsKey(nameToken.Value))
                {
                    _errors.Add(new Error(ErrorProvider.Parser.MacroNameNotFound(nameToken.Value), nameToken.Point));
                    return;
                }

                TemplateMacro macro = templateMacros[nameToken.Value];

                if (EnsureNext(TokenType.OpenParen))
                    return;

                _reader.Read(); // '('

                List<List<Token>> arguments = new List<List<Token>>();
                for (int k = 0; k < macro.TemplateCount; k++)
                {
                    //arguments.Add(_reader.Read());
                    int level = 0;
                    List<Token> buffer = new List<Token>();
                    while (!_reader.MatchNext(TokenType.CloseParen))
                    {
                        if(_reader.MatchNext(TokenType.OpenParen))
                        {
                            level++;
                        }

                        if (_reader.MatchNext(TokenType.CloseParen))
                        {
                            if (level <= 0)
                                break;
                            else
                            {
                                level--;
                            }
                        }

                        if (level == 0 && _reader.MatchNext(TokenType.Comma))
                        {
                            break;
                        }

                        buffer.Add(_reader.Read());
                    }
                    arguments.Add(buffer);
                    
                    if (k != macro.TemplateCount - 1)
                    {
                        if (EnsureNext(TokenType.Comma))
                            return;
                        _reader.Read(); // '.'
                    }
                }

                if (EnsureNext(TokenType.CloseParen))
                    return;

                _reader.Read(); // ')'

                int length = _reader.Position - start;

                List<Token> replacements = new List<Token>();

                for (int i = 0; i < macro.Tokens.Count; i++)
                {
                    if (macro.Tokens[i].Type == TokenType.MacroTemplate)
                    {
                        replacements.AddRange(arguments[int.Parse(macro.Tokens[i].Value)]);
                    }
                    else
                    {
                        replacements.Add(macro.Tokens[i]);
                    }
                }

                _reader.Data.RemoveRange(start, length);
                _reader.Data.InsertRange(start, replacements);

                // reparse
                _reader.Seek(start);
            }
        }
    }

    private Dictionary<string, TemplateMacro> ReadTemplateMacros()
    {
        Dictionary<string, TemplateMacro> map = new Dictionary<string, TemplateMacro>();

        try
        {
            while (!_reader.EOF)
            {
                Token token = _reader.Read();

                if (token.Type == TokenType.Macro)
                {
                    if (EnsureNext(TokenType.Ident))
                        return map;
                    Token nameToken = _reader.Read();

                    if (!_reader.MatchNext(TokenType.OpenParen))
                        continue;

                    _reader.Read(); // '('

                    List<string> args = new List<string>();

                    // Parse arguments
                    while (_reader.MatchNext(TokenType.Ident))
                    {
                        Token ident = _reader.Read();
                        args.Add(ident.Value);

                        if (EnsureNext(TokenType.CloseParen, TokenType.Comma))
                            return map;

                        if (_reader.MatchNext(TokenType.CloseParen))
                            break;
                        else if (_reader.MatchNext(TokenType.Comma))
                        {
                            _reader.Read();
                        }

                    }

                    _reader.Read(); // ')'

                    List<Token> tokens = _reader.TakeWhile(x => x.Type != TokenType.Semicolon);

                    for (int i = 0; i < tokens.Count; i++)
                    {
                        if (tokens[i].Type == TokenType.Ident)
                        {
                            int index = args.IndexOf(tokens[i].Value);
                            if (index > -1)
                            {
                                tokens[i] = new Token(index.ToString(), TokenType.MacroTemplate, tokens[i].Point);
                            }
                        }
                    }

                    map[nameToken.Value] = new TemplateMacro(nameToken.Value, tokens, args.Count); 
                }
            }

            return map;
        }
        catch
        {
            _errors.Add(new Error(ErrorProvider.Parser.InvalidSimpleMacro(), default));
            return new Dictionary<string, TemplateMacro>();
        }
    }

    private Dictionary<string, SimpleMacro> ReadSimpleMacros()
    {
        Dictionary<string, SimpleMacro> map = new Dictionary<string, SimpleMacro>();

        try
        {
            while (!_reader.EOF)
            {
                Token token = _reader.Read();

                if (token.Type == TokenType.Macro)
                {
                    if (EnsureNext(TokenType.Ident))
                        return map;
                    Token nameToken = _reader.Read();
                    if (_reader.MatchNext(TokenType.OpenParen))
                        continue;
                    List<Token> tokens = _reader.TakeWhile(x => x.Type != TokenType.Semicolon);
                    _reader.Read();

                    map.TryAdd(nameToken.Value, new SimpleMacro(nameToken.Value, tokens));
                }
            }

            return map;
        }
        catch 
        {
            _errors.Add(new Error(ErrorProvider.Parser.InvalidSimpleMacro(), default));
            return new Dictionary<string, SimpleMacro>();
        }
    }

    private Function ParseGlobalFunction()
    {
        _reader.Read(); // 'how'

        TypeData returnType = ParseType();
        if (returnType is null)
            return null;

        if (EnsureNext(TokenType.Ident))
            return null;

        Token nameToken = _reader.Read();

        if (EnsureNext(TokenType.OpenParen))
            return null;

        _reader.Read(); // '('

        // read arguments section
        List<DefArgument> args = new List<DefArgument>();

        while (!_reader.EOF && (_reader.MatchNext(TokenType.Ident) || _reader.MatchNext(TokenType.Comma)))
        {
            if (_reader.MatchNext(TokenType.Ident))
            {
                Token argNameToken = _reader.Read();
                if (EnsureNext(TokenType.Colon))
                    return null;
                _reader.Read(); // ':'
                TypeData argType = ParseType();
                if (argType is null)
                    return null;

                args.Add(new DefArgument(argNameToken.Value, argType, argNameToken));
            }
            else if (_reader.MatchNext(TokenType.Comma))
            {
                _reader.Read(); // ','
            }
        }

        if (EnsureNext(TokenType.CloseParen))
            return null;

        _reader.Read(); // ')'

        if (EnsureNext(TokenType.Colon, TokenType.OpenBrace))
            return null;

        TypeData extType = null;
        string extName = null;
        if (_reader.MatchNext(TokenType.Colon))
        {
            _reader.Read();  // ':'

            if (EnsureNext(TokenType.Extend))
                return null;

            (extType, extName) = ParseExtend();
            if (extType is null)
                return null;
        }

        StatementBlock body = ParseStatementBlock();
        if (body is null)
            return null;

        return new Function(_moduleName, nameToken.Value + "~" + _globalIndexer.GetIndex(nameToken.Value), returnType, args, extType, extName, body, nameToken);
    }

    private (TypeData type, string name) ParseExtend()
    {
        _reader.Read(); // 'extend'

        if (EnsureNext(TokenType.LessThan))
            return (null, null);

        _reader.Read();  // '<'

        TypeData type = ParseType();
        if (type is null)
            return (null, null);

        if (EnsureNext(TokenType.GreaterThan))
            return (null, null);

        _reader.Read();  // '>'

        if (EnsureNext(TokenType.Ident))
            return (null, null);

        Token nameToken = _reader.Read();


        return (type, nameToken.Value);
    }

    private MethodModel ParseMethod()
    {
        _reader.Read(); // 'how'

        TypeData returnType = ParseType();
        if (returnType is null)
            return null;

        if (EnsureNext(TokenType.Ident))
            return null;

        Token nameToken = _reader.Read();

        if (EnsureNext(TokenType.OpenParen))
            return null;

        _reader.Read(); // '('

        // read arguments section
        List<DefArgument> args = new List<DefArgument>();

        while (!_reader.EOF && (_reader.MatchNext(TokenType.Ident) || _reader.MatchNext(TokenType.Comma)))
        {
            if (_reader.MatchNext(TokenType.Ident))
            {
                Token argNameToken = _reader.Read();
                if (EnsureNext(TokenType.Colon))
                    return null;
                _reader.Read(); // ':'
                TypeData argType = ParseType();
                if (argType is null)
                    return null;

                args.Add(new DefArgument(argNameToken.Value, argType, argNameToken));
            }
            else if (_reader.MatchNext(TokenType.Comma))
            {
                _reader.Read(); // ','
            }
        }

        if (EnsureNext(TokenType.CloseParen))
            return null;

        _reader.Read(); // ')'

        List<AccessIdentifier> identifiers;

        if (EnsureNext(TokenType.OpenBrace, TokenType.Colon))
            return null;
        
        if (_reader.MatchNext(TokenType.OpenBrace))
        {
            // without identifiers (private)
            identifiers = new List<AccessIdentifier>() { AccessIdentifier.Private };
        }
        else
        {
            // with identifiers
            _reader.Read(); // ':'
            identifiers = ParseAccessIdentifiers();
        }

        if (EnsureNext(TokenType.OpenBrace))
            return null;

        StatementBlock body = ParseStatementBlock();
        if (body is null)
            return null;

        return new MethodModel(nameToken.Value, returnType, args, body, identifiers, nameToken);
    }

    private MethodModel ParseConstructor()
    {
        Token token = _reader.Read(); // 'init'

        TypeData returnType = new TypeData(BuiltinTypes.NULL);

        if (EnsureNext(TokenType.OpenParen))
            return null;

        _reader.Read(); // '('

        // read arguments section
        List<DefArgument> args = new List<DefArgument>();

        while (!_reader.EOF && (_reader.MatchNext(TokenType.Ident) || _reader.MatchNext(TokenType.Comma)))
        {
            if (_reader.MatchNext(TokenType.Ident))
            {
                Token argNameToken = _reader.Read();
                if (EnsureNext(TokenType.Colon))
                    return null;
                _reader.Read(); // ':'
                TypeData argType = ParseType();
                if (argType is null)
                    return null;

                args.Add(new DefArgument(argNameToken.Value, argType, argNameToken));
            }
            else if (_reader.MatchNext(TokenType.Comma))
            {
                _reader.Read(); // ','
            }
        }

        if (EnsureNext(TokenType.CloseParen))
            return null;

        _reader.Read(); // ')'

        List<AccessIdentifier> identifiers;

        if (EnsureNext(TokenType.OpenBrace, TokenType.Colon))
            return null;
        
        if (_reader.MatchNext(TokenType.OpenBrace))
        {
            // without identifiers (private)
            identifiers = new List<AccessIdentifier>() { AccessIdentifier.Private };
        }
        else
        {
            // with identifiers
            _reader.Read(); // ':'
            identifiers = ParseAccessIdentifiers();
        }

        if (EnsureNext(TokenType.OpenBrace))
            return null;

        StatementBlock body = ParseStatementBlock();
        if (body is null)
            return null;

        return new MethodModel("ctor", returnType, args, body, identifiers, token);
    }

    private ClassModel ParseClass()
    {
        _reader.Read(); // 'class'
        FunctionIndexer findexer = new FunctionIndexer();

        if (EnsureNext(TokenType.Ident))
            return null;

        string className  = _reader.Read().Value;

        GenericList genericContext = null;

        if (_reader.MatchNext(TokenType.LessThan))
        {
            genericContext = ParseGenerics();
        }

        _currentGenericContext = genericContext;

        if (EnsureNext(TokenType.Colon, TokenType.OpenBrace))
            return null;

        List<AccessIdentifier> identifiers;
        
        if (_reader.MatchNext(TokenType.Colon))
        {
            // with identifiers
            _reader.Read(); // ':'
            identifiers = ParseAccessIdentifiers();
        }
        else
        {
            // without explicit identifiers (public)
            identifiers = new List<AccessIdentifier>() { AccessIdentifier.Public };
        }

        TypeData baseType = null;

        // read if inherited
        if (_reader.MatchNext(TokenType.Inherit))
        {
            _reader.Read(); // '<-'

            TypeData type = ParseType();
            if (type is null)
                return null;

            baseType = type;
        }

        if (EnsureNext(TokenType.OpenBrace))
            return null;

        _reader.Read(); // '{'

        List<FieldModel> fields = new List<FieldModel>();
        List<FieldModel> stcFields = new List<FieldModel>();
        List<MethodModel> methods = new List<MethodModel>();
        List<MethodModel> stcMethods = new List<MethodModel>();

        while (!_reader.EOF)
        {
            Token token = _reader.Read();

            if (token.Type == TokenType.CloseBrace)
                break;

            if (token.Type == TokenType.Field)
            {
                _reader.Retr();
                FieldModel field = ParseField();
                if (field is null)
                    continue;
                if (field.Identifiers.Contains(AccessIdentifier.Static))
                {
                    stcFields.Add(field);
                }
                else
                {
                    fields.Add(field);
                }
            }
            else if (token.Type == TokenType.How)
            {
                _reader.Retr();
                MethodModel method = ParseMethod();
                if (method is not null)
                {
                    method.Name += "~" + findexer.GetIndex(method.Name);
                    if (method.AccessIdentifiers.Contains(AccessIdentifier.Static))
                    {
                        stcMethods.Add(method);
                    }
                    else
                    {
                        methods.Add(method);
                    }
                }
            }
            else if (token.Type == TokenType.Init)
            {
                // parse constructor

                _reader.Retr();
                MethodModel constructor = ParseConstructor();
                constructor.Name += "~" + findexer.GetIndex(constructor.Name);
                if (constructor is not null)
                {
                    if (!constructor.AccessIdentifiers.Contains(AccessIdentifier.Static))
                    {
                        methods.Add(constructor);
                    }
                    else
                    {
                        // Error. constructor can't be static.
                        AddError(ErrorProvider.Parser.ConstructorStatic(), token);
                    }
                }
            }
        }

        _currentGenericContext = null;

        return new ClassModel()
        {
            Module = _moduleName,
            Name = className,
            Identifiers = identifiers,
            Fields = fields,
            Methods = methods,
            StcFields = stcFields,
            StcMethods = stcMethods,
            GenericCount = genericContext is null ? 0 : genericContext.Count,
            BaseType = baseType
        };
    }

    private StatementBlock ParseStatementBlock(bool allowOneLiner = false, bool inloop = false, bool root = false)
    {
        Token startToken = _reader.GetCurrent();
        if (allowOneLiner && !_reader.MatchNext(TokenType.OpenBrace))
        {
            Statement s = ParseStatement();

            if (s is null)
                return null;
            else
            {
                return new StatementBlock(new List<Statement>() { s }, startToken);
            }
        }
        else
        {
            if (EnsureNext(TokenType.OpenBrace))
                return null;
        }
        _reader.Read();  // '{'

        List<Statement> statements = new List<Statement>();

        while (!_reader.EOF && !_reader.MatchNext(TokenType.CloseBrace))
        {
            Statement s = ParseStatement();
            if (s is not null)
            {
                
                statements.Add(s);
            }
        }

        _reader.Read(); // '}'

        return new StatementBlock(statements, startToken);
    }

    

    private bool EnsureNext(TokenType expected)
    {
        if (_reader.EOF || !_reader.MatchNext(expected))
        {
            _errors.Add(new Error(ErrorProvider.Parser.InvalidToken(_reader.GetCurrent().Value, expected.ToString()), _reader.GetCurrent().Point));
            return true;
        }

        return false;
    }

    private bool EnsureNext(params TokenType[] expecteds)
    {
        if (expecteds.All(x => !_reader.MatchNext(x)))
        {
            _errors.Add(new Error(ErrorProvider.Parser.InvalidToken(_reader.GetCurrent().Value, string.Join(", ", expecteds)), _reader.GetCurrent().Point));
            return true;
        }

        return false;
    }

    private void AddError(string msg)
    {
        Token current = _reader.GetCurrent();
        if (current is null)
            AddError(msg, current);
        else
            AddError(msg, _reader.GetOffset(-1));
    }

    private void AddError(string msg, Token source)
    {
        _errors.Add(new Error(msg, source.Point));
    }



    private LetStatement ParseLetStatement()
    {
        Token rootToken = _reader.Read(); // 'let'

        if (EnsureNext(TokenType.Ident))
            return null;

        Token nameToken = _reader.Read();

        if (EnsureNext(TokenType.Colon))
            return null;

        _reader.Read();

        TypeData type = ParseType();
        if (type is null)
            return null;

        if (EnsureNext(TokenType.Semicolon, TokenType.Assign))
            return null;
        
        if (_reader.MatchNext(TokenType.Semicolon))
        {
            _reader.Read();
            return new LetStatement(nameToken.Value, type, rootToken);
        }

        _reader.Read();

        Expression value = ParseExpression();

        if (EnsureNext(TokenType.Semicolon))
            return null;

        _reader.Read();

        return new LetStatement(nameToken.Value, type, value, rootToken);
    }

    private ConstStatement ParseConstStatement()
    {
        Token rootToken = _reader.Read(); // 'const'

        if (EnsureNext(TokenType.Ident))
            return null;

        Token nameToken = _reader.Read();

        if (EnsureNext(TokenType.Colon))
            return null;

        _reader.Read();

        TypeData type = ParseType();
        if (type is null)
            return null;

        if (EnsureNext(TokenType.Semicolon, TokenType.Assign))
            return null;
        
        if (_reader.MatchNext(TokenType.Semicolon))
        {
            _reader.Read();
            AddError(ErrorProvider.Parser.ConstValueMissing(), rootToken);
            return null;
        }

        _reader.Read();

        Expression value = ParseExpression();

        if (EnsureNext(TokenType.Semicolon))
            return null;

        _reader.Read();

        return new ConstStatement(nameToken.Value, type, value, rootToken);
    }

    private string ParseModuleName()
    {
        if (EnsureNext(TokenType.Ident))
        {
            return null;
        }

        Token nameToken = _reader.Read();

        if (EnsureNext(TokenType.Semicolon))
        {
            return null;
        }

        _reader.Read(); // ';'

        return nameToken.Value;
    }

    private string ParseImport()
    {
        _reader.Read(); // 'mine'

        if (EnsureNext(TokenType.Ident, TokenType.StringLiteral))
        {
            return null;
        }

        Token nameToken = _reader.Read();

        if (EnsureNext(TokenType.Semicolon))
        {
            return null;
        }

        _reader.Read(); // ';'

        return nameToken.Value;
    }

    private string ParseShorten()
    {
        _reader.Read(); // 'shorten'

        if (EnsureNext(TokenType.Ident))
        {
            return null;
        }

        Token nameToken = _reader.Read();

        if (EnsureNext(TokenType.Semicolon))
        {
            return null;
        }

        _reader.Read(); // ';'

        return nameToken.Value;
    }

    private TypeData ParseType()
    {
        if (_reader.MatchNext(TokenType.String))
        {
            _reader.Read();
            return new TypeData(BuiltinTypes.STRING);
        }
        else if (_reader.MatchNext(TokenType.Int))
        {
            _reader.Read();
            return new TypeData(BuiltinTypes.INT);
        }
        else if (_reader.MatchNext(TokenType.Float))
        {
            _reader.Read();
            return new TypeData(BuiltinTypes.FLOAT);
        }
        else if (_reader.MatchNext(TokenType.Double))
        {
            _reader.Read();
            return new TypeData(BuiltinTypes.DOUBLE);
        }
        else if (_reader.MatchNext(TokenType.Boolean))
        {
            _reader.Read();
            return new TypeData(BuiltinTypes.BOOLEAN);
        }
        else if (_reader.MatchNext(TokenType.Any))
        {
            _reader.Read();
            return new TypeData(BuiltinTypes.ANY);
        }
        else if (_reader.MatchNext(TokenType.FuncPtr))
        {
            _reader.Read();
            return new TypeData(BuiltinTypes.FUNC_PTR);
        }
        else if (_reader.MatchNext(TokenType.Void))
        {
            _reader.Read();
            return new TypeData(BuiltinTypes.NULL);
        }
        else if (_reader.MatchNext(TokenType.Array))
        {
            _reader.Read(); // 'array'
            if (EnsureNext(TokenType.LessThan))
            {
                return null;
            }

            _reader.Read(); // '<'

            TypeData innerType = ParseType();
            if (innerType is null)
                return null;
            
            if (EnsureNext(TokenType.GreaterThan))
            {
                return null;
            }

            _reader.Read(); // '>'

            return new TypeData(innerType);
        }
        else if (_reader.MatchNext(TokenType.LessThan))
        {
            _reader.Read();  // '<'

            if (EnsureNext(TokenType.Ident))
                return null;

            Token nameToken = _reader.Read();

            if (EnsureNext(TokenType.GreaterThan))
                return null;

            _reader.Read();  // '>'

            if (_currentGenericContext is null)
            {
                AddError(ErrorProvider.Parser.InvalidGenericType(), nameToken);
                return null;
            }

            int index = _currentGenericContext.GetIndex(nameToken.Value);
            
            if (index == -1)
            {
                AddError(ErrorProvider.Parser.InvalidGenericType(), nameToken);
                return null;
            }

            return TypeData.CreateGeneric(index);
        }
        else if (_reader.MatchNext(TokenType.Arrbase))
        {
            Token arrbaseToken = _reader.Read();

            return TypeData.CreateArrayBase();
        }
        else
        {
            Expression type = ParseSymbolExpression();

            if (type is not IdentExpression && type is not DotExpression &&
                type is not GenericExpression)
            {
                AddError(ErrorProvider.Parser.InvalidType());
                return null;
            }

            return new TypeData(type);
        }
    }

    private List<AccessIdentifier> ParseAccessIdentifiers()
    {
        List<AccessIdentifier> buffer = new List<AccessIdentifier>();

        while (_reader.MatchNext(TokenType.Public) ||
                _reader.MatchNext(TokenType.Private) ||
                _reader.MatchNext(TokenType.Moduled) ||
                _reader.MatchNext(TokenType.Static))
        {
            Token token = _reader.Read();

            if (token.Type == TokenType.Public)
                buffer.Add(AccessIdentifier.Public);
            else if (token.Type == TokenType.Private)
                buffer.Add(AccessIdentifier.Private);
            else if (token.Type == TokenType.Moduled)
                buffer.Add(AccessIdentifier.Moduled);
            else if (token.Type == TokenType.Static)
                buffer.Add(AccessIdentifier.Static);

            if (_reader.MatchNext(TokenType.Comma))
            {
                _reader.Read();
            }
        }

        return buffer;
    }

    private GenericList ParseGenerics()
    {
        GenericList list = new GenericList();

        _reader.Read(); // '<'

        while (_reader.MatchNext(TokenType.Ident))
        {
            Token token = _reader.Read();

            list.RegisterBack(token.Value);

            if (!_reader.MatchNext(TokenType.Comma))
            {
                break;
            }
            else
            {
                _reader.Read();
            }
        }

        if (EnsureNext(TokenType.GreaterThan))
            return list;

        _reader.Read();  // '>'

        return list;
    }

    private FieldModel ParseField()
    {
        _reader.Read(); // 'field'

        List<AccessIdentifier> identifiers;

        if (_reader.MatchNext(TokenType.Ident))
        {
            // without identifiers (private)
            identifiers = new List<AccessIdentifier>() { AccessIdentifier.Private };
        }
        else
        {
            identifiers = ParseAccessIdentifiers();
        }

        if (EnsureNext(TokenType.Ident))
            return null;

        Token nameToken = _reader.Read();

        if (EnsureNext(TokenType.Colon))
            return null;

        _reader.Read(); // ':'

        TypeData type = ParseType();
        
        if (type is null)
            return null;

        if (EnsureNext(TokenType.Semicolon, TokenType.Assign))
            return null;
        
        if (_reader.MatchNext(TokenType.Semicolon))
        {
            _reader.Read(); // ';'

            return new FieldModel(nameToken.Value, identifiers, type, nameToken);
        }
        else
        {
            _reader.Read(); // '='

            Expression value = ParseExpression();

            if (EnsureNext(TokenType.Semicolon))
                return null;

            _reader.Read(); // ';'

            return new FieldModel(nameToken.Value, identifiers, type, value, nameToken);
        }
    }

    #region Expression Parsers

    private Expression ParseSymbolExpression()
    {
        Expression expr = null;
        while (_reader.MatchNext(TokenType.Ident))
        {
            Token token = _reader.Read();

            if (expr is null)
            {
                expr = new IdentExpression(token.Value, token);
            }
            else
            {
                expr = new DotExpression(expr, token.Value, token);
            }

            if (_reader.MatchNext(TokenType.LessThan))
            {
                // parse generics
                Token t = _reader.Read();  // '<'

                List<TypeData> types = new List<TypeData>();

                while (true)
                {
                    TypeData type = ParseType();

                    if (type is null)
                        break;

                    types.Add(type);

                    if (_reader.MatchNext(TokenType.Comma))
                    {
                        _reader.Read();
                    }
                    else
                    {
                        break;
                    }
                }
                

                if (EnsureNext(TokenType.GreaterThan))
                {
                    return null;
                }

                _reader.Read();  // '>'

                expr = new GenericExpression(expr, types, t);
            }

            if (_reader.MatchNext(TokenType.DotOperator))
            {
                _reader.Read();
            }
            else
            {
                break;
            }
        }

        return expr;
    }

    private Expression ParseExpression()
    {
        Expression expr = ParseTermEq();
        while (!_reader.EOF && (
            _reader.MatchNext(TokenType.LogOr) ||
            _reader.MatchNext(TokenType.LogAnd)
        ))
        {
            Token t = _reader.Read();

            if (t.Type == TokenType.LogOr)
            {
                expr = new CalcExpression(Operator.LogOr, expr, ParseTermEq());
            }
            else if (t.Type == TokenType.LogAnd)
            {
                expr = new CalcExpression(Operator.LogAnd, expr, ParseTermEq());
            }
        }

        return expr;
    }

    private Expression ParseTermEq()
    {
        Expression expr = ParseTermCU();

        while (!_reader.EOF &&(
            _reader.MatchNext(TokenType.Equal) ||
            _reader.MatchNext(TokenType.NotEqual)
        ))
        {
            Token t = _reader.Read();
            if (t.Type == TokenType.Equal)
            {
                expr = new CalcExpression(Operator.Equal, expr, ParseTermCU());
            }
            else if (t.Type == TokenType.NotEqual)
            {
                expr = new CalcExpression(Operator.NotEqual, expr, ParseTermCU());
            }
        }

        return expr;
    }

    private Expression ParseTermCU()
    {
        Expression expr = ParseTermF();
        while (!_reader.EOF &&(
            _reader.MatchNext(TokenType.GreaterThan) ||
            _reader.MatchNext(TokenType.GreaterThanEqual) ||
            _reader.MatchNext(TokenType.LessThan) ||
            _reader.MatchNext(TokenType.LessThanEqual)
        ))
        {
            Token t = _reader.Read();
            

            if (t.Type == TokenType.GreaterThan)
            {
                expr = new CalcExpression(Operator.Gt, expr, ParseTermF());
            }
            else if (t.Type == TokenType.GreaterThanEqual)
            {
                expr = new CalcExpression(Operator.Gte, expr, ParseTermF());
            }
            else if (t.Type == TokenType.LessThan)
            {
                expr = new CalcExpression(Operator.Lt, expr, ParseTermF());
            }
            else if (t.Type == TokenType.LessThanEqual)
            {
                expr = new CalcExpression(Operator.Lte, expr, ParseTermF());
            }
        }

        return expr;
    }

    private Expression ParseTermF()
    {
        Expression expr = ParseTerm0();
        while (!_reader.EOF && (
            _reader.MatchNext(TokenType.Plus) ||
            _reader.MatchNext(TokenType.Minus)
        ))
        {
            Token t = _reader.Read();

            if (t.Type == TokenType.Plus)
            {
                expr = new CalcExpression(Operator.Add, expr, ParseTerm0());
            }
            else if (t.Type == TokenType.Minus)
            {
                expr = new CalcExpression(Operator.Sub, expr, ParseTerm0());
            }
        }

        return expr;
    }
    
    private Expression ParseTerm0()
    {
        Expression term1 = ParseTerm1();
        Expression expr = term1;
        while (!_reader.EOF && (
               _reader.MatchNext(TokenType.As) ||
               _reader.MatchNext(TokenType.Asterisk) ||
               _reader.MatchNext(TokenType.Power) ||
               _reader.MatchNext(TokenType.Slash) ||
               _reader.MatchNext(TokenType.Percent)))
        {
            Token t = _reader.Read();
            if (t.Type == TokenType.As)
            {
                TypeData type = ParseType();
                if (type is null)
                    return ImmediateExpression.MakeInt(0, default);

                expr = new ObjectCastExpression(expr, type, t);
            }
            else if (t.Type == TokenType.Asterisk)
            {
                expr = new CalcExpression(Operator.Mult, expr, ParseTerm1());
            }
            else if (t.Type == TokenType.Power)
            {
                expr = new CalcExpression(Operator.Power, expr, ParseTerm1());
            }
            else if (t.Type == TokenType.Slash)
            {
                expr = new CalcExpression(Operator.Div, expr, ParseTerm1());
            }
            else if (t.Type == TokenType.Percent)
            {
                expr = new CalcExpression(Operator.Mod, expr, ParseTerm1());
            }
        }

        return expr;
    }

    private Expression ParseTerm1()
    {
        Expression term2 = ParseTerm2();
        if (_reader.MatchNext(TokenType.Increment))
        {
            _reader.Read();
            return new IncrExpression(term2, _reader.GetOffset(-1));
        }
        if (_reader.MatchNext(TokenType.Decrement))
        {
            _reader.Read();
            return new DecrExpression(term2, _reader.GetOffset(-1));
        }

        if (!_reader.EOF && _reader.MatchNext(TokenType.OpenParen))
        {
            List<Expression> args = ParseFunctionArgBody();

            term2 = new FuncExpression(term2, args);
        }

        while (!_reader.EOF && (
                _reader.MatchNext(TokenType.DotOperator)
               ))
        {
            Token t = _reader.Read();
            if (_reader.MatchNext(TokenType.Ident))
            {
                Token idtoken = _reader.Read();
                term2 = new DotExpression(term2, idtoken.Value, idtoken);
            }

            if (!_reader.EOF && _reader.MatchNext(TokenType.OpenBracket))
            {
                _reader.Read();  // '['

                Expression index = ParseExpression();

                if (EnsureNext(TokenType.CloseBracket))
                {
                    return ImmediateExpression.MakeInt(0, Token.Empty);
                }

                _reader.Read(); // ']'

                term2 = new IndexExpression(term2, index);
            }

            if (!_reader.EOF && _reader.MatchNext(TokenType.OpenParen))
            {
                List<Expression> args = ParseFunctionArgBody();

                term2 = new FuncExpression(term2, args);
            }
        }

        return term2;
    }

    private Expression ParseTerm2()
    {
        Expression term3 = ParseTerm3();

        if (!_reader.EOF && _reader.MatchNext(TokenType.OpenBracket))
        {
            _reader.Read();  // '['

            Expression index = ParseExpression();

            if (EnsureNext(TokenType.CloseBracket))
            {
                return ImmediateExpression.MakeInt(0, Token.Empty);
            }

            _reader.Read(); // ']'

            term3 = new IndexExpression(term3, index);
        }

        if (!_reader.EOF && _reader.MatchNext(TokenType.OpenParen))
        {
            List<Expression> args = ParseFunctionArgBody();

            term3 = new FuncExpression(term3, args);
        }

        while (!_reader.EOF && _reader.MatchNext(TokenType.DotOperator))
        {
            _reader.Read();
            
            if (EnsureNext(TokenType.Ident))
            {
                return ImmediateExpression.MakeInt(0, Token.Empty);
            }

            Token token = _reader.Read();
            term3 = new DotExpression(term3, token.Value, token);

            if (!_reader.EOF && _reader.MatchNext(TokenType.OpenBracket))
            {
                _reader.Read();  // '['

                Expression index = ParseExpression();

                if (EnsureNext(TokenType.CloseBracket))
                {
                    return ImmediateExpression.MakeInt(0, Token.Empty);
                }

                _reader.Read(); // ']'

                term3 = new IndexExpression(term3, index);
            }

            if (!_reader.EOF && _reader.MatchNext(TokenType.OpenParen))
            {
                List<Expression> args = ParseFunctionArgBody();

                term3 = new FuncExpression(term3, args);
            }
        }

        

        return term3;
    }

    private Expression ParseTerm3()
    {
        if (_reader.MatchNext(TokenType.StringLiteral))
        {
            return ImmediateExpression.MakeString(_reader.Read().Value, _reader.GetOffset(-1));
        }
        else if (_reader.MatchNext(TokenType.IntLiteral))
        {
            return ImmediateExpression.MakeInt(int.Parse(_reader.Read().Value), _reader.GetOffset(-1));
        }
        else if (_reader.MatchNext(TokenType.FloatLiteral))
        {
            return ImmediateExpression.MakeFloat(float.Parse(_reader.Read().Value), _reader.GetOffset(-1));
        }
        else if (_reader.MatchNext(TokenType.DoubleLiteral))
        {
            return ImmediateExpression.MakeDouble(double.Parse(_reader.Read().Value), _reader.GetOffset(-1));
        }
        else if (_reader.MatchNext(TokenType.Self))
        {
            return new SelfExpression(_reader.Read());
        }
        else if (_reader.MatchNext(TokenType.Null))
        {
            return new NullExpression(_reader.Read());
        }
        else if (_reader.MatchNext(TokenType.NvRetv))
        {
            Token token = _reader.Read();  // 'nvretv'

            TypeData type = ParseType();
            if (type is null)
                return ImmediateExpression.MakeInt(0, _reader.GetCurrent());

            return new NvRetvExpression(type, token);
        }
        else if (_reader.MatchNext(TokenType.True) || _reader.MatchNext(TokenType.False))
        {
            return ImmediateExpression.MakeBoolean(bool.Parse(_reader.Read().Value), _reader.GetOffset(-1));
        }
        else if (_reader.MatchNext(TokenType.OpenParen))
        {
            // exp with ()
            _reader.Read();

            if (_reader.MatchNext(TokenType.Float) ||
                _reader.MatchNext(TokenType.Double))
            {
                Token token = _reader.GetCurrent();
                TypeData type = ParseType();

                _reader.Read();

                Expression target = ParseTerm1();

                return new CastExpression(target, type, token);
            }
            else
            {
                Expression expr = ParseExpression();

                _reader.Read();
                return expr;
            }
        }
        else if (_reader.MatchNext(TokenType.Minus))
        {
            _reader.Read();
            Token token = _reader.Read();
            _reader.Retr();
            Expression target = ParseTerm1();
            return new CalcExpression(Operator.Mult, ImmediateExpression.MakeInt(-1, token), target);
        }
        else if (_reader.MatchNext(TokenType.And))
        {
            // Pointer extraction.

            Token and = _reader.Read();

            // parse type map.
            List<TypeData> argumentTypes = new List<TypeData>();

            if (EnsureNext(TokenType.OpenParen))
                return ImmediateExpression.MakeInt(0, default);

            _reader.Read(); // '('

            while (!_reader.EOF &&
                    !_reader.MatchNext(TokenType.CloseParen))
            {
                TypeData type = ParseType();

                if (type is null)
                    return ImmediateExpression.MakeInt(0, default);

                argumentTypes.Add(type);

                if (EnsureNext(TokenType.Comma, TokenType.CloseParen))
                    return ImmediateExpression.MakeInt(0, default);

                if (_reader.MatchNext(TokenType.Comma))
                    _reader.Read();
            }

            _reader.Read();  // ')'


            Expression right = ParseTerm1();

            if (right is not IdentExpression &&
                right is not DotExpression)
            {
                _errors.Add(new Error(ErrorProvider.Parser.InvalidPointerTarget(), right.Token.Point));
                return ImmediateExpression.MakeInt(0, default);
            }

            return new FuncPointerExpression(right, argumentTypes, and);
        }
        else if (_reader.MatchNext(TokenType.Ident))
        {
            Token token = _reader.Read();
            return new IdentExpression(token.Value, token);

        }
        else if (_reader.MatchNext(TokenType.New))
        {
            Token token = _reader.Read(); // 'new'

            TypeData type = ParseType();
            if (type is null)
            {
                AddError(ErrorProvider.Parser.InvalidType(), _reader.GetCurrent());
                return ImmediateExpression.MakeInt(0, _reader.GetCurrent());
            }

            if (EnsureNext(TokenType.OpenParen))
                return ImmediateExpression.MakeInt(0, _reader.GetCurrent());

            List<Expression> args = ParseFunctionArgBody();

            return new NewExpression(type, args, token);
        }
        else if (_reader.MatchNext(TokenType.Excl))
        {
            // TODO: negate booleans.
            return ImmediateExpression.MakeInt(0, _reader.GetCurrent());
        }
        else if (_reader.MatchNext(TokenType.Alloc))
        {
            // array allocation.

            Token allocToken = _reader.Read(); // 'alloc'

            TypeData arrType = ParseType();
            if (arrType is null)
            {
                AddError(ErrorProvider.Parser.InvalidType(), _reader.GetCurrent());
                return ImmediateExpression.MakeInt(0, Token.Empty);
            }

            if (EnsureNext(TokenType.OpenBracket))
            {
                return ImmediateExpression.MakeInt(0, Token.Empty);
            }

            _reader.Read(); // '['

            Expression length = ParseExpression();
            if (length is null)
                return ImmediateExpression.MakeInt(0, Token.Empty);

            if (EnsureNext(TokenType.CloseBracket))
            {
                return ImmediateExpression.MakeInt(0, Token.Empty);
            }

            _reader.Read(); // ']'

            return new AllocExpression(arrType, length, allocToken);
        }
        else
        {
            Token next = _reader.GetOffset(-1);
            AddError(ErrorProvider.Parser.InvalidExpressionToken(next.Value), next);
            return ImmediateExpression.MakeInt(0, Token.Empty);
        }
    }

    #endregion

    private List<Expression> ParseFunctionArgBody()
    {
        _reader.Read(); // '('

        if (_reader.MatchNext(TokenType.CloseParen))  // No argument found
        {
            _reader.Read(); // ')'
            return new List<Expression>();
        }

        List<Expression> args = new List<Expression>();
        Expression first = ParseExpression();
        args.Add(first);

        while (!_reader.EOF && _reader.MatchNext(TokenType.Comma))
        {
            _reader.Read(); // ','
            Expression value = ParseExpression();
            args.Add(value);
        }

        if (EnsureNext(TokenType.CloseParen))
        {
            return new List<Expression>();
        }

        _reader.Read(); // ')'

        return args;
    }

    private Statement ParseStatement()
    {
        while (!_reader.EOF && !_reader.MatchNext(TokenType.CloseBrace))
        {
            Token token = _reader.Read();

            if (token.Type == TokenType.Let)
            {
                _reader.Retr();
                LetStatement s = ParseLetStatement();
                if (s is not null)
                {
                    return s;
                }
            }
            else if (token.Type == TokenType.Const)
            {
                _reader.Retr();
                ConstStatement s = ParseConstStatement();
                if (s is not null)
                {
                    return s;
                }
            }
            else if (token.Type == TokenType.If)
            {
                _reader.Retr();
                IfStatement s = ParseIfStatement();
                if (s is not null)
                    return s;
            }
            else if (token.Type == TokenType.While)
            {
                _reader.Retr();
                WhileStatement s = ParseWhileStatement();
                if (s is not null)
                    return s;
            }
            else if (token.Type == TokenType.For)
            {
                _reader.Retr();
                ForStatement s = ParseForStatement();
                if (s is not null)
                    return s;
            }
            else if (token.Type == TokenType.Return)
            {
                _reader.Retr();
                ReturnStatement s = ParseReturnStatement();
                if (s is not null)
                    return s;
            }
            else if (token.Type == TokenType.Continue)
            {
                _reader.Retr();
                ContinueStatement s = ParseContinueStatement();
                if (s is not null)
                    return s;
            }
            else if (token.Type == TokenType.Break)
            {
                _reader.Retr();
                BreakStatement s = ParseBreakStatement();
                if (s is not null)
                    return s;
            }
            else if (token.Type == TokenType.Scopebreak)
            {
                _reader.Retr();
                ScopebreakStatement s = ParseScopebreakStatement();
                if (s is not null)
                    return s;
            }
            else if (token.Type == TokenType.NvCall)
            {
                _reader.Retr();
                NvCallStatement s = ParseNvCallStatement();
                if (s is not null)
                    return s;
            }
            else if (token.Type == TokenType.Error)
            {
                _reader.Retr();
                ErrorStatement s = ParseErrorStatement();
                if (s is not null)
                    return s;
            }
            else if (token.Type == TokenType.OpenBrace)
            {
                _reader.Retr();
                StatementBlock s = ParseStatementBlock();
                if (s is not null)
                    return s;
            }
            else if (token.Type == TokenType.Ident)
            {
                _reader.Retr();
                Statement s = ParseIdentStartingStatement();
                if (s is not null)
                    return s;
            }
        }

        return null;
    }

    private Statement ParseIdentStartingStatement()
    {
        Expression exp = ParseExpression();
        if (exp is not null)
        {
            if (exp is FuncExpression fexp)
            {
                // func
                if (EnsureNext(TokenType.Semicolon))
                    return null;
                _reader.Read(); // ';'
                return new FuncStatement(fexp);
            }
            else if (exp is IncrExpression incr)
            {
                if (EnsureNext(TokenType.Semicolon))
                    return null;
                _reader.Read();  // ';'
                return new IncrStatement(incr);
            }
            else if (exp is DecrExpression decr)
            {
                if (EnsureNext(TokenType.Semicolon))
                    return null;
                _reader.Read();  // ';'
                return new DecrStatement(decr);
            }
            else
            {
                // assign
                if (!EnsureNext(TokenType.Assign, TokenType.AddAssign, TokenType.SubAssign, TokenType.MultAssign, TokenType.DivAssign, TokenType.ModAssign))
                {
                    TokenType assignType = _reader.Read().Type; // '=', '+=', '-=', '*=', '/=', '%='
                    Token valueToken = _reader.Read();
                    _reader.Retr();
                    Expression value = ParseExpression();
                    if (!EnsureNext(TokenType.Semicolon))
                    {
                        _reader.Read(); // ';'

                        if (assignType == TokenType.AddAssign)
                            value = new CalcExpression(Operator.Add, exp, value);
                        else if (assignType == TokenType.SubAssign)
                            value = new CalcExpression(Operator.Sub, exp, value);
                        else if (assignType == TokenType.MultAssign)
                            value = new CalcExpression(Operator.Mult, exp, value);
                        else if (assignType == TokenType.DivAssign)
                            value = new CalcExpression(Operator.Div, exp, value);
                        else if (assignType == TokenType.ModAssign)
                            value = new CalcExpression(Operator.Mod, exp, value);

                        return new AssignStatement(exp, value, valueToken);
                    }
                }
            }
        }

        return null;
    }

    private IfStatement ParseIfStatement()
    {
        _reader.Read(); // 'if'

        Expression condition = ParseExpression();

        StatementBlock block = ParseStatementBlock(allowOneLiner: true);
        if (block is null)
            return null;
        
        if (_reader.MatchNext(TokenType.Else))
        {
            _reader.Read();


            StatementBlock elseBlock = ParseStatementBlock(allowOneLiner: true);
            if (elseBlock is null)
                return null;

            return new IfStatement(condition, block, elseBlock);
        }
        else
        {
            return new IfStatement(condition, block);
        }
    }

    private WhileStatement ParseWhileStatement()
    {
        _reader.Read(); // 'while'

        Expression condition = ParseExpression();

        StatementBlock body = ParseStatementBlock(inloop: true);

        if (body is null)
            return null;

        return new WhileStatement(condition, body);
    }

    private ForStatement ParseForStatement()
    {
        _reader.Read(); // 'for'

        if (EnsureNext(TokenType.Ident))
            return null;

        Token varNameToken = _reader.Read();

        if (EnsureNext(TokenType.Comma))
            return null;

        _reader.Read(); // ','

        Expression from = ParseExpression();

        if (EnsureNext(TokenType.To))
            return null;

        _reader.Read(); // 'to'

        Expression to = ParseExpression();

        StatementBlock body = ParseStatementBlock(inloop: true);

        return new ForStatement(varNameToken.Value, from, to, body);
    }

    private ReturnStatement ParseReturnStatement()
    {
        Token token = _reader.Read(); // 'return'

        if (_reader.MatchNext(TokenType.Semicolon))
        {
            _reader.Read(); // ';'
            return new ReturnStatement(null, token);
        }
        else
        {
            Token valueToken = _reader.Read();
            _reader.Retr();
            Expression value = ParseExpression();
            return new ReturnStatement(value, valueToken);
        }
    }

    private ContinueStatement ParseContinueStatement()
    {
        Token token = _reader.Read(); // 'continue'

        if (EnsureNext(TokenType.Semicolon))
            return null;

        return new ContinueStatement(token);
    }

    private BreakStatement ParseBreakStatement()
    {
        Token token = _reader.Read(); // 'break'

        if (EnsureNext(TokenType.Semicolon))
            return null;

        return new BreakStatement(token);
    }

    private ScopebreakStatement ParseScopebreakStatement()
    {
        Token token = _reader.Read(); // 'scopebreak'

        if (EnsureNext(TokenType.Semicolon))
        {
            return null;
        }

        return new ScopebreakStatement(token);
    }

    private NvCallStatement ParseNvCallStatement()
    {
        Token token = _reader.Read();  // 'nvcall'

        if (EnsureNext(TokenType.StringLiteral))
            return null;

        Token asmToken = _reader.Read();

        if (EnsureNext(TokenType.StringLiteral))
            return null;

        Token clsToken = _reader.Read();

        if (EnsureNext(TokenType.StringLiteral))
            return null;

        Token symbolToken = _reader.Read();

        List<Expression> args = ParseFunctionArgBody();
        if (args is null)
            return null;

        if (EnsureNext(TokenType.Semicolon))
            return null;

        return new NvCallStatement(asmToken.Value, clsToken.Value, symbolToken.Value, args, token);
    }

    private ErrorStatement ParseErrorStatement()
    {
        Token token = _reader.Read(); // 'err'

        Expression errorCode = ParseExpression();
        Expression errorMessage = ParseExpression();

        if (EnsureNext(TokenType.Semicolon))
            return null;

        return new ErrorStatement(errorCode, errorMessage, token);
    }
}