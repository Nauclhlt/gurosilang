namespace Gurosi;

public sealed class NullExpression : Expression
{
    public NullExpression(Token token)
    {
        _token = token;
    }
}