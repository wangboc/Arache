using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AracheTest.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Windows.Forms;

namespace AracheTest.Tools
{
    class DBConnector
    {
        const string constr = "server=121.41.109.137;User Id=root;password=;Database=test;Connection Timeout=60;charset=gb2312";

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
                    connection.Clone();
                    return dt;
                }
                catch (Exception e)
                {
                    return null;
                }
            }

        }}
}
