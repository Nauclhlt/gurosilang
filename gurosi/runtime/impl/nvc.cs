namespace Gurosi;

// ランタイム実装関数
public static class NVC
{
    private static IValueObject _returnValue;

    public static void Run(string asm, string cls, string symbol, ProgramRuntime runtime)
    {
        switch (asm)
        {
            case "sys":
                {
                    if (cls == "string")
                    {
                        RunSysString(symbol, runtime);
                    }
                    else if (cls == "log")
                    {
                        RunSysLog(symbol, runtime);
                    }
                    break;
                }
            default:
                return;
        }
    }

    private static void RunSysString(string symbol, ProgramRuntime runtime)
    {
        if (symbol == "length")
        {
            StringValueObject str = (StringValueObject)runtime.MainStack.Pop();
            _returnValue = new IntValueObject(str.Value.Length);
        }
        else if (symbol == "substring")
        {
            IntValueObject length = (IntValueObject)runtime.MainStack.Pop();
            IntValueObject start = (IntValueObject)runtime.MainStack.Pop();
            StringValueObject str = (StringValueObject)runtime.MainStack.Pop();

            _returnValue = new StringValueObject(str.Value.Substring(start.GetNumericValue<int>(), length.GetNumericValue<int>()));
        }
        else if (symbol == "indexOf")
        {
            StringValueObject value = (StringValueObject)runtime.MainStack.Pop();
            StringValueObject target = (StringValueObject)runtime.MainStack.Pop();

            _returnValue = new IntValueObject(target.Value.IndexOf(value.Value));
        }
        else if (symbol == "replace")
        {
            StringValueObject to = (StringValueObject)runtime.MainStack.Pop();
            StringValueObject from = (StringValueObject)runtime.MainStack.Pop();
            StringValueObject target = (StringValueObject)runtime.MainStack.Pop();

            _returnValue = new StringValueObject(target.Value.Replace(from.Value, to.Value, StringComparison.Ordinal));
        }
        else if (symbol == "split")
        {
            IntValueObject max = (IntValueObject)runtime.MainStack.Pop();
            StringValueObject delimiter = (StringValueObject)runtime.MainStack.Pop();
            StringValueObject target = (StringValueObject)runtime.MainStack.Pop();

            string[] split = target.Value.Split(delimiter.Value);

            _returnValue = GenerateArray(runtime, TypePath.STRING, split, x =>
            {
                return new StringValueObject(x);
            });
        }
        else if (symbol == "as_str")
        {
            IValueObject value = runtime.MainStack.Pop();

            // TODO: implement
            _returnValue = new StringValueObject("Not implemented");
        }
    }

    private static void RunSysLog(string symbol, ProgramRuntime runtime)
    {
        if (symbol == "print")
        {
            StringValueObject value = (StringValueObject)runtime.MainStack.Pop();
            Console.Write(value.Value);
        }
        else if (symbol == "println")
        {
            StringValueObject value = (StringValueObject)runtime.MainStack.Pop();
            Console.WriteLine(value.Value);
        }
        else if (symbol == "print_v")
        {
            IValueObject value = runtime.MainStack.Pop();
            Console.Write(value);
        }
        else if (symbol == "println_v")
        {
            IValueObject value = runtime.MainStack.Pop();
            Console.WriteLine(value);
        }
        else if (symbol == "scanln")
        {
            string input = Console.ReadLine();
            _returnValue = new StringValueObject(input);
        }
        else if (symbol == "clear")
        {
            Console.Clear();
        }
    }

    private static void RunSysArray(string symbol, ProgramRuntime runtime)
    {
        // TODO: implement

    }

    public static IValueObject GetReturnValue()
    {
        return _returnValue;
    }

    private static RefValueObject GenerateArray<T>(ProgramRuntime runtime, TypePath elementType, T[] source, Func<T, IValueObject> conv)
    {
        ArrayValueObject arr = new ArrayValueObject(elementType, source.Length);

        for (int i = 0; i < source.Length; i++)
        {
            arr.Body[i] = conv(source[i]);
        }

        int addr = runtime.Heap.Alloc();
        runtime.Heap.Write(addr, arr);

        return new RefValueObject(arr.Type, addr);
    }
}