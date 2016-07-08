using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Product.Models
{
    public class UnitPriceItemModel
    {
        #region properties

        public string Code { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Customer { get; set; }
        public string Material { get; set; }
        public bool IsCombined { get; set; }
        public ObservableCollection<UnitPriceItemModel> CombinedList { get; set; }

        #endregion
    }
}
