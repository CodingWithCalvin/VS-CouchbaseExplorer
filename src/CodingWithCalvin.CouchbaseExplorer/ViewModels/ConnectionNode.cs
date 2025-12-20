namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class ConnectionNode : TreeNodeBase
    {
        private bool _isConnected;
        private string _connectionString;

        public override string NodeType => "Connection";

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

        public ConnectionNode()
        {
            // Add placeholder for lazy loading
            Children.Add(new PlaceholderNode());
        }

        protected override void OnExpanded()
        {
            // TODO: Load buckets and indexes when connected and expanded
        }
    }
}
