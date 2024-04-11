namespace Gurosi;

public sealed class ClassModel {
    private string _module;
    private string _name;
    private List<AccessIdentifier> _identifiers;
    private List<FieldModel> _fields;
    private List<FieldModel> _stcFields;
    private List<MethodModel> _methods;
    private List<MethodModel> _stcMethods;
    private int _genericCount = 0;
    private TypeData _baseType;

    public string Module
    {
        get => _module;
        init => _module = value;
    }

    public string Name
    {
        get => _name;
        init => _name = value;
    }

    public List<FieldModel> Fields
    {
        get => _fields;
        init => _fields = value;
    }

    public List<FieldModel> StcFields
    {
        get => _stcFields;
        init => _stcFields = value;
    }

    public List<MethodModel> Methods
    {
        get => _methods;
        init => _methods = value;
    }

    public List<MethodModel> StcMethods
    {
        get => _stcMethods;
        init => _stcMethods = value;
    }

    public List<AccessIdentifier> Identifiers
    {
        get => _identifiers;
        init => _identifiers = value;
    }

    public int GenericCount
    {
        get => _genericCount;
        set => _genericCount = value;
    }

    public TypeData BaseType
    {
        get => _baseType;
        set => _baseType = value;
    }
}