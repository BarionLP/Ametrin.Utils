using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Ametrin.Utils;

public class BulkObservableCollection<T> : ObservableCollection<T>{
    public void AddRange(IEnumerable<T> collection){
        if (collection is null || !collection.Any()) return;

        CheckReentrancy();

        foreach (var item in collection){
            Items.Add(item);
        }

        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }
}
