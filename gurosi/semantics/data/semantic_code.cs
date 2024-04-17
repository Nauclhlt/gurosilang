using System.Collections.Immutable;

namespace Gurosi;

public sealed class SemanticCode
{
    private string _fileName;
    private StatementBlock _runBlock;
    private List<string> _imports;
    private List<string> _shortens;
    private string _moduleName;
    private List<Function> _globalFunctions;
    private List<ClassModel> _classes;
    private int _memSize;

    public string FileName
    {
        get => _fileName;
        set => _fileName = value;
    }

    internal StatementBlock RunBlock
    {
        get => _runBlock;
        init => _runBlock = value;
    }

    internal List<string> Imports
    {
        get => _imports;
        init => _imports = value;
    }

    internal List<string> Shortens
    {
        get => _shortens;
        set => _shortens = value;
    }

    public string ModuleName
    {
        get => _moduleName;
        init => _moduleName = value;
    }

    internal List<Function> GlobalFunctions
    {
        get => _globalFunctions;
        init => _globalFunctions = value;
    }

    internal List<ClassModel> Classes
    {
        get => _classes;
        init => _classes = value;
    }

    internal int MemSize
    {
        get => _memSize;
        set => _memSize = value;
    }

    internal bool MemSizeSpecified => _memSize != -1;

    public void _PrintDebug()
    {
        // make tests
        if (_runBlock is not null)
        {
            for (int i = 0; i < _runBlock.Statements.Count; i++)
            {
                Console.WriteLine(_runBlock.Statements[i].GetType().Name);
            }
        }
    }
}