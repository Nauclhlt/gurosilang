namespace Gurosi;

public static class CalcUtil
{
    public static IValueObject Add(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(rightType))
        {
            if (leftType.CompareEquality(TypePath.INT))
            {
                return new IntValueObject(left.GetNumericValue<int>() + right.GetNumericValue<int>());
            }
            if (leftType.CompareEquality(TypePath.FLOAT))
            {
                return new FloatValueObject(left.GetNumericValue<float>() + right.GetNumericValue<float>());
            }
            if (leftType.CompareEquality(TypePath.DOUBLE))
            {
                return new DoubleValueObject(left.GetNumericValue<double>() + right.GetNumericValue<double>());
            }
            if (leftType.CompareEquality(TypePath.STRING))
            {
                return new StringValueObject((left as StringValueObject).Value + (right as StringValueObject).Value);
            }

            return IValueObject.NullRef;
        }

        return IValueObject.NullRef;
    }

    public static IValueObject Sub(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(rightType))
        {
            if (leftType.CompareEquality(TypePath.INT))
            {
                return new IntValueObject(left.GetNumericValue<int>() - right.GetNumericValue<int>());
            }
            if (leftType.CompareEquality(TypePath.FLOAT))
            {
                return new FloatValueObject(left.GetNumericValue<float>() - right.GetNumericValue<float>());
            }
            if (leftType.CompareEquality(TypePath.DOUBLE))
            {
                return new DoubleValueObject(left.GetNumericValue<double>() - right.GetNumericValue<double>());
            }

            return IValueObject.NullRef;
        }

        return IValueObject.NullRef;
    }

    public static IValueObject Mul(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(rightType))
        {
            if (leftType.CompareEquality(TypePath.INT))
            {
                return new IntValueObject(left.GetNumericValue<int>() * right.GetNumericValue<int>());
            }
            if (leftType.CompareEquality(TypePath.FLOAT))
            {
                return new FloatValueObject(left.GetNumericValue<float>() * right.GetNumericValue<float>());
            }
            if (leftType.CompareEquality(TypePath.DOUBLE))
            {
                return new DoubleValueObject(left.GetNumericValue<double>() * right.GetNumericValue<double>());
            }

            return IValueObject.NullRef;
        }

        return IValueObject.NullRef;
    }

    public static IValueObject Div(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(rightType))
        {
            if (leftType.CompareEquality(TypePath.INT))
            {
                int rightValue = right.GetNumericValue<int>();
                if (rightValue == 0)
                {
                    runtime.RaiseDivideByZero();
                    return IValueObject.NullRef;
                }
                return new IntValueObject(left.GetNumericValue<int>() / rightValue);
            }
            if (leftType.CompareEquality(TypePath.FLOAT))
            {
                float rightValue = right.GetNumericValue<float>();
                if (rightValue == 0f)
                {
                    runtime.RaiseDivideByZero();
                    return IValueObject.NullRef;
                }
                return new FloatValueObject(left.GetNumericValue<float>() / rightValue);
            }
            if (leftType.CompareEquality(TypePath.DOUBLE))
            {
                double rightValue = right.GetNumericValue<double>();
                if (rightValue == 0.0)
                {
                    runtime.RaiseDivideByZero();
                    return IValueObject.NullRef;
                }
                return new DoubleValueObject(left.GetNumericValue<double>() / rightValue);
            }

            return IValueObject.NullRef;
        }

        return IValueObject.NullRef;
    }

    public static IValueObject Mod(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(rightType))
        {
            if (leftType.CompareEquality(TypePath.INT))
            {
                int rightValue = int.Abs(right.GetNumericValue<int>());
                if (rightValue == 0)
                {
                    runtime.RaiseDivideByZero();
                    return IValueObject.NullRef;
                }
                return new IntValueObject(left.GetNumericValue<int>() % rightValue);
            }
            if (leftType.CompareEquality(TypePath.FLOAT))
            {
                float rightValue = float.Abs(right.GetNumericValue<float>());
                if (rightValue == 0f)
                {
                    runtime.RaiseDivideByZero();
                    return IValueObject.NullRef;
                }
                return new FloatValueObject(left.GetNumericValue<float>() % rightValue);
            }
            if (leftType.CompareEquality(TypePath.DOUBLE))
            {
                double rightValue = double.Abs(right.GetNumericValue<double>());
                if (rightValue == 0.0)
                {
                    runtime.RaiseDivideByZero();
                    return IValueObject.NullRef;
                }
                return new DoubleValueObject(left.GetNumericValue<double>() % rightValue);
            }

            return IValueObject.NullRef;
        }

        return IValueObject.NullRef;
    }

    public static IValueObject LessThan(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(rightType))
        {
            if (leftType.CompareEquality(TypePath.INT))
            {
                return new BooleanValueObject(left.GetNumericValue<int>() < right.GetNumericValue<int>());
            }
            if (leftType.CompareEquality(TypePath.FLOAT))
            {
                return new BooleanValueObject(left.GetNumericValue<float>() < right.GetNumericValue<float>());
            }
            if (leftType.CompareEquality(TypePath.DOUBLE))
            {
                return new BooleanValueObject(left.GetNumericValue<double>() < right.GetNumericValue<double>());
            }

            return IValueObject.NullRef;
        }

        return IValueObject.NullRef;
    }

    public static IValueObject LessThanEqual(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(rightType))
        {
            if (leftType.CompareEquality(TypePath.INT))
            {
                return new BooleanValueObject(left.GetNumericValue<int>() <= right.GetNumericValue<int>());
            }
            if (leftType.CompareEquality(TypePath.FLOAT))
            {
                return new BooleanValueObject(left.GetNumericValue<float>() <= right.GetNumericValue<float>());
            }
            if (leftType.CompareEquality(TypePath.DOUBLE))
            {
                return new BooleanValueObject(left.GetNumericValue<double>() <= right.GetNumericValue<double>());
            }

            return IValueObject.NullRef;
        }

        return IValueObject.NullRef;
    }

    public static IValueObject GreaterThan(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(rightType))
        {
            if (leftType.CompareEquality(TypePath.INT))
            {
                return new BooleanValueObject(left.GetNumericValue<int>() > right.GetNumericValue<int>());
            }
            if (leftType.CompareEquality(TypePath.FLOAT))
            {
                return new BooleanValueObject(left.GetNumericValue<float>() > right.GetNumericValue<float>());
            }
            if (leftType.CompareEquality(TypePath.DOUBLE))
            {
                return new BooleanValueObject(left.GetNumericValue<double>() > right.GetNumericValue<double>());
            }

            return IValueObject.NullRef;
        }

        return IValueObject.NullRef;
    }

    public static IValueObject GreaterThanEqual(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(rightType))
        {
            if (leftType.CompareEquality(TypePath.INT))
            {
                return new BooleanValueObject(left.GetNumericValue<int>() >= right.GetNumericValue<int>());
            }
            if (leftType.CompareEquality(TypePath.FLOAT))
            {
                return new BooleanValueObject(left.GetNumericValue<float>() >= right.GetNumericValue<float>());
            }
            if (leftType.CompareEquality(TypePath.DOUBLE))
            {
                return new BooleanValueObject(left.GetNumericValue<double>() >= right.GetNumericValue<double>());
            }

            return IValueObject.NullRef;
        }

        return IValueObject.NullRef;
    }

    public static IValueObject Equal(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(rightType))
        {
            if (leftType.CompareEquality(TypePath.INT))
            {
                return new BooleanValueObject(left.GetNumericValue<int>() == right.GetNumericValue<int>());
            }
            if (leftType.CompareEquality(TypePath.FLOAT))
            {
                return new BooleanValueObject(left.GetNumericValue<float>() == right.GetNumericValue<float>());
            }
            if (leftType.CompareEquality(TypePath.DOUBLE))
            {
                return new BooleanValueObject(left.GetNumericValue<double>() == right.GetNumericValue<double>());
            }
            if (leftType.CompareEquality(TypePath.BOOLEAN))
            {
                return new BooleanValueObject((left as BooleanValueObject).Value == (right as BooleanValueObject).Value);
            }
            if (leftType.CompareEquality(TypePath.STRING))
            {
                return new BooleanValueObject((left as StringValueObject).Value == (right as StringValueObject).Value);
            }
            if (leftType.CompareEquality(TypePath.FUNC_PTR))
            {
                // TODO: implement;
                return BooleanValueObject.False();
            }
            if (leftType.CompareEquality(TypePath.ANY))
            {
                runtime.RaiseAnyCompare();
            }

            // compare reference types.
            if (left.IsHeapValue)
            {
                return new BooleanValueObject(left.HeapPointer == right.HeapPointer);
            }


            return IValueObject.NullRef;
        }

        return IValueObject.NullRef;
    }

    public static IValueObject NotEqual(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        IValueObject val = Equal(left, right, runtime);
        if (val == IValueObject.NullRef)
            return IValueObject.NullRef;

        BooleanValueObject bv = val as BooleanValueObject;
        bv.Value = !bv.Value;

        return bv;
    }

    public static IValueObject And(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(TypePath.BOOLEAN) && rightType.CompareEquality(TypePath.BOOLEAN))
        {
            return new BooleanValueObject((left as BooleanValueObject).Value && (right as BooleanValueObject).Value);
        }

        return IValueObject.NullRef;
    }

    public static IValueObject Or(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(TypePath.BOOLEAN) && rightType.CompareEquality(TypePath.BOOLEAN))
        {
            return new BooleanValueObject((left as BooleanValueObject).Value || (right as BooleanValueObject).Value);
        }

        return IValueObject.NullRef;
    }

    public static IValueObject Xor(IValueObject left, IValueObject right, ProgramRuntime runtime)
    {
        TypePath leftType = left.Type;
        TypePath rightType = right.Type;

        if (leftType.CompareEquality(TypePath.BOOLEAN) && rightType.CompareEquality(TypePath.BOOLEAN))
        {
            return new BooleanValueObject((left as BooleanValueObject).Value ^ (right as BooleanValueObject).Value);
        }

        return IValueObject.NullRef;
    }

    public static IValueObject Increment(IValueObject value, ProgramRuntime runtime)
    {
        TypePath type = value.Type;

        if (type.CompareEquality(TypePath.INT))
        {
            return new IntValueObject(value.GetNumericValue<int>() + 1);
        }
        else if (type.CompareEquality(TypePath.FLOAT))
        {
            return new FloatValueObject(value.GetNumericValue<float>() + 1f);
        }
        else if (type.CompareEquality(TypePath.DOUBLE))
        {
            return new DoubleValueObject(value.GetNumericValue<double>() + 1.0);
        }
        else
        {
            // invalid type.
            return IValueObject.NullRef;
        }
    }

    public static IValueObject Decrement(IValueObject value, ProgramRuntime runtime)
    {
        TypePath type = value.Type;

        if (type.CompareEquality(TypePath.INT))
        {
            return new IntValueObject(value.GetNumericValue<int>() - 1);
        }
        else if (type.CompareEquality(TypePath.FLOAT))
        {
            return new FloatValueObject(value.GetNumericValue<float>() - 1f);
        }
        else if (type.CompareEquality(TypePath.DOUBLE))
        {
            return new DoubleValueObject(value.GetNumericValue<double>() - 1.0);
        }
        else
        {
            // invalid type.
            return IValueObject.NullRef;
        }
    }

    public static IValueObject PrCastIF(IValueObject value, ProgramRuntime runtime)
    {
        if (!value.Type.CompareEquality(TypePath.INT))
            return IValueObject.NullRef;

        return new FloatValueObject(value.GetNumericValue<int>());
    }

    public static IValueObject PrCastFD(IValueObject value, ProgramRuntime runtime)
    {
        if (!value.Type.CompareEquality(TypePath.FLOAT))
            return IValueObject.NullRef;

        return new DoubleValueObject(value.GetNumericValue<float>());
    }

    public static IValueObject PrCastID(IValueObject value, ProgramRuntime runtime)
    {
        if (!value.Type.CompareEquality(TypePath.INT))
            return IValueObject.NullRef;

        return new DoubleValueObject(value.GetNumericValue<int>());
    }

    public static bool IsPositive(IValueObject value, ProgramRuntime runtime)
    {
        TypePath type = value.Type;

        if (type.CompareEquality(TypePath.INT))
            return value.GetNumericValue<int>() > 0;
        else if (type.CompareEquality(TypePath.FLOAT))
            return value.GetNumericValue<float>() > 0f;
        else if (type.CompareEquality(TypePath.DOUBLE))
            return value.GetNumericValue<double>() > 0.0;
        else
        {
            return false;
        }
    }

    public static bool IsNegative(IValueObject value, ProgramRuntime runtime)
    {
        TypePath type = value.Type;

        if (type.CompareEquality(TypePath.INT))
            return value.GetNumericValue<int>() < 0;
        else if (type.CompareEquality(TypePath.FLOAT))
            return value.GetNumericValue<float>() < 0f;
        else if (type.CompareEquality(TypePath.DOUBLE))
            return value.GetNumericValue<double>() < 0.0;
        else
        {
            return false;
        }
    }

    public static bool IsZero(IValueObject value, ProgramRuntime runtime)
    {
        TypePath type = value.Type;

        if (type.CompareEquality(TypePath.INT))
            return value.GetNumericValue<int>() == 0;
        else if (type.CompareEquality(TypePath.FLOAT))
            return value.GetNumericValue<float>() == 0f;
        else if (type.CompareEquality(TypePath.DOUBLE))
            return value.GetNumericValue<double>() == 0.0;
        else
        {
            return false;
        }
    }
}