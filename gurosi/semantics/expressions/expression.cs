namespace Gurosi;

public abstract class Expression {
    protected Token _token;

    public Token Token => _token;
    internal virtual string _PrintDebug() => "";
    internal virtual string GetPath() => "~~~";
}