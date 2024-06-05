namespace Ametrin.Utils.Transformation;

public sealed class Transform2D
{
    public double Scale { get; set; } = 1;
    public Angle Rotation { get; set; } = Angle.Zero;
    public (int x, int y) Offset { get; set; } = (0, 0);
    public UVPoint Center { get; set; } = UVPoint.Middle;
    public bool Wrap { get; set; } = false;
}
