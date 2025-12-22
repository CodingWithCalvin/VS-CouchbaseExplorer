using System;
using System.Threading.Tasks;
using CodingWithCalvin.CouchbaseExplorer.Services;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class ScopeNode : TreeNodeBase
    {
        private bool _hasLoadedCollections;

        public override string NodeType => "Scope";

        public string ConnectionId { get; set; }

        public string BucketName { get; set; }

        public ScopeNode()
        {
            // Add placeholder for lazy loading
            Children.Add(new PlaceholderNode());
        }

        protected override async void OnExpanded()
        {
            if (_hasLoadedCollections)
            {
                return;
            }

            await LoadCollectionsAsync();
        }

        public async Task RefreshAsync()
        {
            _hasLoadedCollections = false;
            Children.Clear();
            Children.Add(new PlaceholderNode { Name = "Refreshing..." });
            await LoadCollectionsAsync();
        }

        public async Task LoadCollectionsAsync()
        {
            IsLoading = true;

            try
            {
                var collections = await CouchbaseService.GetCollectionsAsync(ConnectionId, BucketName, Name);

                Children.Clear();

                foreach (var collection in collections)
                {
                    var collectionNode = new CollectionNode
                    {
                        Name = collection.Name,
                        ConnectionId = ConnectionId,
                        BucketName = BucketName,
                        ScopeName = Name,
                        Parent = this
                    };
                    Children.Add(collectionNode);
                }

                if (Children.Count == 0)
                {
                    Children.Add(new PlaceholderNode { Name = "(No collections)" });
                }

                _hasLoadedCollections = true;
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
