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
        public SqlConnection(string PATH)
        {
            connection = new SQLiteConnection("Data Source=" + PATH);
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
