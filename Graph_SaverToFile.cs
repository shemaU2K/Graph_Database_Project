using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class NodeDTO<TKey, TData>
{
    public TKey Id { get; set; }
    public TData Data { get; set; }
}

public class EdgeDTO<TKey, TEdgeData>
{
    public TKey FromId { get; set; }
    public TKey ToId { get; set; }
    public TEdgeData Data { get; set; }
}

public class GraphContainer<TKey, TData, TEdgeData>
    where TData : Graph_Data
    where TEdgeData : Edge_Data
{
    public List<NodeDTO<TKey, TData>> Nodes { get; set; } = new();
    public List<EdgeDTO<TKey, TEdgeData>> Edges { get; set; } = new();
}
public class Graph_SaverToFile<TKey, TData, TEdge_data>
    where TData : Graph_Data
    where TEdge_data : Edge_Data
{
    public static void SaveToFile<TKey, TData, TEdgeData>(
        Graph<TKey, TData, TEdgeData> graph,
        string filePath)
        where TData : Graph_Data
        where TEdgeData : Edge_Data
    {
        var container = new GraphContainer<TKey, TData, TEdgeData>();
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

        foreach (var nodeDTO in container.Nodes)
        {
            graph.AddNode(nodeDTO.Id, nodeDTO.Data);
        }

        foreach (var edgeDTO in container.Edges)
        {
            graph.AddEdge(edgeDTO.FromId, edgeDTO.ToId, edgeDTO.Data);
        }
        return graph;
    }
}
