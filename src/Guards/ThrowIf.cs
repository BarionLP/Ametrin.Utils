using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using static Ametrin.Guards.ExceptionMessages;
using static Ametrin.Guards.GuardConditions;

namespace Ametrin.Guards;

public static class ThrowIf
{
    [StackTraceHidden]
    public static T Null<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string expression = "") where T : class
        => value ?? throw new ArgumentNullException(expression);

    [StackTraceHidden]
    public static T Null<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string expression = "") where T : struct
        => value ?? throw new ArgumentNullException(expression);

    [StackTraceHidden]
    public static T Not<T>([NotNull] object? value, [CallerArgumentExpression(nameof(value))] string expression = "")
        => Null(value) is T t ? t : throw new ArgumentException($"Unable to cast object of type '{value.GetType().FullName}' to type '{typeof(T).FullName}'", expression);

    [StackTraceHidden]
    public static T NullOrEmpty<T>([NotNull] T? sequence, [CallerArgumentExpression(nameof(sequence))] string expression = "") where T : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(sequence, expression);
        return IsEmpty(sequence) ? throw new ArgumentException(COLLECTION_EMPTY, expression) : sequence;
    }

    [StackTraceHidden]
    public static string NullOrEmpty([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string expression = "")
    {
        ArgumentException.ThrowIfNullOrEmpty(value, expression);
        return value;
    }

    [StackTraceHidden]
    public static string NullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string expression = "")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, expression);
        return value;
    }

    [StackTraceHidden]
    public static ReadOnlySpan<T> Empty<T>(ReadOnlySpan<T> value, [CallerArgumentExpression(nameof(value))] string expression = "")
        => value.IsEmpty ? throw new ArgumentException(SPAN_EMPTY, expression) : value;

    [StackTraceHidden]
    public static ReadOnlySpan<char> EmptyOrWhiteSpace(ReadOnlySpan<char> value, [CallerArgumentExpression(nameof(value))] string expression = "")
        => value.IsWhiteSpace() ? throw new ArgumentException(SPAN_WHITESPACE, expression) : value;

    [StackTraceHidden]
    public static T InRange<T>(T value, T minInclusive, T maxExclusive, [CallerArgumentExpression(nameof(value))] string expression = "", [CallerArgumentExpression(nameof(minInclusive))] string minExpression = "", [CallerArgumentExpression(nameof(maxExclusive))] string maxExpression = "")
        where T : notnull, IComparable<T>
        => IsInRange(value, minInclusive, maxExclusive) ? throw ExceptionFactory.InRange(expression, value, minExpression, maxExpression) : value;

    [StackTraceHidden]
    public static T OutOfRange<T>(T value, T minInclusive, T maxExclusive, [CallerArgumentExpression(nameof(value))] string expression = "", [CallerArgumentExpression(nameof(minInclusive))] string minExpression = "", [CallerArgumentExpression(nameof(maxExclusive))] string maxExpression = "")
        where T : notnull, IComparable<T>
        => !IsInRange(value, minInclusive, maxExclusive) ? throw ExceptionFactory.OutOfRange(expression, value, minExpression, maxExpression) : value;

    [StackTraceHidden]
    public static T LessThan<T>(T value, T min, [CallerArgumentExpression(nameof(value))] string expression = "", [CallerArgumentExpression(nameof(min))] string minExpression = "")
        where T : notnull, IComparable<T>
        => IsLessThan(value, min) ? throw ExceptionFactory.LessThan(expression, value, minExpression) : value;

    [StackTraceHidden]
    public static T LessThanOrEqual<T>(T value, T min, [CallerArgumentExpression(nameof(value))] string expression = "", [CallerArgumentExpression(nameof(min))] string minExpression = "")
        where T : notnull, IComparable<T>
        => IsLessThanOrEqual(value, min) ? throw ExceptionFactory.LessThanOrEqual(expression, value, minExpression) : value;

    [StackTraceHidden]
    public static T GreaterThan<T>(T value, T max, [CallerArgumentExpression(nameof(value))] string expression = "", [CallerArgumentExpression(nameof(max))] string maxExpression = "")
        where T : notnull, IComparable<T>
        => IsGreaterThan(value, max) ? throw ExceptionFactory.GreaterThan(expression, value, maxExpression) : value;

    [StackTraceHidden]
    public static T GreaterThanOrEqual<T>(T value, T max, [CallerArgumentExpression(nameof(value))] string expression = "", [CallerArgumentExpression(nameof(max))] string maxExpression = "")
        where T : notnull, IComparable<T>
        => IsGreaterThanOrEqual(value, max) ? throw ExceptionFactory.GreaterThanOrEqual(expression, value, maxExpression) : value;

    /// <inheritdoc cref="IsPositive{T}(T)"/>
    [StackTraceHidden]
    public static T Positive<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "")
        where T : INumber<T>
        => IsPositive(value) ? throw new ArgumentOutOfRangeException(expression, value, VALUE_POSITIVE) : value;

    [StackTraceHidden]
    public static T Zero<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "")
        where T : INumber<T>
    {
        ArgumentOutOfRangeException.ThrowIfZero(value, expression);
        return value;
    }

    [StackTraceHidden]
    public static T Negative<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "")
        where T : INumber<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, expression);
        return value;
    }
}
