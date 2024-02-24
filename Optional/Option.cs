using Microsoft.VisualBasic.FileIO;

namespace Ametrin.Utils.Optional;

// based on https://github.com/zoran-horvat/optional
public readonly record struct Option<T> : IOption<T>, IComparable<Option<T>> {
    private readonly T _content;
    public readonly bool HasValue { get; }


    private Option(T content, bool hasValue) {
        _content = content;
        HasValue = hasValue;
    }

    public static Option<T> Some(T? obj) => obj is null ? None() : new(obj, true);
    public static Option<T> None() => new(default!, false);

    public readonly Option<TResult> Map<TResult>(Func<T, TResult> map) => HasValue ? map(_content) : Option<TResult>.None();
    public readonly Option<TResult> Map<TResult>(Func<T, Option<TResult>> map) => HasValue ? map(_content) : Option<TResult>.None();
    public readonly Result<TResult> Map<TResult>(Func<T, Result<TResult>> map, ResultFlag defaultFlag = ResultFlag.Failed) => HasValue ? map(_content) : Result<TResult>.Failed(defaultFlag);

    public readonly Option<TResult> Cast<TResult>(){
        if(HasValue && _content is TResult castedContent){
            return Option<TResult>.Some(castedContent);
        }
        return Option<TResult>.None();
    }
    public readonly TResult Map<TResult>(Func<T, TResult> map, TResult orElse) => HasValue ? map(_content) : orElse;
    public readonly TResult Map<TResult>(Func<T, TResult> map, Func<TResult> orElse) => HasValue ? map(_content) : orElse();

    public readonly T Reduce(T orElse) => HasValue ? _content : orElse;
    public readonly T Reduce(Func<T> orElse) => HasValue ? _content : orElse();
    public readonly T ReduceOrThrow() => HasValue ? _content : throw new NullReferenceException($"Option was empty");


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

    public override string ToString() => HasValue ? _content!.ToString() ?? "NullString" : "None";
    public bool Equals(T? other) => other is null ? !HasValue : HasValue && EqualityComparer<T>.Default.Equals(_content, other);
    public bool Equals(Option<T> other) => HasValue ? other.HasValue && EqualityComparer<T>.Default.Equals(_content, other._content) : !other.HasValue;
    public override readonly int GetHashCode() => HasValue ? _content!.GetHashCode() : 0;

    int IComparable.CompareTo(object? obj) => obj is Option<T> o ? CompareTo(o) : obj is T t ? CompareTo(t) : 1;
    public int CompareTo(Option<T> other) {
        if(!other.HasValue) return HasValue ? 1 : 0;
        if(!HasValue) return -1;

        return CompareTo(other._content);
    }

    T IOption<T>.Content => _content;

    static IOption<T> IOption<T>.Some(T? obj) => Some(obj);
    static IOption<T> IOption<T>.None() => None();
    IOption<T> IOption<T>.Where(Func<T, bool> predicate) => Where(predicate);
    IOption<T> IOption<T>.WhereNot(Func<T, bool> predicate) => WhereNot(predicate);


    public static implicit operator Option<T>(T? value) => Some(value);
}
