namespace Gurosi;

public sealed class TypeData {
    private int _builtIn;
    private Expression _symbol;
    private TypeData _arrayType;
    private int _genericParamIndex;
    private TypeKind _kind;

    public int BuiltIn => _builtIn;
    public Expression Symbol => _symbol;
    public TypeData ArrayType => _arrayType;
    public int GenericParamIndex => _genericParamIndex;
    public TypeKind Kind => _kind;

    private TypeData()
    {
        
    }

    public TypeData(int builtin)
    {
        _builtIn = builtin;
        _kind = TypeKind.BuiltIn;
    }

    public TypeData(Expression symbol)
    {
        _symbol = symbol;
        _kind = TypeKind.Symbol;
    }
    
    public TypeData(TypeData arrayType)
    {
        _arrayType = arrayType;
        _kind = TypeKind.Array;
    }

    public static TypeData CreateGeneric(int index)
    {
        return new TypeData()
        {
            _genericParamIndex = index,
            _kind = TypeKind.Generic
        };
    }

    public static TypeData CreateArrayBase()
    {
        return new TypeData()
        {
            _kind = TypeKind.ArrayBase
        };
    }

    public override string ToString()
    {
        if (_kind == TypeKind.BuiltIn)
            return _builtIn.ToString();
        else if (_kind == TypeKind.Symbol)
            return _symbol._PrintDebug();
        else
        {
            return $"Array<{_arrayType.ToString()}>";
        }
    }
}