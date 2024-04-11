namespace Gurosi;

sealed class CalcExpression : Expression {
    private Operator _operator;
    private Expression _left;
    private Expression _right;

    public Operator Operator => _operator;
    public Expression Left => _left;
    public Expression Right => _right;

    public CalcExpression(Operator @operator, Expression left, Expression right)
    {
        _left = left;
        _right = right;
        _operator = @operator;
        this._token = _left.Token;
    }

    internal override string _PrintDebug()
    {
        return $"{_operator} [ {_left._PrintDebug()}, {_right._PrintDebug()} ]";
    }
}