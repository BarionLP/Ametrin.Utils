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

        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Add { StartIndex: 0, Items: ["Hello"] }).IsTrue();
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

        await Assert.That(@event.Sender).IsSameReferenceAs(view);
        await Assert.That(@event.Arguments is ListChangedEventArgs<string>.Add { StartIndex: 0, Items: ["Hello"] }).IsTrue();
    }

    public static IEnumerable<Func<IList<string>>> GetListImplementations()
    {
        yield return static () => new List<string>();
        yield return static () => new ObservableCollection<string>();
    }
}
