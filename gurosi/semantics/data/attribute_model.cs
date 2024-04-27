namespace Gurosi;

[Flags]
public enum AttributeModel
{
    IndexerGet = 1 << 0,
    IndexerSet = 1 << 1,
}

public static class AttributeModelExtensions
{
    public static bool Has(this AttributeModel self, AttributeModel attribute)
    {
        return (self & attribute) != 0;
    }
}