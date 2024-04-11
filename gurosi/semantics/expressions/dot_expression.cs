namespace Gurosi;

sealed class DotExpression : Expression {
    private Expression _source;
    private string _right;

    public Expression Source => _source;
    public string Right => _right;

    public DotExpression(Expression source, string right, Token token)
    {
        _token = token;
        _source = source;
        _right = right;
    }

    internal override string _PrintDebug()
    {
        return $"{_source._PrintDebug()}->{_right}";
    }

    internal override string GetPath()
    {
        return $"{_source.GetPath()};{_right}";
    }
}