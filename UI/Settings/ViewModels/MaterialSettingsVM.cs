using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Command;
using Common.Models;
using Common.ViewModels;
using UI.Settings.Models;
using UI.Settings.Dao;

namespace UI.Settings.ViewModels
{
    public class MaterialSettingsVM : ListViewModel<MaterialListModel, MaterialItemModel>
    {
        #region constructor
        public MaterialSettingsVM(MaterialListModel _listMoodel)
            : base(_listMoodel)
        {
            this.ApplyFilter();
        }
        #endregion

        #region fields
        public static MaterialSettingsVM MaterialSettings
        {
            get { return _MaterialSettings = _MaterialSettings ?? new MaterialSettingsVM(new MaterialListModel()); }
        }
        private static MaterialSettingsVM _MaterialSettings;

        public string InputName
        {
            get { return _InputName; }
            set { _InputName = value; OnPropertyChanged("InputName"); }
        }
        private string _InputName = string.Empty;
        #endregion

        #region commands

        public ICommand CreateItemCommand
        {
            get
            {
                return new ActiveDelegateCommand<MaterialSettingsVM>(this,
                    (p) => { CreateItem(); },
                    (p) => { return !string.IsNullOrEmpty(InputName) && this._listModel.FirstOrDefault(x => x.Name == InputName) == null; });
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return new SelectionDelegateCommand<MaterialSettingsVM>(this,
                    (vm) => { UpdateItem(); },
                    (vm) => { return this._listModel.FirstOrDefault(x => x.Code == SelectedItem.Code) != null && !string.IsNullOrEmpty(InputName); });
            }
        }

        public ICommand DeleteCommand
        {
            get { return new SelectionDelegateCommand<MaterialSettingsVM>(this, (vm) => { DeleteItem(); }); }
        }

        public ICommand ClearCommand
        {
            get { return new RelayCommand<object>((p) => { ClearInput(); }); }
        }

        public ICommand DetailChangedCommand
        {
            get { return new RelayCommand(DetailChanged); }
        }
        #endregion
        #region methods

        protected override bool FilterItem(MaterialItemModel item)
        {
            return true;
        }

        public static string GetNewMaterialCode(ObservableCollection<MaterialItemModel> items)
        {
            MaterialItemModel last = items.OrderByDescending(x => x.Code).FirstOrDefault();
            int newId = 0;
            if (last != null)
            {
                newId = Convert.ToInt32(last.Code.Substring(2)) + 1;
            }
            return string.Format("M{0:D2}", newId);
        }

        private void CreateItem()
        {
            MaterialItemModel item = new MaterialItemModel() { Code = GetNewMaterialCode(_listModel), Name = InputName };
            SettingsDao.CreateMaterial(item);
            this._listModel.Add(item);
            ClearInput();
        }

        private void UpdateItem()
        {
            SelectedItem.Name = this.InputName;
            SettingsDao.UpdateMaterial(SelectedItem);
            ClearInput();
        }

        private void DeleteItem()
        {
            if (this.SelectedItems != null)
            {
                for (int i = SelectedItems.Count - 1; i >= 0; i--)
                {
                    SettingsDao.DeleteMaterial(((MaterialItemModel)SelectedItems[i]).Code);
                    this._listModel.Remove((MaterialItemModel)SelectedItems[i]);
                }
            }
            ClearInput();
        }

        private void ClearInput(bool flag = true)
        {
            InputName = string.Empty;
            if (flag)
            {
                SelectedItem = null;
            }
        }

        private void DetailChanged()
        {
            if (this.SelectedItems != null)
            {
                if (this.SelectedItems.Count == 1)
                {
                    InputName = SelectedItem.Name;
                    return;
                }
            }
            ClearInput(false);
        }
        #endregion
    }
}
