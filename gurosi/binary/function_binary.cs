using System.Reflection;

namespace Gurosi;

public sealed class FunctionBinary : IBinary
{
    private string _module;
    private string _name;
    private List<AccessIdentifier> _identifiers;
    private TypePath _returnType;
    private List<ArgumentBinary> _arguments;
    private TypePath _extendType;
    private string _extendName;
    private CodeBinary _body;
    private AttributeModel _attributes;

    public string Module
    {
        get => _module;
        set => _module = value;
    }

    public string Name
    {
        get => _name;
        set => _name = value;
    }
    public TypePath ReturnType
    {
        get => _returnType;
        set => _returnType = value;
    }
    public List<AccessIdentifier> Identifiers
    {
        get => _identifiers;
        set => _identifiers = value;
    }
    public List<ArgumentBinary> Arguments
    {
        get => _arguments;
        set => _arguments = value;
    }
    public TypePath ExtendType
    {
        get => _extendType;
        set => _extendType = value;
    }
    public string ExtendName
    {
        get => _extendName;
        set => _extendName = value;
    }
    public CodeBinary Body
    {
        get => _body;
        set => _body = value;
    }
    public AttributeModel Attributes
    {
        get => _attributes;
        set => _attributes = value;
    }

    public bool ReturnsValueR => !_returnType.CompareEquality(TypePath.NULL);
    public bool IsExtendR => _extendType is not null;

    public FunctionBinary()
    {
        _arguments = new List<ArgumentBinary>();
        _identifiers = new List<AccessIdentifier>();
    }

    public void Read(BinaryReader reader)
    {
        _module = reader.ReadString();
        _name = reader.ReadString();
        _returnType = new TypePath(string.Empty, string.Empty);
        _returnType.Read(reader);

        _identifiers = new List<AccessIdentifier>();
        int ic = reader.ReadInt32();
        for (int i = 0; i < ic; i++)
        {
            _identifiers.Add((AccessIdentifier)reader.ReadInt32());
        }

        _arguments = new List<ArgumentBinary>();
        int ac = reader.ReadInt32();
        for (int i = 0; i < ac; i++)
        {
            _arguments.Add(new ArgumentBinary());
            _arguments[i].Read(reader);
        }

        bool ex = reader.ReadBoolean();
        if (ex)
        {
            _extendType = new TypePath(string.Empty, string.Empty);
            _extendType.Read(reader);
            _extendName = reader.ReadString();
        }

        _body = new CodeBinary();
        if (reader.ReadBoolean())
            _body.Read(reader);

        _attributes = (AttributeModel)reader.ReadInt32();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(_module);
        writer.Write(_name);
        _returnType.Write(writer);
        writer.Write(_identifiers.Count);
        for (int i = 0; i < _identifiers.Count; i++)
        {
            writer.Write((int)_identifiers[i]);
        }
        writer.Write(_arguments.Count);
        for (int i = 0; i < _arguments.Count; i++)
        {
            _arguments[i].Write(writer);
        }
        writer.Write(_extendType is not null);
        if (_extendType is not null)
        {
            _extendType.Write(writer);
            writer.Write(_extendName);
        }

        writer.Write(_body is not null);
        if (_body is not null)
            _body.Write(writer);

        writer.Write((int)_attributes);
    }

    public static FunctionBinary CreatePrototype(MethodModel model, RuntimeEnv runtime)
    {
        FunctionBinary fb = new FunctionBinary();
        fb._identifiers = model.AccessIdentifiers.ToList();
        fb._arguments = model.Parameters.Select(x => ArgumentBinary.FromModel(x, runtime)).ToList();
        fb._name = model.Name;
        fb._returnType = runtime.Interpolate(TypePath.FromModel(model.ReturnType));
        fb._attributes = model.Attributes;
        return fb;
    }

    public static FunctionBinary CreateDummy(ImplBinary impl, ClassBinary cls, RuntimeEnv runtime)
    {
        FunctionBinary fb = new FunctionBinary();
        fb._module = cls.Path.ModuleName;
        fb._identifiers = impl.Identifiers.ToList();
        fb._arguments = impl.Arguments.ToList();
        fb._name = impl.Name;
        fb._returnType = impl.ReturnType;
        fb._attributes = 0;

        return fb;
    }

    public static FunctionBinary CreateDummy(ImplModel model, RuntimeEnv runtime)
    {
        FunctionBinary fb = new FunctionBinary();
        fb._identifiers = model.AccessIdentifiers.ToList();
        fb._arguments = model.Parameters.Select(x => ArgumentBinary.FromModel(x, runtime)).ToList();
        fb._name = model.Name;
        fb._returnType = runtime.Interpolate(TypePath.FromModel(model.ReturnType));

        return fb;
    }

    public static FunctionBinary CreatePrototype(Function model, RuntimeEnv runtime)
    {
        FunctionBinary fb = new FunctionBinary();
        fb._module = model.Module;
        fb._identifiers = new List<AccessIdentifier>();
        fb._arguments = model.Parameters.Select(x => ArgumentBinary.FromModel(x, runtime)).ToList();
        fb._name = model.Name;
        fb._returnType = runtime.Interpolate(TypePath.FromModel(model.ReturnType));
        if (model.ExtendType is not null)
        {
            fb._extendType = runtime.Interpolate(TypePath.FromModel(model.ExtendType));
            fb._extendName = model.ExtendName;
        }
        return fb;
    }
}