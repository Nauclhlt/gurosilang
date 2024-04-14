using System.Numerics;

namespace Gurosi;

public sealed class FloatValueObject : IValueObject
{
    public TypePath Type => TypePath.FLOAT;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private float _value;

    public FloatValueObject(float value)
    {
        _value = value;
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        return T.CreateChecked(_value);
    }

    public IValueObject Clone()
    {
        return new FloatValueObject(_value);
    }

    public string Str()
    {
        return _value.ToString();
    }
}