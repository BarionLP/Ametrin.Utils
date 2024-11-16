namespace Ametrin.Utils.Registry;

[Obsolete]
public interface IRegistry<TKey, TValue> : IEnumerable<TValue>
{
    public IEnumerable<TKey> Keys { get; }
    public int Count { get; }
    public TValue this[TKey key] { get; }
    public Option<TValue> TryGet(TKey key);
    public bool ContainsKey(TKey key);
}

[Obsolete]
public interface IMutableRegistry<TKey, TValue> : IRegistry<TKey, TValue>
{
    public new TValue this[TKey key] { get; set; }
    public Option TryRegister(TKey key, TValue value);
}
