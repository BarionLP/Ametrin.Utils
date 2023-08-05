using System.Collections.ObjectModel;

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

    public static string Join(this IEnumerable<string> source, char separator) {
        return string.Join(separator, source);
    }
    public static string Join(this IEnumerable<string> source, string separator) {
        return string.Join(separator, source);
    }

    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items) {
        if(collection == null) throw new ArgumentNullException(nameof(collection));

        if(items == null) throw new ArgumentNullException(nameof(items)); 

        foreach(var item in items) {
            collection.Add(item);
        }
    }
}
