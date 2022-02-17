using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Globalization;
using Gizmox.Controls;
using JDataEngine;
using JurisAuthenticator;
using Gizmox.CSharp;
using JurisUtilityBase.Properties;
using System.Data.OleDb;
using System.Runtime.CompilerServices;
using Microsoft.Office.Interop.Excel;

namespace JurisUtilityBase
{
    public partial class UtilityBaseMain : Form
    {
        #region Private  members

        private JurisUtility _jurisUtility;
        private bool isActivated;
        private System.Drawing.Point pt;

        #endregion

        #region Public properties

        public string CompanyCode { get; set; }

        public string JurisDbName { get; set; }

        public string JBillsDbName { get; set; }

        private int empsys = 0;

        private bool firstFocus = true;

        #endregion

        #region Constructor

        public UtilityBaseMain()
        {
            InitializeComponent();
            _jurisUtility = new JurisUtility();
            isActivated = false;

            //MessageBox.Show(JEncrypt(@"vMPR", "Athens"));
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


        private bool verifyDBHash(string hash, string dbName)
        {
            Encrypt eec = new Encrypt();
            if (dbName.Equals(eec.DecryptString("b14ca5898a4e41ca7bce2ea2315a1916", hash), StringComparison.OrdinalIgnoreCase))
                return true;
            else
                return false;


        }

        #endregion

        #region Public methods

        public void LoadCompanies()
        {
            var companies = _jurisUtility.Companies.Cast<object>().Cast<Instance>().ToList();
//            listBoxCompanies.SelectedIndexChanged -= listBoxCompanies_SelectedIndexChanged;
            listBoxCompanies.ValueMember = "Code";
            listBoxCompanies.DisplayMember = "Key";
            listBoxCompanies.DataSource = companies;
//            listBoxCompanies.SelectedIndexChanged += listBoxCompanies_SelectedIndexChanged;
            var defaultCompany = companies.FirstOrDefault(c => c.Default == Instance.JurisDefaultCompany.jdcJuris);
            if (companies.Count > 0)
            {
                listBoxCompanies.SelectedItem = defaultCompany ?? companies[0];
            }
        }

        #endregion

        #region MainForm events

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void checkForTables()
        {
            string sql = "IF  NOT EXISTS (SELECT * FROM sys.objects " +
            " WHERE object_id = OBJECT_ID(N'[dbo].[Defaults]') AND type in (N'U')) " +
            " BEGIN " +
            " Create Table [dbo].[Defaults](ID int, name varchar(300), UserID int,  CreationDate datetime, IsStandard char, AllData varchar(250)) " +
            " END";

            _jurisUtility.ExecuteSqlCommand(0, sql);

            sql = "IF  NOT EXISTS (SELECT * FROM sys.objects " +
            " WHERE object_id = OBJECT_ID(N'[dbo].[DefaultSettings]') AND type in (N'U')) " +
            " BEGIN " +
            " Create Table [dbo].[DefaultSettings] (DefaultID int, name varchar(50), data varchar(255), entryType varchar(50), empsys int) " +
            " END";

            _jurisUtility.ExecuteSqlCommand(0, sql);

        }

        private void listBoxCompanies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_jurisUtility.DbOpen)
            {
                _jurisUtility.CloseDatabase();
            }
            CompanyCode = "Company" + listBoxCompanies.SelectedValue;
            _jurisUtility.SetInstance(CompanyCode);
            JurisDbName = _jurisUtility.Company.DatabaseName;
            JBillsDbName = "JBills" + _jurisUtility.Company.Code;
            _jurisUtility.OpenDatabase();
            if (_jurisUtility.DbOpen)
            {
                ///GetFieldLengths();
            }
            //create base table if not exist
            string sql = "IF  NOT EXISTS (SELECT * FROM sys.objects " +
            " WHERE object_id = OBJECT_ID(N'[dbo].[CMIActivation]') AND type in (N'U')) " +
            " BEGIN " +
            " Create Table[dbo].[CMIActivation](productID int, productName varchar(300), hash varchar(50)) " +
            " END";

            _jurisUtility.ExecuteSqlCommand(0, sql);

        }


        private string gethashFromDB()
        {
            string hash = "";
            string sql = "select hash from CMIActivation where productID = 1";
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            if (dds != null && dds.Tables.Count > 0 && dds.Tables[0].Rows.Count > 0)
            {
                if (dds.Tables[0].Rows.Count > 1) //why do they have more than one per db?
                {
                    sql = "delete from CMIActivation where productid = 1";
                    _jurisUtility.ExecuteNonQuery(0, sql);
                }
                else
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        hash = dr[0].ToString();

                    }
                }
            }
            return hash;
        }


        #endregion

        #region Private methods

        private void DoDaFix()
        {
            //does key already exist?
            string hash = "";
            string sql = "";




            hash = gethashFromDB();
            if (!string.IsNullOrEmpty(hash)) //does the hash exits? if so....
            {
                //if it does, verify it
                isActivated = verifyDBHash(hash, JurisDbName); //if it matches, we are good
                if (!isActivated) //if not remove that info and make them reactivate
                {
                    sql = "delete from CMIActivation where productid = 1";
                    _jurisUtility.ExecuteNonQuery(0, sql);
                    MessageBox.Show("That Activation Code does not correspond with your Juris database" + "\r\n" + "The product has to be Reactivated", "Activation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(0);
                }
            }
            else //it does not exist so make them activate
            {
                this.Location = pt;
                CMIActivation cmi = new CMIActivation(_jurisUtility, pt);
                cmi.ShowDialog();
                //this adds it to DB. Now verify its good
                hash = gethashFromDB();
                if (!string.IsNullOrEmpty(hash))
                {
                    isActivated = verifyDBHash(hash, JurisDbName); //if it matches, we are good
                    if (!isActivated) //if not remove that info and make them reactivate
                    {
                        sql = "delete from CMIActivation where productid = 1";
                        _jurisUtility.ExecuteNonQuery(0, sql);
                        MessageBox.Show("That Activation Code does not correspond with your Juris database" + "\r\n" + "The product has to be Reactivated", "Activation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        System.Environment.Exit(0);
                    }             }
                else
                {
                    MessageBox.Show("There was a problem activating. Please ensure you code is correct and try again", "Activation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(0);
                }
            }

            if (isActivated) // fail safe in case I missed something
            {
                //now delete it as it isnt a preset and is only temp because we arent moving client info over to a matter screen
                this.Location = pt;
                //force user to login
                checkForTables();
                UserLogin ul = new UserLogin(_jurisUtility, pt);
                this.Hide();
                Employee emp = new Employee();
                emp = ul.emp;
                ul.ShowDialog();

                if (emp.empsysnbr != 0) // did we get a valid logon and empsysnbr?
                {
                    //if the setting was stored (login success), open program...else...exit
                    empsys = emp.empsysnbr;
                    if (!isUserAlreadyLoggedOn())
                    {
                        sql = "delete from DefaultSettings where defaultid in (999999, 999998, 999997, 999994, 999996) and empsys = " + empsys.ToString(); // only remove that user id
                        _jurisUtility.ExecuteNonQuery(0, sql);
                        sql = "delete from Defaults where id in (999999, 999998, 999997, 999994, 999996) and userid = " + empsys.ToString();    
                        _jurisUtility.ExecuteNonQuery(0, sql);
                        if (radioButtonCliOnly.Checked)
                        {
                            ClientForm cf = new ClientForm(_jurisUtility, 0, false, pt, empsys);
                            firstFocus = false;
                            this.Hide();
                            //cf.Closed += (s, args) => this.Close();
                            cf.Show();
                        }
                        else
                        {
                            MatterForm mf = new MatterForm(_jurisUtility, 0, "", 0, pt, empsys);
                            firstFocus = false;
                            this.Hide();
                            //mf.Closed += (s, args) => this.Close();
                            mf.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show("That User is already Logged In", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        System.Environment.Exit(0);
                    }
                }
                else
                {
                    MessageBox.Show("No valid login supplied", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.Show();
                }

            }
            else
            {
                System.Environment.Exit(0);

            }

        }

        private bool isUserAlreadyLoggedOn()
        {
            string sql = "select name from defaults where id = 999993 and userid = " + empsys.ToString();
            DataSet myRSPC2 = _jurisUtility.RecordsetFromSQL(sql);

            if (myRSPC2.Tables[0].Rows.Count == 0) //if there is no 999993 record with that empsys number then they arent loged on
            {
                sql = "insert into defaults (ID, name, userid, CreationDate, IsStandard, AllData ) " +
                " values (999993, 'LogIn', " + empsys.ToString() + ", getdate(), 'N', '')";

                _jurisUtility.ExecuteNonQuery(0, sql);
                return false;
            }
            else // if it does exist...someone is logged on as that user...
            {

                return true;
            }







        }

        private bool VerifyFirmName()
        {
            //    Dim SQL     As String
            //    Dim rsDB    As ADODB.Recordset
            //
            //    SQL = "SELECT CASE WHEN SpTxtValue LIKE '%firm name%' THEN 'Y' ELSE 'N' END AS Firm FROM SysParam WHERE SpName = 'FirmName'"
            //    Cmd.CommandText = SQL
            //    Set rsDB = Cmd.Execute
            //
            //    If rsDB!Firm = "Y" Then
            return true;
            //    Else
            //        VerifyFirmName = False
            //    End If

        }

        private bool FieldExistsInRS(DataSet ds, string fieldName)
        {

            foreach (DataColumn column in ds.Tables[0].Columns)
            {
                if (column.ColumnName.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }


        private static bool IsDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum; 
        }

        private void WriteLog(string comment)
        {
            var sql =
                string.Format("Insert Into UtilityLog(ULTimeStamp,ULWkStaUser,ULComment) Values('{0}','{1}', '{2}')",
                    DateTime.Now, GetComputerAndUser(), comment);
            _jurisUtility.ExecuteNonQueryCommand(0, sql);
        }

        private string GetComputerAndUser()
        {
            var computerName = Environment.MachineName;
            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var userName = (windowsIdentity != null) ? windowsIdentity.Name : "Unknown";
            return computerName + "/" + userName;
        }



        private void DeleteLog()
        {
            string AppDir = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            string filePathName = Path.Combine(AppDir, "VoucherImportLog.txt");
            if (File.Exists(filePathName + ".ark5"))
            {
                File.Delete(filePathName + ".ark5");
            }
            if (File.Exists(filePathName + ".ark4"))
            {
                File.Copy(filePathName + ".ark4", filePathName + ".ark5");
                File.Delete(filePathName + ".ark4");
            }
            if (File.Exists(filePathName + ".ark3"))
            {
                File.Copy(filePathName + ".ark3", filePathName + ".ark4");
                File.Delete(filePathName + ".ark3");
            }
            if (File.Exists(filePathName + ".ark2"))
            {
                File.Copy(filePathName + ".ark2", filePathName + ".ark3");
                File.Delete(filePathName + ".ark2");
            }
            if (File.Exists(filePathName + ".ark1"))
            {
                File.Copy(filePathName + ".ark1", filePathName + ".ark2");
                File.Delete(filePathName + ".ark1");
            }
            if (File.Exists(filePathName ))
            {
                File.Copy(filePathName, filePathName + ".ark1");
                File.Delete(filePathName);
            }

        }

            

        private void LogFile(string LogLine)
        {
            string AppDir = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            string filePathName = Path.Combine(AppDir, "VoucherImportLog.txt");
            using (StreamWriter sw = File.AppendText(filePathName))
            {
                sw.WriteLine(LogLine);
            }	
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

            DoDaFix();
        }

        private void buttonReport_Click(object sender, EventArgs e)
        {

            System.Environment.Exit(0);
          
        }

        private void UtilityBaseMain_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Enter Admin Password", "Admin Log On", "");
            if (name.Equals("AthensDBO"))
            {
                menuStrip1.Visible = true;
            }
            else
            {
                System.Environment.Exit(0);
            }
        }

        private void clearAllUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ddr = MessageBox.Show("This will fix stuck users on the backend." + "\r\n" + "Ensure everyone has closed the tool before clicking Yes. Continue?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ddr == DialogResult.Yes)
            {
                string sql = "delete from Defaults where id in (999993)";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }
        }

        private void clearAllTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ddr = MessageBox.Show("This will delete all existing templates." + "\r\n" + "Ensure everyone is logged out of the tool before clicking Yes. Continue?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ddr == DialogResult.Yes)
            {
                string sql = "delete fromDefaultSettings where DefaultID < 999990";
                _jurisUtility.ExecuteNonQuery(0, sql);

                sql = "delete from Defaults where id < 999990";
                _jurisUtility.ExecuteNonQuery(0, sql);

            }
        }

        private void clearAllTempDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ddr = MessageBox.Show("This will remove any remaining temp data added by the tool in the event of a crash" + "\r\n" + "Ensure everyone is logged out of the tool before clicking Yes. Continue?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ddr == DialogResult.Yes)
            {
                string sql = "delete fromDefaultSettings where DefaultID > 999990";
                _jurisUtility.ExecuteNonQuery(0, sql);

                sql = "delete from Defaults where id > 999990";
                _jurisUtility.ExecuteNonQuery(0, sql);

            }
        }

        private void UtilityBaseMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            string sql = "delete from Defaults where id in (999993) and userid = " + empsys.ToString();
            _jurisUtility.ExecuteNonQuery(0, sql);
        }

        private void UtilityBaseMain_Enter(object sender, EventArgs e)
        {
            if (!firstFocus)
            {
                this.Show();

                firstFocus = true;
            }
        }

        private void clearSingleUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogOffUserSelect lu = new LogOffUserSelect(_jurisUtility);
            lu.ShowDialog();
        }
    }
}
