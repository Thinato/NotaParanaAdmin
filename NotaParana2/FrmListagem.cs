﻿using System;
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
    public partial class FrmListagem : Form
    {
        SqlConnection conn;
        public FrmListagem(SqlConnection _conn)
        {
            InitializeComponent();
            conn = _conn;
        }
    }
}
