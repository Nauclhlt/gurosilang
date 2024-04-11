namespace Gurosi;

public sealed class Library
{
    private List<ClassBinary> _classes;
    private List<FunctionBinary> _functions;
    private List<string> _imported;
    private string _fileName;

    public List<ClassBinary> Classes => _classes;
    public List<FunctionBinary> Functions => _functions;
    public string FileName => _fileName;
    public List<string> Imported => _imported;

    public Library()
    {
        _classes = new List<ClassBinary>();
        _functions = new List<FunctionBinary>();
        _imported = new List<string>();
    }

    public void Write(string filename)
    {
        _fileName = filename;

        using FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using BinaryWriter writer = new BinaryWriter(fs, Encoding.Unicode);

        writer.Write(_classes.Count);
        for (int i = 0; i < _classes.Count; i++)
        {
            _classes[i].Write(writer);
        }

        writer.Write(_functions.Count);
        for (int i = 0; i < _functions.Count; i++)
        {
            _functions[i].Write(writer);
        }
    }

    public static Library Load(string filename)
    {
        Library lib = new Library();
        lib._fileName = filename;

        using FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using BinaryReader reader = new BinaryReader(fs, Encoding.Unicode);

        int cc = reader.ReadInt32();
        for (int i = 0; i < cc; i++)
        {
            lib._classes.Add(new ClassBinary());
            lib._classes[i].Read(reader);
        }

        int fc = reader.ReadInt32();
        for (int i = 0; i < fc; i++)
        {
            lib._functions.Add(new FunctionBinary());
            lib._functions[i].Read(reader);
        }

        return lib;
    }

    public void _PrintDebug()
    {
        List<string> code = _classes[0].Functions[0].Body.Code;
        for (int j = 0; j < code.Count; j++)
        {
            if (j != 0 && GIL.OperandMap.ContainsKey(code[j]))
            {
                Console.WriteLine();
            }
            Console.Write(code[j]);
            Console.Write(" ");
        }
    }

    public void AddImports(List<string> imports)
    {
        for (int i = 0; i < imports.Count; i++)
        {
            if (!_imported.Contains(imports[i]))
            {
                _imported.Add(imports[i]);
            }
        }
    }
}