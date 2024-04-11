namespace Gurosi;

public sealed class CodeBinary : IBinary
{
    private List<string> _body;

    public List<string> Code => _body;

    public CodeBinary()
    {
        _body = new List<string>();
    }

    public CodeBinary(List<string> code)
    {
        _body = code;
    }

    public void Read(BinaryReader reader)
    {
        GILReader gilReader = new GILReader(reader);

        int count = reader.ReadInt32();

        for (int i = 0; i < count; i++)
        {
            _body.AddRange(gilReader.ReadInstruction());
        }

        gilReader = null;
    }

    public void Write(BinaryWriter writer)
    {
        GILWriter gilWriter = new GILWriter(writer);
        int count = GIL.CountInstructions(_body, 0, _body.Count);

        writer.Write(count);

        for (int i = 0; i < _body.Count; i++)
        {
            if (GIL.OperandMap.ContainsKey(_body[i]))
            {
                gilWriter.WriteInstruction(_body, i);
            }
        }

        gilWriter = null;
    }
}