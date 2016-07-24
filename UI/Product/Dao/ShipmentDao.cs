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
    public class ShipmentDao
    {
        /// <summary>
        /// 回傳所有出貨紀錄
        /// </summary>
        /// <returns></returns>
        public static List<ShipmentItemModel> GetShipmentRecordList()
        {
            List<ShipmentItemModel> rlt = new List<ShipmentItemModel>();

            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM ShipmentRecord WHERE 1 = 1";
                    SQLiteDataReader sqlite_datareader = cmd.ExecuteReader();

                    while (sqlite_datareader.Read())
                    {
                        long id = (long)sqlite_datareader["SerialNumber"];
                        string code = sqlite_datareader["ProductCode"].ToString();
                        string name = sqlite_datareader["ProductName"].ToString();
                        double pricce = (double)sqlite_datareader["Price"];
                        string customer = sqlite_datareader["CustomerCode"].ToString();
                        int qty = Convert.ToInt32(sqlite_datareader["ShipQty"]);
                        DateTime dt = Convert.ToDateTime(sqlite_datareader["ShipDate"]);
                        string color = sqlite_datareader["Color"].ToString();

                        rlt.Add(new ShipmentItemModel()
                        {
                            SerialNumber = id,
                            ProductCode = code,
                            ProductName = name,
                            UnitPrice = pricce,
                            CustomerCode = customer,
                            ShipQty = qty,
                            ShipDate = dt,
                            ColorCode = color,
                        });
                    }
                }
            }
            return rlt;
        }

        /// <summary>
        /// 新增ㄧ筆出貨紀錄
        /// </summary>
        /// <param name="item"></param>
        public static long Create(ShipmentItemModel item)
        {
            long number = -1;
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"INSERT INTO ShipmentRecord (CustomerCode, ProductCode, ProductName, Price, ShipQty, ShipDate, Color, UpdatedTime) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8)";
                    cmd.Parameters.Add(new SQLiteParameter("@p1", item.CustomerCode));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", item.ProductCode));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", item.ProductName));
                    cmd.Parameters.Add(new SQLiteParameter("@p4", item.UnitPrice));
                    cmd.Parameters.Add(new SQLiteParameter("@p5", item.ShipQty));
                    cmd.Parameters.AddWithValue("@p6", item.ShipDate);
                    cmd.Parameters.Add(new SQLiteParameter("@p7", item.ColorCode));
                    cmd.Parameters.Add(new SQLiteParameter("@p8", DateTime.Now));
                    cmd.ExecuteNonQuery();
                    number = conn.LastInsertRowId;
                }
            }
            return number;
        }

        public static void Update(ShipmentItemModel item)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"Update ShipmentRecord SET ProductName=@p0, Price=@p1, ShipQty=@p2, ShipDate=@p3, Color=@p4, UpdatedTime=@p5 WHERE SerialNumber=@p6";
                    cmd.Parameters.AddWithValue("@p0", item.ProductName);
                    cmd.Parameters.AddWithValue("@p1", item.UnitPrice);
                    cmd.Parameters.AddWithValue("@p2", item.ShipQty);
                    cmd.Parameters.AddWithValue("@p3", item.ShipDate);
                    cmd.Parameters.AddWithValue("@p4", item.ColorCode);
                    cmd.Parameters.AddWithValue("@p5", DateTime.Now);
                    cmd.Parameters.AddWithValue("@p6", item.SerialNumber);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Delete(long id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"Delete FROM ShipmentRecord WHERE SerialNumber=@p0";
                    cmd.Parameters.AddWithValue("@p0", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
