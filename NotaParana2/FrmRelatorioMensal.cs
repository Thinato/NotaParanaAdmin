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
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;

namespace NotaParana2
{
    public partial class FrmRelatorioMensal : Form
    {
        SqlConnection conn;
        CultureInfo BR;
        DataTable motoristas, motoristas_resultado;
        DataTable cadastradores, cadastradores_resultado;
        double totalNotas, totalSorteio, cpnMax;

        Series distribuicaoLucro;
        Series notasDia;
        Series scadastradores;

        bool ocupado;

        private void cmbChart_SelectedIndexChanged(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            switch (cmbChart.SelectedIndex)
            {
                case 0:
                    chart1.Series.Add(distribuicaoLucro);
                    break;
                case 1:
                    chart1.Series.Add(notasDia);
                    break;
                case 2:
                    chart1.Series.Add(scadastradores);
                    break;
            }
        }

        public FrmRelatorioMensal(SqlConnection _conn)
        {
            InitializeComponent();
            conn = _conn;
            BR = new CultureInfo("pt-BR");

            ocupado = false;
            chart1.ChartAreas[0].AxisX.IsMarginVisible = false;

            motoristas_resultado = new DataTable();
            motoristas_resultado.Columns.Add("ID", typeof(string));
            motoristas_resultado.Columns.Add("Nome", typeof(string));
            motoristas_resultado.Columns.Add("Bônus", typeof(string));
            motoristas_resultado.Columns.Add("total", typeof(double));
            motoristas_resultado.Columns.Add("comissao", typeof(double));
            motoristas_resultado.Columns.Add("notas_total", typeof(int));
            motoristas_resultado.Columns.Add("notas_apro", typeof(int));
            motoristas_resultado.Columns.Add("Aproveitamento", typeof(string));

            cadastradores_resultado = new DataTable();
            cadastradores_resultado.Columns.Add("ID", typeof(string));
            cadastradores_resultado.Columns.Add("Nome", typeof(string));
            cadastradores_resultado.Columns.Add("cpf", typeof(string));
            cadastradores_resultado.Columns.Add("Bônus", typeof(string));
            cadastradores_resultado.Columns.Add("total", typeof(int));
            cadastradores_resultado.Columns.Add("comissao", typeof(double));


            motoristas = new DataTable();
            SQLiteDataAdapter data = new SQLiteDataAdapter("select * from motorista", conn.connection);
            data.Fill(motoristas);
            foreach (DataRow row in motoristas.Rows)
            {
                motoristas_resultado.Rows.Add(row[0], row[2], 0.0, 0.0, row[3], 0, 0, "0%");
            }
            dgMotoristas.DataSource = motoristas_resultado;
            dgMotoristas.Columns["total"].Visible = false;
            dgMotoristas.Columns["comissao"].Visible = false;
            dgMotoristas.Columns["notas_total"].Visible = false;
            dgMotoristas.Columns["notas_apro"].Visible = false;

            dgMotoristas.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgMotoristas.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgMotoristas.Columns["Bônus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgMotoristas.Columns["Aproveitamento"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            cadastradores = new DataTable();
            data = new SQLiteDataAdapter("select * from nome_cpf", conn.connection);
            data.Fill(cadastradores);
            foreach (DataRow row in cadastradores.Rows)
            {
                if (row[3].ToString() == "1")
                    cadastradores_resultado.Rows.Add(row[0], row[1], row[2], "0", 0, row[4]);
            }

            dgCadastradores.DataSource = cadastradores_resultado;
            dgCadastradores.Columns["cpf"].Visible = false;
            dgCadastradores.Columns["total"].Visible = false;
            dgCadastradores.Columns["comissao"].Visible = false;

            dgCadastradores.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgCadastradores.Columns["Nome"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgCadastradores.Columns["Bônus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            if (ocupado)
                return;
            progressBar1.Value = 0;
            progressBar1.Step = 1;
            Thread thread = new Thread(() => Gerar());
            thread.Start();
            ocupado = true;

            cmbChart.Items.Clear();
            cmbChart.Items.Add("Distribuição de Lucro");
            cmbChart.Items.Add("Notas/Dia");
            cmbChart.Items.Add("Cadastradores");
        }

        private void Gerar()
        {
            distribuicaoLucro = new Series();
            notasDia = new Series();
            scadastradores = new Series();

            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn.connection;

            foreach (DataRow row in cadastradores_resultado.Rows)
            {
                row["total"] = 0;
            }
            foreach (DataRow row in motoristas_resultado.Rows)
            {
                row["total"] = 0;
                row["notas_apro"] = 0;
                row["notas_total"] = 0;
            }
            totalNotas = 0;
            totalSorteio = 0;
            cpnMax = 0;
            double bonusCadastrador = 0;
            double bonusMotorista = 0;


            DateTime data = dateTimePicker1.Value;

            DataTable nota_cpf = new DataTable();
            SQLiteDataAdapter d = new SQLiteDataAdapter($"select * from nota_cpf where data between '{data:yyyy-MM}-0' and '{data:yyyy-MM}-31' ", conn.connection);
            d.Fill(nota_cpf);

            DataTable nota_lugar = new DataTable();
            d = new SQLiteDataAdapter($"select * from nota_lugar where data between '{data:yyyy-MM}-0' and '{data:yyyy-MM}-31' ", conn.connection);
            d.Fill(nota_lugar);

            notasDia.ChartType = SeriesChartType.Line;
            notasDia.BorderWidth = 3;
            for (int i = 1; i < 32; i++)
            {
                cmd.CommandText = $"select count(*) from nota_lugar where data between '{data:yyyy-MM}-{i:00}' and '{data:yyyy-MM}-31'";
                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                {
                    break;
                }
                cmd.CommandText = $"select count(*) from nota_lugar where data='{data:yyyy-MM}-{i:00}'";
                notasDia.Points.AddXY(i, Convert.ToInt32(cmd.ExecuteScalar()));
            }
            ThreadHelperClass.AddMaximumProgress(this, progressBar1, motoristas_resultado.Rows.Count + cadastradores_resultado.Rows.Count + nota_lugar.Rows.Count + nota_cpf.Rows.Count);
            


            string id;
            foreach (DataRow row in nota_lugar.Rows) 
            {
                cmd.CommandText = $"select motorista from lugar where cnpj='{row[1]}'";
                id = cmd.ExecuteScalar().ToString();
                DataRow[] found = motoristas_resultado.Select($"ID = {id}");

                double val = Convert.ToDouble(row[4]);
                totalNotas += val;
                if (val > cpnMax)
                    cpnMax = val;
                if (val > 0)
                    found[0]["notas_apro"] = Convert.ToInt32(found[0]["notas_apro"]) + 1;
                found[0]["notas_total"] = Convert.ToInt32(found[0]["notas_total"]) + 1;
                found[0]["total"] = Convert.ToDouble(found[0]["total"]) + val;
                ThreadHelperClass.StepProgress(this, progressBar1);

            }

            foreach (DataRow row in nota_cpf.Rows)
            {
                DataRow[] found = cadastradores_resultado.Select($"cpf = '{row[3]}'");
                if (found.Length > 0)
                    found[0]["total"] = Convert.ToInt32(found[0]["total"]) + 1;

                ThreadHelperClass.StepProgress(this, progressBar1);
            }

            foreach (DataRow row in cadastradores_resultado.Rows)
            {
                double val = Convert.ToDouble(row["total"]) * Convert.ToDouble(row["comissao"]);
                row["Bônus"] = val.ToString("C", BR);
                bonusCadastrador += val;
                ThreadHelperClass.StepProgress(this, progressBar1);
            }

            foreach (DataRow row in motoristas_resultado.Rows)
            {
                double val = Convert.ToDouble(row["total"]) * Convert.ToDouble(row["comissao"]);
                row["Bônus"] = val.ToString("C", BR);
                row["Aproveitamento"] = (Convert.ToDouble(row["notas_apro"]) / Convert.ToDouble(row["notas_total"]) * 100).ToString("0.00") + "%";
                bonusMotorista += val;
                ThreadHelperClass.StepProgress(this, progressBar1);
            }


            ThreadHelperClass.SetText(this, lblGeral, $"Total de Notas:     {totalNotas.ToString("C", BR)} ({nota_lugar.Rows.Count})\n" +
                            $"Total de Sorteios:  {totalSorteio.ToString("C", BR)}\n" +
                            $"Lucro Total:        {(totalNotas + totalSorteio).ToString("C", BR)}\n\n" +
                            $"CPN Médio:          {(totalNotas / (double)nota_lugar.Rows.Count).ToString("C", BR)}\n" +
                            $"CPN Máximo:         {cpnMax.ToString("C", BR)}\n\n" +
                            $"Lucro Projeto Vida: {(totalNotas + totalSorteio - (bonusCadastrador + bonusMotorista)).ToString("C", BR)}");


            distribuicaoLucro.ChartType = SeriesChartType.Pie;
            distribuicaoLucro.Points.AddXY("Projeto Vida", totalNotas + totalSorteio - (bonusCadastrador + bonusMotorista));
            distribuicaoLucro.Points.AddXY("Motoristas", bonusMotorista);
            distribuicaoLucro.Points.AddXY("Cadastradores", bonusCadastrador);
            ocupado = false;


        }
    }
}
