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

    public int Count => itemIndices?.Count ?? source.Count;
    public bool IsReadOnly => source.IsReadOnly;
    public T this[int index]
    {
        get => source[index];
        set
        {
            if (IsPassthrough)
            {
                var oldItem = source[index];
                source[index] = value;

                if (isSourceObservable) return;
                Source_ListChanged(source, ListChangedEventArgs.ReplaceSingle(index, value, oldItem));
            }
            else
            {
                Debug.Assert(IsFiltered);
                var sourceIndex = itemIndices[index];
                var oldItem = source[sourceIndex];
                source[sourceIndex] = value;

                if (isSourceObservable) return;
                Source_ListChanged(source, ListChangedEventArgs.ReplaceSingle(sourceIndex, value, oldItem));
            }
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
                    var sourceIndex = added.StartIndex;
                    var filteredIndex = itemIndices.BinarySearch(sourceIndex);
                    // BinarySearch returns the bitwise complement of the index of the next element that is larger than startIndex
                    if (filteredIndex < 0)
                    {
                        filteredIndex = ~filteredIndex;
                    }

                    // shift existing indices if not added at the end
                    if (source.Count - added.Items.Count != sourceIndex)
                    {
                        foreach (var i in itemIndices.IndexRange)
                        {
                            var index = itemIndices[i];
                            if (index >= sourceIndex)
                            {
                                itemIndices[i] = index + sourceIndex;
                            }
                        }
                    }

                    var newItems = new List<T>();
                    var insertAt = filteredIndex;
                    foreach (var item in added.Items)
                    {
                        if (activeFilter(item))
                        {
                            itemIndices.Insert(insertAt, sourceIndex);
                            newItems.Add(source[sourceIndex]);
                            insertAt++;
                        }
                        else
                        {
                            if (newItems.Count > 0)
                            {
                                RaiseChanged(ListChangedEventArgs.AddMany(filteredIndex, newItems));
                                newItems = [];
                            }
                        }
                        sourceIndex++;
                    }

                    if (newItems.Count > 0)
                    {
                        RaiseChanged(ListChangedEventArgs.AddMany(filteredIndex, newItems));
                    }
                }
                break;

            case ListChangedEventArgs<T>.Replace replace:
                {
                    var sourceIndex = replace.StartIndex;

                    foreach (var (@new, old) in replace.NewItems.Zip(replace.OldItems))
                    {
                        Debug.Assert(EqualityComparer<T>.Default.Equals(source[sourceIndex], @new));
                        if (activeFilter(@new))
                        {
                            RaiseChanged(ListChangedEventArgs.ReplaceSingle(sourceIndex, @new, old));
                        }
                        else
                        {
                            itemIndices.Remove(sourceIndex);
                            RaiseChanged(ListChangedEventArgs.RemoveSingle(sourceIndex, old));
                        }

                        sourceIndex++;
                    }
                }
                break;

            case ListChangedEventArgs<T>.Remove remove:
                {
                    var sourceIndex = remove.StartIndex;

                    var workingIndex = sourceIndex;
                    foreach (var item in remove.Items)
                    {
                        var itemIndex = itemIndices.IndexOf(workingIndex);
                        if (itemIndex > -1)
                        {
                            itemIndices.RemoveAt(itemIndex);
                            RaiseChanged(ListChangedEventArgs.RemoveSingle(itemIndex, item));
                        }
                        workingIndex++;
                    }

                    // shift existing indices if not removed from end
                    if (source.Count - remove.Items.Count != sourceIndex)
                    {
                        foreach (var i in itemIndices.IndexRange)
                        {
                            var index = itemIndices[i];
                            if (index >= sourceIndex)
                            {
                                itemIndices[i] = index - sourceIndex;
                            }
                        }
                    }
                }
                break;

            case ListChangedEventArgs<T>.Reset:
                RebuildItems();
                break;

            default:
                throw new UnreachableException();
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
        Source_ListChanged(source, ListChangedEventArgs.AddSingle(source.Count - 1, item));
    }

    public void Insert(int index, T item)
    {
        source.Insert(index, item);
        if (isSourceObservable) return;
        Source_ListChanged(source, ListChangedEventArgs.AddSingle(index, item));
    }

    public bool Remove(T item)
    {
        if (isSourceObservable)
        {
            return source.Remove(item);
        }

        var sourceIndex = source.IndexOf(item);
        if (sourceIndex < 0) return false;
        source.RemoveAt(sourceIndex);
        Source_ListChanged(source, ListChangedEventArgs.RemoveSingle(sourceIndex, item));
        return true;
    }

    public void RemoveAt(int index)
    {
        var sourceIndex = IsPassthrough ? index : itemIndices[index];
        var item = source[sourceIndex];
        source.RemoveAt(sourceIndex);
        if (isSourceObservable) return;
        Source_ListChanged(source, ListChangedEventArgs.RemoveSingle(sourceIndex, item));
    }

    public void Clear()
    {
        var wasEmpty = source.Count is 0;
        source.Clear();
        if (isSourceObservable) return;
        if (!wasEmpty) Source_ListChanged(source, ListChangedEventArgs.Reset<T>());
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

        var olditemIndices = itemIndices;
        itemIndices = new(capacity: source.Count);

        foreach (var i in source.IndexRange)
        {
            if (FilterByIndex(i))
            {
                itemIndices.Add(i);
            }
        }

        if (olditemIndices is null || !itemIndices.SequenceEqual(olditemIndices))
        {
            RaiseChanged(ListChangedEventArgs.Reset<T>());
        }
    }

    private bool FilterByIndex(int index)
    {
        Debug.Assert(IsFiltered);
        return activeFilter(this[index]);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
