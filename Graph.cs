using System;

public class Graph<TKey, TData, TEdge_data>
    where TData : Graph_Data
    where TEdge_data : Edge_Data
{
    private Dictionary<TKey, Node<TKey, TData, TEdge_data>> nodes = new Dictionary<TKey, Node<TKey, TData, TEdge_data>>();

    private List<Edge<TKey, TData, TEdge_data>> edges = new List<Edge<TKey, TData, TEdge_data>>();

    public void AddNode(TKey id, TData data)
    {
        if (nodes.ContainsKey(id))
        {
            Console.WriteLine($"Error: Node with ID {id} already exists.");
            return;
        }

        var newNode = new Node<TKey, TData, TEdge_data>(id, data);
        nodes.Add(id, newNode);

        Console.WriteLine($"Node '{newNode.Data.GetDisplayName()}' added with ID {id}.");
    }

    public void AddEdge(TKey fromId, TKey toId, TEdge_data edge_data)
    {
        if (nodes.TryGetValue(fromId, out var fromNode) &&
            nodes.TryGetValue(toId, out var toNode))
        {
            var edge = new Edge<TKey, TData, TEdge_data>(fromNode, toNode, edge_data);
            edges.Add(edge);
            fromNode.Edges.Add(edge);

            Console.WriteLine($"Edge from '{fromNode.Data.GetDisplayName()}' to '{toNode.Data.GetDisplayName()}' added.");
        }
        else
        {
            Console.WriteLine("Error: One or both nodes not found by ID.");
        }
    }
    public void AddEdge(Node<TKey, TData, TEdge_data> fromNode, Node<TKey, TData, TEdge_data> toNode, TEdge_data edge_data)
    {
        if (fromNode == null || toNode == null)
        {
            Console.WriteLine("Error: One or both nodes are null.");
            return;
        }
        else
        {
            var edge = new Edge<TKey, TData, TEdge_data>(fromNode, toNode, edge_data);
            edges.Add(edge);
            fromNode.Edges.Add(edge);
            Console.WriteLine($"Edge from '{fromNode.Data.GetDisplayName()}' to '{toNode.Data.GetDisplayName()}' added.");
        }
    }

    public void RemoveNode(TKey id)
    {
        if (nodes.Remove(id, out var node))
        {
            edges.RemoveAll(e => e.From == node || e.To == node);
            Console.WriteLine($"Node with ID {id} removed.");
        }
        else
        {
            Console.WriteLine($"Error: Node with ID {id} not found.");
        }
    }
    public Node<TKey, TData, TEdge_data> GetNode(TKey id)
    {
        nodes.TryGetValue(id, out var node);
        return node;
    }
    public Node<TKey, TData, TEdge_data> GetNode(TData data)
    {
        return nodes.Values.FirstOrDefault(n => n.Data.Equals(data));
    }
    public IEnumerable<Node<TKey, TData, TEdge_data>> GetAllNodes()
    {
        return nodes.Values;
    }
}