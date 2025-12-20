namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class LoadMoreNode : TreeNodeBase
    {
        public override string NodeType => "LoadMore";

        public int NextOffset { get; set; }

        public LoadMoreNode()
        {
            Name = "Load More...";
        }
    }
}
