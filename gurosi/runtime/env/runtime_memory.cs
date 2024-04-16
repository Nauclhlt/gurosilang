namespace Gurosi;

public sealed class RuntimeMemory
{
    private const int DefaultMemorySize = 1024;

    private IValueObject[] _memory;
    private int _maxIndex = -1;

    public RuntimeMemory()
        : this(DefaultMemorySize)
    {
    }

    public RuntimeMemory(int memorySize)
    {
        if (memorySize < 0)
        {
            throw new ArgumentException("Memory size needs to be positive.", nameof(memorySize));
        }

        _memory = new IValueObject[memorySize];
    }

    public SlicedMemory Slice()
    {
        return new SlicedMemory(this, _maxIndex + 1);
    }

    public void Alloc(int address)
    {
        _maxIndex = int.Max(_maxIndex, address);
    }

    public void Release(SlicedMemory memory)
    {
        Array.Fill(_memory, null, memory.Start, memory.Length);

        _maxIndex -= memory.Length;
    }

    public void Write(int address, IValueObject value)
    {
        _memory[address] = value;
    }

    public IValueObject Read(int address)
    {
        return _memory[address];
    }
}