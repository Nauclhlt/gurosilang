namespace Gurosi;

public sealed class ImplModel
{
    private string _name;
    private List<AccessIdentifier> _accessIdentifiers;
    private TypeData _returnType;
    private List<DefArgument> _parameters;
    private Token _token;

    public string Name
    {
        get => _name;
        set => _name = value;
    }
    public List<AccessIdentifier> AccessIdentifiers => _accessIdentifiers;
    public TypeData ReturnType => _returnType;
    public List<DefArgument> Parameters => _parameters;
    public Token Token => _token;

    public ImplModel(string name, TypeData returnType, List<DefArgument> parameters, List<AccessIdentifier> identifiers, Token token)
    {
        _name = name;
        _returnType = returnType;
        _parameters = parameters;
        _accessIdentifiers = identifiers;
        _token = token;
    }
}