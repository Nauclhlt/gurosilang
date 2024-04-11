namespace Gurosi;

sealed class ContinueStatement : Statement {
    private Token _token;

    public Token Token => _token;

    public ContinueStatement(Token token)
    {
        _token = token;
    }

    internal override string _PrintDebug()
    {
        return "continue";
    }
}