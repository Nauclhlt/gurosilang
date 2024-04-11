using System.Numerics;

namespace Gurosi;

public sealed class IntValueObject : IValueObject
{
    public TypePath Type => TypePath.INT;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private int _value;

    public IntValueObject(int value)
    {
        _value = value;
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        return T.CreateChecked(_value);
    }

    public IValueObject Clone()
    {
        return new IntValueObject(_value);
    }
}