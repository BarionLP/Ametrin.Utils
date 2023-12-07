namespace Ametrin.Utils; 

public static class LinqExtensions {
    public static TimeSpan Sum<T>(this IEnumerable<T> values, Func<T, TimeSpan> selector) {
        var tmp = TimeSpan.Zero;
        foreach (var value in values) {
            tmp += selector(value);
        }
        return tmp;
    }
    public static TimeSpan Sum(this IEnumerable<TimeSpan> values) {
        var tmp = TimeSpan.Zero;
        foreach (var value in values) {
            tmp += value;
        }
        return tmp;
    }
}
