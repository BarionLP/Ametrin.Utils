using Ametrin.Optional;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Ametrin.Utils.Graph;

[DataContract(Namespace = "Ametrin.Utils", IsReference = true)]
public sealed class Graph<TNode, TValue> where TNode : INode<TValue> where TValue : notnull
{
    [DataMember(Name = "nodes")]
    private readonly HashSet<TNode> Nodes = new();

    public int Count => Nodes.Count;

    public Result<TNode> TryGet(TValue value)
    {
        if (value is null)
        {
            return new ArgumentNullException(nameof(value));
        }

        var result = Nodes.FirstOrDefault(entry => entry.Value.Equals(value));
        return result is null ? new InvalidOperationException("Graph contains no matching element") : result;
    }

    public Result<TNode> TryGet(Index idx)
    {
        try
        {
            return Nodes.ElementAt(idx);
        }
        catch (ArgumentOutOfRangeException e)
        {
            return e;
        }
        catch
        {
            return new Exception();
        }
    }

    public ErrorState<Exception> TryAdd(TNode node)
    {
        if (node is null)
        {
            return new ArgumentNullException(nameof(node));
        }

        if (Exists(node.Value))
        {
            return new ArgumentException("Duplicate Node", nameof(node));
        }

        Nodes.Add(node);
        return null;
    }

    public void Link(TNode a, TNode b)
    {
        if (!Exists(a) || !Exists(b))
            throw new ArgumentException("Cannot link nodes outside of the graph");
        a.Link(b);
        b.Link(a);
    }

    public bool Exists(TValue value)
    {
        return Nodes.Any(entry => entry.Value.Equals(value));
    }
    public bool Exists(TNode node)
    {
        return Nodes.Contains(node);
    }

    public bool TryRemove(TValue value)
    {
        var node = Nodes.FirstOrDefault(entry => entry.Value.Equals(value));
        if (node is null)
            return false;
        return TryRemove(node);
    }
    public int TryRemoveAll(TValue value)
    {
        return Nodes.RemoveWhere(entry => entry.Value.Equals(value));
    }
    public bool TryRemove(TNode node)
    {
        node.Delete();
        return Nodes.Remove(node);
    }
}

[DataContract(IsReference = true)]
public sealed class Node<T> : INode<T> where T : notnull
{
    [DataMember(Name = "value"), NotNull]
    public required T Value { get; init; }
    [DataMember(Name = "links", EmitDefaultValue = false)]
    private readonly HashSet<INode<T>> Links = [];

    public void Link(INode<T> node)
    {
        Links.Add(node);
    }
    public bool HasLink(INode<T> node)
    {
        return Links.Contains(node);
    }
    public bool RemoveLink(INode<T> node)
    {
        return Links.Remove(node);
    }
    public void Delete()
    {
        foreach (var node in Links)
        {
            node.RemoveLink(this);
        }
        Links.Clear();
    }

    public static implicit operator T(Node<T> node) { return node.Value; }
}

public interface INode<T> where T : notnull
{
    T Value { get; }
    public void Link(INode<T> node);
    public bool HasLink(INode<T> node);
    public bool RemoveLink(INode<T> node);
    public void Delete();
}

public static class GraphSerializer
{
    public static readonly DataContractSerializerSettings DataContractSettings = new() { };

    public static void Serialize<TNode, TValue>(Graph<TNode, TValue> graph, FileInfo target, bool indent = false) where TNode : class, INode<TValue> where TValue : notnull
    {
        using var stream = target.Create();
        var serializer = new DataContractSerializer(typeof(Graph<TNode, TValue>), DataContractSettings);
        var settings = new XmlWriterSettings
        {
            Indent = indent,
        };
        using var writer = XmlWriter.Create(stream, settings);
        serializer.WriteObject(writer, graph);
    }

    public static Result<Graph<TNode, TValue>> Deserialize<TNode, TValue>(FileInfo target) where TNode : class, INode<TValue> where TValue : notnull
    {
        if (!target.Exists)
            return new FileNotFoundException(null, target.FullName);
        using var stream = target.OpenRead();
        var serializer = new DataContractSerializer(typeof(Graph<TNode, TValue>));

        try
        {
            return serializer.ReadObject(stream) is Graph<TNode, TValue> result ? result : new NullReferenceException();
        }
        catch (Exception e)
        {
            return e;
        }
    }
}