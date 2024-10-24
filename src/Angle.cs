using System.Numerics;

namespace Ametrin.Utils;

public readonly record struct Angle : 
    IComparable<Angle>, 
    IUnaryNegationOperators<Angle, Angle>,
    IAdditionOperators<Angle, Angle, Angle>,
    ISubtractionOperators<Angle, Angle, Angle>,
    IMultiplyOperators<Angle, double, Angle>,
    IDivisionOperators<Angle, double, Angle>
{
    public const double DEGREES_TO_RADIANS = Math.PI / 180;
    public const double RADIANS_TO_DEGREES = 180 / Math.PI;
    public const double MAX_RADIANS = Math.PI * 2;
    public static Angle Zero { get; } = new(0);
    
    public double Radians { get; }
    public double Degrees => Radians * RADIANS_TO_DEGREES;
    public Angle Normalized => FromRadiants(Radians % MAX_RADIANS);
    public double Revolutions => Radians / MAX_RADIANS;

    private Angle(double radians)
    {
        Radians = radians;
    }

    public int CompareTo(Angle other) => Radians.CompareTo(other.Radians);

    public static Angle FromRadiants(double radians) => new(radians);
    public static Angle FromDegrees(double degrees) => FromRadiants(degrees * DEGREES_TO_RADIANS);

    public static Angle operator -(Angle angle) => FromRadiants(-angle.Radians);
    public static Angle operator +(Angle left, Angle right) => FromRadiants(left.Radians + right.Radians);
    public static Angle operator -(Angle left, Angle right) => FromRadiants(left.Radians - right.Radians);
    public static Angle operator /(Angle angle, double factor) => FromRadiants(angle.Radians / factor);
    public static Angle operator *(Angle angle, double factor) => FromRadiants(angle.Radians * factor);
}
