using System.Collections.Specialized;

namespace Ametrin.Utils;

public static class NotifyCollectionChangedExtensions
{
    extension(NotifyCollectionChangedEventArgs)
    {
        public static NotifyCollectionChangedEventArgs Add<T>(T item)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item);
        }

        public static NotifyCollectionChangedEventArgs Remove<T>(T item)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
        }

        public static NotifyCollectionChangedEventArgs Reset()
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        }
    }
}