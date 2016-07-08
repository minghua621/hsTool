using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Common.ViewModels
{
    public class ListViewModel<T> : NotificationBase
    {
        public ObservableCollection<T> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }
        private ObservableCollection<T> _items = new ObservableCollection<T>();

        public T SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }
        private T _selectedItem;

        public IList<T> SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; OnPropertyChanged("SelectedItems"); }
        }
        private IList<T> _selectedItems;
    }
}
