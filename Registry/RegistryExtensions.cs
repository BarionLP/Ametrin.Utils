namespace Ametrin.Utils.Registry; 

public static class RegistryExtensions {
    public static Result<TValue> TryGet<TValue>(this IRegistry<string, TValue> registry, ReadOnlySpan<char> spanKey) {
        foreach(var key in registry.Keys) {
            if(spanKey.SequenceEqual(key)) return registry[key];
        }
        return ResultFlag.Null;
    }

    public static ResultFlag TryRegister<TType>(this MutableTypeRegistry<string> registry) {
        var type = typeof(TType);
        if(type.FullName is not string name) throw new ArgumentException("Cannot register Type without name");
        return registry.TryRegister(name, type);
    }

    public static IRegistry<TKey, TValue> ToRegistry<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dic) where TKey : notnull => new Registry<TKey, TValue>(dic);
    public static IRegistry<TKey, TValue> ToRegistry<TKey, TValue>(this IEnumerable<TValue> entries, Func<TValue, TKey> keyProvider) where TKey : notnull => new Registry<TKey, TValue>(entries, keyProvider);
    public static IMutableRegistry<TKey, TValue> ToMutableRegistry<TKey, TValue>(this IDictionary<TKey, TValue> dic) where TKey : notnull => new MutableRegistry<TKey, TValue>(dic);
    public static IMutableRegistry<TKey, TValue> ToMutableRegistry<TKey, TValue>(this IEnumerable<TValue> entries, Func<TValue, TKey> keyProvider) where TKey : notnull => new MutableRegistry<TKey, TValue>(entries, keyProvider);
}