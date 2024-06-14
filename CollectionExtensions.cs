namespace Ametrin.Utils;

public static class CollectionExtensions
{
    public static T GetRandomElement<T>(this ICollection<T> collection) => collection.GetRandomElement(Random.Shared);
    public static T GetRandomElement<T>(this ICollection<T> collection, Random random) => collection.ElementAt(random.Next(0, collection.Count));
    public static IEnumerable<T> GetRandomElements<T>(this ICollection<T> collection, int count, Random? randomSource = null)
    {
        randomSource ??= Random.Shared;
        foreach(int _ in ..count)
        {
            yield return collection.GetRandomElement(randomSource);
        }
    }

    public static int MaxIndex<T>(this IList<T> list) where T : IComparable<T>
    {
        var maxIndex = 0;
        for(int i = 1; i < list.Count; i++)
        {
            if(list[i].CompareTo(list[maxIndex]) > 0)
            {
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    public static void Move<T>(this IList<T> from, int idx, ICollection<T> to)
    {
        if(idx < 0 || idx >= from.Count)
            throw new IndexOutOfRangeException(nameof(idx));

        to.Add(from[idx]);
        from.RemoveAt(idx);
    }

    public static bool Contains<T>(this ICollection<T> values, IEnumerable<T> contains)
    {
        foreach(var contain in contains)
        {
            if(!values.Contains(contain))
                return false;
        }
        return true;
    }

    public static bool StartsWith<T>(this IEnumerable<T> collection, T value) => collection.Any() && collection.First()!.Equals(value);

    public static T[] Copy<T>(this T[] original)
    {
        var clone = new T[original.Length];
        Array.Copy(original, clone, original.Length);
        return clone;
    }
    public static T[,] Copy<T>(this T[,] original)
    {
        var clone = new T[original.GetLength(0), original.GetLength(1)];
        Array.Copy(original, clone, original.Length);
        return clone;
    }
}
