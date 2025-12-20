namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class ScopeNode : TreeNodeBase
    {
        public override string NodeType => "Scope";

        public ScopeNode()
        {
            // Add placeholder for lazy loading
            Children.Add(new PlaceholderNode());
        }

        protected override void OnExpanded()
        {
            // TODO: Load collections when expanded
        }
    }
}
