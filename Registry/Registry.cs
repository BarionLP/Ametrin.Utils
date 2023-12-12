using Ametrin.Utils.Optional;
using System.Collections;
using System.Collections.Frozen;

namespace Ametrin.Utils.Registry; 

public class Registry<TKey, TValue>(FrozenDictionary<TKey, TValue> entries) : IRegistry<TKey, TValue> where TKey : notnull {
    private readonly FrozenDictionary<TKey, TValue> Entries = entries;
    public int Count => Entries.Count;
    public IEnumerable<TKey> Keys => Entries.Keys;
    public TValue this[TKey key] => Entries[key];

    public Registry(IReadOnlyDictionary<TKey, TValue> entries) : this(entries.ToFrozenDictionary()) {}
    public Registry(IEnumerable<TValue> values, Func<TValue, TKey> keyProvider) : this(values.ToFrozenDictionary(keyProvider)) { }

    public Option<TValue> TryGet(TKey key) {
        if(Entries.TryGetValue(key, out var value)) {
            return value;
        }
        return Option<TValue>.None();
    }
    public bool ContainsKey(TKey key) => Entries.ContainsKey(key);

    public IEnumerator<TValue> GetEnumerator(){
        foreach (var value in Entries.Values){
            yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}