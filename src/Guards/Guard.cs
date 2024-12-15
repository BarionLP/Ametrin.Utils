using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ametrin.Guards;

public static class Guard
{
    [StackTraceHidden]
    public static T ThrowIfNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string expression = "") where T : class
        => value is null ? throw new ArgumentNullException(expression) : value;

    [StackTraceHidden]
    public static T Is<T>(object? value, [CallerArgumentExpression(nameof(value))] string expression = "")
        => value is null ? throw new ArgumentNullException(expression) : value is T t ? t : throw new InvalidCastException($"Unable to cast object of type '{value.GetType().FullName}' to type '{typeof(T).FullName}'");

    [StackTraceHidden]
    public static T ThrowIfNullOrEmpty<T>(T? collection, [CallerArgumentExpression(nameof(collection))] string expression = "") where T : System.Collections.IEnumerable
        => collection is not null && collection.GetEnumerator().MoveNext() ? collection : throw new ArgumentNullException(expression);


    [StackTraceHidden]
    public static string ThrowIfNullOrEmpty(string? value, [CallerArgumentExpression(nameof(value))] string expression = "")
        => string.IsNullOrEmpty(value) ? throw new ArgumentNullException(expression) : value;

    [StackTraceHidden]
    public static string ThrowIfNullOrWhiteSpace(string? value, [CallerArgumentExpression(nameof(value))] string expression = "")
        => string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException(expression) : value;

    [StackTraceHidden]
    public static T InRange<T>(T value, T minInclusive, T maxInclusive, [CallerArgumentExpression(nameof(value))] string expression = "")
        where T : notnull, IComparisonOperators<T, T, bool>
        => value < minInclusive || value > maxInclusive ? throw new ArgumentOutOfRangeException(expression) : value;
        
    [StackTraceHidden]
    public static T LessThan<T>(T value, T max, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(max))] string maxExpression = "")
        where T : notnull, IComparisonOperators<T, T, bool>
        => value < max ? value : throw new ArgumentOutOfRangeException(valueExpression, $"{valueExpression} must be less than {maxExpression}.");
    
    [StackTraceHidden]
    public static T LessThanOrEqual<T>(T value, T max, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(max))] string maxExpression = "")
        where T : notnull, IComparisonOperators<T, T, bool>
        => value <= max ? value : throw new ArgumentOutOfRangeException(valueExpression, $"{valueExpression} must be less than or equal to {maxExpression}.");
            
    [StackTraceHidden]
    public static T GreaterThan<T>(T value, T min, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(min))] string maxExpression = "")
        where T : notnull, IComparisonOperators<T, T, bool>
        => value > min ? value : throw new ArgumentOutOfRangeException(valueExpression, $"{valueExpression} must be greater than {maxExpression}.");
    
    [StackTraceHidden]
    public static T GreaterThanOrEqual<T>(T value, T min, [CallerArgumentExpression(nameof(value))] string valueExpression = "", [CallerArgumentExpression(nameof(min))] string maxExpression = "")
        where T : notnull, IComparisonOperators<T, T, bool>
        => value >= min ? value : throw new ArgumentOutOfRangeException(valueExpression, $"{valueExpression} must be greater than or equal to {maxExpression}.");
}