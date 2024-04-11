namespace Gurosi;

public sealed class NvRetvExpression : Expression
{
    private TypeData _type;

    public TypeData Type => _type;

    public NvRetvExpression(TypeData type, Token token)
    {
        _type = type;
        _token = token;
    }
}