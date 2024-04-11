using System.Collections.Immutable;

namespace Gurosi;

public sealed class Lexical {
    private List<Token> _tokens;

    private Lexical()
    {
        _tokens = new();
    }

    internal Lexical(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public ImmutableList<Token> GetTokens()
    {
        return _tokens.ToImmutableList();
    }

    internal List<Token> GetList() => _tokens;

    public void _PrintDebug()
    {
        for (int i = 0; i < _tokens.Count; i++)
        {
            Console.WriteLine($"'{_tokens[i].Value}': {_tokens[i].Type} at ({_tokens[i].Point.Line}, {_tokens[i].Point.Column})");
        }
    }
}