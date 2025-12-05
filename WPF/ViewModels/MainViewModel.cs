using Graph_database;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using GraphX.Common.Enums;
using GraphX.Logic.Models;
using QuickGraph;

namespace Graph_Database_WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // --- Input Properties (Auto-properties are okay usually, but specific ones need notification) ---
        public string NewNodeId { get; set; }
        private DataGraph _graphToVisualize;
        public DataGraph GraphToVisualize
        {
            get => _graphToVisualize;
            set
            {
                _graphToVisualize = value;
                OnPropertyChanged(nameof(GraphToVisualize));
            }
        }
        public GXLogicCore LogicCore { get; set; }

        private Dictionary<string, Point> _cachedPositions = new Dictionary<string, Point>();
        private Random _rnd = new Random();
        // Person
        public string PersonName { get; set; }
        public int PersonAge { get; set; }

        // City
        public string CityName { get; set; }
        public int CityPopulation { get; set; }

        // Company
        public string CompanyName { get; set; }
        public string CompanyIndustry { get; set; }

        // Edge
        public string SourceNodeId { get; set; }
        public string TargetNodeId { get; set; }
        public int EdgeWeight { get; set; }
        public string EdgeRole { get; set; }

        // --- Status Message для XAML ---
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        // --- Collections for UI ---
        public ObservableCollection<string> NodesList { get; set; }
        public ObservableCollection<string> EdgesList { get; set; }

        private readonly GraphService _graphService;

        // --- ALGORITHMS PROPERTIES ---
        public string AlgoStartNodeId { get; set; }
        public string AlgoEndNodeId { get; set; }

        private string _algoResult;
        public string AlgoResult
        {
            get => _algoResult;
            set
            {
                _algoResult = value;
                OnPropertyChanged(nameof(AlgoResult));
            }
        }

        // --- Commands ---
        public RelayCommand AddPersonCommand { get; set; }
        public RelayCommand AddCityCommand { get; set; }
        public RelayCommand AddCompanyCommand { get; set; }
        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand RemoveNodeCommand { get; set; }
        public RelayCommand RemoveEdgeCommand { get; set; }
        public RelayCommand CheckCycleCommand { get; set; } 
        public RelayCommand RunDijkstraCommand { get; set; }
        public RelayCommand RunBFSCommand { get; set; }
        public RelayCommand RunDFSCommand { get; set; }

        public MainViewModel()
        {
            _graphService = new GraphService();
            NodesList = new ObservableCollection<string>();
            EdgesList = new ObservableCollection<string>();


            // --- COMMANDS IMPLEMENTATION ---

            AddPersonCommand = new RelayCommand(obj =>
            {
                try
                {
                    _graphService.AddPersonNode(NewNodeId, PersonName, PersonAge);
                    RefreshLists();
                    StatusMessage = $"Success: Person '{PersonName}' added (ID: {NewNodeId})";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"ERROR adding Person: {ex.Message}";
                }
            });

            AddCityCommand = new RelayCommand(obj =>
            {
                try
                {
                    _graphService.AddCityNode(NewNodeId, CityName, CityPopulation);
                    RefreshLists();
                    StatusMessage = $"Success: City '{CityName}' added (ID: {NewNodeId})";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"ERROR adding City: {ex.Message}";
                }
            });

            AddCompanyCommand = new RelayCommand(obj =>
            {
                try
                {
                    _graphService.AddCompanyNode(NewNodeId, CompanyName, CompanyIndustry);
                    RefreshLists();
                    StatusMessage = $"Success: Company '{CompanyName}' added (ID: {NewNodeId})";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"ERROR adding Company: {ex.Message}";
                }
            });

            RemoveNodeCommand = new RelayCommand(obj =>
            {
                try
                {
                    _graphService.RemoveNode(NewNodeId);
                    RefreshLists();
                    StatusMessage = $"Node {NewNodeId} removed.";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error removing node: {ex.Message}";
                }
            });

            RemoveEdgeCommand = new RelayCommand(obj =>
            {
                try
                {
                    _graphService.RemoveEdge(SourceNodeId, TargetNodeId);
                    RefreshLists();
                    StatusMessage = $"Edge removed: {SourceNodeId} -> {TargetNodeId}";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error removing edge: {ex.Message}";
                }
            });

            ConnectCommand = new RelayCommand(obj =>
            {
                try
                {
                    _graphService.ConnectNodes(SourceNodeId, TargetNodeId, EdgeWeight, EdgeRole);
                    if (_graphService.HasCycle())
                    {
                        StatusMessage = $"WARNING: Cycle detected after connecting {SourceNodeId} to {TargetNodeId}!";
                        MessageBox.Show("Cycle detected!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        StatusMessage = $"Edge created: {SourceNodeId} -> {TargetNodeId}";
                    }

                    RefreshLists();
                }
                catch (Exception ex)
                {
                    StatusMessage = $"ERROR connecting: {ex.Message}";
                }
            });

            SaveCommand = new RelayCommand(obj =>
            {
                try
                {
                    _graphService.Save();
                    StatusMessage = "Graph saved successfully to file.";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"ERROR saving: {ex.Message}";
                }
            });

            LoadCommand = new RelayCommand(obj =>
            {
                try
                {
                    _graphService.Load();
                    RefreshLists();
                    StatusMessage = "Graph loaded successfully.";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"ERROR loading: {ex.Message}";
                }
            });

            RunDijkstraCommand = new RelayCommand(obj =>
                ExecuteAlgorithm(_graphService.FindShortestPathDijkstra, "Dijkstra"));

            RunBFSCommand = new RelayCommand(obj =>
                ExecuteAlgorithm(_graphService.FindPathBFS, "BFS"));

            RunDFSCommand = new RelayCommand(obj =>
                ExecuteAlgorithm(_graphService.FindPathDFS, "DFS"));
            _graphService = new GraphService();

            NodesList = new ObservableCollection<string>();
            EdgesList = new ObservableCollection<string>();

            LogicCore = new GXLogicCore();
            LogicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK;
            LogicCore.DefaultLayoutAlgorithmParams = LogicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);

            ((GraphX.Logic.Algorithms.LayoutAlgorithms.KKLayoutParameters)LogicCore.DefaultLayoutAlgorithmParams).K = 40;

            LogicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            LogicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            LogicCore.AsyncAlgorithmCompute = false;
        }

        // --- HELPER METHODS ---

        private void RefreshLists()
        {
            NodesList.Clear();
            EdgesList.Clear();
            var allNodes = _graphService.GetAllNodes();
            var allEdges = _graphService.GetAllEdges();

            NodesList.Add("=== NODES ===");
            foreach (var node in allNodes)
            {
                NodesList.Add($"[{node.ID}] {node.Data}");
            }

            NodesList.Add("");
            NodesList.Add("=== EDGES ===");
            foreach (var edge in allEdges)
            {
                string edgeInfo = $"{edge.From.ID} --[{edge.Data}]--> {edge.To.ID}";
                EdgesList.Add(edgeInfo);
            }

            var newGraph = new DataGraph();
            var vertexLookup = new Dictionary<string, DataVertex>();
            foreach (var node in allNodes)
            {
                var v = new DataVertex
                {
                    ID = _graphService.GetNodeIdAsLong(node.ID),
                    Text = node.ID
                };

                newGraph.AddVertex(v);
                vertexLookup[node.ID] = v;
            }

            foreach (var edge in allEdges)
            {
                if (vertexLookup.ContainsKey(edge.From.ID) && vertexLookup.ContainsKey(edge.To.ID))
                {
                    var sourceV = vertexLookup[edge.From.ID];
                    var targetV = vertexLookup[edge.To.ID];

                    var e = new DataEdge(sourceV, targetV)
                    {
                        Text = edge.Data.ToString()
                    };
                    newGraph.AddEdge(e);
                }
            }

            GraphToVisualize = newGraph;
        }
        private void ExecuteAlgorithm(Func<string, string, List<string>> algorithmMethod, string algoName)
        {
            try
            {
                StatusMessage = $"Running {algoName}...";
                AlgoResult = "Calculating...";

                if (string.IsNullOrWhiteSpace(AlgoStartNodeId) || string.IsNullOrWhiteSpace(AlgoEndNodeId))
                {
                    StatusMessage = "Please enter both Start and End Node IDs.";
                    AlgoResult = "Input Error: IDs are missing.";
                    return;
                }

                var path = algorithmMethod(AlgoStartNodeId, AlgoEndNodeId);

                if (path == null || path.Count == 0)
                {
                    AlgoResult = $"No path found using {algoName}.";
                    StatusMessage = $"{algoName} finished: Target unreachable.";
                }
                else
                {
                    AlgoResult = string.Join(" -> ", path);
                    StatusMessage = $"{algoName} Success! Path length: {path.Count - 1}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"{algoName} Error: {ex.Message}";
                AlgoResult = $"Error: {ex.Message}";
            }
        }
    }
}