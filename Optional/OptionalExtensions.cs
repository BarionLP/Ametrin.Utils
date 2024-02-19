namespace Ametrin.Utils.Optional;


public static class OptionalExtensions{
    public static Option<T> ToOption<T>(this T? obj) where T : class => obj is null ? Option<T>.None() : Option<T>.Some(obj);
    public static Option<T> ToOption<T>(this T? obj) where T : struct => obj.HasValue ? Option<T>.Some(obj.Value) : Option<T>.None();
    public static Option<T> ToOption<T>(this object? obj) => obj is T t ? Option<T>.Some(t) : Option<T>.None();
    public static T? ReduceOrDefault<T>(this Option<T> option) => option.HasValue ? option.ReduceOrThrow() : default;
    public static T? ReduceOrNull<T>(this Option<T> option) where T : struct => option.HasValue ? option.ReduceOrThrow() : null;

    public static T? ReduceOrDefault<T>(this Result<T> option) => option.IsSuccess ? option.ReduceOrThrow() : default;
    public static T? ReduceOrNull<T>(this Result<T> option) where T : struct => option.IsSuccess ? option.ReduceOrThrow() : null;


    public static IEnumerable<Option<T>> WhereSome<T>(this IEnumerable<Option<T>> source) => source.Where(option => option.HasValue);
    public static IEnumerable<T> ReduceSome<T>(this IEnumerable<Option<T>> source) => source.Where(t => t.HasValue).Select(s => s.ReduceOrThrow());
    public static IEnumerable<T> Reduce<T>(this IEnumerable<Option<T>> source, T @default) => source.Select(s => s.Reduce(@default));
    public static IEnumerable<TResult> SelectSome<TInput, TResult>(this IEnumerable<TInput> source, Func<TInput, Option<TResult>> action) => source.Select(p => action(p)).ReduceSome();
    
    public static IEnumerable<Result<T>> WhereSuccess<T>(this IEnumerable<Result<T>> source) => source.Where(result => result.IsSuccess);
    public static IEnumerable<T> ReduceSuccess<T>(this IEnumerable<Result<T>> source) => source.WhereSuccess().Select(s => s.ReduceOrThrow());
    public static IEnumerable<T> Reduce<T>(this IEnumerable<Result<T>> source, T @default) => source.Select(s => s.Reduce(@default));
    public static IEnumerable<TResult> SelectSuccess<TInput, TResult>(this IEnumerable<TInput> source, Func<TInput, Result<TResult>> action) => source.Select(p => action(p)).ReduceSuccess();

    public static Option<R> Map<R, T1, T2>(this (Option<T1> option1, Option<T2> option2) options, Func<T1, T2, R> map) {
        if(!options.option1.HasValue || !options.option2.HasValue) return Option<R>.None();
        return Option<R>.Some(map(options.option1.ReduceOrThrow(), options.option2.ReduceOrThrow()));
    }
    
    public static Option<R> Map<R, T1, T2>(this (T1?, T2?) items, Func<T1, T2, R> map) {
        if(items.Item1 is null || items.Item2 is null) return Option<R>.None();
        return Option<R>.Some(map(items.Item1, items.Item2));
    }
}