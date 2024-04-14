namespace Gurosi;

public enum FunctionPointerKind
{
    Method,
    StaticMethod,
    Global
}

public sealed class FunctionPointerObject : IValueObject
{
    public TypePath Type => TypePath.FUNC_PTR;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private FunctionPointerKind _kind;
    private ClassBinary _class;
    private FunctionBinary _function;
    private RefValueObject _self;

    public FunctionPointerKind Kind => _kind;
    public ClassBinary Class => _class;
    public FunctionBinary Function => _function;
    public RefValueObject Self => _self;

    private FunctionPointerObject()
    {
    }

    public T GetNumericValue<T>() where T : INumber<T>
    {
        return default(T);
    }

    public IValueObject Clone()
    {
        return new FunctionPointerObject()
        {
            _kind = this._kind,
            _class = this._class,
            _function = this._function,
            _self = this._self
        };
    }

    public string Str()
    {
        return $"{{Fptr}}";
    }

    public static FunctionPointerObject MakeMethod(ClassBinary cls, FunctionBinary function, RefValueObject self)
    {
        return new FunctionPointerObject()
        {
            _kind = FunctionPointerKind.Method,
            _class = cls,
            _function = function,
            _self = self
        };
    }

    public static FunctionPointerObject MakeGlobalFunc(FunctionBinary function)
    {
        return new FunctionPointerObject()
        {
            _kind = FunctionPointerKind.Global,
            _function = function
        };
    }

    public static FunctionPointerObject MakeStaticMethod(ClassBinary cls, FunctionBinary function)
    {
        return new FunctionPointerObject()
        {
            _kind = FunctionPointerKind.StaticMethod,
            _class = cls,
            _function = function
        };
    }
}