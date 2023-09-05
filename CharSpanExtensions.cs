namespace Ametrin.Utils; 

public static class CharSpanExtensions {
    public static IList<Range> Split(this ReadOnlySpan<char> span, char delimiter) {
        int start = 0;
        var result = new List<Range>();
        for(int i = 0; i < span.Length; i++) {
            if(span[i] == delimiter) {
                if(i > start) result.Add(new Range(start, i));

                start = i + 1;
            }
        }

        if(span.Length > start) result.Add(new Range(start, span.Length));
        return result;
    }
}
