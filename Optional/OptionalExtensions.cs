namespace Ametrin.Utils.Optional;


public static class OptionalExtensions{
    public static Option<T> ToOption<T>(this T? obj) where T : class 
        => Option<T>.Some(obj);
    public static Option<T> ToOption<T>(this T? obj) where T : struct
        // would return Option<int?> if done without explicit struct nullability
        => obj.HasValue ? Option<T>.Some(obj.Value) : Option<T>.None();
    public static Option<T> ToOption<T>(this IOptional<T> optional) 
        => Option<T>.Of(optional);
    public static Option<T> ToOptionWhereExists<T>(this T? directoryInfo) where T : FileSystemInfo 
        => directoryInfo.ToOption().Where(dir => dir.Exists);

    public static Result<T> ToResult<T>(this T? obj, ResultFlag flag = ResultFlag.Null) where T : class
        => obj is not null ? Result<T>.Success(obj) : Result<T>.Fail(flag);    
    public static Result<T> ToResult<T>(this T? obj, ResultFlag flag = ResultFlag.Null) where T : struct
        // would return Result<int?> if done without explicit struct nullability
        => obj.HasValue ? Result<T>.Success(obj.Value) : Result<T>.Fail(flag);
    public static Result<T> ToResult<T>(this IOptional<T> optional) 
        => Result<T>.Of(optional);
    public static Result<T> ToResultWhereExists<T>(this T? fileSystemInfo) where T : FileSystemInfo 
        => fileSystemInfo.ToResult().Where(dir => dir.Exists, ResultFlag.PathNotFound);

    public static TResult MapReduce<T, TResult>(this IOptional<T> optional, Func<T, TResult> map, TResult defaultValue) 
        => optional.HasValue ? map(optional.Value!) : defaultValue;
    public static TResult MapReduce<T, TResult>(this IOptional<T> optional, Func<T, TResult> map, Func<TResult> defaultSupplier) 
        => optional.HasValue ? map(optional.Value!) : defaultSupplier();

    public static T Reduce<T>(this IOptional<T> optional, T defaultValue) 
        => optional.HasValue ? optional.Value! : defaultValue;
    public static T Reduce<T>(this IOptional<T> optional, Func<T> defaultSupplier) 
        => optional.HasValue ? optional.Value! : defaultSupplier();
    public static T? ReduceOrDefault<T>(this IOptional<T> optional) 
        => optional.HasValue ? optional.Value : default;
    public static T? ReduceOrNull<T>(this IOptional<T> option) where T : struct 
        => option.HasValue ? option.Value : null;

    public static void Resolve<T>(this IOptional<T> optional, Action<T> action, Action? failed = null){
        if (optional.HasValue) action(optional.Value!);
        else failed?.Invoke();
    }

    public static Option<R> Map<R, T1, T2>(this (Option<T1>, Option<T2>) options, Func<T1, T2, R> map) {
        if(!options.Item1.HasValue || !options.Item2.HasValue) return Option<R>.None();
        return Option<R>.Some(map(options.Item1.Value!, options.Item2.Value!));
    }
    
    public static Option<R> Map<R, T1, T2>(this (T1?, T2?) items, Func<T1, T2, R> map) {
        if(items.Item1 is null || items.Item2 is null) return Option<R>.None();
        return Option<R>.Some(map(items.Item1, items.Item2));
    }
}