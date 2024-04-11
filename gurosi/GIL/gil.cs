namespace Gurosi;

public static class GIL
{
    public static Dictionary<string, OperandType[]> OperandMap = new Dictionary<string, OperandType[]>()
    {
        // メモリ上の値をメインスタックに積む。
        { "push", new []{ OperandType.Address } },
        // メインスタックから値をとりだし、メモリ上に入れる。
        { "pop", new[]{ OperandType.Address } },
        // メインスタックから値をとりだし、捨てる。
        { "disc", null },
        // メモリ上の値を一時スタックに積む。
        { "pushm", new[] { OperandType.Address } },
        // スタック上の値を一時スタックに積む。
        { "pushs", null },
        // 一時スタックから値をとりだし、メモリ上に入れる。
        { "popm", new[] { OperandType.Address } },
        // 一時スタックから値をとりだし、メインスタックに積む。
        { "pops", null },
        // メインスタック上から値をとりだし、その下に積まれたクラスのインスタンスにおけるi番目のフィールドに格納する。
        { "popf", new[] { OperandType.Int } },
        // メインスタック上から値をとりだし、その下に積まれた配列の指定インデックスに格納する。
        // スタック上から インデックス => 配列 => 値。
        { "popidx", null },
        // メインスタック上から値をとりだし、その下に積まれた名前におけるi番目の静的変数に値を格納する。
        { "popstc", new[] { OperandType.Int } },
        // メインスタック上の値を加算する。スタックの上から(右辺)=>(左辺)
        { "add", null },
        // 減算。
        { "sub", null },
        // 乗算
        { "mul", null },
        // 除算
        { "div", null },
        // 剰余
        { "mod", null },
        // <
        { "lt", null },
        // <=
        { "lte", null },
        // >
        { "gt", null },
        // >=
        { "gte", null },
        // ==
        { "eq", null },
        // !=
        { "neq", null },
        // AND演算。
        { "and", null },
        // OR演算
        { "or", null },
        // XOR演算
        { "xor", null },
        // 大小比較。
        { "comp", null },
        // 左シフト。
        { "shl", null },
        // 右シフト
        { "shr", null },
        // 命令を相対的にジャンプする。パラメータはオフセット値。
        { "jmp", new[] { OperandType.Int } },
        // スタック上の値がtrueならジャンプする。
        { "cjmp", new[] { OperandType.Int } },
        // falseなら。
        { "njmp", new[] { OperandType.Int } },
        // trueなら第一パラメータ, falseなら第二パラメータジャンプする。
        { "tfjmp", new[] { OperandType.Int, OperandType.Int } },
        // スタック上の値が正ならジャンプする。
        { "pjmp", new[]{ OperandType.Int } },
        // スタック上の値が負ならジャンプする。
        { "mjmp", new[] { OperandType.Int } },
        // スタック上の値がゼロならジャンプする。
        { "zjmp", new[] { OperandType.Int } },
        // スタック上の値から指定した名前を選択する。
        { "sel", new[] { OperandType.String } },
        // 指定したアドレスを指定した型で確保する。
        { "alloc", new[] { OperandType.Address, OperandType.String } },
        // 指定した型の配列を作成して、スタック上に積む。
        // スタック上の値を長さとする。
        { "arr", new[] { OperandType.String } },
        // 指定したクラスのインスタンスを作成する。
        { "inst", new[] { OperandType.String } },
        // スタック上の値を取り出し、呼び出し元に返却する。
        { "ret", null },
        // 実行時エラーをトリガーする。
        // スタック上から エラーメッセージ => エラーコード
        { "err", null },
        // 文字列即値をスタックに積む。
        { "pushstr", new[] { OperandType.String } },
        { "pushint", new[] { OperandType.Int } },
        { "pushflt", new[] { OperandType.Float } },
        { "pushdbl", new[] { OperandType.Double } },
        { "pushbool", new[] { OperandType.Boolean } },
        { "pushnull", null },
        // 指定した名前のオブジェクトをスタックに積む。
        // (関数、クラス、変数、フィールド等)
        { "mov", new[] { OperandType.String } },
        // スタックに積まれた関数を呼び出す。
        { "call", null },
        // スタックに積まれた値をインデックスとして値を取り出し、スタックに積む。
        // スタック上で上からインデックス、取り出し元。
        { "idx", null },
        // 代入をする。スタック上で上から右辺, 左辺。
        { "set", null },
        // 現在実行されているクラスのインスタンスをスタック上に積む。
        { "self", null },
        // continueで移動するラベル。
        { "cpl", null },
        // breakで移動するラベル。
        { "bpl", null },
        // continue
        { "cont", null },
        // break
        { "brk", null },
        // スタック上のクラスのインスタンスにおけるi番目のフィールドをスタックに積む。
        { "heap", new[]{ OperandType.Int } },
        // スタック上のクラスにおけるi番目の静的フィールドをスタックに積む。
        { "hpstc", new[] { OperandType.Int } },
        // ランタイム実装関数を検索呼び出し。
        { "nvc", new []{ OperandType.String, OperandType.String, OperandType.String } },
        // ラインタイム実装関数から返された値をスタック上に積む。
        { "nvretv", null },
        // int => float のキャストを行う。
        { "cstif", null },
        // int => double のキャストを行う。
        { "cstid", null },
        // float => double のキャストを行う。
        { "cstfd", null },
        { "inc", null },
        { "dec", null },
        // スタック上のオブジェクトを目的の型にキャストする。
        { "cstobj", new[] {OperandType.String} },
        // スタック上の関数のポインタを取得し、スタックに積む。
        { "fptr", null },
    };

    public const byte PUSH = 0;
    public const byte POP = 1;
    public const byte PUSHM = 2;
    public const byte PUSHS = 3;
    public const byte POPM = 4;
    public const byte POPS = 5;
    public const byte POPF = 6;
    public const byte POPSTC = 7;
    public const byte ADD = 8;
    public const byte SUB = 9;
    public const byte MUL = 10;
    public const byte DIV = 11;
    public const byte MOD = 12;
    public const byte LT = 13;
    public const byte LTE = 14;
    public const byte GT = 15;
    public const byte GTE = 16;
    public const byte EQ = 17;
    public const byte NEQ = 18;
    public const byte AND = 19;
    public const byte OR = 20;
    public const byte XOR = 21;
    public const byte COMP = 22;
    public const byte SHL = 23;
    public const byte SHR = 24;
    public const byte JMP = 25;
    public const byte CJMP = 26;
    public const byte PJMP = 27;
    public const byte MJMP = 28;
    public const byte ZJMP = 29;
    public const byte SEL = 30;
    public const byte ALLOC = 31;
    public const byte ARR = 32;
    public const byte INST = 33;
    public const byte RET = 34;
    public const byte ERR = 35;
    public const byte PUSHSTR = 36;
    public const byte PUSHINT = 37;
    public const byte PUSHFLT = 38;
    public const byte PUSHDBL = 39;
    public const byte PUSHBOOL = 40;
    public const byte PUSHNULL = 41;
    public const byte MOV = 42;
    public const byte CALL = 43;
    public const byte IDX = 44;
    public const byte SET = 45;
    public const byte CPL = 46;
    public const byte BPL = 47;
    public const byte HEAP = 48;
    public const byte HPSTC = 49;
    public const byte NVC = 50;
    public const byte NVRETV = 51;
    public const byte CSTIF = 52;
    public const byte CSTID = 53;
    public const byte CSTFD = 54;
    public const byte INC = 55;
    public const byte DEC = 56;
    public const byte CSTOBJ = 57;
    public const byte FPTR = 58;
    public const byte DISC = 59;
    public const byte POPIDX = 60;
    public const byte NJMP = 61;
    public const byte TFJMP = 62;
    public const byte SELF = 63;
    public const byte CONT = 64;
    public const byte BRK = 65;

    // 順番を変えると動かない。
    public static string[] CodeMap = new string[]{
        "push",
        "pop",
        "pushm", 
        "pushs",
        "popm",
        "pops",
        "popf",
        "popstc",
        "add",
        "sub",
        "mul",
        "div",
        "mod",
        "lt",
        "lte",
        "gt",
        "gte",
        "eq",
        "neq",
        "and",
        "or",
        "xor",
        "comp",
        "shl",
        "shr",
        "jmp",
        "cjmp",
        "pjmp",
        "mjmp",
        "zjmp",
        "sel",
        "alloc",
        "arr",
        "inst",
        "ret",
        "err",
        "pushstr",
        "pushint",
        "pushflt",
        "pushdbl",
        "pushbool",
        "pushnull",
        "mov",
        "call",
        "idx",
        "set",
        "cpl",
        "bpl",
        "heap",
        "hpstc",
        "nvc",
        "nvretv",
        "cstif",
        "cstid",
        "cstfd",
        "inc",
        "dec",
        "cstobj",
        "fptr",
        "disc",
        "popidx",
        "njmp",
        "tfjmp",
        "self",
        "cont",
        "brk"
    };

    public static int CountInstructions(List<string> code, int start, int length)
    {
        int count = 0;

        for (int i = start; i < start + length; i++)
        {
            if (GIL.OperandMap.ContainsKey(code[i]))
            {
                count++;
            }
        }

        return count;
    }
}