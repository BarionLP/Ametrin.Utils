using System.Collections.ObjectModel;

namespace Ametrin.Utils;

// based on https://stackoverflow.com/a/527000
// only works on the concrete type
// TODO: properly do this
public sealed class ObservableOrderedSet<T> : ObservableCollection<T>
{
    private readonly IEqualityComparer<T> comparer;
    private readonly HashSet<T> set;

    public ObservableOrderedSet(IEqualityComparer<T> comparer)
    {
        this.comparer = comparer;
        set = new(comparer);
    }

    public ObservableOrderedSet(IEnumerable<T> values, IEqualityComparer<T> comparer)
    {
        this.comparer = comparer;
        set = new(comparer);
        foreach (var v in values)
        {
            // each item must go through InsertItem
            Add(v);
        }
    }

    protected override void InsertItem(int index, T item)
    {
        if (Contains(item)) return;

        set.Add(item);
        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, T item)
    {
        int i = IndexOf(item);
        if (i >= 0 && i != index) return;

        set.Remove(this[index]);
        set.Add(item);
        base.SetItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
        set.Remove(this[index]);
        base.RemoveItem(index);
    }

    protected override void ClearItems()
    {
        set.Clear();
        base.ClearItems();
    }

    public new bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index < 0) return false;
        RemoveItem(index);
        return true;
    }

    public new int IndexOf(T item)
    {
        foreach (var i in Items.IndexRange)
        {
            if (comparer.Equals(item, Items[i]))
            {
                return i;
            }
        }

        return -1;
    }

    public new bool Contains(T item) => set.Contains(item);
}
