using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace NotaParana2
{
    public partial class FrmMotoristas : Form
    {
        SqlConnection conn;
        public FrmMotoristas(SqlConnection _conn)
        {
            InitializeComponent();
            conn = _conn;

            DataTable dt = new DataTable();
            SQLiteDataAdapter data = new SQLiteDataAdapter("select * from motorista", conn.connection);
            data.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
    }
}
