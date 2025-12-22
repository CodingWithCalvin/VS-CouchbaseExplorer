using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            ExplorerTreeView.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
            ExplorerTreeView.MouseDoubleClick += OnMouseDoubleClick;
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeNodeBase node)
            {
                ViewModel.SelectedNode = node;
            }
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                treeViewItem.IsSelected = true;
                e.Handled = true;
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedNode is DocumentNode)
            {
                ViewModel.OpenDocumentCommand.Execute(null);
                e.Handled = true;
            }
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T result)
                {
                    return result;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }
}
