namespace Gurosi;

public sealed class NvCallStatement : Statement
{
    private Token _token;
    private string _asm;
    private string _cls;
    private string _symbol;
    private List<Expression> _arguments;

    public Token Token => _token;
    public string Asm => _asm;
    public string Cls => _cls;
    public string Symbol => _symbol;
    public List<Expression> Arguments => _arguments;

    public NvCallStatement(string asm, string cls, string symbol, List<Expression> args, Token token)
    {
        _asm = asm;
        _cls = cls;
        _symbol = symbol;
        _arguments = args;
        _token = token;
    }
}