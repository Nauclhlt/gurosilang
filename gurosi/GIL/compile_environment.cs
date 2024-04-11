namespace Gurosi;

public sealed class CompileEnvironment
{
    private List<string> _code;
    private List<Error> _errors;

    public List<string> Code => _code;
    public List<Error> Errors => _errors;
    
    public CompileEnvironment()
    {
        _code = new List<string>();
        _errors = new List<Error>();
    }
}