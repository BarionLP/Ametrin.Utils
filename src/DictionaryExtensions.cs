namespace Ametrin.Utils;

public static class DictionaryExtensions
{
    public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue @default)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        dictionary.Add(key, @default);
        return @default;
    }
    public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
    {
        if (!dictionary.TryGetValue(key, out var val))
        {
            val = new TValue();
            dictionary.Add(key, val);
        }

        return val;
    }
    public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            value = valueFactory();
            dictionary.Add(key, value);
        }

        return value;
    }

    public static bool TryAdd<TType>(this Dictionary<string, Type> dictionary)
    {
        var type = typeof(TType);
        return dictionary.TryAdd(type.FullName ?? type.Name, type);
    }

    public static TKey GetKey<TKey, TData>(this IDictionary<TKey, TData> dictionary, TData value) => dictionary.GetKey(value, null);
    public static TKey GetKey<TKey, TData>(this IDictionary<TKey, TData> dictionary, TData value, EqualityComparer<TData>? comparer)
    {
        comparer ??= EqualityComparer<TData>.Default;
        return dictionary.First(pair => comparer.Equals(pair.Value, value)).Key;
    }
}