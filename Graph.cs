using System;
using System.Collections.Generic;
using System.Linq;
namespace Graph_database; 
/// <summary>
/// Represents a generic graph data structure implemented using adjacency lists.
/// Supports generic keys, polymorphic node data, and polymorphic edge data.
/// </summary>
/// <typeparam name="TKey">The type of the unique identifier for each node (e.g., string, int, Guid).</typeparam>
/// <typeparam name="TData">The type of data stored in the node. Must inherit from <see cref="Graph_Data"/>.</typeparam>
/// <typeparam name="TEdge_data">The type of data stored in the edge. Must inherit from <see cref="Edge_Data"/>.</typeparam>
public class Graph<TKey, TData, TEdge_data>
    where TData : Graph_Data
    where TEdge_data : Edge_Data
{
    /// <summary>
    /// Internal dictionary for O(1) node retrieval by ID.
    /// </summary>
    private Dictionary<TKey, Node<TKey, TData, TEdge_data>> nodes = new Dictionary<TKey, Node<TKey, TData, TEdge_data>>();

    /// <summary>
    /// Global list of all edges in the graph.
    /// </summary>
    private List<Edge<TKey, TData, TEdge_data>> edges = new List<Edge<TKey, TData, TEdge_data>>();

    /// <summary>
    /// Adds a new node to the graph with a unique identifier and associated data.
    /// </summary>
    /// <param name="id">The unique key to identify the node.</param>
    /// <param name="data">The polymorphic data payload for the node.</param>
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

    /// <summary>
    /// Creates a directed edge between two existing nodes identified by their keys.
    /// </summary>
    /// <param name="fromId">The key of the source node.</param>
    /// <param name="toId">The key of the target node.</param>
    /// <param name="edge_data">The data payload for the edge (e.g., weight, relationship type).</param>
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

    /// <summary>
    /// Overloaded method to create a directed edge directly between two node objects.
    /// </summary>
    /// <param name="fromNode">The source node object.</param>
    /// <param name="toNode">The target node object.</param>
    /// <param name="edge_data">The data payload for the edge.</param>
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

    /// <summary>
    /// Removes a node and all its connected edges (incoming and outgoing) from the graph.
    /// </summary>
    /// <param name="id">The unique identifier of the node to remove.</param>
    public void RemoveNode(TKey id)
    {
        if (nodes.Remove(id, out var node))
        {
            // Removes edges associated with this node from the global list
            edges.RemoveAll(e => e.From == node || e.To == node);
            Console.WriteLine($"Node with ID {id} removed.");
            foreach (var otherNode in nodes.Values)
            {
                otherNode.Edges.RemoveAll(e => e.To == node);
            }
        }
        else
        {
            Console.WriteLine($"Error: Node with ID {id} not found.");
        }
    }

    /// <summary>
    /// Retrieves a node by its unique identifier. This operation is O(1).
    /// </summary>
    /// <param name="id">The key of the node.</param>
    /// <returns>The node object if found; otherwise, null (default).</returns>
    public Node<TKey, TData, TEdge_data> GetNode(TKey id)
    {
        nodes.TryGetValue(id, out var node);
        return node;
    }

    /// <summary>
    /// Retrieves the first node that contains the specified data payload. This operation is O(N).
    /// </summary>
    /// <param name="data">The data object to search for.</param>
    /// <returns>The node object if found; otherwise, null.</returns>
    public Node<TKey, TData, TEdge_data> GetNode(TData data)
    {
        return nodes.Values.FirstOrDefault(n => n.Data.Equals(data));
    }

    /// <summary>
    /// Returns an enumerable collection of all nodes in the graph.
    /// </summary>
    /// <returns>A collection of nodes.</returns>
    public IEnumerable<Node<TKey, TData, TEdge_data>> GetAllNodes()
    {
        return nodes.Values;
    }

    /// <summary>
    /// Returns an enumerable collection of all edges in the graph.
    /// </summary>
    /// <returns>A collection of edges.</returns>
    public IEnumerable<Edge<TKey, TData, TEdge_data>> GetAllEdges()
    {
        return edges;
    }

    /// <summary>
    /// Removes a specific directed edge between two nodes.
    /// </summary>
    /// <param name="fromID">The ID of the source node.</param>
    /// <param name="toID">The ID of the target node.</param>
    /// <returns>True if the edge was successfully removed; otherwise, false.</returns>
    public bool RemoveEdge(TKey fromID, TKey toID)
    {
        if (nodes.TryGetValue(fromID, out var fromNode))
        {
            // Find the edge in the source node's adjacency list
            var edge = fromNode.Edges.FirstOrDefault(e => e.To.ID.Equals(toID));

            if (edge != null)
            {
                // Remove from local list and global list
    
                fromNode.Edges.Remove(edge);
                edges.Remove(edge);
                Console.WriteLine($"[INFO]: Edge removed.");
                return true;
            }
        }

        Console.WriteLine("[ERROR]: Edge or Node not found.");
        return false;
    }
}

