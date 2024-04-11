namespace Gurosi;

sealed class IdentExpression : Expression {
    private string _identifier;

    public string Identifier => _identifier;

    public IdentExpression(string identifier, Token token)
    {
        _identifier = identifier;
        _token = token;
    }

    internal override string _PrintDebug()
    {
        return _identifier;
    }

    internal override string GetPath()
    {
        return _identifier;
    }
}