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
        Console.WriteLine("=== Graph Database Project (v1.0) ===");
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

            int choice = ReadInt("> Ваш вибір: ");

            switch (choice)
            {
                case 1: HandleAddNode(); break;
                case 2: HandleAddEdge(); break;
                case 3: HandleRemoveNode(); break;
                case 4: HandleRemoveEdge(); break;
                case 5: HandlePathfinding(); break; // <-- Треба дописати цей метод виклику алгоритмів
                case 6:
                    bool hasCycle = GraphAlgorithms.HasCycle(graph);
                    Console.WriteLine(hasCycle ? "[!!!] Graph has cycle!" : "[OK] Cycles not found.");
                    break;
                case 7:
                    Graph_SaverToFile.SaveToFile(graph, FILE_PATH);
                    break;
                case 8:
                    graph = Graph_SaverToFile.LoadFromFile<string, Graph_Data, Edge_Data>(FILE_PATH);
                    break;
                case 9:
                    foreach (var node in graph.GetAllNodes())
                        Console.WriteLine(node.Data.GetDetails());
                    break;
                case 0: return;
                default: Console.WriteLine("Uknown command."); break;
            }
        }
    }
    private static void ShowPath(List<NodeType> path, string algorithmName)
    {
        Console.WriteLine($"\n--- Результат: {algorithmName} ---");
        if (path == null || path.Count == 0)
        {
            Console.WriteLine("Шлях не знайдено, або граф містить цикли.");
            return;
        }

        Console.WriteLine($"Знайдено шлях (довжина: {path.Count - 1}):");
        Console.WriteLine(string.Join(" -> ", path.Select(n => n.Data.GetDisplayName())));
    }
    private static int ReadInt(string prompt)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int result))
        {
            return result;
        }
        Console.WriteLine("[ПОМИЛКА]: Невірний формат числа. Спробуйте ще раз.");
        return ReadInt(prompt);
    }
    private static string ReadString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
    private static void HandleAddNode()
    {
        Console.WriteLine("\n--- ДОДАВАННЯ ВУЗЛА ---");
        string nodeID = ReadString("Введіть унікальний ID вузла: ");

        Console.WriteLine("Оберіть тип даних:");
        Console.WriteLine("1: PersonData (Людина)");
        Console.WriteLine("2: CityData (Місто)");
        Console.WriteLine("3: CompanyData (Компанія)");

        int choice = ReadInt("Вибір: ");
        Graph_Data nodeData = null;
        switch (choice)
        {
            case 1:
                string name = ReadString("Ім'я: ");
                int age = ReadInt("Вік: ");
                nodeData = new Graph_Data.PersonData { Name = name, Age = age };
                break;
            case 2:
                string city = ReadString("Назва Міста: ");
                int population = ReadInt("Населення: ");
                nodeData = new Graph_Data.CityData { CityName = city, Population = population };
                break;

            case 3:
                string company = ReadString("Назва Компанії: ");
                string industry = ReadString("Галузь: ");
                nodeData = new Graph_Data.CompanyData { CompanyName = company, Industry = industry };
                break;

            default:
                Console.WriteLine("[ПОМИЛКА]: Невірний тип вузла. Додавання скасовано.");
                return;
        }
        if (nodeData != null)
        {
            graph.AddNode(nodeID, nodeData);
            Console.WriteLine($"[УСПІХ]: Вузол '{nodeData.GetDisplayName()}' ({nodeID}) додано.");
        }
    }
    private static void HandleAddEdge()
    {
        Console.WriteLine("\n--- ДОДАВАННЯ ЗВ'ЯЗКУ ---");
        var nodeFromID = ReadString("Введіть ID початкового вузла: ");
        var nodeToID = ReadString("Введіть ID кінцевого вузла: ");

        Console.WriteLine("Оберіть тип даних:");
        Console.WriteLine("1: Friends since");
        Console.WriteLine("2: Work as");

        int choice = ReadInt("Вибір: ");
        Edge_Data edgeData = null;
        switch (choice)
        {
            case 1:
                int year = ReadInt("Рік початку дружби: ");
                var since = new DateTime(year, 1, 1);
                var weigth = ReadInt("Вага ребра (число): ");
                edgeData = new Edge_Data.Edge_Friends { FriendsSince = since, Weight = weigth };
                break;
            case 2:
                string role = ReadString("Роль: ");
                var weight = ReadInt("Вага ребра (число): ");
                edgeData = new Edge_Data.WorksAtEdge { Role = role, Weight = weight };
                break;
            default:
                Console.WriteLine("[ПОМИЛКА]: Невірний тип зв'язку. Додавання скасовано.");
                return;
        }
        if (edgeData != null)
        {
            var nodeFrom = graph.GetNode(nodeFromID);
            var nodeTo = graph.GetNode(nodeToID);

            if (nodeFrom != null && nodeTo != null)
            {
                graph.AddEdge(nodeFromID, nodeToID, edgeData);
                Console.WriteLine($"[УСПІХ]: Зв'язок {edgeData.GetDecscription()} додано.");
            }
            else
            {
                Console.WriteLine("[ПОМИЛКА]: Один або обидва вузли не знайдено.");
            }
        }
    }
    static void HandleRemoveNode()
    {
        Console.WriteLine("\n--- ВИДАЛЕННЯ ВУЗЛА ---");
        string nodeID = ReadString("Введіть ID вузла для видалення: ");
        graph.RemoveNode(nodeID);
    }
    static void HandleRemoveEdge()
    {
        Console.WriteLine("\n--- ВИДАЛЕННЯ ЗВ'ЯЗКУ ---");
        string fromID = ReadString("Введіть ID початкового вузла: ");
        string toID = ReadString("Введіть ID кінцевого вузла: ");
        if (graph.RemoveEdge(fromID, toID))
        {
            Console.WriteLine("[УСПІХ]: Зв'язок видалено.");
        }
        else
        {
            Console.WriteLine("[ПОМИЛКА]: Зв'язок або вузли не знайдено.");
        }
    }
    private static void DemonstratePolymorphism()
    {
        Console.WriteLine("\n--- 1. Демонстрація Динамічного Поліморфізму (GetDetails) ---");
        foreach (var node in graph.GetAllNodes())
        {
            Console.WriteLine($"- {node.Data.GetDetails()}");
        }

        var firstEdge = graph.GetAllEdges().FirstOrDefault();
        if (firstEdge != null)
        {
            Console.WriteLine($"\n-> Перше ребро: {firstEdge.From.Data.GetDisplayName()} -> {firstEdge.To.Data.GetDisplayName()}");
            Console.WriteLine($"   Тип ребра: {firstEdge.Data.GetDecscription()}");
        }
    }
    private static void RunPathfinding()
    {
        Console.WriteLine("\n--- 2. Пошук Шляху (Start: Іван, End: Київ) ---");
        var start = graph.GetNode("p1");
        var end = graph.GetNode("c2");

        if (start == null || end == null)
        {
            Console.WriteLine("Помилка: Початковий/кінцевий вузол не знайдено.");
            return;
        }

        var pathBfs = GraphAlgorithms.BFS_Research(graph, start, n => n == end);
        ShowPath(pathBfs, "BFS (Кількість кроків)");

        var pathDfs = GraphAlgorithms.DFS_Research(graph, start, n => n == end);
        ShowPath(pathDfs, "DFS (Перший знайдений)");

        var pathDijkstra = GraphAlgorithms.Dijkstra_Search(graph, start, end);
        ShowPath(pathDijkstra, "Дейкстра (Найменша вага)");
    }
    private static void HandlePathfinding()
    {
        string startId = ReadString("Start Node ID: ");
        string endId = ReadString("End Node ID: ");
        var startNode = graph.GetNode(startId);
        var endNode = graph.GetNode(endId);

        if (startNode != null && endNode != null)
        {
            Console.WriteLine("Оберіть алгоритм: 1-BFS, 2-DFS, 3-Dijkstra");
            int alg = ReadInt("Вибір: ");

            List<NodeType> path = null;

            if (alg == 1) path = GraphAlgorithms.BFS_Research(graph, startNode, n => n == endNode);
            else if (alg == 2) path = GraphAlgorithms.DFS_Research(graph, startNode, n => n == endNode);
            else if (alg == 3) path = GraphAlgorithms.Dijkstra_Search(graph, startNode, endNode);

            if (path != null && path.Count > 0)
            {
                Console.WriteLine("Шлях знайдено: " + string.Join(" -> ", path.Select(n => n.ID)));
            }
            else Console.WriteLine("Шлях не знайдено.");
        }
        else Console.WriteLine("Вузли не знайдено.");
    }
    private static void RunCycleCheck()
    {
        Console.WriteLine("\n--- 3. Перевірка на Цикли ---");
        if (GraphAlgorithms.HasCycle(graph))
        {
            Console.WriteLine("[!!!] Граф МІСТИТЬ цикл.");
        }
        else
        {
            Console.WriteLine("[OK] Граф НЕ містить циклів.");
        }
    }
}
