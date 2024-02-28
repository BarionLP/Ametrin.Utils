using System.Collections.Concurrent;

namespace Ametrin.Utils.Registry;

public class ConcurrentRegistry<TKey, TValue>(IEnumerable<TValue> values, Func<TValue, TKey> keyProvider) 
: MutableRegistry<TKey, TValue>(new ConcurrentDictionary<TKey, TValue>(values.Select(val=>new KeyValuePair<TKey, TValue>(keyProvider(val), val)))) where TKey : notnull{

}