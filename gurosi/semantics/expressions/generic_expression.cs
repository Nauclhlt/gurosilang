namespace Gurosi;

public sealed class GenericExpression : Expression
{
    private Expression _source;
    private List<TypeData> _types;

    public Expression Source => _source;
    public List<TypeData> Types => _types;

    public GenericExpression(Expression source, List<TypeData> types, Token token)
    {
        _source = source;
        _types = types;
        _token = token;
    }
}