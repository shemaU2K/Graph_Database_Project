using System;
using System.Collections.Generic;
using System.Linq;

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
        public string PositionInCompany { get; set; }
        override public string GetDisplayName()
        {
            return Name;
        }
        override public string GetDetails()
        {
            return $"Name: {Name}, Age: {Age}, Position in company: {PositionInCompany}";
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
public class Node<Tkey, Tdata> where Tdata : Graph_Data
{
    public Tkey ID { get; set; }
    public Tdata Data { get; set; }
    public List<Edge<Tkey, Tdata>> Edges { get; set; } = new List<Edge<Tkey, Tdata>>();    
    public Node(Tkey id , Tdata data)
    {
        ID = id;
        Data = data;
    }
}

public class Edge<Tkey, Tdata> where Tdata : Graph_Data
{
    public Node<Tkey,Tdata>  From { get; set; }
    public Node<Tkey, Tdata> To { get; set; }
    public Edge(Node<Tkey, Tdata> from, Node<Tkey,Tdata> to)
    {
        From = from;
        To = to;
    }
}

public class Graph<TKey, TData> where TData : Graph_Data
{
    private Dictionary<TKey, Node<TKey, TData>> nodes = new Dictionary<TKey, Node<TKey, TData>>();

    private List<Edge<TKey, TData>> edges = new List<Edge<TKey, TData>>();

    public void AddNode(TKey id, TData data)
    {
        if (nodes.ContainsKey(id))
        {
            Console.WriteLine($"Error: Node with ID {id} already exists.");
            return;
        }

        var newNode = new Node<TKey, TData>(id, data);
        nodes.Add(id, newNode);

        Console.WriteLine($"Node '{newNode.Data.GetDisplayName()}' added with ID {id}.");
    }

    public void AddEdge(TKey fromId, TKey toId)
    {
        // Шукаємо вузли за їхніми КЛЮЧАМИ (ID)
        if (nodes.TryGetValue(fromId, out var fromNode) &&
            nodes.TryGetValue(toId, out var toNode))
        {
            var edge = new Edge<TKey, TData>(fromNode, toNode);
            edges.Add(edge);
            fromNode.Edges.Add(edge);

            Console.WriteLine($"Edge from '{fromNode.Data.GetDisplayName()}' to '{toNode.Data.GetDisplayName()}' added.");
        }
        else
        {
            Console.WriteLine("Error: One or both nodes not found by ID.");
        }
    }

    public Node<TKey, TData> GetNode(TKey id)
    {
        nodes.TryGetValue(id, out var node);
        return node;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var graph = new Graph<string, Graph_Data>();

        var person1 = new Graph_Data.PersonData { Name = "Іван", Age = 30 };
        var person2 = new Graph_Data.PersonData { Name = "Марія", Age = 25 };
        var city1 = new Graph_Data.CityData { CityName = "Київ", Population = 2_800_000 };

        graph.AddNode("p1", person1); 
        graph.AddNode("p2", person2);
        graph.AddNode("c1", city1);

        graph.AddEdge("p1", "p2");
        graph.AddEdge("p1", "c1");

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
