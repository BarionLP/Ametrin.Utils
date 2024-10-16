namespace Ametrin.Utils.Registry;

public static class RegistryExtensions
{
    [Obsolete("replace with GetAlternateLookup")]
    public static Option<TValue> TryGet<TValue>(this IRegistry<string, TValue> registry, ReadOnlySpan<char> spanKey)
    {
        foreach (var key in registry.Keys)
        {
            if (spanKey.SequenceEqual(key))
                return registry[key];
        }
        return default;
    }

    public static ResultFlag TryRegister<TType>(this MutableTypeRegistry<string> registry)
    {
        var type = typeof(TType);
        if (type.FullName is not string name)
            throw new ArgumentException("Cannot register Type without name");
        return registry.TryRegister(name, type);
    }

    public static Registry<TKey, TValue> ToRegistry<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> values) where TKey : notnull => new(values);
    public static Registry<TKey, TValue> ToRegistry<TKey, TValue>(this IEnumerable<TValue> entries, Func<TValue, TKey> keyProvider) where TKey : notnull => new(entries, keyProvider);
    public static MutableRegistry<TKey, TValue> ToMutableRegistry<TKey, TValue>(this IDictionary<TKey, TValue> dic) where TKey : notnull => new(dic);
    public static MutableRegistry<TKey, TValue> ToMutableRegistry<TKey, TValue>(this IEnumerable<TValue> entries, Func<TValue, TKey> keyProvider) where TKey : notnull => new(entries, keyProvider);
}