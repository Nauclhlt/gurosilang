using System.Numerics;

namespace Gurosi;

public sealed class BooleanValueObject : IValueObject
{
    public TypePath Type => TypePath.BOOLEAN;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private bool _value;

    public BooleanValueObject(bool value)
    {
        _value = value;
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        int integer = _value ? 1 : 0;
        return T.CreateChecked(integer);
    }

    public IValueObject Clone()
    {
        return new BooleanValueObject(_value);
    }
}