namespace Gurosi;

public sealed class FuncPointerExpression : Expression
{
    private Expression _targetFunction;
    private List<TypeData> _argumentTypes;

    public Expression TargetFunction => _targetFunction;
    public List<TypeData> ArgumentTypes => _argumentTypes;

    public FuncPointerExpression(Expression function, List<TypeData> argumentTypes, Token token)
    {
        _targetFunction = function;
        _argumentTypes = argumentTypes;
        _token = token;
    }
}