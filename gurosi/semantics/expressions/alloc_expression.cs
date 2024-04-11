namespace Gurosi;

sealed class AllocExpression : Expression {
    private TypeData _arrayType;
    private Expression _length;

    public TypeData ArrayType => _arrayType;
    public Expression Length => _length;

    public AllocExpression(TypeData arrayType, Expression length, Token token) {
        _token = token;
        _arrayType = arrayType;
        _length = length;
    }

    internal override string _PrintDebug()
    {
        return $"{_arrayType.ToString()}[{_length._PrintDebug()}]";
    }
}