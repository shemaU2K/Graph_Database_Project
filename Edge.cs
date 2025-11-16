using System;

public class Edge<Tkey, Tdata, TEdge_data>
    where Tdata : Graph_Data
    where TEdge_data : Edge_Data
{
    public Node<Tkey, Tdata, TEdge_data> From { get; set; }
    public Node<Tkey, Tdata, TEdge_data> To { get; set; }
    public TEdge_data Data { get; set; }
    public Edge(Node<Tkey, Tdata, TEdge_data> from, Node<Tkey, Tdata, TEdge_data> to, TEdge_data data)
    {
        From = from;
        To = to;
        Data = data;
    }
}
