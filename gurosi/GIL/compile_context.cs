namespace Gurosi;

public sealed class CompileContext
{
    private List<CodeScope> _scopeStack = new List<CodeScope>();
    private ClassBinary _scopeClass;
    private FunctionBinary _scopeFunction;
    private RuntimeEnv _runtime;
    private int _addressIndex = 0;
    private bool _static = false;

    public bool HasClass => _scopeClass is not null;
    public bool HasFunction => _scopeFunction is not null;
    public bool CurrentlyInLoop => _scopeStack.Any(x => x.IsLoopScope);
    public bool IsRoot => _scopeStack.Count == 1;
    public RuntimeEnv Runtime => _runtime;
    public bool Static => _static;

    public int AddressIndex
    {
        get => _addressIndex;
        set => _addressIndex = value;
    }

    public ClassBinary ScopeClass => _scopeClass;
    public FunctionBinary ScopeFunction => _scopeFunction;

    private CompileContext()
    {
    }

    public void MakeFunctionContext(FunctionBinary func)
    {
        _scopeFunction = func;
        CreateNewScope();
        
        if (func.IsExtendR)
        {
            DeclareLocal(func.ExtendName, func.ExtendType, AllocAddr());
        }

        for (int i = 0; i < func.Arguments.Count; i++)
        {
            int addr = AllocAddr();
            DeclareLocal(func.Arguments[i].Name, func.Arguments[i].Type, addr);
        }
    }

    public void CreateNewLoopScope()
    {
        _scopeStack.Add(new CodeScope(true));
    }

    public void CreateNewScope()
    {
        _scopeStack.Add(new CodeScope(false));
    }

    public int AllocAddr()
    {
        int addr = _addressIndex;
        _addressIndex++;
        return addr;
    }

    public void BreakLastScope()
    {
        if (_scopeStack.Count > 1)
        {
            CodeScope scope = _scopeStack[^1];
            _scopeStack.RemoveAt(_scopeStack.Count - 1);
            _addressIndex -= scope.SymbolTable.Count;
        }
    }

    public bool IsLocalDeclared(string varName)
    {
        for (int i = _scopeStack.Count - 1; i >= 0; i--)
        {
            if (_scopeStack[i].SymbolTable.ContainsKey(varName))
                return true;
        }

        return false;
    }

    public void DeclareLocal(string varName, TypePath type, int address)
    {
        // if already declared
        if (IsLocalDeclared(varName))
            return;
        _scopeStack[^1].SymbolTable.Add(varName, new Local(type, address));
    }

    public void DeclareLocalConst(string varName, TypePath type, int address)
    {
        if (IsLocalDeclared(varName))
            return;
        _scopeStack[^1].SymbolTable.Add(varName, Local.MakeConstant(type, address));
    }

    public void UndeclareLocal(string varName)
    {
        if (!IsLocalDeclared(varName))
            return;

        _scopeStack[^1].SymbolTable.Remove(varName);
    }

    public bool TryGetLocal(string varName, out Local local)
    {
        for (int i = _scopeStack.Count - 1; i >= 0; i--)
        {
            if (_scopeStack[i].SymbolTable.TryGetValue(varName, out Local lcl))
            {
                local = lcl;
                return true;
            }
        }

        local = null;
        return false;
    }

    public Local GetLocal(string varName)
    {
        if (!IsLocalDeclared(varName))
            return null;
        
        for (int i = _scopeStack.Count - 1; i >= 0; i--)
        {
            if (_scopeStack[i].SymbolTable.TryGetValue(varName, out Local lcl))
            {
                return lcl;
            }
        }

        return null;
    }

    public bool IsFieldAccessible(ClassBinary cls, FieldBinary field)
    {
        List<AccessIdentifier> identifiers = field.Identifiers;
        AccessIdentifier main;
        bool exists = identifiers.Any(x => x != AccessIdentifier.Static);
        if (!exists)
        {
            main = AccessIdentifier.Private;
        }
        else
        {
            main = identifiers.First(x => x != AccessIdentifier.Static);
        }

        if (main == AccessIdentifier.Public)
            return true;
        else if (main == AccessIdentifier.Private)
        {
            return HasClass && ScopeClass.Path.CompareEquality(cls.Path);
        }
        else if (main == AccessIdentifier.Moduled)
        {
            return _runtime.ModuleName == cls.Path.ModuleName;
        }
        else
            return true;
    }

    public bool IsMethodAccessible(ClassBinary cls, FunctionBinary func)
    {
        List<AccessIdentifier> identifiers = func.Identifiers;
        AccessIdentifier main;
        bool exists = identifiers.Count(x => x != AccessIdentifier.Static) != 0;
        
        if (!exists)
        {
            
            main = AccessIdentifier.Private;
        }
        else
        {
            main = identifiers.First(x => x != AccessIdentifier.Static);
        }

        if (main == AccessIdentifier.Public)
            return true;
        else if (main == AccessIdentifier.Private)
        {
            return HasClass && ScopeClass.Path.CompareEquality(cls.Path);
        }
        else if (main == AccessIdentifier.Moduled)
        {
            return _runtime.ModuleName == cls.Path.ModuleName;
        }
        else
            return true;
    }

    public bool IsClassAccessible(ClassBinary cls)
    {
        List<AccessIdentifier> identifiers = cls.Identifiers;
        AccessIdentifier main;
        bool exists = identifiers.Any(x => x != AccessIdentifier.Static);
        if (!exists)
        {
            main = AccessIdentifier.Private;
        }
        else
        {
            main = identifiers.First(x => x != AccessIdentifier.Static);
        }

        if (main == AccessIdentifier.Public)
            return true;
        else if (main == AccessIdentifier.Private)
            return false;  // private class not supported.
        else if (main == AccessIdentifier.Moduled)
        {
            return cls.Path.ModuleName == _runtime.ModuleName;
        }
        else
            return true;
    }


    public static CompileContext CreateRootEmpty(RuntimeEnv runtime)
    {
        CompileContext c = new CompileContext();
        c._runtime = runtime;

        return c;
    }

    public static CompileContext CreateClassContext(RuntimeEnv runtime, ClassBinary @class)
    {
        CompileContext c = new CompileContext();
        c._scopeClass = @class;
        c._runtime = runtime;

        return c;
    }

    public static CompileContext CreateClassContextStatic(RuntimeEnv runtime, ClassBinary @class)
    {
        CompileContext c = new CompileContext();
        c._scopeClass = @class;
        c._runtime = runtime;
        c._static = true;

        return c;
    }
}