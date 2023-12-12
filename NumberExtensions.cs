namespace Ametrin.Utils; 

public static class NumberExtensions {
    public static bool Approximately(this double x, double y, double tolerance) => x.Difference(y) <= tolerance;
    public static double Difference(this double x, double y) => x > y ? x - y : y - x;
}
