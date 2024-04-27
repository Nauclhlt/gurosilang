namespace Gurosi;

public static class ParserValidator
{
    public static bool ValidateFieldAttributes(List<Token> attributes, out Error error)
    {
        error = null;
        if (attributes.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < attributes.Count; i++)
        {
            error = new Error(ErrorProvider.Parser.InvalidAttribute(attributes[i].Value, "field"), attributes[i].Point);
            return true;
        }

        return false;
    }

    public static bool ValidateStaticMethodAttributes(List<Token> attributes, out Error error)
    {
        error = null;
        if (attributes.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < attributes.Count; i++)
        {
            error = new Error(ErrorProvider.Parser.InvalidAttribute(attributes[i].Value, "static method"), attributes[i].Point);
            return true;
        }

        return false;
    }

    static readonly HashSet<string> MethodAttribs = new HashSet<string>()
    {
        "indexer_get", "indexer_set", "inline"
    };

    public static bool ValidateMethodAttributes(List<Token> attributes, out Error error)
    {
        error = null;
        if (attributes.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < attributes.Count; i++)
        {
            if (!MethodAttribs.Contains(attributes[i].Value))
            {
                error = new Error(ErrorProvider.Parser.InvalidAttribute(attributes[i].Value, "method"), attributes[i].Point);
                return true;
            }
        }

        if (attributes.Any(x => x.Value == "indexer_get") &&
            attributes.Any(x => x.Value == "indexer_set"))
        {
            error = new Error(ErrorProvider.Parser.AttributeMismatch("indexer_get", "indexer_set"), attributes.First(x => x.Value == "indexer_get").Point);
            return true;
        }

        return false;
    }

    public static bool ValidateIndexerGet(MethodModel model, out Error error)
    {
        error = null;

        if (model.Parameters.Count != 1)
        {
            error = new Error(ErrorProvider.Parser.InvalidIndexerMethod(), model.Token.Point);
            return true;
        }

        if (model.ReturnType.Kind == TypeKind.BuiltIn &&
            model.ReturnType.BuiltIn == BuiltinTypes.NULL)
        {
            error = new Error(ErrorProvider.Parser.InvalidIndexerMethod(), model.Token.Point);
            return true;
        }

        if (model.AccessIdentifiers.Contains(AccessIdentifier.Static))
        {
            error = new Error(ErrorProvider.Parser.InvalidIndexerMethod(), model.Token.Point);
            return true;
        }

        return false;
    }

    public static bool ValidateIndexerSet(MethodModel model, out Error error)
    {
        error = null;

        if (model.Parameters.Count != 2)
        {
            error = new Error(ErrorProvider.Parser.InvalidIndexerMethod(), model.Token.Point);
            return true;
        }

        if (model.ReturnType.Kind != TypeKind.BuiltIn ||
            model.ReturnType.BuiltIn != BuiltinTypes.NULL)
        {
            error = new Error(ErrorProvider.Parser.InvalidIndexerMethod(), model.Token.Point);
            return true;
        }

        if (model.AccessIdentifiers.Contains(AccessIdentifier.Static))
        {
            error = new Error(ErrorProvider.Parser.InvalidIndexerMethod(), model.Token.Point);
            return true;
        }

        return false;
    }
}