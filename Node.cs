using System;

public class Node<Tkey, Tdata, TEdge_data>
    where Tdata : Graph_Data
    where TEdge_data : Edge_Data
{
    public Tkey ID { get; set; }
    public Tdata Data { get; set; }
    public List<Edge<Tkey, Tdata, TEdge_data>> Edges { get; set; } = new List<Edge<Tkey, Tdata, TEdge_data>>();
    public Node(Tkey id, Tdata data)
    {
        ID = id;
        Data = data;
    }
}