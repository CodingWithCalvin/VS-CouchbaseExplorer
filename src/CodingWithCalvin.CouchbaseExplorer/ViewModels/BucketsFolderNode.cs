namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public class BucketsFolderNode : TreeNodeBase
    {
        public override string NodeType => "BucketsFolder";

        public BucketsFolderNode()
        {
            Name = "Buckets";
        }
    }
}
