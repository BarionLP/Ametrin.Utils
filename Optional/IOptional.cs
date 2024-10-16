namespace Ametrin.Utils.Optional;

// The entire Optional Framework is based on Zoran Horvats Option type https://github.com/zoran-horvat/optional
[Obsolete]
public interface IOptional<T> : IEquatable<IOptional<T>>, IComparable<IOptional<T>>, IEquatable<T>, IComparable<T>, IComparable
{
    public bool HasValue { get; }
    public T? Value { get; }

    public IOptional<T> Where(Func<T, bool> predicate);
    public IOptional<T> WhereNot(Func<T, bool> predicate);

    public IOptional<TResult> Map<TResult>(Func<T, TResult> map);
    public IOptional<TResult> Cast<TResult>();

    public T ReduceOrThrow();

    bool IEquatable<IOptional<T>>.Equals(IOptional<T>? other) => OptionalHelper.Equals(this, other);
    int IComparable<IOptional<T>>.CompareTo(IOptional<T>? other) => OptionalHelper.CompareTo(this, other);
    bool IEquatable<T>.Equals(T? other) => other is null ? !HasValue : HasValue && EqualityComparer<T>.Default.Equals(Value, other);
    int IComparable.CompareTo(object? obj) => obj is IOptional<T> o ? CompareTo(o) : obj is T t ? CompareTo(t) : 1;
    int IComparable<T>.CompareTo(T? other)
    {
        if(other is null)
            return HasValue ? 1 : 0;
        if(!HasValue)
            return -1;

        return Value switch
        {
            IComparable<T> c => c.CompareTo(other),
            IComparable c => c.CompareTo(other),
            _ => throw new InvalidOperationException($"{typeof(T).Name} does not implement IComparable"),
        };
    }
}
