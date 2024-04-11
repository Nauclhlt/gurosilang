namespace Gurosi;

sealed class AssignStatement : Statement {
    private Expression _target;
    private Expression _value;
    private Token _valueToken;

    public Expression Target => _target;
    public Expression Value => _value;
    public Token ValueToken => _valueToken;

    public AssignStatement(Expression target, Expression value, Token valueToken)
    {
        _target = target;
        _value = value;
        _valueToken = valueToken;
    }

    internal override string _PrintDebug()
    {
        return $"assign target={_target._PrintDebug()}  value={_value._PrintDebug()}";
    }
}