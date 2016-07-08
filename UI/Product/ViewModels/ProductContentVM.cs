using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using UI.Main;

namespace UI.Product.ViewModels
{
    public enum ProductPanelType
    {
        BasicUnitPrice = 0,
        CombinedUnitPrice = 1,
    }

    public class ProductContentVM : NotificationBase
    {
        #region properties

        /// <summary>
        /// current product view
        /// </summary>
        public ProductPanelType State
        {
            get { return _state; }
            set 
            { 
                if(_state!=value)
                {
                    _state = value;
                    OnPropertyChanged("State");
                    OnPropertyChanged("ShowBasicUP");
                    OnPropertyChanged("ShowCombinedUP");
                }
            }
        }
        private ProductPanelType _state = ProductPanelType.BasicUnitPrice;

        /// <summary>
        /// display BasicUnitPriceView or not
        /// </summary>
        public bool ShowBasicUP 
        {
            get { return State == ProductPanelType.BasicUnitPrice; }
            set { State = ProductPanelType.BasicUnitPrice; }
        }

        /// <summary>
        /// display CombinedUnitPriceView or not
        /// </summary>
        public bool ShowCombinedUP
        {
            get { return State == ProductPanelType.CombinedUnitPrice; }
            set { State = ProductPanelType.CombinedUnitPrice; }
        }

        #endregion
    }
}
