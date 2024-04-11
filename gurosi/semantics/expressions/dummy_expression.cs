namespace Gurosi;

public sealed class DummyExpression : Expression
{
    private TypeData _type;

    public TypeData Type => _type;

    public DummyExpression(TypeData type)
    {
        _type = type;
    }
}