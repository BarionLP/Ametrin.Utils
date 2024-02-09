namespace Ametrin.Utils;

public struct TrackedValue<T> : IComparable<TrackedValue<T>>, IComparable, IComparable<T> {
    public event Action? OnChanged;

    private T _Value;
    public T? OldValue { get; private set; }

    public readonly T Value => _Value;
    public bool HasChanged { get; private set; } = false;

    public TrackedValue(T value) {
        _Value = value;
        Forget();
    }

    public void Set(T value) {
        if(EqualityComparer<T>.Default.Equals(_Value, value)) return;
        SetSilent(value);
        HasChanged = true;
        OnChanged?.Invoke();
    }
    
    public void SetSilent(T value) {
        OldValue = _Value;
        _Value = value;
    }

    public void Forget() {
        HasChanged = false;
        OldValue = default;
    }

    public readonly override string ToString() => Value?.ToString() ?? "EmptyTrackedValue";
    public readonly override bool Equals(object? obj) {
        return obj is TrackedValue<T> other &&
               EqualityComparer<T>.Default.Equals(Value, other.Value) &&
               HasChanged == other.HasChanged;
    }

    public readonly override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public readonly int CompareTo(TrackedValue<T> other) => CompareTo(other.Value);
    public readonly int CompareTo(T? other) {
        if(Value is null) return other is null ? 0 : -1;

        if(Value is IComparable<T> comparable1) return comparable1.CompareTo(other);
        if(Value is IComparable<T?> comparable2) return comparable2.CompareTo(other);
        if(Value is IComparable comparable3) return comparable3.CompareTo(other);

        throw new InvalidOperationException($"{typeof(T).Name} does not implement the IComparable interface.");
    }

    public readonly int CompareTo(object? other) {
        if(other is null) return 1;

        if(other is TrackedValue<T> trackedValue) return CompareTo(trackedValue.Value);
        if(other is T t) return CompareTo(t);

        throw new InvalidOperationException($"Cannot compare {typeof(T).FullName} to {other.GetType().FullName}");
    }


    public static bool operator ==(TrackedValue<T> A, TrackedValue<T> B) => A.Equals(B);
    public static bool operator !=(TrackedValue<T> A, TrackedValue<T> B) => !A.Equals(B);
    public static implicit operator T(TrackedValue<T> value) => value.Value;
}
