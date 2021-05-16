using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace JurisUtilityBase
{
    public partial class PresetManager : Form
    {
        public PresetManager(DataSet ds)
        {
            InitializeComponent();
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].Width = 300;
            dataGridView1.Columns[2].Width = 75;
            dataGridView1.Columns[3].Width = 150;
            dataGridView1.Columns[4].Width = 60;
            dataGridView1.Columns[5].Width = 60;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {



            
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {

        }
    }
}
