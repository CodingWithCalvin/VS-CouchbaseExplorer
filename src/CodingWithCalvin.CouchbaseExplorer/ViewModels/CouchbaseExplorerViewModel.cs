using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CodingWithCalvin.CouchbaseExplorer.Dialogs;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class CouchbaseExplorerViewModel : INotifyPropertyChanged
    {
        private TreeNodeBase _selectedNode;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TreeNodeBase> Connections { get; } = new ObservableCollection<TreeNodeBase>();

        public TreeNodeBase SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (_selectedNode != value)
                {
                    _selectedNode = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddConnectionCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CollapseAllCommand { get; }

        // Connection context menu commands
        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand EditConnectionCommand { get; }
        public ICommand DeleteConnectionCommand { get; }

        // Collection context menu commands
        public ICommand NewDocumentCommand { get; }

        // Document context menu commands
        public ICommand OpenDocumentCommand { get; }
        public ICommand DeleteDocumentCommand { get; }
        public ICommand CopyDocumentIdCommand { get; }

        public CouchbaseExplorerViewModel()
        {
            AddConnectionCommand = new RelayCommand(OnAddConnection);
            RefreshCommand = new RelayCommand(OnRefresh, CanRefresh);
            CollapseAllCommand = new RelayCommand(OnCollapseAll, _ => Connections.Count > 0);

            ConnectCommand = new RelayCommand(OnConnect, CanConnect);
            DisconnectCommand = new RelayCommand(OnDisconnect, CanDisconnect);
            EditConnectionCommand = new RelayCommand(OnEditConnection, IsConnectionSelected);
            DeleteConnectionCommand = new RelayCommand(OnDeleteConnection, IsConnectionSelected);

            NewDocumentCommand = new RelayCommand(OnNewDocument, IsCollectionSelected);

            OpenDocumentCommand = new RelayCommand(OnOpenDocument, IsDocumentSelected);
            DeleteDocumentCommand = new RelayCommand(OnDeleteDocument, IsDocumentSelected);
            CopyDocumentIdCommand = new RelayCommand(OnCopyDocumentId, IsDocumentSelected);
        }

        private HashSet<string> GetExistingConnectionNames()
        {
            return new HashSet<string>(
                Connections.OfType<ConnectionNode>().Select(c => c.Name),
                StringComparer.OrdinalIgnoreCase);
        }

        private void OnAddConnection(object parameter)
        {
            var existingNames = GetExistingConnectionNames();
            var dialogViewModel = new ConnectionDialogViewModel(existingNames);

            var dialog = new ConnectionDialog(dialogViewModel);
            dialog.Owner = Application.Current.MainWindow;

            if (dialog.ShowDialog() == true)
            {
                var newConnection = new ConnectionNode
                {
                    Name = dialogViewModel.ConnectionName,
                    ConnectionString = dialogViewModel.Host,
                    Username = dialogViewModel.Username,
                    UseSsl = dialogViewModel.UseSsl,
                    IsConnected = false
                };

                // TODO: Store password securely using Windows Credential Manager

                Connections.Add(newConnection);
            }
        }

        private void OnRefresh(object parameter)
        {
            // TODO: Refresh selected node or all connections
        }

        private bool CanRefresh(object parameter)
        {
            return SelectedNode != null || Connections.Count > 0;
        }

        private void OnCollapseAll(object parameter)
        {
            foreach (var connection in Connections)
            {
                CollapseNode(connection);
            }
        }

        private void CollapseNode(TreeNodeBase node)
        {
            node.IsExpanded = false;
            foreach (var child in node.Children)
            {
                CollapseNode(child);
            }
        }

        private void OnConnect(object parameter)
        {
            // TODO: Connect to cluster
        }

        private bool CanConnect(object parameter)
        {
            return SelectedNode is ConnectionNode conn && !conn.IsConnected;
        }

        private void OnDisconnect(object parameter)
        {
            // TODO: Disconnect from cluster
        }

        private bool CanDisconnect(object parameter)
        {
            return SelectedNode is ConnectionNode conn && conn.IsConnected;
        }

        private void OnEditConnection(object parameter)
        {
            if (SelectedNode is ConnectionNode connection)
            {
                var existingNames = GetExistingConnectionNames();
                var dialogViewModel = new ConnectionDialogViewModel(existingNames, connection);

                var dialog = new ConnectionDialog(dialogViewModel);
                dialog.Owner = Application.Current.MainWindow;

                if (dialog.ShowDialog() == true)
                {
                    connection.Name = dialogViewModel.ConnectionName;
                    connection.ConnectionString = dialogViewModel.Host;
                    connection.Username = dialogViewModel.Username;
                    connection.UseSsl = dialogViewModel.UseSsl;

                    // TODO: Update password in Windows Credential Manager
                }
            }
        }

        private void OnDeleteConnection(object parameter)
        {
            if (SelectedNode is ConnectionNode connection)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the connection '{connection.Name}'?",
                    "Delete Connection",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Disconnect first if connected
                    if (connection.IsConnected)
                    {
                        // TODO: Disconnect from cluster
                        connection.IsConnected = false;
                    }

                    // TODO: Remove password from Windows Credential Manager

                    Connections.Remove(connection);
                }
            }
        }

        private bool IsConnectionSelected(object parameter)
        {
            return SelectedNode is ConnectionNode;
        }

        private void OnNewDocument(object parameter)
        {
            // TODO: Open new document editor
        }

        private bool IsCollectionSelected(object parameter)
        {
            return SelectedNode is CollectionNode;
        }

        private void OnOpenDocument(object parameter)
        {
            // TODO: Open document in editor
        }

        private void OnDeleteDocument(object parameter)
        {
            // TODO: Confirm and delete document
        }

        private void OnCopyDocumentId(object parameter)
        {
            if (SelectedNode is DocumentNode doc)
            {
                System.Windows.Clipboard.SetText(doc.DocumentId);
            }
        }

        private bool IsDocumentSelected(object parameter)
        {
            return SelectedNode is DocumentNode;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
