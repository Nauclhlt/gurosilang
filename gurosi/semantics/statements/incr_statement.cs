namespace Gurosi;

public sealed class IncrStatement : Statement
{
    private IncrExpression _inner;

    public IncrExpression Inner => _inner;
    
    public IncrStatement(IncrExpression inner)
    {
        _inner = inner;
    }
}