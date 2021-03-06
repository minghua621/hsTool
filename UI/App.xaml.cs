﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Common.Logger;
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

            string config = @"D:\home\hsTool\AppSettings.xml";
            bool isTest = e.Args.Count() > 0 && e.Args[0] == "-test" ? true : false;

            AppSettings.Initialize(config, isTest);
            Log.Initialize(string.Format("{0}{1}.txt", AppSettings.LogDir, DateTime.Now.ToString("yyyyMMdd")));            
            Log.Info("Application Start");

            /*
            if (!GoogleDrive.Settings.Initialize(AppSettings.GoogleDriveKey))
            {
                Log.Error("Fail to update and backup");
            }
            else
            {
                //check DB
                bool rlt = GoogleDrive.BackupAndUpdate.UpdateDB(AppSettings.DBPath);

                //check program

                //check settings
            }*/
            this.MainWindow.Title = "hsTool";
            this.MainWindow.Icon = this.Resources["application_icon"] as System.Windows.Media.Imaging.BitmapImage;
            this.MainWindow.WindowState = WindowState.Maximized;
            this.MainWindow.Content = new Main.Views.RootFrameView();            
            this.MainWindow.Show();
        }
    }
}
