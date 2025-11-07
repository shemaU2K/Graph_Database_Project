using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Edge_Data;

public abstract class Graph_Data
{
    public abstract string GetDisplayName();
    public virtual string GetDetails()
    {
        return $"Data: {GetDisplayName}";
    }
    public class PersonData : Graph_Data
    {
        public string Name { get; set; }
        public int Age { get; set; }
        override public string GetDisplayName()
        {
            return Name;
        }
        override public string GetDetails()
        {
            return $"Name: {Name}, Age: {Age}";
        }
    }
    public class CityData : Graph_Data
    {
        public string CityName { get; set; }
        public int Population { get; set; }
        override public string GetDisplayName()
        {
            return CityName;
        }
        override public string GetDetails()
        {
            return $"City: {CityName}, Population: {Population}";
        }
    }
    public class CompanyData : Graph_Data 
    {
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public override string GetDisplayName()
        {
            return CompanyName;
        }
        public override string GetDetails()
        {
            return $"Company: {CompanyName}, Indystry: {Industry}";
        }
    }

}
public class Node<Tkey, Tdata ,TEdge_data> 
    where Tdata : Graph_Data 
    where TEdge_data : Edge_Data
{
    public Tkey ID { get; set; }
    public Tdata Data { get; set; }
    public List<Edge<Tkey, Tdata, TEdge_data>> Edges { get; set; } = new List<Edge<Tkey, Tdata, TEdge_data>>();    
    public Node(Tkey id , Tdata data)
    {
        ID = id;
        Data = data;
    }
}
public abstract class Edge_Data
{
    public double Weight { get; set; } = 1.0;
    public abstract string GetDecscription();
    public class Edge_Friends : Edge_Data
    {
        public DateTime FriendsSince { get; set; }
        public override string GetDecscription() 
        { 
            return $"Friends since {FriendsSince.Year}"; 
        }
    }
    public class WorksAtEdge : Edge_Data
    {
        public string Role { get; set; }
        public override string GetDecscription()
        {
            return $"Works since {Role}";
        }
    } 
}
public class Edge<Tkey, Tdata ,TEdge_data> 
    where Tdata : Graph_Data 
    where TEdge_data : Edge_Data
{
    public Node<Tkey,Tdata, TEdge_data>  From { get; set; }
    public Node<Tkey, Tdata, TEdge_data> To { get; set; }
    public TEdge_data Data { get; set; }
    public Edge(Node<Tkey, Tdata, TEdge_data> from, Node<Tkey,Tdata, TEdge_data> to, TEdge_data data)
    {
        From = from;
        To = to;
        Data = data;
    }
}

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

    public void AddEdge(TKey fromId, TKey toId , TEdge_data edge_data)
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

    public Node<TKey, TData, TEdge_data> GetNode(TKey id)
    {
        nodes.TryGetValue(id, out var node);
        return node;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var graph = new Graph<string, Graph_Data,Edge_Data>();

        var person1 = new Graph_Data.PersonData { Name = "Іван", Age = 30 };
        var person2 = new Graph_Data.PersonData { Name = "Марія", Age = 25 };
        var city1 = new Graph_Data.CityData { CityName = "Київ", Population = 2_800_000 };
        var friendship = new Edge_Friends { FriendsSince = new DateTime(2020, 1, 1) };
        var job = new WorksAtEdge { Role = "Developer" };
        graph.AddNode("p1", person1); 
        graph.AddNode("p2", person2);
        graph.AddNode("c1", city1);

        graph.AddEdge("p1", "p2",friendship);
        graph.AddEdge("p1", "c1",job);

        Console.WriteLine("\n--- Details of node 'p1' ---");
        var node1 = graph.GetNode("p1");
        if (node1 != null)
        {
            Console.WriteLine(node1.Data.GetDetails());
        }

        Console.WriteLine("\n--- Details of node 'c1' ---");
        var node2 = graph.GetNode("c1");
        if (node2 != null)
        {
            Console.WriteLine(node2.Data.GetDetails());
        }
    }
}
