using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace CodingWithCalvin.CouchbaseExplorer
{
    [Guid("d7fbfae3-4b71-4507-86b0-0534e77d0292")]
    public class CouchbaseExplorerWindow : ToolWindowPane
    {
        public CouchbaseExplorerWindow()
            : base(null)
        {
            this.Caption = "Couchbase Explorer";
            this.Content = new CouchbaseExplorerWindowControl();
        }
    }
}
