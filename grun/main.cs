using System.IO;
using System.Linq;
using Gurosi;

if (args.Length == 0)
{
    Println("Specify the target to run.", ConsoleColor.Red);
    return;
}

string target = args[0];

if (!File.Exists(target))
{
    Println($"File '{target}' not found.", ConsoleColor.Red);
    return;
}

if (Path.GetExtension(target) != ".grex")
{
    Println($"File '{target}' is not valid executable file.", ConsoleColor.Red);
    return;
}

Executable exe;
ProgramRuntime runtime;

try
{
    exe = Executable.Load(target);

    runtime = new ProgramRuntime(exe);
}
catch (Exception)
{
    Println("Error loading the executable file.", ConsoleColor.Red);
    return;
}

runtime.Execute();

if (runtime.HasRuntimeError)
{
    Println(">> Runtime Error Occurred", ConsoleColor.Red);
    Println(runtime.RuntimeError.ErrorCode + ": " + runtime.RuntimeError.Message, ConsoleColor.Red);
}

static void Print(string msg, ConsoleColor color)
{
    ConsoleColor push = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.Write(msg);
    Console.ForegroundColor = push;
}

static void Println(string msg, ConsoleColor color)
{
    ConsoleColor push = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine(msg);
    Console.ForegroundColor = push;
}