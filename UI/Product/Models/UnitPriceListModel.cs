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
    public class UnitPriceListModel : ListModel<UnitPriceItemModel>
    {
        public string _customerCode { get; private set; }

        public UnitPriceListModel(string customer)
        {
            _customerCode = customer;
            LoadByCustomer();
        }

        public void LoadByCustomer()
        {
            foreach (var item in Dao.UnitPriceDao.GetUnitPriceList())
            {
                if (item.CustomerCode == _customerCode && item.IsDeleted == false)
                {
                    this.Add(item);
                }
            }
        }

        public static List<UnitPriceListModel> Units
        {
            get { return _units = _units ?? Initialize(); }
        }
        private static List<UnitPriceListModel> _units;

        private static List<UnitPriceListModel> Initialize()
        {
            _units = new List<UnitPriceListModel>();

            foreach (CustomerItemModel item in CustomerSettinigsVM.CustomerSettinigs.Items)
            {
                _units.Add(new UnitPriceListModel(item.Code));
            }
            return _units;
        }

        public override void Load()
        {
        }
    }
}
