namespace Ametrin.Utils;

public sealed class TrackedValue<T> : IComparable<TrackedValue<T>>, IComparable, IComparable<T> {
    public event Action? OnChanged;

    private T _value;
    public T? OldValue { get; private set; }

    public  T Value => _value;
    public bool HasChanged { get; private set; } = false;

    public TrackedValue(T value) {
        _value = value;
        Forget();
    }

    public void Set(T value) {
        if(EqualityComparer<T>.Default.Equals(_value, value)) return;
        SetSilent(value);
        HasChanged = true;
        OnChanged?.Invoke();
    }
    
    public void SetSilent(T value) {
        OldValue = _value;
        _value = value;
    }

    public void Forget() {
        HasChanged = false;
        OldValue = default;
    }

    public override string ToString() => Value?.ToString() ?? "EmptyTrackedValue";
    public override bool Equals(object? obj) {
        return obj is TrackedValue<T> other &&
               EqualityComparer<T>.Default.Equals(Value, other.Value) &&
               HasChanged == other.HasChanged;
    }

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public int CompareTo(TrackedValue<T>? other) => other is null ? 1 : CompareTo(other.Value);
    
    //sniff sniff
    public int CompareTo(T? other) {
        if(Value is null) return other is null ? 0 : -1;

        if(Value is IComparable<T> comparable1) return comparable1.CompareTo(other);
        if(Value is IComparable<T?> comparable2) return comparable2.CompareTo(other);
        if(Value is IComparable comparable3) return comparable3.CompareTo(other);

        throw new InvalidOperationException($"{typeof(T).Name} does not implement the IComparable interface.");
    }

    public int CompareTo(object? other) {
        if(other is null) return 1;

        if(other is TrackedValue<T> trackedValue) return CompareTo(trackedValue.Value);
        if(other is T t) return CompareTo(t);

        throw new InvalidOperationException($"Cannot compare {typeof(T).FullName} to {other.GetType().FullName}");
    }


    public static bool operator ==(TrackedValue<T> A, TrackedValue<T> B) => A.Equals(B);
    public static bool operator !=(TrackedValue<T> A, TrackedValue<T> B) => !A.Equals(B);
    public static implicit operator T(TrackedValue<T> value) => value.Value;
}
