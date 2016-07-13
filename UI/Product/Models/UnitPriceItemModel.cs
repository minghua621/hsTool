using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using UI.Main;

namespace UI.Product.Models
{
    public class UnitPriceItemModel : ItemModel
    {
        #region properties

        public string Code { get; set; }
        public string Name 
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged("Name"); }
        }
        private string _Name;
        public double Price 
        {
            get { return _Price; }
            set { _Price = value; OnPropertyChanged("Price"); }
        }
        private double _Price;
        public string CustomerCode { get; set; }
        public string CustomerName
        {
            get { return AppSettings.CustomerList[CustomerCode].Name; }
        }
        public string MaterialCode
        {
            get { return _MaterialCode; }
            set { _MaterialCode = value; OnPropertyChanged("MaterialName"); }
        }
        private string _MaterialCode;
        public string MaterialName
        {
            get { return AppSettings.MaterialList.Keys.Contains(MaterialCode) ? AppSettings.MaterialList[MaterialCode].Name : string.Empty; }
        }
        public string Size
        {
            get { return _Size; }
            set { _Size = value; OnPropertyChanged("Size"); }
        }
        private string _Size;
        public string Processing0
        {
            get { return _Processing0; }
            set { _Processing0 = value; OnPropertyChanged("Processing0"); }
        }
        private string _Processing0;
        public bool IsCombined { get; set; }
        public string CombinedUnits { get; set; }
        public bool IsDeleted { get; set; }

        #endregion
    }
}
