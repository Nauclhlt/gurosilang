namespace Gurosi;

public sealed class CompileResult
{
    private List<string> _code;
    private ClassBinary _class;
    private FunctionBinary _function;
    private List<Error> _errors;

    public List<string> Code => _code;
    public List<Error> Errors => _errors;
    public ClassBinary Class => _class;
    public FunctionBinary Function => _function;
    public bool HasError => _errors.Count != 0;

    public CompileResult(CompileEnvironment env)
    {
        _code = env.Code;
        _errors = env.Errors;
    }

    public CompileResult(List<string> code, List<Error> errors)
    {
        _code = code;
        _errors = errors;
    }

    public CompileResult(ClassBinary cls, List<Error> errors)
    {
        _class = cls;
        _errors = errors;
    }

    public CompileResult(FunctionBinary func, List<Error> errors)
    {
        _function = func;
        _errors = errors;
    }

    public void _PrintDebug()
    {
        for (int i = 0; i < _class.Functions.Count; i++)
        {
            List<string> code = _class.Functions[i].Body.Code;
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
    }
}