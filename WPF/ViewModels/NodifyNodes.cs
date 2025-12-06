using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Graph_Database_WPF.ViewModels
{
    public partial class ConnectorViewModel : ObservableObject
    {
        [ObservableProperty] private string _title;
        [ObservableProperty] private Point _anchor;
        public NodeViewModel ParentNode { get; set; }
    }

    public partial class NodeViewModel : ObservableObject
    {      
        [ObservableProperty] private Point _location;
        [ObservableProperty] private string _id;
        [ObservableProperty] private string _title;
        [ObservableProperty] private Brush _color;

        public ObservableCollection<ConnectorViewModel> Inputs { get; } = new();
        public ObservableCollection<ConnectorViewModel> Outputs { get; } = new();
    }

    public partial class ConnectionViewModel : ObservableObject
    {
        [ObservableProperty] private ConnectorViewModel _source;
        [ObservableProperty] private ConnectorViewModel _target;
    }
}
