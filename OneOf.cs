namespace Ametrin.Utils;

public class OneOf<T1, T2> {
    private readonly object _value;

    public OneOf(T1 value) {
        _value = value!;
    }
    public OneOf(T2 value) {
        _value = value!;
    }

    public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2) {
        return _value switch {
            T1 x => f1(x),
            T2 x => f2(x),
            _ => throw new InvalidOperationException($"Unexpected type! must be {nameof(T1)} or {nameof(T2)}"),
        };
    }
    
    public void Match<TResult>(Action<T1> a1, Action<T2> a2) {
        switch(_value) {
            case T1 x:
                a1(x);
                break;
            case T2 x:
                a2(x);
                break;

            default:
                throw new InvalidOperationException($"Unexpected type! must be {nameof(T1)} or {nameof(T2)}");
        };
    }


    public static implicit operator OneOf<T1, T2>(T1 t1) => new(t1);
    public static implicit operator OneOf<T1, T2>(T2 t2) => new(t2);
}

public class OneOf<T1, T2, T3> {
    private readonly object _value;

    public OneOf(T1 value) {
        _value = value!;
    }
    public OneOf(T2 value) {
        _value = value!;
    }
    public OneOf(T3 value) {
        _value = value!;
    }

    public TResult Match<TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3) {
        return _value switch {
            T1 t1 => f1(t1),
            T2 t2 => f2(t2),
            T3 t3 => f3(t3),
            _ => throw new InvalidOperationException($"Unexpected type! must be {nameof(T1)}, {nameof(T2)}, or {nameof(T3)}")
        };
    }

    public void Match(Action<T1> a1, Action<T2> a2, Action<T3> a3) {
        switch(_value) {
            case T1 t1:
                a1(t1);
                break;
            case T2 t2:
                a2(t2);
                break;
            case T3 t3:
                a3(t3);
                break;
            default:
                throw new InvalidOperationException($"Unexpected type! must be {nameof(T1)}, {nameof(T2)}, or {nameof(T3)}");
        }
    }

    public static implicit operator OneOf<T1, T2, T3>(T1 t1) => new(t1);
    public static implicit operator OneOf<T1, T2, T3>(T2 t2) => new(t2);
    public static implicit operator OneOf<T1, T2, T3>(T3 t3) => new(t3);
}

