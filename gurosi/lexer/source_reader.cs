namespace Gurosi;

sealed class SourceReader {
    private char[] _data;
    private int _position;

    public bool EOF => _position >= _data.Length;
    public int Length => _data.Length;
    public char[] DataSource => _data;
    public int Position => _position;

    public SourceReader(string source) : this(source.ToCharArray())
    {
    }

    public SourceReader(char[] source)
    {
        _data = source;
        _position = 0;
    }

    private void ThrowEOF()
    {
        if (EOF)
        {
            throw new Exception("Tried to read beyond the end of the file.");
        }
    }

    public char Read()
    {
        ThrowEOF();
        return _data[_position++];
    }

    public bool MatchNext(char c)
    {
        ThrowEOF();
        return _data[_position] == c;
    }

    public bool MatchNext(Predicate<char> predicate)
    {
        if (EOF) return false;
        return predicate(_data[_position]);
    }

    public string Take(int length)
    {
        ThrowEOF();
        if (_position + length >= _data.Length)
        {
            throw new Exception("Can't take string beyond the end of the file.");
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            builder.Append(Read());
        }

        return builder.ToString();
    }

    public string TakeWhile(Predicate<char> predicate)
    {
        ThrowEOF();

        StringBuilder builder = new StringBuilder();
        while (MatchNext(predicate))
        {
            builder.Append(Read());
        }

        return builder.ToString();
    }

    public void Seek(int position)
    {
        if (position < 0 || position >= Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }
        _position = position;
    }

    public void Retr()
    {
        if (_position > 0)
            _position--;
    }
}