namespace Ametrin.Utils;

public static class SpanExtensions
{
    public static bool All<T>(this ReadOnlySpan<T> span, Func<T, bool> condition)
    {
        foreach (var element in span)
        {
            if (!condition(element))
                return false;
        }
        return true;
    }

    public static int FirstDigitIndex(this ReadOnlySpan<char> s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (char.IsDigit(s[i]))
            {
                return i;
            }
        }
        return s.Length;
    }

    public static string ToHexString(this ReadOnlySpan<byte> bytes) => Convert.ToHexString(bytes);
    public static string ToHexStringLower(this ReadOnlySpan<byte> bytes) => Convert.ToHexStringLower(bytes);
    public static string ToBase64String(this ReadOnlySpan<byte> bytes) => Convert.ToBase64String(bytes);
}
