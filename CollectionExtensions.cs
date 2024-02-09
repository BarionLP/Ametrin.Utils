using Ametrin.Utils.Optional;

namespace Ametrin.Utils;
public static class CollectionExtensions {
    private static readonly Random _random = new(DateTime.UtcNow.Millisecond);
    public static T GetRandomElement<T>(this ICollection<T> collection) => collection.GetRandomElement(_random);
    public static T GetRandomElement<T>(this ICollection<T> collection, Random random){
        return collection.ElementAt(random.Next(0, collection.Count));
    }

    public static void Move<T>(this IList<T> from, int idx, ICollection<T> to){
        if (idx < 0 || idx >= from.Count) throw new IndexOutOfRangeException(nameof(idx));

        to.Add(from[idx]);
        from.RemoveAt(idx);
    }

    public static bool Contains<T>(this ICollection<T> values, IEnumerable<T> contains){
        foreach(var contain in contains){
            if(!values.Contains(contain)) return false;
        }
        return true;
    }

    public static Option<TValue> Get<TValue, TKey>(this IDictionary<TKey, TValue> dic, TKey key){
        if(dic.TryGetValue(key, out var res)) {
            return res;
        }
        return Option<TValue>.None();
    }

    public static bool StartsWith<T>(this ReadOnlySpan<T> span, T value) => !span.IsEmpty && span[0]!.Equals(value);
    public static bool StartsWith<T>(this ICollection<T> collection, T value) => collection.Count > 0 && collection.ElementAt(0)!.Equals(value);
}
