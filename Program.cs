using System;
using System.Collections.Generic;
using System.Linq;

class Node<T>
{
    public T Value { get; set; }
    public List<Edge<T>> Edges { get; set; } = new List<Edge<T>>();
    public Node(T value)
    {
        Value = value;
    }
}

class Edge<T>
{
    public Node<T> From { get; set; }
    public Node<T> To { get; set; }
    public Edge(Node<T> from, Node<T> to)
    {
        From = from;
        To = to;
    }
}

class Graph<T>
{
    private Dictionary<T, Node<T>> nodes = new Dictionary<T, Node<T>>();
    private List<Edge<T>> edges = new List<Edge<T>>();

    public void AddNode(Node<T> node)
    {
        if (nodes.ContainsKey(node.Value))
        {
            Console.WriteLine($"Error: Node with value {node.Value} already exists.");
            return;
        }
        nodes.Add(node.Value, node);
        Console.WriteLine($"Node with value {node.Value} added.");
    }

    public void AddEdge(Node<T> from, Node<T> to)
    {
        if (!nodes.ContainsKey(from.Value) || !nodes.ContainsKey(to.Value))
        {
            Console.WriteLine("Error: One or both nodes do not exist in the graph.");
            return;
        }

        var edge = new Edge<T>(from, to);
        edges.Add(edge);
        from.Edges.Add(edge);
        Console.WriteLine($"Edge from {from.Value} to {to.Value} added.");
    }

    public Node<T> GetNode(T value)
    {
        if (nodes.TryGetValue(value, out Node<T> node))
        {
            return node;
        }
        return null;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var graph = new Graph<int>();
        while (true)
        {
            Console.WriteLine("Enter command (Add_node, Add_edge, Exit):");
            var command = Console.ReadLine();
            if (command == "Exit") break;

            switch (command)
            {
                case "Add_node":
                    Console.WriteLine("Enter node value (as int):");
                    var value = int.Parse(Console.ReadLine());
                    var node = new Node<int>(value);
                    graph.AddNode(node);
                    break;

                case "Add_edge":
                    Console.WriteLine("Enter from node value:");
                    var fromValue = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter to node value:");
                    var toValue = int.Parse(Console.ReadLine());
                    var fromNode = graph.GetNode(fromValue);
                    var toNode = graph.GetNode(toValue);

                    if (fromNode != null && toNode != null)
                    {
                        graph.AddEdge(fromNode, toNode);
                    }
                    else
                    {
                        Console.WriteLine("One or both nodes not found.");
                    }
                    break;

                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }
}
