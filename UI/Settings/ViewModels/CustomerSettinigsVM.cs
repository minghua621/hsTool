using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.ViewModels;
using UI.Settings.Models;

namespace UI.Settings.ViewModels
{
    public class CustomerSettinigsVM : ListViewModel<CustomerListModel, CustomerItemModel>
    {
        #region constructor
        public CustomerSettinigsVM(CustomerListModel _listMoodel)
            : base(_listMoodel)
        {
            this.ApplyFilter();
        }
        #endregion

        #region fields
        public static CustomerSettinigsVM CustomerSettinigs
        {
            get { return _CustomerSettinigs = _CustomerSettinigs ?? new CustomerSettinigsVM(new CustomerListModel()); }
        }
        private static CustomerSettinigsVM _CustomerSettinigs;
        #endregion

        #region methods
        protected override bool FilterItem(CustomerItemModel item)
        {
            return true;
        }
        #endregion
    }
}
