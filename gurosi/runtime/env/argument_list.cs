namespace Gurosi;

public sealed class ArgumentList
{
    private IValueObject[] _arguments;

    public ArgumentList(int count)
    {
        _arguments = new IValueObject[count];
    }

    public void Load(int index, IValueObject value)
    {
        _arguments[index] = value;
    }

    public void ExtractOnMemory(SlicedMemory memory, RuntimeHeap heap)
    {
        for (int i = 0; i < _arguments.Length; i++)
        {
            memory.Alloc(i);
            if (_arguments[i] is RefValueObject r)
            {
                r.AddRefCount(heap);
            }
            memory.Write(i, _arguments[i].Clone());
        }
    }
}