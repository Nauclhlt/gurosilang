namespace Gurosi;

public sealed class ObjectCastExpression : Expression
{
    private TypeData _targetType;
    private Expression _source;

    public TypeData TargetType => _targetType;
    public Expression Source => _source;

    public ObjectCastExpression(Expression source, TypeData targetType, Token token)
    {
        _source = source;
        _targetType = targetType;
        _token = token;
    }
}