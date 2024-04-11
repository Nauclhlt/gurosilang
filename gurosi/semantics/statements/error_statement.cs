namespace Gurosi;

public sealed class ErrorStatement : Statement
{
    private Token _token;
    private Expression _errorCode;
    private Expression _errorMessage;

    public Token Token => _token;
    public Expression ErrorCode => _errorCode;
    public Expression ErrorMessage => _errorMessage;

    public ErrorStatement(Expression errorCode, Expression errorMessage, Token token)
    {
        _errorCode = errorCode;
        _errorMessage = errorMessage;
        _token = token;
    }
}