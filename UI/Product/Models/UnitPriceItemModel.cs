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
        private string _Name = string.Empty;
        public double Price 
        {
            get { return _Price; }
            set { _Price = value; OnPropertyChanged("Price"); }
        }
        private double _Price = 0;
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
        private string _MaterialCode = string.Empty;
        public string MaterialName
        {
            get { return AppSettings.MaterialList.Keys.Contains(MaterialCode) ? AppSettings.MaterialList[MaterialCode].Name : string.Empty; }
        }
        public string Size
        {
            get { return _Size; }
            set { _Size = value; OnPropertyChanged("Size"); }
        }
        private string _Size = string.Empty;
        public string Processing0
        {
            get { return _Processing0; }
            set { _Processing0 = value; OnPropertyChanged("Processing0"); }
        }
        private string _Processing0 = string.Empty;
        public bool IsCombined { get; set; }
        public string CombinedUnits
        {
            get { return _CombinedUnits; }
            set { _CombinedUnits = value; OnPropertyChanged("CombinedUnits"); }
        }
        private string _CombinedUnits;
        public bool IsDeleted { get; set; }

        #endregion

        #region methods

        public static string GetNewProductCode(ObservableCollection<UnitPriceItemModel> items, string customer, bool isCombined)
        {
            UnitPriceItemModel last = items.Where(x => x.CustomerCode == customer && x.IsCombined == isCombined).OrderByDescending(x => x.Code).FirstOrDefault();
            int newId = 0;
            if (last != null)
            {
                newId = Convert.ToInt32(last.Code.Substring(4)) + 1;
            }
            return string.Format("{0}{1}{2:D4}", customer, isCombined ? "1" : "0", newId);
        }
        #endregion
    }
}
