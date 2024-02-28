using System.Collections;

namespace Ametrin.Utils.Optional;

public static class OptionalLinqExtensions {
    public static Option<T> FirstOrNone<T>(this IEnumerable<T> source) => source.Any() ? source.First() : Option<T>.None();
    public static IEnumerable<TOption> WhereSome<T, TOption>(this IEnumerable<TOption> source) where TOption : IOptional<T> 
        => source.Where(option => option.HasValue);
    public static IEnumerable<T> ReduceSome<T, TOption>(this IEnumerable<TOption> source) where TOption : IOptional<T> 
        => source.Where(t => t.HasValue).Select(s => s.ReduceOrThrow());
    public static IEnumerable<T> Reduce<T, TOption>(this IEnumerable<TOption> source, T @default) where TOption : IOptional<T> 
        => source.Select(s => s.Reduce(@default));
    public static IEnumerable<TResult> SelectSome<TInput, TResult, TOption>(this IEnumerable<TInput> source, Func<TInput, TOption> action) where TOption : IOptional<TResult> 
        => source.Select(p => action(p)).ReduceSome<TResult, TOption>();

    public static IEnumerable<T> ReduceSome<T>(this IEnumerable<Option<T>> source) => source.ReduceSome<T, Option<T>>();
}