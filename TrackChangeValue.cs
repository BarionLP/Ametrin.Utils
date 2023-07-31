using System.Runtime.Serialization;

namespace Ametrin.Utils;

[DataContract]
public sealed class TrackChangeValue<T> : IComparable<TrackChangeValue<T>>, IComparable {
    [DataMember(Name = "value")] private T _Value;

    public T Value {
        get => _Value;
        set {
            if(EqualityComparer<T>.Default.Equals(_Value, value)) return;
            _Value = value;
            HasChanged = true;
        }
    }

    public bool HasChanged { get; private set; } = false;

    public TrackChangeValue(T value) {
        _Value = value;
        HasChanged = false;
    }

    public void Forget() {
        HasChanged = false;
    }

    public override string ToString() {
        return Value?.ToString()!;
    }

    public override bool Equals(object? obj) {
        return obj is TrackChangeValue<T> other &&
               EqualityComparer<T>.Default.Equals(Value, other.Value) &&
               HasChanged == other.HasChanged;
    }

    public static bool operator ==(TrackChangeValue<T> A, TrackChangeValue<T> B) {
        if(A is null) return B is null;
        return A.Equals(B);
    }

    public static bool operator !=(TrackChangeValue<T> A, TrackChangeValue<T> B) {
        if(A is null) return B is not null;
        return !A.Equals(B);
    }

    public static implicit operator T(TrackChangeValue<T> value) => value.Value;
    public static implicit operator TrackChangeValue<T>(T value) => new(value);

    public override int GetHashCode() => Value!.GetHashCode();

    public int CompareTo(TrackChangeValue<T>? other) {
        if(other is null) return 1;
        return CompareTo(other.Value);
    }

    public int CompareTo(T? other) {
        if(Value is null && other is null) return 0;
        if(other is null) return 1;
        if(Value is null) return -1;

        if(Value is IComparable<T> comparable1) return comparable1.CompareTo(other);
        if(Value is IComparable<T?> comparable2) return comparable2.CompareTo(other);
        if(Value is IComparable comparable3) return comparable3.CompareTo(other);

        throw new InvalidOperationException("The type of Value does not implement the IComparable interface.");
    }

    public int CompareTo(object? other) {
        if(other is null) return 1;

        if(other is TrackChangeValue<T> trackedValue) return CompareTo(trackedValue.Value);
        if(other is T t) return CompareTo(t);

        throw new InvalidOperationException($"Cannot compare {typeof(T).FullName} to {other.GetType().FullName}");
    }
}
