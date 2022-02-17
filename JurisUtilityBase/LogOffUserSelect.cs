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
    public partial class LogOffUserSelect : Form
    {
        public LogOffUserSelect(JurisUtility JJ)
        {
            InitializeComponent();
            JU = JJ;
            string SQLPC2 = "select empinitials,empid + '    ' + empname as emp from employee  order by empinitials, empid";
            DataSet myRSPC2 = JU.RecordsetFromSQL(SQLPC2);
            if (myRSPC2.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dd in myRSPC2.Tables[0].Rows)
                    comboBox1.Items.Add(dd["emp"].ToString());
                comboBox1.SelectedIndex = 0;
            }
        }

        JurisUtility JU;
        private string empID = "";

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            empID = comboBox1.GetItemText(comboBox1.SelectedItem).Split(' ')[0];
        }

        private void LogOffUserSelect_Load(object sender, EventArgs e)
        {

        }

        private void buttonAddData_Click(object sender, EventArgs e)
        {
            int empsys = 0;
            string sql = "select empsysnbr from employee where empid = '" + empID + "'";
            
            DataSet ds = JU.RecordsetFromSQL(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                empsys = Convert.ToInt32(dr[0].ToString());
            }
            sql = "delete from Defaults where id in (999993) and userid = " + empsys.ToString();
            JU.ExecuteNonQuery(0, sql);
            this.Close();
        }
    }
}
