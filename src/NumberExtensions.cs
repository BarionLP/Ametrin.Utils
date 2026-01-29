using System.Numerics;

namespace Ametrin.Utils;

public static class NumberExtensions
{
    extension<T>(T x) where T : IComparisonOperators<T, T, bool>, ISubtractionOperators<T, T, T>
    {
        public bool Approximately(T y, T tolerance)
            => x.Difference(y) <= tolerance;
        public T Difference(T y)
            => x > y ? x - y : y - x;
    }
}
