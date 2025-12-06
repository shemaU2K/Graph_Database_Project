using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System;
using System.Windows.Media;
using Graph_database;

namespace Graph_Database_WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly GraphService _graphService;
        private ConnectorViewModel _pendingSource;
        private Random _random = new Random();

        private ObservableCollection<NodeViewModel> _nodes = new();
        public ObservableCollection<NodeViewModel> Nodes { get => _nodes; set => SetProperty(ref _nodes, value); }

        private ObservableCollection<ConnectionViewModel> _connections = new();
        public ObservableCollection<ConnectionViewModel> Connections { get => _connections; set => SetProperty(ref _connections, value); }

        private string _startNodeId = "";
        public string StartNodeId { get => _startNodeId; set => SetProperty(ref _startNodeId, value); }

        private string _endNodeId = "";
        public string EndNodeId { get => _endNodeId; set => SetProperty(ref _endNodeId, value); }

        private string _algorithmResult = "Ready";
        public string AlgorithmResult { get => _algorithmResult; set => SetProperty(ref _algorithmResult, value); }

        private string _newNodeId = "";
        public string NewNodeId { get => _newNodeId; set => SetProperty(ref _newNodeId, value); }

        private string _newNodeName = "";
        public string NewNodeName { get => _newNodeName; set => SetProperty(ref _newNodeName, value); }

        private string _newNodeExtra = "";
        public string NewNodeExtra { get => _newNodeExtra; set => SetProperty(ref _newNodeExtra, value); }

        private string _removeNodeId = "";
        public string RemoveNodeId { get => _removeNodeId; set => SetProperty(ref _removeNodeId, value); }

        private string _connSourceId = "";
        public string ConnSourceId { get => _connSourceId; set => SetProperty(ref _connSourceId, value); }

        [ObservableProperty]
        private string _statusMessage = "Ready";

        private string _connTargetId = "";
        public string ConnTargetId { get => _connTargetId; set => SetProperty(ref _connTargetId, value); }

        private int _defaultWeight = 1;
        public int DefaultWeight { get => _defaultWeight; set => SetProperty(ref _defaultWeight, value); }

        private string _defaultRole = "connected";
        public string DefaultRole { get => _defaultRole; set => SetProperty(ref _defaultRole, value); }
        public MainViewModel()
        {

            _graphService = new GraphService();
            try { _graphService.Load(); } catch { }
            LoadGraphToUI();
        }
        private void LoadGraphToUI()
        {
            Nodes.Clear();
            Connections.Clear();

            foreach (var node in _graphService.GetAllNodes())
            {
                string title = node.ID;
                Brush color = Brushes.Gray;

                if (node.Data is Graph_Data.PersonData p) { title = p.Name; color = Brushes.DodgerBlue; }
                else if (node.Data is Graph_Data.CityData c) { title = c.CityName; color = Brushes.Orange; }
                else if (node.Data is Graph_Data.CompanyData comp) { title = comp.CompanyName; color = Brushes.SpringGreen; }

                AddNodeToCanvas(node.ID, title, color, GetRandomLocation());
            }

            foreach (var edge in _graphService.GetAllEdges())
            {
                ConnectNodesOnCanvas(edge.From.ID, edge.To.ID);
            }
        }
        private Point GetRandomLocation() => new Point(_random.Next(50, 600), _random.Next(50, 500));
        private void AddNodeToCanvas(string id, string title, Brush color, Point location)
        {
            var uiNode = new NodeViewModel
            {
                Id = id,
                Title = title,
                Location = location,
                Color = color
            };
            uiNode.Inputs.Add(new ConnectorViewModel { Title = "In", ParentNode = uiNode });
            uiNode.Outputs.Add(new ConnectorViewModel { Title = "Out", ParentNode = uiNode });
            Nodes.Add(uiNode);
        }
        private void ConnectNodesOnCanvas(string sourceId, string targetId)
        {
            var sourceNode = Nodes.FirstOrDefault(n => n.Id == sourceId);
            var targetNode = Nodes.FirstOrDefault(n => n.Id == targetId);

            if (sourceNode != null && targetNode != null)
            {
                if (!Connections.Any(c => c.Source.ParentNode.Id == sourceId && c.Target.ParentNode.Id == targetId))
                {
                    Connections.Add(new ConnectionViewModel
                    {
                        Source = sourceNode.Outputs.FirstOrDefault(),
                        Target = targetNode.Inputs.FirstOrDefault()
                    });
                }
            }
        }



        [RelayCommand]
        private void SaveToFile()
        {
            try
            {
                _graphService.Save();
                StatusMessage = "Success: Graph saved to file!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving: {ex.Message}";
                MessageBox.Show(ex.Message);
            }
        }
        [RelayCommand]
        private void LoadFromFile()
        {
            try
            {
                _graphService.Load();
                LoadGraphToUI();

                StatusMessage = "Success: Graph loaded from file!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading: {ex.Message}";
                MessageBox.Show(ex.Message);
            }
        }
        [RelayCommand]
        private void OnConnectionStarted(ConnectorViewModel source)
        {
            _pendingSource = source;
        }
        [RelayCommand]
        private void OnConnectionCompleted(object result)
        {
            ConnectorViewModel target = null;

            if (result is System.ValueTuple<object, object> tuple)
            {
                target = tuple.Item2 as ConnectorViewModel;
            }

            else if (result is ConnectorViewModel c)
            {
                target = c;
            }

            if (_pendingSource != null && target != null && _pendingSource.ParentNode != target.ParentNode)
            {
                try
                {

                    _graphService.ConnectNodes(
                        _pendingSource.ParentNode.Id,
                        target.ParentNode.Id,
                        DefaultWeight,
                        DefaultRole
                    );

                    if (!Connections.Any(c => c.Source == _pendingSource && c.Target == target))
                    {
                        Connections.Add(new ConnectionViewModel
                        {
                            Source = _pendingSource,
                            Target = target
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error connecting: {ex.Message}");
                }
            }

            _pendingSource = null;
        }
        [RelayCommand]
        private void DeleteConnection(ConnectionViewModel connection)
        {
            if (connection == null) return;

            try
            {
                _graphService.RemoveEdge(connection.Source.ParentNode.Id, connection.Target.ParentNode.Id);
                Connections.Remove(connection);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error removing edge: " + ex.Message);
            }
        }
        [RelayCommand]
        private void AddPersonNode()
        {
            if (string.IsNullOrEmpty(NewNodeId)) NewNodeId = Guid.NewGuid().ToString().Substring(0, 5);
            try
            {
                _graphService.AddPersonNode(NewNodeId, NewNodeName, 25);
                var newNode = new NodeViewModel
                {
                    Id = NewNodeId,
                    Title = string.IsNullOrEmpty(NewNodeName) ? NewNodeId : NewNodeName,
                    Location = new Point(100, 100),
                    Color = Brushes.DodgerBlue
                };
                newNode.Inputs.Add(new ConnectorViewModel { Title = "In", ParentNode = newNode });
                newNode.Outputs.Add(new ConnectorViewModel { Title = "Out", ParentNode = newNode });
                Nodes.Add(newNode);
                StatusMessage = $"Added Person: {NewNodeName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                MessageBox.Show(ex.Message);
            }
        }
        [RelayCommand]
        private void AddCityNode()
        {
            if (string.IsNullOrEmpty(NewNodeId)) NewNodeId = Guid.NewGuid().ToString().Substring(0, 5);
            try
            {
                _graphService.AddCityNode(NewNodeId, NewNodeName, 100000);
                var newNode = new NodeViewModel
                {
                    Id = NewNodeId,
                    Title = string.IsNullOrEmpty(NewNodeName) ? NewNodeId : NewNodeName,
                    Location = new Point(100, 100),
                    Color = Brushes.DodgerBlue
                };
                newNode.Inputs.Add(new ConnectorViewModel { Title = "In", ParentNode = newNode });
                newNode.Outputs.Add(new ConnectorViewModel { Title = "Out", ParentNode = newNode });
                Nodes.Add(newNode);
                StatusMessage = $"Added City: {NewNodeName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                MessageBox.Show(ex.Message);
            }
        }
        [RelayCommand]
        private void AddCompanyNode()
        {
            if (string.IsNullOrEmpty(NewNodeId)) NewNodeId = Guid.NewGuid().ToString().Substring(0, 5);
            try
            {
                _graphService.AddCompanyNode(NewNodeId, NewNodeName, DefaultRole);
                var newNode = new NodeViewModel
                {
                    Id = NewNodeId,
                    Title = string.IsNullOrEmpty(NewNodeName) ? NewNodeId : NewNodeName,
                    Location = new Point(100, 100),
                    Color = Brushes.DodgerBlue
                };
                newNode.Inputs.Add(new ConnectorViewModel { Title = "In", ParentNode = newNode });
                newNode.Outputs.Add(new ConnectorViewModel { Title = "Out", ParentNode = newNode });
                Nodes.Add(newNode);
                StatusMessage = $"Added Person: {NewNodeName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                MessageBox.Show(ex.Message);
            }
        }
        [RelayCommand]
        private void AddConnectionManual()
        {
        
            string sId = ConnSourceId?.Trim();
            string tId = ConnTargetId?.Trim();

            if (string.IsNullOrEmpty(sId) || string.IsNullOrEmpty(tId))
            {
                MessageBox.Show("Enter both IDs"); return;
            }

            var src = Nodes.FirstOrDefault(n => n.Id == sId);
            var trg = Nodes.FirstOrDefault(n => n.Id == tId);

            if (src == null || trg == null)
            {
                MessageBox.Show($"Node not found! Source exists: {src != null}, Target exists: {trg != null}");
                return;
            }

            ConnectNodesOnCanvas(sId, tId);
            try
            {
                _graphService.ConnectNodes(sId, tId, DefaultWeight, DefaultRole);
                _graphService.Save();
                ConnSourceId = ""; ConnTargetId = "";
            }
            catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
        }
        [RelayCommand]
        private void RemoveNode()
        {
            if (string.IsNullOrEmpty(RemoveNodeId)) return;
            _graphService.RemoveNode(RemoveNodeId); _graphService.Save();
            var n = Nodes.FirstOrDefault(x => x.Id == RemoveNodeId);
            if (n != null)
            {
                var edges = Connections.Where(c => c.Source.ParentNode == n || c.Target.ParentNode == n).ToList();
                foreach (var e in edges) Connections.Remove(e);
                Nodes.Remove(n);
            }
        }
        [RelayCommand] private void CheckCycle() { AlgorithmResult = _graphService.HasCycle() ? "Cycle Detected!" : "No Cycles."; }
        [RelayCommand]
        private void RemoveEdge()
        {
            if (string.IsNullOrEmpty(ConnSourceId) || string.IsNullOrEmpty(ConnTargetId)) return;
            try
            {
                _graphService.RemoveEdge(ConnSourceId, ConnTargetId);
                _graphService.Save();

                var conn = Connections.FirstOrDefault(c =>
                    c.Source.ParentNode.Id == ConnSourceId &&
                    c.Target.ParentNode.Id == ConnTargetId);

                if (conn != null) Connections.Remove(conn);

                ConnSourceId = "";
                ConnTargetId = "";
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
        [RelayCommand]
        private void RunDijkstra()
        {
            if (string.IsNullOrEmpty(StartNodeId) || string.IsNullOrEmpty(EndNodeId))
            {
                AlgorithmResult = "Error: Please enter Start and End IDs";
                return;
            }
            try
            {
                var path = _graphService.FindShortestPathDijkstra(StartNodeId, EndNodeId);

                if (path == null || path.Count == 0)
                {
                    AlgorithmResult = "No Path Found.";
                    return;
                }
                AlgorithmResult = "Path: " + string.Join(" -> ", path);
            }
            catch (Exception ex) { AlgorithmResult = "Error: " + ex.Message; }
        }
        [RelayCommand]
        private void RunBFS()
        {
            if (string.IsNullOrEmpty(StartNodeId) || string.IsNullOrEmpty(EndNodeId)) return;
            try
            {
                var path = _graphService.FindPathBFS(StartNodeId, EndNodeId);

                if (path == null || path.Count == 0) { AlgorithmResult = "No Path Found."; return; }
                AlgorithmResult = "Path: " + string.Join(" -> ", path);
            }
            catch (Exception ex) { AlgorithmResult = "Error: " + ex.Message; }
        }
        [RelayCommand]
        private void RunDFS()
        {
            if (string.IsNullOrEmpty(StartNodeId) || string.IsNullOrEmpty(EndNodeId)) return;
            try
            {
                var path = _graphService.FindPathDFS(StartNodeId, EndNodeId);

                if (path == null || path.Count == 0) { AlgorithmResult = "No Path Found."; return; }
                AlgorithmResult = "Path: " + string.Join(" -> ", path);
            }
            catch (Exception ex) { AlgorithmResult = "Error: " + ex.Message; }
        }
    }
}

