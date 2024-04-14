using System.Numerics;

namespace Gurosi;

// ヒープ上の値を参照するValue.
public sealed class RefValueObject : IValueObject
{
    private TypePath _type;
    private int _pointer;

    public TypePath Type => _type;
    public bool IsHeapValue => true;
    public int HeapPointer => _pointer;

    public RefValueObject(TypePath type, int pointer)
    {
        _type = type;
        _pointer = pointer;
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        return default(T);
    }

    public IValueObject Clone()
    {
        return new RefValueObject(_type, _pointer);
    }

    public IValueObject Refer(RuntimeHeap heap)
    {
        return heap.Read(_pointer);
    }

    public T Refer<T>(RuntimeHeap heap) where T : IValueObject
    {
        return (T)Refer(heap);
    }

    public string Str()
    {
        return $"PTR={_pointer}";
    }
}