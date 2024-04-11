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
            // TODO: implement
            _fields.Add(IValueObject.DefaultOf(cls.Fields[i].Type));
        }
    }

    public IValueObject FieldValueOf(int index)
    {
        return _fields[index];
    }
}