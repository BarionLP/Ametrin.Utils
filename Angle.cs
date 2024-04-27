namespace Ametrin.Utils;

public readonly record struct Angle {
    public const double DEGREES_TO_RADIANS = Math.PI / 180;
    public const double RADIANS_TO_DEGREES = 180 / Math.PI;
    public static readonly Angle Zero = new(0);
    public double Radians { get; }
    public double Degrees => Radians * RADIANS_TO_DEGREES;
    private Angle(double radians) {
        Radians = radians;
    }

    public static Angle FromRadiants(double radians) => new(radians);
    public static Angle FromDegrees(double degrees) => FromRadiants(degrees*DEGREES_TO_RADIANS);
    
    public static Angle operator -(Angle angle) => FromRadiants(-angle.Radians);
    public static Angle operator +(Angle left, Angle right) => FromRadiants(left.Radians + right.Radians);
    public static Angle operator -(Angle left, Angle right) => FromRadiants(left.Radians - right.Radians);
    public static Angle operator /(Angle left, Angle right) => FromRadiants(left.Radians / right.Radians);
    public static Angle operator *(Angle left, Angle right) => FromRadiants(left.Radians * right.Radians);
}
