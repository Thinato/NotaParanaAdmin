using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace NotaParana2
{
    public partial class FrmMain : Form
    {
        private Form activeForm = null;
        SqlConnection conn;
        public FrmMain()
        {
            InitializeComponent();
        }
        private void OpenChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pMDIForm.Controls.Add(childForm);
            pMDIForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void lugaresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmLugares(conn));
        }

        private void listagemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmListagem(conn));
        }

        private void associarNomesCPFsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmNomesCPF(conn));
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            conn.Disconnect();
        }

        private void motoristasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmMotoristas(conn));
        }

        private void relatórioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmRelatorioMensal(conn));
        }

        private void importarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmImportar(conn));
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Text = $"Nota Paraná Admin ({System.Reflection.Assembly.GetExecutingAssembly().GetName().Version})";
            CheckDB();
        }
        private void CheckDB()
        {
            string path = $@"{Environment.ExpandEnvironmentVariables(@"%appdata%")}\NPA\data.db";
            try
            {
                if (File.Exists(path))
                {
                    conn = new SqlConnection(path);
                    conn.Connect();
                }
                else
                {   if (!Directory.Exists($@"{Environment.ExpandEnvironmentVariables(@"%appdata%")}\NPA"))
                        Directory.CreateDirectory($@"{Environment.ExpandEnvironmentVariables(@"%appdata%")}\NPA");
                    SQLiteConnection.CreateFile(path);
                    conn = new SqlConnection(path);
                    conn.Connect();
                    SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE \"lugar\" ( \"CNPJ\" TEXT NOT NULL, \"NOME\" TEXT NOT NULL, \"MOTORISTA\" INTEGER NOT NULL, FOREIGN KEY(\"MOTORISTA\") REFERENCES \"motorista\"(\"ID\"), PRIMARY KEY(\"CNPJ\") );", conn.connection);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE \"motorista\" ( \"ID\" INTEGER NOT NULL, \"CPF\" TEXT NOT NULL, \"NOME\" TEXT NOT NULL, \"COMISSAO\" REAL NOT NULL DEFAULT 0.05, PRIMARY KEY(\"ID\" AUTOINCREMENT));";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE \"nome_cpf\" ( \"ID\" INTEGER NOT NULL, \"NOME\" TEXT NOT NULL, \"CPF\" TEXT NOT NULL, \"CADASTRADOR\" INTEGER NOT NULL, \"COMISSAO\" REAL NOT NULL DEFAULT 0.01, PRIMARY KEY(\"ID\" AUTOINCREMENT));";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE \"nota_cpf\" ( \"ID\" INTEGER NOT NULL, \"COD\" TEXT NOT NULL, \"DATA\" TEXT NOT NULL, \"CPF\" TEXT NOT NULL, PRIMARY KEY(\"ID\" AUTOINCREMENT));";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE \"nota_lugar\" ( \"ID\"INTEGER NOT NULL, \"CNPJ\" TEXT NOT NULL, \"NOTA\" TEXT NOT NULL, \"DATA\" TEXT NOT NULL, \"CREDITO\" REAL NOT NULL, PRIMARY KEY(\"ID\" AUTOINCREMENT) );";
                    cmd.ExecuteNonQuery();
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
