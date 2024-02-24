namespace Ametrin.Utils.Optional;

public interface IOption<T> : IEquatable<T>, IComparable<T>, IComparable{
    public bool HasValue { get; }
    public T? Content { get; }

    public static abstract IOption<T> Some(T? obj);
    public static abstract IOption<T> None();

    public T ReduceOrThrow();

    bool IEquatable<T>.Equals(T? other) => other is null ? !HasValue : HasValue && EqualityComparer<T>.Default.Equals(Content, other);
    int IComparable<T>.CompareTo(T? other) {
        if(other is null) return HasValue ? 1 : 0;
        if(!HasValue) return -1;

        return Content switch {
            IComparable<T> c => c.CompareTo(other),
            IComparable c => c.CompareTo(other),
            _ => throw new InvalidOperationException($"{typeof(T).Name} does not implement IComparable"),
        };
    }
}
