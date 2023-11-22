namespace Ametrin.Utils.Optional;

// from https://github.com/zoran-horvat/optional
public struct ValueOption<T> : IEquatable<ValueOption<T>> where T : struct{
    private T? _content;

    public static ValueOption<T> Some(T obj) => new() { _content = obj };
    public static ValueOption<T> None() => new();

    public readonly Option<TResult> Map<TResult>(Func<T, TResult> map) where TResult : class =>
        _content.HasValue ? Option<TResult>.Some(map(_content.Value)) : Option<TResult>.None();
    public readonly ValueOption<TResult> MapValue<TResult>(Func<T, TResult> map) where TResult : struct =>
        new() { _content = _content.HasValue ? map(_content.Value) : null };

    public readonly Option<TResult> Map<TResult>(Func<T, Option<TResult>> map) where TResult : class =>
        _content.HasValue ? map(_content.Value) : Option<TResult>.None();
    public readonly ValueOption<TResult> Map<TResult>(Func<T, ValueOption<TResult>> map) where TResult : struct =>
        _content.HasValue ? map(_content.Value) : ValueOption<TResult>.None();

    public readonly T Reduce(T orElse) => _content ?? orElse;
    public readonly T Reduce(Func<T> orElse) => _content ?? orElse();

    public readonly ValueOption<T> Where(Func<T, bool> predicate) =>
        _content.HasValue && predicate(_content.Value) ? this : ValueOption<T>.None();

    public readonly ValueOption<T> WhereNot(Func<T, bool> predicate) =>
        _content.HasValue && !predicate(_content.Value) ? this : ValueOption<T>.None();

    public readonly override int GetHashCode() => _content?.GetHashCode() ?? 0;
    public readonly override bool Equals(object? other) => other is ValueOption<T> option && Equals(option);

    public readonly bool Equals(ValueOption<T> other) =>
        _content.HasValue ? other._content.HasValue && _content.Value.Equals(other._content.Value)
        : !other._content.HasValue;

    public static bool operator ==(ValueOption<T> a, ValueOption<T> b) => a.Equals(b);
    public static bool operator !=(ValueOption<T> a, ValueOption<T> b) => !(a.Equals(b));
}