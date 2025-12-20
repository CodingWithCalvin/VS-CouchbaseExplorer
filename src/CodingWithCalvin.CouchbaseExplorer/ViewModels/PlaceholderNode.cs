namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class PlaceholderNode : TreeNodeBase
    {
        public override string NodeType => "Placeholder";

        public PlaceholderNode()
        {
            Name = "Loading...";
        }
    }
}
