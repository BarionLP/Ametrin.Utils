namespace Ametrin.Utils;

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

    public static T[] PadLeft<T>(this T[] source, int totalLength, T padValue)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (totalLength <= source.Length)
            return [.. source];

        var padCount = totalLength - source.Length;
        var result = new T[totalLength];

        for (int i = 0; i < padCount; i++)
            result[i] = padValue;

        Array.Copy(source, 0, result, padCount, source.Length);
        return result;
    }

    public static T[] PadRight<T>(this T[] source, int totalLength, T padValue)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (totalLength <= source.Length)
            return [.. source];

        var result = new T[totalLength];

        Array.Copy(source, result, source.Length);

        for (int i = source.Length; i < totalLength; i++)
            result[i] = padValue;

        return result;
    }

}
