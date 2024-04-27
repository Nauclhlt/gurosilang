namespace Gurosi;

public static class ParserUtil
{
    private static readonly Dictionary<string, AttributeModel> AttributeMap = new()
    {
        { "indexer_get", AttributeModel.IndexerGet },
        { "indexer_set", AttributeModel.IndexerSet },
    };

    public static bool IsValidAttribute(string attribute)
    {
        return AttributeMap.ContainsKey(attribute);
    }

    public static AttributeModel ConvertAttributes(List<Token> list)
    {
        AttributeModel attrib = 0;

        for (int i = 0; i < list.Count; i++)
        {
            attrib |= AttributeMap[list[i].Value];
        }

        return attrib;
    }
}