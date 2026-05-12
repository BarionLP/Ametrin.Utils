using System.Collections.Specialized;

namespace Ametrin.Utils;

public interface INotifyListChanged<T>
{
    event EventHandler<ListChangedEventArgs<T>>? ListChanged;
}

public static class ListChangedEventArgs
{
    public static ListChangedEventArgs<T> AddSingle<T>(int index, T newItem) => new ListChangedEventArgs<T>.Add(index, new SingleItemReadOnlyList<T>(newItem));
    public static ListChangedEventArgs<T> RemoveSingle<T>(int index, T oldItem) => new ListChangedEventArgs<T>.Remove(index, new SingleItemReadOnlyList<T>(oldItem));
    public static ListChangedEventArgs<T> ReplaceSingle<T>(int index, T newItem, T oldItem) => new ListChangedEventArgs<T>.Replace(index, new SingleItemReadOnlyList<T>(newItem), new SingleItemReadOnlyList<T>(oldItem));
    public static ListChangedEventArgs<T> MoveSingle<T>(int newIndex, int oldIndex, T item) => new ListChangedEventArgs<T>.Move(newIndex, oldIndex, new SingleItemReadOnlyList<T>(item));
    // public static ListChangedEventArgs<T> ModifySingle<T>(int index, T item) => new ListChangedEventArgs<T>.Modify(index, new SingleItemReadOnlyList<T>(item));

    public static ListChangedEventArgs<T> AddMany<T>(int index, params IReadOnlyList<T> newItems) => new ListChangedEventArgs<T>.Add(index, ThrowIf.NullOrEmpty(newItems));
    public static ListChangedEventArgs<T> RemoveMany<T>(int index, params IReadOnlyList<T> oldItems) => new ListChangedEventArgs<T>.Remove(index, ThrowIf.NullOrEmpty(oldItems));
    public static ListChangedEventArgs<T> ReplaceMany<T>(int index, IReadOnlyList<T> newItems, IReadOnlyList<T> oldItems)
    {
        ThrowIf.NullOrEmpty(newItems);
        if (newItems.Count != oldItems.Count)
        {
            throw new ArgumentException("newItems.Count != oldItems.Count");
        }
        return new ListChangedEventArgs<T>.Replace(index, newItems, oldItems);
    }

    public static ListChangedEventArgs<T> MoveMany<T>(int newIndex, int oldIndex, params IReadOnlyList<T> items) => new ListChangedEventArgs<T>.Move(newIndex, oldIndex, ThrowIf.NullOrEmpty(items));
    // public static ListChangedEventArgs<T> ModifyMany<T>(int index, params IReadOnlyList<T> items) => new ListChangedEventArgs<T>.Modify(index, items);

    public static ListChangedEventArgs<T> Reset<T>() => ListChangedEventArgs<T>.Reset.Instance;

    public static ListChangedEventArgs<T> Of<T>(NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                {
                    Debug.Assert(args.OldItems is null);
                    Debug.Assert(args.NewItems is { Count: > 0 });
                    Debug.Assert(args.OldStartingIndex is -1);
                    if (args.NewItems is [T single])
                    {
                        return AddSingle(args.NewStartingIndex, single);
                    }
                    else
                    {
                        return AddMany(args.NewStartingIndex, [.. args.NewItems.Cast<T>()]);
                    }
                }

            case NotifyCollectionChangedAction.Remove:
                {
                    Debug.Assert(args.NewItems is null);
                    Debug.Assert(args.OldItems is { Count: > 0 });
                    Debug.Assert(args.NewStartingIndex is -1);
                    if (args.OldItems is [T single])
                    {
                        return RemoveSingle(args.OldStartingIndex, single);
                    }
                    else
                    {
                        return RemoveMany(args.OldStartingIndex, [.. args.OldItems.Cast<T>()]);
                    }
                }

            case NotifyCollectionChangedAction.Replace:
                {
                    Debug.Assert(args.NewItems is { Count: > 0 });
                    Debug.Assert(args.OldItems?.Count == args.NewItems.Count);
                    Debug.Assert(args.NewStartingIndex == args.OldStartingIndex);
                    if (args.NewItems is [T newItem])
                    {
                        var oldItem = (T)args.OldItems[0]!;
                        return ReplaceSingle(args.NewStartingIndex, newItem, oldItem);
                    }
                    else
                    {
                        return ReplaceMany(args.NewStartingIndex, [.. args.NewItems.Cast<T>()], [.. args.OldItems.Cast<T>()]);
                    }
                }

            case NotifyCollectionChangedAction.Move:
                {
                    Debug.Assert(args.NewItems is { Count: > 0 });
                    Debug.Assert(args.OldItems is null);
                    Debug.Assert(args.NewStartingIndex != args.OldStartingIndex);
                    if (args.NewItems is [T newItem])
                    {
                        return MoveSingle(args.NewStartingIndex, args.OldStartingIndex, newItem);
                    }
                    else
                    {
                        return MoveMany(args.NewStartingIndex, args.OldStartingIndex, [.. args.NewItems.Cast<T>()]);
                    }
                }

            case NotifyCollectionChangedAction.Reset:
                return Reset<T>();

            default:
                throw new UnreachableException();
        }
    }
}

// TODO C# 15: convert to union
public abstract record ListChangedEventArgs<T>
{
    public sealed record Add(int StartIndex, IReadOnlyList<T> Items) : ListChangedEventArgs<T>;
    public sealed record Remove(int StartIndex, IReadOnlyList<T> Items) : ListChangedEventArgs<T>;
    public sealed record Replace(int StartIndex, IReadOnlyList<T> NewItems, IReadOnlyList<T> OldItems) : ListChangedEventArgs<T>;
    public sealed record Move(int NewStartIndex, int OldStartIndex, IReadOnlyList<T> Items) : ListChangedEventArgs<T>;
    // public sealed record Modify(int StartIndex, IReadOnlyList<T> Items) : ListChangedEventArgs<T>;
    public sealed record Reset : ListChangedEventArgs<T>
    {
        public static Reset Instance => field ??= new();
    }

    public NotifyCollectionChangedEventArgs ToLegacy() => this switch
    {
        Add add => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)add.Items, add.StartIndex),
        Remove remove => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)remove.Items, remove.StartIndex),
        Replace replace => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, (IList)replace.NewItems, (IList)replace.OldItems, replace.StartIndex),
        Move move => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, (IList)move.Items, move.NewStartIndex, move.OldStartIndex),
        Reset => NotifyCollectionChangedEventArgs.Reset,
        _ => throw new UnreachableException(),
    };
}
