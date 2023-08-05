using System.Globalization;
using System.Text.RegularExpressions;

namespace Ametrin.Utils;
public static partial class StringExtensions {
    public static T Parse<T>(this string input, IFormatProvider? provider = null) where T : IParsable<T> {
        return T.Parse(input, provider);
    }
    public static T Parse<T>(this ReadOnlySpan<char> input, IFormatProvider? provider = null) where T : ISpanParsable<T> {
        return T.Parse(input, provider);
    }

    public static bool TryParse<T>(this ReadOnlySpan<char> input, out T result, IFormatProvider? provider = null) where T : ISpanParsable<T> {
        return T.TryParse(input, provider, out result!);
    }
    public static bool TryParse<T>(this string input, out T result, IFormatProvider? provider = null) where T : IParsable<T> {
        return T.TryParse(input, provider, out result!);
    }

    public static T ParseOrDefault<T>(this string input, IFormatProvider? provider = null, T defaultValue = default!) where T : IParsable<T> {
        return input.TryParse(out T result, provider) ? result : defaultValue;
    }
    public static T ParseOrDefault<T>(this ReadOnlySpan<char> input, IFormatProvider? provider = null, T defaultValue = default!) where T : ISpanParsable<T> {
        return input.TryParse(out T result, provider) ? result : defaultValue;
    }
    public static string ToNumberFriendly(this string input, NumberFormatInfo? formatInfo = null) {
        formatInfo ??= CultureInfo.CurrentCulture.NumberFormat;
        input = DecimalRegex().Replace(input, formatInfo.NumberDecimalSeparator);
        return DigitCommaRegex().Replace(input, "");
    }

    public static string ToIntFriendly(this string input) => NonDigitRegex().Replace(input, "");

    public static string ToXMLFriendly(this string input, string replacement = "") => ValidXMLCharacters().Replace(input, replacement);
    public static bool ContainsInvalidXmlChars(string input) => ValidXMLCharacters().IsMatch(input);


    [GeneratedRegex("[.,]", RegexOptions.Compiled)]
    private static partial Regex DecimalRegex();

    [GeneratedRegex("[^0-9,]", RegexOptions.Compiled)]
    private static partial Regex DigitCommaRegex();

    [GeneratedRegex("\\D", RegexOptions.Compiled)]
    private static partial Regex NonDigitRegex();
    [GeneratedRegex("[^\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]")]
    private static partial Regex ValidXMLCharacters();
}