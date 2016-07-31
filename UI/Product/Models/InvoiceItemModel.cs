using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Product.Models
{
    public class InvoiceItemModel
    {
        public string CustomerCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName
        {
            get
            {
                string rlt = string.Empty;
                UnitPriceListModel list = UnitPriceListModel.Units.FirstOrDefault(x => x._customerCode == CustomerCode);
                if (list != null)
                {
                    UnitPriceItemModel unit = list.FirstOrDefault(x => x.Code == ProductCode);
                    if (unit != null)
                    {
                        rlt = unit.Name;
                    }
                }
                return rlt;
            }
        }
        public double Price { get; set; }
        public int Qty { get; set; }
        public double SubTotal
        {
            get { return Math.Round(Price * Qty, MidpointRounding.AwayFromZero); }
        }
    }

    public class SampleItem
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
    }
}
