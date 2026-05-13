namespace Ametrin.Guards;

public static class ExceptionMessages
{
    public const string COLLECTION_EMPTY = "The collection cannot be empty.";
    public const string STRING_EMPTY = "The value cannot be an empty string.";
    public const string STRING_WHITESPACE = "The value cannot be an empty string or composed entirely of whitespace.";
    public const string SPAN_EMPTY = "The span cannot be string.";
    public const string SPAN_WHITESPACE = "The span cannot be empty or composed entirely of whitespace.";
    public const string VALUE_POSITIVE = "must be a non-positive value.";
    public const string VALUE_ZERO = "must be a non-zero value.";
    public const string VALUE_NEGATIVE = "must be a non-negative value.";

    public static string InRange(string paramName, string minInclusive, string maxExclusive)
        => $"{paramName} must be in [{minInclusive}, {maxExclusive}).";        
    public static string NotInRange(string paramName, string minInclusive, string maxExclusive)
        => $"{paramName} cannot be in [{minInclusive}, {maxExclusive}).";
    public static string NotLessThan(string paramName, string minExpression)
        => $"{paramName} cannot be less than {minExpression}";
    public static string NotLessThanOrEqual(string paramName, string minExpression)
        => $"{paramName} cannot be less than or equal to {minExpression}";
    public static string NotGreaterThan(string paramName, string maxExpression)
        => $"{paramName} cannot be greater than {maxExpression}";
    public static string NotGreaterThanOrEqual(string paramName, string maxExpression)
        => $"{paramName} cannot be greater than or equal to {maxExpression}";
}

public static class ExceptionFactory
{
    public static Exception OutOfRange(string paramName, object value, string minInclusive, string maxExclusive)
        => new ArgumentOutOfRangeException(paramName, value, ExceptionMessages.InRange(paramName, minInclusive, maxExclusive));
    public static Exception InRange(string paramName, object value, string minInclusive, string maxExclusive)
        => new ArgumentOutOfRangeException(paramName, value, ExceptionMessages.NotInRange(paramName, minInclusive, maxExclusive));

    public static Exception LessThan(string paramName, object value, string minExpression)
        => new ArgumentOutOfRangeException(paramName, value, ExceptionMessages.NotLessThan(paramName, minExpression));
    public static Exception LessThanOrEqual(string paramName, object value, string minExpression)
        => new ArgumentOutOfRangeException(paramName, value, ExceptionMessages.NotLessThanOrEqual(paramName, minExpression));
    public static Exception GreaterThan(string paramName, object value, string minExpression)
        => new ArgumentOutOfRangeException(paramName, value, ExceptionMessages.NotGreaterThan(paramName, minExpression));
    public static Exception GreaterThanOrEqual(string paramName, object value, string minExpression)
        => new ArgumentOutOfRangeException(paramName, value, ExceptionMessages.NotGreaterThanOrEqual(paramName, minExpression));
}