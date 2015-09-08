using System;
using MySql.Data.MySqlClient;
using System.Data;

namespace AracheTest.Tools
{
    internal class DBConnector
    {
        private const string constr =
            "server=121.41.109.137;User Id=root;password=;Database=test;Connection Timeout=60;charset=gb2312";

        public static DataTable ExecuteSql(string sqlcommand)
        {
            using (MySqlConnection connection = new MySqlConnection(constr))
            {
                try
                {
                    connection.Open();
                    MySqlCommand mycmd = new MySqlCommand(sqlcommand, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(mycmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    connection.Close();
                    return dt;}
                catch (Exception e)
                {
                    LogHelper.WriteLog(typeof (frmMain), "数据库连接失败" + " " + DateTime.Now.ToString("D"));
                    return null;
                }
            }
        }
    }
}