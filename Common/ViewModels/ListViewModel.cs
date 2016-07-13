using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Command;
using Common.Models;

namespace Common.ViewModels
{
    public abstract class ListViewModel<TListModel, TItemModel> : NotificationBase
        where TListModel : ListModel<TItemModel>
        where TItemModel : ItemModel
    {
        protected TListModel _listModel;

        protected abstract bool FilterItem(TItemModel item);

        public ListViewModel(TListModel listModel)
        {
            _listModel = listModel;
            _listModel.CollectionChanged += _listModel_CollectionChanged;

            var rows = from x in _listModel where FilterItem(x) select x;
            foreach (var item in rows)
            {
                this.Items.Add(item);
            }
        }

        private void _listModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var rows = from x in e.NewItems.Cast<TItemModel>() where FilterItem(x) select x;
                foreach (var item in rows)
                {
                    this.Items.Add(item);
                }
            }
        }

        protected void ApplyFilter()
        {
            this.Items.Clear();

            var rows = from x in _listModel where FilterItem(x) select x;            
            foreach (var item in rows)
            {                
                this.Items.Add(item);
            }
        }

        #region fields

        public ObservableCollection<TItemModel> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }
        private ObservableCollection<TItemModel> _items = new ObservableCollection<TItemModel>();

        public TItemModel SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }
        private TItemModel _selectedItem;

        public IList SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; OnPropertyChanged("SelectedItems"); }
        }
        private IList _selectedItems;

        #endregion

        public ICommand SelectionChangedCommand
        {
            get
            {
                return selectionChangedCommand = selectionChangedCommand ?? new RelayCommand<IList>(p =>
                {
                    this.SelectedItems = p;
                });
            }
        }
        private ICommand selectionChangedCommand;

        public event EventHandler SelectionChanged;

        protected void onSelectionChanged(string propertyName)
        {
            var handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public class SelectionDelegateCommand<TViewModel> : ActiveDelegateCommand<TViewModel>
            where TViewModel : ListViewModel<TListModel, TItemModel>
        {
            public SelectionDelegateCommand(TViewModel viewModel, Action<TViewModel> execute, Func<TViewModel, bool> canExecute = null)
                : base(viewModel, execute, canExecute)
            {
                _viewModel.SelectionChanged += (s, e) =>
                {
                    this.OnCanExecuteChanged();
                };
            }

            public override bool CanExecute(object parameter)
            {
                return _viewModel.SelectedItem != null && (_canExecute == null || _canExecute(_viewModel));
            }
        }
    }
}
