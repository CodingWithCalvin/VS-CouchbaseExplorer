using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CodingWithCalvin.CouchbaseExplorer.Dialogs;
using CodingWithCalvin.CouchbaseExplorer.Services;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class CouchbaseExplorerViewModel : INotifyPropertyChanged
    {
        private TreeNodeBase _selectedNode;
        private ConnectionSettingsService _settingsService;

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

            LoadConnections();
        }

        private void LoadConnections()
        {
            try
            {
                _settingsService = new ConnectionSettingsService();
                var savedConnections = _settingsService.LoadConnections();

                foreach (var connectionInfo in savedConnections)
                {
                    var connectionNode = new ConnectionNode
                    {
                        Id = connectionInfo.Id,
                        Name = connectionInfo.Name,
                        ConnectionString = connectionInfo.ConnectionString,
                        Username = connectionInfo.Username,
                        UseSsl = connectionInfo.UseSsl,
                        IsConnected = false
                    };

                    connectionNode.ConnectRequested += OnConnectRequested;
                    Connections.Add(connectionNode);
                }
            }
            catch (Exception)
            {
                // If settings can't be loaded, start with empty connections
            }
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

                newConnection.ConnectRequested += OnConnectRequested;

                CredentialManagerService.SavePassword(newConnection.Id, dialogViewModel.Password);

                var connectionInfo = new ConnectionInfo
                {
                    Id = newConnection.Id,
                    Name = newConnection.Name,
                    ConnectionString = newConnection.ConnectionString,
                    Username = newConnection.Username,
                    UseSsl = newConnection.UseSsl
                };
                _settingsService.AddConnection(connectionInfo);

                Connections.Add(newConnection);
            }
        }

        private async void OnConnectRequested(ConnectionNode connection)
        {
            await ConnectToNodeAsync(connection);
        }

        private async void OnRefresh(object parameter)
        {
            var node = parameter as TreeNodeBase ?? SelectedNode;
            if (node == null)
            {
                return;
            }

            switch (node)
            {
                case ConnectionNode connection when connection.IsConnected:
                    await LoadBucketsAsync(connection);
                    break;
                case BucketNode bucket:
                    await bucket.RefreshAsync();
                    break;
                case ScopeNode scope:
                    await scope.RefreshAsync();
                    break;
                case CollectionNode collection:
                    await collection.RefreshAsync();
                    break;
            }
        }

        private bool CanRefresh(object parameter)
        {
            var node = parameter as TreeNodeBase ?? SelectedNode;
            return node is ConnectionNode conn ? conn.IsConnected : node is BucketNode || node is ScopeNode || node is CollectionNode;
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

        private async void OnConnect(object parameter)
        {
            var connection = parameter as ConnectionNode ?? SelectedNode as ConnectionNode;
            if (connection != null)
            {
                await ConnectToNodeAsync(connection);
            }
        }

        private async Task ConnectToNodeAsync(ConnectionNode connection)
        {
            if (connection.IsConnected || connection.IsLoading)
            {
                return;
            }

            connection.IsLoading = true;
            connection.Children.Clear();
            connection.Children.Add(new PlaceholderNode { Name = "Connecting..." });

            try
            {
                var password = CredentialManagerService.GetPassword(connection.Id);

                var clusterConnection = await Task.Run(async () =>
                {
                    return await CouchbaseService.ConnectAsync(
                        connection.Id,
                        connection.ConnectionString,
                        connection.Username,
                        password,
                        connection.UseSsl);
                });

                connection.IsConnected = true;
                connection.HasQueryService = clusterConnection.HasQueryService;
                connection.HasKvService = clusterConnection.HasKvService;

                // Load buckets
                await LoadBucketsAsync(connection);
            }
            catch (Exception ex)
            {
                connection.Children.Clear();
                connection.Children.Add(new PlaceholderNode { Name = "(Connection failed)" });

                MessageBox.Show(
                    $"Failed to connect to cluster: {ex.Message}",
                    "Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                connection.IsLoading = false;
            }
        }

        private bool CanConnect(object parameter)
        {
            return SelectedNode is ConnectionNode conn && !conn.IsConnected;
        }

        private async void OnDisconnect(object parameter)
        {
            var connection = parameter as ConnectionNode ?? SelectedNode as ConnectionNode;
            if (connection != null)
            {
                try
                {
                    await CouchbaseService.DisconnectAsync(connection.Id);
                    connection.IsConnected = false;
                    connection.Children.Clear();
                    connection.Children.Add(new PlaceholderNode());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to disconnect: {ex.Message}",
                        "Disconnect Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private bool CanDisconnect(object parameter)
        {
            return SelectedNode is ConnectionNode conn && conn.IsConnected;
        }

        private async Task LoadBucketsAsync(ConnectionNode connection)
        {
            try
            {
                connection.Children.Clear();
                connection.Children.Add(new PlaceholderNode { Name = "Loading buckets..." });

                var buckets = await Task.Run(async () =>
                {
                    return await CouchbaseService.GetBucketsAsync(connection.Id);
                });

                connection.Children.Clear();

                foreach (var bucket in buckets)
                {
                    var bucketNode = new BucketNode
                    {
                        Name = bucket.Name,
                        ConnectionId = connection.Id,
                        BucketType = bucket.BucketType.ToString(),
                        Parent = connection
                    };
                    connection.Children.Add(bucketNode);
                }

                if (connection.Children.Count == 0)
                {
                    connection.Children.Add(new PlaceholderNode { Name = "(No buckets)" });
                }
            }
            catch (Exception ex)
            {
                connection.Children.Clear();
                connection.Children.Add(new PlaceholderNode { Name = $"(Error: {ex.Message})" });
            }
        }

        private void OnEditConnection(object parameter)
        {
            var connection = parameter as ConnectionNode ?? SelectedNode as ConnectionNode;
            if (connection != null)
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

                    CredentialManagerService.SavePassword(connection.Id, dialogViewModel.Password);

                    var connectionInfo = new ConnectionInfo
                    {
                        Id = connection.Id,
                        Name = connection.Name,
                        ConnectionString = connection.ConnectionString,
                        Username = connection.Username,
                        UseSsl = connection.UseSsl
                    };
                    _settingsService.UpdateConnection(connectionInfo);
                }
            }
        }

        private async void OnDeleteConnection(object parameter)
        {
            var connection = parameter as ConnectionNode ?? SelectedNode as ConnectionNode;
            if (connection == null)
            {
                return;
            }

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
                    await CouchbaseService.DisconnectAsync(connection.Id);
                    connection.IsConnected = false;
                }

                // Unsubscribe from events
                connection.ConnectRequested -= OnConnectRequested;

                // Delete saved password
                CredentialManagerService.DeletePassword(connection.Id);

                // Delete from settings
                _settingsService.DeleteConnection(connection.Id);

                // Remove from UI
                Connections.Remove(connection);
            }
        }

        private bool IsConnectionSelected(object parameter)
        {
            return parameter is ConnectionNode || SelectedNode is ConnectionNode;
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
