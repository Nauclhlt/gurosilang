namespace Gurosi;

// Local Variable
public sealed class Local
{
    private TypePath _type;
    private bool _constant = false;
    private int _address;

    public TypePath Type => _type;
    public int Address => _address;
    public bool Constant => _constant;

    public Local(TypePath type, int address)
    {
        _type = type;
        _address = address;
    }

    public static Local MakeConstant(TypePath type, int address)
    {
        Local lcl = new Local(type, address);
        lcl._constant = true;
        return lcl;
    }
}