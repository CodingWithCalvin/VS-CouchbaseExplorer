using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public abstract class TreeNodeBase : INotifyPropertyChanged
    {
        private string _name;
        private bool _isExpanded;
        private bool _isSelected;
        private bool _isLoading;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (SetProperty(ref _isExpanded, value) && value)
                {
                    OnExpanded();
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<TreeNodeBase> Children { get; } = new ObservableCollection<TreeNodeBase>();

        public TreeNodeBase Parent { get; set; }

        public abstract string NodeType { get; }

        protected virtual void OnExpanded()
        {
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
