using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JurisUtilityBase
{
    public partial class MatLookUp : Form
    {
        public MatLookUp(JurisUtility jutil, System.Drawing.Point ppt, int clisys)
        {
            InitializeComponent();
            _jurisUtility = jutil;
            pt = ppt;
            clisysnbr = clisys;
        }

        JurisUtility _jurisUtility;
        private System.Drawing.Point pt;
        public int clisysnbr = 0;

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MatLookUp_Load(object sender, EventArgs e)
        {
            string sql = "";
                sql = "select dbo.jfn_FormatMatterCode(matcode) as MatterCode, matreportingname as MatterName from matter where matclinbr = " + clisysnbr.ToString() + " order by dbo.jfn_FormatMatterCode(matcode)";
            DataSet ds = _jurisUtility.RecordsetFromSQL(sql);

            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].Width = 250;
        }
    }
}
