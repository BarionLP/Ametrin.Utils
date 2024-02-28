using System.Diagnostics;

namespace Ametrin.Utils.Optional;

public readonly record struct Result<T> : IOptional<T>{
    public bool HasValue => Flag.IsSuccess();
    public bool IsSuccess => Flag.IsSuccess();
    public bool IsFail => Flag.IsFail();

    public T? Value { get; private init; }
    public ResultFlag Flag { get; private init; }

    public Result<T> Where(Func<T, bool> predicate, ResultFlag flag = ResultFlag.Failed) => IsSuccess ? (predicate(Value!) ? this : Fail(flag)) : Fail(Flag);
    public Result<T> WhereNot(Func<T, bool> predicate, ResultFlag flag = ResultFlag.Failed) => IsSuccess ? (!predicate(Value!) ? this : Fail(flag)) : Fail(Flag);

    public Result<TResult> Map<TResult>(Func<T, TResult> map)
        => HasValue ? Result<TResult>.Success(map(Value!)) : Result<TResult>.Fail(Flag);
    public Result<TResult> Map<TResult>(Func<T, IOptional<TResult>> map)
        => HasValue ? Result<TResult>.Of(map(Value!)) : Result<TResult>.Fail(Flag);
    public Result<TResult> Map<TResult>(Func<T, IOptional<TResult>> map, ResultFlag flag)
        => HasValue ? Result<TResult>.Of(map(Value!), flag) : Result<TResult>.Fail(Flag);
    public Result<TResult> Cast<TResult>(ResultFlag flag = ResultFlag.InvalidType) 
        => IsFail ? Result<TResult>.Fail(Flag) 
        : Value is not TResult casted ? Result<TResult>.Fail(flag) 
        : Result<TResult>.Success(casted);

    public TResult MapReduce<TResult>(Func<T, TResult> map, Func<ResultFlag, TResult> defaultSupplier) => IsSuccess ? map(Value!) : defaultSupplier(Flag);

    public T Reduce(Func<ResultFlag, T> defaultSupplier) => IsSuccess ? Value! : defaultSupplier(Flag);
    public T ReduceOrThrow() => IsSuccess ? Value! : throw new NullReferenceException($"Result has Failed: {Flag}");

    public void Resolve(Action<T> action, Action<ResultFlag> failed){
        if (IsSuccess) action(Value!);
        failed.Invoke(Flag);
    }

    public static Result<T> Success(T value) => value is not null ? new() { Value = value, Flag = ResultFlag.Succeeded } : throw new ArgumentNullException(nameof(value), "Cannot create Result with null value");
    public static Result<T> Fail(ResultFlag flag = ResultFlag.Failed) => flag.IsFail() ? new() { Flag = flag } : throw new ArgumentException("Cannot Fail with Succeed flag");
    public static Result<T> Of(T? value, ResultFlag flag = ResultFlag.Null) => value is not null ? Success(value) : Fail(flag);
    public static Result<T> Of(IOptional<T> optional, ResultFlag flag = ResultFlag.Null) => optional switch
    {
        Result<T> option => option,
        IOptional<T> option when option.HasValue => Success(option.Value!),
        IOptional<T> option when !option.HasValue => Fail(flag),
        _ => throw new UnreachableException(),
    };

    IOptional<T> IOptional<T>.Where(Func<T, bool> predicate) => Where(predicate);
    IOptional<T> IOptional<T>.WhereNot(Func<T, bool> predicate) => WhereNot(predicate);
    IOptional<TResult> IOptional<T>.Map<TResult>(Func<T, TResult> map) => Map(map);
    IOptional<TResult> IOptional<T>.Map<TResult>(Func<T, IOptional<TResult>> map) => Map(map);
    IOptional<TResult> IOptional<T>.Cast<TResult>() => Cast<TResult>();

    public override string ToString() => IsSuccess ? Value!.ToString() ?? "NullString" : "None";
    public override int GetHashCode() => IsSuccess ? HashCode.Combine(Value!.GetHashCode(), Flag.GetHashCode()) : HashCode.Combine(0, Flag.GetHashCode());
}

public static class ResultExtensions {
    public static bool IsFail(this ResultFlag flag) => flag.HasFlag(ResultFlag.Failed);
    public static bool IsSuccess(this ResultFlag flag) => flag is ResultFlag.Succeeded;
}


[Flags] //for fails first bit must be 1
public enum ResultFlag {
    Succeeded           = 0b0000000000000000000000000000000,
    Failed              = 0b1000000000000000000000000000000,
    InvalidArgument     = 0b1000000000000000000000000000001,
    IOError             = 0b1000000000000000000000000000010,
    WebError            = 0b1000000000000000000000000000100,
    Null                = 0b1000000000000000000000000001000,
    ConnectionFailed    = 0b1000000000000000000000000010000,
    AlreadyExists       = 0b1000000000000000000000000100000,
    Canceled            = 0b1000000000000000000000001000000,
    OutOfRange          = 0b1000000000000000000000010000000,
    AccessDenied        = 0b1000000000000000000001000000000,
    InvalidType         = 0b1000000000000000000010000000000,
    Impossible          = 0b1100000000000000000000000000000,
    PathNotFound        = IOError | Null,
    PathAlreadyExists   = IOError | AlreadyExists,
    NoInternet          = WebError | ConnectionFailed,
    InvalidFile         = IOError | InvalidArgument,
}
