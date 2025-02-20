using System.Diagnostics;

namespace Ametrin.Utils.Optional;

[Obsolete]
public readonly record struct Result<T> : IOptional<T>
{
    public bool IsSuccess => ResultFlag.IsSuccess();
    public bool IsFail => ResultFlag.IsFail();
    bool IOptional<T>.HasValue => IsSuccess;

    public T? Value { get; private init; }
    public ResultFlag ResultFlag { get; private init; }

    public Result<T> Where(Func<T, bool> predicate, ResultFlag flag = ResultFlag.Failed) => IsSuccess ? (predicate(Value!) ? this : Fail(flag)) : Fail(ResultFlag);
    public Result<T> WhereNot(Func<T, bool> predicate, ResultFlag flag = ResultFlag.Failed) => IsSuccess ? (!predicate(Value!) ? this : Fail(flag)) : Fail(ResultFlag);

    public Result<TResult> Map<TResult>(Func<T, TResult?> map, ResultFlag flag = ResultFlag.NullOrEmpty)
        => IsSuccess ? Result<TResult>.Of(map(Value!), flag) : ResultFlag;
    public Result<TResult> Map<TResult>(Func<T, TResult> map)
        => IsSuccess ? map(Value!) : ResultFlag;
    public Result<TResult> Map<TResult>(Func<T, IOptional<TResult>> map)
        => IsSuccess ? Result<TResult>.Of(map(Value!)) : ResultFlag;
    //public Result<TResult> Map<TResult>(Func<T, IOptional<TResult>> map, ResultFlag flag)
    //    => IsSuccess ? Result<TResult>.Of(map(Value!), flag) : ResultFlag;

    public Result<TResult> Map<TResult>(Func<T, Result<TResult>> map)
        => IsSuccess ? map(Value!) : ResultFlag;
    public Result<TResult> Map<TResult>(Func<T, Option<TResult>> map, ResultFlag flag = ResultFlag.Failed)
        => IsSuccess ? Result<TResult>.Of(map(Value!), flag) : ResultFlag;
    public Result<TResult> Cast<TResult>(ResultFlag flag = ResultFlag.InvalidType)
        => IsFail ? ResultFlag
        : Value is not TResult casted ? flag
        : casted;

    public TResult MapReduce<TResult>(Func<T, TResult> map, Func<ResultFlag, TResult> defaultSupplier) => IsSuccess ? map(Value!) : defaultSupplier(ResultFlag);

    public T Reduce(Func<ResultFlag, T> defaultSupplier) => IsSuccess ? Value! : defaultSupplier(ResultFlag);
    public T ReduceOrThrow() => IsSuccess ? Value! : throw new NullReferenceException($"Result has Failed: {ResultFlag}");

    public void Resolve(Action<T> action, Action<ResultFlag> failed)
    {
        if (IsSuccess)
            action(Value!);
        else
            failed.Invoke(ResultFlag);
    }

    public static Result<T> Success(T value) => value is not null ? new() { Value = value, ResultFlag = ResultFlag.Succeeded } : throw new ArgumentNullException(nameof(value), "Cannot create Result with null value");
    public static Result<T> Fail(ResultFlag flag = ResultFlag.Failed) => flag.IsFail() ? new() { ResultFlag = flag } : throw new ArgumentException("Cannot Fail with Succeed flag");
    public static Result<T> Of(T? value, ResultFlag flag = ResultFlag.NullOrEmpty) => value is not null ? Success(value) : Fail(flag);
    public static Result<T> Of(IOptional<T> optional, ResultFlag flag = ResultFlag.NullOrEmpty) => optional switch
    {
        Result<T> option => option,
        IOptional<T> option when option.HasValue => Success(option.Value!),
        IOptional<T> option when !option.HasValue => Fail(flag),
        _ => throw new UnreachableException(),
    };

    IOptional<T> IOptional<T>.Where(Func<T, bool> predicate) => Where(predicate);
    IOptional<T> IOptional<T>.WhereNot(Func<T, bool> predicate) => WhereNot(predicate);
    IOptional<TResult> IOptional<T>.Map<TResult>(Func<T, TResult> map) => Map(map);
    IOptional<TResult> IOptional<T>.Cast<TResult>() => Cast<TResult>();

    public override string ToString() => IsSuccess ? Value!.ToString() ?? "NullString" : ResultFlag.ToString();
    public override int GetHashCode() => IsSuccess ? HashCode.Combine(Value!.GetHashCode(), ResultFlag.GetHashCode()) : HashCode.Combine(0, ResultFlag.GetHashCode());

    public static implicit operator Result<T>(ResultFlag flag) => Fail(flag);
    public static implicit operator Result<T>(T? obj) => Of(obj);
}