namespace Ametrin.Utils;

public readonly record struct NonEmptyString(string Value) : IComparable<NonEmptyString> {
    public readonly string Value = !string.IsNullOrWhiteSpace(Value) ? Value : throw new ArgumentException("String was empty");

    public bool StartsWith(char value) => Value.StartsWith(value);
    public bool StartsWith(string value) => Value.StartsWith(value);
    public int CompareTo(NonEmptyString other) => Value.CompareTo(other.Value);

    public override string ToString() => Value;
    
    public static implicit operator string(NonEmptyString value) => value.Value;
    public static implicit operator NonEmptyString(string value) => new(value);
    public static implicit operator ReadOnlySpan<char>(NonEmptyString value) => value.Value;
};