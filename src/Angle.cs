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
    public const double MAX_RADIANS = double.Pi * 2;
    public static Angle Zero { get; } = new(0);

    public double Radians { get; }
    public double Degrees => double.RadiansToDegrees(Radians);
    public Angle Normalized => FromRadians(Radians % MAX_RADIANS);
    public double Revolutions => Radians / MAX_RADIANS;

    private Angle(double radians)
    {
        Radians = radians;
    }

    public int CompareTo(Angle other) => Radians.CompareTo(other.Radians);

    public static Angle FromRadians(double radians) => new(radians);
    public static Angle FromDegrees(double degrees) => FromRadians(double.DegreesToRadians(degrees));
    public static Angle Clamp(Angle value, Angle min, Angle max) => new(double.Clamp(value.Radians, min.Radians, max.Radians));

    public static Angle operator -(Angle angle) => FromRadians(-angle.Radians);
    public static Angle operator +(Angle left, Angle right) => FromRadians(left.Radians + right.Radians);
    public static Angle operator -(Angle left, Angle right) => FromRadians(left.Radians - right.Radians);
    public static Angle operator /(Angle angle, double factor) => FromRadians(angle.Radians / factor);
    public static Angle operator *(Angle angle, double factor) => FromRadians(angle.Radians * factor);
}

public readonly record struct AngleF :
    IComparable<AngleF>,
    IUnaryNegationOperators<AngleF, AngleF>,
    IAdditionOperators<AngleF, AngleF, AngleF>,
    ISubtractionOperators<AngleF, AngleF, AngleF>,
    IMultiplyOperators<AngleF, float, AngleF>,
    IDivisionOperators<AngleF, float, AngleF>
{
    public const float MAX_RADIANS = float.Pi * 2;
    public static AngleF Zero { get; } = new(0);

    public float Radians { get; }
    public float Degrees => float.RadiansToDegrees(Radians);
    public AngleF Normalized => FromRadians(Radians % MAX_RADIANS);
    public float Revolutions => Radians / MAX_RADIANS;

    private AngleF(float radians)
    {
        Radians = radians;
    }

    public int CompareTo(AngleF other) => Radians.CompareTo(other.Radians);

    public static AngleF FromRadians(float radians) => new(radians);
    public static AngleF FromDegrees(float degrees) => FromRadians(float.DegreesToRadians(degrees));

    public static AngleF Clamp(AngleF value, AngleF min, AngleF max) => new(float.Clamp(value.Radians, min.Radians, max.Radians));

    public static AngleF operator -(AngleF angle) => new(-angle.Radians);
    public static AngleF operator +(AngleF left, AngleF right) => new(left.Radians + right.Radians);
    public static AngleF operator -(AngleF left, AngleF right) => new(left.Radians - right.Radians);
    public static AngleF operator *(AngleF angle, float factor) => new(angle.Radians * factor);
    public static AngleF operator *(float factor, AngleF angle) => new(angle.Radians * factor);
    public static AngleF operator /(AngleF angle, float factor) => new(angle.Radians / factor);
}
