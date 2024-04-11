namespace Gurosi;

public sealed class LocalContext
{
    private ProgramRuntime _runtime;
    private RuntimeStack _mainStack;
    private RuntimeStack _tempStack;
    private SlicedMemory _memory;
    private CompiledCode _code;

    private int _counter = 0;

    public ProgramRuntime Runtime => _runtime;
    public SlicedMemory Memory => _memory;
    public CompiledCode Code => _code;

    public LocalContext(ProgramRuntime runtime, SlicedMemory memory, CompiledCode code)
    {
        _runtime = runtime;
        _memory = memory;
        _code = code;
        
        _mainStack = runtime.MainStack;
        _tempStack = runtime.TempStack;
    }

    public bool RunInstruction()
    {
        if (_code.EndOfCode)
            return true;

        byte opcode = _code.ReadOpCode();

        switch (opcode)
        {
            case GIL.PUSH:
                {
                    // メモリ上の値をメインスタックに積む。
                    int addr = _code.ReadAddress();
                    IValueObject source = _memory.Read(addr);
                    _mainStack.Push(source);
                    break;
                }
            case GIL.POP:
                {
                    // メインスタックから値をとりだし、メモリ上に入れる。
                    int addr = _code.ReadAddress();
                    IValueObject source = _mainStack.Pop();
                    _memory.Write(addr, source);
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
                    _tempStack.Push(source);
                    break;
                }
            case GIL.PUSHS:
                {
                    // スタック上の値を一時スタックに積む。
                    IValueObject source = _mainStack.Top();
                    _tempStack.Push(source);
                    break;
                }
            case GIL.POPM:
                {
                    // 一時スタックから値をとりだし、メモリ上に入れる。
                    int addr = _code.ReadInt();
                    IValueObject source = _tempStack.Pop();
                    _memory.Write(addr, source);
                    break;
                }
            case GIL.POPS:
                {
                    // 一時スタックから値をとりだし、メインスタックに積む。
                    IValueObject source = _tempStack.Pop();
                    _mainStack.Push(source);
                    break;
                }
            case GIL.POPF:
                {
                    // メインスタック上から値をとりだし、その下に積まれたクラスのインスタンスにおけるi番目のフィールドに格納する。

                    break;
                }
            case GIL.POPIDX:
                {
                    // メインスタック上から値をとりだし、その下に積まれた配列の指定インデックスに格納する。
                    // スタック上から インデックス => 配列 => 値。
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

                    break;
                }
            case GIL.SUB:
                {
                    break;
                }
            case GIL.MUL:
                {
                    break;
                }
            case GIL.DIV:
                {
                    break;
                }
            case GIL.MOD:
                {
                    break;
                }
            case GIL.LT:
                {
                    break;
                }
            case GIL.LTE:
                {
                    break;
                }
            case GIL.GT:
                {
                    break;
                }
            case GIL.GTE:
                {
                    break;
                }
            case GIL.EQ:
                {
                    break;
                }
            case GIL.NEQ:
                {
                    break;
                }
            case GIL.AND:
                {
                    break;
                }
            case GIL.OR:
                {
                    break;
                }
            case GIL.XOR:
                {
                    break;
                }
            case GIL.COMP:
                {
                    break;
                }
            case GIL.SHL:
                {
                    break;
                }
            case GIL.SHR:
                {
                    break;
                }
            case GIL.JMP:
                {
                    // 命令を相対的にジャンプする。パラメータはオフセット値。
                    break;
                }
            case GIL.CJMP:
                {
                    // スタック上の値がtrueならジャンプする。
                    break;
                }
            case GIL.NJMP:
                {
                    // falseなら。
                    break;
                }
            case GIL.TFJMP:
                {
                    // trueなら第一パラメータ, falseなら第二パラメータジャンプする。
                    break;
                }
            case GIL.PJMP:
                {
                    // スタック上の値が正ならジャンプする。
                    break;
                }
            case GIL.MJMP:
                {
                    // スタック上の値が負ならジャンプする。
                    break;
                }
            case GIL.ZJMP:
                {
                    // スタック上の値がゼロならジャンプする。
                    break;
                }
            case GIL.SEL:
                {
                    // スタック上の値から指定した名前を選択する。
                    break;
                }
            case GIL.ALLOC:
                {
                    // 指定したアドレスを指定した型で確保する。
                    break;
                }
            case GIL.ARR:
                {
                    // 指定した型の配列を作成して、スタック上に積む。
                    // スタック上の値を長さとする。
                    break;
                }
            case GIL.INST:
                {
                    // 指定したクラスのインスタンスを作成する。
                    break;
                }
            case GIL.RET:
                {
                    // スタック上の値を取り出し、呼び出し元に返却する。
                    break;
                }
            case GIL.ERR:
                {
                    // 実行時エラーをトリガーする。
                    // スタック上から エラーメッセージ => エラーコード
                    break;
                }
            // 以下即値のpush
            case GIL.PUSHSTR:
                {
                    break;
                }
            case GIL.PUSHINT:
                {
                    break;
                }
            case GIL.PUSHFLT:
                {
                    break;
                }
            case GIL.PUSHDBL:
                {
                    break;
                }
            case GIL.PUSHBOOL:
                {
                    break;
                }
            case GIL.PUSHNULL:
                {
                    break;
                }
            case GIL.MOV:
                {
                    // 指定した名前のオブジェクトをスタックに積む。
                    // (関数、クラス、変数、フィールド等)
                    break;
                }
            case GIL.CALL:
                {
                    // スタックに積まれた関数を呼び出す。
                    break;
                }
            case GIL.IDX:
                {
                    // スタックに積まれた値をインデックスとして値を取り出し、スタックに積む。
                    // スタック上で上からインデックス、取り出し元。
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
                    break;
                }
            case GIL.CPL:
                {
                    // continueで移動するラベル。
                    break;
                }
            case GIL.BPL:
                {
                    // breakで移動するラベル。
                    break;
                }
            case GIL.CONT:
                {
                    // continue
                    break;
                }
            case GIL.BRK:
                {
                    // break
                    break;
                }
            case GIL.HEAP:
                {
                    // スタック上のクラスのインスタンスにおけるi番目のフィールドをスタックに積む。
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
                    break;
                }
            case GIL.NVRETV:
                {
                    // ラインタイム実装関数から返された値をスタック上に積む。
                    break;
                }
            case GIL.CSTIF:
                {
                    // int => float のキャストを行う。
                    break;
                }
            case GIL.CSTID:
                {
                    // int => double のキャストを行う。
                    break;
                }
            case GIL.CSTFD:
                {
                    // float => double のキャストを行う。
                    break;
                }
            case GIL.INC:
                {
                    // インクリメント
                    break;
                }
            case GIL.DEC:
                {
                    // デクリメント
                    break;
                }
            case GIL.CSTOBJ:
                {
                    // スタック上のオブジェクトを目的の型にキャストする。
                    break;
                }
            case GIL.FPTR:
                {
                    // スタック上の関数のポインタを取得し、スタックに積む。
                    break;
                }
            default:
                // unknown instruction.
                return true;
        }

        return false;
    }
}