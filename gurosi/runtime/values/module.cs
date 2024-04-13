namespace Gurosi;

public sealed class ModuleNameObject : IValueObject
{
    public TypePath Type => TypePath.UNKNOWN;
    public bool IsHeapValue => false;
    public int HeapPointer => IValueObject.Nullptr;

    private string _name;

    public string Name => _name;

    public ModuleNameObject(string name)
    {
        _name = name;
    }
    
    public T GetNumericValue<T>() where T : INumber<T>
    {
        return default(T);
    }

    public IValueObject Clone()
    {
        return new ModuleNameObject(_name);
    }
}