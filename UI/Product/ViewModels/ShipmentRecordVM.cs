using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Command;
using Common.ViewModels;
using UI.Product.Models;
using UI.Product.Dao;
using UI.Main;
using UI.Settings.Models;
using UI.Settings.ViewModels;
using UI.Localization.Messages;

namespace UI.Product.ViewModels
{
    public class ShipmentRecordVM : ListViewModel<ShipmentListModel, ShipmentItemModel>
    {
        #region constructor
        public ShipmentRecordVM(ShipmentListModel _listMoodel, string customerCode)
            : base(_listMoodel)
        {
            _customerType = customerCode;
            this.ApplyFilter();
        }
        #endregion

        #region fields

        private string _customerType { get; set; }

        public static List<ShipmentRecordVM> Units
        {
            get { return _units = _units ?? Initialize(); }
        }
        private static List<ShipmentRecordVM> _units;

        public ObservableCollection<UnitPriceItemModel> ProductItems
        {
            get { return UnitPriceListModel.Units.FirstOrDefault(x => x._customerCode == this._customerType); }
        }

        public UnitPriceItemModel SelectedInput
        {
            get { return _SelectedInput; }
            set
            {
                _SelectedInput = value;
                if (_SelectedInput != null)
                {
                    unitPrice = _SelectedInput.Price;
                }
                OnPropertyChanged("SelectedInput");
                OnPropertyChanged("ProductInfo");
                OnPropertyChanged("ColorItems");
            }
        }
        private UnitPriceItemModel _SelectedInput = null;

        public string SelectedInputText
        {
            get { return _SelectedInputText; }
            set { _SelectedInputText = value; OnPropertyChanged("SelectedInputText"); }
        }
        private string _SelectedInputText = string.Empty;

        public string ProductInfo
        {
            get
            {
                string rlt = string.Empty;
                if (SelectedInput != null)
                {
                    rlt += string.Format("{0}: {1}\n", ApplicationStrings.header_customer_name, SelectedInput.CustomerName);
                    rlt += string.Format("{0}: {1}\n", ApplicationStrings.header_product_code, SelectedInput.Code);
                    rlt += string.Format("{0}: {1}\n", ApplicationStrings.header_unit_price, unitPrice);
                    rlt += string.IsNullOrEmpty(SelectedInput.MaterialName) ? "" : string.Format("{0}: {1}\n", ApplicationStrings.header_material, SelectedInput.MaterialName);
                    rlt += string.IsNullOrEmpty(SelectedInput.Size) ? "" : string.Format("{0}: {1}\n", ApplicationStrings.header_size, SelectedInput.Size);
                    rlt += string.IsNullOrEmpty(SelectedInput.Processing0) ? "" : string.Format("{0}: {1}\n", ApplicationStrings.header_processing, SelectedInput.Processing0);
                }
                return rlt;
            }
        }

        private double unitPrice { get; set; }

        public List<ColorItem> ColorItems
        {
            get
            {
                if (SelectedInput == null)
                {
                    return null;
                }
                else
                {
                    List<ColorItem> colors = new List<ColorItem>();
                    if (SelectedInput.IsCombined == true)
                    {
                        string[] combines = SelectedInput.CombinedUnits.Split(",".ToCharArray());
                        UnitPriceListModel list = UnitPriceListModel.Units.FirstOrDefault(x => x._customerCode == this._customerType);
                        if (list != null)
                        {
                            foreach (string str in combines)
                            {
                                UnitPriceItemModel unit = list.FirstOrDefault(x => x.Code == str);
                                if (unit != null)
                                {
                                    List<ColorItem> rlt = BasicUnitPriceVM.StringToColorItem(unit.ColorTypes);
                                    foreach(ColorItem color in rlt)
                                    {
                                        if(!colors.Contains(color))
                                        {
                                            colors.Add(color);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        colors = BasicUnitPriceVM.StringToColorItem(SelectedInput.ColorTypes);
                    }
                    return colors;
                }
            }
        }

        public ColorItem SelectedColor
        {
            get { return _SelectedColor; }
            set { _SelectedColor = value; OnPropertyChanged("SelectedColor"); }
        }
        private ColorItem _SelectedColor = null;

        public string SelectedColorText
        {
            get { return _SelectedColorText; }
            set { _SelectedColorText = value; OnPropertyChanged("SelectedColorText"); }
        }
        private string _SelectedColorText = string.Empty;

        public string ShipQtyText
        {
            get { return _ShipQtyText; }
            set { _ShipQtyText = value; OnPropertyChanged("ShipQtyText"); }
        }
        private string _ShipQtyText = string.Empty;

        private int InputQty
        {
            get 
            {
                int rlt = 0;
                if (int.TryParse(ShipQtyText, out rlt))
                {
                    return rlt;
                }
                else
                {
                    return 0;
                }                
            }
        }

        public DateTime InputDate
        {
            get { return _InputDate; }
            set { _InputDate = value; OnPropertyChanged("InputDate"); }
        }
        private DateTime _InputDate = DateTime.Today;

        #endregion

        #region commands

        public ICommand CreateItemCommand
        {
            get
            {
                return new ActiveDelegateCommand<ShipmentRecordVM>(this, (p) => { CreateItem(); },
                    (p) => { return SelectedInput != null && IsIntText(ShipQtyText); });
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return new SelectionDelegateCommand<ShipmentRecordVM>(this,
                    (vm) => { UpdateItem(); },
                    (vm) => { return OnlyOneItemSelected() && SelectedInput != null && IsIntText(ShipQtyText); });
            }
        }

        public ICommand DeleteCommand
        {
            get { return new SelectionDelegateCommand<ShipmentRecordVM>(this, (vm) => { DeleteItem(); }); }
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

        protected override bool FilterItem(ShipmentItemModel item)
        {
            return item.CustomerCode == this._customerType;
        }
        
        private static List<ShipmentRecordVM> Initialize()
        {
            _units = new List<ShipmentRecordVM>();

            foreach (CustomerItemModel item in CustomerSettinigsVM.CustomerSettinigs.Items)
            {
                _units.Add(new ShipmentRecordVM(new ShipmentListModel(), item.Code));
            }
            return _units;
        }

        public static bool IsIntText(string text)
        {
            return new Regex("^[0-9]+$").IsMatch(text);
        }

        private void CreateItem()
        {
            ShipmentItemModel item = new ShipmentItemModel()
            {
                CustomerCode = this._customerType,
                ProductCode = SelectedInput.Code,
                UnitPrice = SelectedInput.Price,
                ShipQty = this.InputQty,
                ShipDate = this.InputDate,
                ColorCode = SelectedColor == null ? string.Empty : SelectedColor.item.Code,
                ColorName = SelectedColor == null ? string.Empty : SelectedColor.item.Name,
            };
            long id = ShipmentDao.Create(item);
            item.SerialNumber = id;
            this._listModel.Add(item);
            ClearInput();
        }

        private void UpdateItem()
        {
            SelectedItem.UnitPrice = SelectedInput.Price;
            SelectedItem.ShipQty = this.InputQty;
            SelectedItem.ShipDate = this.InputDate;
            SelectedItem.ColorCode = SelectedColor == null ? string.Empty : SelectedColor.item.Code;
            SelectedItem.ColorName = SelectedColor == null ? string.Empty : SelectedColor.item.Name;
            ShipmentDao.Update(SelectedItem);
            ClearInput();
        }

        private void DeleteItem()
        {
            if (this.SelectedItems != null)
            {
                for (int i = SelectedItems.Count - 1; i >= 0; i--)
                {
                    ShipmentDao.Delete(((ShipmentItemModel)SelectedItems[i]).SerialNumber);
                    this._listModel.Remove((ShipmentItemModel)SelectedItems[i]);
                }
            }
            ClearInput();
        }

        private void ClearInput(bool flag = true)
        {
            SelectedInputText = string.Empty;
            ShipQtyText = string.Empty;
            SelectedColorText = string.Empty;
            if(flag)
            {
                SelectedItem = null;
            }
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

        private void DetailChanged()
        {
            if (OnlyOneItemSelected())
            {
                SelectedInputText = SelectedItem.ProductName;
                ShipQtyText = SelectedItem.ShipQty.ToString();                
                InputDate = SelectedItem.ShipDate;
                SelectedColorText = string.IsNullOrEmpty(SelectedItem.ColorCode) ? SelectedItem.ColorName : SelectedItem.ColorCode;
                unitPrice = SelectedItem.UnitPrice;
            }
            else
            {
                ClearInput(false);
            }
            OnPropertyChanged("ProductInfo");
        }
        #endregion
    }
}
