using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using UI.Settings.Models;
using Common.Logger;

namespace UI.Main
{
    public static class AppSettings
    {
        public static void Initialize(string configXml, bool isTest)
        {
            if (File.Exists(configXml))
            {
                string baseDirectory = Path.GetDirectoryName(configXml) + @"\";
                XDocument xd = XDocument.Load(configXml);
                foreach (var section in xd.Root.Elements())
                {
                    switch (section.Attribute(XName.Get("Key")).Value)
                    {
                        case "DB":
                            if (isTest)
                            {
                                dbPath = baseDirectory + GetValue(section, "TestPath");
                            }
                            else
                            {
                                dbPath = baseDirectory + GetValue(section, "Path");
                            }
                            connectString = string.Format("Data source={0}", dbPath);
                            break;
                        case "Log":
                            logDir = baseDirectory + GetValue(section, "Path");
                            break;
                        case "InvoiceTemplate":
                            invoiceTemplate = baseDirectory + GetValue(section, "Path");
                            break;
                        case "GoogleDriveKey":
                            googleDriveKey = baseDirectory + GetValue(section, "Path");
                            break;
                        case "Company":
                            company = GetCompanySetting(section);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static string GetValue(XElement section, string KeyValue)
        {
            string rlt = string.Empty;
            foreach (var item in section.Elements())
            {
                if (item.Attribute(XName.Get("Key")).Value == KeyValue)
                {
                    rlt = item.Attribute(XName.Get("Value")).Value;
                    break;
                }
            }
            return rlt;
        }

        private static CompanySetting GetCompanySetting(XElement section)
        {
            CompanySetting rlt = new CompanySetting();
            foreach (var item in section.Elements())
            {
                rlt.Name = item.Attribute(XName.Get("Name")).Value;
                rlt.FullName = item.Attribute(XName.Get("FullName")).Value;
                rlt.Phone = item.Attribute(XName.Get("Phone")).Value;
            }
            return rlt;
        }

        public static string DBPath
        {
            get { return dbPath; }
        }
        private static string dbPath = string.Empty;

        public static string ConnectString
        {
            get { return connectString; }
        }
        private static string connectString = string.Empty;

        public static string LogDir
        {
            get { return logDir; }
        }
        private static string logDir = string.Empty;

        public static string InvoiceTemplate
        {
            get { return invoiceTemplate; }
        }
        private static string invoiceTemplate = string.Empty;

        public static string GoogleDriveKey
        {
            get { return googleDriveKey; }
        }
        private static string googleDriveKey = string.Empty;

        public static CompanySetting Company
        {
            get { return company; }
        }
        private static CompanySetting company;
    }

    public class CompanySetting
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
    }
}
