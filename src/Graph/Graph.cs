namespace Ametrin.Utils.Graph;

public sealed class Graph<TNode> where TNode : notnull
{
    private readonly HashSet<TNode> _nodes = [];
    private readonly HashSet<LinkEntry> _links = [];
    public IEnumerable<TNode> Nodes => _nodes;
    public int Count => _nodes.Count;

    public ErrorState TryAdd(TNode node)
    {
        if (node is null)
        {
            return new ArgumentNullException(nameof(node));
        }

        if (_nodes.Add(node))
        {
            return default;
        }

        return new ArgumentException("Duplicate Node", nameof(node));
    }

    public bool Link(TNode a, TNode b)
    {
        if (!Contains(a) || !Contains(b))
        {
            throw new InvalidOperationException("Cannot link nodes outside of the graph");
        }

        return _links.Add(new LinkEntry(a, b));
    }

    public bool Contains(TNode value) => _nodes.Contains(value);
    public bool TryRemove(TNode node) => _nodes.Remove(node);

    public readonly record struct LinkEntry(TNode A, TNode B) : IEquatable<LinkEntry>
    {
        public bool Equals(LinkEntry other)
        {
            return (A.Equals(other.A) && B.Equals(other.B)) || (A.Equals(other.B) && B.Equals(other.A));
        }
        
        // hashcode needs to be equal if A and B are reversed
        public override int GetHashCode() => A.GetHashCode() ^ B.GetHashCode();
    }
}
