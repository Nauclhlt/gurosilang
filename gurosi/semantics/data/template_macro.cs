namespace Gurosi;

public sealed class TemplateMacro
{
    private List<Token> _tokens = new List<Token>();
    private string _name;
    private int _templateCount;

    public List<Token> Tokens => _tokens;
    public string Name => _name;
    public int TemplateCount => _templateCount;

    public TemplateMacro(string name, List<Token> tokens, int templateCount)
    {
        _name = name;
        _tokens = tokens;
        _templateCount = templateCount;
    }
}