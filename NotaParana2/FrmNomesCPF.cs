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
    public partial class FrmNomesCPF : Form
    {
        SqlConnection conn;
        public FrmNomesCPF(SqlConnection _conn)
        {
            InitializeComponent();
            conn = _conn;

            DataTable dt = new DataTable();
            SQLiteDataAdapter data = new SQLiteDataAdapter("select * from nome_cpf", conn.connection);
            data.Fill(dt);

            dataGridView1.DataSource = dt;

            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (txtCPF.Text.Length > 11)
            {
                DataTable dt = new DataTable();
                SQLiteDataAdapter data = new SQLiteDataAdapter($"select * from nome_cpf where cpf='{txtCPF.Text}' ", conn.connection);
                data.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    txtNome.Text = dt.Rows[0][1].ToString();
                    if (dt.Rows[0][3].ToString() == "1")
                        checkBox1.Checked = true;
                    else
                        checkBox1.Checked = false;
                    txtComissao.Text = dt.Rows[0][4].ToString();
                    btnNovo.Text = "Salvar";
                }
                else
                {
                    btnNovo.Text = "Novo";
                }
            }            
        }

        private void btnApagar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                SQLiteCommand cmd = new SQLiteCommand($"delete from nome_cpf where cpf='{dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[2].Value}'", conn.connection);
                cmd.ExecuteNonQuery();
            }
            DataTable dt = new DataTable();
            SQLiteDataAdapter data = new SQLiteDataAdapter("select * from nome_cpf", conn.connection);
            data.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            int cada = 0;
            if (checkBox1.Checked)
                cada = 1;
            double comi = 0;
            try
            {
                comi = Convert.ToDouble(txtComissao.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "erro");
            }

            if (btnNovo.Text == "Novo")
            {
                
                SQLiteCommand cmd = new SQLiteCommand($"insert into nome_cpf (nome, cpf, cadastrador, comissao) values ('{txtNome.Text}', '{txtCPF.Text}', {cada}, {comi})", conn.connection);
                cmd.ExecuteNonQuery();
            } 
            else if (btnNovo.Text == "Salvar")
            {
                SQLiteCommand cmd = new SQLiteCommand($"update nome_cpf set nome='{txtNome.Text}', cadastrador={cada}, comissao={comi} where cpf='{txtCPF.Text}'", conn.connection);
                cmd.ExecuteNonQuery();
            }
            DataTable dt = new DataTable();
            SQLiteDataAdapter data = new SQLiteDataAdapter("select * from nome_cpf", conn.connection);
            data.Fill(dt);
            dataGridView1.DataSource = dt;
        }
    }
}
