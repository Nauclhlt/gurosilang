namespace Gurosi;

public sealed class ProgramRuntime
{
    private RuntimeMemory _memory;
    private RuntimeHeap _heap;
    private Stack<LocalContext> _callStack;
    private Executable _executable;
    private RuntimeStack _mainStack;
    private RuntimeStack _tempStack;
    private RuntimeError _runtimeError;

    private SymbolStore _loadedSymbols;

    public RuntimeMemory Memory => _memory;
    public RuntimeHeap Heap => _heap;
    public Stack<LocalContext> CallStack => _callStack;
    public Executable Executable => _executable;
    public RuntimeStack MainStack => _mainStack;
    public RuntimeStack TempStack => _tempStack;
    public RuntimeError RuntimeError => _runtimeError;

    public SymbolStore LoadedSymbols => _loadedSymbols;

    public bool HasRuntimeError => _runtimeError is not null;

    public ProgramRuntime(Executable executable)
    {
        _memory = new RuntimeMemory(executable.MemorySize);
        _heap = new RuntimeHeap();
        _callStack = new Stack<LocalContext>();
        _mainStack = new RuntimeStack();
        _tempStack = new RuntimeStack();

        _executable = executable;
    }

    public void CallMethod(ClassBinary cls, FunctionBinary method, RefValueObject self, ArgumentList args)
    {
        _callStack.Push(new LocalContext(this, _memory.Slice(), CompiledCode.CompileFromBinary(method.Body),
            cls, method, self));
        args.ExtractOnMemory(_callStack.Peek().Memory);
    }

    public void CallStaticMethod(ClassBinary cls, FunctionBinary method, ArgumentList args)
    {
        _callStack.Push(new LocalContext(this, _memory.Slice(), CompiledCode.CompileFromBinary(method.Body), cls, method));
        args.ExtractOnMemory(_callStack.Peek().Memory);
    }

    public void CallFunction(FunctionBinary func, ArgumentList args)
    {
        _callStack.Push(new LocalContext(this, _memory.Slice(), CompiledCode.CompileFromBinary(func.Body), func));
        args.ExtractOnMemory(_callStack.Peek().Memory);
    }

    public void RaiseRuntimeError(RuntimeError error)
    {
        _runtimeError = error;
    }

    public void Execute()
    {
        // Load references

        List<Library> refs = [_executable.SelfLibrary];
        for (int i = 0; i < _executable.AllImports.Count; i++)
        {
            string target = _executable.AllImports[i];
            if (!File.Exists(target))
            {
                // not found.
                this.RaiseLibNotFound();
                return;
            }

            try
            {
                Library lib = Library.Load(target);
                refs.Add(lib);
            }
            catch (Exception)
            {
                // error loading.
                this.RaiseLibNotFound();
            }
        }

        _loadedSymbols = SymbolStore.FromLibraries(refs);
        //



        _callStack.Push(new LocalContext(this, _memory.Slice(),
            CompiledCode.CompileFromBinary(_executable.EntryCode)));

        while (true)
        {
            LocalContext top = _callStack.Peek();
            
            if (top.RunInstruction())
            {
                // Program ending
                if (_callStack.Count == 1)
                {
                    // When leaving run block.
                    return;
                }
                else
                {
                    LocalContext last = _callStack.Pop();
                    
                    _memory.Release(last.Memory);
                }
            }

            if (_runtimeError is not null)
            {
                return;
            }
        }
    }
}