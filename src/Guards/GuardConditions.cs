using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Ametrin.Guards;

public static class GuardConditions
{
    public static bool IsNull<T>([NotNullWhen(false)] T? value) where T : class => value is null;
    public static bool IsNull<T>([NotNullWhen(false)] T? value) where T : struct => value is null;
    public static bool HasValue<T>([NotNullWhen(true)] T? value) where T : struct => value.HasValue;

    public static bool IsOfType<T>([NotNullWhen(true)] object? value) => value is T;
    public static bool IsNotOfType<T>(object? value) => value is not T;

    public static bool IsEmpty<T>(T sequence) where T : notnull, IEnumerable
    {
        if (sequence is ICollection collection)
        {
            return collection.Count is 0;
        }

        var enumerator = sequence.GetEnumerator();
        try
        {
            return !enumerator.MoveNext();
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }

    public static bool IsEmpty<T>(ReadOnlySpan<T> span) => span.IsEmpty;
    public static bool IsWhiteSpace<T>(ReadOnlySpan<char> span) => span.IsWhiteSpace();

    public static bool IsInRange<T>(T value, T minInclusive, T maxExclusive)
        where T : notnull, IComparable<T>
        => value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxExclusive) < 0;

    public static bool IsLessThan<T>(T value, T max)
        where T : notnull, IComparable<T>
        => value.CompareTo(max) < 0;

    public static bool IsLessThanOrEqual<T>(T value, T max)
        where T : notnull, IComparable<T>
        => value.CompareTo(max) <= 0;

    public static bool IsGreaterThan<T>(T value, T max)
        where T : notnull, IComparable<T>
        => value.CompareTo(max) > 0;

    public static bool IsGreaterThanOrEqual<T>(T value, T max)
        where T : notnull, IComparable<T>
        => value.CompareTo(max) >= 0;

    /// <summary>
    /// 0.0 and -0.0 return false (because 0 should be considered neutral)
    /// </summary>
    /// <remarks>
    /// this is different from <see cref="INumberBase{T}.IsPositive(T)"/>
    /// </remarks>
    public static bool IsPositive<T>(T value)
        where T : INumber<T>
        => T.Sign(value) is 1; // T.IsPositive treats 0 as true

    /// <summary>
    /// -0.0 and 0.0 return true
    /// </summary>
    public static bool IsZero<T>(T value)
        where T : INumber<T>
        => T.IsZero(value);

    /// <summary>
    /// -0.0 returns true but 0.0 returns false
    /// </summary>
    public static bool IsNegative<T>(T value)
        where T : INumber<T>
        => T.IsNegative(value);
}
