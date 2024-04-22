using System.Diagnostics.CodeAnalysis;

namespace Gurosi;

file class DoubleTypeComparer : IEqualityComparer<(TypePath, TypePath)>
{
    public bool Equals((TypePath, TypePath) x, (TypePath, TypePath) y)
    {
        if (!x.Item1.CompareEquality(y.Item1) ||
            !x.Item2.CompareEquality(y.Item2))
        {
            return false;
        }

        return true;
    }

    public int GetHashCode([DisallowNull] (TypePath, TypePath) obj)
    {
        return obj.Item1.GetHashCode() + obj.Item2.GetHashCode();
    }
}

file class TypeMapComparer : IEqualityComparer<(TypePath, TypePath, Operator)>
{
    public bool Equals((TypePath, TypePath, Operator) x, (TypePath, TypePath, Operator) y)
    {
        if (x.Item3 != y.Item3)
            return false;

        if (!x.Item1.CompareEquality(y.Item1) || !x.Item2.CompareEquality(y.Item2))
            return false;

        return true;
    }

    public int GetHashCode([DisallowNull] (TypePath, TypePath, Operator) obj)
    {
        return obj.Item1.GetHashCode() + obj.Item2.GetHashCode() + obj.Item3.GetHashCode();
    }
}

public static class TypeEvaluator
{
    public static readonly HashSet<TypePath> Decrementable = new HashSet<TypePath>()
    {
        TypePath.INT, TypePath.FLOAT, TypePath.DOUBLE
    };
    public static readonly HashSet<TypePath> Incrementable = new HashSet<TypePath>()
    {
        TypePath.INT, TypePath.FLOAT, TypePath.DOUBLE
    };

    public static readonly Dictionary<(TypePath, TypePath), string> CastMap = new Dictionary<(TypePath, TypePath), string>(
        new DoubleTypeComparer()
    )
    {
        { (TypePath.INT, TypePath.FLOAT), "cstif" },
        { (TypePath.INT, TypePath.DOUBLE), "cstid" },
        { (TypePath.FLOAT, TypePath.DOUBLE), "cstfd" },
        { (TypePath.FLOAT, TypePath.INT), "cstfi" },
        { (TypePath.DOUBLE, TypePath.INT), "cstdi" }
    };

    public static readonly HashSet<(TypePath, TypePath)> ImplicitCastMap = new HashSet<(TypePath, TypePath)>(
        new DoubleTypeComparer()
    )
    {
        (TypePath.INT, TypePath.FLOAT),
        (TypePath.FLOAT, TypePath.DOUBLE),
        (TypePath.INT, TypePath.DOUBLE)
    };

    public static readonly Dictionary<(TypePath, TypePath, Operator), TypePath> TypeMap = new Dictionary<(TypePath, TypePath, Operator), TypePath>(
        new TypeMapComparer()
    )
    {
        { (TypePath.INT, TypePath.INT, Operator.Add), TypePath.INT },
        { (TypePath.INT, TypePath.INT, Operator.Sub), TypePath.INT },
        { (TypePath.INT, TypePath.INT, Operator.Mult), TypePath.INT },
        { (TypePath.INT, TypePath.INT, Operator.Div), TypePath.INT },
        { (TypePath.INT, TypePath.INT, Operator.Mod), TypePath.INT },
        { (TypePath.INT, TypePath.INT, Operator.Power), TypePath.INT },
        { (TypePath.INT, TypePath.INT, Operator.Equal), TypePath.BOOLEAN },
        { (TypePath.INT, TypePath.INT, Operator.NotEqual), TypePath.BOOLEAN },
        { (TypePath.INT, TypePath.INT, Operator.Gt), TypePath.BOOLEAN },
        { (TypePath.INT, TypePath.INT, Operator.Lt), TypePath.BOOLEAN },
        { (TypePath.INT, TypePath.INT, Operator.Gte), TypePath.BOOLEAN },
        { (TypePath.INT, TypePath.INT, Operator.Lte), TypePath.BOOLEAN },

        { (TypePath.FLOAT, TypePath.FLOAT, Operator.Add), TypePath.FLOAT },
        { (TypePath.FLOAT, TypePath.FLOAT, Operator.Sub), TypePath.FLOAT },
        { (TypePath.FLOAT, TypePath.FLOAT, Operator.Mult), TypePath.FLOAT },
        { (TypePath.FLOAT, TypePath.FLOAT, Operator.Div), TypePath.FLOAT },
        { (TypePath.FLOAT, TypePath.FLOAT, Operator.Power), TypePath.FLOAT },
        { (TypePath.FLOAT, TypePath.FLOAT, Operator.Equal), TypePath.BOOLEAN },
        { (TypePath.FLOAT, TypePath.FLOAT, Operator.NotEqual), TypePath.BOOLEAN },
        { (TypePath.FLOAT, TypePath.FLOAT, Operator.Gt), TypePath.BOOLEAN },
        { (TypePath.FLOAT, TypePath.FLOAT, Operator.Lt), TypePath.BOOLEAN },
        { (TypePath.FLOAT, TypePath.FLOAT, Operator.Gte), TypePath.BOOLEAN },
        { (TypePath.FLOAT, TypePath.FLOAT, Operator.Lte), TypePath.BOOLEAN },

        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.Add), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.Sub), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.Mult), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.Div), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.Power), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.Equal), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.NotEqual), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.Gt), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.Lt), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.Gte), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.DOUBLE, Operator.Lte), TypePath.BOOLEAN },

        { (TypePath.STRING, TypePath.STRING, Operator.Add), TypePath.STRING },
        { (TypePath.STRING, TypePath.STRING, Operator.Equal), TypePath.BOOLEAN },
        
        { (TypePath.BOOLEAN, TypePath.BOOLEAN, Operator.LogAnd), TypePath.BOOLEAN },
        { (TypePath.BOOLEAN, TypePath.BOOLEAN, Operator.LogOr), TypePath.BOOLEAN },

        // Type converted
        { (TypePath.FLOAT, TypePath.INT, Operator.Add), TypePath.FLOAT },
        { (TypePath.FLOAT, TypePath.INT, Operator.Sub), TypePath.FLOAT },
        { (TypePath.FLOAT, TypePath.INT, Operator.Mult), TypePath.FLOAT },
        { (TypePath.FLOAT, TypePath.INT, Operator.Div), TypePath.FLOAT },
        { (TypePath.FLOAT, TypePath.INT, Operator.Equal), TypePath.BOOLEAN },
        { (TypePath.FLOAT, TypePath.INT, Operator.NotEqual), TypePath.BOOLEAN },
        { (TypePath.FLOAT, TypePath.INT, Operator.Gt), TypePath.BOOLEAN },
        { (TypePath.FLOAT, TypePath.INT, Operator.Lt), TypePath.BOOLEAN },
        { (TypePath.FLOAT, TypePath.INT, Operator.Gte), TypePath.BOOLEAN },
        { (TypePath.FLOAT, TypePath.INT, Operator.Lte), TypePath.BOOLEAN },


        { (TypePath.DOUBLE, TypePath.INT, Operator.Add), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.INT, Operator.Sub), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.INT, Operator.Mult), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.INT, Operator.Div), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.INT, Operator.Equal), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.INT, Operator.NotEqual), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.INT, Operator.Gt), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.INT, Operator.Lt), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.INT, Operator.Gte), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.INT, Operator.Lte), TypePath.BOOLEAN },


        { (TypePath.DOUBLE, TypePath.FLOAT, Operator.Add), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.FLOAT, Operator.Sub), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.FLOAT, Operator.Mult), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.FLOAT, Operator.Div), TypePath.DOUBLE },
        { (TypePath.DOUBLE, TypePath.FLOAT, Operator.Equal), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.FLOAT, Operator.NotEqual), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.FLOAT, Operator.Gt), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.FLOAT, Operator.Lt), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.FLOAT, Operator.Gte), TypePath.BOOLEAN },
        { (TypePath.DOUBLE, TypePath.FLOAT, Operator.Lte), TypePath.BOOLEAN },


        { (TypePath.STRING, TypePath.INT, Operator.Add), TypePath.STRING },
        { (TypePath.STRING, TypePath.FLOAT, Operator.Add), TypePath.STRING },
        { (TypePath.STRING, TypePath.DOUBLE, Operator.Add), TypePath.STRING },
        { (TypePath.STRING, TypePath.BOOLEAN, Operator.Add), TypePath.STRING },
    };

    public static TypePath Evaluate(Expression expr, RuntimeEnv runtime, CompileContext c)
    {
        if (expr is CalcExpression calc)
        {
            TypePath left = Evaluate(calc.Left, runtime, c);
            TypePath right = Evaluate(calc.Right, runtime, c);

            if (TypeMap.TryGetValue((left, right, calc.Operator), out TypePath result))
            {
                return result;
            }
            else if (TypeMap.TryGetValue((right, left, calc.Operator), out TypePath alt))
            {
                return alt;
            }
            else if (calc.Operator == Operator.Equal || calc.Operator == Operator.NotEqual)
            {
                return TypePath.BOOLEAN;
            }
            else
            {
                return TypePath.UNKNOWN;
            }
        }
        else
        {
            return EvaluateSingle(expr, runtime, c);
        }
    }

    // except calc expressions.
    private static TypePath EvaluateSingle(Expression expr, RuntimeEnv runtime, CompileContext c)
    {
        if (expr is IdentExpression ident)
        {
            // local, field, or module name.
            string source = ident.Identifier;
            // local
            if (c.TryGetLocal(source, out Local local))
            {
                return local.Type;
            }
            else if (c.HasClass && c.ScopeClass.HasField(source))
            {
                FieldBinary field = c.ScopeClass.GetField(source);
                return field.Type;
            }
            else if (c.HasClass && c.ScopeClass.HasStcField(source))
            {
                FieldBinary stc = c.ScopeClass.GetStcField(source);
                return stc.Type;
            }
            else
            {
                return new TypePath(source);
            }
        }
        else if (expr is NvRetvExpression nv)
        {
            return c.Runtime.Interpolate(TypePath.FromModel(nv.Type));
        }
        else if (expr is DecrExpression decr)
        {
            return Evaluate(decr.Target, runtime, c);
        }
        else if (expr is IncrExpression incr)
        {
            return Evaluate(incr.Target, runtime, c);
        }
        else if (expr is ImmediateExpression imm)
        {
            return TypePath.FromModel(new TypeData(imm.Type));
        }
        else if (expr is SelfExpression)
        {
            if (c.ScopeClass is not null)
            {
                return c.ScopeClass.Path;
            }
            else
            {
                return TypePath.UNKNOWN;
            }
        }
        else if (expr is NullExpression)
        {
            return TypePath.NULL;
        }
        else if (expr is FuncExpression func)
        {
            (FunctionBinary function, ClassBinary cls, TypePath type, Expression source) = FindFunctionWithClassPathSource(func, runtime, c);
            
            if (function is null)
                return TypePath.UNKNOWN;

            if (cls is null || cls.GenericCount == 0)
                return function.ReturnType;
            else
            {
                
                return function.ReturnType.ApplyGenerics(type);
            }
                
        }
        else if (expr is CastExpression cast)
        {
            return TypePath.FromModel(cast.TargetType);
        }
        else if (expr is ObjectCastExpression oct)
        {
            return runtime.Interpolate(TypePath.FromModel(oct.TargetType));
        }
        else if (expr is FuncPointerExpression ptr)
        {
            return TypePath.FUNC_PTR;
        }
        else if (expr is AllocExpression alloc)
        {
            return runtime.Interpolate(TypePath.FromModel(alloc.ArrayType)).CloneAsArray();
        }
        else if (expr is NewExpression ne)
        {
            return runtime.Interpolate(TypePath.FromModel(ne.Type));
        }
        else if (expr is IndexExpression idx)
        {
            TypePath arrType = Evaluate(idx.Target, runtime, c);
            if (!arrType.IsArray)
                return TypePath.UNKNOWN;

            return arrType.GetArrayType();
        }
        else if (expr is DotExpression dot)
        {
            TypePath sourceType = Evaluate(dot.Source, runtime, c);
            TypePath pureType = runtime.Interpolate(TypePath.FromModel(new TypeData(dot.Source)));
            if (runtime.TypeExists(sourceType))
            {
                ClassBinary cls = runtime.GetClass(sourceType);
                if (cls.HasField(dot.Right))
                {
                    if (cls.GenericCount == 0)
                        return cls.GetField(dot.Right).Type;
                    else
                        return cls.GetField(dot.Right).Type.ApplyGenerics(sourceType);
                }
                else
                {
                    return TypePath.UNKNOWN;
                }
            }
            else if (pureType is not null && c.Runtime.TypeExists(pureType))
            {
                // static
                ClassBinary cls = c.Runtime.GetClass(pureType);
                if (cls.HasStcField(dot.Right))
                {
                    if (cls.GenericCount == 0)
                        return cls.GetStcField(dot.Right).Type;
                    else
                        return cls.GetStcField(dot.Right).Type.ApplyGenerics(pureType);
                }
                else
                {
                    return TypePath.UNKNOWN;
                }
            }
            else
            {
                return TypePath.UNKNOWN;
            }
        }
        else if (expr is DummyExpression dummy)
        {
            return runtime.Interpolate(TypePath.FromModel(dummy.Type));
        }

        return TypePath.UNKNOWN;
    }



    public static bool IsValidNameFunction(Expression funcExpr, RuntimeEnv runtime, CompileContext c)
    {
        // expr is supposed to be DotExpression.
        Expression expr = funcExpr;
        if (expr is DotExpression dot)
        {
            if (dot.Source is IdentExpression moduleIdent &&
                runtime.ModuleExists(moduleIdent.Identifier))
            {
                string moduleName = moduleIdent.Identifier;

                return runtime.HasFunction(moduleName, dot.Right);
            }

            TypePath sourceType = Evaluate(dot.Source, runtime, c);
            TypePath pureType = runtime.Interpolate(TypePath.FromModel(new TypeData(dot.Source)));
            
            string name = dot.Right;
            
            
            if (runtime.TypeExists(sourceType))
            {
                // 拡張関数の存在
                if (runtime.ExtendFuncExists(sourceType, dot.Right, runtime))
                {
                    return true;
                }
                else if (runtime.IsClass(sourceType))
                {
                    ClassBinary cls = runtime.GetClass(sourceType);
                    if (cls.HasFunction(name))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (pureType is not null && runtime.TypeExists(pureType))
            {
                if (runtime.IsClass(pureType))
                {
                    ClassBinary cls = runtime.GetClass(pureType);
                    if (cls.HasStcFunction(dot.Right))
                    {
                        return true;
                    }
                    else
                    {
                        
                        return false;
                    }
                }
            }
        }
        else if (expr is IdentExpression ident)
        {
            // shortened or in class.
            if (c.HasClass)
            {
                return c.ScopeClass.HasFunction(ident.Identifier);
            }

            string moduleName = runtime.InterpolateGlblFuncModule(ident.Identifier);
            if (moduleName is null)
                return false;
            else
            {
                return runtime.HasFunction(moduleName, ident.Identifier);
            }
        }

        return false;
    }

    public static (FunctionBinary, ClassBinary, TypePath, Expression) FindFunctionWithClassPathSource(FuncExpression funcExpr, RuntimeEnv runtime, CompileContext c)
    {
        
        Expression expr = funcExpr.Function;
        if (expr is DotExpression dot)
        {
            if (dot.Source is IdentExpression moduleIdent &&
                runtime.ModuleExists(moduleIdent.Identifier))
            {
                string moduleName = moduleIdent.Identifier;
                return (runtime.MatchFunction(moduleName, dot.Right, funcExpr, c), null, null, null);
            }

            TypePath sourceType = Evaluate(dot.Source, runtime, c);
            TypePath pureType = runtime.Interpolate(TypePath.FromModel(new TypeData(dot.Source)));
            
            string name = dot.Right;
            
            
            if (runtime.TypeExists(sourceType))
            {
                // 拡張関数の存在
                if (runtime.ExtendFuncExists(sourceType, dot.Right, runtime))
                {
                    FunctionBinary match = runtime.MatchExtendFunction(sourceType, name, funcExpr, c);
                    return (match, null, null, dot.Source);
                }
                else if (runtime.IsClass(sourceType))
                {
                    ClassBinary cls = runtime.GetClass(sourceType);
                    if (cls.HasFunction(name))
                    {
                        FunctionBinary match = cls.MatchFunction(name, funcExpr, c, sourceType);
                        return (match, cls, sourceType, dot.Source);
                    }
                    else
                    {
                        return (null, null, null, null);
                    }
                }
            }
            else if (pureType is not null && runtime.TypeExists(pureType))
            {
                if (runtime.IsClass(pureType))
                {
                    ClassBinary cls = runtime.GetClass(pureType);
                    if (cls.HasStcFunction(dot.Right))
                    {
                        FunctionBinary match = cls.MatchStcFunction(name, funcExpr, c);
                        
                        return (match, cls, pureType, dot.Source);
                    }
                    else
                    {
                        
                        return (null, null, null, null);
                    }
                }
            }
        }
        else if (expr is IdentExpression ident)
        {
            // shortened or in class.
            if (c.HasClass)
            {
                FunctionBinary f = c.ScopeClass.MatchFunction(ident.Identifier, funcExpr, c, c.ScopeClass.Path);

                if (f is not null)
                {
                    return (f, c.ScopeClass, c.ScopeClass.Path, ident);
                }
            }

            string moduleName = runtime.InterpolateGlblFuncModule(ident.Identifier);
            if (moduleName is null)
                return (null, null, null, null);
            else
            {
                FunctionBinary match = runtime.MatchFunction(moduleName, ident.Identifier, funcExpr, c);
                return (match, null, null, null);
            }
        }

        return (null, null, null, null);
    }

    public static (FunctionBinary, ClassBinary) FindFunctionWithClass(FuncExpression funcExpr, RuntimeEnv runtime, CompileContext c)
    {
        (FunctionBinary fb, ClassBinary cb, TypePath path, Expression source) = FindFunctionWithClassPathSource(funcExpr, runtime, c);
        return (fb, cb);
    }

    public static FunctionBinary FindFunction(FuncExpression funcExpr, RuntimeEnv runtime, CompileContext c)
    {
        (FunctionBinary fb, ClassBinary cb, TypePath path, Expression source) = FindFunctionWithClassPathSource(funcExpr, runtime, c);
        return fb;
    }
}