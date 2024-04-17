namespace Gurosi;

// RuntimeEnv: あるファイルをコンパイルする際の環境を保持。
// Library: あるライブラリのインスタンスを保持。
// RuntimeEnv << Library+
public sealed class Builder
{
    private SemanticCode[] _codes;

    public Builder(SemanticCode[] codes)
    {
        _codes = codes;

        ResolveGlblFuncConflicts();
    }

    private void ResolveGlblFuncConflicts()
    {
        FunctionIndexer indexer = new FunctionIndexer();
        
        foreach (var code in _codes)
        {
            for (int i = 0; i < code.GlobalFunctions.Count; i++)
            {
                Function func = code.GlobalFunctions[i];
                string rawName = func.Name.Substring(0, func.Name.IndexOf('~'));
                func.Name = rawName + "~" + indexer.GetIndex(rawName);
            }
        }
    }

    public Executable BuildExecutable(List<Error> errors)
    {
        Library self = BuildLibrary(errors);

        if (self is null)
            return null;

        Compiler compiler = new Compiler();
        CodeBinary code = null;

        if (_codes.Count(x => x.RunBlock is not null) > 1)
        {
            errors.Add(new Error(ErrorProvider.RunBlockDuplicate(), new TextPoint(0, 0)));
            return null;
        }
        if (!_codes.Any(x => x.RunBlock is not null))
        {
            errors.Add(new Error(ErrorProvider.RunBlockMissing(), new TextPoint(0, 0)));
            return null;
        }
        
        for (int i = 0; i < _codes.Length; i++)
        {
            SemanticCode c = _codes[i];

            RuntimeEnv runtime = new RuntimeEnv()
            {
                ModuleName = c.ModuleName
            };
           
            for (int j = 0; j < c.Imports.Count; j++)
            {
                Library import = ImportLib(c.Imports[j]);
                
                // add dependencies

                if (import is not null)
                {
                    runtime.Types.AddRange(import.Classes.Select(x => x.Path));
                    runtime.Classes.AddRange(import.Classes);
                    runtime.Functions.AddRange(import.Functions);
                }
            }

            runtime.Shortens.AddRange(c.Shortens);

            LoadSelfContained(runtime, _codes);

            if (c.RunBlock is not null)
            {
                CompileResult result = compiler.CompileRunBlock(c.RunBlock, runtime);

                Error.AttachFileName(c.FileName, result.Errors);

                errors.AddRange(result.Errors);
                code = new CodeBinary(result.Code);
            }
        }

        if (errors.Count > 0)
            return null;

        Executable exe = new Executable(code, self.Imported, self);

        exe.MemorySize = int.Max(exe.MemorySize, _codes.Max(x => x.MemSize));

        return exe;
    }

    public Library BuildLibrary(List<Error> errors)
    {
        Library lib = new Library();

        Compiler compiler = new Compiler();

        for (int i = 0; i < _codes.Length; i++)
        {
            SemanticCode c = _codes[i];
            RuntimeEnv runtime = new RuntimeEnv()
            {
                ModuleName = c.ModuleName
            };

            // add imports
            lib.AddImports(c.Imports);

            
            for (int j = 0; j < c.Imports.Count; j++)
            {
                Library import = ImportLib(c.Imports[j]);
                // add dependencies
                if (import is not null)
                {
                    lib.AddImports(import.Imported);

                    runtime.Types.AddRange(import.Classes.Select(x => x.Path));
                    runtime.Classes.AddRange(import.Classes);
                    runtime.Functions.AddRange(import.Functions);
                }
                else
                {
                    errors.Add(new Error(ErrorProvider.ImportNotFound(c.Imports[j]), default));
                    errors[^1].FileName = c.FileName;
                    return null;
                }
            }

            runtime.Shortens.AddRange(c.Shortens);

            LoadSelfContained(runtime, _codes);

            

            // Build process start.

            for (int j = 0; j < c.Classes.Count; j++)
            {
                //lib.Classes.Add(ClassBinary.FromModel(c.Classes[j], runtime));
                CompileResult result  = compiler.CompileClass(c.Classes[j], runtime);
                if (!result.HasError)
                {
                    lib.Classes.Add(result.Class);
                }
                else
                {
                    Error.AttachFileName(c.FileName, result.Errors);

                    errors.AddRange(result.Errors);
                }
            }

            for (int j = 0; j < c.GlobalFunctions.Count; j++)
            {
                CompileResult result = compiler.CompileFunction(c.GlobalFunctions[j], runtime);

                

                if (!result.HasError)
                {
                    lib.Functions.Add(result.Function);
                }
                else
                {
                    Error.AttachFileName(c.FileName, result.Errors);

                    errors.AddRange(result.Errors);
                }
            }
        }

        if (errors.Count > 0)
            return null;

        return lib;
    }

    private void LoadSelfContained(RuntimeEnv runtime, SemanticCode[] codes)
    {
        for (int i = 0; i < codes.Length; i++)
        {
            SemanticCode code = _codes[i];
            runtime.Types.AddRange(code.Classes.Select(x => new TypePath(x.Module, x.Name)));
        }

        for (int i = 0; i < codes.Length; i++)
        {
            SemanticCode code = _codes[i];
            runtime.Classes.AddRange(code.Classes.Select(x => ClassBinary.CreatePrototype(x, runtime)));
            runtime.Functions.AddRange(code.GlobalFunctions.Select(x => FunctionBinary.CreatePrototype(x, runtime)));
        }
        
        for (int i = 0; i < runtime.Classes.Count; i++)
        {
            runtime.Classes[i].ResolveTypes(runtime);
        }
    }

    private Library ImportLib(string source)
    {
        if (File.Exists(source))
        {
            return Library.Load(source);
        }

        return null;
    }
}