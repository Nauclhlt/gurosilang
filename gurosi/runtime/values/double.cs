using System.Numerics;

namespace Gurosi;

public sealed class DoubleValueObject : IValueObject
{
    public TypePath Type => TypePath.DOUBLE;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private double _value;

    public DoubleValueObject(double value)
    {
        _value = value;
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        return T.CreateChecked(_value);
    }

    public IValueObject Clone()
    {
        return new DoubleValueObject(_value);
    }
}