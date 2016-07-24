using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using UI.Main;

namespace UI.Settings.Models
{
    public class ColorListModel : ListModel<ColorItemModel>
    {
        public override void Load()
        {
            List<ColorItemModel> rlt = Dao.SettingsDao.GetColorList();
            foreach (var item in rlt)
            {
                this.Add(item);
            }
        }
    }
}
