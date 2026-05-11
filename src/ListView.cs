using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Ametrin.Utils;

public sealed class ListView<T> : IList<T>, INotifyListChanged<T>, INotifyCollectionChanged, IDisposable
{
    private readonly IList<T> source;
    private readonly bool isSourceObservable = false;
    private List<int>? itemIndices;
    [MemberNotNullWhen(false, nameof(itemIndices))]
    private bool IsPassthrough => itemIndices is null;
    private Func<T?, bool>? activeFilter;

    public int Count => source.Count;
    public bool IsReadOnly => source.IsReadOnly;
    public T this[int index]
    {
        get => source[index];
        set
        {
            var oldItem = source[index];
            source[index] = value;

            if (isSourceObservable) return;
            RaiseChanged(ListChangedEventArgs.ReplaceSingle(index, value, oldItem));
        }
    }

    [MemberNotNullWhen(true, nameof(activeFilter))]
    public bool IsFiltered => activeFilter is not null;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event EventHandler<ListChangedEventArgs<T>>? ListChanged;

    public ListView(IList<T> source)
    {
        this.source = source;
        if (source is INotifyListChanged<T> observable)
        {
            isSourceObservable = true;
            observable.ListChanged += Source_ListChanged;
        }
        else if (source is INotifyCollectionChanged legacyObservable)
        {
            isSourceObservable = true;
            legacyObservable.CollectionChanged += Source_CollectionChanged;
        }
    }

    public void Dispose()
    {
        if (source is INotifyListChanged<T> observable)
        {
            Debug.Assert(isSourceObservable);
            observable.ListChanged -= Source_ListChanged;
        }
        else if (source is INotifyCollectionChanged legacyObservable)
        {
            Debug.Assert(isSourceObservable);
            legacyObservable.CollectionChanged -= Source_CollectionChanged;
        }
    }

    private void Source_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        Source_ListChanged(sender, ListChangedEventArgs.Of<T>(args));
    }

    private void Source_ListChanged(object? sender, ListChangedEventArgs<T> args)
    {
        if (IsPassthrough)
        {
            RaiseChanged(args);
            return;
        }

        Debug.Assert(IsFiltered);

        switch (args)
        {
            case ListChangedEventArgs<T>.Add added:
                {
                    var startIndex = added.StartIndex;
                    var filteredIndex = itemIndices.BinarySearch(startIndex);
                    // BinarySearch returns the bitwise complement of the index of the next element that is larger than startIndex
                    if (filteredIndex < 0)
                    {
                        filteredIndex = ~filteredIndex;
                    }

                    foreach (var i in itemIndices.IndexRange)
                    {
                        var index = itemIndices[i];
                        if (index >= startIndex)
                        {
                            itemIndices[i] = index + startIndex;
                        }
                    }

                    var newItems = new List<T>(capacity: added.Items.Count);
                    var insertAt = filteredIndex;
                    foreach (T item in added.Items)
                    {
                        if (activeFilter(item))
                        {
                            itemIndices.Insert(insertAt, startIndex);
                            newItems.Add(source[startIndex]);
                            insertAt++;
                        }
                        startIndex++;
                    }

                    if (newItems.Count > 0)
                    {
                        RaiseChanged(ListChangedEventArgs.AddMany(filteredIndex, newItems));
                    }
                }
                break;
        }
    }

    private void RaiseChanged(ListChangedEventArgs<T> args)
    {
        ListChanged?.Invoke(this, args);
        CollectionChanged?.Invoke(this, args.ToLegacy());
    }

    public bool Contains(T item) => source.Contains(item);

    public int IndexOf(T item) => source.IndexOf(item);

    public void Add(T item)
    {
        source.Add(item);
        if (isSourceObservable) return;
        RaiseChanged(ListChangedEventArgs.AddSingle(source.Count - 1, item));
    }

    public void Insert(int index, T item)
    {
        source.Insert(index, item);
        if (isSourceObservable) return;
        RaiseChanged(ListChangedEventArgs.AddSingle(index, item));
    }

    public bool Remove(T item)
    {
        if (isSourceObservable)
        {
            return source.Remove(item);
        }

        var index = source.IndexOf(item);
        if (index < 0) return false;
        source.RemoveAt(index);
        RaiseChanged(ListChangedEventArgs.RemoveSingle(index, item));
        return true;
    }

    public void RemoveAt(int index)
    {
        var item = source[index];
        source.RemoveAt(index);
        if (isSourceObservable) return;
        RaiseChanged(ListChangedEventArgs.RemoveSingle(index, item));
    }

    public void Clear()
    {
        source.Clear();
        if (isSourceObservable) return;
        RaiseChanged(ListChangedEventArgs.Reset<T>());
    }

    public void SetFilter(Func<T?, bool> newFilter)
    {
        if (activeFilter == newFilter) return;
        activeFilter = newFilter;
        RebuildItems();
    }

    public void CopyTo(T[] array, int arrayIndex) => source.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => IsPassthrough ? source.GetEnumerator() : itemIndices.Select(i => source[i]).GetEnumerator();

    private void RebuildItems()
    {
        if (!IsFiltered)
        {
            if (!IsPassthrough)
            {
                itemIndices = null;
                RaiseChanged(ListChangedEventArgs.Reset<T>());
            }
            return;
        }

        if (IsPassthrough)
        {
            itemIndices = new(capacity: source.Count);
        }
        else
        {
            itemIndices.Clear();
        }

        foreach (var i in source.IndexRange)
        {
            if (FilterByIndex(i))
            {
                itemIndices.Add(i);
            }
        }

        RaiseChanged(ListChangedEventArgs.Reset<T>());
    }

    private bool FilterByIndex(int index)
    {
        Debug.Assert(IsFiltered);
        return activeFilter(this[index]);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

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
