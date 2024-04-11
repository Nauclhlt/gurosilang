namespace Gurosi;

public sealed class SelfExpression : Expression
{
    public SelfExpression(Token token)
    {
        _token = token;
    }
}