using System.Text;

namespace Ametrin.Utils;

public static class StringBuilderExtensions
{
    extension(StringBuilder builder)
    {
        public StringBuilder AppendRepeated(string value, int repeatCount)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(value);
            ArgumentOutOfRangeException.ThrowIfNegative(repeatCount);

            builder.EnsureCapacity(builder.Length + (value.Length * repeatCount));
            foreach (var _ in ..repeatCount)
            {
                builder.Append(value);
            }
            return builder;
        }

        // keep it for consistency
        public StringBuilder AppendRepeated(char value, int repeatCount)
            => builder.Append(value, repeatCount);

        public StringBuilder Indent(int count) => builder.Append('\t', count);
        public StringBuilder AppendIndeted(string value, int indent) => builder.Indent(indent).Append(value);
        public StringBuilder AppendLineIndeted(string value, int indent) => builder.Indent(indent).AppendLine(value);
    }
}
