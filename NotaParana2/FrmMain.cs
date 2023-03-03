using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotaParana2
{
    public partial class FrmMain : Form
    {
        SqlConnection conn = new SqlConnection();
        private Form activeForm = null;
        public FrmMain()
        {
            InitializeComponent();
            conn.Connect();
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
    }
}
