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
    public partial class FrmLugares : Form
    {
        SqlConnection conn;
        public FrmLugares(SqlConnection _conn)
        {
            InitializeComponent();
            conn = _conn;

            DataTable dt = new DataTable();
            SQLiteDataAdapter data = new SQLiteDataAdapter("select * from lugar", conn.connection);
            data.Fill(dt);

            dataGridView1.DataSource = dt;

            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


            DataTable dtMoto = new DataTable();
            SQLiteDataAdapter dataMoto = new SQLiteDataAdapter("select * from motorista", conn.connection);
            dataMoto.Fill(dtMoto);
            comboBox1.Items.Clear();
            foreach (DataRow row in dtMoto.Rows)
            {
                comboBox1.Items.Add(row[2]);
            }
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            if (btnNovo.Text == "Novo") // insert
            {
                SQLiteCommand cmd = new SQLiteCommand($"insert into lugar (cnpj, nome, motorista) values ('{txtCNPJ.Text}', '{txtNome.Text}', {comboBox1.SelectedIndex + 1})", conn.connection);
                cmd.ExecuteNonQuery();
            }
            else if (btnNovo.Text == "Salvar") // update
            {
                SQLiteCommand cmd = new SQLiteCommand($"update lugar set nome='{txtNome.Text}', motorista={comboBox1.SelectedIndex + 1} where cnpj='{txtCNPJ.Text}'", conn.connection);
                cmd.ExecuteNonQuery();
            }
            DataTable dt = new DataTable();
            SQLiteDataAdapter data = new SQLiteDataAdapter("select * from lugar", conn.connection);
            data.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void txtCNPJ_TextChanged(object sender, EventArgs e)
        {
            if (txtCNPJ.Text.Length > 15)
            {
                DataTable dt = new DataTable();
                SQLiteDataAdapter data = new SQLiteDataAdapter($"select * from lugar where cnpj='{txtCNPJ.Text}' ", conn.connection);
                data.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    txtNome.Text = dt.Rows[0][2].ToString();
                    comboBox1.SelectedIndex = Convert.ToInt32(dt.Rows[0][3].ToString()) - 1;
                    btnNovo.Text = "Salvar";
                }
                else
                {
                    btnNovo.Text = "Novo";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                SQLiteCommand cmd = new SQLiteCommand($"delete from lugar where cnpj='{dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[1].Value}'", conn.connection);
                cmd.ExecuteNonQuery();
            }
            DataTable dt = new DataTable();
            SQLiteDataAdapter data = new SQLiteDataAdapter("select * from lugar", conn.connection);
            data.Fill(dt);
            dataGridView1.DataSource = dt;
        }
    }
}
