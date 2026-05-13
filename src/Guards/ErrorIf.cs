using System.Numerics;
using System.Runtime.CompilerServices;
using static Ametrin.Guards.ExceptionMessages;
using static Ametrin.Guards.GuardConditions;

namespace Ametrin.Guards;

// experimenting with the idea for an actual Ametrin.Optional implementation
public static class ErrorIf
{
    public static Result<T> Empty<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "") 
        where T : IEnumerable
        => !IsEmpty(value) ? value : new ArgumentException(COLLECTION_EMPTY, expression);
    public static Result<T> Empty<T>(Result<T> result, [CallerArgumentExpression(nameof(result))] string expression = "") 
        where T : IEnumerable
        => result.Branch(out var value, out var error) ? Empty(value, expression) : error;

    public static Result<string> Empty(Result<string> result, [CallerArgumentExpression(nameof(result))] string expression = "")
        => result.Branch(out var value, out var error) ? !string.IsNullOrEmpty(value) ? value : new ArgumentException(STRING_EMPTY, expression) : error;

    public static Result<string> WhiteSpace(Result<string> result, [CallerArgumentExpression(nameof(result))] string expression = "")
        => result.Branch(out var value, out var error) ? !string.IsNullOrWhiteSpace(value) ? value : new ArgumentException(STRING_WHITESPACE, expression) : error;

    public static Result<T> InRange<T>(Result<T> result, T minInclusive, T maxExclusive, [CallerArgumentExpression(nameof(result))] string expression = "", [CallerArgumentExpression(nameof(minInclusive))] string minExpression = "", [CallerArgumentExpression(nameof(maxExclusive))] string maxExpression = "")
        where T : notnull, IComparable<T>
        => result.Branch(out var value, out var error) ? !IsInRange(value, minInclusive, maxExclusive) ? value : ExceptionFactory.InRange(expression, value, minExpression, maxExpression) : error;

    public static Result<T> OutOfRange<T>(Result<T> result, T minInclusive, T maxExclusive, [CallerArgumentExpression(nameof(result))] string expression = "", [CallerArgumentExpression(nameof(minInclusive))] string minExpression = "", [CallerArgumentExpression(nameof(maxExclusive))] string maxExpression = "")
        where T : notnull, IComparable<T>
        => result.Branch(out var value, out var error) ? IsInRange(value, minInclusive, maxExclusive) ? value : ExceptionFactory.OutOfRange(expression, value, minExpression, maxExpression) : error;

    public static Result<T> LessThan<T>(Result<T> result, T min, [CallerArgumentExpression(nameof(result))] string expression = "", [CallerArgumentExpression(nameof(min))] string minExpression = "")
        where T : notnull, IComparable<T>
        => result.Branch(out var value, out var error) ? !IsLessThan(value, min) ? value : ExceptionFactory.LessThan(expression, value, minExpression) : error;

    public static Result<T> LessThanOrEqual<T>(Result<T> result, T min, [CallerArgumentExpression(nameof(result))] string expression = "", [CallerArgumentExpression(nameof(min))] string minExpression = "")
        where T : notnull, IComparable<T>
        => result.Branch(out var value, out var error) ? !IsLessThanOrEqual(value, min) ? value : ExceptionFactory.LessThanOrEqual(expression, value, minExpression) : error;

    public static Result<T> GreaterThan<T>(Result<T> result, T max, [CallerArgumentExpression(nameof(result))] string expression = "", [CallerArgumentExpression(nameof(max))] string maxExpression = "")
        where T : notnull, IComparable<T>
        => result.Branch(out var value, out var error) ? !IsGreaterThan(value, max) ? value : ExceptionFactory.GreaterThan(expression, value, maxExpression) : error;

    public static Result<T> GreaterThanOrEqual<T>(Result<T> result, T max, [CallerArgumentExpression(nameof(result))] string expression = "", [CallerArgumentExpression(nameof(max))] string maxExpression = "")
        where T : notnull, IComparable<T>
        => result.Branch(out var value, out var error) ? !IsGreaterThanOrEqual(value, max) ? value : ExceptionFactory.GreaterThanOrEqual(expression, value, maxExpression) : error;

    /// <inheritdoc cref="IsPositive{T}(T)"/>
    public static Result<T> Positive<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "")
        where T : INumber<T>
        => IsPositive(value) ? new ArgumentOutOfRangeException(expression, value, VALUE_POSITIVE) : value;
    /// <inheritdoc cref="Positive{T}(T, string)"/>
    public static Result<T> Positive<T>(Result<T> result, [CallerArgumentExpression(nameof(result))] string expression = "")
        where T : INumber<T>
        => result.Branch(out var value, out var error) ? Positive(value, expression) : error;

    /// <inheritdoc cref="IsZero{T}(T)"/>
    public static Result<T> Zero<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "")
        where T : INumber<T>
        => IsZero(value) ? new ArgumentOutOfRangeException(expression, value, VALUE_ZERO) : value;
    /// <inheritdoc cref="Zero{T}(T, string)"/>
    public static Result<T> Zero<T>(Result<T> result, [CallerArgumentExpression(nameof(result))] string expression = "")
        where T : INumber<T>
        => result.Branch(out var value, out var error) ? Zero(value, expression) : error;

    /// <inheritdoc cref="IsNegative{T}(T)"/>
    public static Result<T> Negative<T>(T value, [CallerArgumentExpression(nameof(value))] string expression = "")
        where T : INumber<T>
        => IsNegative(value) ? new ArgumentOutOfRangeException(expression, value, VALUE_NEGATIVE) : value;
    /// <inheritdoc cref="Negative{T}(T, string)"/>
    public static Result<T> Negative<T>(Result<T> result, [CallerArgumentExpression(nameof(result))] string expression = "")
        where T : INumber<T>
        => result.Branch(out var value, out var error) ? Negative(value, expression) : error;
}