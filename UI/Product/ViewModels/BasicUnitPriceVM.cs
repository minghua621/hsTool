using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common.Command;
using Common.ViewModels;
using UI.Main;
using UI.Product.Models;
using UI.Product.Dao;
using UI.Settings.Models;
using UI.Settings.ViewModels;

namespace UI.Product.ViewModels
{
    public class BasicUnitPriceVM : ListViewModel<UnitPriceListModel, UnitPriceItemModel>
    {
        #region constructor

        public BasicUnitPriceVM(UnitPriceListModel _listMoodel, string customerCode)
            : base(_listMoodel)
        {
            _customerType = customerCode;
            this.ApplyFilter();
        }

        #endregion

        #region fields

        public string _customerType { get; private set; }

        public string ProductCode
        {
            get { return productCode; }
            set { productCode = value; OnPropertyChanged("ProductCode"); }
        }
        private string productCode = string.Empty;

        public string CustomerName
        {
            get { return AppSettings.CustomerList[_customerType].Name; }
        }

        public static List<BasicUnitPriceVM> Units
        {
            get { return _units = _units ?? Initialize(); }
        }
        private static List<BasicUnitPriceVM> _units;

        public string InputName
        {
            get { return _inputName; }
            set { _inputName = value; OnPropertyChanged("InputName"); }
        }
        private string _inputName = string.Empty;

        public string InputPriceText
        {
            get { return _inputPriceText; }
            set { _inputPriceText = value; OnPropertyChanged("InputPriceText"); }
        }
        private string _inputPriceText = string.Empty;

        private double inputPrice
        {
            get { return Convert.ToDouble(InputPriceText); }
        }

        public List<MaterialSettings> MaterialNameList
        {
            get { return AppSettings.MaterialList.Values.ToList(); }
        }

        public MaterialSettings SelectedMaterial
        {
            get { return _selectedMaterial; }
            set { _selectedMaterial = value; OnPropertyChanged("SelectedMaterial"); }
        }
        private MaterialSettings _selectedMaterial;

        public string MaterialText
        {
            get { return _materialText; }
            set { _materialText = value; OnPropertyChanged("MaterialText"); }
        }
        private string _materialText = string.Empty;

        public string SizeText
        {
            get { return _sizeText; }
            set { _sizeText = value; OnPropertyChanged("SizeText"); }
        }
        private string _sizeText = string.Empty;

        public string Processing0Text
        {
            get { return _processing0Text; }
            set { _processing0Text = value; OnPropertyChanged("Processing0Text"); }
        }
        private string _processing0Text = string.Empty;

        public ObservableCollection<ColorItemModel> Colors
        {
            get { return ColorSettingsVM.ColorSettings.Items; }
        }

        public ColorItemModel SelectedColor { get; set; }

        public string SelectedColorText 
        {
            get { return _SelectedColorText; }
            set { _SelectedColorText = value; OnPropertyChanged("SelectedColorText"); }
        }
        private string _SelectedColorText = string.Empty;

        public ObservableCollection<ColorItemModel> ColorList
        {
            get { return _ColorList; }
            set { _ColorList = value; OnPropertyChanged("ColorList"); }
        }
        private ObservableCollection<ColorItemModel> _ColorList = new ObservableCollection<ColorItemModel>();

        public ColorItemModel SelectedColorItem
        {
            get { return _SelectedColorItem; }
            set { _SelectedColorItem = value; OnPropertyChanged("SelectedColorItem"); }
        }
        private ColorItemModel _SelectedColorItem;
        #endregion

        #region commands

        public ICommand CreateItemCommand
        {
            get
            {
                return new ActiveDelegateCommand<BasicUnitPriceVM>(this,
                    (vm) => { CreateItem(); },
                    (vm) => { return CanCreateItem(); });
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return new SelectionDelegateCommand<BasicUnitPriceVM>(this,
                    (vm) => { UpdateItem(); },
                    (vm) => { return OnlyOneItemSelected() && IsNumericText(InputPriceText); });
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new SelectionDelegateCommand<BasicUnitPriceVM>(this,
                    (vm) => { DeleteItem(); },
                    (vm) => { return OnlyOneItemSelected(); });
            }
        }

        public ICommand ClearCommand
        {
            get { return new RelayCommand<object>((p) => { ClearInput(); }); }
        }

        public ICommand DetailChangedCommand
        {
            get { return new RelayCommand(DetailChanged); }
        }

        public ICommand IncreaseColorCommand
        {
            get
            {
                return new ActiveDelegateCommand<BasicUnitPriceVM>(this,
                    (p) => { ColorList.Add(SelectedColor); SelectedColorText = string.Empty; },
                    (p) => { return SelectedColor != null && !ColorList.Contains(SelectedColor); });
            }
        }

        public ICommand DecreaseColorCommand
        {
            get
            {
                return new ActiveDelegateCommand<BasicUnitPriceVM>(this,
                    (p) => { ColorList.Remove(SelectedColorItem); },
                    (p) => { return SelectedColorItem != null; });
            }
        }


        #endregion

        #region methods

        protected override bool FilterItem(UnitPriceItemModel item)
        {
            return !item.IsDeleted && !item.IsCombined && item.CustomerCode == this._customerType;
        }

        private static List<BasicUnitPriceVM> Initialize()
        {
            _units = new List<BasicUnitPriceVM>();

            foreach (CustomerSettings item in AppSettings.CustomerList.Values)
            {
                _units.Add(new BasicUnitPriceVM(new UnitPriceListModel(), item.Code));
            }
            return _units;
        }

        private static bool IsNumericText(string text)
        {
            return new Regex("^[0-9]+[.]?[0-9]+$").IsMatch(text) || new Regex("^[0-9]+$").IsMatch(text);
        }
        
        private bool CanCreateItem()
        {
            if (string.IsNullOrEmpty(productCode) && !string.IsNullOrEmpty(_inputName) && IsNumericText(_inputPriceText))
            {
                return true;
            }
            return false;
        }

        private void CreateItem()
        {
            UnitPriceItemModel item = new UnitPriceItemModel()
            {
                Code = UnitPriceItemModel.GetNewProductCode(_listModel, _customerType, false),
                Name = _inputName,
                Price = inputPrice,
                CustomerCode = _customerType,
                MaterialCode = (SelectedMaterial == null ? string.Empty : SelectedMaterial.Code),
                Size = SizeText,
                Processing0 = Processing0Text,
                IsCombined = false,
                IsDeleted = false,
                ColorTypes = ColorSettingsVM.ColorItemToCode(ColorList.ToList()),
            };
            this._listModel.Add(item);
            UnitPriceDao.Create(item);
            ClearInput();
        }

        private void UpdateItem()
        {
            SelectedItem.Name = _inputName;
            SelectedItem.Price = inputPrice;
            SelectedItem.MaterialCode = (SelectedMaterial == null ? string.Empty : SelectedMaterial.Code);
            SelectedItem.Size = SizeText;
            SelectedItem.Processing0 = Processing0Text;
            SelectedItem.ColorTypes = ColorSettingsVM.ColorItemToCode(ColorList.ToList());
            UnitPriceDao.Update(SelectedItem);
            ClearInput();
        }

        private bool OnlyOneItemSelected()
        {
            if (this.SelectedItems != null)
            {
                if (this.SelectedItems.Count == 1)
                {
                    return true;
                }
            }
            return false;
        }

        private void DeleteItem()
        {
            SelectedItem.IsDeleted = true;
            UnitPriceDao.Delete(SelectedItem);
            this.ApplyFilter();
            ClearInput();
        }

        private void ClearInput(bool flag = true)
        {
            InputName = string.Empty;
            InputPriceText = string.Empty;
            ProductCode = string.Empty;
            MaterialText = string.Empty;
            SizeText = string.Empty;
            Processing0Text = string.Empty;
            SelectedColorText = string.Empty;
            ColorList.Clear();
            if (flag)
            {
                SelectedItem = null;
            }
        }

        private void DetailChanged()
        {
            if (OnlyOneItemSelected())
            {
                InputName = this.SelectedItem.Name;
                InputPriceText = this.SelectedItem.Price.ToString();
                ProductCode = this.SelectedItem.Code;
                MaterialText = this.SelectedItem.MaterialName;
                SizeText = this.SelectedItem.Size;
                Processing0Text = this.SelectedItem.Processing0;
                ColorList = new ObservableCollection<ColorItemModel>(ColorSettingsVM.CodeToColorItemModel(this.SelectedItem.ColorTypes));
            }
            else
            {
                ClearInput(false);
            }
        }
        #endregion
    }
}
