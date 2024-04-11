namespace Gurosi;

public sealed class SlicedMemory
{
    private RuntimeMemory _rtMemory;
    private int _start;
    private int _length;

    public RuntimeMemory RuntimeMemory => _rtMemory;
    public int Start => _start;
    public int Length => _length;

    public SlicedMemory(RuntimeMemory rtMemory, int start)
    {
        _rtMemory = rtMemory;
        _start = start;
    }

    public void Alloc(int address)
    {
        _length = int.Max(_length, address - _start + 1);
        _rtMemory.Alloc(address - _start);
    }

    public void Write(int address, IValueObject value)
    {
        _rtMemory.Write(address - _start, value);
    }

    public IValueObject Read(int address)
    {
        return _rtMemory.Read(address);
    }
}