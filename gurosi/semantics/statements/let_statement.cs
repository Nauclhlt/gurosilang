namespace Gurosi;

sealed class LetStatement : Statement {
    private string _varName;
    private TypeData _type;
    private Expression _value;
    private Token _token;

    public string VarName => _varName;
    public TypeData Type => _type;
    public Expression Value => _value;
    public Token Token => _token;

    public LetStatement(string varName, TypeData type, Token token)
    {
        _varName = varName;
        _value = null;
        _type = type;
        _token = token;
    }

    public LetStatement(string varName, TypeData type, Expression value, Token token)
    {
        _varName = varName;
        _type = type;
        _value = value;
        _token = token;
    }

    internal override string _PrintDebug()
    {
        return $"let name={_varName}  type={_type.ToString()}  value={_value?._PrintDebug()}";
    }
}