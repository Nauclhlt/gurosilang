namespace Gurosi;

public sealed class Function {
    private string _module;
    private string _name;
    private TypeData _returnType;
    private List<DefArgument> _parameters;
    private TypeData _extendType;
    private string _extendName;
    private StatementBlock _body;
    private Token _token;

    public string Module => _module;
    public string Name
    {
        get => _name;
        set => _name = value;
    }
    public TypeData ReturnType => _returnType;
    public List<DefArgument> Parameters => _parameters;
    public TypeData ExtendType => _extendType;
    public string ExtendName => _extendName;
    public StatementBlock Body => _body;
    public Token Token => _token;

    public Function(string module, string name, TypeData returnType, List<DefArgument> parameters, StatementBlock body, Token token)
    : this(module, name, returnType, parameters, null, null, body, token)
    {   
    }

    public Function(string module, string name, TypeData returnType, List<DefArgument> parameters, TypeData extendType, string extendName, StatementBlock body, Token token)
    {
        _module = module;
        _name = name;
        _returnType = returnType;
        _parameters = parameters;
        _body = body;
        _token = token;
        _extendType = extendType;
        _extendName = extendName;
    }
}