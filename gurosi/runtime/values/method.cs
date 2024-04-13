namespace Gurosi;

public sealed class MethodNameObject : IValueObject
{
    public TypePath Type => TypePath.UNKNOWN;
    public bool IsHeapValue => false;
    public int HeapPointer => 0;

    private RefValueObject _source;
    private ClassBinary _class;
    private FunctionBinary _function;

    public RefValueObject Source => _source;
    public ClassBinary Class => _class;
    public FunctionBinary Function => _function;

    public MethodNameObject(RefValueObject source, ClassBinary cls, FunctionBinary function)
    {
        _source = source;
        _class = cls;
        _function = function;
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        return default(T);
    }

    public IValueObject Clone()
    {
        return new MethodNameObject(_source, _class, _function);
    }
}