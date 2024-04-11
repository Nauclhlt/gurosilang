namespace Gurosi;

public sealed class Semantic
{
    private List<Error> _errors;
    private SemanticCode _code;

    public IEnumerable<Error> Errors => _errors;
    public SemanticCode Code => _code;

    private Semantic()
    {
        
    }

    internal Semantic(List<Error> errors, SemanticCode code = null)
    {
        _errors = errors;
        _code = code;
    }
}