using System.Collections;

namespace Ametrin.Utils.Registry;

[Obsolete]
public class MutableRegistry<TKey, TValue>(Dictionary<TKey, TValue> entries) : IMutableRegistry<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _entries = entries;
    public int Count => _entries.Count;
    public IEnumerable<TKey> Keys => _entries.Keys;

    public TValue this[TKey key]
    {
        get => _entries[key];
        set
        {
            _entries[key] = value;
        }
    }
    public MutableRegistry(IEnumerable<TValue> values, Func<TValue, TKey> keyProvider) : this(values.ToDictionary(keyProvider)) { }
    public MutableRegistry(IEnumerable<KeyValuePair<TKey, TValue>> entries) : this(entries.ToDictionary()) { }
    public MutableRegistry() : this([]) { }

    public Option<TValue> TryGet(TKey key)
        => _entries.TryGetValue(key, out var value) ? value : default;

    public Option TryRegister(TKey key, TValue value)
        => _entries.TryAdd(key, value);
    public bool ContainsKey(TKey key) => _entries.ContainsKey(key);

#if NET9_0_OR_GREATER
    public Dictionary<TKey, TValue>.AlternateLookup<TAlternate> GetAlternateLookup<TAlternate>() where TAlternate : notnull
        => _entries.GetAlternateLookup<TAlternate>();
#endif

    public IEnumerator<TValue> GetEnumerator() => _entries.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_entries).GetEnumerator();
}