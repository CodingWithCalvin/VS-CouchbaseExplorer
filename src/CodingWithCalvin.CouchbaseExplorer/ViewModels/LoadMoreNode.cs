using System;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class LoadMoreNode : TreeNodeBase
    {
        public override string NodeType => "LoadMore";

        public int NextOffset { get; set; }

        public event Action<LoadMoreNode> LoadMoreRequested;

        public LoadMoreNode()
        {
            Name = "Load More...";
        }

        public void RequestLoadMore()
        {
            LoadMoreRequested?.Invoke(this);
        }

        protected override void OnSelected()
        {
            RequestLoadMore();
        }
    }
}
