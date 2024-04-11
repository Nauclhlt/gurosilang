namespace Gurosi;

public sealed class DefArgument
{
    private string _name;
    private TypeData _type;
    private Token _token;

    public string Name => _name;
    public TypeData Type => _type;
    public Token Token => _token;

    public DefArgument(string name, TypeData type, Token token)
    {
        _name = name;
        _type = type;
        _token = token;
    }
}