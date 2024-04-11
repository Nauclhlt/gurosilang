namespace Gurosi;

sealed class ScopebreakStatement : Statement {
    private Token _token;

    public Token Token => _token;

    public ScopebreakStatement(Token token)
    {
        _token = token;
    }

    internal override string _PrintDebug()
    {
        return "scopebreak";
    }
}