using Microsoft.VisualBasic.FileIO;

namespace Ametrin.Utils.Optional;


public static class OptionalExtensions{
    public static Option<T> ToOption<T>(this T? obj) where T : class => Option<T>.Some(obj);
    public static Option<T> ToOption<T>(this T? obj) where T : struct => obj.HasValue ? Option<T>.Some(obj.Value) : Option<T>.None();
    public static Option<T> ToOption<T>(this object? obj) => obj is T t ? Option<T>.Some(t) : Option<T>.None();
    public static Option<object> ToOption(this object? obj) => Option<object>.Some(obj);

    public static TOption Where<T, TOption>(TOption option, Func<T, bool> predicate) where TOption : IOption<T, TOption>
        => option.HasValue && predicate(option.Content!) ? option : TOption.None();
    public static TOption WhereNot<T, TOption>(TOption option, Func<T, bool> predicate) where TOption : IOption<T, TOption>
        => option.HasValue && !predicate(option.Content!) ? option : TOption.None();


    public static T Reduce<T>(this Result<T> option, Func<ResultFlag, T> defaultSupplier)
        => option.IsSuccess ? option.ReduceOrThrow() : defaultSupplier(option.Status);
    public static T Reduce<T, TOption>(this TOption option, Func<T> defaultSupplier) where TOption : IOption<T, TOption>
        => option.HasValue ? option.Content! : defaultSupplier();
    public static T Reduce<T, TOption>(this TOption option, T @default) where TOption : IOption<T, TOption> 
        => option.HasValue ? option.Content! : @default;

    public static T? ReduceOrDefault<T, TOption>(this TOption option) where TOption : IOption<T, TOption> 
        => option.HasValue ? option.Content : default;
    public static T? ReduceOrNull<T, TOption>(this TOption option) where T : struct where TOption : IOption<T, TOption>
        => option.HasValue ? option.Content : null;

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

public static class OptionalLinqExtensions {
    public static IEnumerable<TOption> WhereSome<T, TOption>(this IEnumerable<TOption> source) where TOption : IOption<T, TOption> 
        => source.Where(option => option.HasValue);
    public static IEnumerable<T> ReduceSome<T, TOption>(this IEnumerable<TOption> source) where TOption : IOption<T, TOption> 
        => source.Where(t => t.HasValue).Select(s => s.ReduceOrThrow());
    public static IEnumerable<T> Reduce<T, TOption>(this IEnumerable<TOption> source, T @default) where TOption : IOption<T, TOption> 
        => source.Select(s => s.Reduce(@default));
    public static IEnumerable<TResult> SelectSome<TInput, TResult, TOption>(this IEnumerable<TInput> source, Func<TInput, TOption> action) where TOption : IOption<TResult, TOption> 
        => source.Select(p => action(p)).ReduceSome<TResult, TOption>();

    //public static IEnumerable<Result<T>> WhereSuccess<T>(this IEnumerable<Result<T>> source) => source.Where(result => result.IsSuccess);
    //public static IEnumerable<T> ReduceSuccess<T>(this IEnumerable<Result<T>> source) => source.WhereSuccess().Select(s => s.ReduceOrThrow());
    //public static IEnumerable<T> Reduce<T>(this IEnumerable<Result<T>> source, T @default) => source.Select(s => s.Reduce(@default));
    //public static IEnumerable<TResult> SelectSuccess<TInput, TResult>(this IEnumerable<TInput> source, Func<TInput, Result<TResult>> action) => source.Select(p => action(p)).ReduceSuccess();
}