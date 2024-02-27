using Ametrin.Utils.Optional;

namespace Ametrin.Utils;

public static class DictionaryExtensions{
    public static bool TryGetValue<T>(this IReadOnlyDictionary<string, T> dictionary, ReadOnlySpan<char> spanKey, out T result){
        foreach(var key in dictionary.Keys){
            if(spanKey.SequenceEqual(key)){
                result = dictionary[key];
                return true;
            }
        }

        result = default!;
        return false;
    }

    public static Option<TValue> Get<TValue, TKey>(this IReadOnlyDictionary<TKey, TValue> dic, TKey key) {
        if (dic.TryGetValue(key, out var res)) {
            return res;
        }
        return Option<TValue>.None();
    }

    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue @default){
        if (dictionary.TryGetValue(key, out var value)) return value;

        dictionary.Add(key, @default);
        return @default;
    }
    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new(){
        if (!dict.TryGetValue(key, out var val)){
            val = new TValue();
            dict.Add(key, val);
        }

        return val;
    }
    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory){
        if (!dictionary.TryGetValue(key, out var value)){
            value = valueFactory();
            dictionary.Add(key, value);
        }

        return value;
    }
}