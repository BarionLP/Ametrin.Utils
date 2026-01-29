namespace Ametrin.Utils;

public static class SpanExtensions
{
    extension<T>(ReadOnlySpan<T> span)
    {
        public bool All(Func<T, bool> condition)
        {
            foreach (var element in span)
            {
                if (!condition(element))
                    return false;
            }
            return true;
        }
    }

    extension(ReadOnlySpan<char> s)
    {
        public int FirstDigitIndex()
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
    }

    extension(ReadOnlySpan<byte> bytes)
    {
        public string ToHexString() => Convert.ToHexString(bytes);
        public string ToHexStringLower() => Convert.ToHexStringLower(bytes);
        public string ToBase64String() => Convert.ToBase64String(bytes);
    }
}
