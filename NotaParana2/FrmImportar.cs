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
using System.Threading;

namespace NotaParana2
{
    public partial class FrmImportar : Form
    {
        SqlConnection conn;

        int de = 0;
        int dne = 0;
        int aguardando = 0;
        int novosLocais = 0;
        int atualizacoesNotas = 0;
        int atualizacoesLocais = 0;
        int novasNotas = 0;
        int notaCalculada = 0;
        int notaNaoCalculada = 0;
        public FrmImportar(SqlConnection _conn)
        {
            InitializeComponent();
            this.AllowDrop = true;
            conn = _conn;
        }
        

        private void FrmImportar_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void FrmImportar_DragDrop(object sender, DragEventArgs e)
        {
            de = 0;
            dne = 0;
            aguardando = 0;
            novosLocais = 0;
            atualizacoesNotas = 0;
            atualizacoesLocais = 0;
            novasNotas = 0;
            notaCalculada = 0;
            notaNaoCalculada = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = 1;
            progressBar1.Step = 1;


            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                try
                {
                    Thread thread = new Thread(() => AbrirArquivo(file));
                    //Thread thread = new Thread(new ThreadStart(AbrirArquivo(Fi)));
                    thread.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
           
        }

        private void AbrirArquivo(string filePath)
        {
            //lblStatus.Text = "Lendo arquivo...";
            var fileContent = string.Empty;

            int count = 1;
            int idCounter = 1;
            int lineCount = File.ReadLines(filePath).Count();
            //progressBar1.Value = 0;
            //progressBar1.Maximum = lineCount;
            ThreadHelperClass.AddMaximumProgress(this, progressBar1, lineCount);
            int process = 0;
            
            using ( StreamReader sr = new StreamReader(filePath))
            {
                string first = sr.ReadLine();
                if (first.StartsWith("LISTAGEM DE DOCUMENTOS FISCAIS"))
                    process = 1;
                else if (first.StartsWith("LISTAGEM DE NOTA"))
                    process = 2;
                else
                    throw new Exception("Arquivo não reconhecido.");
            }
            var lines = File.ReadLines(filePath, Encoding.Default).ToList();
            lines.RemoveAt(0);
            lines.RemoveAt(1);

            Parallel.ForEach(File.ReadLines(filePath, Encoding.Default), line =>
            {
                if (process == 1 && count > 2)
                {
                    #region junk hack
                    string nline = line;
                    if (nline.Contains("&"))
                    {
                        nline = nline.Replace("&amp;", "&");
                        nline = nline.Replace("&Eacute;", "É");
                        nline = nline.Replace("&Aacute;", "Á");
                        nline = nline.Replace("&Acirc;", "Â");
                        nline = nline.Replace("&Atilde;", "Ã");
                        nline = nline.Replace("&Iacute;", "Í");
                        nline = nline.Replace("&Oacute;", "Ó");
                        nline = nline.Replace("&eacute;", "é");
                        nline = nline.Replace("&aacute;", "á");
                        nline = nline.Replace("&acirc;", "â");
                        nline = nline.Replace("&atilde;", "ã");
                        nline = nline.Replace("&iacute;", "í");
                        nline = nline.Replace("&oacute;", "ó");
                    }
                    nline = nline.Replace("; ", "");

                    #endregion
                    string[] split = nline.Split(';');
                    if (split.Length != 7)
                    {
                        MessageBox.Show($"Erro na interpretação da linha!\nLinha {count} foi ignorada!\n{line}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    switch (split[6])
                    {
                        case "CALCULADO":
                            notaCalculada++;
                            SQLiteCommand cmd = new SQLiteCommand($"select count(*) from lugar where cnpj='{split[0]}'", conn.connection);
                            int c = Convert.ToInt32(cmd.ExecuteScalar());
                            if (c == 0)
                            {
                                cmd.CommandText = $"insert into lugar (cnpj, nome, motorista) values ('{split[0]}', '{split[1]}', 1)";
                                cmd.ExecuteNonQuery();
                            }
                            cmd.CommandText = $"select count(*) from nota_lugar where cnpj='{split[0]}' and nota='{split[2]}' ";
                            c = Convert.ToInt32(cmd.ExecuteScalar());
                            string query = string.Empty;
                            string credito = split[5].Substring(3);
                            credito = credito.Replace(".", "");
                            credito = credito.Replace(",", ".");
                            if (c > 0)
                            {
                                query = $"update nota_lugar set data='{split[3].Substring(6, 4)}-{split[3].Substring(3, 2)}-{split[3].Substring(0, 2)}', credito={credito} where cnpj='{split[0]}' and nota='{split[2]}'";
                                atualizacoesLocais++;
                            }
                            else
                            {
                                query = $"insert into nota_lugar (cnpj, nota, data, credito) values ('{split[0]}', '{split[2]}', '{split[3].Substring(6, 4)}-{split[3].Substring(3, 2)}-{split[3].Substring(0, 2)}', {credito})";
                                novosLocais++;
                            }
                            cmd = new SQLiteCommand(query, conn.connection);
                            cmd.ExecuteNonQuery();
                            break;
                        default:
                            notaNaoCalculada++;
                            break;
                    }
                }
                else if (process == 2 && count > 2)
                {
                    string[] split = line.Split(';');
                    switch (split[3])
                    {
                        case "Doação efetivada":
                            de++;
                            SQLiteCommand cmd = new SQLiteCommand($"select count(*) from nota_cpf where cod='{split[0]}'", conn.connection);
                            int c = Convert.ToInt32(cmd.ExecuteScalar());

                            if (c > 0)
                            {
                                ThreadHelperClass.StepProgress(this, progressBar1);
                                return;
                            }
                            else
                            {
                                cmd.CommandText = $"insert into nota_cpf (cod, data, cpf) values ('{split[0]}', '{split[1].Substring(6, 4)}-{split[1].Substring(3, 2)}-{split[1].Substring(0, 2)}', '{split[2]}')";
                                novasNotas++;
                                cmd.ExecuteNonQuery();
                            }
                            break;
                        case "Doação não efetivada":
                            dne++;
                            break;
                        case "Aguardando processamento":
                            aguardando++;
                            break;
                    }
                }
                ThreadHelperClass.StepProgress(this, progressBar1);
                double prog = (double)progressBar1.Value / (double)progressBar1.Maximum * 100;
                ThreadHelperClass.SetText(this, lblStatus, $"Processando... {progressBar1.Value}/{progressBar1.Maximum} ({prog:0.00}%)");
                count++;
            });
            
            
            ThreadHelperClass.SetText(this, lblResult, $"Doações efetivadas:       { de,6}\n" +
                             $"Doações não efetivadas:   {dne,6}\n" +
                             $"Aguardando processamento: {aguardando,6}\n" +
                             $"Novas Notas:              {novasNotas,6}\n" +
                             $"Notas Atualizadas:        {atualizacoesNotas,6}\n" +
                             $"Notas Calculadas:         {notaCalculada,6}\n" +
                             $"Notas Não Calculadas:     {notaNaoCalculada,6}\n" +
                             $"Novos Locais:             {novosLocais,6}\n" +
                             $"Locais Atualizados:       {atualizacoesLocais,6}");
            //lblStatus.Text = $"Finalizado.";

           
        }
    }
    
}
