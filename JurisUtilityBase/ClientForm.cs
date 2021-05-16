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
    public partial class ClientForm : Form
    {
        public ClientForm(JurisUtility jutil)
        {
            InitializeComponent();
            JU = jutil;
        }


        JurisUtility JU;

        private void ClientForm_Load(object sender, EventArgs e)
        {

        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sql = "select ID, name as [Default Name], PopulateMatter as [Populate Matter], Employee.empname as Creator, CreationDate as [Creation Date], isStandard as [Default] from Defaults inner join employee on empsysnbr = Creator";
            DataSet ds = JU.RecordsetFromSQL(sql);
            PresetManager DM = new PresetManager(ds);
            DM.ShowDialog();
        }

        private void clearFormToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void clearFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox26_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Client xxxxx was added successfully." + "\r\n" + "Would you like to Add a Matter to this Client?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}
