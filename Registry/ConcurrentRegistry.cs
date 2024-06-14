using System.Collections.Concurrent;

namespace Ametrin.Utils.Registry;

public sealed class ConcurrentRegistry<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values)
    : MutableRegistry<TKey, TValue>(new ConcurrentDictionary<TKey, TValue>(values)) where TKey : notnull
{
    public ConcurrentRegistry(IEnumerable<TValue> values, Func<TValue, TKey> keyProvider) : this(values.Select(val => new KeyValuePair<TKey, TValue>(keyProvider(val), val))) { }
}