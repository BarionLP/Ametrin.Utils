namespace Ametrin.Utils.Transformation;

public static class Transformer {
    public static double[] ApplySmooth(this Transform2D transform, double[] array, int width, double defaultValue = 0){
        return ((FlatTransformable<double>)transform.ApplySmooth(new FlatTransformable<double>(array, width), defaultValue)).Array;
    }
    public static ITransformable2D<TData> ApplySmooth<TData>(this Transform2D transform, ISmoothTransformable2D<TData> transformable, TData defaultValue){
        // x/y : 0..Width/Height
        // u/v : 0..1
        var transformed = transformable.CreateTarget();
        if(transform.Scale == 0) return transformed;

        (double x, double y) iHat = (Math.Cos(transform.Rotation.Radians) / transform.Scale, Math.Sin(transform.Rotation.Radians) / transform.Scale);
        (double x, double y) jHat = (-iHat.y, iHat.x);
        foreach (int y in ..transformable.Height) {
            foreach (int x in ..transformable.Width) {
                var (u, v) = transformable.ProjectToUV(x, y);

                double uTransformed = iHat.x * (u - transform.Center.U) + jHat.x * (v - transform.Center.U) + transform.Center.U;
                double vTransformed = iHat.y * (u - transform.Center.V) + jHat.y * (v - transform.Center.V) + transform.Center.V;

                var (xTransformed, yTransformed) = transformable.ProjectFromUV(uTransformed, vTransformed);
                xTransformed += transform.Offset.x; //positive means right
                yTransformed -= transform.Offset.y; //positive means up

                if(transform.Wrap){
                    xTransformed = xTransformed % transformable.Width;
                    yTransformed = yTransformed % transformable.Height;
                }

                transformed[x, y] = transformable.IsInBounds(xTransformed, yTransformed) ? transformable.Sample(xTransformed, yTransformed) : defaultValue;
            }
        }
        
        return transformed;
    }
}
