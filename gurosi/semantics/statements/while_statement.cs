namespace Gurosi;

sealed class WhileStatement : Statement {
    private Expression _condition;
    private StatementBlock _body;

    public Expression Condition => _condition;
    public StatementBlock Body => _body;

    public WhileStatement(Expression condition, StatementBlock body)
    {
        _condition = condition;
        _body = body;
    }

    internal override string _PrintDebug()
    {
        return $"while condition: {_condition._PrintDebug()}" + Environment.NewLine +
        "body:" + Environment.NewLine + _body._PrintDebug();
    }
}