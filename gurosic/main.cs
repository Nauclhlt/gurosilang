using System.IO;
using System.Reflection;
using System.Text;
using CommandLine;
using Gurosi;
using Gurosic;
using static System.Runtime.InteropServices.JavaScript.JSType;

using CmdParser = CommandLine.Parser;
using GrError = Gurosi.Error;
using GrParser = Gurosi.Parser;

if (args.Length == 0)
{
    Println("Pass at least one argument.", ConsoleColor.Red);
    return;
}

string action = args[0].ToLower();

if (action == "lib")
{
    new CmdParser(with =>
    {
        with.CaseInsensitiveEnumValues = true;
    }).ParseArguments<LibraryBuildOptions>(args[1..])
    .WithParsed(BuildLibrary)
    .WithNotParsed(e =>
    {
    });
}
else if (action == "exe")
{
    new CmdParser(with =>
    {
        with.CaseInsensitiveEnumValues = true;
    }).ParseArguments<ExeBuildOptions>(args[1..])
    .WithParsed(BuildExecutable)
    .WithNotParsed(e =>
    {

    });
}
else
{
    Println($"Action '{action}' not found.", ConsoleColor.Red);
}

void BuildLibrary(LibraryBuildOptions options)
{
    string[] targets;

    if (File.Exists(options.Target))
    {
        targets = [options.Target];
    }
    else
    {
        SearchOption so = options.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        if (Directory.Exists(options.Target))
        {
            targets = Directory.GetFiles(options.Target, options.Wildcard, so);
        }
        else
        {
            targets = Directory.GetFiles(Path.GetDirectoryName(options.Target), Path.GetFileName(options.Target), so);
        }
    }

    SemanticCode[] codes = new SemanticCode[targets.Length];
    bool anyError = false;

    if (targets.Length == 0)
    {
        Println("No target source file found.", ConsoleColor.Yellow);
        return;
    }
    else
    {
        Println(targets.Length + " target(s) found.", ConsoleColor.Cyan);
    }

    for (int i = 0; i < targets.Length; i++)
    {
        Tokenizer lexer = new Tokenizer(File.ReadAllText(targets[i]));
        Lexical lexical = lexer.Tokenize();

        GrParser parser = new GrParser();
        Semantic semantic = parser.Parse(targets[i], lexical);

        if (semantic.Errors.Any())
        {
            anyError = true;

            Println("Syntax error(s) generated.", ConsoleColor.Red);

            foreach (GrError error in semantic.Errors)
            {
                Println(error.Message.Replace("]", $"][{error.Point.Line}, {error.Point.Column}]") + $"  ({error.FileName})", ConsoleColor.Red);
            }
            continue;
        }
        else
        {
            codes[i] = semantic.Code;
        }
    }

    if (!anyError)
    {
        Builder builder = new Builder(codes);
        List<GrError> errors = new List<GrError>();
        Library library = builder.BuildLibrary(errors);

        if (errors.Any())
        {
            Println("Build Failed.", ConsoleColor.Red);

            foreach (GrError error in errors)
            {
                Println(error.Message.Replace("]", $"][{error.Point.Line}, {error.Point.Column}]") + $"  ({error.FileName})", ConsoleColor.Red);
            }
        }
        else
        {
            string output = options.OutputFileName;
            if (Path.HasExtension(output))
            {
                output = Path.ChangeExtension(output, "grsl");
            }

            string fullPath = Path.GetFullPath(output);

            library.Write(fullPath);

            Println("Build Successful.", ConsoleColor.Green);
            Println("Output ----> '" + fullPath + "'.", ConsoleColor.Green);
        }
    }
}

void BuildExecutable(ExeBuildOptions options)
{
    string[] targets;

    if (File.Exists(options.Target))
    {
        targets = [options.Target];
    }
    else
    {
        SearchOption so = options.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        if (Directory.Exists(options.Target))
        {
            targets = Directory.GetFiles(options.Target, options.Wildcard, so);
        }
        else
        {
            targets = Directory.GetFiles(Path.GetDirectoryName(options.Target), Path.GetFileName(options.Target), so);
        }
    }

    SemanticCode[] codes = new SemanticCode[targets.Length];
    bool anyError = false;

    if (targets.Length == 0)
    {
        Println("No target source file found.", ConsoleColor.Yellow);
        return;
    }
    else
    {
        Println(targets.Length + " target(s) found.", ConsoleColor.Cyan);
    }

    for (int i = 0; i < targets.Length; i++)
    {
        Tokenizer lexer = new Tokenizer(File.ReadAllText(targets[i]));
        Lexical lexical = lexer.Tokenize();

        GrParser parser = new GrParser();
        Semantic semantic = parser.Parse(targets[i], lexical);

        if (semantic.Errors.Any())
        {
            anyError = true;

            Println("Syntax error(s) generated.", ConsoleColor.Red);

            foreach (GrError error in semantic.Errors)
            {
                Println(error.Message.Replace("]", $"][{error.Point.Line}, {error.Point.Column}]") + $"  ({error.FileName})", ConsoleColor.Red);
            }
            continue;
        }
        else
        {
            codes[i] = semantic.Code;
        }
    }

    if (!anyError)
    {
        Builder builder = new Builder(codes);
        List<GrError> errors = new List<GrError>();
        //Library library = builder.BuildLibrary(errors);
        Executable exe = builder.BuildExecutable(errors);

        if (errors.Any())
        {
            Println("Build Failed.", ConsoleColor.Red);

            foreach (GrError error in errors)
            {
                Println(error.Message.Replace("]", $"][{error.Point.Line}, {error.Point.Column}]") + $"  ({error.FileName})", ConsoleColor.Red);
            }
        }
        else
        {
            string output = options.OutputFileName;
            if (Path.HasExtension(output))
            {
                output = Path.ChangeExtension(output, "grex");
            }

            string fullPath = Path.GetFullPath(output);

            exe.Write(fullPath);

            Println("Build Successful.", ConsoleColor.Green);
            Println("Output ----> '" + fullPath + "'.", ConsoleColor.Green);
        }
    }
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