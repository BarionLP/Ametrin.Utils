using System.Diagnostics;

namespace Ametrin.Utils.Optional;

public readonly record struct ErrorOr<T> : IOptional<T>
{
    public bool HasValue { get; private init; }
    public T? Value { get; private init; }
    public string Message { get; private init; }

    public ErrorOr<T> Where(Func<T, bool> condition, string conditionFalseMessage = "") => HasValue ? (condition(Value!) ? this : conditionFalseMessage) : this;
    public ErrorOr<T> WhereNot(Func<T, bool> condition, string conditionTrueMessage = "") => HasValue ? (!condition(Value!) ? this : conditionTrueMessage) : this;

    public ErrorOr<TResult> Map<TResult>(Func<T, TResult> map)
        => HasValue ? map(Value!) : Message;
    public ErrorOr<TResult> Map<TResult>(Func<T, IOptional<TResult>> map)
        => HasValue ? ErrorOr<TResult>.Of(map(Value!)) : Message!;
    public ErrorOr<TResult> Map<TResult>(Func<T, IOptional<TResult>> map, string mapFailedMessage)
        => HasValue ? ErrorOr<TResult>.Of(map(Value!), mapFailedMessage) : Message!;

    public ErrorOr<TResult> Cast<TResult>(string castFailedMessage = "")
        => !HasValue ? Message
        : Value is not TResult casted ? castFailedMessage
        : casted;

    public T ReduceOrThrow() => HasValue ? Value! : throw new NullReferenceException(Message);
    public void Resolve(Action<T> action, Action<string> failed)
    {
        if(HasValue)
            action(Value!);
        else
            failed.Invoke(Message);
    }

    public static ErrorOr<T> Success(T value) => value is not null ? new() { HasValue = true, Value = value } : throw new ArgumentNullException(nameof(value), "Cannot create success state with null value");
    public static ErrorOr<T> Fail(string msg) => new() { HasValue = false, Message = msg };
    public static ErrorOr<T> Of(T? value, string valueNullMessage = "") => value is not null ? Success(value) : Fail(valueNullMessage);
    public static ErrorOr<T> Of(IOptional<T> optional, string optionFailedMessage = "") => optional switch
    {
        ErrorOr<T> option => option,
        IOptional<T> option when option.HasValue => Success(option.Value!),
        IOptional<T> option when !option.HasValue => Fail(optionFailedMessage),
        _ => throw new UnreachableException(),
    };

    public static implicit operator ErrorOr<T>(T? value) => Of(value);
    public static implicit operator ErrorOr<T>(string error) => Fail(error);

    IOptional<TResult> IOptional<T>.Cast<TResult>() => Cast<TResult>();
    IOptional<T> IOptional<T>.Where(Func<T, bool> condition) => Where(condition);
    IOptional<T> IOptional<T>.WhereNot(Func<T, bool> condition) => WhereNot(condition);
    IOptional<TResult> IOptional<T>.Map<TResult>(Func<T, TResult> map) => Map(map);
    IOptional<TResult> IOptional<T>.Map<TResult>(Func<T, IOptional<TResult>> map) => Map(map);
    T IOptional<T>.ReduceOrThrow() => throw new NotImplementedException();
}
