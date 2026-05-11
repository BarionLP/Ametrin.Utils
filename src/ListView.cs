using System.Collections.Specialized;

namespace Ametrin.Utils;

public sealed class ListView<T> : IList<T>, INotifyCollectionChanged, IDisposable
{
    private readonly IList<T> items;
    private IList<int>? itemsOverrideIndices;
    private readonly bool itemsIsObservable = false;
    private bool IsPassthrough => itemsOverrideIndices is null;
    private Func<T, bool>? _filter;

    public int Count => items.Count;
    public bool IsReadOnly => items.IsReadOnly;
    public T this[int index]
    {
        get => items[index];
        set
        {
            var oldItem = items[index];
            items[index] = value;

            if (itemsIsObservable) return;
            RaiseChanged(NotifyCollectionChangedEventArgs.Replace(value, oldItem, index));
        }
    }

    public bool IsFiltered => _filter is not null;
    
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public ListView(IList<T> items)
    {
        this.items = items;
        if (items is INotifyCollectionChanged changed)
        {
            itemsIsObservable = true;
            changed.CollectionChanged += Items_CollectionChanged;
        }
    }

    public void Dispose()
    {
        if (items is INotifyCollectionChanged changed)
        {
            Debug.Assert(itemsIsObservable);
            changed.CollectionChanged -= Items_CollectionChanged;
        }
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        RaiseChanged(args);
    }

    private void RaiseChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    public bool Contains(T item) => items.Contains(item);
    public int IndexOf(T item) => items.IndexOf(item);

    public void Add(T item)
    {
        items.Add(item);
        if (itemsIsObservable) return;
        RaiseChanged(NotifyCollectionChangedEventArgs.Add(item, items.Count - 1));
    }

    public void Insert(int index, T item)
    {
        items.Insert(index, item);
        if (itemsIsObservable) return;
        RaiseChanged(NotifyCollectionChangedEventArgs.Add(item, index));
    }

    public bool Remove(T item)
    {
        if (itemsIsObservable)
        {
            return items.Remove(item);
        }

        var index = items.IndexOf(item);
        if (index < 0) return false;
        items.RemoveAt(index);
        RaiseChanged(NotifyCollectionChangedEventArgs.Remove(item, index));
        return true;
    }

    public void RemoveAt(int index)
    {
        var item = items[index];
        items.RemoveAt(index);
        if (itemsIsObservable) return;
        RaiseChanged(NotifyCollectionChangedEventArgs.Remove(item, index));
    }

    public void Clear()
    {
        items.Clear();
        if (itemsIsObservable) return;
        RaiseChanged(NotifyCollectionChangedEventArgs.Reset);
    }

    public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);
    public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}