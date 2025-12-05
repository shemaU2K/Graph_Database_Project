using Graph_Database_WPF.ViewModels;
using System.Windows;

namespace Graph_Database_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}