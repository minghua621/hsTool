using System;
using System.ComponentModel;
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
    public class ColorItem
    {
        public ColorItemModel item { get; set; }
        public string codeAid { get; set; }
        public string amount { get; set; }
    }

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

        public ObservableCollection<MaterialItemModel> MaterialNameList
        {
            get { return MaterialSettingsVM.MaterialSettings.Items; }
        }

        public MaterialItemModel SelectedMaterial
        {
            get { return _selectedMaterial; }
            set { _selectedMaterial = value; OnPropertyChanged("SelectedMaterial"); }
        }
        private MaterialItemModel _selectedMaterial;

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

        public string PieceWeightText
        {
            get { return _pieceWeightText; }
            set { _pieceWeightText = value; OnPropertyChanged("PieceWeightText"); }
        }
        private string _pieceWeightText = string.Empty;

        public string PackageText
        {
            get { return _packageText; }
            set { _packageText = value; OnPropertyChanged("PackageText"); }
        }
        private string _packageText = string.Empty;

        public ICollectionView Colors
        {
            get { return ColorSettingsVM.ColorSettings.ItemsView; }
        }

        public AutoCompleteFilterPredicate<object> ColorFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as ColorItemModel).Code.StartsWith(searchText)
                    || (obj as ColorItemModel).Name.Contains(searchText);
            }
        }

        public ColorItemModel SelectedColor 
        {
            get { return _SelectedColor; }
            set { _SelectedColor = value; OnPropertyChanged("SelectedColor"); }
        }
        private ColorItemModel _SelectedColor = null;

        public string SelectedColorText 
        {
            get { return _SelectedColorText; }
            set { _SelectedColorText = value; OnPropertyChanged("SelectedColorText"); }
        }
        private string _SelectedColorText = string.Empty;

        public ObservableCollection<ColorItem> ColorList
        {
            get { return _ColorList; }
            set { _ColorList = value; OnPropertyChanged("ColorList"); }
        }
        private ObservableCollection<ColorItem> _ColorList = new ObservableCollection<ColorItem>();

        public ColorItem SelectedColorItem
        {
            get { return _SelectedColorItem; }
            set { _SelectedColorItem = value; OnPropertyChanged("SelectedColorItem"); }
        }
        private ColorItem _SelectedColorItem;

        public string ColorCodeAid
        {
            get { return _ColorCodeAid; }
            set { _ColorCodeAid = value; OnPropertyChanged("ColorCodeAid"); }
        }
        private string _ColorCodeAid = string.Empty;

        public string ColorAmount
        {
            get { return _ColorAmount; }
            set { _ColorAmount = value; OnPropertyChanged("ColorAmount"); }
        }
        private string _ColorAmount = string.Empty;
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

        private bool IsExistedColor()
        {
            bool rlt = false;
            foreach (ColorItem color in ColorList)
            {
                if (color.item == SelectedColor)
                {
                    rlt = true;
                    break;
                }
            }
            return rlt;
        }

        public ICommand IncreaseColorCommand
        {
            get
            {
                return new ActiveDelegateCommand<BasicUnitPriceVM>(this,
                    (p) => { ColorList.Add(new ColorItem() { item = SelectedColor, codeAid = ColorCodeAid, amount = ColorAmount }); SelectedColorText = string.Empty; ColorCodeAid = string.Empty; ColorAmount = string.Empty; },
                    (p) => { return SelectedColor != null && !IsExistedColor(); });
            }
        }

        public ICommand DecreaseColorCommand
        {
            get
            {
                return new ActiveDelegateCommand<BasicUnitPriceVM>(this,
                    (p) =>
                    {
                        SelectedColor = SelectedColorItem.item; 
                        ColorCodeAid = SelectedColorItem.codeAid;
                        ColorAmount = SelectedColorItem.amount;
                        ColorList.Remove(SelectedColorItem);
                    },
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

            foreach (CustomerItemModel item in CustomerSettinigsVM.CustomerSettinigs.Items)
            {
                _units.Add(new BasicUnitPriceVM(UnitPriceListModel.Units.FirstOrDefault(x => x._customerCode == item.Code), item.Code));
            }
            return _units;
        }

        public static bool IsNumericText(string text)
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

        private static string ColorItemToString(List<ColorItem> items)
        {
            string rlt = string.Empty;
            string delimiter = "^";

            foreach (var color in items.OrderBy(x => x.item.Code))
            {
                rlt += string.Format("{0}{1}{2}{3}{4}{5}{6},", color.item.Code, delimiter, color.item.Name, delimiter, color.codeAid, delimiter, color.amount);
            }
            return rlt;
        }

        public static List<ColorItem> StringToColorItem(string colorList)
        {
            string delimiter = "^";
            List<ColorItem> rlt = new List<ColorItem>();

            if (!string.IsNullOrEmpty(colorList))
            {
                string[] colors = colorList.Split(",".ToCharArray());
                foreach (string color in colors)
                {
                    if (!string.IsNullOrEmpty(color))
                    {
                        string[] colorString = color.Split(delimiter.ToCharArray());
                        ColorItemModel itemModel = Settings.ViewModels.ColorSettingsVM.ColorSettings.Items.FirstOrDefault(x => x.Code == colorString[0] && x.Name == colorString[1]);
                        if (itemModel != null)
                        {
                            rlt.Add(new ColorItem() { item = itemModel, codeAid = colorString[2], amount = colorString[3] });
                        }
                    }
                }
            }
            return rlt;
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
                PieceWeight = PieceWeightText,
                Package = PackageText,
                IsCombined = false,
                IsDeleted = false,
                ColorTypes = ColorItemToString(ColorList.ToList()),
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
            SelectedItem.PieceWeight = PieceWeightText;
            SelectedItem.Package = PackageText;
            SelectedItem.ColorTypes = ColorItemToString(ColorList.ToList());
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
            PieceWeightText = string.Empty;
            PackageText = string.Empty;
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
                PieceWeightText = this.SelectedItem.PieceWeight;
                PackageText = this.SelectedItem.Package;
                ColorList = new ObservableCollection<ColorItem>(StringToColorItem(this.SelectedItem.ColorTypes));
            }
            else
            {
                ClearInput(false);
            }
        }
        #endregion
    }
}
