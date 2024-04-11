namespace Gurosi;

sealed class IfStatement : Statement {
    private Expression _condition;
    private StatementBlock _body;
    private StatementBlock _elseBody;

    public Expression Condition => _condition;
    public StatementBlock Body => _body;
    public StatementBlock ElseBody => _elseBody;

    public IfStatement(Expression condition, StatementBlock body)
    {
        _condition = condition;
        _body = body;
        _elseBody = null;
    }

    public IfStatement(Expression condition, StatementBlock body, StatementBlock elseBody)
    {
        _condition = condition;
        _body = body;
        _elseBody = elseBody;
    }

    internal override string _PrintDebug()
    {
        string s = $"if condition: {_condition._PrintDebug()}" + Environment.NewLine +
        "body: " + Environment.NewLine + _body._PrintDebug();
        if (_elseBody is not null)
        {
            s += Environment.NewLine +
            "else_body: " + Environment.NewLine +
            _elseBody._PrintDebug();
        }

        return s;
    }
}