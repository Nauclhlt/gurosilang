namespace Gurosi;

public sealed class DecrStatement : Statement
{
    private DecrExpression _inner;

    public DecrExpression Inner => _inner;

    public DecrStatement(DecrExpression inner)
    {
        _inner = inner;
    }
}