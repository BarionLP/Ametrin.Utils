namespace Ametrin.Utils.Optional;

// based on https://github.com/zoran-horvat/optional
public readonly struct Option<T> : IEquatable<Option<T>>{
    private readonly T _content;
    public bool HasValue { get; private init; }

    private Option(T content, bool hasValue) {
        _content = content;
        HasValue = hasValue;
    }


    public static Option<T> Some(T obj) {
        if(obj is null) return None();
        return new(obj, true);
    }
    public static Option<T> None() => new(default!, false);

    public readonly Option<TResult> Map<TResult>(Func<T, TResult> map) where TResult : class => HasValue ? map(_content) : Option<TResult>.None();
    public readonly Option<TResult> Map<TResult>(Func<T, Option<TResult>> map) where TResult : class => HasValue ? map(_content) : Option<TResult>.None();

    public readonly Option<TResult> Cast<TResult>() where TResult : class{
        if(HasValue && _content is TResult castedContent){
            return Option<TResult>.Some(castedContent);
        }
        return Option<TResult>.None();
    }

    public readonly T Reduce(T orElse) => _content ?? orElse;
    public readonly T Reduce(Func<T> orElse) => _content ?? orElse();
    public readonly T ReduceOrThrow() => HasValue ? _content! : throw new NullReferenceException($"Option was empty");


    public readonly Option<T> Where(Func<T, bool> predicate) => HasValue && predicate(_content) ? this : Option<T>.None();
    public readonly Option<T> WhereNot(Func<T, bool> predicate) => HasValue && !predicate(_content) ? this : Option<T>.None();

    public readonly void Resolve(Action<T> success, Action? failed = null) {
        if(!HasValue) {
            failed?.Invoke();
            return;
        }

        success(_content!);
    }

    public override readonly int GetHashCode() => HasValue ? _content!.GetHashCode() : 0;
    public override readonly bool Equals(object? other) => other is Option<T> option && Equals(option);
    public readonly bool Equals(Option<T> other) => HasValue ? other.HasValue : _content!.Equals(other._content);

    public static bool operator ==(Option<T>? a, Option<T>? b) => a is null ? b is null : a.Equals(b);
    public static bool operator !=(Option<T>? a, Option<T>? b) => !(a == b);

    public static implicit operator Option<T>(T value) => Option<T>.Some(value);
}