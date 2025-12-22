using System;
using System.Windows;
using System.Windows.Controls;
using CodingWithCalvin.CouchbaseExplorer.Services;
using Newtonsoft.Json;

namespace CodingWithCalvin.CouchbaseExplorer.Editors
{
    public partial class DocumentEditorControl : UserControl
    {
        public string ConnectionId { get; private set; }
        public string BucketName { get; private set; }
        public string ScopeName { get; private set; }
        public string CollectionName { get; private set; }
        public string DocumentId { get; private set; }
        public ulong Cas { get; private set; }

        public DocumentEditorControl()
        {
            InitializeComponent();
        }

        public void SetDocument(string connectionId, string documentId, string bucketName, string scopeName, string collectionName, object content, ulong cas)
        {
            ConnectionId = connectionId;
            DocumentId = documentId;
            BucketName = bucketName;
            ScopeName = scopeName;
            CollectionName = collectionName;
            Cas = cas;

            DocumentIdText.Text = documentId;
            CollectionPathText.Text = $"{bucketName}.{scopeName}.{collectionName}";
            CasText.Text = cas.ToString();

            if (content != null)
            {
                JsonContentBox.Text = JsonConvert.SerializeObject(content, Formatting.Indented);
            }
            else
            {
                JsonContentBox.Text = "null";
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RefreshButton.IsEnabled = false;
                RefreshButton.Content = "Refreshing...";

                var content = await CouchbaseService.GetDocumentAsync(
                    ConnectionId,
                    BucketName,
                    ScopeName,
                    CollectionName,
                    DocumentId);

                Cas = content.Cas;
                CasText.Text = content.Cas.ToString();

                if (content.Content != null)
                {
                    JsonContentBox.Text = JsonConvert.SerializeObject(content.Content, Formatting.Indented);
                }
                else
                {
                    JsonContentBox.Text = "null";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to refresh document: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                RefreshButton.IsEnabled = true;
                RefreshButton.Content = "Refresh";
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(JsonContentBox.Text);
        }
    }
}
