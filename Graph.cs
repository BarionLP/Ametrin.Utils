using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Ametrin.Utils;

[DataContract(IsReference = true)]
public sealed class Graph<TNode, TValue> where TNode : INode<TValue> where TValue : notnull{
    [DataMember(Name = "nodes")]
    private readonly HashSet<TNode> Nodes = new();

    public int Count => Nodes.Count;

    public Result<TNode> TryGet(TValue value) {
        if(value is null) return ResultStatus.InvalidArgument;
        var result = Nodes.FirstOrDefault(entry => entry.Value.Equals(value));
        if(result is null) return ResultStatus.Null;
        return result;
    }

    public Result<TNode> TryGet(Index idx) {
        try {
            return Nodes.ElementAt(idx);
        } catch(ArgumentOutOfRangeException) {
            return ResultStatus.OutOfRange;
        } catch {
            return ResultStatus.Failed;
        }

    }

    public Result TryAdd(TNode node) {
        if(node is null) return ResultStatus.InvalidArgument;
        if(Exists(node.Value)) return ResultStatus.AlreadyExists;
        Nodes.Add(node);
        return ResultStatus.Succeeded;
    }

    public void Link(TNode a, TNode b) {
        if(!Exists(a) || !Exists(b)) throw new ArgumentException("Cannot link nodes outside of the graph");
        a.Link(b);
        b.Link(a);
    }

    public bool Exists(TValue value) {
        return Nodes.Any(entry => entry.Value.Equals(value));
    }
    public bool Exists(TNode node) {
        return Nodes.Contains(node);
    }

    public bool TryRemove(TValue value) {
        var node = Nodes.FirstOrDefault(entry => entry.Value.Equals(value));
        if(node is null) return false;
        return TryRemove(node);
    }
    public int TryRemoveAll(TValue value) {
        return Nodes.RemoveWhere(entry => entry.Value.Equals(value));
    }
    public bool TryRemove(TNode node) {
        node.Delete();
        return Nodes.Remove(node);
    }
}

[DataContract(IsReference = true)]
public sealed class Node<T> : INode<T> where T : notnull {
    [DataMember(Name = "value"), NotNull]
    public required T Value { get; init; }
    [DataMember(Name = "links", EmitDefaultValue = false)]
    private readonly HashSet<INode<T>> Links = new();

    public void Link(INode<T> node) {
        Links.Add(node);
    }
    public bool HasLink(INode<T> node) {
        return Links.Contains(node);
    }
    public bool RemoveLink(INode<T> node) {
        return Links.Remove(node);
    }
    public void Delete() {
        foreach(var node in Links) {
            node.RemoveLink(this);
        }
        Links.Clear();
    }

    public static implicit operator T(Node<T> node) { return node.Value; }
}

public interface INode<T> where T : notnull{
    T Value { get; }
    public void Link(INode<T> node);
    public bool HasLink(INode<T> node);
    public bool RemoveLink(INode<T> node);
    public void Delete();
}

public static class GraphSerializer{
    public static void Serialize<TNode, TValue>(Graph<TNode, TValue> graph, string path) where TNode : INode<TValue> where TValue : notnull {
        using var stream = new FileStream(path, FileMode.Create);
        var serializer = new DataContractSerializer(typeof(Graph<TNode, TValue>));
        serializer.WriteObject(stream, graph);
    }

    public static Result<Graph<TNode, TValue>> Deserialize<TNode, TValue>(string path) where TNode : INode<TValue> where TValue : notnull {
        if(!File.Exists(path)) return ResultStatus.PathNotFound;
        using var stream = new FileStream(path, FileMode.Open);
        var serializer = new DataContractSerializer(typeof(Graph<TNode, TValue>));

        try{
            if(serializer.ReadObject(stream) is not Graph<TNode, TValue> result) return ResultStatus.Null;
            return result;
        } catch(Exception) {
            return ResultStatus.Failed;
        }
    }
}