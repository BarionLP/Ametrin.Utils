using Ametrin.Utils.Optional;
using System.Collections;

namespace Ametrin.Utils.Registry; 

public class MutableRegistry<TKey, TValue>(IDictionary<TKey, TValue> entries) : IMutableRegistry<TKey, TValue> where TKey : notnull {
    private readonly IDictionary<TKey, TValue> Entries = entries;
    public int Count => Entries.Count;
    public IEnumerable<TKey> Keys => Entries.Keys;

    public TValue this[TKey key] {
        get => Entries[key];
        set {
            Entries[key] = value;
        }
    }
    public MutableRegistry(IEnumerable<TValue> values, Func<TValue, TKey> keyProvider) : this(values.ToDictionary(keyProvider)) { }
    public MutableRegistry(IEnumerable<KeyValuePair<TKey, TValue>> entries) : this(entries.ToDictionary()) { }
    public MutableRegistry() : this(new Dictionary<TKey, TValue>()) { }

    public Option<TValue> TryGet(TKey key) {
        if(Entries.TryGetValue(key, out var value)) {
            return value;
        }
        return Option<TValue>.None();
    }

    public ResultFlag TryRegister(TKey key, TValue value) {
        if(Entries.TryAdd(key, value)) {
            return ResultFlag.Succeeded;
        }

        return ResultFlag.AlreadyExists;
    }
    public bool ContainsKey(TKey key) => Entries.ContainsKey(key);

    public IEnumerator<TValue> GetEnumerator() => Entries.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Entries).GetEnumerator();

}