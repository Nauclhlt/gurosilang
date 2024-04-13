namespace Gurosi;

public sealed class GILWriter
{
    private BinaryWriter _writer;

    public GILWriter(BinaryWriter writer)
    {
        _writer = writer;
    }


    // <1byte> | op
    // <1byte> | number of arguments
    // ( <4byte> | length of argument = N
    //   <Nbyte> | body of argument )+

    public void WriteInstruction(List<string> code, int start)
    {
        if (start >= code.Count)
            return;

        List<byte> bytes = new List<byte>(64);

        string op = code[start];
        OperandType[] operands = GIL.OperandMap[op];
        
        // no operand
        if (operands is null)
        {
            operands = Array.Empty<OperandType>();
        }

        int bodylen = 0;

        for (int i = 0; i < operands.Length; i++)
        {
            OperandType type = operands[i];

            string source = code[start + i + 1];

            
            

            if (type == OperandType.String)
            {
                byte[] strbytes = Encoding.Unicode.GetBytes(source);
                bytes.AddRange(BitConverter.GetBytes(strbytes.Length));
                bytes.AddRange(strbytes);
                bodylen += 4 + strbytes.Length;
            }
            else if (type == OperandType.Int || type == OperandType.Address)
            {
                byte[] intbytes = BitConverter.GetBytes(int.Parse(source));
                bytes.AddRange(BitConverter.GetBytes(intbytes.Length));
                bytes.AddRange(intbytes);
                bodylen += 8;
            }
            else if (type == OperandType.Float)
            {
                byte[] floatbytes = BitConverter.GetBytes(float.Parse(source));
                bytes.AddRange(BitConverter.GetBytes(floatbytes.Length));
                bytes.AddRange(floatbytes);
                bodylen += 8;
            }
            else if (type == OperandType.Boolean)
            {
                byte[] boolbytes = BitConverter.GetBytes(bool.Parse(source));
                bytes.AddRange(BitConverter.GetBytes(boolbytes.Length));
                bytes.AddRange(boolbytes);
                bodylen += 5;
            }
            else if (type == OperandType.Double)
            {
                byte[] doublebytes = BitConverter.GetBytes(double.Parse(source));
                bytes.AddRange(BitConverter.GetBytes(doublebytes.Length));
                bytes.AddRange(doublebytes);
                bodylen += 12;
            }
        }

        byte opcode = unchecked((byte)Array.IndexOf(GIL.CodeMap, op));
        byte argnum = unchecked((byte)operands.Length);

        _writer.Write(opcode);
        _writer.Write(argnum);
        bodylen += 2;

        _writer.Write(CollectionsMarshal.AsSpan(bytes));
        _writer.Write(bodylen + 4);
    }
}

public sealed class GILReader
{
    private BinaryReader _reader;

    public GILReader(BinaryReader reader)
    {
        _reader = reader;
    }

    public List<string> ReadInstruction()
    {
        List<string> code = new List<string>();

        byte opcode = _reader.ReadByte();
        byte argnum = _reader.ReadByte();

        string op = GIL.CodeMap[opcode];
        OperandType[] operands = GIL.OperandMap[op];

        code.Add(op);

        // no operands
        if (operands is null)
        {
            _reader.ReadInt32();
            return code;
        }

        for (int i = 0; i < operands.Length; i++)
        {
            OperandType type = operands[i];

            if (type == OperandType.String)
            {
                //byte[] strbytes = Encoding.Unicode.GetBytes(source);
                //bytes.AddRange(BitConverter.GetBytes(strbytes.Length));
                //bytes.AddRange(strbytes);

                int len = BitConverter.ToInt32(_reader.ReadBytes(4));
                string body = Encoding.Unicode.GetString(_reader.ReadBytes(len));

                code.Add(body);
            }
            else if (type == OperandType.Int || type == OperandType.Address)
            {
                //byte[] intbytes = BitConverter.GetBytes(int.Parse(source));
                //bytes.AddRange(BitConverter.GetBytes(intbytes.Length));
                //bytes.AddRange(intbytes);

                _reader.ReadBytes(4); // skip length data.
                code.Add(BitConverter.ToInt32(_reader.ReadBytes(4)).ToString());
            }
            else if (type == OperandType.Float)
            {
                //byte[] floatbytes = BitConverter.GetBytes(float.Parse(source));
                //bytes.AddRange(BitConverter.GetBytes(floatbytes.Length));
                //bytes.AddRange(floatbytes);

                _reader.ReadBytes(4);  // skip length data.
                code.Add(BitConverter.ToSingle(_reader.ReadBytes(4)).ToString());
            }
            else if (type == OperandType.Boolean)
            {
                //byte[] boolbytes = BitConverter.GetBytes(bool.Parse(source));
                //bytes.AddRange(BitConverter.GetBytes(boolbytes.Length));
                //bytes.AddRange(boolbytes);
                _reader.ReadBytes(4);  // skip length data.
                code.Add(BitConverter.ToBoolean(_reader.ReadBytes(1)).ToString().ToLower());
            }
            else if (type == OperandType.Double)
            {
                //byte[] doublebytes = BitConverter.GetBytes(double.Parse(source));
                //bytes.AddRange(BitConverter.GetBytes(doublebytes.Length));
                //bytes.AddRange(doublebytes);

                _reader.ReadBytes(4);  // skip length data.
                code.Add(BitConverter.ToDouble(_reader.ReadBytes(8)).ToString());
            }
        }

        _reader.ReadInt32();

        return code;
    }
}