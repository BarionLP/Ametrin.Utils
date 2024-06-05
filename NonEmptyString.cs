namespace Ametrin.Utils;

public readonly record struct NonEmptyString(string Value)
{
    public readonly string Value = !string.IsNullOrWhiteSpace(Value) ? Value : throw new ArgumentException("String was empty");
    public override string ToString() => Value;

    public static implicit operator string(NonEmptyString value) => value.Value;
    public static implicit operator NonEmptyString(string value) => new(value); // good idea?
};