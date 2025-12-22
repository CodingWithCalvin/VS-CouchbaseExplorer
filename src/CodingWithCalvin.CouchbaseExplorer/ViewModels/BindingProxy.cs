using System.Windows;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    /// <summary>
    /// A proxy class that allows binding to a DataContext from outside the visual tree.
    /// This is commonly used for ContextMenus, Popups, and other elements that exist
    /// in a separate visual tree.
    /// </summary>
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                nameof(Data),
                typeof(object),
                typeof(BindingProxy),
                new UIPropertyMetadata(null));

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}
