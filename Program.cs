using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using static Edge_Data;
using static Graph_Data;

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

        Console.WriteLine($"Знайдено людей: {people.Count}");
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
