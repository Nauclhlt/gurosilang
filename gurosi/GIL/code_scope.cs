namespace Gurosi;

public sealed class CodeScope
{
    private bool _isLoopScope = false;
    private Dictionary<string, Local> _symbolTable = new Dictionary<string, Local>();

    public Dictionary<string, Local> SymbolTable => _symbolTable;
    public bool IsLoopScope => _isLoopScope;

    public CodeScope(bool isLoopScope)
    {
        _isLoopScope = isLoopScope;
    }
}