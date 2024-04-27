namespace Gurosi;

public sealed class FieldModel {
    private string _name;
    private List<AccessIdentifier> _identifiers;
    private Expression _initialValue;
    private TypeData _type;
    private Token _token;
    private AttributeModel _attributes;

    public string Name => _name;
    public List<AccessIdentifier> Identifiers => _identifiers;
    public Expression InitialValue => _initialValue;
    public TypeData Type => _type;
    public Token Token => _token;
    public AttributeModel Attributes
    {
        get => _attributes;
        set => _attributes = value;
    }

    public FieldModel(string name, List<AccessIdentifier> identifiers, TypeData type, Token token)
    {
        _name = name;
        _identifiers = identifiers;
        _type = type;
        _initialValue = null;
        _token = token;
    }

    public FieldModel(string name, List<AccessIdentifier> identifiers, TypeData type, Expression initialValue, Token token)
    {
        _name = name;
        _identifiers = identifiers;
        _type = type;
        _initialValue = initialValue;
        _token = token;
    }
}