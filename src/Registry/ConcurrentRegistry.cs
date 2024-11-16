using System.Collections;
using System.Collections.Concurrent;

namespace Ametrin.Utils.Registry;

[Obsolete]
public sealed class ConcurrentRegistry<TKey, TValue>(ConcurrentDictionary<TKey, TValue> entries) : IMutableRegistry<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> _entries = entries;
    public int Count => _entries.Count;
    public IEnumerable<TKey> Keys => _entries.Keys;

    public TValue this[TKey key]
    {
        get => _entries[key];
        set => _entries[key] = value;
    }

    public ConcurrentRegistry(IEnumerable<KeyValuePair<TKey, TValue>> values)
    : this(new ConcurrentDictionary<TKey, TValue>(values)) { }
    public ConcurrentRegistry(IEnumerable<TValue> values, Func<TValue, TKey> keyProvider)
        : this(values.Select(val => new KeyValuePair<TKey, TValue>(keyProvider(val), val))) { }
    public ConcurrentRegistry() : this([]) { }

    public Option<TValue> TryGet(TKey key)
        => _entries.TryGetValue(key, out var value) ? (Option<TValue>)value : default;
    public Option TryRegister(TKey key, TValue value)
        => _entries.TryAdd(key, value);

    public bool ContainsKey(TKey key) => _entries.ContainsKey(key);

#if NET9_0_OR_GREATER
    public ConcurrentDictionary<TKey, TValue>.AlternateLookup<TAlternate> GetAlternateLookup<TAlternate>() where TAlternate : notnull
        => _entries.GetAlternateLookup<TAlternate>();
#endif

    public IEnumerator<TValue> GetEnumerator() => _entries.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_entries).GetEnumerator();
}