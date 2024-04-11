namespace Gurosi;

public sealed class GenericList
{
    private List<string> _list;

    public int Count => _list.Count;

    public GenericList()
    {
        _list = new List<string>();
    }

    public void RegisterBack(string name)
    {
        _list.Add(name);
    }

    public int GetIndex(string name)
    {
        return _list.IndexOf(name);
    }
}