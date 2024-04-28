namespace Ametrin.Utils.Transformation;

public interface ITransformable2D<TData> {
    public int Width { get; }
    public int Height { get; }
    public TData this[Index x, Index y] { get; set; }
    internal ITransformable2D<TData> CreateTarget();

    public bool IsInBounds(double x, double y) => x >= 0 && x < Width && y >= 0 && y < Height;
    public (double x, double y) ProjectFromUV(double u, double v){
        double x = u * (Width - 1);
        double y = v * (Height - 1);
        return (x, y);
    }
    public (double u, double v) ProjectToUV(double x, double y){
        double u = x / (Width - 1);
        double v = y / (Height - 1);
        return (u, v);
    }
}

public interface ISmoothTransformable2D<TData> : ITransformable2D<TData> {
    public TData Sample(double x, double y);
    //public TData SampleUV(double u, double v);
}
