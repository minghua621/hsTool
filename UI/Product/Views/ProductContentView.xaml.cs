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
using UI.Product.ViewModels;

namespace UI.Product.Views
{
    /// <summary>
    /// Interaction logic for ProductContentView.xaml
    /// </summary>
    public partial class ProductContentView : UserControl
    {
        public ProductContentView()
        {
            InitializeComponent();
            this.DataContext = new ProductContentVM();           
        }
    }
}
