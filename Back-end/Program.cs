using System;
using System.Collections.Generic;
using System.Linq;
using static Graph_Data;


// Alias types for convenience and code brevity
using GraphType = Graph<string, Graph_Data, Edge_Data>;
using NodeType = Node<string, Graph_Data, Edge_Data>;
namespace Graph_database;
/// <summary>
/// The entry point of the Graph Database Console Application.
/// Provides a CLI (Command Line Interface) for interacting with the graph: 
/// creating nodes, connecting them, running algorithms, and saving data.
/// </summary>
class Program
{
    /// <summary>
    /// The global graph instance used throughout the application runtime.
    /// </summary>
    private static GraphType graph = new GraphType();

    /// <summary>
    /// The default file path for saving/loading the database.
    /// </summary>
    private const string FILE_PATH = "graph_database.json";

    /// <summary>
    /// Main entry point. Configures the console and starts the application loop.
    /// </summary>
    /// <param name="args">Command line arguments (not used).</param>
    static void Main(string[] args)
    {
        // Set encoding to support Cyrillic characters if needed
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Graph Database Project (v1.0) ===");

        // Start the interactive menu loop
        RunMenuLoop();
    }

    /// <summary>
    /// Displays the main menu and handles user selection in an infinite loop.
    /// Acts as the main controller for the application logic.
    /// </summary>
    private static void RunMenuLoop()
    {
        while (true)
        {
            Console.WriteLine("\n-------------------------------------------");
            Console.WriteLine("MAIN MENU:");
            Console.WriteLine("1. Add Node");
            Console.WriteLine("2. Add Edge");
            Console.WriteLine("3. Remove Node");
            Console.WriteLine("4. Remove Edge");
            Console.WriteLine("5. Find Path (BFS / DFS / Dijkstra)");
            Console.WriteLine("6. Check for Cycles");
            Console.WriteLine("7. Save to File");
            Console.WriteLine("8. Load from File");
            Console.WriteLine("9. Show All Nodes (Polymorphism Demo)");
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
                    Console.WriteLine("Saving...");
                    Graph_SaverToFile.SaveToFile(graph, FILE_PATH);
                    break;
                case 8:
                    Console.WriteLine("Loading...");
                    graph = Graph_SaverToFile.LoadFromFile<string, Graph_Data, Edge_Data>(FILE_PATH);
                    break;
                case 9: DemonstratePolymorphism(); break;
                case 0: return;
                default: Console.WriteLine("Unknown command."); break;
            }
        }
    }

    // --- HELPER METHODS FOR SAFE INPUT ---

    /// <summary>
    /// Safely reads an integer from the console. 
    /// Prevents crashes by handling invalid format exceptions recursively.
    /// </summary>
    /// <param name="prompt">The text to display to the user.</param>
    /// <returns>A valid integer entered by the user.</returns>
    private static int ReadInt(string prompt)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int result))
        {
            return result;
        }
        Console.WriteLine("[ERROR]: Invalid number format. Please try again.");
        return ReadInt(prompt);
    }

    /// <summary>
    /// Reads a string from the console.
    /// </summary>
    /// <param name="prompt">The text to display to the user.</param>
    /// <returns>The string entered by the user.</returns>
    private static string ReadString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    // --- COMMAND HANDLERS (Private implementations) ---

    private static void HandleAddNode()
    {
        Console.WriteLine("\n--- ADD NODE ---");
        string nodeId = ReadString("Enter Unique ID: ");

        Console.WriteLine("Select Data Type:");
        Console.WriteLine("1: PersonData");
        Console.WriteLine("2: CityData");
        Console.WriteLine("3: CompanyData");

        int choice = ReadInt("Selection: ");
        Graph_Data nodeData = null;

        switch (choice)
        {
            case 1:
                nodeData = new PersonData { Name = ReadString("Name: "), Age = ReadInt("Age: ") };
                break;
            case 2:
                nodeData = new CityData { CityName = ReadString("City Name: "), Population = ReadInt("Population: ") };
                break;
            case 3:
                nodeData = new CompanyData { CompanyName = ReadString("Company Name: "), Industry = ReadString("Industry: ") };
                break;
            default:
                Console.WriteLine("[ERROR]: Invalid type selected.");
                return;
        }

        if (nodeData != null) graph.AddNode(nodeId, nodeData);
    }

    private static void HandleAddEdge()
    {
        Console.WriteLine("\n--- ADD EDGE ---");
        string fromID = ReadString("From Node ID: ");
        string toID = ReadString("To Node ID: ");

        Console.WriteLine("Select Relationship Type:");
        Console.WriteLine("1: Friendship (has Date)");
        Console.WriteLine("2: Work (has Role)");

        int choice = ReadInt("Selection: ");
        Edge_Data edgeData = null;
        double weight = 0;

        switch (choice)
        {
            case 1:
                weight = (double)ReadInt("Weight (cost): ");
                int year = ReadInt("Friends since (Year): ");
                edgeData = new Edge_Data.Edge_Friends
                {
                    Weight = weight,
                    FriendsSince = new DateTime(year, 1, 1)
                };
                break;
            case 2:
                weight = (double)ReadInt("Weight (cost): ");
                string role = ReadString("Role: ");
                edgeData = new Edge_Data.WorksAtEdge
                {
                    Weight = weight,
                    Role = role
                };
                break;
            default:
                Console.WriteLine("[ERROR]: Invalid type.");
                return;
        }

        if (edgeData != null) graph.AddEdge(fromID, toID, edgeData);
    }

    private static void HandleRemoveNode()
    {
        string id = ReadString("Enter Node ID to remove: ");
        graph.RemoveNode(id);
    }

    private static void HandleRemoveEdge()
    {
        string from = ReadString("From ID: ");
        string to = ReadString("To ID: ");
        if (graph.RemoveEdge(from, to)) Console.WriteLine("[SUCCESS]: Edge removed.");
    }

    private static void HandlePathfinding()
    {
        string startId = ReadString("Start Node ID: ");
        string endId = ReadString("End Node ID: ");
        var startNode = graph.GetNode(startId);
        var endNode = graph.GetNode(endId);

        if (startNode == null || endNode == null)
        {
            Console.WriteLine("[ERROR]: Nodes not found.");
            return;
        }

        Console.WriteLine("Algorithm: 1-BFS, 2-DFS, 3-Dijkstra");
        int alg = ReadInt("Selection: ");
        List<NodeType> path = null;

        switch (alg)
        {
            case 1: path = GraphAlgorithms.BFS_Research(graph, startNode, n => n == endNode); break;
            case 2: path = GraphAlgorithms.DFS_Research(graph, startNode, n => n == endNode); break;
            case 3: path = GraphAlgorithms.Dijkstra_Search(graph, startNode, endNode); break;
        }

        if (path != null && path.Count > 0)
        {
            Console.WriteLine("Path found: " + string.Join(" -> ", path.Select(n => n.Data.GetDisplayName())));
        }
        else Console.WriteLine("Path not found.");
    }

    private static void RunCycleCheck()
    {
        bool hasCycle = GraphAlgorithms.HasCycle(graph);
        Console.WriteLine(hasCycle ? "[WARNING]: Graph contains cycles!" : "[INFO]: No cycles detected.");
    }

    private static void DemonstratePolymorphism()
    {
        Console.WriteLine("\n--- Nodes in Graph ---");
        foreach (var node in graph.GetAllNodes())
        {
            Console.WriteLine($"[{node.ID}]: {node.Data.GetDetails()}");
        }
    }
}

