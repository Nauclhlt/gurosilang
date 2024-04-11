namespace Gurosi;

public sealed class FieldBinary : IBinary
{
    private string _name;
    private TypePath _type;
    private List<AccessIdentifier> _identifiers;

    public string Name => _name;
    public TypePath Type => _type;
    public List<AccessIdentifier> Identifiers => _identifiers;

    public FieldBinary()
    {
        _identifiers = new List<AccessIdentifier>();
    }

    public void Read(BinaryReader reader)
    {
        _name = reader.ReadString();
        _type = new TypePath(string.Empty, string.Empty);
        _type.Read(reader);
        _identifiers = new List<AccessIdentifier>();
        int ic = reader.ReadInt32();
        for (int i = 0; i < ic; i++)
        {
            _identifiers.Add((AccessIdentifier)reader.ReadInt32());
        }
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(_name);
        _type.Write(writer);
        writer.Write(_identifiers.Count);
        for (int i = 0; i < _identifiers.Count; i++)
        {
            writer.Write((int)_identifiers[i]);
        }
    }

    public static FieldBinary FromModel(FieldModel model, RuntimeEnv runtime)
    {
        FieldBinary fb = new FieldBinary();
        fb._name = model.Name;
        fb._identifiers = model.Identifiers.ToList();
        fb._type = runtime.Interpolate(TypePath.FromModel(model.Type));

        return fb;
    }
}