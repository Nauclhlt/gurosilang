namespace Gurosi;

sealed class TokenReader {
    private List<Token> _data;
    private int _position;

    public bool EOF => _position >= _data.Count;
    public int Length => _data.Count;
    public int Position => _position;
    public List<Token> Data => _data;

    public TokenReader(Lexical source) : this(source.GetList())
    {
    }

    internal TokenReader(List<Token> source)
    {
        _data = source;
        _position = 0;
    }

    private void ThrowEOF()
    {
        if (EOF)
        {
            throw new Exception("Tried to read beyond the end of the tokens.");
        }
    }

    public Token Read()
    {
        ThrowEOF();
        return _data[_position++];
    }

    public Token GetCurrent()
    {
        if (EOF) return null;
        return _data[_position];
    }

    public bool MatchNext(Token token)
    {
        if (EOF) return false;
        return _data[_position].Equals(token);
    }

    public bool MatchNext(TokenType type)
    {
        if (EOF) return false;
        return _data[_position].Type == type;
    }

    public bool MatchNext(Predicate<Token> predicate)
    {
        if (EOF) return false;
        return predicate(_data[_position]);
    }

    public string Take(int length)
    {
        ThrowEOF();
        if (_position + length >= _data.Count)
        {
            throw new Exception("Can't take string beyond the end of the tokens.");
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            builder.Append(Read());
        }

        return builder.ToString();
    }

    public List<Token> TakeWhile(Predicate<Token> predicate)
    {
        ThrowEOF();

        List<Token> buffer = new List<Token>();
        while (MatchNext(predicate))
        {
            //builder.Append(Read());
            buffer.Add(Read());
        }

        return buffer;
    }

    public void Seek(int position)
    {
        if (position < 0 || position >= Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }
        _position = position;
    }

    public Token GetOffset(int offset)
    {
        if (_position + offset >= 0 && _position + offset < Length)
        {
            return _data[_position + offset];
        }
        else
        {
            return null;
        }
    }

    public void Retr()
    {
        if (_position > 0)
            _position--;
    }

    public int GetState()
    {
        return _position;
    }
}