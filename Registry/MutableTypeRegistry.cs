namespace Ametrin.Utils.Registry; 

public sealed class MutableTypeRegistry<TKey> : MutableRegistry<TKey, Type> where TKey : notnull{
    public Result TryRegister<TType>(TKey key) => TryRegister(key, typeof(TType));
}
