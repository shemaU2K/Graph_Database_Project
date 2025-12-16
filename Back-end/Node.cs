using System;
using System.Collections.Generic;
namespace Graph_database;
/// <summary>
/// Represents a single node (vertex) within the graph structure.
/// Acts as a container for data and maintains a list of outgoing connections.
/// </summary>
/// <typeparam name="Tkey">The type of the unique identifier for the node.</typeparam>
/// <typeparam name="Tdata">The type of the data payload stored in the node.</typeparam>
/// <typeparam name="TEdge_data">The type of data stored in the connected edges.</typeparam>
public class Node<Tkey, Tdata, TEdge_data>
    where Tdata : Graph_Data
    where TEdge_data : Edge_Data
{
    /// <summary>
    /// Gets or sets the unique identifier for this node.
    /// Used for O(1) lookups in the graph dictionary.
    /// </summary>
    public Tkey ID { get; set; }

    /// <summary>
    /// Gets or sets the payload data associated with this node.
    /// Can be any type derived from <see cref="Graph_Data"/> (e.g., PersonData, CityData).
    /// </summary>
    public Tdata Data { get; set; }

    /// <summary>
    /// Gets or sets the list of outgoing edges (connections) from this node.
    /// This property implements the Adjacency List pattern.
    /// </summary>
    public List<Edge<Tkey, Tdata, TEdge_data>> Edges { get; set; } = new List<Edge<Tkey, Tdata, TEdge_data>>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Node{Tkey, Tdata, TEdge_data}"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the node.</param>
    /// <param name="data">The data payload to store in the node.</param>
    public Node(Tkey id, Tdata data)
    {
        ID = id;
        Data = data;
    }
}

