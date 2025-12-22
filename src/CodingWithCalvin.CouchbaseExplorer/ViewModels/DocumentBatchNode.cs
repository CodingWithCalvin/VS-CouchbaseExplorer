using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodingWithCalvin.CouchbaseExplorer.Services;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class DocumentBatchNode : TreeNodeBase
    {
        private bool _hasLoadedDocuments;
        private List<string> _documentIds;

        public override string NodeType => "DocumentBatch";

        public string ConnectionId { get; set; }

        public string BucketName { get; set; }

        public string ScopeName { get; set; }

        public string CollectionName { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public DocumentBatchNode()
        {
            Children.Add(new PlaceholderNode());
        }

        public DocumentBatchNode(List<string> documentIds, int startIndex)
        {
            _documentIds = documentIds;
            StartIndex = startIndex;
            EndIndex = startIndex + documentIds.Count - 1;
            Name = $"[{StartIndex + 1}-{EndIndex + 1}]";
            Children.Add(new PlaceholderNode());
        }

        protected override async void OnExpanded()
        {
            if (_hasLoadedDocuments)
            {
                return;
            }

            await LoadDocumentsAsync();
        }

        public async Task RefreshAsync()
        {
            _hasLoadedDocuments = false;
            Children.Clear();
            Children.Add(new PlaceholderNode { Name = "Refreshing..." });
            await LoadDocumentsAsync();
        }

        private async Task LoadDocumentsAsync()
        {
            IsLoading = true;

            try
            {
                Children.Clear();

                if (_documentIds != null && _documentIds.Count > 0)
                {
                    foreach (var docId in _documentIds)
                    {
                        var docNode = new DocumentNode
                        {
                            Name = docId,
                            DocumentId = docId,
                            ConnectionId = ConnectionId,
                            BucketName = BucketName,
                            ScopeName = ScopeName,
                            CollectionName = CollectionName,
                            Parent = this
                        };
                        Children.Add(docNode);
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
    }
}
