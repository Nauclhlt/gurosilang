

namespace Gurosi;

public sealed class StringValueObject : IValueObject
{
    public TypePath Type => TypePath.STRING;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private string _value;

    public StringValueObject(string value)
    {
        _value = value;
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        return default;
    }

    public IValueObject Clone()
    {
        return new StringValueObject(_value);
    }
}