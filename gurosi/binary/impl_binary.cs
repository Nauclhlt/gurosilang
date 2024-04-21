namespace Gurosi;

public sealed class ImplBinary : IBinary
{
    private string _module;
    private string _name;
    private List<AccessIdentifier> _identifiers;
    private TypePath _returnType;
    private List<ArgumentBinary> _arguments;

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

    public bool ReturnsValueR => !_returnType.CompareEquality(TypePath.NULL);

    public ImplBinary()
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
    }

    public static ImplBinary CreatePrototype(ImplModel model, RuntimeEnv runtime)
    {
        ImplBinary fb = new ImplBinary();
        fb._identifiers = model.AccessIdentifiers.ToList();
        fb._arguments = model.Parameters.Select(x => ArgumentBinary.FromModel(x, runtime)).ToList();
        fb._name = model.Name;
        fb._returnType = runtime.Interpolate(TypePath.FromModel(model.ReturnType));
        return fb;
    }

    public static ImplBinary FromModel(ImplModel model, ClassBinary cls, RuntimeEnv runtime)
    {
        ImplBinary fb = new ImplBinary();
        fb._module = cls.Path.ModuleName;
        fb._identifiers = model.AccessIdentifiers.ToList();
        fb._arguments = model.Parameters.Select(x => ArgumentBinary.FromModel(x, runtime)).ToList();
        fb._name = model.Name;
        fb._returnType = runtime.Interpolate(TypePath.FromModel(model.ReturnType));

        return fb;
    }
}