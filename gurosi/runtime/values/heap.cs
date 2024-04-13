using System.Numerics;

namespace Gurosi;

// ヒープ上のValue.
public class HeapValueObject : IValueObject
{
    protected TypePath _type;

    public TypePath Type => _type;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    public HeapValueObject(TypePath type)
    {
        _type = type;
    }

    public virtual T GetNumericValue<T>() where T : INumber<T>
    {
        return default(T);
    }

    public virtual IValueObject Clone()
    {
        return new HeapValueObject(_type);
    }
}