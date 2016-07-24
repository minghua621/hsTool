using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Common.Command;
using Common.ViewModels;
using UI.Localization.Messages;
using UI.Main;
using UI.Product.Models;
using UI.Product.Dao;

namespace UI.Product.ViewModels
{
    public class CombinedUnitPriceVM : ListViewModel<UnitPriceListModel, UnitPriceItemModel>
    {
        #region constructor
        public CombinedUnitPriceVM(UnitPriceListModel _listMoodel, string customerCode)
            : base(_listMoodel)
        {
            _customerType = customerCode;
            this.ApplyFilter();
        }
        #endregion

        #region fields

        private string _customerType { get; set; }
        public static List<CombinedUnitPriceVM> Units
        {
            get { return _units = _units ?? Initialize(); }
        }
        private static List<CombinedUnitPriceVM> _units;

        public ObservableCollection<UnitPriceItemModel> BasicUnits
        {
            get { return _BasicUnits; }
            set { _BasicUnits = value; OnPropertyChanged("BasicUnits"); }
        }
        private ObservableCollection<UnitPriceItemModel> _BasicUnits = new ObservableCollection<UnitPriceItemModel>();

        public ObservableCollection<UnitPriceItemModel> CombiningUnits
        {
            get { return _CombiningUnits; }
            set { _CombiningUnits = value; OnPropertyChanged("CombiningUnits"); }
        }
        private ObservableCollection<UnitPriceItemModel> _CombiningUnits = new ObservableCollection<UnitPriceItemModel>();

        public UnitPriceItemModel SelectedBasicItem
        {
            get { return _SelectedBasicItem; }
            set { _SelectedBasicItem = value; OnPropertyChanged("SelectedBasicItem"); }
        }
        private UnitPriceItemModel _SelectedBasicItem;
        private IList BasicItems;

        public UnitPriceItemModel SelectedCombining
        {
            get { return _SelectedCombining; }
            set { _SelectedCombining = value; OnPropertyChanged("SelectedCombining"); }
        }
        private UnitPriceItemModel _SelectedCombining;
        private IList CombiningItems;

        public string ProductCode
        {
            get { return productCode; }
            set { productCode = value; OnPropertyChanged("ProductCode"); }
        }
        private string productCode = string.Empty;

        public Visibility CodePanelVisibility
        {
            get { return _CodePanelVisibility; }
            set { _CodePanelVisibility = value; OnPropertyChanged("CodePanelVisibility"); }
        }
        private Visibility _CodePanelVisibility = Visibility.Collapsed;

        public string InputName
        {
            get { return _inputName; }
            set { _inputName = value; OnPropertyChanged("InputName"); }
        }
        private string _inputName = string.Empty;

        public double CombinedPrice
        {
            get
            {
                double price = 0;
                foreach(var item in CombiningUnits)
                {
                    price += item.Price;
                }
                return price;
            }
        }

        public string EditName
        {
            get { return _EditName; }
            set { _EditName = value; OnPropertyChanged("EditName"); }
        }
        private string _EditName = string.Empty;
        #endregion

        #region commands

        public ICommand IncreaseCommand
        {
            get
            {
                return new ActiveDelegateCommand<CombinedUnitPriceVM>(this,
                    (vm) => { Increase(); },
                    (vm) => { return SelectedBasicItem != null; });
            }
        }

        public ICommand DecreaseCommand
        {
            get
            {
                return new ActiveDelegateCommand<CombinedUnitPriceVM>(this,
                    (vm) => { Decrease(); },
                    (vm) => { return SelectedCombining != null; });
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                return new ActiveDelegateCommand<CombinedUnitPriceVM>(this,
                        (vm) => { Create(); },
                        (vm) => { return !string.IsNullOrEmpty(InputName) && CombiningUnits.Count > 1 && Items.FirstOrDefault(x => x.CombinedUnits == GetCombinedString()) == null; });
            }
        }

        public ICommand CopyCommand
        {
            get
            {
                return new ActiveDelegateCommand<CombinedUnitPriceVM>(this,
                        (vm) => { Copy(); },
                        (vm) => { return SelectedItem != null; });
            }
        }

        public ICommand EditCommand
        {
            get
            {
                return new SelectionDelegateCommand<CombinedUnitPriceVM>(this,
                        (vm) => { Edit(); },
                        (vm) => { return SelectedItem != null; });
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new SelectionDelegateCommand<CombinedUnitPriceVM>(this,
                    (vm) => { Delete(); },
                    (vm) => { return SelectedItem != null; });
            }
        }

        public ICommand ChangeListCommand
        {
            get
            {
                return new SelectionDelegateCommand<CombinedUnitPriceVM>(this,
                    (vm) =>
                    {
                        CodePanelVisibility = Visibility.Visible;
                        ProductCode = SelectedItem.Code;
                        InputName = string.Empty;
                        CombiningUnits.Clear();
                        Copy();
                    },
                    (vm) => { return SelectedItem != null; });
            }
        }

        public ICommand BasicItemsChangedCommand
        {
            get { return new RelayCommand<IList>(p => { BasicItems = p; }); }
        }

        public ICommand CombiningItemsChangedCommand
        {
            get { return new RelayCommand<IList>(p => { CombiningItems = p; }); }
        }

        public ICommand SelectedItemsChangedCommand
        {
            get
            {
                return new RelayCommand<object>((p) =>
                {
                    if (SelectedItem != null)
                    {
                        EditName = SelectedItem.Name;
                    }
                });
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return new ActiveDelegateCommand<CombinedUnitPriceVM>(this, (p) =>
                {
                    UnitPriceItemModel item = Items.FirstOrDefault(x => x.Code == ProductCode);
                    if (item != null)
                    {
                        string newUnits = GetCombinedString();
                        UnitPriceItemModel prev = Items.FirstOrDefault(x => x.CombinedUnits == newUnits);
                        if (prev != null)
                        {
                            if (prev != item)
                            {
                                MessageBox.Show(string.Format(MessageStrings.unitprice_same_units_warning, prev.Name));
                                return;
                            }
                        }
                        else
                        {
                            item.CombinedUnits = newUnits;
                            UnitPriceDao.Update(item);
                        }
                    }
                    ProductCode = string.Empty;
                    CombiningUnits.Clear();
                    CodePanelVisibility = Visibility.Collapsed;
                    OnPropertyChanged("CombinedPrice");
                }, (p) => CombiningUnits.Count > 1);
            }
        }
        #endregion

        #region methods

        private static List<CombinedUnitPriceVM> Initialize()
        {
            _units = new List<CombinedUnitPriceVM>();

            foreach (CustomerSettings item in AppSettings.CustomerList.Values)
            {
                _units.Add(new CombinedUnitPriceVM(new UnitPriceListModel(), item.Code) { BasicUnits = BasicUnitPriceVM.Units.FirstOrDefault(x => x._customerType == item.Code).Items });                
            }            
            return _units;
        }

        protected override bool FilterItem(UnitPriceItemModel item)
        {
            return !item.IsDeleted && item.IsCombined && item.CustomerCode == this._customerType;
        }

        private void Increase()
        {
            foreach (var item in BasicItems)
            {
                if (!CombiningUnits.Contains((UnitPriceItemModel)item))
                {
                    CombiningUnits.Add((UnitPriceItemModel)item);
                }
            }
            SelectedBasicItem = null;
            OnPropertyChanged("CombinedPrice");
        }

        private void Decrease()
        {
            for (int i = CombiningItems.Count - 1; i >= 0; i--)
            {
                CombiningUnits.Remove((UnitPriceItemModel)CombiningItems[i]);
            }
            SelectedCombining = null;
            OnPropertyChanged("CombinedPrice");
        }

        private string GetCombinedString()
        {
            string rlt = string.Empty;
            foreach (var item in CombiningUnits.OrderBy(x => x.Code))
            {
                rlt += item.Code + ",";
            }
            return rlt;
        }

        private void Create()
        {
            UnitPriceItemModel item = new UnitPriceItemModel()
            {
                Code = UnitPriceItemModel.GetNewProductCode(_listModel, _customerType, true),
                Name = _inputName,
                Price = CombinedPrice,
                CustomerCode = _customerType,
                IsCombined = true,
                CombinedUnits = GetCombinedString(),
                IsDeleted = false,
            };
            this._listModel.Add(item);
            UnitPriceDao.Create(item);
            CombiningUnits.Clear();
            InputName = string.Empty;
            OnPropertyChanged("CombinedPrice");
        }

        private void Copy()
        {
            string[] rlt = SelectedItem.CombinedUnits.Split(",".ToCharArray());
            for (int i = 0; i < rlt.Count(); i++)
            {
                UnitPriceItemModel item = BasicUnits.FirstOrDefault(x => x.Code == rlt[i]);
                if (item != null)
                {
                    if (!CombiningUnits.Contains(item))
                    {
                        CombiningUnits.Add(item);
                    }
                }
            }
            EditName = string.Empty;
            SelectedItem = null;
            OnPropertyChanged("CombinedPrice");
        }
        
        private void Edit()
        {
            SelectedItem.Name = EditName;
            UnitPriceDao.Update(SelectedItem);
            SelectedItem = null;
            EditName = string.Empty;
        }

        private void Delete()
        {
            SelectedItem.IsDeleted = true;
            UnitPriceDao.Delete(SelectedItem);
            this.ApplyFilter();
            SelectedItem = null;
            EditName = string.Empty;
        }
        #endregion
    }
}
