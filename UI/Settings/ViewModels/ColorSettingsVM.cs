﻿using System;
using System.ComponentModel;
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
            ItemsView.SortDescriptions.Add(new SortDescription("Code", ListSortDirection.Ascending));
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
        #endregion

        #region commands

        public ICommand CreateItemCommand
        {
            get
            {
                return new ActiveDelegateCommand<ColorSettingsVM>(this, (p) => { CreateItem(); },
                    (p) => { return !string.IsNullOrEmpty(InputName) && this._listModel.FirstOrDefault(x => x.Code == InputCode.Trim() && x.Name == InputName) == null; });
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
                Code = this.InputCode.Trim(),
                Name = this.InputName,
            };
            SettingsDao.CreateColor(item);
            this._listModel.Add(item);
            ClearInput();
        }

        private void UpdateItem()
        {
            SelectedItem.Code = this.InputCode;
            SelectedItem.Name = this.InputName;
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

        private void ClearInput(bool flag = true)
        {
            this.InputCode = string.Empty;
            this.InputName = string.Empty;
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
                    InputCode = SelectedItem.Code;
                    InputName = SelectedItem.Name;
                    return;
                }
            }
            ClearInput(false);
        }
        #endregion
    }
}
