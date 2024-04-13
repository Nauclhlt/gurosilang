namespace Gurosi;

public static class ErrorUtil
{
    public static void RaiseDivideByZero(this ProgramRuntime runtime)
    {
        runtime.RaiseRuntimeError(new RuntimeError(ErrorRsrc.DivideByZero, ErrorRsrc.DivideByZeroMsg));
    }

    public static void RaiseAnyCompare(this ProgramRuntime runtime)
    {
        runtime.RaiseRuntimeError(new RuntimeError(ErrorRsrc.AnyCompare, ErrorRsrc.AnyCompareMsg));
    }

    public static void RaiseIndexOutOfRange(this ProgramRuntime runtime)
    {
        runtime.RaiseRuntimeError(new RuntimeError(ErrorRsrc.IndexRange, ErrorRsrc.IndexRangeMsg));
    }

    public static void RaiseLibNotFound(this ProgramRuntime runtime)
    {
        runtime.RaiseRuntimeError(new RuntimeError(ErrorRsrc.LibNotFound, ErrorRsrc.LibNotFoundMsg));
    }
}