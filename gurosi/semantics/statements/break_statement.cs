namespace Gurosi;

sealed class BreakStatement : Statement {
    private Token _token;

    public Token Token => _token;

    public BreakStatement(Token token)
    {
        _token = token;
    }

    internal override string _PrintDebug()
    {
        return "break";
    }
}