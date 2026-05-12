namespace Ametrin.Utils;

public sealed class SingleItemReadOnlyList<T>(T item) : IReadOnlyList<T>
{
    private const string MUTATION_ERROR_MESSAGE = "Collection is read only.";

    private readonly T _item = item;

    public T this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0);
            return _item;
        }
    }

    public int Count => 1;

    public int IndexOf(T value) => Contains(value) ? 0 : -1;
    public bool Contains(T value) => EqualityComparer<T>.Default.Equals(_item, value);

    public IEnumerator<T> GetEnumerator()
    {
        yield return _item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
