﻿namespace Ametrin.Utils;

public static class CollectionExtensions {
    public static T GetRandomElement<T>(this ICollection<T> collection) => collection.GetRandomElement(Random.Shared);
    public static T GetRandomElement<T>(this ICollection<T> collection, Random random) => collection.ElementAt(random.Next(0, collection.Count));
    public static IEnumerable<T> GetRandomElements<T>(this ICollection<T> collection, int count) => collection.GetRandomElements(count, Random.Shared);
    public static IEnumerable<T> GetRandomElements<T>(this ICollection<T> collection, int count, Random randomSource) {
        foreach(int _ in ..count) {
            yield return collection.GetRandomElement(randomSource);
        }
    }

    public static void Move<T>(this IList<T> from, int idx, ICollection<T> to) {
        if (idx < 0 || idx >= from.Count) throw new IndexOutOfRangeException(nameof(idx));

        to.Add(from[idx]);
        from.RemoveAt(idx);
    }

    public static bool Contains<T>(this ICollection<T> values, IEnumerable<T> contains) {
        foreach(var contain in contains){
            if(!values.Contains(contain)) return false;
        }
        return true;
    }

    public static bool StartsWith<T>(this IEnumerable<T> collection, T value) => collection.Any() && collection.First()!.Equals(value);
}
