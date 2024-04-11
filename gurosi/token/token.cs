namespace Gurosi;

public sealed class Token {

    public static readonly Token Empty = new Token(string.Empty, TokenType.Ident, default);

    private string _value;
    private TokenType _type;
    private TextPoint _point;

    public string Value => _value;
    public TokenType Type => _type;
    public TextPoint Point => _point;

    public Token(string value, TokenType type, TextPoint point)
    {
        _value = value;
        _type = type;
        _point = point;
    }

    public override bool Equals(object obj)
    {
        if (obj is not Token || obj is null)
            return false;

        Token token = obj as Token;
        return Value == token.Value && Type == token.Type;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}