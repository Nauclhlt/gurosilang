namespace Gurosi;

sealed class ForStatement : Statement {
    private string _varName;
    private Expression _from;
    private Expression _to;
    private StatementBlock _body;

    public string VarName => _varName;
    public Expression From => _from;
    public Expression To => _to;
    public StatementBlock Body => _body;

    public ForStatement(string varName, Expression from, Expression to, StatementBlock body)
    {
        _varName = varName;
        _from = from;
        _to = to;
        _body = body;
    }

    internal override string _PrintDebug()
    {
        return $"for var_name={_varName}  from={_from._PrintDebug()}  to={_to._PrintDebug()}" + Environment.NewLine +
        "body:" + Environment.NewLine +
        _body._PrintDebug();
    }
}