namespace Gurosi;

// ヒープ上の配列の値。
public sealed class ArrayValueObject : HeapValueObject
{
    private int _length;
    private IValueObject[] _body;

    public int Length => _length;
    public IValueObject[] Body => _body;

    public ArrayValueObject(TypePath elementType, int length)
        : base(new TypePath("arr", elementType.ToString()))
    {
        _length = length;

        _body = new IValueObject[length];
        for (int i = 0; i < length; i++)
        {
            _body[i] = IValueObject.DefaultOf(elementType);
        }
    }

    public override IValueObject Clone()
    {
        ArrayValueObject dest = new ArrayValueObject(_type.GetArrayType(), _length);
        for (int i = 0; i < _length; i++)
        {
            dest.Body[i] = _body[i];
        }

        return dest;
    }
}