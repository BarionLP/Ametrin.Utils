namespace Ametrin.Utils;

public static class TimeSpanExtensions
{
    extension(TimeSpan x)
    {
        public bool Approximately(TimeSpan y, TimeSpan tolerance) => x.Difference(y) <= tolerance;
        public TimeSpan Difference(TimeSpan y) => x > y ? x - y : y - x;
    }
}
