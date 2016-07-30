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
using UI.Product.Models;
using UI.Product.ViewModels;
using UI.Settings.Models;
using UI.Settings.ViewModels;

namespace UI.Product.Views
{
    /// <summary>
    /// Interaction logic for BasicUnitPriceHeaderView.xaml
    /// </summary>
    public partial class BasicUnitPriceHeaderView : UserControl
    {
        public BasicUnitPriceHeaderView()
        {
            InitializeComponent();

            this.Loaded += BasicUnitPriceHeaderView_Loaded;
        }

        private void BasicUnitPriceHeaderView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= BasicUnitPriceHeaderView_Loaded;
            int count = 0;
            foreach (CustomerItemModel item in CustomerSettinigsVM.CustomerSettinigs.Items)
            {
                Border bd = new Border();
                bd.Child = new BasicUintPriceView() { DataContext = BasicUnitPriceVM.Units[count++] };
                tabControl.Items.Add(new TabItem() { Header = item.Name, Content = bd });
            }
        }
    }
}
