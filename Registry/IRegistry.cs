using Ametrin.Utils.Optional;

namespace Ametrin.Utils.Registry;

public interface IRegistry<TKey, TValue> : IEnumerable<TValue> {
    public IEnumerable<TKey> Keys { get; }
    public int Count { get; }
    public TValue this[TKey key] { get; }
    public Option<TValue> TryGet(TKey key);
    public bool ContainsKey(TKey key);
}

public interface IMutableRegistry<TKey, TValue> : IRegistry<TKey, TValue> {
    public new TValue this[TKey key] { get; set; }
    public ResultFlag TryRegister(TKey key, TValue value);
}
