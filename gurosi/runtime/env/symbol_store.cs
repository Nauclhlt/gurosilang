namespace Gurosi;

public sealed class SymbolStore
{
    private List<ClassBinary> _classes;
    private List<FunctionBinary> _globalFunctions;
    private Dictionary<string, ClassBinary> _classMap;
    private Dictionary<string, FunctionBinary> _globalFunctionMap;

    public List<ClassBinary> Classes => _classes;
    public List<FunctionBinary> GlobalFunctions => _globalFunctions;

    private SymbolStore()
    {
        _classes = new List<ClassBinary>();
        _globalFunctions = new List<FunctionBinary>();
        _classMap = new Dictionary<string, ClassBinary>();
        _globalFunctionMap = new Dictionary<string, FunctionBinary>();
    }

    public ClassBinary FindClass(TypePath type)
    {
        string exp = type.WithoutGenerics().ToString();
        if (_classMap.TryGetValue(exp, out ClassBinary cls))
        {
            return cls;
        }
        else
        {
            return null;
        }
    }

    public FunctionBinary FindGlobalFunction(string module, string name)
    {
        string query = module + "::" + name;
        if (_globalFunctionMap.TryGetValue(query, out FunctionBinary func))
        {
            return func;
        }
        else
        {
            return null;
        }
    }

    public static SymbolStore FromLibraries(IEnumerable<Library> libraries)
    {
        SymbolStore instance = new SymbolStore();

        foreach (Library lib in libraries)
        {
            instance._classes.AddRange(lib.Classes);
            instance._globalFunctions.AddRange(lib.Functions);

            for (int i = 0; i < lib.Classes.Count; i++)
            {
                instance._classMap.TryAdd(lib.Classes[i].Path.ToString(), lib.Classes[i]);
            }

            for (int i = 0; i < lib.Functions.Count; i++)
            {
                instance._globalFunctionMap.TryAdd(lib.Functions[i].Module + "::" + lib.Functions[i].Name, lib.Functions[i]);
            }
        }

        return instance;
    }
}