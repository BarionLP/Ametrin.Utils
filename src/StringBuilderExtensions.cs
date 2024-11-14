using System.Text;

namespace Ametrin.Utils;

public static class StringBuilderExtensions
{
    // Todo Optimize this (prechange capacity)
    public static StringBuilder AppendRepeated(this StringBuilder builder, string value, int count)
    {
        for (int i = 0; i < count; i++)
        {
            builder.Append(value);
        }
        return builder;
    }

    public static StringBuilder AppendRepeated(this StringBuilder builder, char value, int count)
    {
        for (int i = 0; i < count; i++)
        {
            builder.Append(value);
        }
        return builder;
    }

    public static StringBuilder Indent(this StringBuilder builder, int count) => builder.AppendRepeated('\t', count);
    public static StringBuilder Append(this StringBuilder builder, string value, int indent) => builder.Indent(indent).Append(value);
    public static StringBuilder AppendLine(this StringBuilder builder, string value, int indent) => builder.Indent(indent).AppendLine(value);
}
