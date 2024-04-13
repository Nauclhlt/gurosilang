namespace Gurosi;

public sealed class GlobalFuncObject : IValueObject
{
    public TypePath Type => TypePath.UNKNOWN;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private FunctionBinary _function;

    public FunctionBinary Function => _function;

    public GlobalFuncObject(FunctionBinary function)
    {
        _function = function;
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        return default(T);
    }

    public IValueObject Clone()
    {
        return new GlobalFuncObject(_function);
    }
}