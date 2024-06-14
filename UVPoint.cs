namespace Ametrin.Utils;

public readonly record struct UVPoint(double U, double V)
{
    public static readonly UVPoint Middle = new(0.5, 0.5);
    public static readonly UVPoint LeftTop = new(0, 0);
    public static readonly UVPoint RightTop = new(1, 0);
    public static readonly UVPoint LeftBottom = new(0, 1);
    public static readonly UVPoint RightBottom = new(1, 1);
}
