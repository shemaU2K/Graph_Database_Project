using System;

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

    public static List<Node<Tkey, Tdata, TEdge_data>> BFS_Reserch<Tkey, Tdata, TEdge_data>(
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
