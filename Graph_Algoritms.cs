using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides a static collection of algorithms for graph analysis and traversal.
/// Includes standard pathfinding methods (BFS, DFS, Dijkstra), structural checks (Cycle Detection), and utility methods.
/// </summary>
public static class GraphAlgorithms
{
    /// <summary>
    /// Retrieves all direct neighbors (adjacent nodes) of a specified node based on outgoing edges.
    /// </summary>
    /// <typeparam name="TKey">The type of the node's unique identifier.</typeparam>
    /// <typeparam name="TData">The type of the node's data.</typeparam>
    /// <typeparam name="TEdge_data">The type of the edge's data.</typeparam>
    /// <param name="node">The node to inspect.</param>
    /// <returns>A list of nodes connected via outgoing edges. Returns an empty list if the input node is null.</returns>
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

    /// <summary>
    /// Calculates the out-degree (number of outgoing edges) of a specified node.
    /// </summary>
    /// <param name="node">The node to inspect.</param>
    /// <returns>The count of outgoing edges, or 0 if the node is null.</returns>
    public static int GetOutDegree<TKey, TData, TEdge_data>(
        Node<TKey, TData, TEdge_data> node)
        where TData : Graph_Data
        where TEdge_data : Edge_Data
    {
        return node?.Edges.Count ?? 0;
    }

    /// <summary>
    /// Filters and retrieves nodes from the graph that hold a specific type of data.
    /// Useful for finding all 'Person' nodes or 'City' nodes in a polymorphic graph.
    /// </summary>
    /// <typeparam name="TFindData">The specific data type to search for (e.g., PersonData).</typeparam>
    /// <param name="graph">The graph instance to search.</param>
    /// <returns>A list of nodes containing data of type <typeparamref name="TFindData"/>.</returns>
    public static List<Node<TKey, TData, TEdge_data>> FindNodesByType<TKey, TData, TEdge_data, TFindData>(
        Graph<TKey, TData, TEdge_data> graph)
        where TData : Graph_Data
        where TEdge_data : Edge_Data
        where TFindData : Graph_Data
    {
        var allNodes = graph.GetAllNodes();
        var filteredNodes = allNodes
            .Where(node => node.Data is TFindData)
            .ToList();
        return filteredNodes;
    }

    /// <summary>
    /// Performs a Breadth-First Search (BFS) to find the shortest path in terms of the number of edges (steps).
    /// </summary>
    /// <param name="graph">The graph to traverse.</param>
    /// <param name="startNode">The starting node.</param>
    /// <param name="goalTest">A delegate function to determine if a node is the target.</param>
    /// <returns>A list of nodes representing the path from start to goal. Returns an empty list if no path is found.</returns>
    public static List<Node<Tkey, Tdata, TEdge_data>> BFS_Research<Tkey, Tdata, TEdge_data>(
        Graph<Tkey, Tdata, TEdge_data> graph,
        Node<Tkey, Tdata, TEdge_data> startNode,
        Func<Node<Tkey, Tdata, TEdge_data>, bool> goalTest)
        where Tdata : Graph_Data
        where TEdge_data : Edge_Data
    {
        var visited = new HashSet<Node<Tkey, Tdata, TEdge_data>>();
        var queue = new Queue<Node<Tkey, Tdata, TEdge_data>>();
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

    /// <summary>
    /// Performs a Depth-First Search (DFS) to find a path between nodes.
    /// Explores as far as possible along each branch before backtracking.
    /// </summary>
    /// <param name="graph">The graph to traverse.</param>
    /// <param name="startNode">The starting node.</param>
    /// <param name="goalTest">A delegate function to determine if a node is the target.</param>
    /// <returns>A list of nodes representing a path from start to goal. Returns an empty list if no path is found.</returns>
    static public List<Node<Tkey, Tdata, TEdge_data>> DFS_Research<Tkey, Tdata, TEdge_data>(
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

    /// <summary>
    /// Implements Dijkstra's algorithm to find the optimal path with the lowest total edge weight.
    /// </summary>
    /// <param name="graph">The graph to traverse.</param>
    /// <param name="startNode">The starting node.</param>
    /// <param name="goalNode">The specific target node.</param>
    /// <returns>A list of nodes representing the cheapest path. Returns an empty list if no path is found.</returns>
    /// <remarks>
    /// Requires edges to have a 'Weight' property. Uses a PriorityQueue to efficiently select the next node.
    /// </remarks>
    public static List<Node<Tkey, Tdata, TEdge_data>> Dijkstra_Search<Tkey, Tdata, TEdge_data>(
        Graph<Tkey, Tdata, TEdge_data> graph,
        Node<Tkey, Tdata, TEdge_data> startNode,
        Node<Tkey, Tdata, TEdge_data> goalNode)
        where Tdata : Graph_Data
        where TEdge_data : Edge_Data
    {
        var cost = new Dictionary<Node<Tkey, Tdata, TEdge_data>, double>();
        var priorityQueue = new PriorityQueue<Node<Tkey, Tdata, TEdge_data>, double>();
        var cameFrom = new Dictionary<Node<Tkey, Tdata, TEdge_data>, Node<Tkey, Tdata, TEdge_data>>();
        
        priorityQueue.Enqueue(startNode, 0);
        cost[startNode] = 0;
        
        while (priorityQueue.Count > 0)
        {
            if (!priorityQueue.TryDequeue(out var currentNode, out var currentCost))
            {
                break;
            }
            
            if (goalNode == currentNode)
            {
                break;
            }
            
            foreach (var edge in currentNode.Edges)
            {
                var neighbor = edge.To;
                double newCost = currentCost + edge.Data.Weight;
                double oldCost = cost.ContainsKey(neighbor) ? cost[neighbor] : double.MaxValue;
                
                if (newCost < oldCost)
                {
                    cost[neighbor] = newCost;       
                    cameFrom[neighbor] = currentNode;
                    priorityQueue.Enqueue(neighbor, newCost);
                }
            }
        }
        
        var resultPath = new List<Node<Tkey, Tdata, TEdge_data>>();
        
        // If we haven't reached the goal and it's not the start node
        if (!cameFrom.ContainsKey(goalNode) && startNode != goalNode)
        {
            return resultPath;
        }

        var currentPathNode = goalNode;
        while (currentPathNode != startNode)
        {
            resultPath.Add(currentPathNode);
            currentPathNode = cameFrom[currentPathNode];
        }
        resultPath.Add(startNode);
        resultPath.Reverse();
        return resultPath;
    }

    /// <summary>
    /// Detects if the graph contains any cycles (circular dependencies).
    /// </summary>
    /// <param name="graph">The graph to check.</param>
    /// <returns>True if at least one cycle is detected; otherwise, False.</returns>
    public static bool HasCycle<TKey, TData, TEdgeData>(Graph<TKey, TData, TEdgeData> graph)
        where TData : Graph_Data
        where TEdgeData : Edge_Data
    {
        var visited = new HashSet<Node<TKey, TData, TEdgeData>>();
        var pathSet = new HashSet<Node<TKey, TData, TEdgeData>>();
        
        foreach (var node in graph.GetAllNodes())
        {
            if (!visited.Contains(node))
            {
                if (HasCycleRecursive(node, visited, pathSet))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Recursive helper method for cycle detection using DFS.
    /// </summary>
    private static bool HasCycleRecursive<TKey, TData, TEdgeData>(
        Node<TKey, TData, TEdgeData> node,
        HashSet<Node<TKey, TData, TEdgeData>> visited,
        HashSet<Node<TKey, TData, TEdgeData>> pathSet)
        where TData : Graph_Data
        where TEdgeData : Edge_Data
    {
        // If the node is already in the current recursion stack (pathSet), a cycle exists.
        if (pathSet.Contains(node))
        {
            return true;
        }
        // If the node has been visited and processed in a previous iteration, no cycle here.
        if (visited.Contains(node))
        {
            return false;
        }

        pathSet.Add(node);
        visited.Add(node);

        foreach (var edge in node.Edges)
        {
            var neighbor = edge.To;
            if (HasCycleRecursive(neighbor, visited, pathSet))
            {
                return true;
            }
        }

        // Backtrack: remove node from current recursion path
        pathSet.Remove(node);
        return false;
    }
}
