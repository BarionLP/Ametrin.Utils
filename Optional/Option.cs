namespace Ametrin.Utils.Optional;

// based on https://github.com/zoran-horvat/optional
public readonly struct Option<T> : IEquatable<Option<T>>{
    private readonly T _content;
    public bool HasValue { get; }

    private Option(T content, bool hasValue) {
        _content = content;
        HasValue = hasValue;
    }

    public static Option<T> Some(T? obj) => obj is null ? None() : new(obj, true);
    public static Option<T> None() => new(default!, false);

    public readonly Option<TResult> Map<TResult>(Func<T, TResult> map) => HasValue ? map(_content) : Option<TResult>.None();
    public readonly Option<TResult> Map<TResult>(Func<T, Option<TResult>> map) => HasValue ? map(_content) : Option<TResult>.None();
    public readonly Result<TResult> Map<TResult>(Func<T, Result<TResult>> map) => HasValue ? map(_content) : Result<TResult>.Failed();

    public readonly Option<TResult> Cast<TResult>(){
        if(HasValue && _content is TResult castedContent){
            return Option<TResult>.Some(castedContent);
        }
        return Option<TResult>.None();
    }

    public readonly T Reduce(T orElse) => _content ?? orElse;
    public readonly T Reduce(Func<T> orElse) => _content ?? orElse();
    public readonly T ReduceOrThrow() => HasValue ? _content! : throw new NullReferenceException($"Option was empty");


    public readonly Option<T> Where(Func<T, bool> predicate) => HasValue && predicate(_content) ? this : None();
    public readonly Option<T> WhereNot(Func<T, bool> predicate) => HasValue && !predicate(_content) ? this : None();

    public readonly void Resolve(Action<T> success, Action? failed = null) {
        if(!HasValue) {
            failed?.Invoke();
            return;
        }

        success(_content!);
    }

    public Result<T> ToResult(ResultFlag failedStatus = ResultFlag.Failed) => HasValue ? Result<T>.Of(_content) : Result<T>.Failed(failedStatus);

    public override readonly int GetHashCode() => HasValue ? _content!.GetHashCode() : 0;
    public override readonly bool Equals(object? other) => other is Option<T> option && Equals(option);
    public readonly bool Equals(Option<T> other) {
        if(HasValue) {
            if(other.HasValue) {
                return _content!.Equals(other._content);
            }
            return false;
        }
        return !other.HasValue;
    }

    public override string ToString() => HasValue ? _content!.ToString() ?? "NullString" : "None";

    public static bool operator ==(Option<T> a, Option<T> b) => a.Equals(b);
    public static bool operator !=(Option<T> a, Option<T> b) => !(a == b);

    public static implicit operator Option<T>(T? value) => Some(value);
}