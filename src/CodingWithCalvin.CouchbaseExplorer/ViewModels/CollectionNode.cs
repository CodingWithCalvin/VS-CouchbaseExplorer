using System;
using System.Threading.Tasks;
using CodingWithCalvin.CouchbaseExplorer.Services;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class CollectionNode : TreeNodeBase
    {
        private bool _hasLoadedDocuments;
        private int _currentOffset;
        private const int BatchSize = 50;

        public override string NodeType => "Collection";

        public string ConnectionId { get; set; }

        public string BucketName { get; set; }

        public string ScopeName { get; set; }

        public CollectionNode()
        {
            // Add placeholder for lazy loading
            Children.Add(new PlaceholderNode());
        }

        protected override async void OnExpanded()
        {
            if (_hasLoadedDocuments)
            {
                return;
            }

            await LoadDocumentBatchesAsync();
        }

        public async Task RefreshAsync()
        {
            _hasLoadedDocuments = false;
            _currentOffset = 0;
            Children.Clear();
            Children.Add(new PlaceholderNode { Name = "Refreshing..." });
            await LoadDocumentBatchesAsync();
        }

        private async Task LoadDocumentBatchesAsync()
        {
            IsLoading = true;

            try
            {
                Children.Clear();
                Children.Add(new PlaceholderNode { Name = "Loading documents..." });

                var result = await CouchbaseService.GetDocumentIdsAsync(
                    ConnectionId, BucketName, ScopeName, Name, BatchSize, _currentOffset);

                Children.Clear();

                if (result.DocumentIds.Count > 0)
                {
                    var batchNode = new DocumentBatchNode(result.DocumentIds, _currentOffset)
                    {
                        ConnectionId = ConnectionId,
                        BucketName = BucketName,
                        ScopeName = ScopeName,
                        CollectionName = Name,
                        Parent = this
                    };
                    Children.Add(batchNode);

                    _currentOffset += result.DocumentIds.Count;

                    if (result.HasMore)
                    {
                        var loadMoreNode = new LoadMoreNode
                        {
                            Name = "Load More...",
                            Parent = this
                        };
                        loadMoreNode.LoadMoreRequested += OnLoadMoreRequested;
                        Children.Add(loadMoreNode);
                    }
                }
                else
                {
                    Children.Add(new PlaceholderNode { Name = "(No documents)" });
                }

                _hasLoadedDocuments = true;
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

        private async void OnLoadMoreRequested(LoadMoreNode node)
        {
            node.LoadMoreRequested -= OnLoadMoreRequested;
            Children.Remove(node);

            IsLoading = true;

            try
            {
                var result = await CouchbaseService.GetDocumentIdsAsync(
                    ConnectionId, BucketName, ScopeName, Name, BatchSize, _currentOffset);

                if (result.DocumentIds.Count > 0)
                {
                    var batchNode = new DocumentBatchNode(result.DocumentIds, _currentOffset)
                    {
                        ConnectionId = ConnectionId,
                        BucketName = BucketName,
                        ScopeName = ScopeName,
                        CollectionName = Name,
                        Parent = this
                    };
                    Children.Add(batchNode);

                    _currentOffset += result.DocumentIds.Count;

                    if (result.HasMore)
                    {
                        var loadMoreNode = new LoadMoreNode
                        {
                            Name = "Load More...",
                            Parent = this
                        };
                        loadMoreNode.LoadMoreRequested += OnLoadMoreRequested;
                        Children.Add(loadMoreNode);
                    }
                }
            }
            catch (Exception ex)
            {
                Children.Add(new PlaceholderNode { Name = $"(Error loading more: {ex.Message})" });
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
