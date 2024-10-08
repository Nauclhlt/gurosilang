namespace Gurosi;

// Represents a type by path.
// Primitive types:
//                  _module = "sys", // <= primitives "arr" // <= Array types
//                  _name = "int", "string", "float", "double", "boolean", "<module;name>" // <= Element type of Array types
public sealed class TypePath {
    private string[] _route;
    private string _name;
    private List<TypePath> _generics;

    public string[] Route => _route;
    public string Name => _name;
    
    public bool IsArray => ModuleName == "arr";

    public bool IsGenericParam => ModuleName == "gen";
    public int GenericParamIndex => int.Parse(_name);

    public List<TypePath> Generics => _generics;

    public bool IsModule => _name == string.Empty && _route.Length == 1;
    // gets module name. does not return the name of the module this type belongs to.
    public string ModuleName
    {
        get {
            if (_route.Length == 0)
                return string.Empty;
            else
                return _route[0];
        }
    }

    private TypePath()
    {
        _generics = [];
    }

    public TypePath(string module, string name)
    {
        _route = [ module ];
        _name = name;
        _generics = [];
    }

    public TypePath(string[] route, string name)
    {
        _route = route;
        _name = name;
        _generics = [];
    }

    public TypePath(string module)
    {
        _route = [ module ];
        _name = string.Empty;
        _generics = [];
    }

    public static readonly TypePath NULL = new TypePath("sys", "null");
    public static readonly TypePath UNKNOWN = new TypePath("sys", "unknown");
    public static readonly TypePath STRING = new TypePath("sys", "string");
    public static readonly TypePath INT = new TypePath("sys", "int");
    public static readonly TypePath FLOAT = new TypePath("sys", "float");
    public static readonly TypePath DOUBLE = new TypePath("sys", "double");
    public static readonly TypePath BOOLEAN = new TypePath("sys", "boolean");
    public static readonly TypePath ANY = new TypePath("sys", "any");
    public static readonly TypePath FUNC_PTR = new TypePath("sys", "funcptr");

    public static TypePath FromModel(TypeData type)
    {
        if (type.Kind == TypeKind.BuiltIn)
        {
            return type.BuiltIn switch
            {
                0 => NULL,
                1 => STRING,
                2 => INT,
                3 => FLOAT,
                4 => DOUBLE,
                5 => BOOLEAN,
                6 => ANY,
                7 => FUNC_PTR,
                _ => UNKNOWN
            };
        }
        else if (type.Kind == TypeKind.Symbol)
        {
            Expression symbol = type.Symbol;
            List<TypePath> generics = [];

            if (symbol is GenericExpression generic)
            {
                symbol = generic.Source;
                generics = generic.Types.Select(FromModel).ToList();
            }

            string path = symbol.GetPath();
            if (path.Contains("~~~"))
                return null;

            string[] split = path.Split(';');
            TypePath result = new TypePath(split[0..^1], split[^1]);
            result.Generics.AddRange(generics);
            return result;
        }
        else if (type.Kind == TypeKind.Array)
        {
            return new TypePath("arr", FromModel(type.ArrayType).ToString());
        }
        else if (type.Kind == TypeKind.ArrayBase)
        {
            return new TypePath("sys", "arrbase");
        }
        else if (type.Kind == TypeKind.Generic)
        {
            return new TypePath("gen", type.GenericParamIndex.ToString());
        }

        return null;
    }

    public TypePath CloneAsArray()
    {
        TypePath clone = new TypePath("arr", this.ToString());
        return clone;
    }

    public TypePath GetArrayType()
    {
        if (IsArray)
            return FromString(_name);
        else
            return TypePath.UNKNOWN;
    }

    public TypePath WithoutGenerics()
    {
        return new TypePath(_route, _name);
    }

    public TypePath ApplyGenerics(TypePath source)
    {
        if (!IsGenericParam)
            return this;
        if (source.Generics.Count == 0)
            return this;

        return source.Generics[GenericParamIndex];
    }

    public bool NotNull()
    {
        return !CompareEquality(UNKNOWN);
    }

    public override string ToString()
    {
        string s = $"{string.Join('.', _route)}::{_name}";
        if (_generics.Count > 0)
        {
            s += "^{" + string.Join(';', _generics) + "}";
        }
        return s;
    }

    public static TypePath FromString(string source)
    {
        TypePath t = new TypePath();
        string[] split = source.Split("::", 2);
        string[] rightSplit = split[^1].Split('^', 2);
        t._name = rightSplit[0];
        t._route = split[0..^1];
        if (rightSplit.Length > 1)
        {
            char[] ca = rightSplit[1].Trim('{').TrimEnd('}').ToCharArray();
            int level = 0;
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < ca.Length; i++)
            {
                if (ca[i] == '{')
                {
                    level++;
                }
                if (ca[i] == '}')
                {
                    level--;
                }

                if (level == 0 && ca[i] == ';')
                {
                    string buf = buffer.ToString();
                    buffer.Clear();
                    t._generics.Add(TypePath.FromString(buf));
                    continue;
                }

                buffer.Append(ca[i]);
            }

            t._generics.Add(TypePath.FromString(buffer.ToString()));
        }
        
        return t;
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(_route.Length);
        for (int i = 0; i < _route.Length; i++)
        {
            writer.Write(_route[i]);
        }
        writer.Write(_name);
    }

    public void Read(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        _route = new string[count];
        for (int i = 0; i < count; i++)
        {
            _route[i] = reader.ReadString();
        }
        _name = reader.ReadString();
    }

    public bool CompareEquality(TypePath t)
    {
        if (IsArray)
        {
            if (!t.IsArray)
                return false;
            
            return GetArrayType().CompareEquality(t.GetArrayType());
        }

        if (_name != t.Name)
            return false;

        if (this.Generics?.Count > 0)
        {
            if (!_route.SequenceEqual(t.Route))
            {
                return false;
            }
            
            if (Generics.Count != t.Generics?.Count)
            {
                return false;
            }

            for (int i = 0; i < Generics.Count; i++)
            {
                if (!Generics[i].CompareEquality(t.Generics[i]))
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            return _route.SequenceEqual(t.Route);
        }
    }

    public bool IsCompatibleWith(TypePath t, RuntimeEnv runtime, bool casting = true)
    {
        return IsCompatibleWithInternal(t, type => runtime.GetClass(type), casting);
    }

    public bool IsCompatibleWith(TypePath t, SymbolStore symbols, bool casting =  true)
    {
        return IsCompatibleWithInternal(t, type => symbols.FindClass(type), casting);
    }

    private bool IsCompatibleWithInternal(TypePath t, Func<TypePath, ClassBinary> getClass, bool casting)
    {
        if (CompareEquality(t))
            return true;

        if (t.ModuleName == "sys" && t.Name == "arrbase" && this.IsArray)
            return true;

        if (t.CompareEquality(ANY))  // any can bind any types.
        {
            return true;
        }

        if (CompareEquality(NULL))
        {
            return true;  // any type can be null.
        }

        ClassBinary cls = getClass(this);
        if (cls is not null)
        {
            if (cls.HasBaseType && cls.BaseType.IsCompatibleWithInternal(t, getClass, casting))
            {
                return true;
            }
        }

        if (casting && TypeEvaluator.ImplicitCastMap.Contains((this, t)))
        {
            return true;
        }

        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;

        if (obj is not TypePath)
            return false;

        return CompareEquality(obj as TypePath);
    }

    public override int GetHashCode()
    {
        return _name.GetHashCode() + _route.Select(x => x.GetHashCode()).Sum();
    }
}