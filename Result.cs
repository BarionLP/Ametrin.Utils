namespace Ametrin.Utils;

public sealed class Result<T> {
    public readonly ResultStatus Status = ResultStatus.Failed;
    private T? Value { get; } = default;

    private Result(ResultStatus status) {        
        Status = status;
    }
    private Result(T? value) : this(ResultStatus.Succeeded) {
        Value = value;
    }

    public static Result<T> Succeeded(in T value) {
        if(value is null) throw new ArgumentNullException(nameof(value), "Cannot succeed when result is null");
        return new Result<T>(value);
    }

    public static Result<T> Failed(ResultStatus status = ResultStatus.Failed) {
        if(status is ResultStatus.Succeeded) throw new ArgumentException("Cannot Succeed without value! Use Result.Succeeded", nameof(status));
        return new(status);
    }

    public void Resolve(Action<T> success, Action<ResultStatus>? failed = null) {
        if(HasFailed()) {
            failed?.Invoke(Status);
            return;
        }

        success(Value!);
    }
    
    public bool TryResolve(out T result) {
        result = Value!;
        return !HasFailed();
    }

    public TReturn Resolve<TReturn>(Func<T, TReturn> success, Func<ResultStatus, TReturn> failed) => HasFailed() ? failed(Status) : success(Value!);

    public TReturn Resolve<TReturn>(Func<T, TReturn> success, TReturn @default) => HasFailed() ? @default : success(Value!);

    public Result<TReturn> IfPresent<TReturn>(Func<T, TReturn> operation) => HasFailed() ? Result<TReturn>.Failed(Status) : (Result<TReturn>) operation(Value!);

    public T Get() {
        if(Value is null) throw new InvalidDataException("Trying to read a failed result! Validate or use GetOrDefault");
        return Value;
    }

    public T? GetOrDefault() => Value;
    public T GetOrDefault(T @default) => Value ?? @default;
    public bool HasFailed() => Status.HasFlag(ResultStatus.Failed);

    public static implicit operator Result<T>(ResultStatus status) => Result<T>.Failed(status);
    public static implicit operator Result<T>(T? value) => value is null ? Result<T>.Failed(ResultStatus.ResultNull) : Result<T>.Succeeded(value);
}

public sealed class Result {
    public readonly ResultStatus Status = ResultStatus.Failed;

    private Result(ResultStatus status) {
        Status = status;
    }

    public void Resolve(Action success, Action<ResultStatus>? failed = null) {
        if(HasFailed()) {
            failed?.Invoke(Status);
            return;
        }

        success();
    }

    public TReturn Resolve<TReturn>(Func<TReturn> success, Func<ResultStatus, TReturn> failed) => HasFailed() ? failed(Status) : success();

    public TReturn Resolve<TReturn>(Func<TReturn> success, TReturn @default) => HasFailed() ? @default : success();

    public bool HasFailed() => Status.HasFlag(ResultStatus.Failed);
    public bool IsSuccess() => Status is ResultStatus.Succeeded;

    public static Result Of(ResultStatus status) => new(status);

    public static implicit operator Result(ResultStatus status) => Of(status);
}

[Flags] //for fails first bit must be 1
public enum ResultStatus {
    Succeeded           = 0b0000000000000000000000000000000,
    Failed              = 0b1000000000000000000000000000000,
    InvalidArgument     = 0b1000000000000000000000000000001,
    IOError             = 0b1000000000000000000000000000010,
    NotFound            = 0b1000000000000000000000000000100,
    ResultNull          = 0b1000000000000000000000000001000,
    ConnectionFailed    = 0b1000000000000000000000000010000,
    AlreadyExists       = 0b1000000000000000000000000100000,
    Canceled            = 0b1000000000000000000000001000000,
    OutOfRange          = 0b1000000000000000000000010000000
}
