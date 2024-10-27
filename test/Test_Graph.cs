using Ametrin.Utils.Graph;

namespace Ametrin.Utils.Test;

public sealed class Test_Graph
{
    [Test]
    public async Task Tests()
    {
        var graph = new Graph<int>();
        graph.TryAdd(1);
        graph.TryAdd(2);

        await Assert.That(graph.Count).IsEqualTo(2);

        await Assert.That(graph.Link(1, 2)).IsTrue();
        await Assert.That(graph.Link(1, 2)).IsFalse();
        await Assert.That(graph.Link(2, 1)).IsFalse();
    }
}
