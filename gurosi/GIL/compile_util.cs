namespace Gurosi;

public static class CompileUtil
{
    public static bool TildeEquals(this string self, string name)
    {
        return self.RemoveTilde() == name.RemoveTilde();
    }

    public static string RemoveTilde(this string self)
    {
        int idx = self.IndexOf('~', StringComparison.Ordinal);
        if (idx == -1) return self;
        return self[..idx];
    }

    public static int GetTilde(this string self)
    {
        int idx = self.IndexOf('~', StringComparison.Ordinal);
        return int.Parse(self.Remove(0, idx + 1));
    }

    public static bool IdentifiersEquals(List<AccessIdentifier> a, List<AccessIdentifier> b)
    {
        if (a.Count != b.Count)
            return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
                return false;
        }

        return true;
    }

    public static bool ArgumentEquals(List<DefArgument> args1, List<DefArgument> args2)
    {
        if (args1.Count != args2.Count)
            return false;

        for (int i = 0; i < args1.Count; i++)
        {
            if (!args1[i].Type.CompareEquality(args2[i].Type))
                return false;
        }

        return true;
    }

    public static bool ArgumentEquals(List<ArgumentBinary> args1, List<ArgumentBinary> args2)
    {
        if (args1.Count != args2.Count)
            return false;

        for (int i = 0; i < args1.Count; i++)
        {
            if (!args1[i].Type.CompareEquality(args2[i].Type))
                return false;
        }

        return true;
    }
}