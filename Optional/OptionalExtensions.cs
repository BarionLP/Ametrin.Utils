namespace Ametrin.Utils.Optional;


// modified from https://github.com/zoran-horvat/optional
public static class OptionalExtensions{
    public static Option<T> ToOption<T>(this T? obj) => obj is null ? Option<T>.None() : Option<T>.Some(obj);
    public static Option<T> ToOption<T>(this object? obj) => obj is T t ? Option<T>.Some(t) : Option<T>.None();

    //public static Option<T> Where<T>(this T? obj, Func<T, bool> predicate) =>
    //    obj is not null && predicate(obj) ? Option<T>.Some(obj) : Option<T>.None();

    //public static Option<T> WhereNot<T>(this T? obj, Func<T, bool> predicate) =>
    //    obj is not null && !predicate(obj) ? Option<T>.Some(obj) : Option<T>.None();

    public static IEnumerable<Option<T>> WhereNotEmpty<T>(this IEnumerable<Option<T>> source){
        return source.Where(option => option.HasValue);
    }
    public static IEnumerable<T> ReduceFiltered<T>(this IEnumerable<Option<T>> source){
        return source.Where(t => t.HasValue).Select(s => s.ReduceOrThrow());
    }
    public static IEnumerable<T> Reduce<T>(this IEnumerable<Option<T>> source, T @default){
        return source.Select(s=>s.Reduce(@default));
    }

    [Obsolete]
    public static IEnumerable<ValueOption<T>> WhereNotEmpty<T>(this IEnumerable<ValueOption<T>> source) where T : struct{
        return source.Where(option => option.HasValue);
    }
    [Obsolete]
    public static IEnumerable<T> ReduceFiltered<T>(this IEnumerable<ValueOption<T>> source) where T : struct{
        return source.Where(t => t.HasValue).Select(s=>s.ReduceOrThrow());
    }
    [Obsolete]
    public static IEnumerable<T> Reduce<T>(this IEnumerable<ValueOption<T>> source, T @default) where T : struct{
        return source.Select(s=>s.Reduce(@default));
    }
}