using System.Diagnostics;

namespace Ametrin.Utils.Optional;

// based on https://github.com/zoran-horvat/optional
public readonly record struct Option<T> : IOptional<T>{
    public bool HasValue { get; private init; }
    public T? Value { get; private init; }

    public Option<T> Where(Func<T, bool> predicate) => HasValue ? predicate(Value!) ? this : None() : this;
    public Option<T> WhereNot(Func<T, bool> predicate) => HasValue ? !predicate(Value!) ? this : None() : this;

    public Option<TResult> Map<TResult>(Func<T, TResult> map) => HasValue ? Option<TResult>.Some(map(Value!)) : Option<TResult>.None();
    public Option<TResult> Map<TResult>(Func<T, Option<TResult>> map) => HasValue ? Option<TResult>.Of(map(Value!)) : Option<TResult>.None();
    public Option<TResult> Map<TResult>(Func<T, IOptional<TResult>> map) => HasValue ? Option<TResult>.Of(map(Value!)) : Option<TResult>.None();
    public Option<TResult> Cast<TResult>(){
        if (HasValue && Value is TResult casted){
            return Option<TResult>.Some(casted);
        }
        return Option<TResult>.None();
    }

    public T ReduceOrThrow() => HasValue ? Value! : throw new NullReferenceException("Option was empty");

    public static Option<T> Some(T? value) => value is T t ? new() { HasValue = true, Value = t } : None();
    public static Option<T> None() => new() { HasValue = false };
    public static Option<T> Of(IOptional<T> optional){
        return optional switch{
            Option<T> option => option,
            IOptional<T> option when option.HasValue => Some(option.Value),
            IOptional<T> option when !option.HasValue => None(),
            _ => throw new UnreachableException(),
        };
    }

    IOptional<T> IOptional<T>.Where(Func<T, bool> predicate) => Where(predicate);
    IOptional<T> IOptional<T>.WhereNot(Func<T, bool> predicate) => WhereNot(predicate);
    IOptional<TResult> IOptional<T>.Map<TResult>(Func<T, IOptional<TResult>> map) => Map(map);
    IOptional<TResult> IOptional<T>.Map<TResult>(Func<T, TResult> map) => Map(map);
    IOptional<TResult> IOptional<T>.Cast<TResult>() => Cast<TResult>();

    public override string ToString() => HasValue ? Value!.ToString() ?? "NoString" : "None";
    public override readonly int GetHashCode() => HasValue ? Value!.GetHashCode() : 0;

    public static implicit operator Option<T>(T? obj) => Some(obj);
}
