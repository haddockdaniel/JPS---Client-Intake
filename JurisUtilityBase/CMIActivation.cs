using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gizmox.Controls;
using JDataEngine;
using JurisAuthenticator;
using JurisUtilityBase.Properties;

using System.Data.OleDb;

namespace JurisUtilityBase
{
    public partial class CMIActivation : Form
    {
        public CMIActivation(JurisUtility JJ, System.Drawing.Point ppt)
        {
            InitializeComponent();
            JU = JJ;
            pt = ppt;
        }

        JurisUtility JU;
        private System.Drawing.Point pt;

        private void buttonActivate_Click(object sender, EventArgs e)
        {

            string sql = "insert into CMIActivation (productID, productName, hash) values (1, 'Client Matter Intake', '" + textBox1.Text + "')";
            JU.ExecuteNonQuery(0, sql);

            this.Close();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void CMIActivation_Load(object sender, EventArgs e)
        {
            this.Location = pt;
        }
    }
}
