using System.Numerics;

namespace Gurosi;

// 値を表すインターフェース。
// クラスのインスタンス -> ヒープに乗せてポインタのみ保持。
// これを継承して作成する。
public interface IValueObject
{
    public TypePath Type { get; }
    public bool IsHeapValue { get; }
    public int HeapPointer { get; }

    public T GetNumericValue<T>() where T : INumber<T>;

    public IValueObject Clone();

    public static readonly int Nullptr = 0;

    public static IValueObject DefaultOf(TypePath type)
    {
        if (type.CompareEquality(TypePath.INT))
        {
            return new IntValueObject(0);
        }
        else if (type.CompareEquality(TypePath.FLOAT))
        {
            return new FloatValueObject(0f);
        }
        else if (type.CompareEquality(TypePath.DOUBLE))
        {
            return new DoubleValueObject(0.0);
        }
        else if (type.CompareEquality(TypePath.STRING))
        {
            return new StringValueObject(string.Empty);
        }
        else if (type.CompareEquality(TypePath.BOOLEAN))
        {
            return new BooleanValueObject(false);
        }
        else if (type.CompareEquality(TypePath.FUNC_PTR))
        {
            // TODO: implement.
            return null;
        }
        else
        {
            return new RefValueObject(type, IValueObject.Nullptr);
        }
    }

    public static bool IsNumericCalc(TypePath type)
    {
        if (type.CompareEquality(TypePath.INT) ||
            type.CompareEquality(TypePath.FLOAT) ||
            type.CompareEquality(TypePath.DOUBLE))
        {
            return true;
        }

        return false;
    }
}