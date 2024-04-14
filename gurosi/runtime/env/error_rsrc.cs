namespace Gurosi;

internal static class ErrorRsrc
{
    public static readonly string DivideByZero = "DividedByZero";
    public static readonly string DivideByZeroMsg = "Attempted to divide by 0.";
    public static readonly string AnyCompare = "ComparedAny";
    public static readonly string AnyCompareMsg = "Attempted to compare 'any' types.";
    public static readonly string IndexRange = "IndexOutOfRange";
    public static readonly string IndexRangeMsg = "Index is outside the bounds.";
    public static readonly string LibNotFound = "LibraryNotFound";
    public static readonly string LibNotFoundMsg = "One or more of the library(ies) not found.";
    public static readonly string NoReturnValue = "NoReturnValue";
    public static readonly string NoReturnValueMsg = "Specified function does not return a value.";
    public static readonly string InvalidCast = "InvalidCast";
    public static readonly string InvalidCastMsg = "Attempted an invalid cast.";
}