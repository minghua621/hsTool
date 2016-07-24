using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace UI.Product.Models
{
    public class ShipmentListModel : ListModel<ShipmentItemModel>
    {
        public override void Load()
        {
            List<ShipmentItemModel> rlt = Dao.ShipmentDao.GetShipmentRecordList();
            foreach (var item in rlt)
            {
                this.Add(item);
            }
        }
    }
}
