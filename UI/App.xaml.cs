using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UI.Main;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            Localization.ResourceWrapper.CurrentCultrue = "zh-TW";

            string config = @"C:\home\hsTool\AppSettings.xml";
            AppSettings.Initialize(config);            
            this.MainWindow.Width = 1080;
            this.MainWindow.Height = 640;
            this.MainWindow.Title = "hsTool";
            this.MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.MainWindow.Content = new Main.Views.RootFrameView();            
            this.MainWindow.Show();
        }
    }
}
