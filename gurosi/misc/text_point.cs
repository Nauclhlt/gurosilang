namespace Gurosi;

public readonly struct TextPoint
{
    public readonly int Line;
    public readonly int Column;

    public TextPoint(int line, int column)
    {
        Line = line;
        Column = column;
    }
}