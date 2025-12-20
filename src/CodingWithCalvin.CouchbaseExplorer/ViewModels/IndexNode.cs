namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class IndexNode : TreeNodeBase
    {
        public override string NodeType => "Index";

        public string IndexName { get; set; }

        public bool IsPrimary { get; set; }

        public string BucketName { get; set; }

        public string ScopeName { get; set; }

        public string CollectionName { get; set; }
    }
}
