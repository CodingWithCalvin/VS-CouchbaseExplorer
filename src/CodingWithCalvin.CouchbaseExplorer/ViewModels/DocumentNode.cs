namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class DocumentNode : TreeNodeBase
    {
        public override string NodeType => "Document";

        public string DocumentId { get; set; }

        public string BucketName { get; set; }

        public string ScopeName { get; set; }

        public string CollectionName { get; set; }
    }
}
