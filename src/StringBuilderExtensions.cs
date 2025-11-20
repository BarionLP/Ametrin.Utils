using System.Text;

namespace Ametrin.Utils;

public static class StringBuilderExtensions
{
    extension(StringBuilder builder)
    {
        // TODO: optimize this (prechange capacity)
        public StringBuilder AppendRepeated(string value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                builder.Append(value);
            }
            return builder;
        }

        public StringBuilder AppendRepeated(char value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                builder.Append(value);
            }
            return builder;
        }

        public StringBuilder Indent(int count) => builder.AppendRepeated('\t', count);
        public StringBuilder Append(string value, int indent) => builder.Indent(indent).Append(value);
        public StringBuilder AppendLine(string value, int indent) => builder.Indent(indent).AppendLine(value);
    }
}
