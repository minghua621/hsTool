using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Common.ViewModels;
using UI.Settings.Models;

namespace UI.Settings.ViewModels
{
    public class MaterialSettingsVM : ListViewModel<MaterialListModel, MaterialItemModel>
    {
        #region constructor
        public MaterialSettingsVM(MaterialListModel _listMoodel)
            : base(_listMoodel)
        {
            this.ApplyFilter();
        }
        #endregion

        #region fields
        public static MaterialSettingsVM MaterialSettings
        {
            get { return _MaterialSettings = _MaterialSettings ?? new MaterialSettingsVM(new MaterialListModel()); }
        }
        private static MaterialSettingsVM _MaterialSettings;
        #endregion

        #region methods
        protected override bool FilterItem(MaterialItemModel item)
        {
            return true;
        }
        #endregion
    }
}
