using System;
using System.Collections.Generic;
using System.Linq;
using GraphType = Graph<string, Graph_Data, Edge_Data>;
using NodeType = Node<string, Graph_Data, Edge_Data>;

public class Program
{
    private static GraphType graph = new GraphType();
    private const string FILE_PATH = "graph_database.json";

    static void Main(string[] args)
    {
        Console.WriteLine("=== Graph Database Project (v1.01) ===");
        RunMenuLoop();
    }
    private static void RunMenuLoop()
    {
        while (true)
        {
            Console.WriteLine("\n-------------------------------------------");
            Console.WriteLine("Main menu:");
            Console.WriteLine("1. Add Node");
            Console.WriteLine("2. Add Edge");
            Console.WriteLine("3. Remove Node");
            Console.WriteLine("4. Remove Edge");
            Console.WriteLine("5. BFS / DFS / Dijkstra");
            Console.WriteLine("6. Has Cycle?");
            Console.WriteLine("7. Save");
            Console.WriteLine("8. Load");
            Console.WriteLine("9. Demo Polymorphism");
            Console.WriteLine("0. Exit");

            int choice = ReadInt("> Your choice: ");

            switch (choice)
            {
                case 1: HandleAddNode(); break;
                case 2: HandleAddEdge(); break;
                case 3: HandleRemoveNode(); break;
                case 4: HandleRemoveEdge(); break;
                case 5: HandlePathfinding(); break;
                case 6: RunCycleCheck(); break;
                case 7:
                    Graph_SaverToFile.SaveToFile(graph, FILE_PATH);
                    break;
                case 8:
                    graph = Graph_SaverToFile.LoadFromFile<string, Graph_Data, Edge_Data>(FILE_PATH);
                    break;
                case 9: DemonstratePolymorphism(); break;
                case 0: return;
                default: Console.WriteLine("Uknown command."); break;
            }
        }
    }
    private static int ReadInt(string prompt)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int result))
        {
            return result;
        }
        Console.WriteLine("[ERROR]:Icorrect number format. Try again.");
        return ReadInt(prompt);
    }
    private static string ReadString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
    private static void HandleAddNode()
    {
        Console.WriteLine("\n--- Adding Node ---");
        string nodeID = ReadString("Enter unique ID: ");

        Console.WriteLine("Choose data format:");
        Console.WriteLine("1: PersonData");
        Console.WriteLine("2: CityData");
        Console.WriteLine("3: CompanyData");

        int choice = ReadInt("Choice: ");
        Graph_Data nodeData = null;
        switch (choice)
        {
            case 1:
                string name = ReadString("Name: ");
                int age = ReadInt("Age: ");
                nodeData = new Graph_Data.PersonData { Name = name, Age = age };
                break;
            case 2:
                string city = ReadString("City Name: ");
                int population = ReadInt("Population: ");
                nodeData = new Graph_Data.CityData { CityName = city, Population = population };
                break;

            case 3:
                string company = ReadString("Company Name: ");
                string industry = ReadString("Industry: ");
                nodeData = new Graph_Data.CompanyData { CompanyName = company, Industry = industry };
                break;

            default:
                Console.WriteLine("[ERROR]: Incorrect type of node. Adding canceled.");
                return;
        }
        if (nodeData != null)
        {
            graph.AddNode(nodeID, nodeData);
            Console.WriteLine($"[SUCCESS]: Node '{nodeData.GetDisplayName()}' ({nodeID}) Added.");
        }
    }
    private static void HandleAddEdge()
    {
        Console.WriteLine("\n--- Adding Edge ---");
        var nodeFromID = ReadString("Enter ID start Node: ");
        var nodeToID = ReadString("Enter ID end Node: ");

        Console.WriteLine("Choose data format:");
        Console.WriteLine("1: Friends since");
        Console.WriteLine("2: Work as");

        int choice = ReadInt("Choice: ");
        Edge_Data edgeData = null;
        switch (choice)
        {
            case 1:
                int year = ReadInt("The year the friendship began: ");
                var since = new DateTime(year, 1, 1);
                var weigth = ReadInt("Edge`s weigth(number): ");
                edgeData = new Edge_Data.Edge_Friends { FriendsSince = since, Weight = weigth };
                break;
            case 2:
                string role = ReadString("Role: ");
                var weight = ReadInt("Edge`s weigth(number): ");
                edgeData = new Edge_Data.WorksAtEdge { Role = role, Weight = weight };
                break;
            default:
                Console.WriteLine("[ERROR]: Incorrect type of edge. Adding canceled.");
                return;
        }
        if (edgeData != null)
        {
            var nodeFrom = graph.GetNode(nodeFromID);
            var nodeTo = graph.GetNode(nodeToID);

            if (nodeFrom != null && nodeTo != null)
            {
                graph.AddEdge(nodeFromID, nodeToID, edgeData);
                Console.WriteLine($"[SUCCESS]: Edge {edgeData.GetDecscription()} added.");
            }
            else
            {
                Console.WriteLine("[ERROR]:One or both nodes not found.");
            }
        }
    }
    static void HandleRemoveNode()
    {
        Console.WriteLine("\n--- Removing Node ---");
        string nodeID = ReadString("Enter Node`s ID for removing: ");
        graph.RemoveNode(nodeID);
    }
    static void HandleRemoveEdge()
    {
        Console.WriteLine("\n--- Removing Edge ---");
        string fromID = ReadString("Enter ID start Node: ");
        string toID = ReadString("Enter ID end Node: ");
        if (graph.RemoveEdge(fromID, toID))
        {
            Console.WriteLine("[SUCCESS]: Edge removed.");
        }
        else
        {
            Console.WriteLine("[ERROR]: Edge or nodes not found.");
        }
    }
    private static void DemonstratePolymorphism()
    {
        Console.WriteLine("\n--- 1. Demonstrating Dynamic Polymorphism (GetDetails) ---");
        foreach (var node in graph.GetAllNodes())
        {
            Console.WriteLine($"- {node.Data.GetDetails()}");
        }

        var firstEdge = graph.GetAllEdges().FirstOrDefault();
        if (firstEdge != null)
        {
            Console.WriteLine($"\n-> First edge: {firstEdge.From.Data.GetDisplayName()} -> {firstEdge.To.Data.GetDisplayName()}");
            Console.WriteLine($"   Edge`s type: {firstEdge.Data.GetDecscription()}");
        }
    }
    private static void HandlePathfinding()
    {
        string startId = ReadString("Start Node ID: ");
        string endId = ReadString("End Node ID: ");
        var startNode = graph.GetNode(startId);
        var endNode = graph.GetNode(endId);

        if (startNode != null && endNode != null)
        {
            Console.WriteLine("Choose algorithm: 1-BFS, 2-DFS, 3-Dijkstra");
            int alg = ReadInt("Choice: ");

            List<NodeType> path = null;

            if (alg == 1) path = GraphAlgorithms.BFS_Research(graph, startNode, n => n == endNode);
            else if (alg == 2) path = GraphAlgorithms.DFS_Research(graph, startNode, n => n == endNode);
            else if (alg == 3) path = GraphAlgorithms.Dijkstra_Search(graph, startNode, endNode);

            if (path != null && path.Count > 0)
            {
                Console.WriteLine("Path found: " + string.Join(" -> ", path.Select(n => n.ID)));
            }
            else Console.WriteLine("Path not found.");
        }
        else Console.WriteLine("Nodes not found.");
    }
    private static void RunCycleCheck()
    {
        Console.WriteLine("\n--- 3. Check for Cycles ---");
        if (GraphAlgorithms.HasCycle(graph))
        {
            Console.WriteLine("[!!!] Graph contains cycles.");
        }
        else
        {
            Console.WriteLine("[OK] Graph do not contains cycles.");
        }
    }
}
