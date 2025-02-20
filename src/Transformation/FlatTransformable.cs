using System.Numerics;

namespace Ametrin.Utils.Transformation;

internal sealed class FlatTransformable<TData>(TData[] array, int width) : ISmoothTransformable2D<TData> where TData : IMultiplyOperators<TData, double, TData>, IAdditionOperators<TData, TData, TData>, ISubtractionOperators<TData, TData, TData>
{
    public TData[] Array { get; set; } = array;
    public int Width { get; } = width;
    public int Height { get; } = array.Length / width;
    public TData this[Index x, Index y]
    {
        get => Array[GetFlatIndex(x, y)];
        set => Array[GetFlatIndex(x, y)] = value;
    }

    public ITransformable2D<TData> CreateTarget() => new FlatTransformable<TData>(new TData[Array.Length], Width);

    private int GetFlatIndex(int x, int y) => x + y * Width;
    private int GetFlatIndex(Index ix, Index iy)
    {
        int x = ix.IsFromEnd ? Width - ix.Value : ix.Value;
        int y = iy.IsFromEnd ? Height - iy.Value : iy.Value;
        return GetFlatIndex(x, y);
    }

    public TData Sample(double x, double y)
    {
        int indexLeft = (int) x;
        int indexBottom = (int) y;
        int indexRight = Math.Min(indexLeft + 1, Width - 1);
        int indexTop = Math.Min(indexBottom + 1, Height - 1);

        var blendX = x - indexLeft;
        var blendY = y - indexBottom;

        var bottomLeft = this[indexLeft, indexBottom];
        var bottomRight = this[indexRight, indexBottom];
        var topLeft = this[indexLeft, indexTop];
        var topRight = this[indexRight, indexTop];

        var valueBottom = bottomLeft + (bottomRight - bottomLeft) * blendX;
        var valueTop = topLeft + (topRight - topLeft) * blendX;
        var interpolatedValue = valueBottom + (valueTop - valueBottom) * blendY;
        return interpolatedValue;
    }
}