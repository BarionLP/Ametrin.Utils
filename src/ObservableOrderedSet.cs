using System.Collections.ObjectModel;

namespace Ametrin.Utils;

// based on https://stackoverflow.com/a/527000
// TODO: properly do this
public sealed class ObservableOrderedSet<T>(IEqualityComparer<T> comparer) : ObservableCollection<T>
{
    private readonly IEqualityComparer<T> comparer = comparer;

    protected override void InsertItem(int index, T item)
    {
        if (this.Contains(item, comparer)) return;

        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, T item)
    {
        int i = IndexOf(item, comparer);
        if (i >= 0 && i != index) return;

        base.SetItem(index, item);
    }

    private int IndexOf(T item, IEqualityComparer<T> comparer)
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
}
