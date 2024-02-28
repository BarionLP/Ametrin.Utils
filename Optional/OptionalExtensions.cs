namespace Ametrin.Utils.Optional;


public static class OptionalExtensions{
    public static Option<T> ToOption<T>(this T? obj) => Option<T>.Some(obj);
    public static Option<T> ToOption<T>(this IOptional<T> optional) => Option<T>.Of(optional);
    public static Option<T> ToOptionWhereExists<T>(this T? directoryInfo) where T : FileSystemInfo => directoryInfo.ToOption().Where(dir => dir.Exists);

    public static Result<T> ToResult<T>(this T? obj, ResultFlag flag = ResultFlag.Null) => obj is not null ? Result<T>.Success(obj) : Result<T>.Fail(flag);
    public static Result<T> ToResult<T>(this IOptional<T> optional) => Result<T>.Of(optional);
    public static Result<T> ToResultWhereExists<T>(this T? fileSystemInfo) where T : FileSystemInfo => fileSystemInfo.ToResult().Where(dir => dir.Exists, ResultFlag.PathNotFound);

    public static TResult MapReduce<T, TResult>(this IOptional<T> optional, Func<T, TResult> map, TResult defaultValue) => optional.HasValue ? map(optional.Value!) : defaultValue;
    public static TResult MapReduce<T, TResult>(this IOptional<T> optional, Func<T, TResult> map, Func<TResult> defaultSupplier) => optional.HasValue ? map(optional.Value!) : defaultSupplier();

    public static T Reduce<T>(this IOptional<T> optional, T defaultValue) => optional.HasValue ? optional.Value! : defaultValue;
    public static T Reduce<T>(this IOptional<T> optional, Func<T> defaultSupplier) => optional.HasValue ? optional.Value! : defaultSupplier();
    public static T? ReduceOrDefault<T>(this IOptional<T> optional) => optional.HasValue ? optional.Value : default;

    public static void Resolve<T>(this IOptional<T> optional, Action<T> action, Action? failed = null){
        if (optional.HasValue) action(optional.Value!);
        else failed?.Invoke();
    }


    public static T? ReduceOrDefault<T>(this Result<T> option) => option.IsSuccess ? option.ReduceOrThrow() : default;
    public static T? ReduceOrNull<T>(this Result<T> option) where T : struct => option.IsSuccess ? option.ReduceOrThrow() : null;

    public static Option<R> Map<R, T1, T2>(this (Option<T1> option1, Option<T2> option2) options, Func<T1, T2, R> map) {
        if(!options.option1.HasValue || !options.option2.HasValue) return Option<R>.None();
        return Option<R>.Some(map(options.option1.ReduceOrThrow(), options.option2.ReduceOrThrow()));
    }
    
    public static Option<R> Map<R, T1, T2>(this (T1?, T2?) items, Func<T1, T2, R> map) {
        if(items.Item1 is null || items.Item2 is null) return Option<R>.None();
        return Option<R>.Some(map(items.Item1, items.Item2));
    }
}