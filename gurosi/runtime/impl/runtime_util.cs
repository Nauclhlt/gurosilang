namespace Gurosi;

public static class RuntimeUtil
{
    public static ArgumentList PopArguments(this ProgramRuntime runtime, int count)
    {
        ArgumentList list = new ArgumentList(count);
        for (int i = count - 1; i >= 0; i--)
        {
            list.Load(i, runtime.MainStack.Pop());
        }

        return list;
    }
}