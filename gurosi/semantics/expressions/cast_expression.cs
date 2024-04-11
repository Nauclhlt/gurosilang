namespace Gurosi;

public sealed class CastExpression : Expression
{
    private TypeData _targetType;
    private Expression _value;

    public TypeData TargetType => _targetType;
    public Expression Value => _value;

    public CastExpression(Expression value, TypeData targetType, Token token)
    {
        _value = value;
        _targetType = targetType;
        _token = token;
    }
}