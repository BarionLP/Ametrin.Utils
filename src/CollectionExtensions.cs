﻿namespace Ametrin.Utils;

public static class CollectionExtensions
{
    public static T GetRandomElement<T>(this ICollection<T> collection, Random? random = null)
    {
        random ??= Random.Shared;
        return collection.ElementAt(random.Next(0, collection.Count));
    }

    public static IEnumerable<T> GetRandomElements<T>(this ICollection<T> collection, int count, Random? random = null)
    {
        random ??= Random.Shared;
        foreach (int _ in 0..count)
        {
            yield return collection.GetRandomElement(random);
        }
    }

    public static int IndexOfMax<T>(this IList<T> list) where T : IComparable<T>
    {
        var maxIndex = 0;
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].CompareTo(list[maxIndex]) > 0)
            {
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    [Obsolete]
    public static void Move<T>(this IList<T> from, int idx, ICollection<T> to)
    {
        if (idx < 0 || idx >= from.Count)
        {
            throw new IndexOutOfRangeException(nameof(idx));
        }

        to.Add(from[idx]);
        from.RemoveAt(idx);
    }

    [Obsolete]
    public static bool Contains<T>(this ICollection<T> values, IEnumerable<T> contains)
    {
        foreach (var contain in contains)
        {
            if (!values.Contains(contain))
            {
                return false;
            }
        }
        return true;
    }

    public static bool StartsWith<T>(this IEnumerable<T> source, T value) => source.Any() && source.First()!.Equals(value);

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

    public static string ToHexString(this byte[] bytes) => Convert.ToHexString(bytes);
    public static string ToBase64String(this byte[] bytes) => Convert.ToBase64String(bytes);
}
