using System.Diagnostics;

namespace Ametrin.Utils.Optional;

public readonly record struct Except<T> : IOptional<T>
{
    public bool HasValue { get; private init; }
    public bool IsSuccess => HasValue;
    public bool IsFail => !HasValue;
    public T? Value { get; private init; }
    public Exception? Error { get; private init; }

    public Except<T> Where(Func<T, bool> predicate) => Where(predicate, () => new Exception("Condition Failed"));
    public Except<T> Where(Func<T, bool> predicate, Func<Exception> errorSupplier) => IsSuccess ? (predicate(Value!) ? this : errorSupplier()) : this;
    public Except<T> WhereNot(Func<T, bool> predicate) => WhereNot(predicate, () => new Exception("Contition Failed"));
    public Except<T> WhereNot(Func<T, bool> predicate, Func<Exception> errorSupplier) => IsSuccess ? (!predicate(Value!) ? this : errorSupplier()) : this;

    public Except<TResult> Map<TResult>(Func<T, TResult> map)
        => IsSuccess ? map(Value!) : Error!;
    public Except<TResult> Map<TResult>(Func<T, IOptional<TResult>> map)
        => IsSuccess ? Except<TResult>.Of(map(Value!), () => new Exception()) : Error!;
    public Except<TResult> Map<TResult>(Func<T, IOptional<TResult>> map, Func<Exception> errorSupplier)
        => IsSuccess ? Except<TResult>.Of(map(Value!), errorSupplier) : Error!;
    public Except<TResult> Cast<TResult>() => Cast<TResult>(() => new InvalidCastException());
    public Except<TResult> Cast<TResult>(Func<Exception> errorSupplier)
        => IsFail ? Error!
        : Value is not TResult casted ? errorSupplier()
        : casted;

    public TResult MapReduce<TResult>(Func<T, TResult> map, Func<Exception, TResult> defaultSupplier) => IsSuccess ? map(Value!) : defaultSupplier(Error!);

    public T Reduce(Func<Exception, T> defaultSupplier) => IsSuccess ? Value! : defaultSupplier(Error!);
    public T ReduceOrThrow() => IsSuccess ? Value! : throw new NullReferenceException($"Result has Failed", Error);

    public void Resolve(Action<T> action, Action<Exception> failed)
    {
        if(IsSuccess)
            action(Value!);
        else
            failed.Invoke(Error!);
    }

    public static Except<T> Success(T value) => value is not null ? new() { HasValue = true, Value = value } : throw new ArgumentNullException(nameof(value), "Cannot create Result with null value");
    public static Except<T> Fail(Exception error) => new() { HasValue = false, Error = error };
    public static Except<T> Of(T? value) => Of(value, () => new NullReferenceException());
    public static Except<T> Of(T? value, Func<Exception> errorSupplier) => value is not null ? Success(value) : Fail(errorSupplier());
    public static Except<T> Of(IOptional<T> optional, Func<Exception> errorSupplier) => optional switch
    {
        Except<T> option => option,
        IOptional<T> option when option.HasValue => Success(option.Value!),
        IOptional<T> option when !option.HasValue => Fail(errorSupplier()),
        _ => throw new UnreachableException(),
    };

    public static Except<T> Try(Func<T> func)
    {
        try
        {
            return Success(func());
        }
        catch(Exception e)
        {
            return Fail(e);
        }
    }

    IOptional<T> IOptional<T>.Where(Func<T, bool> predicate) => Where(predicate);
    IOptional<T> IOptional<T>.WhereNot(Func<T, bool> predicate) => WhereNot(predicate);
    IOptional<TResult> IOptional<T>.Map<TResult>(Func<T, TResult> map) => Map(map);
    IOptional<TResult> IOptional<T>.Cast<TResult>() => Cast<TResult>();

    public static implicit operator Except<T>(T? value) => Of(value);
    public static implicit operator Except<T>(Exception error) => Fail(error);

}