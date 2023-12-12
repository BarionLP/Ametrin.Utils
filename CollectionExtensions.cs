using Ametrin.Utils.Optional;

namespace Ametrin.Utils;
public static class CollectionExtensions {
    private static readonly Random _Random = new(DateTime.UtcNow.Millisecond);
    
    public static T GetRandomElement<T>(this ICollection<T> enumerable){
        return enumerable.ElementAt(_Random.Next(0, enumerable.Count));
    }

    public static void Move<T>(this IList<T> from, int idx, ICollection<T> to){
        if (idx < 0 || idx >= from.Count) throw new IndexOutOfRangeException(nameof(idx));

        var item = from[idx];
        to.Add(item);
        from.RemoveAt(idx);
    }

    public static string Dump(this IEnumerable<string> source, char separator) {
        return string.Join(separator, source);
    }
    public static string Dump(this IEnumerable<string> source, string separator) {
        return string.Join(separator, source);
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
}
