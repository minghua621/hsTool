using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common.Command;
using Common.ViewModels;
using UI.Main;
using UI.Product.Models;

namespace UI.Product.ViewModels
{
    public enum CustomerType
    {
        C00,
        C01,
        C02,
    }

    public class BasicUnitPriceVM : ListViewModel<UnitPriceItemModel>
    {
        public BasicUnitPriceVM(CustomerType type)
        {
            _listType = type;

            for (int i = 0; i < 10; i++)
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                double tmp = random.Next(1, 9) * 0.15;
                this.Items.Add(new UnitPriceItemModel() { Name = string.Format("Unit{0}", i), Price = Math.Round(tmp, 2), Customer = CustomerName, IsCombined = false });
            }
        }

        #region fields

        private CustomerType _listType { get; set; }

        public string CustomerName
        {
            get 
            {
                return AppSettings.CustomerList[(int)_listType].Name;
            }
        }

        public string UnitName
        {
            get { return unitName; }
            set { unitName = value; OnPropertyChanged("UnitName"); }
        }
        private string unitName = string.Empty;

        public string UnitPriceText
        {
            get { return unitPriceText; }
            set { unitPriceText = value; OnPropertyChanged("UnitPriceText"); }
        }
        private string unitPriceText = string.Empty;

        private double unitPrice
        {
            get { return Convert.ToDouble(UnitPriceText); }
        }
        #endregion

        #region commands

        public ICommand CreateItemCommand
        {
            get { return new RelayCommand(CreateItem); }
        }
        private bool CanCreateItem()
        {
            if(string.IsNullOrEmpty(UnitName) || string.IsNullOrEmpty(UnitPriceText))
            {
                return false;
            }
            return true;
        }
        private void CreateItem()
        {
            if(CanCreateItem())
            {
                this.Items.Add(new UnitPriceItemModel() { Name = UnitName, Price = unitPrice, Customer = CustomerName, IsCombined = false });
                ClearInput();
            }
        }

        public ICommand ClearCommand
        {
            get { return new RelayCommand(ClearInput); }
        }

        private void ClearInput()
        {
            UnitName = string.Empty;
            UnitPriceText = string.Empty;
        }
        #endregion

        #region Methods
        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItems = (sender as DataGrid).SelectedItems.Cast<UnitPriceItemModel>().ToList();
        }
        #endregion
    }
}
