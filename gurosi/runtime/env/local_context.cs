using System.Reflection.Metadata;

namespace Gurosi;

public enum CallContext
{
    Root,
    Method,
    StaticMethod,
    Glbl
}

public sealed class LocalContext
{
    private ProgramRuntime _runtime;
    private RuntimeStack _mainStack;
    private RuntimeStack _tempStack;
    private SlicedMemory _memory;
    private CompiledCode _code;
    private SymbolStore _loadedSymbols;
    private CallContext _callContext;
    private RefValueObject _contextSelf;
    private ClassBinary _contextClass;
    private FunctionBinary _contextFunction;
    

    //private int _counter = 0;

    public ProgramRuntime Runtime => _runtime;
    public SlicedMemory Memory => _memory;
    public CompiledCode Code => _code;
    public SymbolStore LoadedSymbols => _loadedSymbols;
    public CallContext CallContext => _callContext;
    public RefValueObject ContextSelf => _contextSelf;
    public ClassBinary ContextClass => _contextClass;
    public FunctionBinary ContextFunction => _contextFunction;

    public bool IsRoot => _callContext == CallContext.Root;
    public bool IsMethod => _callContext == CallContext.Method;
    public bool IsGlbl => _callContext == CallContext.Glbl;

    public LocalContext(ProgramRuntime runtime, SlicedMemory memory, CompiledCode code)
    {
        _runtime = runtime;
        _memory = memory;
        _code = code;
        
        _mainStack = runtime.MainStack;
        _tempStack = runtime.TempStack;
        _loadedSymbols = runtime.LoadedSymbols;
    }

    public LocalContext(ProgramRuntime runtime, SlicedMemory memory, CompiledCode code,
        ClassBinary cls, FunctionBinary method, RefValueObject self)
        : this(runtime, memory, code)
    {
        _callContext = CallContext.Method;
        _contextClass = cls;
        _contextFunction = method;
        _contextSelf = self;
    }

    public LocalContext(ProgramRuntime runtime, SlicedMemory memory, CompiledCode code,
        ClassBinary cls, FunctionBinary method)
        : this(runtime, memory, code)
    {
        _callContext = CallContext.StaticMethod;
        _contextClass = cls;
        _contextFunction = method;
    }

    public LocalContext(ProgramRuntime runtime, SlicedMemory memory, CompiledCode code,
        FunctionBinary func)
        : this(runtime, memory, code)
    {
        _callContext = CallContext.Glbl;
        _contextFunction = func;
    }

    private void SkipInstruction()
    {
        if (_code.EndOfCode)
            return;

        byte opcode = _code.ReadOpCode();
        _code.ReadArgnum();
        
        OperandType[] operands = GIL.OperandMap[GIL.CodeMap[opcode]];

        if (operands is not null)
        {
            for (int i = 0; i < operands.Length; i++)
            {
                OperandType type = operands[i];
                if (type == OperandType.Address)
                    _code.ReadAddress();
                if (type == OperandType.Int)
                    _code.ReadInt();
                if (type == OperandType.Boolean)
                    _code.ReadBoolean();
                if (type == OperandType.Double)
                    _code.ReadDouble();
                if (type == OperandType.Float)
                    _code.ReadFloat();
                if (type == OperandType.String)
                    _code.ReadString();
            }
        }

        _code.ReadInstLen();
    }

    private void BackInstruction()
    {
        _code.Retr(4);
        int length = _code.ReadInstLen();
        _code.Retr(length);
    }

    private void RelativeJump(int offset)
    {
        if (offset < 0)
        {
            for (int i = 0; i < -offset + 1; i++)
            {
                BackInstruction();
            }
        }
        else
        {
            for (int i = 0; i < offset; i++)
            {
                SkipInstruction();
            }
        }
    }

    private void Continue()
    {
        if (_code.EndOfCode)
            return;

        while (true)
        {
            byte opcode = _code.ReadOpCode();

            _code.ReadArgnum();

            OperandType[] operands = GIL.OperandMap[GIL.CodeMap[opcode]];

            if (operands is not null)
            {
                for (int i = 0; i < operands.Length; i++)
                {
                    OperandType type = operands[i];
                    if (type == OperandType.Address)
                        _code.ReadAddress();
                    if (type == OperandType.Int)
                        _code.ReadInt();
                    if (type == OperandType.Boolean)
                        _code.ReadBoolean();
                    if (type == OperandType.Double)
                        _code.ReadDouble();
                    if (type == OperandType.Float)
                        _code.ReadFloat();
                    if (type == OperandType.String)
                        _code.ReadString();
                }
            }

            _code.ReadInstLen();

            if (opcode == GIL.CPL)
            {
                break;
            }
        }
    }

    private void Break()
    {
        if (_code.EndOfCode)
            return;

        while (true)
        {
            byte opcode = _code.ReadOpCode();

            _code.ReadArgnum();

            OperandType[] operands = GIL.OperandMap[GIL.CodeMap[opcode]];

            if (operands is not null)
            {
                for (int i = 0; i < operands.Length; i++)
                {
                    OperandType type = operands[i];
                    if (type == OperandType.Address)
                        _code.ReadAddress();
                    if (type == OperandType.Int)
                        _code.ReadInt();
                    if (type == OperandType.Boolean)
                        _code.ReadBoolean();
                    if (type == OperandType.Double)
                        _code.ReadDouble();
                    if (type == OperandType.Float)
                        _code.ReadFloat();
                    if (type == OperandType.String)
                        _code.ReadString();
                }
            }

            _code.ReadInstLen();

            if (opcode == GIL.BPL)
            {
                break;
            }
        }
    }

    public bool RunInstruction()
    {
        if (_code.EndOfCode)
            return true;

        byte opcode = _code.ReadOpCode();
        _code.ReadArgnum();

        bool jmpFlag = false;

        switch (opcode)
        {
            case GIL.PUSH:
                {
                    // メモリ上の値をメインスタックに積む。
                    int addr = _code.ReadAddress();
                    IValueObject source = _memory.Read(addr);
                    _mainStack.Push(source.Clone());

                    DebugWrite($"[DBG] Stack Push  addr={addr}");
                    break;
                }
            case GIL.POP:
                {
                    // メインスタックから値をとりだし、メモリ上に入れる。
                    int addr = _code.ReadAddress();
                    IValueObject source = _mainStack.Pop();
                    _memory.Write(addr, source.Clone());

                    DebugWrite($"[DBG] Stack Pop  addr={addr}");
                    break;
                }
            case GIL.DISC:
                {
                    // メインスタックから値をとりだし、捨てる。
                    _mainStack.Pop();
                    break;
                }
            case GIL.PUSHM:
                {
                    // メモリ上の値を一時スタックに積む。
                    int addr = _code.ReadInt();
                    IValueObject source = _memory.Read(addr);
                    _tempStack.Push(source.Clone());
                    break;
                }
            case GIL.PUSHS:
                {
                    // スタック上の値を一時スタックに積む。
                    // ※取り出しなし
                    IValueObject source = _mainStack.Top();
                    _tempStack.Push(source.Clone());
                    break;
                }
            case GIL.POPM:
                {
                    // 一時スタックから値をとりだし、メモリ上に入れる。
                    int addr = _code.ReadInt();
                    IValueObject source = _tempStack.Pop();
                    _memory.Write(addr, source.Clone());
                    break;
                }
            case GIL.POPS:
                {
                    // 一時スタックから値をとりだし、メインスタックに積む。
                    IValueObject source = _tempStack.Pop();
                    _mainStack.Push(source.Clone());
                    break;
                }
            case GIL.POPF:
                {
                    // メインスタック上から値をとりだし、その下に積まれたクラスのインスタンスにおけるi番目のフィールドに格納する。
                    int index = _code.ReadInt();
                    IValueObject value = _mainStack.Pop();
                    RefValueObject targetRef = (RefValueObject)_mainStack.Pop();

                    ClassValueObject target = targetRef.Refer<ClassValueObject>(_runtime.Heap);

                    target.SetFieldValue(index, value.Clone());

                    DebugWrite($"[DBG] Field set idx={index}");
                    break;
                }
            case GIL.POPIDX:
                {
                    // メインスタック上から値をとりだし、その下に積まれた配列の指定インデックスに格納する。
                    // スタック上から インデックス => 配列 => 値。

                    // int型保証
                    IValueObject indexSource = _mainStack.Pop();
                    // arrayのポインタ保証
                    IValueObject arrRefSource = _mainStack.Pop();
                    IValueObject valueSource = _mainStack.Pop();

                    int index = indexSource.GetNumericValue<int>();
                    ArrayValueObject array = (ArrayValueObject)_runtime.Heap.Read(arrRefSource.HeapPointer);

                    if (index < 0 || index >= array.Length)
                    {
                        // index outside the bounds of the array.
                        _runtime.RaiseIndexOutOfRange();
                    }
                    else
                    {
                        array.Body[index] = valueSource.Clone();

                        DebugWrite($"[DBG] Array assign  index={index}");
                    }

                    break;
                }
            case GIL.POPSTC:
                {
                    // メインスタック上から値をとりだし、その下に積まれた名前におけるi番目の静的変数に値を格納する。
                    break;
                }
            case GIL.ADD:
                {
                    // メインスタック上の値を加算する。スタックの上から(右辺)=>(左辺)
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();
                    // 計算。型はコンパイル時に保証されている。
                    IValueObject result = CalcUtil.Add(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Add");
                    break;
                }
            case GIL.SUB:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.Sub(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Sub");
                    break;
                }
            case GIL.MUL:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.Mul(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Mul");
                    break;
                }
            case GIL.DIV:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.Div(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Div");
                    break;
                }
            case GIL.MOD:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.Mod(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Mod");
                    break;
                }
            case GIL.LT:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.LessThan(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Less than");
                    break;
                }
            case GIL.LTE:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.LessThanEqual(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Less than equal");
                    break;
                }
            case GIL.GT:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.GreaterThan(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Greater than");
                    break;
                }
            case GIL.GTE:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.GreaterThanEqual(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Greater than equal");
                    break;
                }
            case GIL.EQ:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.Equal(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Equality");
                    break;
                }
            case GIL.NEQ:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.NotEqual(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Not equal");
                    break;
                }
            case GIL.AND:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.And(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] And");
                    break;
                }
            case GIL.OR:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.Or(left, right, _runtime);

                    _mainStack.Push(result);

                    DebugWrite($"[DBG] Or");
                    break;
                }
            case GIL.XOR:
                {
                    IValueObject right = _mainStack.Pop();
                    IValueObject left = _mainStack.Pop();

                    IValueObject result = CalcUtil.Xor(left, right, _runtime);

                    _mainStack.Push(result);
                    break;
                }
            case GIL.COMP:
                {
                    break;
                }
            case GIL.SHL:
                {
                    // 左ビットシフト
                    break;
                }
            case GIL.SHR:
                {
                    // 右ビットシフト
                    break;
                }
            case GIL.JMP:
                {
                    // 命令を相対的にジャンプする。パラメータはオフセット値。
                    int offset = _code.ReadInt();

                    _code.ReadInstLen();
                    RelativeJump(offset);

                    jmpFlag = true;
                    DebugWrite($"[DBG] Jump  offset={offset}");
                    break;
                }
            case GIL.CJMP:
                {
                    // スタック上の値がtrueならジャンプする。
                    int offset = _code.ReadInt();
                    IValueObject value = _mainStack.Pop();

                    if ((value as BooleanValueObject).Value)
                    {
                        _code.ReadInstLen();
                        RelativeJump(offset);
                        jmpFlag = true;
                    }
                    
                    break;
                }
            case GIL.NJMP:
                {
                    // falseなら。
                    int offset = _code.ReadInt();
                    IValueObject value = _mainStack.Pop();

                    if (!(value as BooleanValueObject).Value)
                    {
                        _code.ReadInstLen();
                        RelativeJump(offset);
                        jmpFlag = true;
                    }
                    break;
                }
            case GIL.TFJMP:
                {
                    // trueなら第一パラメータ, falseなら第二パラメータジャンプする。
                    int offset1 = _code.ReadInt();
                    int offset2 = _code.ReadInt();
                    IValueObject value = _mainStack.Pop();

                    _code.ReadInstLen();
                    if ((value as BooleanValueObject).Value)
                    {
                        RelativeJump(offset1);
                    }
                    else
                    {
                        RelativeJump(offset2);
                    }
                    jmpFlag = true;

                    break;
                }
            case GIL.PJMP:
                {
                    // スタック上の値が正ならジャンプする。
                    int offset = _code.ReadInt();
                    IValueObject value = _mainStack.Pop();

                    if (CalcUtil.IsPositive(value, _runtime))
                    {
                        _code.ReadInstLen();
                        RelativeJump(offset);
                        jmpFlag = true;
                    }

                    break;
                }
            case GIL.MJMP:
                {
                    // スタック上の値が負ならジャンプする。
                    int offset = _code.ReadInt();
                    IValueObject value = _mainStack.Pop();

                    if (CalcUtil.IsNegative(value, _runtime))
                    {
                        _code.ReadInstLen();
                        RelativeJump(offset);
                        jmpFlag = true;
                    }

                    break;
                }
            case GIL.ZJMP:
                {
                    // スタック上の値がゼロならジャンプする。
                    int offset = _code.ReadInt();
                    IValueObject value = _mainStack.Pop();

                    if (CalcUtil.IsZero(value, _runtime))
                    {
                        _code.ReadInstLen();
                        RelativeJump(offset);
                        jmpFlag = true;
                    }

                    break;
                }
            case GIL.SEL:
                {
                    // スタック上の値から指定した名前(主に関数)を選択する。

                    string name = _code.ReadString();
                    IValueObject top = _mainStack.Pop();
                    if (top is RefValueObject refv)
                    {
                        ClassValueObject cls = refv.Refer<ClassValueObject>(_runtime.Heap);
                        FunctionBinary func = cls.Class.GetFunctionByFixedName(name);

                        _mainStack.Push(new MethodNameObject((RefValueObject)refv.Clone(), cls.Class, func));
                    }
                    else if (top is ClassNameObject cn)
                    {
                        FunctionBinary func = cn.Class.GetStcFunctionByFixedName(name);

                        _mainStack.Push(new StaticMethodNameObject(cn.Class, func));
                    }
                    else if (top is ModuleNameObject module)
                    {
                        FunctionBinary function = _loadedSymbols.FindGlobalFunction(module.Name, name);

                        if (function is not null)
                        {
                            _mainStack.Push(new GlobalFuncObject(function));
                        }
                        else
                        {
                            ClassBinary c = _loadedSymbols.FindClass(new TypePath(module.Name, name));

                            _mainStack.Push(new ClassNameObject(c));
                        }
                    }

                    break;
                }
            case GIL.ALLOC:
                {
                    // 指定したアドレスを指定した型で確保する。
                    int addr = _code.ReadInt();
                    TypePath type = TypePath.FromString(_code.ReadString());
                    type = WithGenerics(type);

                    _memory.Alloc(addr);
                    _memory.Write(addr, IValueObject.DefaultOf(type));

                    DebugWrite($"[DBG] Alloc  addr={addr}  type={type}");
                    break;
                }
            case GIL.ARR:
                    {
                    // 指定した型の配列を作成して、スタック上に積む。
                    // スタック上の値を長さとする。
                    TypePath elementType = TypePath.FromString(_code.ReadString());
                    elementType = WithGenerics(elementType);
                    IValueObject lengthSource = _mainStack.Pop();
                    int length = lengthSource.GetNumericValue<int>();

                    ArrayValueObject arr = new ArrayValueObject(elementType, length);

                    int addr = _runtime.Heap.Alloc();
                    _runtime.Heap.Write(addr, arr);

                    _mainStack.Push(new RefValueObject(arr.Type, addr));

                    DebugWrite($"[DBG] Array gen  length={length}  type={elementType}");

                    break;
                }
            case GIL.INST:
                {
                    // 指定したクラスのインスタンスを作成する。
                    TypePath classType = TypePath.FromString(_code.ReadString());

                    ClassValueObject value = new ClassValueObject(_loadedSymbols.FindClass(classType), classType);

                    int addr = _runtime.Heap.Alloc();
                    _runtime.Heap.Write(addr, value);

                    _mainStack.Push(new RefValueObject(value.Type, addr));
                    break;
                }
            case GIL.RET:
                {
                    // スタック上の値を取り出し、呼び出し元に返却する。
                    _code.ReadInstLen();
                    return true;
                }
            case GIL.ERR:
                {
                    // 実行時エラーをトリガーする。
                    // スタック上から エラーメッセージ => エラーコード

                    // stringであることが保証されている。
                    StringValueObject message = (StringValueObject)_mainStack.Pop();
                    StringValueObject code = (StringValueObject)_mainStack.Pop();

                    _runtime.RaiseRuntimeError(new RuntimeError(code.Value, message.Value));

                    break;
                }
            // 以下即値のpush
            case GIL.PUSHSTR:
                {
                    string value = _code.ReadString();

                    _mainStack.Push(new StringValueObject(value));

                    DebugWrite($"[DBG] Pushstr value={value}");
                    break;
                }
            case GIL.PUSHINT:
                {
                    int value = _code.ReadInt();

                    _mainStack.Push(new IntValueObject(value));

                    DebugWrite($"[DBG] Pushint value={value}");
                    break;
                }
            case GIL.PUSHFLT:
                {
                    float value = _code.ReadFloat();

                    _mainStack.Push(new FloatValueObject(value));

                    DebugWrite($"[DBG] Pushflt value={value}");
                    break;
                }
            case GIL.PUSHDBL:
                {
                    double value = _code.ReadDouble();

                    _mainStack.Push(new DoubleValueObject(value));

                    DebugWrite($"[DBG] Pushdbl value={value}");
                    break;
                }
            case GIL.PUSHBOOL:
                {
                    bool value = _code.ReadBoolean();

                    _mainStack.Push(new BooleanValueObject(value));

                    DebugWrite($"[DBG] Pushbool value={value}");
                    break;
                }
            case GIL.PUSHNULL:
                {
                    _mainStack.Push(IValueObject.NullRef);

                    DebugWrite($"[DBG] Push null");
                    break;
                }
            case GIL.MOV:
                {
                    // 指定した名前のモジュールをスタックに積む。

                    string moduleName = _code.ReadString();

                    _mainStack.Push(new ModuleNameObject(moduleName));
                    break;
                }
            case GIL.CALL:
                {
                    // スタックに積まれた関数を呼び出す。

                    IValueObject top = _mainStack.Pop();

                    if (top is MethodNameObject method)
                    {
                        _runtime.CallMethod(method.Class, method.Function, method.Source,
                            _runtime.PopArguments(method.Function.Arguments.Count));

                        DebugWrite($"[DBG] Call  name={method.Function.Name}");
                    }
                    else if (top is StaticMethodNameObject stcm)
                    {
                        _runtime.CallStaticMethod(stcm.Class, stcm.Function, _runtime.PopArguments(stcm.Function.Arguments.Count));

                        DebugWrite($"[DBG] Call  name={stcm.Function.Name}");
                    }
                    else if (top is GlobalFuncObject func)
                    {
                        ArgumentList args;
                        if (func.Function.IsExtendR)
                        {
                            args = _runtime.PopArguments(func.Function.Arguments.Count + 1);
                        }
                        else
                        {
                            args = _runtime.PopArguments(func.Function.Arguments.Count);
                        }
                        _runtime.CallFunction(func.Function, args);
                        DebugWrite($"[DBG] Call  name={func.Function.Name}");
                    }

                    break;
                }
            case GIL.IDX:
                {
                    // スタックに積まれた値をインデックスとして値を取り出し、スタックに積む。
                    // スタック上で上からインデックス、取り出し元。
                    IntValueObject index = (IntValueObject)_mainStack.Pop();
                    RefValueObject source = (RefValueObject)_mainStack.Pop();

                    int idx = index.GetNumericValue<int>();

                    ArrayValueObject arr = source.Refer<ArrayValueObject>(_runtime.Heap);

                    if (idx < 0 || idx >= arr.Length)
                    {
                        _runtime.RaiseIndexOutOfRange();
                        break;
                    }

                    _mainStack.Push(arr.Body[idx].Clone());

                    break;
                }
            case GIL.SET:
                {
                    // This may not be used.
                    break;
                }
            case GIL.SELF:
                {
                    // 現在実行されているクラスのインスタンスをスタック上に積む。
                    _mainStack.Push(_contextSelf.Clone());
                    break;
                }
            case GIL.CPL:
                {
                    // continueで移動するラベル。
                    // 何も処理しない。
                    break;
                }
            case GIL.BPL:
                {
                    // breakで移動するラベル。
                    // 何も処理しない。
                    break;
                }
            case GIL.CONT:
                {
                    // continue
                    _code.ReadInstLen();
                    Continue();
                    jmpFlag = true;
                    break;
                }
            case GIL.BRK:
                {
                    // break
                    _code.ReadInstLen();
                    Break();
                    jmpFlag = true;
                    break;
                }
            case GIL.HEAP:
                {
                    // スタック上のクラスのインスタンスにおけるi番目のフィールドをスタックに積む。
                    int index = _code.ReadInt();
                    RefValueObject refv = (RefValueObject)_mainStack.Pop();

                    ClassValueObject cls = refv.Refer<ClassValueObject>(_runtime.Heap);

                    _mainStack.Push(cls.FieldValueOf(index));

                    break;
                }
            case GIL.HPSTC:
                {
                    // スタック上のクラスにおけるi番目の静的フィールドをスタックに積む。
                    break;
                }
            case GIL.NVC:
                {
                    // ランタイム実装関数を検索呼び出し。
                    string asm = _code.ReadString();
                    string cls = _code.ReadString();
                    string symbol = _code.ReadString();

                    NVC.Run(asm, cls, symbol, _runtime);

                    DebugWrite($"[DBG] NVC call  {asm}::{cls}::{symbol}");
                    break;
                }
            case GIL.NVRETV:
                {
                    // ラインタイム実装関数から返された値をスタック上に積む。
                    _mainStack.Push(NVC.GetReturnValue().Clone());
                    break;
                }
            case GIL.CSTIF:
                {
                    // int => float のキャストを行う。
                    IValueObject target = _mainStack.Pop();

                    _mainStack.Push(CalcUtil.PrCastIF(target, _runtime));
                    break;
                }
            case GIL.CSTID:
                {
                    // int => double のキャストを行う。
                    IValueObject target = _mainStack.Pop();

                    _mainStack.Push(CalcUtil.PrCastID(target, _runtime));
                    break;
                }
            case GIL.CSTFD:
                {
                    // float => double のキャストを行う。
                    IValueObject target = _mainStack.Pop();

                    _mainStack.Push(CalcUtil.PrCastFD(target, _runtime));
                    break;
                }
            case GIL.INC:
                {
                    // インクリメント
                    IValueObject target = _mainStack.Pop();

                    _mainStack.Push(CalcUtil.Increment(target, _runtime));
                    break;
                }
            case GIL.DEC:
                {
                    // デクリメント
                    IValueObject target = _mainStack.Pop();

                    _mainStack.Push(CalcUtil.Decrement(target, _runtime));
                    break;
                }
            case GIL.CSTOBJ:
                {
                    // スタック上のオブジェクトを目的の型にキャストする。

                    IValueObject from = _mainStack.Pop();
                    TypePath type = TypePath.FromString(_code.ReadString());
                    type = WithGenerics(type);

                    if (from is RefValueObject rv)
                    {
                        // reference type casting.

                        // down-casting
                        if (from.Type.IsCompatibleWith(type, _loadedSymbols, casting: false))
                        {
                            _mainStack.Push(new RefValueObject(type, rv.HeapPointer));
                        }
                        // up-casting
                        else
                        {
                            ClassValueObject ac = rv.Refer<ClassValueObject>(_runtime.Heap);
                            if (ac.Type.IsCompatibleWith(type, _loadedSymbols, casting: false))
                            {
                                _mainStack.Push(new RefValueObject(type, rv.HeapPointer));
                            }
                            else
                            {
                                _runtime.RaiseInvalidCast();
                            }
                        }
                    }
                    else
                    {
                        _mainStack.Push(from.Clone());
                    }
                    

                    break;
                }
            case GIL.FPTR:
                {
                    // スタック上の関数のポインタを取得し、スタックに積む。
                    IValueObject value = _mainStack.Pop();
                    if (value is MethodNameObject m)
                    {
                        _mainStack.Push(FunctionPointerObject.MakeMethod(m.Class, m.Function, m.Source));
                    }
                    else if (value is GlobalFuncObject g)
                    {
                        _mainStack.Push(FunctionPointerObject.MakeGlobalFunc(g.Function));
                    }
                    else if (value is StaticMethodNameObject s)
                    {
                        _mainStack.Push(FunctionPointerObject.MakeStaticMethod(s.Class, s.Function));
                    }

                    break;
                }
            default:
                // unknown instruction.
                return true;
        }

        // skip instruction length.
        if (!jmpFlag)
        {
            _code.ReadInstLen();
        }
        

        return false;
    }

    private TypePath WithGenerics(TypePath type)
    {
        if (!type.IsGenericParam)
            return type;

        if (_callContext == CallContext.Method)
        {
            ClassValueObject cls = _contextSelf.Refer<ClassValueObject>(_runtime.Heap);

            if (cls.Generics.Count == 0)
                return type;
            else
            {
                return cls.Generics[type.GenericParamIndex];
            }
        }
        else
        {
            // FIXME: ??
            // when implementing static function with generics.
            return type;
        }
    }

    private static void DebugWrite(string msg)
    {
        //Console.WriteLine(msg);
    }
}