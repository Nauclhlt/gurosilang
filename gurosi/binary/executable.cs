namespace Gurosi;

public sealed class Executable
{
    private CodeBinary _entryCode;
    private List<string> _allImports;
    private Library _selfLibrary;

    public CodeBinary EntryCode => _entryCode;
    public List<string> AllImports => _allImports;
    public Library SelfLibrary => _selfLibrary;

    public Executable(CodeBinary code, List<string> allImports, Library selfLibrary)
    {
        _entryCode = code;
        _allImports = allImports.Distinct().ToList();
        _selfLibrary = selfLibrary;
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