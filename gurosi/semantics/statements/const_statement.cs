namespace Gurosi;

public sealed class ConstStatement : Statement
{
    private Token _token;
    private string _varName;
    private TypeData _type;
    private Expression _value;

    public Token Token => _token;
    public string VarName => _varName;
    public TypeData Type => _type;
    public Expression Value => _value;

    public ConstStatement(string varName, TypeData type, Expression value, Token token)
    {
        _token = token;
        _varName = varName;
        _type = type;
        _value = value;
    }
}