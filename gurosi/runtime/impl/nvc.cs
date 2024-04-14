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
                    else if (cls == "array")
                    {
                        RunSysArray(symbol, runtime);
                    }
                    else if (cls == "math")
                    {
                        RunSysMath(symbol, runtime);
                    }
                    break;
                }
            case "meta":
                {
                    if (cls == "funcptr")
                    {
                        RunMetaFuncptr(symbol, runtime);
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
            _returnValue = new StringValueObject(IValueObject.AsStr(value, runtime));
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
            Console.Write(IValueObject.AsStr(value, runtime));
        }
        else if (symbol == "println_v")
        {
            IValueObject value = runtime.MainStack.Pop();
            Console.WriteLine(IValueObject.AsStr(value, runtime));
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
        if (symbol == "length")
        {
            RefValueObject arrref = (RefValueObject)runtime.MainStack.Pop();

            ArrayValueObject arr = arrref.Refer<ArrayValueObject>(runtime.Heap);

            _returnValue = new IntValueObject(arr.Length);
        }
        else if (symbol == "copy")
        {
            RefValueObject destref = (RefValueObject)runtime.MainStack.Pop();
            RefValueObject sourceref = (RefValueObject)runtime.MainStack.Pop();

            ArrayValueObject dest = destref.Refer<ArrayValueObject>(runtime.Heap);
            ArrayValueObject source = sourceref.Refer<ArrayValueObject>(runtime.Heap);

            int length = int.Min(dest.Length, source.Length);

            for (int i = 0; i < length; i++)
            {
                dest[i] = source[i].Clone();
            }
        }
        else if (symbol == "copy_c")
        {
            int count = runtime.MainStack.Pop().GetNumericValue<int>();
            RefValueObject destref = (RefValueObject)runtime.MainStack.Pop();
            RefValueObject sourceref = (RefValueObject)runtime.MainStack.Pop();

            ArrayValueObject dest = destref.Refer<ArrayValueObject>(runtime.Heap);
            ArrayValueObject source = sourceref.Refer<ArrayValueObject>(runtime.Heap);

            int length = int.Min(int.Min(count, dest.Length), source.Length);

            for (int i = 0; i < length; i++)
            {
                dest[i] = source[i].Clone();
            }
        }
        else if (symbol == "copy_ic")
        {
            int count = runtime.MainStack.Pop().GetNumericValue<int>();
            int destIdx = runtime.MainStack.Pop().GetNumericValue<int>();
            RefValueObject destref = (RefValueObject)runtime.MainStack.Pop();
            int sourceIdx = runtime.MainStack.Pop().GetNumericValue<int>();
            RefValueObject sourceref = (RefValueObject)runtime.MainStack.Pop();

            ArrayValueObject dest = destref.Refer<ArrayValueObject>(runtime.Heap);
            ArrayValueObject source = sourceref.Refer<ArrayValueObject>(runtime.Heap);

            for (int i = 0; i < count; i++)
            {
                int di = destIdx + i;
                int si = sourceIdx + i;

                if (di < 0 || si < 0 || di >= dest.Length || si >= source.Length)
                    continue;

                dest[di] = source[si].Clone();
            }
        }
    }

    private static void RunSysMath(string symbol, ProgramRuntime runtime)
    {
        if (symbol == "sinf")
        {
            float angle = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Sin(angle));
        }
        else if (symbol == "cosf")
        {
            float angle = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Cos(angle));
        }
        else if (symbol == "tanf")
        {
            float angle = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Tan(angle));
        }
        else if (symbol == "asinf")
        {
            float value = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Asin(value));
        }
        else if (symbol == "acosf")
        {
            float value = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Acos(value));
        }
        else if (symbol == "atanf")
        {
            float value = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Atan(value));
        }
        else if (symbol == "atan2f")
        {
            float x = runtime.MainStack.Pop().GetNumericValue<float>();
            float y = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Atan2(y, x));
        }
        else if (symbol == "log2f")
        {
            float value = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Log2(value));
        }
        else if (symbol == "log10f")
        {
            float value = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Log10(value));
        }
        else if (symbol == "logf")
        {
            float value = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Log(value));
        }
        else if (symbol == "roundf")
        {
            int digits = runtime.MainStack.Pop().GetNumericValue<int>();
            float value = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Round(value, digits));
        }
        else if (symbol == "sqrtf")
        {
            float value = runtime.MainStack.Pop().GetNumericValue<float>();

            _returnValue = new FloatValueObject(MathF.Sqrt(value));
        }
    }

    private static void RunMetaFuncptr(string symbol, ProgramRuntime runtime)
    {
        if (symbol == "invoke" || symbol == "invoke_ret")
        {
            RefValueObject argsRef = (RefValueObject)runtime.MainStack.Pop();
            FunctionPointerObject fptr = (FunctionPointerObject)runtime.MainStack.Pop();

            if (symbol == "invoke_ret" && !fptr.Function.ReturnsValueR)
            {
                runtime.RaiseNoReturnValue();
                return;
            }

            ArrayValueObject args = argsRef.Refer<ArrayValueObject>(runtime.Heap);

            for (int i = 0; i < args.Length; i++)
            {
                runtime.MainStack.Push(args[i].Clone());
            }

            if (fptr.Kind == FunctionPointerKind.Global)
            {
                if (fptr.Function.IsExtendR)
                    runtime.CallFunction(fptr.Function, runtime.PopArguments(fptr.Function.Arguments.Count + 1));
                else
                    runtime.CallFunction(fptr.Function, runtime.PopArguments(fptr.Function.Arguments.Count));
            }
            else if (fptr.Kind == FunctionPointerKind.Method)
            {
                runtime.CallMethod(fptr.Class, fptr.Function, fptr.Self, runtime.PopArguments(fptr.Function.Arguments.Count));
            }
            else if (fptr.Kind == FunctionPointerKind.StaticMethod)
            {
                runtime.CallStaticMethod(fptr.Class, fptr.Function, runtime.PopArguments(fptr.Function.Arguments.Count));
            }

            if (fptr.Function.ReturnsValueR)
            {
                if (symbol == "call_ret")
                {
                    _returnValue = runtime.MainStack.Pop();
                }
                else
                {
                    runtime.MainStack.Pop();
                }
            }
        }
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