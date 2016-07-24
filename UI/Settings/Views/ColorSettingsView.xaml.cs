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
using UI.Settings.Models;
using UI.Settings.ViewModels;

namespace UI.Settings.Views
{
    /// <summary>
    /// Interaction logic for ColorSettingsView.xaml
    /// </summary>
    public partial class ColorSettingsView : UserControl
    {
        public ColorSettingsView()
        {
            InitializeComponent();
            this.DataContext = ColorSettingsVM.ColorSettings;
        }
    }
}
