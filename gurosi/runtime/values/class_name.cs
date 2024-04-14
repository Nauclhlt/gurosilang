namespace Gurosi;

public sealed class ClassNameObject : IValueObject
{
    public TypePath Type => TypePath.UNKNOWN;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private ClassBinary _class;

    public ClassBinary Class => _class;

    public ClassNameObject(ClassBinary cls)
    {
        _class = cls;
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        return default(T);
    }

    public IValueObject Clone()
    {
        return new ClassNameObject(_class);
    }

    public string Str()
    {
        return _class.Path.ToString();
    }
}