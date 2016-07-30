using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using UI.Settings.Models;

namespace UI.Main
{
    public static class AppSettings
    {
        public static bool Initialize(string configXml)
        {
            bool result = false;

            if (File.Exists(configXml))
            {
                string baseDirectory = Path.GetDirectoryName(configXml) + @"\";
                XDocument xd = XDocument.Load(configXml);
                foreach (var section in xd.Root.Elements())
                {
                    switch (section.Attribute(XName.Get("Key")).Value)
                    {
                        case "DB":
                            dbPath = baseDirectory + GetValue(section, "Path");
                            connectString = string.Format("Data source={0}", dbPath);
                            break;
                        case "Log":
                            logDir = baseDirectory + GetValue(section, "Path");
                            break;
                        default:
                            break;
                    }
                }
                result = true;
            }
            return result;
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
    }
}
