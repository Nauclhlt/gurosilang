namespace Gurosi;

sealed class ReturnStatement : Statement {
    private Expression _value;
    private Token _valueToken;

    public Expression Value => _value;
    public Token ValueToken => _valueToken;

    public ReturnStatement(Expression value, Token valueToken)
    {
        _value = value;
        _valueToken = valueToken;
    }

    internal override string _PrintDebug()
    {
        if (_value is null)
            return "return";
        else
            return $"return value={_value._PrintDebug()}";
    }
}