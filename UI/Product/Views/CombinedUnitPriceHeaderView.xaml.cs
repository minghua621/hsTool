using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI.Main;

namespace UI.Product.Views
{
    /// <summary>
    /// Interaction logic for CombinedUnitPriceHeaderView.xaml
    /// </summary>
    public partial class CombinedUnitPriceHeaderView : UserControl
    {
        public CombinedUnitPriceHeaderView()
        {
            InitializeComponent();

            this.Loaded += CombinedUnitPriceHeaderView_Loaded;
        }

        private void CombinedUnitPriceHeaderView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CombinedUnitPriceHeaderView_Loaded;
            foreach (CustomerSettings item in AppSettings.CustomerList.Values)
            {
                Border bd = new Border();
                bd.Child = new CombinedUnitPriceView();
                tabControl.Items.Add(new TabItem() { Header = item.Name, Content = bd });
            }
        }
    }
}
