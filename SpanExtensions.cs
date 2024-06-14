namespace Ametrin.Utils;

public static class SpanExtensions
{
    public static bool StartsWith<T>(this ReadOnlySpan<T> span, T value) => !span.IsEmpty && span[0]!.Equals(value);
    public static List<Range> SplitDynamic(this ReadOnlySpan<char> span, char delimiter)
    {
        int start = 0;
        var result = new List<Range>();
        for(int i = 0; i < span.Length; i++)
        {
            if(span[i] == delimiter)
            {
                if(i > start)
                    result.Add(new Range(start, i));

                start = i + 1;
            }
        }

        if(span.Length > start)
            result.Add(new Range(start, span.Length));
        return result;
    }

    public static bool All<T>(this ReadOnlySpan<T> span, Func<T, bool> condition)
    {
        foreach(var element in span)
        {
            if(!condition(element))
                return false;
        }
        return true;
    }

    public static int FirstDigitIndex(this ReadOnlySpan<char> s)
    {
        for(int i = 0; i < s.Length; i++)
        {
            if(char.IsDigit(s[i]))
                return i;
        }
        return s.Length;
    }
}
