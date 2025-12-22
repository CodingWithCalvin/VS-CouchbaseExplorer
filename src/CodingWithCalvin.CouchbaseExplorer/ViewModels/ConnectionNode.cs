using System;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class ConnectionNode : TreeNodeBase
    {
        private bool _isConnected;
        private string _connectionString;
        private bool _hasQueryService;
        private bool _hasKvService;

        public override string NodeType => "Connection";

        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Event raised when the node is expanded and needs to connect.
        /// </summary>
        public event Action<ConnectionNode> ConnectRequested;

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        public string ConnectionString
        {
            get => _connectionString;
            set => SetProperty(ref _connectionString, value);
        }

        public string Username { get; set; }

        public bool UseSsl { get; set; }

        public bool HasQueryService
        {
            get => _hasQueryService;
            set => SetProperty(ref _hasQueryService, value);
        }

        public bool HasKvService
        {
            get => _hasKvService;
            set => SetProperty(ref _hasKvService, value);
        }

        public ConnectionNode()
        {
            // Add placeholder for lazy loading
            Children.Add(new PlaceholderNode());
        }

        protected override void OnExpanded()
        {
            // If not connected, request connection
            if (!IsConnected)
            {
                ConnectRequested?.Invoke(this);
            }
        }
    }
}
