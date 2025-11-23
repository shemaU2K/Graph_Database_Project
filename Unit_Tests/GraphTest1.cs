using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Edge_Data;
using static Graph_Data;

public class Unit_Tests
{
    [Fact]
    public void AddNode_ShouldIncreaseCount_WhenNodeIsUnique()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        var person = new PersonData { Name = "TestUser", Age = 25 };

        graph.AddNode("id1", person);

        var node = graph.GetNode("id1");
        Assert.NotNull(node);
        Assert.Equal("TestUser", node.Data.GetDisplayName());
    }
    [Fact]
    public void AddEdge_ShouldConnectTwoNodes()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new CityData { CityName = "CityA" });
        graph.AddNode("B", new CityData { CityName = "CityB" });
        var edgeData = new Edge_Data.Edge_Friends { Weight = 1.0 };

        graph.AddEdge("A", "B", edgeData);

        var nodeA = graph.GetNode("A");
        Assert.Single(nodeA.Edges);
        Assert.Equal("B", nodeA.Edges[0].To.ID);
    }
    [Fact]
    public void BFS_ShouldFindShortestPath()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddNode("B", new PersonData());
        graph.AddNode("C", new PersonData());

        graph.AddEdge("A", "B", new Edge_Data.Edge_Friends());
        graph.AddEdge("B", "C", new Edge_Data.Edge_Friends());

        var startNode = graph.GetNode("A");
        var endNode = graph.GetNode("C");
        var path = GraphAlgorithms.BFS_Research(graph, startNode, n => n == endNode);

        Assert.NotNull(path);
        Assert.Equal(3, path.Count);
        Assert.Equal("A", path[0].ID);
        Assert.Equal("C", path[2].ID);
    }
    [Fact]
    public void HasCycle_ShouldReturnTrue_ForCircularGraph()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddNode("B", new PersonData());

        graph.AddEdge("A", "B", new Edge_Data.Edge_Friends());
        graph.AddEdge("B", "A", new Edge_Data.Edge_Friends());

        bool hasCycle = GraphAlgorithms.HasCycle(graph);
        Assert.True(hasCycle);
    }
    [Fact]
    public void AddNode_ShouldNotAddDuplicate()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("1", new PersonData());

        graph.AddNode("1", new PersonData());

        Assert.Equal(1, graph.GetAllNodes().Count());
    }
    [Fact]
    public void RemoveNode_ShouldReturnFalse_IfNodeNotFound()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.RemoveNode("ghost");
        Assert.Null(graph.GetNode("ghost"));
    }
    [Fact]
    public void RemoveNode_ShouldRemoveAssociatedEdges()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddNode("B", new PersonData());
        graph.AddEdge("A", "B", new Edge_Data.Edge_Friends());

        graph.RemoveNode("B");

        var nodeA = graph.GetNode("A");
        Assert.Empty(nodeA.Edges);
    }
    [Fact]
    public void Dijkstra_ShouldChooseCheaperPath()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddNode("B", new PersonData());
        graph.AddNode("C", new PersonData());

        graph.AddEdge("A", "C", new Edge_Data.Edge_Friends { Weight = 10.0 });
        graph.AddEdge("A", "B", new Edge_Data.Edge_Friends { Weight = 1.0 });
        graph.AddEdge("B", "C", new Edge_Data.Edge_Friends { Weight = 1.0 });

        var start = graph.GetNode("A");
        var end = graph.GetNode("C");

        var path = GraphAlgorithms.Dijkstra_Search(graph, start, end);

        Assert.Equal(3, path.Count);
        Assert.Equal("B", path[1].ID);
    }
    [Fact]
    public void HasCycle_ShouldReturnFalse_ForLinearGraph()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddNode("B", new PersonData());
        graph.AddNode("C", new PersonData());
        graph.AddEdge("A", "B", new Edge_Data.Edge_Friends());
        graph.AddEdge("B", "C", new Edge_Data.Edge_Friends());

        Assert.False(GraphAlgorithms.HasCycle(graph));
    }
    [Fact]
    public void Pathfinding_ShouldReturnEmpty_WhenNoPath()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddNode("B", new PersonData());

        var path = GraphAlgorithms.BFS_Research(graph, graph.GetNode("A"), n => n == graph.GetNode("B"));

        Assert.Empty(path);
    }
    [Fact]
    public void Polymorphism_Check()
    {
        var person = new PersonData { Name = "Ivan", Age = 20 };
        var city = new CityData { CityName = "Lviv" };

        Assert.Contains("Person", person.GetDetails());
        Assert.Contains("City", city.GetDetails());
    }
    [Fact]
    public void DFS_ShouldFindDeepPath()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddNode("B", new PersonData());

        graph.AddEdge("A", "B", new Edge_Data.Edge_Friends());

        var path = GraphAlgorithms.DFS_Research(graph, graph.GetNode("A"), n => n == graph.GetNode("B"));

        Assert.NotEmpty(path);
        Assert.Equal("B", path.Last().ID);
    }
    [Fact]
    public void BFS_ShouldReturnEmpty_WhenNoPathExists()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddNode("B", new PersonData());
        graph.AddNode("C", new PersonData());

        graph.AddEdge("A", "B", new Edge_Data.Edge_Friends());

        var start = graph.GetNode("A");
        var end = graph.GetNode("C");

        var path = GraphAlgorithms.BFS_Research(graph, start, n => n == end);

        Assert.Empty(path);
    }
    [Fact]
    public void HasCycle_ShouldDetectSelfLoop()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddEdge("A", "A", new Edge_Data.Edge_Friends());

        bool hasCycle = GraphAlgorithms.HasCycle(graph);

        Assert.True(hasCycle);
    }
    [Fact]
    public void Serialization_ShouldSaveAndLoadGraph()
    {
        var originalGraph = new Graph<string, Graph_Data, Edge_Data>();
        originalGraph.AddNode("Test1", new PersonData { Name = "Alex" });
        originalGraph.AddNode("Test2", new CityData { CityName = "Kyiv" });
        originalGraph.AddEdge("Test1", "Test2", new Edge_Data.Edge_Friends());

        string tempFile = "test_graph.json";

        Graph_SaverToFile.SaveToFile(originalGraph, tempFile);
        var loadedGraph = Graph_SaverToFile.LoadFromFile<string, Graph_Data, Edge_Data>(tempFile);

        Assert.NotNull(loadedGraph);
        Assert.NotNull(loadedGraph.GetNode("Test1"));
        Assert.NotNull(loadedGraph.GetNode("Test2"));
        Assert.Single(loadedGraph.GetNode("Test1").Edges);

        if (File.Exists(tempFile)) File.Delete(tempFile);
    }
    [Fact]
    public void RemoveEdge_ShouldReturnFalse_WhenEdgeDoesNotExist()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddNode("B", new PersonData());

        bool result = graph.RemoveEdge("A", "B");

        Assert.False(result);
    }
    [Fact]
    public void AddEdge_ShouldDoNothing_WhenNodesNotFound()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();

        graph.AddEdge("Ghost1", "Ghost2", new Edge_Data.Edge_Friends());

        Assert.Empty(graph.GetAllEdges());
    }
    [Fact]
    public void Graph_ShouldPreserveConcreteDataTypes()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("Lviv", new CityData { CityName = "Lviv", Population = 720000 });

        var node = graph.GetNode("Lviv");

        Assert.IsType<CityData>(node.Data);
        var city = node.Data as CityData;
        Assert.Equal(720000, city.Population);
    }
    [Fact]
    public void Algorithms_ShouldHandleEmptyGraphSafe()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();

        var hasCycle = GraphAlgorithms.HasCycle(graph);
        Assert.False(hasCycle);
    }
    [Fact]
    public void EdgeData_ShouldBeMutable()
    {
        var graph = new Graph<string, Graph_Data, Edge_Data>();
        graph.AddNode("A", new PersonData());
        graph.AddNode("B", new PersonData());

        var edgeData = new WorksAtEdge { Weight = 5.0 };
        graph.AddEdge("A", "B", edgeData);

        var edge = graph.GetAllEdges().First();
        edge.Data.Weight = 10.0;

        Assert.Equal(10.0, graph.GetAllEdges().First().Data.Weight);
    }
}
