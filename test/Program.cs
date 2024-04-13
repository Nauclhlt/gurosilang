using Gurosi;

string sourceCode = File.ReadAllText(@"C:\workspace\CSharp\VsProjects\gurosilang\gurosi\sample\sample.gr");
Tokenizer lexer = new Tokenizer(sourceCode);
Lexical lexical = lexer.Tokenize();

Parser parser = new Parser();
Semantic semantic = parser.Parse("sample.gr", lexical);

if (semantic.Errors.Any())
{
    foreach (var error in semantic.Errors)
    {
        Console.WriteLine(error.GetFormatted());
    }
    Console.ReadLine();
}
else
{
    SemanticCode code = semantic.Code;
    Builder builder = new Builder([code]);
    List<Error> errors = new List<Error>();
    Executable exe = builder.BuildExecutable(errors);
    if (errors.Any())
    {
        foreach (var error in errors)
        {
            Console.WriteLine(error.GetFormatted());
        }
        Console.ReadLine();
    }
    else
    {
        //exe._PrintDebug();

        ProgramRuntime runtime = new ProgramRuntime(exe);

        runtime.Execute();

        Console.ReadLine();
    }
}