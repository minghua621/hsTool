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
    }
}
