namespace Ametrin.Utils;

public class OneOf<T1, T2> {
    private readonly object Value;

    public OneOf(T1 value) {
        Value = value!;
    }

    public OneOf(T2 value) {
        Value = value!;
    }

    public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2) {
        return Value switch {
            T1 x => f1(x),
            T2 x => f2(x),
            _ => throw new InvalidOperationException($"Unexpected type must be {nameof(T1)} or {nameof(T2)}"),
        };
    }
}
