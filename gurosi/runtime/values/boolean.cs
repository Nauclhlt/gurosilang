using System.Numerics;

namespace Gurosi;

public sealed class BooleanValueObject : IValueObject
{
    public TypePath Type => TypePath.BOOLEAN;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private bool _value;

    public bool Value
    {
        get => _value;
        set => _value = value;
    }

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

    public string Str()
    {
        return _value ? "true" : "false";
    }

    public static BooleanValueObject True()
    {
        return new BooleanValueObject(true);
    }

    public static BooleanValueObject False()
    {
        return new BooleanValueObject(false);
    }
}