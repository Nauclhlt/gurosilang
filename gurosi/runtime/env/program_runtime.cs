namespace Gurosi;

public sealed class ProgramRuntime
{
    private RuntimeMemory _memory;
    private Stack<LocalContext> _callStack;
    private Executable _executable;
    private RuntimeStack _mainStack;
    private RuntimeStack _tempStack;

    public RuntimeMemory Memory => _memory;
    public Stack<LocalContext> CallStack => _callStack;
    public Executable Executable => _executable;
    public RuntimeStack MainStack => _mainStack;
    public RuntimeStack TempStack => _tempStack;

    public ProgramRuntime(Executable executable)
    {
        _memory = new RuntimeMemory();
        _callStack = new Stack<LocalContext>();
        _mainStack = new RuntimeStack();
        _tempStack = new RuntimeStack();

        _executable = executable;
    }

    public void PushCall(LocalContext context)
    {
        _callStack.Push(context);
    }

    public void Execute()
    {
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
                    _callStack.Pop();
                }
            }
        }
    }
}