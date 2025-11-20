namespace Ametrin.Utils;

[Obsolete]
public readonly record struct NonEmptyString(string Value) : IComparable<NonEmptyString>
{
    private readonly string Value = !string.IsNullOrWhiteSpace(Value) ? Value : throw new ArgumentException("String was empty", nameof(Value));

    public NonEmptyString() : this("") { } // will fail

    public bool StartsWith(char value) => Value.StartsWith(value);
    public bool StartsWith(string value) => Value.StartsWith(value);
    public bool StartsWith(string value, StringComparison comparisonType) => Value.StartsWith(value, comparisonType);
    public bool StartsWith(ReadOnlySpan<char> value) => Value.AsSpan().StartsWith(value);
    public bool StartsWith(NonEmptyString value) => Value.StartsWith(value.Value);
    public bool StartsWith(NonEmptyString value, StringComparison comparisonType) => Value.StartsWith(value.Value, comparisonType);
    public int CompareTo(NonEmptyString other) => Value.CompareTo(other.Value);

    public override string ToString() => Value;

    public static implicit operator string(NonEmptyString value) => value.Value;
    public static implicit operator NonEmptyString(string value) => new(value);
    public static implicit operator ReadOnlySpan<char>(NonEmptyString value) => value.Value;
}