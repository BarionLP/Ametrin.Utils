using Ametrin.Utils.Optional;
using System.Collections;
using System.Collections.Frozen;

namespace Ametrin.Utils.Registry; 

public class Registry<TKey, TValue>(FrozenDictionary<TKey, TValue> entries) : IRegistry<TKey, TValue> where TKey : notnull {
    private readonly FrozenDictionary<TKey, TValue> _entries = entries;
    public int Count => _entries.Count;
    public IEnumerable<TKey> Keys => _entries.Keys;
    public TValue this[TKey key] => _entries[key];

    public Registry(IEnumerable<KeyValuePair<TKey, TValue>> entries) : this(entries.ToFrozenDictionary()) {}
    public Registry(IEnumerable<TValue> values, Func<TValue, TKey> keyProvider) : this(values.ToFrozenDictionary(keyProvider)) { }

    public Option<TValue> TryGet(TKey key) {
        if(_entries.TryGetValue(key, out var value)) {
            return value;
        }
        return Option<TValue>.None();
    }
    public bool ContainsKey(TKey key) => _entries.ContainsKey(key);

    public IEnumerator<TValue> GetEnumerator(){
        foreach (var value in _entries.Values){
            yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}