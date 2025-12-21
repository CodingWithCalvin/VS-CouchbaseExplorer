using System.Windows;
using System.Windows.Controls;
using CodingWithCalvin.CouchbaseExplorer.ViewModels;
using Microsoft.VisualStudio.PlatformUI;

namespace CodingWithCalvin.CouchbaseExplorer.Dialogs
{
    public partial class ConnectionDialog : DialogWindow
    {
        public ConnectionDialogViewModel ViewModel { get; }

        public ConnectionDialog(ConnectionDialogViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();

            // Set up close handlers
            ViewModel.RequestClose += OnRequestClose;

            // If editing, set the password box placeholder
            if (ViewModel.IsEditMode && !string.IsNullOrEmpty(ViewModel.Password))
            {
                PasswordBox.Password = ViewModel.Password;
            }
        }

        private void OnRequestClose(bool? dialogResult)
        {
            DialogResult = dialogResult;
            Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                ViewModel.Password = passwordBox.Password;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            ViewModel.RequestClose -= OnRequestClose;
            base.OnClosed(e);
        }
    }
}
