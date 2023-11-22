using System.Collections;

namespace Ametrin.Utils.Registry; 

public class MutableRegistry<TKey, TValue> : IMutableRegistry<TKey, TValue> where TKey : notnull {
    private readonly IDictionary<TKey, TValue> Entries;
    public int Count => Entries.Count;
    public IEnumerable<TKey> Keys => Entries.Keys;

    public TValue this[TKey key] {
        get => Entries[key];
        set {
            Entries[key] = value;
        }
    }
    public MutableRegistry(IEnumerable<TValue> values, Func<TValue, TKey> keyProvider) : this(values.ToDictionary(keyProvider)) { }
    public MutableRegistry(IDictionary<TKey, TValue> entries) {
        Entries = entries;
    }
    public MutableRegistry() : this(new Dictionary<TKey, TValue>()) { }

    public Result<TValue> TryGet(TKey key) {
        if(Entries.TryGetValue(key, out var value)) {
            return value;
        }
        return ResultFlag.Null;
    }

    public Result TryRegister(TKey key, TValue value) {
        if(Entries.TryAdd(key, value)) {
            return ResultFlag.Succeeded;
        }

        return ResultFlag.AlreadyExists;
    }
    public bool ContainsKey(TKey key) => Entries.ContainsKey(key);

    public IEnumerator<TValue> GetEnumerator() => Entries.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Entries).GetEnumerator();

}