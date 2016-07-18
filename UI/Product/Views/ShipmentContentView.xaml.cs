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
    /// Interaction logic for ShipmentContentView.xaml
    /// </summary>
    public partial class ShipmentContentView : UserControl
    {
        public ShipmentContentView()
        {
            InitializeComponent();
            this.Loaded += ShipmentContentView_Loaded;
        }

        private void ShipmentContentView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ShipmentContentView_Loaded;
            int count = 0;
            foreach (CustomerSettings item in AppSettings.CustomerList.Values)
            {
                Border bd = new Border();
                tabControl.Items.Add(new TabItem() { Header = item.Name, Content = bd });
            }        
        }
    }
}
