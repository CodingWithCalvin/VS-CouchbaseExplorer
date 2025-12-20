namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class BucketNode : TreeNodeBase
    {
        public override string NodeType => "Bucket";

        public BucketNode()
        {
            // Add placeholder for lazy loading
            Children.Add(new PlaceholderNode());
        }

        protected override void OnExpanded()
        {
            // TODO: Load scopes when expanded
        }
    }
}
