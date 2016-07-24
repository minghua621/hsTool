using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Command;
using Common.ViewModels;
using UI.Settings.Models;
using UI.Settings.Dao;

namespace UI.Settings.ViewModels
{
    public class ColorSettingsVM : ListViewModel<ColorListModel, ColorItemModel>
    {
        #region constructor

        public ColorSettingsVM(ColorListModel _listMoodel)
            : base(_listMoodel)
        {
            this.ApplyFilter();
        }

        #endregion

        #region fields

        public static ColorSettingsVM ColorSettings
        {
            get { return _ColorSettings = _ColorSettings ?? Initialize(); }
        }
        private static ColorSettingsVM _ColorSettings;

        public string InputCode
        {
            get { return _InputCode; }
            set { _InputCode = value; OnPropertyChanged("InputCode"); }
        }
        private string _InputCode = string.Empty;

        public string InputName
        {
            get { return _InputName; }
            set { _InputName = value; OnPropertyChanged("InputName"); }
        }
        private string _InputName = string.Empty;

        public string InputCodeAid
        {
            get { return _InputCodeAid; }
            set { _InputCodeAid = value; OnPropertyChanged("InputCodeAid"); }
        }
        private string _InputCodeAid = string.Empty;

        #endregion

        #region commands

        public ICommand CreateItemCommand
        {
            get
            {
                return new ActiveDelegateCommand<ColorSettingsVM>(this, (p) => { CreateItem(); },
                    (p) => { return !string.IsNullOrEmpty(InputCode) && !string.IsNullOrEmpty(InputName) && this._listModel.FirstOrDefault(x => x.Code == InputCode) == null; });
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return new SelectionDelegateCommand<ColorSettingsVM>(this,
                    (vm) => { UpdateItem(); },
                    (vm) => { return this._listModel.FirstOrDefault(x => x.Code == InputCode) != null && !string.IsNullOrEmpty(InputName); });
            }
        }

        public ICommand DeleteCommand
        {
            get { return new SelectionDelegateCommand<ColorSettingsVM>(this, (vm) => { DeleteItem(); }); }
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

        protected override bool FilterItem(ColorItemModel item)
        {
            return true;
        }

        private static ColorSettingsVM Initialize()
        {
            return new ColorSettingsVM(new ColorListModel());
        }

        private void CreateItem()
        {
            ColorItemModel item = new ColorItemModel()
            {
                Code = this.InputCode,                
                Name = this.InputName,
                CodeAid = this.InputCodeAid,
            };
            SettingsDao.CreateColor(item);
            this._listModel.Add(item);
            ClearInput();
        }

        private void UpdateItem()
        {
            SelectedItem.Code = this.InputCode;
            SelectedItem.Name = this.InputName;
            SelectedItem.CodeAid = this.InputCodeAid;
            SettingsDao.UpdateColor(SelectedItem);
            ClearInput();
        }

        private void DeleteItem()
        {
            if (this.SelectedItems != null)
            {
                for (int i = SelectedItems.Count - 1; i >= 0; i--)
                {
                    SettingsDao.DeleteColor(((ColorItemModel)SelectedItems[i]).Code);
                    this._listModel.Remove((ColorItemModel)SelectedItems[i]);
                }
            }
            ClearInput();
        }

        private void ClearInput()
        {
            this.InputCode = string.Empty;
            this.InputName = string.Empty;
            this.InputCodeAid = string.Empty;
        }

        private void DetailChanged()
        {
            if (this.SelectedItems != null)
            {
                if (this.SelectedItems.Count == 1)
                {
                    InputCode = SelectedItem.Code;
                    InputName = SelectedItem.Name;
                    InputCodeAid = SelectedItem.CodeAid;
                    return;
                }
            }
            ClearInput();
        }

        public static string ColorItemToCode(List<ColorItemModel> items)
        {
            string rlt = string.Empty;
            foreach (var item in items.OrderBy(x => x.Code))
            {
                rlt += item.Code + ",";
            }
            return rlt;
        }

        public static List<ColorItemModel> CodeToColorItemModel(string colorList)
        {
            List<ColorItemModel> rlt = new List<ColorItemModel>();
            if (!string.IsNullOrEmpty(colorList))
            {
                string[] colors = colorList.Split(",".ToCharArray());
                foreach (string code in colors)
                {
                    ColorItemModel item = ColorSettings.Items.FirstOrDefault(x => x.Code == code);
                    if (item != null)
                    {
                        rlt.Add(item);
                    }
                }
            }
            return rlt;
        }
        #endregion
    }
}
