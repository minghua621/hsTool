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
    public class UnitPriceDao
    {
        /// <summary>
        /// 回傳全部單價資訊
        /// </summary>
        /// <returns></returns>
        public static List<UnitPriceItemModel> GetUnitPriceList()
        {
            List<UnitPriceItemModel> rlt = new List<UnitPriceItemModel>();

            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM UnitPrice WHERE 1 = 1";
                    SQLiteDataReader sqlite_datareader = cmd.ExecuteReader();

                    while (sqlite_datareader.Read()) //read every data
                    {
                        string code = sqlite_datareader["ProductCode"].ToString();
                        string name = sqlite_datareader["ProductName"].ToString();
                        double pricce = (double)sqlite_datareader["Price"];
                        string customer = sqlite_datareader["CustomerCode"].ToString();
                        string material = sqlite_datareader["MaterialCode"].ToString();
                        string size = sqlite_datareader["SizeInfo"].ToString();
                        string processing0 = sqlite_datareader["Processing0"].ToString();
                        bool isCombined = Convert.ToBoolean((decimal)sqlite_datareader["Combined"]);
                        string units = sqlite_datareader["CombinedUnits"].ToString();
                        bool isDeleted = Convert.ToBoolean((decimal)sqlite_datareader["Deleted"]);
                        string colors = sqlite_datareader["Color"].ToString();

                        rlt.Add(new UnitPriceItemModel()
                        {
                            Code = code,
                            Name = name,
                            Price = pricce,
                            CustomerCode = customer,
                            MaterialCode = material,
                            Size = size,
                            Processing0 = processing0,
                            IsDeleted = isDeleted,
                            IsCombined = isCombined,
                            CombinedUnits = units,
                            ColorTypes = colors,
                        });
                    }
                }
            }
            return rlt;
        }

        /// <summary>
        /// 新增一筆單價資訊
        /// </summary>
        /// <param name="item"></param>
        public static void Create(UnitPriceItemModel item)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"INSERT INTO UnitPrice (CustomerCode, ProductCode, ProductName, Price, MaterialCode, SizeInfo, Combined, CombinedUnits, Processing0, Deleted, Color, UpdatedTime) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12)";
                    cmd.Parameters.Add(new SQLiteParameter("@p1", item.CustomerCode));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", item.Code));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", item.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@p4", item.Price));
                    cmd.Parameters.Add(new SQLiteParameter("@p5", item.MaterialCode));
                    cmd.Parameters.Add(new SQLiteParameter("@p6", item.Size));
                    cmd.Parameters.Add(new SQLiteParameter("@p7", item.IsCombined));
                    cmd.Parameters.Add(new SQLiteParameter("@p8", item.CombinedUnits));
                    cmd.Parameters.Add(new SQLiteParameter("@p9", item.Processing0));
                    cmd.Parameters.Add(new SQLiteParameter("@p10", item.IsDeleted));
                    cmd.Parameters.Add(new SQLiteParameter("@p11", item.ColorTypes));
                    cmd.Parameters.Add(new SQLiteParameter("@p12", DateTime.Now));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 更新一筆單價資訊
        /// </summary>
        /// <param name="item"></param>
        public static void Update(UnitPriceItemModel item)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"Update UnitPrice SET ProductName=@p0, Price=@p1, MaterialCode=@p2, SizeInfo=@p3, Processing0=@p4, UpdatedTime=@p5, CombinedUnits=@p6, Color=@p7 WHERE ProductCode=@p8";
                    cmd.Parameters.AddWithValue("@p0", item.Name);
                    cmd.Parameters.AddWithValue("@p1", item.Price);
                    cmd.Parameters.AddWithValue("@p2", item.MaterialCode);
                    cmd.Parameters.AddWithValue("@p3", item.Size);
                    cmd.Parameters.AddWithValue("@p4", item.Processing0);
                    cmd.Parameters.AddWithValue("@p5", DateTime.Now);
                    cmd.Parameters.AddWithValue("@p6", item.CombinedUnits);
                    cmd.Parameters.AddWithValue("@p7", item.ColorTypes);
                    cmd.Parameters.AddWithValue("@p8", item.Code);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 刪除一筆基本單價資訊(隱藏)
        /// </summary>
        /// <param name="item"></param>
        public static void Delete(UnitPriceItemModel item)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"Update UnitPrice SET Deleted=@p0, UpdatedTime=@p1 WHERE ProductCode=@p2";
                    cmd.Parameters.AddWithValue("@p0", item.IsDeleted);
                    cmd.Parameters.AddWithValue("@p1", DateTime.Now);
                    cmd.Parameters.AddWithValue("@p2", item.Code);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
