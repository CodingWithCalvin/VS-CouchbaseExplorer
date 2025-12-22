using System;
using System.IO;
using System.Threading.Tasks;
using CodingWithCalvin.CouchbaseExplorer.Editors;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodingWithCalvin.CouchbaseExplorer.Services
{
    public static class DocumentEditorService
    {
        private static readonly string TempFolder = Path.Combine(Path.GetTempPath(), "CouchbaseExplorer");

        public static async Task OpenDocumentAsync(
            string connectionId,
            string documentId,
            string bucketName,
            string scopeName,
            string collectionName,
            object content,
            ulong cas)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // Ensure temp folder exists
            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }

            // Create a safe filename from the document ID
            var safeDocId = MakeSafeFileName(documentId);
            var fileName = $"{bucketName}.{scopeName}.{collectionName}.{safeDocId}.cbjson";
            var filePath = Path.Combine(TempFolder, fileName);

            // Create an empty file (content will be set via SetDocument)
            File.WriteAllText(filePath, "{}");

            // Get the editor factory GUID
            var editorFactoryGuid = typeof(DocumentEditorFactory).GUID;
            var logicalView = VSConstants.LOGVIEWID_Primary;

            // Open the document with our custom editor and get the frame
            IVsWindowFrame frame;
            VsShellUtilities.OpenDocumentWithSpecificEditor(
                ServiceProvider.GlobalProvider,
                filePath,
                editorFactoryGuid,
                logicalView,
                out IVsUIHierarchy hierarchy,
                out uint itemId,
                out frame);

            if (frame != null)
            {
                // Get the editor pane from the frame
                frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out object docView);

                if (docView is DocumentEditorPane editorPane)
                {
                    editorPane.SetDocument(connectionId, documentId, bucketName, scopeName, collectionName, content, cas);
                }
            }
        }

        private static string MakeSafeFileName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = name;

            foreach (var c in invalidChars)
            {
                safeName = safeName.Replace(c, '_');
            }

            // Limit length
            if (safeName.Length > 50)
            {
                safeName = safeName.Substring(0, 50);
            }

            return safeName;
        }
    }
}
