namespace Gurosi;

sealed class ImmediateExpression : Expression {
    private int _type;
    private string _string;
    private int _int;
    private float _float;
    private double _double;
    private bool _boolean;

    public string String => _string;
    public int Int => _int;
    public float Float => _float;
    public double Double => _double;
    public bool Boolean => _boolean;
    public int Type => _type;

    public static ImmediateExpression MakeString(string value, Token token)
    {
        return new ImmediateExpression() { _type = BuiltinTypes.STRING, _string = value, _token = token };
    }

    public static ImmediateExpression MakeInt(int value, Token token)
    {
        return new ImmediateExpression() { _type = BuiltinTypes.INT, _int = value, _token = token };
    }

    public static ImmediateExpression MakeFloat(float value, Token token)
    {
        return new ImmediateExpression() { _type = BuiltinTypes.FLOAT, _float = value, _token = token };
    }

    public static ImmediateExpression MakeDouble(double value, Token token)
    {
        return new ImmediateExpression() { _type = BuiltinTypes.DOUBLE, _double = value, _token = token };
    }

    public static ImmediateExpression MakeBoolean(bool value, Token token)
    {
        return new ImmediateExpression() { _type = BuiltinTypes.BOOLEAN, _boolean = value, _token = token };
    }

    internal override string _PrintDebug()
    {
        if (_type == BuiltinTypes.STRING)
            return _string;
        if (_type == BuiltinTypes.INT)
            return _int.ToString();
        if (_type == BuiltinTypes.FLOAT)
            return _float.ToString();
        if (_type == BuiltinTypes.DOUBLE)
            return _double.ToString();
        if (_type == BuiltinTypes.BOOLEAN)
            return _boolean.ToString();

        return "";
    }
}