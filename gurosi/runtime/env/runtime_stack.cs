namespace Gurosi;

public sealed class RuntimeStack
{
    private Stack<IValueObject> _body;

    public RuntimeStack()
    {
        _body = new Stack<IValueObject>();
    }
    
    public void Push(IValueObject value)
    {
        _body.Push(value);
    }

    public IValueObject Pop()
    {
        if (_body.Count == 0)
            return null;
        return _body.Pop();
    }

    public IValueObject Top()
    {
        if (_body.Count == 0)
            return null;

        return _body.Peek();
    }

    public bool Any()
    {
        return _body.Any();
    }
}