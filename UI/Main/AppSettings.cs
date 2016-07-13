using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

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
                        case "Customer":
                            foreach (var item in section.Elements())
                            {
                                CustomerSettings cus = new CustomerSettings() { Code = item.Attribute(XName.Get("Key")).Value, Name = item.Attribute(XName.Get("Value")).Value };
                                customerList.Add(cus.Code, cus);
                            }
                            break;
                        case "Material":
                            foreach (var item in section.Elements())
                            {
                                MaterialSettings material = new MaterialSettings() { Code = item.Attribute(XName.Get("Key")).Value, Name = item.Attribute(XName.Get("Value")).Value };
                                materialList.Add(material.Code, material);
                            }
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

        public static Dictionary<string, CustomerSettings> CustomerList
        {
            get { return customerList; }
        }
        private static Dictionary<string, CustomerSettings> customerList = new Dictionary<string, CustomerSettings>();

        public static Dictionary<string, MaterialSettings> MaterialList
        {
            get { return materialList; }
        }
        private static Dictionary<string, MaterialSettings> materialList = new Dictionary<string, MaterialSettings>();
    }

    public class CustomerSettings
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class MaterialSettings
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
