namespace Gurosi;

public sealed class StaticMethodNameObject : IValueObject
{
    public TypePath Type => TypePath.UNKNOWN;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private ClassBinary _class;
    private FunctionBinary _function;

    public ClassBinary Class => _class;
    public FunctionBinary Function => _function;

    public StaticMethodNameObject(ClassBinary @class, FunctionBinary function)
    {
        _class = @class;
        _function = function;
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        return default(T);
    }

    public IValueObject Clone()
    {
        return new StaticMethodNameObject(_class, _function);
    }
}