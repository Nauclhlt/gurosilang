namespace Gurosi;

public sealed class ClassValueObject : HeapValueObject
{
    private ClassBinary _class;
    private List<IValueObject> _fields;

    public ClassBinary Class => _class;
    
    public ClassValueObject(ClassBinary cls)
        : base(cls.Path)
    {
        _class = cls;
        _fields = new List<IValueObject>();

        for (int i = 0; i < cls.Fields.Count; i++)
        {
            _fields.Add(IValueObject.DefaultOf(cls.Fields[i].Type));
        }
    }

    public IValueObject FieldValueOf(int index)
    {
        return _fields[index];
    }

    public void SetFieldValue(int index, IValueObject value)
    {
        _fields[index] = value;
    }

    public bool IsValidFieldIndex(int index)
    {
        return 0 <= index && index < _fields.Count;
    }
}