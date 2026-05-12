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
    private Func<T, bool>? activeFilter;
    // TODO adjust usage of BinarySearch when adding comparers

    public int Count => itemIndices?.Count ?? source.Count;
    public bool IsReadOnly => source.IsReadOnly;
    public T this[int index]
    {
        get => source[ViewToSourceIndex(index)];
        set
        {
            var sourceIndex = ViewToSourceIndex(index);
            var oldItem = source[sourceIndex];
            source[sourceIndex] = value;

            if (isSourceObservable) return;
            Source_ListChanged(source, ListChangedEventArgs.ReplaceSingle(sourceIndex, value, oldItem));
        }
    }

    [MemberNotNullWhen(true, nameof(activeFilter))]
    public bool IsFiltered => activeFilter is not null;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event EventHandler<ListChangedEventArgs<T>>? ListChanged;

    public ListView(IList<T> source)
    {
        this.source = ThrowIf.Null(source);
        // subscribe only to one, prefer modern variant
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
            RaiseViewChanged(args);
            return;
        }

        Debug.Assert(IsFiltered);

        switch (args)
        {
            case ListChangedEventArgs<T>.Add added:
                {
                    var sourceIndex = added.StartIndex;

                    // shift existing indices if not added at the end
                    if (source.Count - added.Items.Count != sourceIndex)
                    {
                        foreach (var i in itemIndices.IndexRange)
                        {
                            var index = itemIndices[i];
                            if (index >= sourceIndex)
                            {
                                itemIndices[i] = index + added.Items.Count;
                            }
                        }
                    }

                    var filteredIndex = GetInsertIndex(sourceIndex);

                    var newItems = new List<T>();
                    var insertAt = filteredIndex;
                    var batchIndex = filteredIndex;
                    foreach (var item in added.Items)
                    {
                        Debug.Assert(EqualityComparer<T>.Default.Equals(source[sourceIndex], item));
                        if (activeFilter(item))
                        {
                            itemIndices.Insert(insertAt, sourceIndex);
                            newItems.Add(item);
                            insertAt++;
                        }
                        else
                        {
                            if (newItems.Count > 0)
                            {
                                RaiseViewChanged(ListChangedEventArgs.AddMany(batchIndex, newItems));
                                newItems = [];
                                batchIndex = insertAt;
                            }
                        }
                        sourceIndex++;
                    }

                    if (newItems.Count > 0)
                    {
                        RaiseViewChanged(ListChangedEventArgs.AddMany(batchIndex, newItems));
                    }
                }
                break;

            case ListChangedEventArgs<T>.Replace replace:
                {
                    var sourceIndex = replace.StartIndex;

                    foreach (var (@new, old) in replace.NewItems.Zip(replace.OldItems))
                    {
                        Debug.Assert(EqualityComparer<T>.Default.Equals(source[sourceIndex], @new));

                        var viewIndex = SourceToViewIndex(sourceIndex);

                        if (viewIndex > -1)
                        {
                            if (activeFilter(@new))
                            {
                                RaiseViewChanged(ListChangedEventArgs.ReplaceSingle(viewIndex, @new, old));
                            }
                            else
                            {
                                itemIndices.RemoveAt(viewIndex);
                                RaiseViewChanged(ListChangedEventArgs.RemoveSingle(viewIndex, old));
                            }
                        }
                        else
                        {
                            if (activeFilter(@new))
                            {
                                viewIndex = GetInsertIndex(sourceIndex);
                                itemIndices.Insert(viewIndex, sourceIndex);
                                RaiseViewChanged(ListChangedEventArgs.AddSingle(viewIndex, @new));
                            }
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
                        var itemIndex = SourceToViewIndex(workingIndex);
                        if (itemIndex > -1)
                        {
                            itemIndices.RemoveAt(itemIndex);
                            RaiseViewChanged(ListChangedEventArgs.RemoveSingle(itemIndex, item));
                        }
                        workingIndex++;
                    }

                    // shift existing indices if not removed from end
                    if (source.Count != sourceIndex)
                    {
                        foreach (var i in itemIndices.IndexRange)
                        {
                            var index = itemIndices[i];
                            if (index >= sourceIndex)
                            {
                                itemIndices[i] = index - remove.Items.Count;
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

    private void RaiseViewChanged(ListChangedEventArgs<T> args)
    {
        ListChanged?.Invoke(this, args);
        CollectionChanged?.Invoke(this, args.ToLegacy());
    }

    public bool Contains(T item) => IndexOf(item) > -1;

    public int IndexOf(T item)
    {
        if (IsPassthrough)
        {
            return source.IndexOf(item);
        }

        foreach (var (i, sourceIndex) in itemIndices.Index())
        {
            if (EqualityComparer<T>.Default.Equals(item, source[sourceIndex]))
            {
                return i;
            }
        }

        return -1;
    }

    public void Add(T item)
    {
        source.Add(item);
        if (isSourceObservable) return;
        Source_ListChanged(source, ListChangedEventArgs.AddSingle(source.Count - 1, item));
    }

    public void Insert(int index, T item)
    {
        if(index == Count)
        {
            Add(item);
            return;
        }

        var sourceIndex = ViewToSourceIndex(index);
        source.Insert(sourceIndex, item);
        if (isSourceObservable) return;
        Source_ListChanged(source, ListChangedEventArgs.AddSingle(sourceIndex, item));
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
        var sourceIndex = ViewToSourceIndex(index);
        var item = source[sourceIndex];
        source.RemoveAt(sourceIndex);
        if (isSourceObservable) return;
        Source_ListChanged(source, ListChangedEventArgs.RemoveSingle(sourceIndex, item));
    }

    public void Clear()
    {
        source.Clear();
        if (isSourceObservable) return;
        // trigger even when source was already empty because ObservableCollection does so too
        Source_ListChanged(source, ListChangedEventArgs.Reset<T>());
    }

    public void SetFilter(Func<T, bool>? newFilter)
    {
        if (activeFilter == newFilter) return;
        activeFilter = newFilter;
        RebuildItems();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (IsPassthrough)
        {
            source.CopyTo(array, arrayIndex);
            return;
        }

        ThrowIf.Null(array);
        if ((uint)arrayIndex > (uint)array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, null);
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Destination array is not long enough.", nameof(array));

        foreach (var sourceIndex in itemIndices)
        {
            array[arrayIndex++] = source[sourceIndex];
        }
    }

    public IEnumerator<T> GetEnumerator() => IsPassthrough ? source.GetEnumerator() : itemIndices.Select(i => source[i]).GetEnumerator();

    private void RebuildItems()
    {
        if (!IsFiltered)
        {
            if (!IsPassthrough)
            {
                itemIndices = null;
                RaiseViewChanged(ListChangedEventArgs.Reset<T>());
            }
            return;
        }

        itemIndices = new(capacity: source.Count);

        foreach (var i in source.IndexRange)
        {
            if (FilterBySourceIndex(i))
            {
                itemIndices.Add(i);
            }
        }

        // we cannot compare with olditemIndices because a reset might end up with the same indices but different elements
        RaiseViewChanged(ListChangedEventArgs.Reset<T>());
    }

    private bool FilterBySourceIndex(int sourceIndex)
    {
        Debug.Assert(IsFiltered);
        return activeFilter(source[sourceIndex]);
    }

    private int SourceToViewIndex(int sourceIndex) => IsPassthrough ? sourceIndex : itemIndices.BinarySearch(sourceIndex);
    private int ViewToSourceIndex(int viewIndex) => IsPassthrough ? viewIndex : itemIndices[viewIndex];
    private int GetInsertIndex(int sourceIndex)
    {
        Debug.Assert(!IsPassthrough);

        var viewIndex = itemIndices.BinarySearch(sourceIndex);
        // BinarySearch returns the bitwise complement of the index of the next element that is larger than startIndex
        return viewIndex < 0 ? ~viewIndex : viewIndex;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
