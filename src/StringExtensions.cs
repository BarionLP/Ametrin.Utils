using System.Globalization;
using System.Text.RegularExpressions;
using Ametrin.Optional;

namespace Ametrin.Utils;

public static partial class StringExtensions
{
    public static T Parse<T>(this string input, IFormatProvider? provider = null) where T : IParsable<T>
        => T.Parse(input, provider);
    public static T Parse<T>(this ReadOnlySpan<char> input, IFormatProvider? provider = null) where T : ISpanParsable<T>
        => T.Parse(input, provider);

    public static bool TryParse<T>(this ReadOnlySpan<char> input, out T result, IFormatProvider? provider = null) where T : ISpanParsable<T>
        => T.TryParse(input, provider, out result!);
    public static bool TryParse<T>(this string input, out T result, IFormatProvider? provider = null) where T : IParsable<T>
        => T.TryParse(input, provider, out result!);

    public static Option<T> TryParse<T>(this ReadOnlySpan<char> input, IFormatProvider? provider = null) where T : ISpanParsable<T>
        => T.TryParse(input, provider, out var result) ? result : default;
    public static Option<T> TryParse<T>(this string input, IFormatProvider? provider = null) where T : IParsable<T>
        => T.TryParse(input, provider, out var result) ? result : default;

    public static T ParseOrDefault<T>(this string input, IFormatProvider? provider = null, T defaultValue = default!) where T : IParsable<T>
        => input.TryParse(out T result, provider) ? result : defaultValue;
    public static T ParseOrDefault<T>(this ReadOnlySpan<char> input, IFormatProvider? provider = null, T defaultValue = default!) where T : ISpanParsable<T>
        => input.TryParse(out T result, provider) ? result : defaultValue;

    public static string Remove(this string value, Span<char> remove)
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

    public static string ToNumberFriendly(this string input, NumberFormatInfo? formatInfo = null)
    {
        formatInfo ??= CultureInfo.CurrentCulture.NumberFormat;
        input = DecimalRegex.Replace(input, formatInfo.NumberDecimalSeparator);
        return DigitCommaRegex.Replace(input, "");
    }

    public static string ToIntFriendly(this string input) => NonDigitRegex.Replace(input, "");

    public static string ToXMLFriendly(this string input, string replacement = "") => ValidXMLCharacters.Replace(input, replacement);
    public static bool ContainsInvalidXmlChars(string input) => ValidXMLCharacters.IsMatch(input);

    public static bool StartsWith(this string str, ReadOnlySpan<char> value)
    {
        if (str is null || value.Length > str.Length)
        {
            return false;
        }

        return str.AsSpan()[..value.Length].SequenceEqual(value);
    }

    public static int FirstDigitIndex(this string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (char.IsDigit(s[i]))
                return i;
        }
        return s.Length;
    }

#if NET9_0_OR_GREATER
    [GeneratedRegex("[.,]", RegexOptions.Compiled)]
    private static partial Regex DecimalRegex { get; }

    [GeneratedRegex("[^0-9,]", RegexOptions.Compiled)]
    private static partial Regex DigitCommaRegex { get; }

    [GeneratedRegex("\\D", RegexOptions.Compiled)]
    private static partial Regex NonDigitRegex { get; }
    [GeneratedRegex("[^\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]")]
    private static partial Regex ValidXMLCharacters { get; }
#else
    private static Regex DecimalRegex => _DecimalRegex();
    private static Regex DigitCommaRegex => _DigitCommaRegex();
    private static Regex NonDigitRegex => _NonDigitRegex();
    private static Regex ValidXMLCharacters => _ValidXMLCharacters();

    [GeneratedRegex("[.,]", RegexOptions.Compiled)]
    private static partial Regex _DecimalRegex();

    [GeneratedRegex("[^0-9,]", RegexOptions.Compiled)]
    private static partial Regex _DigitCommaRegex();

    [GeneratedRegex("\\D", RegexOptions.Compiled)]
    private static partial Regex _NonDigitRegex();
    [GeneratedRegex("[^\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]")]
    private static partial Regex _ValidXMLCharacters();
#endif
}