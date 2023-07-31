using System.Collections.ObjectModel;

namespace Ametrin.Utils;
public static class CollectionExtensions {
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
