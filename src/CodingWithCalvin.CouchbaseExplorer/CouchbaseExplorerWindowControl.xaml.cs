using System.Windows.Controls;
using CodingWithCalvin.CouchbaseExplorer.ViewModels;

namespace CodingWithCalvin.CouchbaseExplorer
{
    public partial class CouchbaseExplorerWindowControl : UserControl
    {
        public CouchbaseExplorerViewModel ViewModel { get; }

        public CouchbaseExplorerWindowControl()
        {
            ViewModel = new CouchbaseExplorerViewModel();
            DataContext = ViewModel;

            InitializeComponent();

            ExplorerTreeView.SelectedItemChanged += OnSelectedItemChanged;
        }

        private void OnSelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeNodeBase node)
            {
                ViewModel.SelectedNode = node;
            }
        }
    }
}
