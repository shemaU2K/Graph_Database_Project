using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Graph_database;
/// <summary>
/// Data Transfer Object (DTO) representing a node for serialization purposes.
/// Decouples the storage format from the runtime logic.
/// </summary>
/// <typeparam name="TKey">The type of the node's unique identifier.</typeparam>
/// <typeparam name="TData">The type of data stored in the node.</typeparam>
public class NodeDTO<TKey, TData>
{
    /// <summary>
    /// Gets or sets the unique identifier of the node.
    /// </summary>
    public TKey Id { get; set; }

    /// <summary>
    /// Gets or sets the data payload of the node.
    /// </summary>
    public TData Data { get; set; }
}

/// <summary>
/// Data Transfer Object (DTO) representing an edge for serialization purposes.
/// Stores references as IDs instead of objects to avoid circular dependencies in JSON.
/// </summary>
/// <typeparam name="TKey">The type of the node identifiers.</typeparam>
/// <typeparam name="TEdgeData">The type of data stored in the edge.</typeparam>
public class EdgeDTO<TKey, TEdgeData>
{
    /// <summary>
    /// Gets or sets the ID of the source node.
    /// </summary>
    public TKey FromId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the target node.
    /// </summary>
    public TKey ToId { get; set; }

    /// <summary>
    /// Gets or sets the data payload of the edge (e.g., Weight).
    /// </summary>
    public TEdgeData Data { get; set; }
}

/// <summary>
/// Container class used to flatten the graph structure into linear lists for serialization.
/// </summary>
/// <typeparam name="TKey">The type of node identifiers.</typeparam>
/// <typeparam name="TData">The type of node data.</typeparam>
/// <typeparam name="TEdgeData">The type of edge data.</typeparam>
public class GraphContainer<TKey, TData, TEdgeData>
    where TData : Graph_Data
    where TEdgeData : Edge_Data
{
    /// <summary>
    /// A list of all nodes in the graph.
    /// </summary>
    public List<NodeDTO<TKey, TData>> Nodes { get; set; } = new();

    /// <summary>
    /// A list of all edges in the graph.
    /// </summary>
    public List<EdgeDTO<TKey, TEdgeData>> Edges { get; set; } = new();
}

/// <summary>
/// Static utility class responsible for persisting the graph to the file system.
/// Implements logic to flatten the graph structure for JSON serialization and reconstruct it upon loading.
/// </summary>
public static class Graph_SaverToFile
{
    /// <summary>
    /// Serializes the graph structure to a JSON file.
    /// Converts the complex graph object into a flat container (DTO pattern) to handle circular references.
    /// </summary>
    /// <typeparam name="TKey">Type of the node ID.</typeparam>
    /// <typeparam name="TData">Type of the node data.</typeparam>
    /// <typeparam name="TEdgeData">Type of the edge data.</typeparam>
    /// <param name="graph">The graph instance to save.</param>
    /// <param name="filePath">The path to the destination file.</param>
    public static void SaveToFile<TKey, TData, TEdgeData>(
        Graph<TKey, TData, TEdgeData> graph,
        string filePath)
        where TData : Graph_Data
        where TEdgeData : Edge_Data
    {
        var container = new GraphContainer<TKey, TData, TEdgeData>();
        
        // Flattening process: Convert runtime objects to DTOs
        foreach (var node in graph.GetAllNodes())
        {
            container.Nodes.Add(new NodeDTO<TKey, TData>
            {
                Id = node.ID,
                Data = node.Data
            });

            foreach (var edge in node.Edges)
            {
                container.Edges.Add(new EdgeDTO<TKey, TEdgeData>
                {
                    FromId = node.ID,
                    ToId = edge.To.ID,
                    Data = edge.Data
                });
            }
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(container, options);
        File.WriteAllText(filePath, jsonString);
    }

    /// <summary>
    /// Deserializes a graph from a JSON file.
    /// Reconstructs the graph in two passes: first creates nodes, then restores connections (edges).
    /// </summary>
    /// <typeparam name="TKey">Type of the node ID.</typeparam>
    /// <typeparam name="TData">Type of the node data.</typeparam>
    /// <typeparam name="TEdgeData">Type of the edge data.</typeparam>
    /// <param name="filePath">The path to the source file.</param>
    /// <returns>A fully reconstructed Graph object, or an empty Graph if the file is not found.</returns>
    public static Graph<TKey, TData, TEdgeData> LoadFromFile<TKey, TData, TEdgeData>(string filePath)
        where TData : Graph_Data
        where TEdgeData : Edge_Data
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("[ERROR]: File not found. Returning new empty graph.");
            return new Graph<TKey, TData, TEdgeData>();
        }

        string jsonString = File.ReadAllText(filePath);
        var container = JsonSerializer.Deserialize<GraphContainer<TKey, TData, TEdgeData>>(jsonString);
        var graph = new Graph<TKey, TData, TEdgeData>();

        // Pass 1: Restore Nodes (The Skeleton)
        foreach (var nodeDTO in container.Nodes)
        {
            graph.AddNode(nodeDTO.Id, nodeDTO.Data);
        }

        // Pass 2: Restore Edges (The Connections)
        // Since all nodes exist now, we can link them by ID
        foreach (var edgeDTO in container.Edges)
        {
            graph.AddEdge(edgeDTO.FromId, edgeDTO.ToId, edgeDTO.Data);
        }
        return graph;
    }
}

