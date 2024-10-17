using System.Numerics;

namespace Ametrin.Utils;

public static class NumberExtensions
{
    //public static bool Approximately(this double x, double y, double tolerance) => x.Difference(y) <= tolerance;
    //public static double Difference(this double x, double y) => x > y ? x - y : y - x;
    public static bool Approximately<T>(this T x, T y, T tolerance)
        where T : IComparisonOperators<T, T, bool>, ISubtractionOperators<T, T, T>
        => x.Difference(y) <= tolerance;
    public static T Difference<T>(this T x, T y) 
        where T : IComparisonOperators<T, T, bool>, ISubtractionOperators<T, T, T>
        => x > y ? x - y : y - x;
}
