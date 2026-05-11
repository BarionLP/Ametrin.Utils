using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

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
        => value is null ? throw new ArgumentNullException(expression) : value is T t ? t : throw new ArgumentException($"Unable to cast object of type '{value.GetType().FullName}' to type '{typeof(T).FullName}'", expression);

    [StackTraceHidden]
    public static T NullOrEmpty<T>([NotNull] T? sequence, [CallerArgumentExpression(nameof(sequence))] string expression = "") where T : IEnumerable
    {
        const string EMPTY_EXCEPTION_MESSAGE = "Collection was empty.";
        if (sequence is null) throw new ArgumentNullException(expression);
        if (sequence is ICollection collection)
        {
            return collection.Count > 0 ? sequence : throw new ArgumentException(EMPTY_EXCEPTION_MESSAGE, expression);
        }

        var enumerator = sequence.GetEnumerator();
        try
        {
            if (!enumerator.MoveNext()) throw new ArgumentException(EMPTY_EXCEPTION_MESSAGE, expression);
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }

        return sequence;
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
        => value.IsEmpty ? throw new ArgumentException("Span cannot be empty.", expression) : value;

    [StackTraceHidden]
    public static ReadOnlySpan<char> EmptyOrWhiteSpace(ReadOnlySpan<char> value, [CallerArgumentExpression(nameof(value))] string expression = "")
        => value.IsWhiteSpace() ? throw new ArgumentException("Span cannot be empty or only white spaces.", expression) : value;

    [StackTraceHidden]
    public static T OutOfRange<T>(T value, T minInclusive, T maxExclusive, [CallerArgumentExpression(nameof(value))] string expression = "")
        where T : notnull, IComparable<T>
        => value.CompareTo(minInclusive) < 0 || value.CompareTo(maxExclusive) >= 0 ? throw new ArgumentOutOfRangeException(expression, value, $"must be in [{minInclusive}, {maxExclusive}).") : value;

    [StackTraceHidden]
    public static T LessThan<T>(T value, T min, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(min))] string minExpression = "")
        where T : notnull, IComparable<T>
        => value.CompareTo(min) < 0 ? throw new ArgumentOutOfRangeException(valueExpression, value, $"cannot be less than {minExpression}.") : value;

    [StackTraceHidden]
    public static T LessThanOrEqual<T>(T value, T min, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(min))] string minExpression = "")
        where T : notnull, IComparable<T>
        => value.CompareTo(min) <= 0 ? throw new ArgumentOutOfRangeException(valueExpression, value, $"cannot be less than or equal to {minExpression}.") : value;

    [StackTraceHidden]
    public static T GreaterThan<T>(T value, T max, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(max))] string maxExpression = "")
        where T : notnull, IComparable<T>
        => value.CompareTo(max) > 0 ? throw new ArgumentOutOfRangeException(valueExpression, value, $"cannot be greater than {maxExpression}.") : value;

    [StackTraceHidden]
    public static T GreaterThanOrEqual<T>(T value, T max, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(max))] string maxExpression = "")
        where T : notnull, IComparable<T> 
        => value.CompareTo(max) >= 0 ? throw new ArgumentOutOfRangeException(valueExpression, value, $"cannot be greater than or equal to {maxExpression}.") : value;

    [StackTraceHidden]
    public static T Positive<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "")
        where T : INumber<T>
    {
        return T.IsPositive(value) ? throw new ArgumentOutOfRangeException(expression, value, "cannot be positive.") : value;
    }

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