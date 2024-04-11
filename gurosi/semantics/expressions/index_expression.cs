namespace Gurosi;

sealed class IndexExpression : Expression {
    private Expression _target;
    private Expression _index;

    public Expression Target => _target;
    public Expression Index => _index;

    public IndexExpression(Expression target, Expression index)
    {
        _target = target;
        _index = index;
        _token = target.Token;
    }

    internal override string _PrintDebug()
    {
        return $"{_target._PrintDebug()}[{_index._PrintDebug()}]";
    }
}