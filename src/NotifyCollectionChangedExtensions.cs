using System.Collections.Specialized;

namespace Ametrin.Utils;

public static class NotifyCollectionChangedExtensions
{
    public static NotifyCollectionChangedEventArgs ResetArgs => field ??= new(NotifyCollectionChangedAction.Reset);
    extension(NotifyCollectionChangedEventArgs)
    {
        public static NotifyCollectionChangedEventArgs Add<T>(T item, int index = -1)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
        }

        public static NotifyCollectionChangedEventArgs Replace<T>(T newItem, T oldItem, int index = -1)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index);
        }

        public static NotifyCollectionChangedEventArgs Remove<T>(T item, int index = -1)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
        }

        public static NotifyCollectionChangedEventArgs Reset => ResetArgs;
    }
}