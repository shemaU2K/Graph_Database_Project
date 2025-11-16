using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using static Edge_Data;
using static Graph_Data;

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
    public void AddEdge(Node<TKey, TData, TEdge_data>fromNode , Node<TKey,TData, TEdge_data> toNode , TEdge_data edge_data)
    {   if(fromNode == null || toNode == null)
        {
            Console.WriteLine("Error: One or both nodes are null.");
            return;
        }
        else {
            var edge = new Edge<TKey, TData, TEdge_data>(fromNode, toNode, edge_data);
        edges.Add(edge);
        fromNode.Edges.Add(edge);
        Console.WriteLine($"Edge from '{fromNode.Data.GetDisplayName()}' to '{toNode.Data.GetDisplayName()}' added."); 
        }
    }

    public void RemoveNode(TKey id) {
        if(nodes.Remove(id, out var node))
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

public static class GraphAlgorithms
{
    public static List<Node<TKey, TData, TEdge_data>> GetNeighbors<TKey, TData, TEdge_data>(
        Node<TKey, TData, TEdge_data> node)
        where TData : Graph_Data
        where TEdge_data : Edge_Data
    {
        if (node == null)
        {
            return new List<Node<TKey, TData, TEdge_data>>();
        }

        List<Node<TKey, TData, TEdge_data>> neighbors = new List<Node<TKey, TData, TEdge_data>>();
        foreach (var edge in node.Edges)
        {
            neighbors.Add(edge.To);
        }
        return neighbors;
    }

    public static int GetOutDegree<TKey, TData, TEdge_data>(
        Node<TKey, TData, TEdge_data> node)
        where TData : Graph_Data
        where TEdge_data : Edge_Data
    {
        return node?.Edges.Count ?? 0;
    }

    public static List<Node<TKey, TData, TEdge_data>> FindNodesByType<TKey, TData, TEdge_data, TFindData>(
        Graph<TKey, TData, TEdge_data> graph)
        where TData : Graph_Data
        where TEdge_data : Edge_Data
        where TFindData : Graph_Data
    {
        var AllNodes = graph.GetAllNodes(); 
        var filteredNodes = AllNodes
            .Where(node => node.Data is TFindData)
            .ToList();
        return filteredNodes;
    }
    
    public static List<Node<Tkey,Tdata,TEdge_data>> BFS_Reserch <Tkey,Tdata,TEdge_data>(
        Graph<Tkey,Tdata,TEdge_data> graph,
        Node<Tkey,Tdata,TEdge_data> startNode,
        Func<Node<Tkey,Tdata,TEdge_data>, bool> goalTest)
        where Tdata : Graph_Data
        where TEdge_data : Edge_Data
    {
        var visited = new HashSet<Node<Tkey,Tdata,TEdge_data>>();
        var queue = new Queue<Node<Tkey,Tdata,TEdge_data>>();
        var cameFrom = new Dictionary<Node<Tkey, Tdata, TEdge_data>, Node<Tkey, Tdata, TEdge_data>>();
        Node<Tkey, Tdata, TEdge_data> goalNode = null; 
        queue.Enqueue(startNode);
        visited.Add(startNode);
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            if (goalTest(currentNode))
            {
                goalNode = currentNode;
                break;
            }
            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = currentNode;
                }
            }
        }
        var resultPath = new List<Node<Tkey, Tdata, TEdge_data>>();
        if (goalNode == null)
        {
            return resultPath;
        }
        var current = goalNode;
        while (current != startNode)
        {
            resultPath.Add(current);
            current = cameFrom[current];
        }
        resultPath.Add(startNode);
        resultPath.Reverse();
        return resultPath;
    }
   static public List<Node<Tkey, Tdata, TEdge_data>> DFS_Reserch<Tkey, Tdata, TEdge_data>(
        Graph<Tkey, Tdata, TEdge_data> graph,
        Node<Tkey, Tdata, TEdge_data> startNode,
        Func<Node<Tkey, Tdata, TEdge_data>, bool> goalTest)
        where Tdata : Graph_Data
        where TEdge_data : Edge_Data
    {
        var visited = new HashSet<Node<Tkey, Tdata, TEdge_data>>();
        var stack = new Stack<Node<Tkey, Tdata, TEdge_data>>();
        var cameFrom = new Dictionary<Node<Tkey, Tdata, TEdge_data>, Node<Tkey, Tdata, TEdge_data>>();
        Node<Tkey, Tdata, TEdge_data> goalNode = null;
        stack.Push(startNode);
        visited.Add(startNode);
        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            if (goalTest(currentNode))
            {
                goalNode = currentNode;
                break;
            }
            foreach (var edge in currentNode.Edges)
            {
                var neighbor = edge.To;
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    cameFrom[neighbor] = currentNode;
                    stack.Push(neighbor);
                }
            }
        }
        var resultPath = new List<Node<Tkey, Tdata, TEdge_data>>();
        if (goalNode == null)
        {
            return resultPath;
        }
        var current = goalNode;
        while (current != startNode)
        {
            resultPath.Add(current);
            current = cameFrom[current];
        }
        resultPath.Add(startNode);
        resultPath.Reverse();
        return resultPath;
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
        graph.AddEdge("p1", "c1", job);
        var people = GraphAlgorithms.FindNodesByType<string, Graph_Data, Edge_Data, PersonData>(graph);

        Console.WriteLine($"Знайдено людей: {people.Count}"); // Виведе: 2
        Console.WriteLine("\n--- Details of node 'p1' ---");
        var node1 = graph.GetNode("p1");
        graph.RemoveNode("p1");
        Console.WriteLine($"Node {person1} has been removed");
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
