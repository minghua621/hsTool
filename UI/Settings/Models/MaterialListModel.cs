using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace UI.Settings.Models
{
    public class MaterialListModel : ListModel<MaterialItemModel>
    {
        public override void Load()
        {
            foreach (var item in Dao.SettingsDao.GetMaterialList())
            {
                this.Add(item);
            }
        }
    }
}
