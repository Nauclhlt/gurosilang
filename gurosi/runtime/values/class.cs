namespace Gurosi;

public sealed class ClassValueObject : HeapValueObject
{
    private ClassBinary _class;
    private List<IValueObject> _fields;
    private List<TypePath> _generics;

    public ClassBinary Class => _class;
    public List<TypePath> Generics => _generics;
    
    public ClassValueObject(ClassBinary cls)
        : base(cls.Path)
    {
        _class = cls;
        _fields = [];

        for (int i = 0; i < cls.Fields.Count; i++)
        {
            _fields.Add(IValueObject.DefaultOf(cls.Fields[i].Type));
        }

        _generics = [];
    }

    public ClassValueObject(ClassBinary cls, TypePath typeWithGeneric)
        : base(typeWithGeneric)
    {
        _class = cls;
        if (typeWithGeneric.Generics is not null)
            _generics = typeWithGeneric.Generics.ToList();
        else
            _generics = [];

        _fields = [];

        for (int i = 0; i < cls.Fields.Count; i++)
        {
            _fields.Add(IValueObject.DefaultOf(ApplyGenerics(cls.Fields[i].Type)));
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

    private TypePath ApplyGenerics(TypePath type)
    {
        if (_generics.Count == 0)
            return type;
        else
        {
            if (type.IsGenericParam)
            {
                return _generics[type.GenericParamIndex];
            }
            else
                return type;
        }
    }

    public override string Str()
    {
        return $"{{Class {_type}}}";
    }
}