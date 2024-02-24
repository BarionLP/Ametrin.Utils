namespace Ametrin.Utils.Optional;

public readonly struct Result<T> : IOption<T>, IEquatable<Result<T>>, IComparable<Result<T>> {
    public required ResultFlag Status { get; init; }
    private T? _content { get; init; }

    public static Result<T> Of(T? value) {
        if(value is null) return Failed(ResultFlag.Null);
        return new() {
            Status = ResultFlag.Succeeded,
            _content = value,
        };
    }
    public static Result<T> Failed(ResultFlag status = ResultFlag.Failed) {
        if(status.IsSuccess()) throw new ArgumentException("Cannot Succeed without value! Use Result.Of", nameof(status));
        return new() { Status = status };
    }

    public readonly bool IsFail => Status.IsFail();
    public readonly bool IsSuccess => Status.IsSuccess();


    public readonly void Resolve(Action<T> success, Action<ResultFlag>? failed = null) {
        if(IsFail) {
            failed?.Invoke(Status);
            return;
        }

        success(_content!);
    }

    public readonly Result<TResult> Map<TResult>(Func<T, TResult> map) => IsSuccess ? map(_content!) : Result<TResult>.Failed(Status);
    public readonly Result<TResult> Map<TResult>(Func<T, Result<TResult>> map) => IsSuccess ? map(_content!) : Result<TResult>.Failed(Status);
    public readonly Result<TResult> Map<TResult>(Func<T, Option<TResult>> map) => IsSuccess ? map(_content!).ToResult(Status) : Result<TResult>.Failed(Status);
    public readonly Result<TResult> Map<TResult>(Func<T, TResult> map, Func<ResultFlag, TResult> error) => IsSuccess ? map(_content!) : error(Status);

    public T ReduceOrThrow() => IsSuccess ? _content! : throw new NullReferenceException($"Optional was empty: {Status}");

    public Option<T> ToOption() => IsSuccess ? Option<T>.Some(_content) : Option<T>.None();

    public override int GetHashCode() => IsSuccess ? HashCode.Combine(_content!.GetHashCode(), Status.GetHashCode()) : HashCode.Combine(0, Status.GetHashCode());
    public override bool Equals(object? obj) => obj is Result<T> result && Equals(result);
    public bool Equals(Result<T> other) {
        if(IsSuccess) {
            if(other.IsFail) return false;
            return _content!.Equals(other._content);
        }
        return other.Status == Status;
    }
    public int CompareTo(Result<T> other) {
        if(other.IsFail) return IsSuccess ? 1 : 0;
        if(IsFail) return -1;

        return (this as IOption<T>).CompareTo(other._content);
    }


    bool IOption<T>.HasValue => IsSuccess;
    T? IOption<T>.Content => _content;
    int IComparable.CompareTo(object? obj) => obj is Result<T> o ? CompareTo(o) : obj is T t ? (this as IOption<T>).CompareTo(t) : 1;
    static IOption<T> IOption<T>.Some(T? obj) => Of(obj);
    static IOption<T> IOption<T>.None() => Failed();

    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);
    public static bool operator !=(Result<T> left, Result<T> right) => !(left == right);
    public static implicit operator Result<T>(ResultFlag status) => Failed(status);
    public static implicit operator Result<T>(T? value) => Of(value);
}

public static class ResultExtensions {
    public static bool IsFail(this ResultFlag flag) => flag.HasFlag(ResultFlag.Failed);
    public static bool IsSuccess(this ResultFlag flag) => flag is ResultFlag.Succeeded;
    public static void Catch(this ResultFlag flag, Action<ResultFlag> action) {
        if(flag.IsFail()) {
            action(flag);
        }
    }
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
    PathNotFound        = IOError | Null,
    PathAlreadyExists   = IOError | AlreadyExists,
    NoInternet          = WebError | ConnectionFailed,
    InvalidFile         = IOError | InvalidArgument,
}
