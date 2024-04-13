namespace Gurosi;

public sealed class Error
{
    private string _message;
    private TextPoint _point;
    private string _filename;

    public string Message => _message;
    public TextPoint Point => _point;
    public string FileName
    {
        get => _filename;
        internal set => _filename = value;
    }

    public string GetFormatted()
    {
        return $"({_point.Line}, {_point.Column}) {_message}  ({_filename})";
    }

    public static void AttachFileName(string filename, IEnumerable<Error> errors)
    {
        foreach (Error item in errors)
        {
            item.FileName = filename;
        }
    }

    public Error(string message, TextPoint point)
    {
        _message = message;
        _point = point;
    }
}