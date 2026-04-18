namespace Ametrin.Utils;

public static class DateTimeExtensions
{
    extension(DateTime)
    {
        public static DateTime GetOlder(DateTime x, DateTime y) => x < y ? x : y;
        public static DateTime? GetOlder(DateTime? x, DateTime? y)
            => x.HasValue ? y.HasValue ? GetOlder(x.Value, y.Value) : x : y;
    }
}