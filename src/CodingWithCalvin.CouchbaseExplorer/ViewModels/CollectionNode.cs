namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class CollectionNode : TreeNodeBase
    {
        public override string NodeType => "Collection";

        public string BucketName { get; set; }

        public string ScopeName { get; set; }

        public CollectionNode()
        {
            // Add placeholder for lazy loading
            Children.Add(new PlaceholderNode());
        }

        protected override void OnExpanded()
        {
            // TODO: Load documents in batches when expanded
        }
    }
}
