namespace Gurosi;

public sealed class CompiledCode : IDisposable
{
    private bool _disposed;
    private MemoryStream _body;
    private BinaryReader _reader;

    private CompiledCode()
    {
        _disposed = false;
    }

    public void Seek(int position)
    {
        _body.Seek(position, SeekOrigin.Begin);
    }

    public bool EndOfCode => _body.Position >= _body.Length;

    public void Retr(int offset)
    {
        _reader.BaseStream.Seek(-offset, SeekOrigin.Current);
    }

    public byte ReadOpCode()
    {
        return _reader.ReadByte();
    }

    public byte ReadArgnum()
    {
        return _reader.ReadByte();
    }

    public string ReadString()
    {
        Span<byte> lengthBuffer = stackalloc byte[4];
        _reader.Read(lengthBuffer);

        int length = BitConverter.ToInt32(lengthBuffer);
        
        Span<byte> strBuffer = stackalloc byte[length];
        _reader.Read(strBuffer);

        return Encoding.Unicode.GetString(strBuffer);
    }

    public int ReadInt()
    {
        Span<byte> buffer4 = stackalloc byte[4];
        _reader.Read(buffer4);
        _reader.Read(buffer4);

        return BitConverter.ToInt32(buffer4);
    }

    public int ReadInstLen()
    {
        if (EndOfCode)
            return 0;
        return _reader.ReadInt32();
    }

    public int ReadAddress()
    {
        return ReadInt();
    }

    public float ReadFloat()
    {
        Span<byte> buffer4 = stackalloc byte[4];
        _reader.Read(buffer4);
        _reader.Read(buffer4);

        return BitConverter.ToSingle(buffer4);
    }

    public double ReadDouble()
    {
        _reader.ReadBytes(4);
        Span<byte> buffer8 = stackalloc byte[8];
        _reader.Read(buffer8);

        return BitConverter.ToDouble(buffer8);
    }

    public bool ReadBoolean()
    {
        _reader.ReadBytes(4);
        return _reader.ReadByte() == (byte)1;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // managed
                _body.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CompiledCode()
    {
        this.Dispose(false);
    }

    public static CompiledCode CompileFromBinary(CodeBinary code)
    {
        // List<string> code to MemoryStream
        List<string> src = code.Code;
        MemoryStream mem = new MemoryStream();

        using (BinaryWriter writer = new BinaryWriter(mem, Encoding.Unicode, true))
        {
            GILWriter gilWriter = new GILWriter(writer);

            for (int i = 0; i < src.Count; i++)
            {
                if (GIL.OperandMap.ContainsKey(src[i]))
                {
                    gilWriter.WriteInstruction(src, i);
                }
            }
        }

        mem.Seek(0, SeekOrigin.Begin);

        return new CompiledCode() { _body = mem, _reader = new BinaryReader(mem, Encoding.Unicode, true) };
    }
}