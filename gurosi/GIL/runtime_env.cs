using System.Text.RegularExpressions;

namespace Gurosi;

public sealed partial class RuntimeEnv
{
    private List<ClassBinary> _classes;
    private List<TypePath> _types;
    private List<FunctionBinary> _functions;
    private List<string> _shortens;
    private string _moduleName;

    public List<ClassBinary> Classes => _classes;
    public List<TypePath> Types => _types;
    public List<FunctionBinary> Functions => _functions;
    public List<string> Shortens => _shortens;
    public string ModuleName
    {
        get => _moduleName;
        set => _moduleName = value;
    }

    public RuntimeEnv()
    {
        _classes = new List<ClassBinary>();
        _functions = new List<FunctionBinary>();
        _shortens = new List<string>();
        _types = new List<TypePath>();
    }

    public RuntimeEnv(IEnumerable<Library> libs)
    {
        foreach (Library lib in libs)
        {
            _classes.AddRange(lib.Classes);
            _functions.AddRange(lib.Functions);
        }
    }

    public bool ModuleExists(string moduleName)
    {
        for (int i = 0; i < _functions.Count; i++)
        {
            if (_functions[i].Module == moduleName)
                return true;
        }

        for (int i = 0; i < _classes.Count; i++)
        {
            if (_classes[i].Path.Route[0] == moduleName)
                return true;
        }

        return false;
    }

    public List<string> ExtractModules()
    {
        HashSet<string> modules = new HashSet<string>();

        for (int i = 0; i < _functions.Count; i++)
        {
            if (!modules.Contains(_functions[i].Module))
            {
                modules.Add(_functions[i].Module);
            }
        }

        for (int i = 0; i < _classes.Count; i++)
        {
            string name = _classes[i].Path.ModuleName;
            if (!modules.Contains(name))
            {
                modules.Add(name);
            }
        }

        return modules.ToList();
    }

    public bool FuncExists(string moduleName, string name)
    {
        for (int i = 0; i < _functions.Count; i++)
        {
            if (_functions[i].Module == moduleName &&
                _functions[i].Name.Replace(name, string.Empty).StartsWith('~'))
            {
                return true;
            }
        }

        return false;
    }

    public bool ExtendFuncExists(TypePath type, string name, RuntimeEnv runtime)
    {
        for (int i = 0; i < _functions.Count; i++)
        {
            if (_functions[i].IsExtendR && type.IsCompatibleWith(_functions[i].ExtendType, runtime) &&
                _functions[i].Name.Replace(name, string.Empty).StartsWith('~'))
            {
                return true;
            }
        }

        return false;
    }

    public FunctionBinary MatchExtendFunction(TypePath targetType, string name, FuncExpression calling, CompileContext c)
    {
        for (int i = 0; i < _functions.Count; i++)
        {
            FunctionBinary func = _functions[i];

            if (!func.IsExtendR)
                continue;
            if (func.Arguments.Count != calling.Arguments.Count)
                continue;
            if (!func.Name.Replace(name, string.Empty).StartsWith('~'))
            {
                continue;
            }
            for (int k = 0; k < func.Arguments.Count; k++)
            {
                TypePath argType = func.Arguments[k].Type;
                TypePath callType = TypeEvaluator.Evaluate(calling.Arguments[k], c.Runtime, c);
                if (!callType.IsCompatibleWith(argType, this))
                {
                    continue;
                }
            }
            if (!targetType.IsCompatibleWith(func.ExtendType, c.Runtime))
                continue;

            return func;
        }

        return null;
    }

    public bool HasFunction(string moduleName, string name)
    {
        for (int i = 0; i < _functions.Count; i++)
        {
            FunctionBinary func = _functions[i];
            
            if (func.IsExtendR)
                continue;
            if (func.Module != moduleName ||
                !func.Name.Replace(name, string.Empty).StartsWith('~'))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    public FunctionBinary MatchFunction(string moduleName, string name, FuncExpression calling, CompileContext c)
    {
        for (int i = 0; i < _functions.Count; i++)
        {
            FunctionBinary func = _functions[i];
            
            if (func.Arguments.Count != calling.Arguments.Count ||
                func.IsExtendR)
                continue;
            if (func.Module != moduleName ||
                !func.Name.Replace(name, string.Empty).StartsWith('~'))
            {
                continue;
            }
            for (int k = 0; k < func.Arguments.Count; k++)
            {
                TypePath argType = func.Arguments[k].Type;
                TypePath callType = TypeEvaluator.Evaluate(calling.Arguments[k], c.Runtime, c);
                if (!callType.IsCompatibleWith(argType, this))
                {
                    continue;
                }
            }

            return func;
        }

        return null;
    }

    public string InterpolateGlblFuncModule(string name)
    {
        for (int i = 0; i < _shortens.Count; i++)
        {
            string module = _shortens[i];
            if (FuncExists(module, name))
            {
                return module;
            }
        }

        return null;
    }

    public TypePath GetWithGeneric(TypePath sourceType, TypePath type)
    {
        if (type.ModuleName == "gen")
        {
            return sourceType.Generics[type.GenericParamIndex];
        }

        return type;
    }

    public TypePath Interpolate(TypePath path)
    {
        if (path is null)
            return null;

        if (path.Route.Length == 0 || path.ModuleName == string.Empty)
        {
            // need to interpolate.
            var modules = _shortens;

            TypePath lastValid = path;
            for (int i = 0; i < modules.Count; i++)
            {
                TypePath candidate = new TypePath(modules[i], path.Name);
                if (_types.Any(x => x.CompareEquality(candidate)))
                {
                    if (!lastValid.CompareEquality(path))
                    {
                        return new TypePath("unknown", path.Name);
                    }
                    else
                    {
                        candidate.Generics.AddRange(path.Generics);
                        lastValid = candidate;
                    }
                }
                    
            }

            return lastValid;
        }
        else
        {
            return path;
        }
    }

    public ClassBinary GetClass(TypePath path)
    {
        for (int i = 0; i < _classes.Count; i++)
        {
            if (_classes[i].Path.CompareEquality(path))
            {
                return _classes[i];
            }
        }

        return null;
    }

    public bool IsClass(TypePath path)
    {
        for (int i = 0; i < _classes.Count; i++)
        {
            if (_classes[i].Path.CompareEquality(path))
                return true;
        }

        return false;
    }

    public bool TypeExists(TypePath path)
    {
        if (path.CompareEquality(TypePath.UNKNOWN))
            return false;

        if (path.Route.Length == 1 && path.Route[0] == "sys")
        {
            if (path.Name != "null")
                return true;
        }

        if (path.Route.Length == 1 && path.Route[0] == "arr")
        {
            TypePath innerType = path.GetArrayType();
            return TypeExists(innerType);
        }

        if (path.Route.Length == 1 && path.Route[0] == "gen")
        {
            return true;
        }

        for (int i = 0; i < _classes.Count; i++)
        {
            if (_classes[i].Path.CompareEquality(path))
                return true;
        }

        return false;
    }
}