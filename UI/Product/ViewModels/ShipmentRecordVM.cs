using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        public string _customerType { get; private set; }

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

        /// <summary>
        /// 包含國際色號or包含顏色名
        /// </summary>
        public static AutoCompleteFilterPredicate<object> ColorFilter
        {
            get { return ColorSettingsVM.ColorFilter; }
        }
        
        public List<ColorItemModel> ColorItems
        {
            get
            {
                if (IsSample)
                {
                    return ColorSettingsVM.GetColors(SelectedColorText);
                }
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
                    return colors.Select(x => x.item).Distinct().ToList();
                }
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
            set { _SelectedColorText = value; OnPropertyChanged("SelectedColorText"); OnPropertyChanged("ColorItems"); }
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

        public bool IsSample
        {
            get { return _isSample; }
            set
            {
                _isSample = value;
                OnPropertyChanged("IsSample");
                OnPropertyChanged("ColorItems");

                if (IsSample)
                {
                    SelectedInputText = string.Empty;
                    SelectedColorText = string.Empty;
                }
                else
                {
                    SampleName = string.Empty;
                    SamplePriceText = string.Empty;
                }
            }
        }
        private bool _isSample = false;

        public string SampleName
        {
            get { return _SampleName; }
            set { _SampleName = value; OnPropertyChanged("SampleName"); }
        }
        private string _SampleName = string.Empty;

        public string SamplePriceText
        {
            get { return _SamplePriceText; }
            set { _SamplePriceText = value; OnPropertyChanged("SamplePriceText"); }
        }
        private string _SamplePriceText = string.Empty;

        private bool _isShipMonth = true;
        public bool IsShipMonth
        {
            get { return _isShipMonth; }
            set 
            { 
                _isShipMonth = value; 
                OnPropertyChanged("IsShipMonth"); 
                if(IsShipMonth)
                {
                    periodStart = _shipMonth;
                    periodEnd = _shipMonth.AddMonths(1);
                }
                else
                {
                    periodStart = _shiptDateStart;
                    periodEnd = _shiptDateEnd.AddDays(1);
                }
                this.ApplyFilter();
                UpdateSumText();
            }
        }

        private DateTime periodStart = DateTime.Now;
        private DateTime periodEnd = DateTime.Now;

        /// <summary>
        /// 出貨月
        /// </summary>
        public DateTime ShipMonth
        {
            get { return _shipMonth; }
            set
            {
                _shipMonth = value;
                OnPropertyChanged("ShipMonth");
                periodStart = _shipMonth;
                periodEnd = _shipMonth.AddMonths(1);
                this.ApplyFilter();
                UpdateSumText();
            }
        }
        private DateTime _shipMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        /// <summary>
        /// 出貨起始日
        /// </summary>
        public DateTime ShiptDateStart
        {
            get { return _shiptDateStart; }
            set
            {
                _shiptDateStart = value; 
                OnPropertyChanged("ShiptDateStart");
                periodStart = _shiptDateStart;
                this.ApplyFilter();
                UpdateSumText();
            }
        }
        private DateTime _shiptDateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        /// <summary>
        /// 出貨結束日
        /// </summary>
        public DateTime ShiptDateEnd
        {
            get { return _shiptDateEnd; }
            set
            {
                _shiptDateEnd = value;
                OnPropertyChanged("ShiptDateEnd");
                periodEnd = _shiptDateEnd.AddDays(1);
                this.ApplyFilter();
                UpdateSumText();
            }
        }
        private DateTime _shiptDateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

        /// <summary>
        /// 小計金額總和
        /// </summary>
        private double SubTotalSum
        {
            get { return _subTotalSum; }
            set { _subTotalSum = value; OnPropertyChanged("SumText"); }

        }
        private double _subTotalSum = 0;

        public string SumText
        {
            get { return string.Format("{0}: {1}", ApplicationStrings.header_ship_subtotal, SubTotalSum); }
        }

        #endregion

        #region commands

        public ICommand CreateItemCommand
        {
            get
            {
                return new ActiveDelegateCommand<ShipmentRecordVM>(this, (p) => { CreateItem(); },
                    (p) => { return ((!IsSample && SelectedInput != null) || (IsSample && !string.IsNullOrEmpty(SampleName) && BasicUnitPriceVM.IsNumericText(SamplePriceText))) && IsIntText(ShipQtyText); });
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return new SelectionDelegateCommand<ShipmentRecordVM>(this,
                    (vm) => { UpdateItem(); },
                    (vm) =>
                    {
                        return OnlyOneItemSelected() && ((!SelectedItem.IsSample && SelectedInput != null)
                            || (SelectedItem.IsSample && !string.IsNullOrEmpty(SampleName) && BasicUnitPriceVM.IsNumericText(SamplePriceText))) && IsIntText(ShipQtyText);
                    });
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
            return item.CustomerCode == this._customerType && item.ShipDate >= this.periodStart && item.ShipDate < this.periodEnd;
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
                ProductCode = IsSample ? SampleName.Trim() : SelectedInput.Code,
                UnitPrice = IsSample ? Convert.ToDouble(SamplePriceText) : SelectedInput.Price,
                ShipQty = this.InputQty,
                ShipDate = this.InputDate,
                ColorCode = SelectedColor == null ? string.Empty : SelectedColor.Code,
                ColorName = SelectedColor == null ? string.Empty : SelectedColor.Name,
                IsSample = this.IsSample
            };
            long id = ShipmentDao.Create(item);
            item.SerialNumber = id;
            this._listModel.Add(item);
            ClearInput();
        }

        private void UpdateItem()
        {
            SelectedItem.ProductCode = IsSample ? SampleName.Trim() : SelectedInput.Code;
            SelectedItem.UnitPrice = SelectedItem.IsSample ? Convert.ToDouble(SamplePriceText) : SelectedInput.Price;
            SelectedItem.ShipQty = this.InputQty;
            SelectedItem.ShipDate = this.InputDate;
            SelectedItem.ColorCode = SelectedColor == null ? string.Empty : SelectedColor.Code;
            SelectedItem.ColorName = SelectedColor == null ? string.Empty : SelectedColor.Name;
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
            SampleName = string.Empty;
            SamplePriceText = string.Empty;
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
                if (SelectedItem.IsSample)
                {
                    IsSample = true;
                    SampleName = SelectedItem.ProductName;
                    SamplePriceText = SelectedItem.UnitPrice.ToString();
                }
                else
                {
                    IsSample = false;
                    SelectedInputText = SelectedItem.ProductName;                    
                    unitPrice = SelectedItem.UnitPrice;
                }
                SelectedColorText = string.IsNullOrEmpty(SelectedItem.ColorCode) ? SelectedItem.ColorName : SelectedItem.ColorCode;
                ShipQtyText = SelectedItem.ShipQty.ToString();
                InputDate = SelectedItem.ShipDate;
            }
            else
            {
                ClearInput(false);
            }
            OnPropertyChanged("ProductInfo");
        }

        public void UpdateColorItems()
        {
            OnPropertyChanged("ColorItems");
        }

        private void UpdateSumText()
        {
            SubTotalSum = Math.Round(this.Items.Sum(x => x.SubTotal), MidpointRounding.AwayFromZero);
        }
        #endregion
    }
}
