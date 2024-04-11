namespace Gurosi;

public sealed class FunctionIndexer
{
    private Dictionary<string, int> _map;

    public FunctionIndexer()
    {
        _map = new Dictionary<string, int>();
    }

    public int GetIndex(string name)
    {
        if (_map.TryGetValue(name, out int idx))
        {
            _map[name] = idx + 1;
            return idx + 1;
        }
        else
        {
            _map.Add(name, 1);
            return 1;
        }
    }
}