using System;
namespace Graph_database;
/// <summary>
/// Represents a directed edge (connection) between two nodes in the graph.
/// Stores references to the source and target nodes, along with specific edge data.
/// </summary>
/// <typeparam name="Tkey">The type of the unique identifier for nodes.</typeparam>
/// <typeparam name="Tdata">The type of data stored in the connected nodes.</typeparam>
/// <typeparam name="TEdge_data">The type of data stored in this edge (must inherit from <see cref="Edge_Data"/>).</typeparam>
public class Edge<Tkey, Tdata, TEdge_data>
    where Tdata : Graph_Data
    where TEdge_data : Edge_Data
{
    /// <summary>
    /// Gets or sets the source node where the edge originates.
    /// </summary>
    public Node<Tkey, Tdata, TEdge_data> From { get; set; }

    /// <summary>
    /// Gets or sets the target node where the edge points to.
    /// </summary>
    public Node<Tkey, Tdata, TEdge_data> To { get; set; }

    /// <summary>
    /// Gets or sets the data payload associated with this connection.
    /// Contains properties like Weight, Role, or Date.
    /// </summary>
    public TEdge_data Data { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Edge{Tkey, Tdata, TEdge_data}"/> class.
    /// </summary>
    /// <param name="from">The source node.</param>
    /// <param name="to">The target node.</param>
    /// <param name="data">The data associated with this relationship.</param>
    public Edge(Node<Tkey, Tdata, TEdge_data> from, Node<Tkey, Tdata, TEdge_data> to, TEdge_data data)
    {
        From = from;
        To = to;
        Data = data;
    }
}


