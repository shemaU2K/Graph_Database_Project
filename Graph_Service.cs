using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph_database
{
    using GraphType = Graph<string, Graph_Data, Edge_Data>;
    using NodeType = Node<string, Graph_Data, Edge_Data>;
    using EdgeType = Edge<string, Graph_Data, Edge_Data>;
    public class GraphService
    {
        private GraphType _graph;
        private const string FILE_PATH = "graph_database.json";

        public GraphService()
        {
            _graph = new GraphType();
        }
        public void AddPersonNode(string id, string name, int age)
        {
            var nodeData = new Graph_Data.PersonData { Name = name, Age = age };
            _graph.AddNode(id, nodeData);
        }
        public void AddCityNode(string id, string cityName, int population)
        {
            var nodeData = new Graph_Data.CityData { CityName = cityName, Population = population };
            _graph.AddNode(id, nodeData);
        }
        public void RemoveNode(string id)
        {
            _graph.RemoveNode(id);
        }
        public void RemoveEdge(string fromId, string toId)
        {
            _graph.RemoveEdge(fromId, toId);
        }
        public void ConnectNodes(string fromId, string toId, int weight, string role)
        {
            var fromNode = _graph.GetNode(fromId);
            var toNode = _graph.GetNode(toId);

            if (fromNode != null && toNode != null)
            {
                var edgeData = new Edge_Data.WorksAtEdge { Role = role, Weight = weight };
                _graph.AddEdge(fromId, toId, edgeData);
            }
            else
            {
                throw new Exception("Edge not found!");
            }
        }
        public IEnumerable<NodeType> GetAllNodes()
        {
            return _graph.GetAllNodes();
        }
        public IEnumerable<EdgeType> GetAllEdges()
        {
            return _graph.GetAllEdges();
        }
        public void AddCompanyNode(string id, string companyName, string industry)
        {
            var nodeData = new Graph_Data.CompanyData { CompanyName = companyName, Industry = industry };
            _graph.AddNode(id, nodeData);
        }
        public void Save()
        {
            Graph_SaverToFile.SaveToFile(_graph, FILE_PATH);
        }
        public void Load()
        {
            _graph = Graph_SaverToFile.LoadFromFile<string, Graph_Data, Edge_Data>(FILE_PATH);
        }
        public bool HasCycle()
        {
            return GraphAlgorithms.HasCycle(_graph);
        }
        public List<string> FindShortestPathDijkstra(string startId, string endId)
        {
            var startNode = _graph.GetNode(startId);
            var endNode = _graph.GetNode(endId);
            if (startNode == null || endNode == null) return null;
            var path = GraphAlgorithms.Dijkstra_Search(_graph, startNode, endNode);
            return path?.Select(n => n.ID).ToList();
        }
        public List<string> FindPathBFS(string startId, string endId)
        {
            var startNode = _graph.GetNode(startId);
            var endNode = _graph.GetNode(endId);
            if (startNode == null || endNode == null) return null;
            var path = GraphAlgorithms.BFS_Research(_graph, startNode, n => n == endNode);
            return path?.Select(n => n.ID).ToList();
        }
        public List<string> FindPathDFS(string startId, string endId)
        {
            var startNode = _graph.GetNode(startId);
            var endNode = _graph.GetNode(endId);
            if (startNode == null || endNode == null) return null;
            var path = GraphAlgorithms.DFS_Research(_graph, startNode, n => n == endNode);
            return path?.Select(n => n.ID).ToList();
        }
        public long GetNodeIdAsLong(string id)
        {
            return (long)Math.Abs(id.GetHashCode());
        }
    }
}
