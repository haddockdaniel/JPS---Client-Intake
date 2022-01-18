using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gizmox.CSharp;

namespace JurisUtilityBase
{
    public partial class UserLogin : Form
    {
        public UserLogin(JurisUtility ju, System.Drawing.Point pp)
        {
            InitializeComponent();
            JUtil = ju;
            pt = pp;
            //this.Location = pt;
            emp = new Employee();
            emp.empsysnbr = 0;
            this.CenterToScreen();
        }

        private bool success = false;
        JurisUtility JUtil;
        private System.Drawing.Point pt;
        public Employee emp;
        

        private void buttonReport_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(textBoxName.Text))
            {
                MessageBox.Show("Please enter a user name", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                string sql = "select EmpPassword, empsysnbr from employee where empid = '" + textBoxName.Text + "' and EmpValidAsUser = 'Y'";
                DataSet dds = JUtil.RecordsetFromSQL(sql);
                string strTemp = JEncrypt(textBoxPWord.Text, "Athens");
                string empsys = "";
                if (dds != null && dds.Tables.Count > 0 && dds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dds.Tables[0].Rows)
                    {
                        if (string.IsNullOrEmpty(row["EmpPassword"].ToString()) && string.IsNullOrEmpty(textBoxPWord.Text))
                        {
                            success = true;
                            empsys = row["empsysnbr"].ToString();
                        }
                        else if (textBoxName.Text.Equals("smgr", StringComparison.OrdinalIgnoreCase))
                        {
                            //MessageBox.Show(row["EmpPassword"].ToString() + " : " + );
                            if (textBoxPWord.Text.Equals(row["EmpPassword"].ToString().Trim()))
                            {
                                success = true;
                                empsys = row["empsysnbr"].ToString();
                            }
                            else if (textBoxPWord.Text.Equals(JEncrypt(row["EmpPassword"].ToString(), "Athens")))
                            {
                                success = true;
                                empsys = row["empsysnbr"].ToString();
                            }
                            else
                                success = false;

                        }
                        else
                        {
                            if (textBoxPWord.Text.Equals(JEncrypt(row["EmpPassword"].ToString(), "Athens")))
                            {
                                success = true;
                                empsys = row["empsysnbr"].ToString();
                            }
                            else
                                success = false;
                        }
                    }
                    if (!success)
                        MessageBox.Show("That user name and password does not match Juris", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    else
                    {
                        this.Hide();
                        emp.empsysnbr = Convert.ToInt32(empsys);
                    }
                }
                else
                {
                    MessageBox.Show("That user does not exist or is not set up as a User in Juris", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    success = false;
                }

            }
            
        }

        public static string JEncrypt(string sSecret, string sPassWord)
        {


            int l = 0;
            int X = 0;
            int @char = 0;
            string sTmp = String.Empty;

            // Secret$ = the string you wish to encrypt or decrypt. 
            // PassWord$ = the password with which to encrypt the string. 

            sTmp = sSecret;
            l = Strings.Len(sPassWord);
            for (X = 1; X <= Strings.Len(sTmp); X++)
            {
                @char = Strings.Asc(Strings.Mid(sPassWord, (X % l) - l * Conversion.BoolToInt((X % l) == 0), 1));
                Gizmox.CSharp.StringType.MidStmtStr(ref sTmp, X, 1, Strings.Chr(Strings.Asc(Strings.Mid(sTmp, X, 1)) ^ @char).ToString());
            }



            return sTmp;
        }

        private void UserLogin_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
