using System.Collections;
using System.Collections.Frozen;

namespace Ametrin.Utils.Registry; 

public class Registry<TKey, TValue> : IRegistry<TKey, TValue> where TKey : notnull {
    private readonly FrozenDictionary<TKey, TValue> Entries;
    public int Count => Entries.Count;
    public IEnumerable<TKey> Keys => Entries.Keys;
    public TValue this[TKey key] => Entries[key];

    public Registry(FrozenDictionary<TKey, TValue> entries) {
        Entries = entries;
    }
    public Registry(IReadOnlyDictionary<TKey, TValue> entries) : this(entries.ToFrozenDictionary()) {}
    public Registry(IEnumerable<TValue> values, Func<TValue, TKey> keyProvider) : this(values.ToFrozenDictionary(keyProvider)) { }

    public Result<TValue> TryGet(TKey key) {
        if(Entries.TryGetValue(key, out var value)) {
            return value;
        }
        return ResultFlag.Null;
    }
    public bool ContainsKey(TKey key) => Entries.ContainsKey(key);

    public IEnumerator<TValue> GetEnumerator(){
        foreach (var value in Entries.Values){
            yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}