namespace Gurosi;

public sealed class NewExpression : Expression
{
    private TypeData _type;
    private List<Expression> _arguments;

    public TypeData Type => _type;
    public List<Expression> Arguments => _arguments;

    public NewExpression(TypeData type, List<Expression> arguments, Token token)
    {
        _token = token;
        _type = type;
        _arguments = arguments;
    }
}