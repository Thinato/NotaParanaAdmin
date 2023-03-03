using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace NotaParana2
{
    public class SqlConnection
    {
        public SQLiteConnection connection;
        public SqlConnection()
        {
            connection = new SQLiteConnection("Data Source=data.db");
        }
        public void Connect()
        {
            connection.Open();
        }
        public void Disconnect()
        {
            connection.Close();
        }
    }
}
