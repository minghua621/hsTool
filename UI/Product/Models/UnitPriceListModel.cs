using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace UI.Product.Models
{
    public class UnitPriceListModel : ListModel<UnitPriceItemModel>
    {
        public override void Load()
        {
            List<UnitPriceItemModel> rlt = Dao.UnitPriceDao.GetUnitPriceList();
            foreach (var item in rlt)
            {
                this.Add(item);
            }
        }
    }
}
