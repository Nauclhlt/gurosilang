namespace Gurosi;

public sealed class DummyExpression : Expression
{
    private TypePath _type;

    public TypePath Type => _type;

    public DummyExpression(TypePath type)
    {
        _type = type;
    }

    public DummyExpression(TypeData type)
    {
        _type = TypePath.FromModel(type);
    }
}