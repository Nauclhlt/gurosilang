namespace Gurosi;

sealed class FuncStatement : Statement {
    private FuncExpression _inner;

    public FuncExpression Inner => _inner;

    public FuncStatement(FuncExpression inner)
    {
        _inner = inner;
    }

    internal override string _PrintDebug()
    {
        return "call inner=" + _inner._PrintDebug();
    }
}