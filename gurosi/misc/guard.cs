namespace Gurosi;

internal static class Guard {
    public static void Null(object value)
    {
        if (value is null)
        {
            throw new NullReferenceException(nameof(value));
        }
    }
}