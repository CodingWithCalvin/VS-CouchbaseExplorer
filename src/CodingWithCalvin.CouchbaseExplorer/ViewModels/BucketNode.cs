using System;
using System.Threading.Tasks;
using CodingWithCalvin.CouchbaseExplorer.Services;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class BucketNode : TreeNodeBase
    {
        private bool _hasLoadedScopes;

        public override string NodeType => "Bucket";

        public string ConnectionId { get; set; }

        public string BucketType { get; set; }

        public long RamQuotaMB { get; set; }

        public int NumReplicas { get; set; }

        public BucketNode()
        {
            // Add placeholder for lazy loading
            Children.Add(new PlaceholderNode());
        }

        protected override async void OnExpanded()
        {
            if (_hasLoadedScopes)
            {
                return;
            }

            await LoadScopesAsync();
        }

        public async Task RefreshAsync()
        {
            _hasLoadedScopes = false;
            Children.Clear();
            Children.Add(new PlaceholderNode { Name = "Refreshing..." });
            await LoadScopesAsync();
        }

        public async Task LoadScopesAsync()
        {
            IsLoading = true;

            try
            {
                var scopes = await CouchbaseService.GetScopesAsync(ConnectionId, Name);

                Children.Clear();

                foreach (var scope in scopes)
                {
                    var scopeNode = new ScopeNode
                    {
                        Name = scope.Name,
                        ConnectionId = ConnectionId,
                        BucketName = Name,
                        Parent = this
                    };
                    Children.Add(scopeNode);
                }

                if (Children.Count == 0)
                {
                    Children.Add(new PlaceholderNode { Name = "(No scopes)" });
                }

                _hasLoadedScopes = true;
            }
            catch (Exception ex)
            {
                Children.Clear();
                Children.Add(new PlaceholderNode { Name = $"(Error: {ex.Message})" });
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
