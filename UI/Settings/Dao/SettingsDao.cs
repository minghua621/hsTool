using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using UI.Main;
using UI.Settings.Models;

namespace UI.Settings.Dao
{
    public class SettingsDao
    {
        public static List<ColorItemModel> GetColorList()
        {
            List<ColorItemModel> rlt = new List<ColorItemModel>();

            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM ColorSettings";
                    SQLiteDataReader sqlite_datareader = cmd.ExecuteReader();

                    while (sqlite_datareader.Read())
                    {
                        string code = sqlite_datareader["Code"].ToString();
                        string name = sqlite_datareader["Name"].ToString();
                        string codeAid = sqlite_datareader["CodeAid"].ToString();

                        rlt.Add(new ColorItemModel()
                        {
                            Code = code,
                            Name = name,
                            CodeAid = codeAid,
                        });
                    }
                }
            }
            return rlt;
        }

        public static void CreateColor(ColorItemModel item)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"INSERT INTO ColorSettings (Code, Name, CodeAid, UpdatedTime) VALUES (@p1,@p2,@p3,@p4)";
                    cmd.Parameters.Add(new SQLiteParameter("@p1", item.Code));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", item.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", item.CodeAid));
                    cmd.Parameters.Add(new SQLiteParameter("@p4", DateTime.Now));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateColor(ColorItemModel item)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"Update ColorSettings SET Name=@p0, CodeAid=@p1, UpdatedTime=@p2 WHERE Code=@p3";
                    cmd.Parameters.Add(new SQLiteParameter("@p0", item.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@p1", item.CodeAid));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", DateTime.Now));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", item.Code));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteColor(string code)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"Delete FROM ColorSettings WHERE Code=@p0";
                    cmd.Parameters.AddWithValue("@p0", code);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<CustomerItemModel> GetCustomerList()
        {
            List<CustomerItemModel> rlt = new List<CustomerItemModel>();

            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM CustomerSettings";
                    SQLiteDataReader sqlite_datareader = cmd.ExecuteReader();

                    while (sqlite_datareader.Read())
                    {
                        string code = sqlite_datareader["Code"].ToString();
                        string name = sqlite_datareader["Name"].ToString();
                        string fullname = sqlite_datareader["FullName"].ToString();
                        string phone = sqlite_datareader["PhoneNumber"].ToString();

                        rlt.Add(new CustomerItemModel()
                        {
                            Code = code,
                            Name = name,
                            FullName = fullname,
                            Phone = phone,
                        });
                    }
                }
            }
            return rlt;
        }

        public static List<MaterialItemModel> GetMaterialList()
        {
            List<MaterialItemModel> rlt = new List<MaterialItemModel>();

            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM MaterialSettings";
                    SQLiteDataReader sqlite_datareader = cmd.ExecuteReader();

                    while (sqlite_datareader.Read())
                    {
                        string code = sqlite_datareader["Code"].ToString();
                        string name = sqlite_datareader["Name"].ToString();

                        rlt.Add(new MaterialItemModel()
                        {
                            Code = code,
                            Name = name,
                        });
                    }
                }
            }
            return rlt;
        }

        public static void CreateMaterial(MaterialItemModel item)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"INSERT INTO MaterialSettings (Code, Name, UpdatedTime) VALUES (@p1,@p2,@p3)";
                    cmd.Parameters.Add(new SQLiteParameter("@p1", item.Code));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", item.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", DateTime.Now));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateMaterial(MaterialItemModel item)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"Update MaterialSettings SET Name=@p0, UpdatedTime=@p1 WHERE Code=@p2";
                    cmd.Parameters.Add(new SQLiteParameter("@p0", item.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@p1", DateTime.Now));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", item.Code));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteMaterial(string code)
        {
            using (SQLiteConnection conn = new SQLiteConnection(AppSettings.ConnectString))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"Delete FROM MaterialSettings WHERE Code=@p0";
                    cmd.Parameters.AddWithValue("@p0", code);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
