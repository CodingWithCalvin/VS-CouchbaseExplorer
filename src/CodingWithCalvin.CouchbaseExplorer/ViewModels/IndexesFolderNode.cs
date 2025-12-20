namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class IndexesFolderNode : TreeNodeBase
    {
        public override string NodeType => "IndexesFolder";

        public IndexesFolderNode()
        {
            Name = "Indexes";
        }
    }
}
