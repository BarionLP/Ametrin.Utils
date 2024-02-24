namespace Ametrin.Utils.Optional;

public interface IOption<T, TOption> : IEquatable<T>, IComparable<T>, IComparable where TOption : IOption<T, TOption>{
    public bool HasValue { get; }
    internal T? Content { get; }

    public static abstract TOption Some(T? obj);
    public static abstract TOption None();

    public T ReduceOrThrow();
}
