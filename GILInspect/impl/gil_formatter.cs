using Gurosi;
using System.Text;

namespace GILInspect;

public static class GILFormatter
{
    public static string Format(Library library)
    {
        StringBuilder builder = new StringBuilder();

        FormatImports(builder, library.Imported);

        builder.AppendLine();

        for (int i = 0; i < library.Classes.Count; i++)
        {
            FormatClass(builder, library.Classes[i]);
        }

        builder.Replace("\t", "    ");

        return builder.ToString();
    }

    private static void FormatImports(StringBuilder buffer, List<string> imports)
    {
        buffer.AppendLine("imports:");

        for (int i = 0; i < imports.Count; i++)
        {
            buffer.AppendLine(imports[i]);
        }
    }

    private static void FormatClass(StringBuilder buffer, ClassBinary c)
    {
        buffer.Append("class* ");
        buffer.Append(c.Path.Name);
        if (c.GenericCount > 0)
        {
            buffer.Append("^[");
            buffer.Append(c.GenericCount);
            buffer.Append(']');
        }
        buffer.Append(" (module=");
        buffer.Append(c.Path.ModuleName);
        buffer.Append(')');

        buffer.AppendLine();

        buffer.AppendLine("fields:");

        for (int i = 0; i < c.Fields.Count; i++)
        {
            buffer.Append('\t');
            buffer.Append(c.Fields[i].Name);
            buffer.AppendLine(":");
            
            buffer.Append("\t\ttype:  ");
            buffer.AppendLine(c.Fields[i].Type.ToString());
            
            buffer.Append("\t\tidentifiers:  ");
            FormatInlineIdentifiers(buffer, c.Fields[i].Identifiers);
            buffer.AppendLine();
        }

        buffer.AppendLine("methods:");

        for (int i = 0; i < c.Functions.Count; i++)
        {
            var f = c.Functions[i];
            buffer.Append('\t');
            buffer.Append(f.Name);
            buffer.AppendLine(":");

            if (f.Arguments.Count > 0)
            {
                buffer.AppendLine("\t\targ(s):");
                for (int j = 0; j < f.Arguments.Count; j++)
                {
                    buffer.Append("\t\t\t");
                    buffer.Append(f.Arguments[j].Name);
                    buffer.AppendLine(":");

                    buffer.Append("\t\t\t\t");
                    buffer.Append("type:  ");
                    buffer.Append(f.Arguments[j].Type.ToString());
                    buffer.AppendLine();
                }
            }

            buffer.Append("\t\treturns:  ");
            buffer.AppendLine(f.ReturnType.ToString());

            buffer.Append("\t\tidentifiers:  ");
            FormatInlineIdentifiers(buffer, f.Identifiers);
            buffer.AppendLine();

            buffer.Append("\t\tbody:");

            var code = f.Body.Code;
            for (int k = 0; k < code.Count; k++)
            {
                if (GIL.OperandMap.ContainsKey(code[k]))
                {
                    buffer.AppendLine();
                    buffer.Append("\t\t\t");
                    buffer.Append(code[k]);
                }
                else
                {
                    buffer.Append(' ');
                    buffer.Append(code[k]);
                }
            }

            buffer.AppendLine();
        }

        for (int i = 0; i < c.StcFunctions.Count; i++)
        {
            var f = c.StcFunctions[i];
            buffer.Append('\t');
            buffer.Append(f.Name);
            buffer.AppendLine(":");

            if (f.Arguments.Count > 0)
            {
                buffer.AppendLine("\t\targ(s):");
                for (int j = 0; j < f.Arguments.Count; j++)
                {
                    buffer.Append("\t\t\t");
                    buffer.Append(f.Arguments[j].Name);
                    buffer.AppendLine(":");

                    buffer.Append("\t\t\t\t");
                    buffer.Append("type:  ");
                    buffer.Append(f.Arguments[j].Type.ToString());
                    buffer.AppendLine();
                }
            }

            buffer.Append("\t\treturns:  ");
            buffer.AppendLine(f.ReturnType.ToString());

            buffer.Append("\t\tidentifiers:  ");
            FormatInlineIdentifiers(buffer, f.Identifiers);
            buffer.AppendLine();

            buffer.Append("\t\tbody:");

            var code = f.Body.Code;
            for (int k = 0; k < code.Count; k++)
            {
                if (GIL.OperandMap.ContainsKey(code[k]))
                {
                    buffer.AppendLine();
                    buffer.Append("\t\t\t");
                    buffer.Append(code[k]);
                }
                else
                {
                    buffer.Append(' ');
                    buffer.Append(code[k]);
                }
            }

            buffer.AppendLine();
        }
    }

    private static void FormatInlineIdentifiers(StringBuilder buffer, List<AccessIdentifier> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (i == 0)
            {
                buffer.Append(list[i].ToString().ToLower());
            }
            else
            {
                buffer.Append(" " + list[i].ToString().ToLower());
            }
        }
    }
}