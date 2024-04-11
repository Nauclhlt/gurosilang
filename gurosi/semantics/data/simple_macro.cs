namespace Gurosi;

public sealed class SimpleMacro
{
    private string _name;
    private List<Token> _tokens;

    public List<Token> Tokens => _tokens;
    public string Name => _name;

    public SimpleMacro(string name, List<Token> tokens)
    {
        _name = name;
        _tokens = tokens;
    }
}