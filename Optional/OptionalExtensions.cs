namespace Ametrin.Utils.Optional;


// from https://github.com/zoran-horvat/optional
public static class OptionalExtensions{
    public static Option<T> ToOption<T>(this T? obj) where T : class =>
        obj is null ? Option<T>.None() : Option<T>.Some(obj);

    //public static Option<T> Where<T>(this T? obj, Func<T, bool> predicate) where T : class =>
    //    obj is not null && predicate(obj) ? Option<T>.Some(obj) : Option<T>.None();

    //public static Option<T> WhereNot<T>(this T? obj, Func<T, bool> predicate) where T : class =>
    //    obj is not null && !predicate(obj) ? Option<T>.Some(obj) : Option<T>.None();

    public static IEnumerable<Option<T>> WhereNotEmpty<T>(this IEnumerable<Option<T>> source) where T : class{
        return source.Where(option => option.Reduce((T)null!) is not null);
    }
    public static IEnumerable<T> ReduceFiltered<T>(this IEnumerable<Option<T>> source) where T : class{
        return source.Select(s=>s.Reduce((T)null!)).Where(t=> t is not null);
    }
}