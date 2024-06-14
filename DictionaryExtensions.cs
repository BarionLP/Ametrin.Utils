using Ametrin.Utils.Optional;

namespace Ametrin.Utils;

public static class DictionaryExtensions
{
    public static bool TryGetValue<T>(this IReadOnlyDictionary<string, T> dictionary, ReadOnlySpan<char> spanKey, out T result)
    {
        foreach(var key in dictionary.Keys)
        {
            if(spanKey.SequenceEqual(key))
            {
                result = dictionary[key];
                return true;
            }
        }

        result = default!;
        return false;
    }

    public static T Get<T>(this IReadOnlyDictionary<string, T> dictionary, ReadOnlySpan<char> spanKey)
    {
        foreach(var key in dictionary.Keys)
        {
            if(spanKey.SequenceEqual(key))
            {
                return dictionary[key];
            }
        }

        throw new KeyNotFoundException();
    }

    public static Option<TValue> Get<TValue, TKey>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
    {
        if(dictionary.TryGetValue(key, out var res))
        {
            return res;
        }
        return Option<TValue>.None();
    }

    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue @default)
    {
        if(dictionary.TryGetValue(key, out var value))
            return value;

        dictionary.Add(key, @default);
        return @default;
    }
    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
    {
        if(!dictionary.TryGetValue(key, out var val))
        {
            val = new TValue();
            dictionary.Add(key, val);
        }

        return val;
    }
    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
    {
        if(!dictionary.TryGetValue(key, out var value))
        {
            value = valueFactory();
            dictionary.Add(key, value);
        }

        return value;
    }

    public static TKey GetKey<TKey, TData>(this IDictionary<TKey, TData> dictionary, TData value)
        => dictionary.First(pair => EqualityComparer<TData>.Default.Equals(pair.Value, value)).Key;
}