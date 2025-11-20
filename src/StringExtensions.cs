using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Ametrin.Utils;

public static partial class StringExtensions
{
    extension(string input)
    {
        public T Parse<T>(IFormatProvider? provider = null) where T : IParsable<T>
            => T.Parse(input, provider);

        public string ToNumberFriendly(NumberFormatInfo? formatInfo = null)
        {
            formatInfo ??= CultureInfo.CurrentCulture.NumberFormat;
            input = DecimalRegex.Replace(input, formatInfo.NumberDecimalSeparator);
            return DigitCommaRegex.Replace(input, "");
        }

        public string ToIntFriendly() => NonDigitRegex.Replace(input, "");
        public string ToXMLFriendly(string replacement = "") => InvalidXMLCharacters.Replace(input, replacement);
    }

    extension(ReadOnlySpan<char> input)
    {
        public T Parse<T>(IFormatProvider? provider = null) where T : ISpanParsable<T>
            => T.Parse(input, provider);

        public bool TryParse<T>(out T result, IFormatProvider? provider = null) where T : ISpanParsable<T>
            => T.TryParse(input, provider, out result!);

        public Option<T> TryParse<T>(IFormatProvider? provider = null) where T : ISpanParsable<T>
            => T.TryParse(input, provider, out var result) ? Option.Success(result) : default;
        public T ParseOrDefault<T>(IFormatProvider? provider = null, T defaultValue = default!) where T : ISpanParsable<T>
            => input.TryParse(out T result, provider) ? result : defaultValue;
    }

    extension([NotNullWhen(true)] string? input)
    {
        public bool TryParse<T>(out T result, IFormatProvider? provider = null) where T : IParsable<T>
        => T.TryParse(input, provider, out result!);
    }

    extension(string? input)
    {
        public Option<T> TryParse<T>(IFormatProvider? provider = null) where T : IParsable<T>
            => T.TryParse(input, provider, out var result) ? Option.Success(result) : default;

        public T ParseOrDefault<T>(IFormatProvider? provider = null, T defaultValue = default!) where T : IParsable<T>
            => input.TryParse(out T result, provider) ? result : defaultValue;
    }

    extension(string value)
    {
        public string Remove(Span<char> remove)
        {
            Span<char> buffer = stackalloc char[value.Length];
            int bufferIdx = 0;
            for (int valueIdx = 0; valueIdx < value.Length; valueIdx++)
            {
                if (remove.Contains(value[valueIdx]))
                {
                    continue;
                }
                buffer[bufferIdx] = value[valueIdx];
                bufferIdx++;
            }
            return new string(buffer[..bufferIdx]);
        }
        public int FirstDigitIndex() => SpanExtensions.FirstDigitIndex(value);
        public static bool ContainsInvalidXmlChars(string input) => InvalidXMLCharacters.IsMatch(input);
    }


    [GeneratedRegex("[.,]", RegexOptions.Compiled)]
    public static partial Regex DecimalRegex { get; }

    [GeneratedRegex("[^0-9,]", RegexOptions.Compiled)]
    public static partial Regex DigitCommaRegex { get; }

    [GeneratedRegex("\\D", RegexOptions.Compiled)]
    public static partial Regex NonDigitRegex { get; }
    [GeneratedRegex("[^\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]")]
    public static partial Regex InvalidXMLCharacters { get; }
}