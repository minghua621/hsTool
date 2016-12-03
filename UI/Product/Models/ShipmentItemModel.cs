using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using UI.Main;
using UI.Settings.Models;
using UI.Settings.ViewModels;

namespace UI.Product.Models
{
    public class ShipmentItemModel : ItemModel
    {
        public long SerialNumber { get; set; }

        public string ProductCode
        {
            get { return _productCode; }
            set { _productCode = value; OnPropertyChanged("ProductName"); }
        }
        private string _productCode = string.Empty;

        public string ProductName
        {
            get
            {
                string rlt = string.Empty;
                if (IsSample)
                {
                    rlt = ProductCode;
                }
                else
                {
                    UnitPriceListModel list = UnitPriceListModel.Units.FirstOrDefault(x => x._customerCode == CustomerCode);
                    if (list != null)
                    {
                        UnitPriceItemModel unit = list.FirstOrDefault(x => x.Code == ProductCode);
                        if (unit != null)
                        {
                            rlt = unit.Name;
                        }
                    }
                }
                return rlt;
            }
        }

        public double UnitPrice
        {
            get { return _UnitPrice; }
            set { _UnitPrice = value; OnPropertyChanged("UnitPrice"); }
        }
        private double _UnitPrice = 0;

        public string CustomerCode { get; set; }
        public string CustomerName
        {
            get 
            {
                CustomerItemModel item = CustomerSettinigsVM.CustomerSettinigs.Items.FirstOrDefault(x => x.Code == CustomerCode);
                return item == null ? string.Empty : item.Name;
            }
        }

        public string ColorCode
        {
            get { return _ColorCode; }
            set { _ColorCode = value; OnPropertyChanged("ColorCode"); }
        }
        private string _ColorCode = string.Empty;

        public string ColorName
        {
            get { return _ColorName; }
            set { _ColorName = value; OnPropertyChanged("ColorName"); }
        }
        private string _ColorName = string.Empty;

        public DateTime ShipDate
        {
            get { return _ShipDate; }
            set { _ShipDate = value; OnPropertyChanged("ShipDate"); }
        }
        private DateTime _ShipDate;

        public int ShipQty
        {
            get { return _ShipQty; }
            set { _ShipQty = value; OnPropertyChanged("ShipQty"); OnPropertyChanged("SubTotal"); }        
        }
        private int _ShipQty = 0;

        public double SubTotal
        {
            get { return Math.Round(UnitPrice * ShipQty, MidpointRounding.AwayFromZero); }
        }

        public bool IsSample { get; set; }
    }
}
