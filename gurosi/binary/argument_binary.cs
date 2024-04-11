namespace Gurosi;

public sealed class ArgumentBinary : IBinary {
    private string _name;   // Named, but not used at runtime.
    private TypePath _type;

    public string Name => _name;
    public TypePath Type => _type;

    public ArgumentBinary()
    {
    }

    public void Read(BinaryReader reader)
    {
        _name = reader.ReadString();
        _type = new TypePath(string.Empty, string.Empty);
        _type.Read(reader);
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(_name);
        _type.Write(writer);
    }

    public static ArgumentBinary FromModel(DefArgument model, RuntimeEnv runtime)
    {
        ArgumentBinary ab = new ArgumentBinary();
        ab._name = model.Name;
        ab._type = runtime.Interpolate(TypePath.FromModel(model.Type));
        return ab;
    }
}