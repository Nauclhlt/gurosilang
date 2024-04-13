namespace Gurosi;

public sealed class RuntimeHeap
{
    private List<IValueObject> _memory;
    private Queue<int> _emptyQueue;

    public RuntimeHeap()
    {
        _memory = new List<IValueObject>(128);
        _emptyQueue = new Queue<int>();
    }

    public int Alloc()
    {
        if (_emptyQueue.Count > 0)
        {
            return _emptyQueue.Dequeue();
        }
        else
        {
            int addr = _memory.Count;
            _memory.Add(IValueObject.NullVal);
            return addr;
        }
    }

    public void Release(int address)
    {
        _memory[address] = IValueObject.NullVal;
        _emptyQueue.Enqueue(address);
    }

    public void Write(int address, IValueObject heapValue)
    {
        _memory[address] = heapValue;
    }

    public IValueObject Read(int address)
    {
        return _memory[address];
    }
}