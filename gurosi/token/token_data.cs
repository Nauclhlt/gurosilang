namespace Gurosi;

public static class TokenData {
    private static readonly Dictionary<string, TokenType> Keywords = new(){
        { "mine", TokenType.Mine },
        { "module", TokenType.Module },
        { "run", TokenType.Run },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "for", TokenType.For },
        { "while", TokenType.While },
        { "how", TokenType.How },
        { "string", TokenType.String },
        { "int", TokenType.Int },
        { "float", TokenType.Float },
        { "double", TokenType.Double },
        { "boolean", TokenType.Boolean },
        { "any", TokenType.Any },
        { "return", TokenType.Return },
        { "err", TokenType.Error },
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "let", TokenType.Let },
        { "to", TokenType.To },
        { "continue", TokenType.Continue },
        { "break", TokenType.Break },
        { "scopebreak", TokenType.Scopebreak },
        { "void", TokenType.Void },
        { "class", TokenType.Class },
        { "field", TokenType.Field },
        { "public", TokenType.Public },
        { "private", TokenType.Private },
        { "moduled", TokenType.Moduled },
        { "static", TokenType.Static },
        { "array", TokenType.Array },
        { "alloc", TokenType.Alloc },
        { "init", TokenType.Init },
        { "new", TokenType.New },
        { "shorten", TokenType.Shorten },
        { "nvcall", TokenType.NvCall },
        { "nvretv", TokenType.NvRetv },
        { "macro", TokenType.Macro },
        { "extend", TokenType.Extend },
        { "arrbase", TokenType.Arrbase },
        { "self", TokenType.Self },
        { "const", TokenType.Const },
        { "null", TokenType.Null },
        { "as", TokenType.As },
        { "funcptr", TokenType.FuncPtr }
    };

    private static readonly HashSet<char> IdentChars = new()
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '_'
    };

    private static HashSet<char> NumberChars = new()
    {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    public static bool IsKeyword(string word)
    {
        return Keywords.ContainsKey(word);
    }

    public static TokenType GetKeywordTokenType(string keyword)
    {
        if (!IsKeyword(keyword))
        {
            throw new ArgumentException(nameof(keyword));
        }

        return Keywords[keyword];
    }

    public static bool IsNumber(char c)
    {
        return NumberChars.Contains(c);
    }

    public static bool IsIdent(char c)
    {
        return IdentChars.Contains(c);
    }
}