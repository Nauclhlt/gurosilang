namespace Gurosi;

public sealed class IncrExpression : Expression {
    public Expression _target;

    public Expression Target => _target;

    public IncrExpression(Expression target, Token token)
    {
        _token = token;
        _target = target;
    }

    internal override string _PrintDebug()
    {
        return $"-- [ {_target._PrintDebug()} ]";
    }
}