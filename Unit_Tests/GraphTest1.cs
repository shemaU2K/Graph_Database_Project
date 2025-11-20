using System.Collections.Generic;
using System.Linq;
using Xunit; 
using static Graph_Data;
 

public class GraphTests
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
}