using System.Runtime.InteropServices;

namespace Ametrin.Utils;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue @default)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        dictionary.Add(key, @default);
        return @default;
    }
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            value = valueFactory(key);
            dictionary.Add(key, value);
        }

        return value;
    }

    // this only exists for normal Dictionary
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue @default) where TKey : notnull
    {
        ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
        if (!exists)
        {
            value = @default;
        }
        return value!;
    }
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> facotry) where TKey : notnull
    {
        ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
        if (!exists)
        {
            value = facotry(key);
        }
        return value!;
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