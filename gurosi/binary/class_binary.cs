using System.Text.RegularExpressions;

namespace Gurosi;

public sealed partial class ClassBinary : IBinary
{
    private TypePath _path;
    private List<AccessIdentifier> _identifiers;
    private List<FieldBinary> _fields;
    private List<FieldBinary> _stcFields;
    private List<FunctionBinary> _functions;
    private List<FunctionBinary> _stcFunctions;
    private List<ImplBinary> _absImpls;
    private int _genericCount;
    private TypePath _baseType;
    private bool _isPrototype;
    private ClassModel _prototypeSource;

    public TypePath Path
    {
        get => _path;
        set => _path = value;
    }
    public List<AccessIdentifier> Identifiers
    {
        get => _identifiers;
        set => _identifiers = value;
    }
    public List<FieldBinary> Fields
    {
        get => _fields;
        set => _fields = value;
    }
    public List<FieldBinary> StcFields
    {
        get => _stcFields;
        set => _stcFields = value;
    }
    public List<FunctionBinary> Functions
    {
        get => _functions;
        set => _functions = value;
    }

    public List<FunctionBinary> StcFunctions
    {
        get => _stcFunctions;
        set => _stcFunctions = value;
    }

    public List<ImplBinary> AbsImpls
    {
        get => _absImpls;
        set => _absImpls = value;
    }

    public int GenericCount
    {
        get => _genericCount;
        set => _genericCount = value;
    }

    public TypePath BaseType
    {
        get => _baseType;
        set => _baseType = value;
    }

    public bool IsPrototype => _isPrototype;
    public ClassModel PrototypeSource => _prototypeSource;

    public bool HasBaseType => _baseType is not null;

    public ClassBinary()
    {
        _fields = new List<FieldBinary>();
        _identifiers = new List<AccessIdentifier>();
        _functions = new List<FunctionBinary>();
        _stcFields = new List<FieldBinary>();
        _stcFunctions = new List<FunctionBinary>();
        _absImpls = new List<ImplBinary>();
        _genericCount = 0;
    }

    public bool HasFunction(string name)
    {
        for (int i = 0; i < _functions.Count; i++)
        {
            if (_functions[i].Name.TildeEquals(name))
                return true;
        }

        return false;
    }

    public bool HasStcFunction(string name)
    {
        for (int i = 0; i < _stcFunctions.Count; i++)
        {
            if (_stcFunctions[i].Name.TildeEquals(name))
                return true;
        }

        return false;
    }

    public FunctionBinary MatchStcFunction(string name, FuncExpression calling, CompileContext c)
    {
        for (int i = 0; i < _stcFunctions.Count; i++)
        {
            FunctionBinary func = _stcFunctions[i];
            
            if (func.Arguments.Count != calling.Arguments.Count)
                continue;
            if (!func.Name.TildeEquals(name))
            {
                continue;
            }

            bool invalid = false;
            for (int k = 0; k < func.Arguments.Count; k++)
            {
                TypePath argType = func.Arguments[k].Type;
                TypePath callType = TypeEvaluator.Evaluate(calling.Arguments[k], c.Runtime, c);
                if (!callType.IsCompatibleWith(argType, c.Runtime))
                {
                    invalid = true;
                    break;
                }
            }

            if (invalid)
                continue;

            return func;
        }

        return null;
    }

    public FunctionBinary GetFunctionByFixedName(string name)
    {
        for (int i = 0; i < _functions.Count; i++)
        {
            if (_functions[i].Name == name)
                return _functions[i];
        }

        return null;
    }

    public FunctionBinary GetStcFunctionByFixedName(string name)
    {
        for (int i = 0; i < _stcFunctions.Count; i++)
        {
            if (_stcFunctions[i].Name == name)
                return _stcFunctions[i];
        }

        

        return null;
    }

    public FunctionBinary MatchFunction(string name, FuncExpression calling, CompileContext c, TypePath type)
    {

        for (int i = 0; i < _functions.Count; i++)
        {
            FunctionBinary func = _functions[i];
            
            if (func.Arguments.Count != calling.Arguments.Count)
                continue;
            if (!func.Name.TildeEquals(name))
            {
                continue;
            }

            bool invalid = false;
            for (int k = 0; k < func.Arguments.Count; k++)
            {
                TypePath argType = func.Arguments[k].Type;
                argType = argType.ApplyGenerics(type);
                TypePath callType = TypeEvaluator.Evaluate(calling.Arguments[k], c.Runtime, c);
                if (!callType.IsCompatibleWith(argType, c.Runtime))
                {
                    invalid = true;
                    break;
                }
            }

            if (invalid)
                continue;

            return func;
        }

        return null;
    }

    public bool HasField(string name)
    {
        for (int i = 0; i < _fields.Count; i++)
        {
            if (_fields[i].Name == name)
                return true;
        }

        return false;
    }

    public bool HasStcField(string name)
    {
        for (int i = 0; i < _stcFields.Count; i++)
        {
            if (_stcFields[i].Name == name)
                return true;
        }

        return false;
    }

    public FieldBinary GetField(string name)
    {
        for (int i = 0; i < _fields.Count; i++)
        {
            if (_fields[i].Name == name)
                return _fields[i];
        }

        return null;
    }

    public FieldBinary GetStcField(string name)
    {
        for (int i = 0; i < _stcFields.Count; i++)
        {
            if (_stcFields[i].Name == name)
                return _stcFields[i];
        }

        return null;
    }

    public int GetFieldIndex(string name)
    {
        for (int i = 0; i < _fields.Count; i++)
        {
            if (_fields[i].Name == name)
                return i;
        }

        return -1;
    }

    public int GetStcFieldIndex(string name)
    {
        for (int i = 0; i < _stcFields.Count; i++)
        {
            if (_stcFields[i].Name == name)
                return i;
        }

        return -1;
    }

    public void Read(BinaryReader reader)
    {
        _path = new TypePath(string.Empty, string.Empty);
        _path.Read(reader);

        _identifiers = new List<AccessIdentifier>();
        int ic = reader.ReadInt32();
        for (int i = 0; i < ic; i++)
        {
            _identifiers.Add((AccessIdentifier)reader.ReadInt32());
        }

        _fields = new List<FieldBinary>();
        int fc = reader.ReadInt32();
        for (int i = 0; i < fc; i++)
        {
            _fields.Add(new FieldBinary());
            _fields[i].Read(reader);
        }

        _stcFields = new List<FieldBinary>();
        int sfc = reader.ReadInt32();
        for (int i = 0; i < sfc; i++)
        {
            _stcFields.Add(new FieldBinary());
            _stcFields[i].Read(reader);
        }

        _functions = new List<FunctionBinary>();
        int ffc = reader.ReadInt32();
        for (int i = 0; i < ffc; i++)
        {
            _functions.Add(new FunctionBinary());
            _functions[i].Read(reader);
        }

        _stcFunctions = new List<FunctionBinary>();
        int stfc = reader.ReadInt32();
        for (int i = 0; i < stfc; i++)
        {
            _stcFunctions.Add(new FunctionBinary());
            _stcFunctions[i].Read(reader);
        }

        _absImpls = new List<ImplBinary>();
        int absc = reader.ReadInt32();
        for (int i = 0; i < absc; i++)
        {
            _absImpls.Add(new ImplBinary());
            _absImpls[i].Read(reader);
        }

        _genericCount = reader.ReadInt32();
        if (reader.ReadBoolean())
        {
            _baseType = new TypePath("", "");
            _baseType.Read(reader);
        }
    }

    public void Write(BinaryWriter writer)
    {
        _path.Write(writer);

        writer.Write(_identifiers.Count);
        for (int i = 0; i < _identifiers.Count; i++)
        {
            writer.Write((int)_identifiers[i]);
        }

        writer.Write(_fields.Count);
        for (int i = 0; i < _fields.Count; i++)
        {
            _fields[i].Write(writer);
        }

        writer.Write(_stcFields.Count);
        for (int i = 0; i < _stcFields.Count; i++)
        {
            _stcFields[i].Write(writer);
        }

        writer.Write(_functions.Count);
        for (int i = 0; i < _functions.Count; i++)
        {
            _functions[i].Write(writer);
        }

        writer.Write(_stcFunctions.Count);
        for (int i = 0; i < _stcFunctions.Count; i++)
        {
            _stcFunctions[i].Write(writer);
        }

        writer.Write(_absImpls.Count);
        for (int i = 0; i < _absImpls.Count; i++)
        {
            _absImpls[i].Write(writer);
        }

        writer.Write(_genericCount);
        writer.Write(HasBaseType);
        if (HasBaseType)
            _baseType.Write(writer);
    }

    public static ClassBinary CreatePrototype(ClassModel model, RuntimeEnv runtime)
    {
        ClassBinary cb = new ClassBinary();
        cb._identifiers = model.Identifiers.ToList();
        cb._fields = model.Fields.Select(x => FieldBinary.FromModel(x, runtime)).ToList();
        cb._stcFields = model.StcFields.Select(x => FieldBinary.FromModel(x, runtime)).ToList();
        cb._path = new TypePath(model.Module, model.Name);
        cb._isPrototype = true;
        cb._prototypeSource = model;
        cb._functions = model.Methods.Select(x => FunctionBinary.CreatePrototype(x, runtime)).ToList();

        cb._functions.AddRange(model.AbsImpls.Select(x => FunctionBinary.CreateDummy(x, runtime)));

        cb._stcFunctions = model.StcMethods.Select(x => FunctionBinary.CreatePrototype(x, runtime)).ToList();
        cb._genericCount = model.GenericCount;

        if (model.BaseType is not null)
            cb._baseType = TypePath.FromModel(model.BaseType);

        return cb;
    }

    public void ResolveTypes(RuntimeEnv runtime)
    {
        if (_baseType is not null)
        {
            _baseType = runtime.Interpolate(_baseType);

            if (runtime.IsClass(_baseType))
            {
                ClassBinary source = runtime.GetClass(_baseType);

                _fields.AddRange(source.Fields);
                _functions.AddRange(source.Functions.Where(x => !x.Name.StartsWith("ctor~")));
            }
        }
    }

    public void _PrintDebug()
    {
        // print debug info.

        Console.WriteLine("type: " + _path.ToString());
        Console.WriteLine("fields:");
        for (int i = 0; i < _fields.Count; i++)
        {
            Console.WriteLine("name: " + _fields[i].Name);
            Console.WriteLine("type: " + _fields[i].Type.ToString());
            Console.WriteLine("idfr: " + string.Join(' ', _fields[i].Identifiers.Select(x => x.ToString())));
        }

    }
}