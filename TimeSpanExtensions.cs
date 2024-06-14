namespace Ametrin.Utils;

public static class TimeSpanExtensions
{
    public static bool Approximately(this TimeSpan x, TimeSpan y, TimeSpan tolerance) => x.Difference(y) <= tolerance;
    public static TimeSpan Difference(this TimeSpan x, TimeSpan y) => x > y ? x - y : y - x;
}
