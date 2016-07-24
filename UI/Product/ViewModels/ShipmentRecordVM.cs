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

        public string CustomerName
        {
            get { return AppSettings.CustomerList[_customerType].Name; }
        }

        public static List<ShipmentRecordVM> Units
        {
            get { return _units = _units ?? Initialize(); }
        }
        private static List<ShipmentRecordVM> _units;

        public List<UnitPriceItemModel> ProductItems
        {
            get { return _ProductItems = _ProductItems ?? UnitPriceDao.GetUnitPriceList().Where(x => x.IsDeleted == false && x.CustomerCode == _customerType).ToList(); }
        }
        private List<UnitPriceItemModel> _ProductItems;

        public UnitPriceItemModel SelectedInput
        {
            get { return _SelectedInput; }
            set
            {
                _SelectedInput = value; 
                OnPropertyChanged("SelectedInput");
                OnPropertyChanged("ProductCode");
                OnPropertyChanged("UnitPrice");
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

        public string ProductCode
        {
            get { return SelectedInput == null ? string.Empty : SelectedInput.Code; }
        }

        public double UnitPrice
        {
            get { return SelectedInput == null ? 0 : SelectedInput.Price; }
        }

        public List<ColorItemModel> ColorItems
        {
            get { return SelectedInput == null ? null : ColorSettingsVM.CodeToColorItemModel(SelectedInput.ColorTypes); }
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

            foreach (CustomerSettings item in AppSettings.CustomerList.Values)
            {
                _units.Add(new ShipmentRecordVM(new ShipmentListModel(), item.Code));
            }
            return _units;
        }

        private static bool IsIntText(string text)
        {
            return new Regex("^[0-9]+$").IsMatch(text);
        }

        private void CreateItem()
        {
            ShipmentItemModel item = new ShipmentItemModel()
            {
                CustomerCode = this._customerType,
                ProductCode = SelectedInput.Code,
                ProductName = SelectedInput.Name,
                UnitPrice = SelectedInput.Price,
                ShipQty = this.InputQty,
                ShipDate = this.InputDate,
                ColorCode = SelectedColor == null ? string.Empty : SelectedColor.Code,
            };
            long id = ShipmentDao.Create(item);
            item.SerialNumber = id;
            this._listModel.Add(item);
            ClearInput();
        }

        private void UpdateItem()
        {
            SelectedItem.ProductName = SelectedInput.Name;
            SelectedItem.UnitPrice = SelectedInput.Price;
            SelectedItem.ShipQty = this.InputQty;
            SelectedItem.ShipDate = this.InputDate;
            SelectedItem.ColorCode = SelectedColor == null ? string.Empty : SelectedColor.Code;
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
            }
            else
            {
                ClearInput(false);
            }
        }
        #endregion
    }
}
