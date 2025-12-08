using System.Runtime.InteropServices;

namespace Ametrin.Utils;

public static class DictionaryExtensions
{
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
        public TValue GetOrAdd(TKey key, TValue @default)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            dictionary.Add(key, @default);
            return @default;
        }
        
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = valueFactory(key);
                dictionary.Add(key, value);
            }

            return value;
        }

        public TKey GetKey(TValue value) => dictionary.GetKey(value, null);
        public TKey GetKey(TValue value, EqualityComparer<TValue>? comparer)
        {
            comparer ??= EqualityComparer<TValue>.Default;
            return dictionary.First(pair => comparer.Equals(pair.Value, value)).Key;
        }
    }

    extension<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        // this only exists for normal Dictionary
        public TValue GetOrAdd(TKey key, TValue @default)
        {
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
            if (!exists)
            {
                value = @default;
            }
            return value!;
        }
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> facotry)
        {
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
            if (!exists)
            {
                value = facotry(key);
            }
            return value!;
        }
    }

    extension(Dictionary<string, Type> dictionary)
    {
        public bool TryAdd<TType>()
        {
            var type = typeof(TType);
            return dictionary.TryAdd(type.FullName ?? type.Name, type);
        }
    }
}