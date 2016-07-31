using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using UI.Main;
using UI.Product.Models;

namespace UI.Product.Dao
{
    public class InvoiceDao
    {
        public static List<InvoiceItemModel> GetInvoiceList(string customerCode, DateTime month)
        {
            List<InvoiceItemModel> rlt = new List<InvoiceItemModel>();

            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"SELECT ProductCode, Price, sum(ShipQty) as Qty FROM ShipmentRecord where CustomerCode=@p0 and ShipDate >= @p1 and ShipDate < @p2 GROUP BY ProductCode";
                    cmd.Parameters.Add(new SQLiteParameter("@p0", customerCode));
                    cmd.Parameters.AddWithValue("@p1", month);
                    cmd.Parameters.AddWithValue("@p2", month.AddMonths(1));

                    SQLiteDataReader sqlite_datareader = cmd.ExecuteReader();
                    while (sqlite_datareader.Read())
                    {
                        string code = sqlite_datareader["ProductCode"].ToString();
                        double price = (double)sqlite_datareader["Price"];
                        int qty = Convert.ToInt32(sqlite_datareader["Qty"]);

                        rlt.Add(new InvoiceItemModel()
                        {
                            CustomerCode = customerCode,
                            ProductCode = code,
                            Price = price,
                            Qty = qty,
                        });
                    }
                }
            }
            return rlt;
        }
    }
}
