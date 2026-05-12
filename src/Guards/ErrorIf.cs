using System.Runtime.CompilerServices;
using static Ametrin.Guards.ExceptionMessages;
using static Ametrin.Guards.GuardConditions;

namespace Ametrin.Guards;

// experimenting with the idea for an actual Ametrin.Optional implementation
public static class ErrorIf
{
    public static Result<T> Empty<T>(Result<T> sequence, [CallerArgumentExpression(nameof(sequence))] string expression = "") where T : IEnumerable
        => sequence.Reject(expression, static (value, _) => IsEmpty(value), static (_, expression) => new ArgumentException(COLLECTION_EMPTY, expression));

    public static Result<string> NullOrEmpty(Result<string> value, [CallerArgumentExpression(nameof(value))] string expression = "")
        => value.Reject(expression, static (value, _) => string.IsNullOrEmpty(value), static (_, expression) => new ArgumentException(STRING_EMPTY, expression));

    public static Result<string> NullOrWhiteSpace(Result<string> value, [CallerArgumentExpression(nameof(value))] string expression = "")
        => value.Reject(expression, static (value, _) => string.IsNullOrWhiteSpace(value), static (_, expression) => new ArgumentException(STRING_WHITESPACE, expression));

    // public static T InRange<T>(T value, T minInclusive, T maxExclusive, [CallerArgumentExpression(nameof(value))] string expression = "")
    //     where T : notnull, IComparable<T>
    //     => GuardConditions.InRange(value, minInclusive, maxExclusive) ? throw new ArgumentOutOfRangeException(expression, value, $"cannot be in [{minInclusive}, {maxExclusive}).") : value;

    // public static T OutOfRange<T>(T value, T minInclusive, T maxExclusive, [CallerArgumentExpression(nameof(value))] string expression = "")
    //     where T : notnull, IComparable<T>
    //     => !GuardConditions.InRange(value, minInclusive, maxExclusive) ? throw new ArgumentOutOfRangeException(expression, value, $"must be in [{minInclusive}, {maxExclusive}).") : value;

    // public static T LessThan<T>(T value, T min, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(min))] string minExpression = "")
    //     where T : notnull, IComparable<T>
    //     => IsLessThan(value, min) ? throw new ArgumentOutOfRangeException(valueExpression, value, $"cannot be less than {minExpression}.") : value;

    // public static T LessThanOrEqual<T>(T value, T min, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(min))] string minExpression = "")
    //     where T : notnull, IComparable<T>
    //     => IsLessThanOrEqual(value, min) ? throw new ArgumentOutOfRangeException(valueExpression, value, $"cannot be less than or equal to {minExpression}.") : value;

    // public static T GreaterThan<T>(T value, T max, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(max))] string maxExpression = "")
    //     where T : notnull, IComparable<T>
    //     => IsGreaterThan(value, max) ? throw new ArgumentOutOfRangeException(valueExpression, value, $"cannot be greater than {maxExpression}.") : value;

    // public static T GreaterThanOrEqual<T>(T value, T max, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(max))] string maxExpression = "")
    //     where T : notnull, IComparable<T>
    //     => IsGreaterThanOrEqual(value, max) ? throw new ArgumentOutOfRangeException(valueExpression, value, $"cannot be greater than or equal to {maxExpression}.") : value;

    // public static T Positive<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "")
    //     where T : INumber<T>
    // {
    //     return T.IsPositive(value) ? throw new ArgumentOutOfRangeException(expression, value, "cannot be positive.") : value;
    // }

    // public static T Zero<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "")
    //     where T : INumber<T>
    // {
    //     ArgumentOutOfRangeException.ThrowIfZero(value, expression);
    //     return value;
    // }

    // public static T Negative<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "")
    //     where T : INumber<T>
    // {
    //     ArgumentOutOfRangeException.ThrowIfNegative(value, expression);
    //     return value;
    // }
}