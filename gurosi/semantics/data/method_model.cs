namespace Gurosi;

public sealed class MethodModel {
    private string _name;
    private List<AccessIdentifier> _accessIdentifiers;
    private TypeData _returnType;
    private List<DefArgument> _parameters;
    private StatementBlock _body;
    private Token _token;

    public string Name
    {
        get => _name;
        set => _name = value;
    }
    public List<AccessIdentifier> AccessIdentifiers => _accessIdentifiers;
    public TypeData ReturnType => _returnType;
    public List<DefArgument> Parameters => _parameters;
    public StatementBlock Body => _body;
    public Token Token => _token;

    public MethodModel(string name, TypeData returnType, List<DefArgument> parameters, StatementBlock body, List<AccessIdentifier> identifiers, Token token)
    {
        _name = name;
        _returnType = returnType;
        _parameters = parameters;
        _body = body;
        _accessIdentifiers = identifiers;
        _token = token;
    }
}