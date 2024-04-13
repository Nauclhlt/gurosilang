namespace Gurosi;

public sealed class RuntimeError
{
    private string _errorCode;
    private string _message;

    public string ErrorCode => _errorCode;
    public string Message => _message;

    public RuntimeError(string errorCode, string message)
    {
        _errorCode = errorCode;
        _message = message;
    }
}