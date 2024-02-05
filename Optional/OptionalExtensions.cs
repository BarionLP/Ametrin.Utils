namespace Ametrin.Utils.Optional;


// modified from https://github.com/zoran-horvat/optional
public static class OptionalExtensions{
    public static Option<T> ToOption<T>(this T? obj) => obj is null ? Option<T>.None() : Option<T>.Some(obj);
    public static Option<T> ToOption<T>(this object? obj) => obj is T t ? Option<T>.Some(t) : Option<T>.None();

    //public static Option<T> Where<T>(this T? obj, Func<T, bool> predicate) =>
    //    obj is not null && predicate(obj) ? Option<T>.Some(obj) : Option<T>.None();

    //public static Option<T> WhereNot<T>(this T? obj, Func<T, bool> predicate) =>
    //    obj is not null && !predicate(obj) ? Option<T>.Some(obj) : Option<T>.None();

    public static IEnumerable<Option<T>> WhereSome<T>(this IEnumerable<Option<T>> source){
        return source.Where(option => option.HasValue);
    }
    public static IEnumerable<T> ReduceSome<T>(this IEnumerable<Option<T>> source){
        return source.Where(t => t.HasValue).Select(s => s.ReduceOrThrow());
    }
    public static IEnumerable<T> ReduceAll<T>(this IEnumerable<Option<T>> source, T @default){
        return source.Select(s=>s.Reduce(@default));
    }
    public static IEnumerable<TResult> SelectSome<TInput, TResult>(this IEnumerable<TInput> inputs, Func<TInput, Option<TResult>> action) {
        return inputs.Select(p => action(p)).ReduceSome();
    }

    public static Option<R> Map<R, T1, T2>(this (Option<T1> option1, Option<T2> option2) options, Func<T1, T2, R> map) {
        if(!options.option1.HasValue || !options.option2.HasValue) return Option<R>.None();
        return Option<R>.Some(map(options.option1.ReduceOrThrow(), options.option2.ReduceOrThrow()));
    }
}