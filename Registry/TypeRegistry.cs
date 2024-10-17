namespace Ametrin.Utils.Registry;

public sealed class TypeRegistry<TKey> : Registry<TKey, Type> where TKey : notnull
{
    public TypeRegistry(IEnumerable<KeyValuePair<TKey, Type>> entries) : base(entries) { }
    public TypeRegistry(IEnumerable<Type> values, Func<Type, TKey> keyProvider) : base(values, keyProvider) { }
}

public sealed class MutableTypeRegistry<TKey> : MutableRegistry<TKey, Type> where TKey : notnull
{
    public MutableTypeRegistry() { }
    public MutableTypeRegistry(IEnumerable<KeyValuePair<TKey, Type>> entries) : base(entries) { }
    public MutableTypeRegistry(IDictionary<TKey, Type> entries) : base(entries) { }

    public ErrorState TryRegister<TType>(TKey key) => TryRegister(key, typeof(TType));
}
