using System.Collections.ObjectModel;

namespace Ametrin.Utils.Test;

public sealed class Test_ListView
{
    [Test]
    [MethodDataSource(nameof(GetListImplementations))]
    public async Task Works(IList<string> source)
    {
        using var view = new ListView<string>(source);

        var @event = Assert.Raises<ListChangedEventArgs<string>>(handler => view.ListChanged += handler, handler => view.ListChanged -= handler, () =>
        {
            view.Add("Hello");
        });
        await Assert.That(view[0]).IsEqualTo(source[0]).And.IsEqualTo("Hello");
        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Add { StartIndex: 0, Items: ["Hello"] }).IsTrue();

        @event = Assert.Raises<ListChangedEventArgs<string>>(handler => view.ListChanged += handler, handler => view.ListChanged -= handler, () =>
        {
            view[0] = "World";
        });
        await Assert.That(view[0]).IsEqualTo(source[0]).And.IsEqualTo("World");
        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Replace { StartIndex: 0, NewItems: ["World"], OldItems: ["Hello"] }).IsTrue();

        @event = Assert.Raises<ListChangedEventArgs<string>>(handler => view.ListChanged += handler, handler => view.ListChanged -= handler, () =>
        {
            view.Add("Salve");
        });
        await Assert.That(view[1]).IsEqualTo(source[1]).And.IsEqualTo("Salve");
        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Add { StartIndex: 1, Items: ["Salve"] }).IsTrue();

        @event = Assert.Raises<ListChangedEventArgs<string>>(handler => view.ListChanged += handler, handler => view.ListChanged -= handler, () =>
        {
            view.Clear();
        });
        await Assert.That(view).IsEmpty();
        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Reset).IsTrue();
    }

    [Test]
    [MethodDataSource(nameof(GetListImplementations))]
    public async Task WorksFiltered(IList<string> source)
    {
        var view = new ListView<string>(source);
        view.SetFilter(m => m?.StartsWith('H') ?? false);

        var @event = Assert.Raises<ListChangedEventArgs<string>>(handler => view.ListChanged += handler, handler => view.ListChanged -= handler, () =>
        {
            view.Add("Hello");
        });
        await Assert.That(view[0]).IsEqualTo(source[0]).And.IsEqualTo("Hello");
        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Add { StartIndex: 0, Items: ["Hello"] }).IsTrue();

        @event = Assert.Raises<ListChangedEventArgs<string>>(handler => view.ListChanged += handler, handler => view.ListChanged -= handler, () =>
        {
            view[0] = "Holla";
        });
        await Assert.That(view[0]).IsEqualTo(source[0]).And.IsEqualTo("Holla");
        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Replace { StartIndex: 0, NewItems: ["Holla"], OldItems: ["Hello"] }).IsTrue();

        @event = Assert.Raises<ListChangedEventArgs<string>>(handler => view.ListChanged += handler, handler => view.ListChanged -= handler, () =>
        {
            view[0] = "World";
        });
        await Assert.That(source[0]).IsEqualTo("World");
        await Assert.That(view).IsEmpty();
        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Remove { StartIndex: 0, Items: ["Holla"] }).IsTrue();

        @event = Assert.Raises<ListChangedEventArgs<string>>(handler => view.ListChanged += handler, handler => view.ListChanged -= handler, () =>
        {
            view.Add("Hello");
        });
        await Assert.That(source[0]).IsEqualTo("World");
        await Assert.That(view[0]).IsEqualTo(source[1]).And.IsEqualTo("Hello");
        await Assert.That(source.Count).IsEqualTo(2);
        await Assert.That(view.Count).IsEqualTo(1);
        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Add { StartIndex: 0, Items: ["Hello"] }).IsTrue();

        Assert.NotRaises<ListChangedEventArgs<string>>(handler => view.ListChanged += handler, handler => view.ListChanged -= handler, () =>
        {
            view.Add("test");
        });
        await Assert.That(source[2]).IsEqualTo("test");
        await Assert.That(source.Count).IsEqualTo(3);
        await Assert.That(view.Count).IsEqualTo(1);

        @event = Assert.Raises<ListChangedEventArgs<string>>(handler => view.ListChanged += handler, handler => view.ListChanged -= handler, () =>
        {
            view.Clear();
        });
        await Assert.That(view).IsEmpty();
        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Reset).IsTrue();
    }

    public static IEnumerable<Func<IList<string>>> GetListImplementations()
    {
        yield return static () => new List<string>();
        yield return static () => new ObservableCollection<string>();
    }
}
