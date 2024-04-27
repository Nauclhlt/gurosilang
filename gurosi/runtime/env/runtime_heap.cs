namespace Gurosi;

public sealed class RuntimeHeap
{
    private List<IValueObject> _memory;
    private Queue<int> _emptyQueue;
    private List<int> _refCounts;

    public RuntimeHeap()
    {
        _memory = new List<IValueObject>(128);
        _emptyQueue = new Queue<int>();
        _refCounts = new List<int>();
    }

    public int Alloc()
    {
        if (_emptyQueue.Count > 0)
        {
            int addr = _emptyQueue.Dequeue();
            _memory[addr] = IValueObject.NullVal;
            _refCounts[addr] = -1;

            return addr;
        }
        else
        {
            int addr = _memory.Count;
            _memory.Add(IValueObject.NullVal);
            _refCounts.Add(-1);
            return addr;
        }
    }

    public void AddReference(int addr)
    {
        _refCounts[addr]++;
        if (_refCounts[addr] == 0)
        {
            _refCounts[addr] = 1;
        }
    }

    public void ReleaseReference(int addr)
    {
        //Console.WriteLine("RELEASE: " + addr);
        _refCounts[addr]--;
    }

    public void Release(int address)
    {
        IValueObject v = _memory[address];
        if (v is ClassValueObject cv)
        {
            cv.Release(this);
        }
        _memory[address] = IValueObject.NullVal;
        _refCounts[address] = -1;
        _emptyQueue.Enqueue(address);
    }

    public void ReleaseZeroRef()
    {
        for (int i = 0; i < _memory.Count; i++)
        {
            if (_refCounts[i] == 0)
            {
                //Console.WriteLine("RELEASE: " + i);
                Release(i);
            }
        }
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