namespace Gurosi;

public sealed class FuncExpression : Expression {
    private Expression _function;
    private List<Expression> _arguments;

    public Expression Function => _function;
    public List<Expression> Arguments => _arguments;

    public FuncExpression(Expression function, List<Expression> arguments)
    {
        _function = function;
        _arguments = arguments;
        _token = _function?.Token;
    }

    internal override string _PrintDebug()
    {
        return $"<{_function._PrintDebug()}>( {string.Join(",", _arguments.Select(x => x._PrintDebug()))} )";
    }
}