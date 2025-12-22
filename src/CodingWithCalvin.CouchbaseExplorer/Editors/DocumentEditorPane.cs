using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace CodingWithCalvin.CouchbaseExplorer.Editors
{
    [Guid("E8A3C5D1-9B2F-4A6C-8D7E-1F2A3B4C5D6E")]
    public class DocumentEditorPane : WindowPane
    {
        private readonly DocumentEditorControl _control;

        public DocumentEditorControl EditorControl => _control;

        public DocumentEditorPane() : base(null)
        {
            _control = new DocumentEditorControl();
            Content = _control;
        }

        public void SetDocument(string connectionId, string documentId, string bucketName, string scopeName, string collectionName, object content, ulong cas)
        {
            _control.SetDocument(connectionId, documentId, bucketName, scopeName, collectionName, content, cas);
        }
    }
}
