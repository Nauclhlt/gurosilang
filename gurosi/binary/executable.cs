namespace Gurosi;

public sealed class Executable
{
    private CodeBinary _entryCode;
    private List<string> _allImports;
    private Library _selfLibrary;
    private int _memorySize = 32;

    public CodeBinary EntryCode => _entryCode;
    public List<string> AllImports => _allImports;
    public Library SelfLibrary => _selfLibrary;
    
    public int MemorySize
    {
        get => _memorySize;
        set => _memorySize = value;
    }

    private Executable()
    {
        _allImports = new List<string>();
        _entryCode = new CodeBinary();
    }

    public Executable(CodeBinary code, List<string> allImports, Library selfLibrary)
    {
        _entryCode = code;
        _allImports = allImports.Distinct().ToList();
        _selfLibrary = selfLibrary;
    }

    public void Write(string filename)
    {
        using FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using BinaryWriter writer = new BinaryWriter(fs, Encoding.Unicode);

        writer.Write(_allImports.Count);
        for (int i = 0; i < _allImports.Count; i++)
        {
            writer.Write(_allImports[i]);
        }

        _selfLibrary.Write(writer);
        _entryCode.Write(writer);
        writer.Write(_memorySize);
    }

    public static Executable Load(string filename)
    {
        Executable e = new Executable();

        using FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        using BinaryReader reader = new BinaryReader(fs, Encoding.Unicode);

        int c = reader.ReadInt32();
        for (int i = 0; i < c; i++)
        {
            e._allImports.Add(reader.ReadString());
        }

        e._selfLibrary = Library.Load(filename, reader);
        e._entryCode.Read(reader);
        e._memorySize = reader.ReadInt32();

        return e;
    }

    public void _PrintDebug()
    {
        for (int i = 0; i < _entryCode.Code.Count; i++)
        {
            if (GIL.OperandMap.ContainsKey(_entryCode.Code[i]) && i != 0)
                Console.WriteLine();

            Console.Write(_entryCode.Code[i]);
            Console.Write("  ");
        }
        Console.WriteLine();
        


        // ClassBinary first = _selfLibrary.Classes[0];
        // for (int i = 0; i < first.Functions.Count; i++)
        // {
        //     FunctionBinary func = first.Functions[i];
        //     Console.WriteLine("name:  " + first.Functions[i].Name);
        //     Console.WriteLine("arg_list:");
        //     for (int j = 0; j < func.Arguments.Count; j++)
        //     {
        //         Console.WriteLine(func.Arguments[j].Name + "\t" + func.Arguments[j].Type.ToString());
        //     }
        //     Console.WriteLine("identifiers:  " + string.Join(",", func.Identifiers.Select(x => x.ToString())));
        //     Console.WriteLine();
        // }

        // for (int i = 0; i < first.StcFunctions.Count; i++)
        // {
        //     FunctionBinary func = first.StcFunctions[i];
        //     Console.WriteLine("name:  " + first.StcFunctions[i].Name);
        //     Console.WriteLine("arg_list:");
        //     for (int j = 0; j < func.Arguments.Count; j++)
        //     {
        //         Console.WriteLine(func.Arguments[j].Name + "\t" + func.Arguments[j].Type.ToString());
        //     }
        //     Console.WriteLine("identifiers:  " + string.Join(",", func.Identifiers.Select(x => x.ToString())));
        //     Console.WriteLine();
        // }
    }
}