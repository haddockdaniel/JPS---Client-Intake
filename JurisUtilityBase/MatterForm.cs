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
using JurisUtilityBase.Properties;
using System.Data.OleDb;
using Microsoft.Win32;
using JurisSVR.ExpenseAttachments;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using JurisSVR;

namespace JurisUtilityBase
{
    public partial class MatterForm : Form
    {
        public MatterForm(JurisUtility jutil, int preID, bool modify, int clisys, string cc)
        {
            InitializeComponent();
            _jurisUtility = jutil;
            presetID = preID;
            isModification = modify;
            clisysnbr = clisys;
            clicode = cc;
        }


        JurisUtility _jurisUtility;
        int presetID = 0;
        string clicode = "";
        bool isModification = false;
        public List<ExceptionHandler> errorList = new List<ExceptionHandler>();
        ExceptionHandler error = null;
        int matsysnbr = 0;
        int clisysnbr = 0;

        //load all default items
        private void ClientForm_Load(object sender, EventArgs e)
        {
            dateTimePickerOpened.Value = DateTime.Now; //OpenedDate

            textBoxCode.Text = clicode;

            //see if a default exists and keep the ID for later use
            string sql = "select id from defaults where IsStandard = 'Y'";
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            if (dds != null && dds.Tables.Count > 0)
            {
                foreach (DataRow dr in dds.Tables[0].Rows)
                {
                    presetID = Convert.ToInt32(dr[0].ToString());
                }
            } //else its not there so add it

            DataSet myRSPC2 = new DataSet();

            //if clicode is Numeric then increment by 1
            if (clisysnbr != 0)
            {
                sql = "SELECT matCode as CL from ( select max(matsysnbr) as MC FROM matter) kkf inner join matter on kkf.MC = matter.matsysnbr where matclinbr = " + clisysnbr.ToString();
                myRSPC2 = _jurisUtility.RecordsetFromSQL(sql);
                if (myRSPC2.Tables[0].Rows.Count != 0)
                {
                    string lastCliCode = "";
                    foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                        lastCliCode = dr["CL"].ToString();
                    try //if isnumeric
                    {
                        int test = Convert.ToInt32(lastCliCode);
                        textBoxMatterCode.Text = (test + 1).ToString();
                    }
                    catch (Exception ex1)
                    { }
                }
                myRSPC2.Clear();
            }


            //Office
            comboBoxOffice.ClearItems();
            
            string SQLPC2 = "select OfcOfficeCode as OfficeCode from OfficeCode order by OfcOfficeCode";
            myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2.Tables[0].Rows.Count == 0)
            {
                error = new ExceptionHandler();
                error.errorMessage = "There are no Office Codes. Correct and run the tool again";
                errorList.Add(error);
            }
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                    comboBoxOffice.Items.Add(dr["OfficeCode"].ToString());
                comboBoxOffice.SelectedIndex = 0;
            }

            //pract Class
            comboBoxPC.ClearItems();
            myRSPC2.Clear();
            SQLPC2 = "select PrctClsCode as PC from PracticeClass order by PrctClsCode";
             myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2.Tables[0].Rows.Count == 0)
            {
                error = new ExceptionHandler();
                error.errorMessage = "There are no Practice Classes. Correct and run the tool again";
                errorList.Add(error);
            }
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                    comboBoxPC.Items.Add(dr["PC"].ToString());
                comboBoxPC.SelectedIndex = 0;
            }

            //All Tkprs
            comboBoxBT.ClearItems();
            comboBoxOT1.ClearItems();
            comboBoxOT2.ClearItems();
            comboBoxOT3.ClearItems();
            comboBoxOT4.ClearItems();
            comboBoxOT5.ClearItems();
            comboBoxRT.ClearItems();
            myRSPC2.Clear();
            SQLPC2 = "select empinitials,empid + '    ' + empname as emp from employee where empvalidastkpr='Y' order by empinitials, empid";
            myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2.Tables[0].Rows.Count == 0)
            {
                error = new ExceptionHandler();
                error.errorMessage = "There are no valid Timekeepers. Correct and run the tool again";
                errorList.Add(error);
            }
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                {
                    comboBoxBT.Items.Add(dr["emp"].ToString());
                    comboBoxRT.Items.Add(dr["emp"].ToString());
                    comboBoxOT1.Items.Add(dr["emp"].ToString());
                    comboBoxOT2.Items.Add(dr["emp"].ToString());
                    comboBoxOT3.Items.Add(dr["emp"].ToString());
                    comboBoxOT4.Items.Add(dr["emp"].ToString());
                    comboBoxOT5.Items.Add(dr["emp"].ToString());

                }
                comboBoxBT.SelectedIndex = 0;
                comboBoxRT.SelectedIndex = 0;
                comboBoxOT1.SelectedIndex = 0;
                comboBoxOT2.SelectedIndex = 0;
                comboBoxOT3.SelectedIndex = 0;
                comboBoxOT4.SelectedIndex = 0;
                comboBoxOT5.SelectedIndex = 0;
            }

            //FeeSch
            string defFeeSched = "";
            string defExpSch = "";
            //get default from sysparam
            myRSPC2.Clear();
            SQLPC2 = "select SpTxtValue from sysparam where spname = 'CfgTransOpts'";
            myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);
            if (myRSPC2.Tables[0].Rows.Count == 0)
            {
                error = new ExceptionHandler();
                error.errorMessage = "Fee or Exp Schedule Standard in sysparam invalid (CfgTransOpts). Correct and run the tool again";
                errorList.Add(error);
            }
            else
            {
                string[] items = myRSPC2.Tables[0].Rows[0][0].ToString().Split(',');
                defFeeSched = items[6];
                defExpSch = items[7];
            }

            comboBoxFeeSched.ClearItems();
            myRSPC2.Clear();
            SQLPC2 = "select FeeSchCode as FS from FeeSchedule where FeeSchActive = 'Y' order by FeeSchCode ";
            myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2.Tables[0].Rows.Count == 0)
            {
                error = new ExceptionHandler();
                error.errorMessage = "There are no Fee Schedules. Correct and run the tool again";
                errorList.Add(error);
            }
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                    comboBoxFeeSched.Items.Add(dr["FS"].ToString());
                comboBoxFeeSched.SelectedIndex = comboBoxFeeSched.FindStringExact(defFeeSched);
            }

            comboBoxExpSched.ClearItems();
            myRSPC2.Clear();
            SQLPC2 = "select ExpSchCode as ES from ExpenseSchedule order by ExpSchCode";
            myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2.Tables[0].Rows.Count == 0)
            {
                error = new ExceptionHandler();
                error.errorMessage = "There are no Expense Schedules. Correct and run the tool again";
                errorList.Add(error);
            }
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                    comboBoxExpSched.Items.Add(dr["ES"].ToString());
                comboBoxExpSched.SelectedIndex = comboBoxExpSched.FindStringExact(defExpSch);
            }

            //Task XRef
            comboBoxTXRef.ClearItems();
            myRSPC2.Clear();
            SQLPC2 = "select TCXLList as FS from TaskCodeXrefList order by TCXLList ";
            myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2.Tables[0].Rows.Count == 0)
            {
                checkBoxTaskXRef.Checked = false;
                checkBoxTaskXRef.Enabled = false;
            }
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                    comboBoxTXRef.Items.Add(dr["FS"].ToString());
                comboBoxTXRef.SelectedIndex = 0;
            }

            //Exp XRef
            comboBoxEXRef.ClearItems();
            myRSPC2.Clear();
            SQLPC2 = "select ECXLList as FS from ExpCodeXrefList order by ECXLList ";
            myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2.Tables[0].Rows.Count == 0)
            {
                checkBoxExpXRef.Checked = false;
                checkBoxExpXRef.Enabled = false;
            }
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                    comboBoxEXRef.Items.Add(dr["FS"].ToString());
                comboBoxEXRef.SelectedIndex = 0;
            }

            //bill layout/prebill layout
            comboBoxBillLayout.ClearItems();
            comboBoxPreBillLayout.ClearItems();
            myRSPC2.Clear();
            SQLPC2 = "select BLCode as ES from BillLayout where blcode <> '{--}' order by BLCode";
            myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2.Tables[0].Rows.Count == 0)
            {
                error = new ExceptionHandler();
                error.errorMessage = "There are no Bill Layouts. Correct and run the tool again";
                errorList.Add(error);
            }
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                {
                    comboBoxBillLayout.Items.Add(dr["ES"].ToString());
                    comboBoxPreBillLayout.Items.Add(dr["ES"].ToString());
                }
                //now default to the most recent one
                myRSPC2.Clear();
                SQLPC2 = "SELECT top 1 BillToBillFormat as ES, count(BillToBillFormat) as Total FROM BillTo group by BillToBillFormat order by count(BillToBillFormat) desc";
                myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                    comboBoxBillLayout.SelectedIndex = comboBoxBillLayout.FindStringExact(dr["ES"].ToString());

                myRSPC2.Clear();
                SQLPC2 = "SELECT top 1 BillToEditFormat as ES, count(BillToEditFormat) as Total FROM BillTo group by BillToEditFormat order by count(BillToEditFormat) desc";
                myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                    comboBoxPreBillLayout.SelectedIndex = comboBoxPreBillLayout.FindStringExact(dr["ES"].ToString());
            }

            //bill Agreement
            comboBoxBAgree.ClearItems();
            comboBoxBAgree.Items.Add("H    Hourly");
            comboBoxBAgree.Items.Add("B    ProBono");
            comboBoxBAgree.Items.Add("C    Contingency");
            comboBoxBAgree.Items.Add("F    Flat Fee");
            comboBoxBAgree.Items.Add("N    Non-Billable");
            comboBoxBAgree.Items.Add("R    Retainer");
            comboBoxBAgree.Items.Add("T    Task Billing");
            comboBoxBAgree.SelectedIndex = 0;

            //Retainer type
            comboBoxRetainerType.ClearItems();
            comboBoxRetainerType.Items.Add("1    Total Bill");
            comboBoxRetainerType.Items.Add("2    Fee Total");
            comboBoxRetainerType.Items.Add("3    Minimum Bill");
            comboBoxRetainerType.Items.Add("4    Minimum Fee");
            comboBoxRetainerType.SelectedIndex = 0;

            //fee/exp frequency
            comboBoxFeeFreq.ClearItems();
            comboBoxFeeFreq.Items.Add("M    Monthly");
            comboBoxFeeFreq.Items.Add("Q    Quarterly");
            comboBoxFeeFreq.Items.Add("S    Semi-Annual");
            comboBoxFeeFreq.Items.Add("A    Annual");
            comboBoxFeeFreq.Items.Add("C    Cycle");
            comboBoxFeeFreq.Items.Add("R    On Request");
            comboBoxFeeFreq.SelectedIndex = 0;
            comboBoxExpFreq.ClearItems();
            comboBoxExpFreq.Items.Add("M    Monthly");
            comboBoxExpFreq.Items.Add("Q    Quarterly");
            comboBoxExpFreq.Items.Add("S    Semi-Annual");
            comboBoxExpFreq.Items.Add("A    Annual");
            comboBoxExpFreq.Items.Add("C    Cycle");
            comboBoxExpFreq.Items.Add("R    On Request");
            comboBoxExpFreq.SelectedIndex = 0;

            //threshold
            comboBoxThreshMain.ClearItems();
            comboBoxThreshMain.Items.Add("0    No thresholds entered");
            comboBoxThreshMain.Items.Add("1    Fee Amount, No Expense");
            comboBoxThreshMain.Items.Add("2    Expense Amount, No Fee");
            comboBoxThreshMain.Items.Add("3    Do not include fee/expense if threshold is met");
            comboBoxThreshMain.Items.Add("5    Fee Threshold, Include Expense");
            comboBoxThreshMain.Items.Add("7    Include expense, not fees if threshold is met");
            comboBoxThreshMain.Items.Add("10   Expense Threshold, Include Fee");
            comboBoxThreshMain.Items.Add("11   Include fee, not expense if threshold is met");
            comboBoxThreshMain.Items.Add("15   Include fee and expense if threshold is met");
            comboBoxThreshMain.SelectedIndex = 0;

            //discount options
            comboBoxDisc.ClearItems();
            comboBoxDisc.Items.Add("0    No discount");
            comboBoxDisc.Items.Add("1    % of fee");
            comboBoxDisc.Items.Add("2    % of bill");
            comboBoxDisc.SelectedIndex = 0;

            //surcharge options
            comboBoxSurcharge.ClearItems();
            comboBoxSurcharge.Items.Add("0    No surcharge");
            comboBoxSurcharge.Items.Add("1    % of fee");
            comboBoxSurcharge.Items.Add("2    % of expense");
            comboBoxSurcharge.Items.Add("3    % of bill");
            comboBoxSurcharge.SelectedIndex = 0;





            if (errorList.Count > 0)
            {
                string allErrors = "";
                foreach (ExceptionHandler ee in errorList)
                    allErrors = allErrors + error.errorMessage + "\r\n";
                MessageBox.Show("There were issues loading the Form. See below for details:" + "\r\n" + allErrors, "Form Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(1);
            }
            else
            {
                if (presetID != 0)
                    loadDfaultPreset();
                if (isModification)
                {
                    buttonCreateClient.Text = "Save Preset";
                    buttonCreateClient.Click -=  button1_Click;
                    buttonCreateClient.Click += buttonModify;
                }
            }




            //                dtOpen.Visible = checkBoxSetDate.Checked;
            //NewDR = dtOpen.Value.Date.ToString("MM/dd/yyyy");
            //if (cbOT.SelectedIndex > 0)
            //  OT = this.cbOT.GetItemText(this.cbOT.SelectedItem).Split(' ')[0];
        }

        private void loadDfaultPreset()
        {
            string sql = "select name, data, entrytype from DefaultSettings where defaultid = " + presetID.ToString();
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            if (dds != null && dds.Tables.Count > 0)
            {
                foreach (DataRow dr in dds.Tables[0].Rows)
                {
                    if (dr[2].ToString().Equals("textBox"))
                    {
                        foreach (var textbox in this.Controls.OfType<TextBox>())
                        {
                            if (dr[0].ToString().Equals(textbox.Name))
                                textbox.Text = dr[1].ToString();
                        }

                    }
                    else if (dr[2].ToString().Equals("comboBox"))
                    {
                        foreach (var cbox in this.Controls.OfType<ComboBox>())
                        {

                            if (dr[0].ToString().Equals(cbox.Name))
                                cbox.SelectedIndex = cbox.FindStringExact(dr[1].ToString());
                        }
                    }
                    else if (dr[2].ToString().Equals("checkBox"))
                    {
                        foreach (var textbox in this.Controls.OfType<CheckBox>())
                        {
                            if (dr[0].ToString().Equals(textbox.Name))
                            {
                                if (dr[1].ToString().Equals("Y"))
                                    textbox.Checked = true;
                                else
                                    textbox.Checked = false;

                            }
                        }
                    }
                    else if (dr[2].ToString().Equals("richTextBox"))
                    {
                        foreach (var textbox in this.Controls.OfType<RichTextBox>())
                        {
                            if (dr[0].ToString().Equals(textbox.Name))
                                textbox.Text = dr[1].ToString();
                        }
                    }
                }

            } //else its not there so add it



        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ds = MessageBox.Show("This will clear anything already on the Client form. Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            DataSet ds1;
            if (ds == DialogResult.Yes)
            {
                checkForTables();
                string sql = "select ID, name as [Default Name], PopulateMatter as [Populate Matter],  convert(varchar,CreationDate, 101) as [Creation Date], isStandard as [Default] from Defaults where DefType = 'C'";
                ds1 = _jurisUtility.RecordsetFromSQL(sql);
                PresetManager DM = new PresetManager(ds1, _jurisUtility, "M");
                DM.Show();
                this.Close();
            }
        }

        private void clearFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MatterForm cleared = new MatterForm(_jurisUtility, 0, false, 0 ,"");
            cleared.Show();
            this.Close();
        }

        private void checkForTables()
        {
            string sql = "IF  NOT EXISTS (SELECT * FROM sys.objects " +
            " WHERE object_id = OBJECT_ID(N'[dbo].[Defaults]') AND type in (N'U')) " +
            " BEGIN " +
            " Create Table[dbo].[Defaults](ID int, name varchar(300), PopulateMatter char,  CreationDate datetime, IsStandard char, DefType char) " +
            " END";

            _jurisUtility.ExecuteSqlCommand(0, sql);

            sql = "IF  NOT EXISTS (SELECT * FROM sys.objects " +
            " WHERE object_id = OBJECT_ID(N'[dbo].[DefaultSettings]') AND type in (N'U')) " +
            " BEGIN " +
            " Create Table [dbo].[DefaultSettings] (DefaultID int, name varchar(50), data varchar(255), entryType varchar(50)) " +
            " END";

            _jurisUtility.ExecuteSqlCommand(0, sql);

        }

        private void clearFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkForTables();
            checkDefaultName();
        }

        private void checkDefaultName()
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Name of new default", "Default Name", "New Default");
            if (!string.IsNullOrEmpty(name))
            {
                //see if default name already exists
                string sql = "select name from defaults";
                DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                bool exists = false;
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        if (name.Equals(dr[0].ToString(), StringComparison.OrdinalIgnoreCase))
                            exists = true;
                    }
                } //else its not there so add it
                if (!exists)
                    createDefault(name);
                else
                    MessageBox.Show("Names must be unique and that name already exists. Default not added", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("A valid name is required. Default not added", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void createDefault(string name)
        {
            string sql = "insert into defaults (ID, name, PopulateMatter, CreationDate, IsStandard, DefType ) " +
                " values ((case when (select max(ID) from defaults) is null then 1 else ((select max(ID) from defaults) + 1) end), '" + name + "', 'N', getdate(), 'N', 'C')";

            _jurisUtility.ExecuteNonQuery(0, sql);

            sql = "select max(id) from defaults";
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            int defID = 0;
            if (dds != null && dds.Tables.Count > 0)
            {
                foreach (DataRow dr in dds.Tables[0].Rows)
                {
                    defID = Convert.ToInt32(dr[0].ToString());
                }
            } //else its not there so add it

            foreach (var textbox in this.Controls.OfType<TextBox>())
            {
                if (!string.IsNullOrEmpty(textbox.Text) && !textbox.Name.Equals("textBoxCode") && !textbox.Name.Equals("textBoxMatterCode"))
                {
                    sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (" + defID + ", '" + textbox.Name + "', '" + textbox.Text + "', 'textBox' )";
                    _jurisUtility.ExecuteNonQuery(0, sql);

                }
            }
            foreach (var cbox in this.Controls.OfType<ComboBox>())
            {
                sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (" + defID + ", '" + cbox.Name + "', '" + cbox.GetItemText(cbox.SelectedItem) + "', 'comboBox' )";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }

            foreach (var textbox in this.Controls.OfType<CheckBox>())
            {
                string isChecked = ((bool?)textbox.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (" + defID + ", '" + textbox.Name + "', '" + isChecked + "', 'checkBox' )";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }

            foreach (var textbox in this.Controls.OfType<RichTextBox>())
            {
                sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (" + defID + ", '" + textbox.Name + "', '" + textbox.Text + "', 'richTextBox' )";
                _jurisUtility.ExecuteNonQuery(0, sql);
            } 
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string sysparam = "  select SpTxtValue from sysparam where SpName = 'FldMatter'";
            DataSet dds2 = _jurisUtility.RecordsetFromSQL(sysparam);
            int clientLength = 5;
            string cell = "";
            if (dds2 != null && dds2.Tables.Count > 0)
            {
                foreach (DataRow dr in dds2.Tables[0].Rows)
                {
                    cell = dr[0].ToString();
                }

            }
            string[] test = cell.Split(',');
            string codesql = "";



                        if (test[1].Equals("C"))
                        {
                            clientLength = Convert.ToInt32(test[2]);
                            codesql = "000000000000" + textBoxMatterCode.Text;
                            codesql = codesql.Substring(codesql.Length - clientLength, clientLength);
                        }
                        else
                        {

                            codesql = "000000000000" + textBoxMatterCode.Text;
                            codesql = codesql.Substring(codesql.Length - 12, 12);
                        }



                        textBoxMatterCode.Text = codesql;

            createMatter();
            
            //get clisysnbr and pass to matter form
        }

        private void buttonModify(object sender, EventArgs e)
        {
            string sql = "delete from DefaultSettings where defaultid = " + presetID.ToString();
            _jurisUtility.ExecuteNonQuery(0, sql);

            foreach (var textbox in this.Controls.OfType<TextBox>())
            {
                if (!string.IsNullOrEmpty(textbox.Text) && !textbox.Name.Equals("textBoxCode") && !textbox.Name.Equals("textBoxMatterCode"))
                {
                    sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (" + presetID + ", '" + textbox.Name + "', '" + textbox.Text + "', 'textBox' )";
                    _jurisUtility.ExecuteNonQuery(0, sql);

                }
            }
            foreach (var cbox in this.Controls.OfType<ComboBox>())
            {
                sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (" + presetID + ", '" + cbox.Name + "', '" + cbox.GetItemText(cbox.SelectedItem) + "', 'comboBox' )";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }

            foreach (var textbox in this.Controls.OfType<CheckBox>())
            {
                string isChecked = ((bool?)textbox.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (" + presetID + ", '" + textbox.Name + "', '" + isChecked + "', 'checkBox' )";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }

            foreach (var textbox in this.Controls.OfType<RichTextBox>())
            {
                sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (" +presetID + ", '" + textbox.Name + "', '" + textbox.Text + "', 'richTextBox' )";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }
            sql = "select ID, name as [Default Name], PopulateMatter as [Populate Matter],  convert(varchar,CreationDate, 101) as [Creation Date], isStandard as [Default] from Defaults where DefType = 'C'";
            DataSet ds1 = _jurisUtility.RecordsetFromSQL(sql);
            PresetManager DM = new PresetManager(ds1, _jurisUtility, "M");
            DM.Show();
            this.Close();

        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void comboBoxBAgree_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool preCheckedState = false;
            if (this.comboBoxBAgree.GetItemText(this.comboBoxBAgree.SelectedItem).Split(' ')[0].Equals("T")) //task billing requires task codes
            {
                preCheckedState = checkBoxReqTaskCodes.Checked;
                checkBoxReqTaskCodes.Checked = true;
                checkBoxReqTaskCodes.Enabled = false;
            }
            else
            {
                checkBoxReqTaskCodes.Enabled = true;
                checkBoxReqTaskCodes.Checked = preCheckedState; // returns it to whatever state it was before the change
            }
            if (this.comboBoxBAgree.GetItemText(this.comboBoxBAgree.SelectedItem).Split(' ')[0].Equals("F")) // flat fee gives them the option to include exps
                checkBoxIncludeExp.Visible = true;
            else
                checkBoxIncludeExp.Visible = false;
            if (this.comboBoxBAgree.GetItemText(this.comboBoxBAgree.SelectedItem).Split(' ')[0].Equals("R")) //retainer requires retainer type
            {
                labelRet.Visible = true;
                comboBoxRetainerType.Visible = true;
            }
            else
            {
                labelRet.Visible = false;
                comboBoxRetainerType.Visible = false;
            }
        }

        private void comboBoxFeeFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("C")) //task billing requires task codes
            {
                labelCycle.Visible = true;
                textBoxCycle.Visible = true;
            }
            else
            {
                labelCycle.Visible = false;
                textBoxCycle.Visible = false;
            }
        }

        private void comboBoxExpFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("C")) //task billing requires task codes
            {
                labelCycle.Visible = true;
                textBoxCycle.Visible = true;
            }
            else
            {
                labelCycle.Visible = false;
                textBoxCycle.Visible = false;
            }
        }

        private void comboBoxThreshMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.comboBoxThreshMain.GetItemText(this.comboBoxThreshMain.SelectedItem).Split(' ')[0].Equals("0")) //if not "no threshold"
            {
                labelExpThresh.Visible = true;
                labelFeeThresh.Visible = true;
                textBoxExpThresh.Visible = true;
                textBoxFeeThresh.Visible = true;
            }
            else
            {
                labelExpThresh.Visible = false;
                labelFeeThresh.Visible = false;
                textBoxExpThresh.Visible = false;
                textBoxFeeThresh.Visible = false;
            }
        }

        private void comboBoxDisc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.comboBoxDisc.GetItemText(this.comboBoxDisc.SelectedItem).Split(' ')[0].Equals("0")) //if discount option selected (not 0)
            {
                labelDPct.Visible = true;
                textBoxDiscPct.Visible = true;
            }
            else
            {
                labelDPct.Visible = false;
                textBoxDiscPct.Visible = false;
            }
        }

        private void comboBoxSurcharge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.comboBoxSurcharge.GetItemText(this.comboBoxSurcharge.SelectedItem).Split(' ')[0].Equals("0")) //if surcharge option selected (not 0)
            {
                labelSPct.Visible = true;
                textBoxSurPct.Visible = true;
            }
            else
            {
                labelSPct.Visible = false;
                textBoxSurPct.Visible = false;
            }
        }

        private bool isNumeric(string value)
        {
            try
            {
                decimal test = Convert.ToDecimal(value);
                return true;
            }
            catch (Exception exx2)
            {
                return false;
            }

        }


        private bool checkFields()
        {
            List<string> incorrectFields = new List<string>();
            if (!isNumeric(textBoxMonth.Text))
            {
                textBoxMonth.Text = "1";
                incorrectFields.Add("Month");
            }
            if (!isNumeric(textBoxCycle.Text))
            {
                textBoxCycle.Text = "1";
                incorrectFields.Add("Cycle");
            }
            if (!isNumeric(textBoxIntDays.Text))
            {
                textBoxIntDays.Text = "0";
                incorrectFields.Add("Interest Days");
            }
            if (!isNumeric(textBoxIntPct.Text))
            {
                textBoxIntPct.Text = "0.00";
                incorrectFields.Add("Interest Pct");
            }
            if (!isNumeric(textBoxDiscPct.Text))
            {
                textBoxDiscPct.Text = "0.00";
                incorrectFields.Add("Discount Pct");
            }
            if (!isNumeric(textBoxSurPct.Text))
            {
                textBoxSurPct.Text = "0.00";
                incorrectFields.Add("Surcharge Pct");
            }
            if (!isNumeric(textBoxExpThresh.Text))
            {
                textBoxExpThresh.Text = "0.00";
                incorrectFields.Add("Expense Threshold");
            }
            if (!isNumeric(textBoxFeeThresh.Text))
            {
                textBoxFeeThresh.Text = "0.00";
                incorrectFields.Add("Fee Threshold");
            }

            //ensure no apostrophes or double quotes as they break sql
            foreach (var textbox in this.Controls.OfType<TextBox>())
                textbox.Text = textbox.Text.Replace("'", "").Replace("\"", "");

            foreach (var textbox in this.Controls.OfType<RichTextBox>())
                textbox.Text = textbox.Text.Replace("'", "").Replace("\"", "");

            if (incorrectFields.Count == 0)
            {
                foreach (var textbox in this.Controls.OfType<TextBox>())
                {
                    if (string.IsNullOrEmpty(textbox.Text)) //if there is nothing in it, is it required?
                    {
                        if (!textbox.Name.Equals("textBoxDesc") && !textbox.Name.Equals("textBoxFax") && !textbox.Name.Equals("textBoxBAFax") && !textbox.Name.Equals("textBoxBACountry") && !textbox.Name.Equals("textBoxBAEmail"))
                        {
                            MessageBox.Show("All fields in black text are required. Please correct this issue and retry", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                    }

                }

                    if (testOrigPct())
                        return true;
                    else
                        return false;

            }

            else
            {
                string items = "";
                foreach (string dd in incorrectFields)
                    items = items + dd + " ";
                MessageBox.Show("All numeric fields must have a number in them." + "\r\n" + "The following fields are invalid and will be reset" + "\r\n" + items + "\r\n" + "Please adjust if needed and continue.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


                return false;
            }

        }

        private void createMatter()
        {

            if (checkFields())
            {

                //see if they use char or numeric codes
                string sysparam = "  select SpTxtValue from sysparam where SpName = 'FldMatter'";
                DataSet dds = _jurisUtility.RecordsetFromSQL(sysparam);
                int clientLength = 5;
                string cell = "";
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        cell = dr[0].ToString();
                    }

                }
                string[] test = cell.Split(',');
                string codesql = "";
                if (test[1].Equals("C"))
                {
                    clientLength = Convert.ToInt32(test[2]);
                    codesql = " right('000000000000' + '" + textBoxMatterCode.Text.Trim() + "', " + clientLength + ") ";
                }
                else
                {

                    codesql = " right('000000000000' + '" + textBoxMatterCode.Text.Trim() + "', 12) ";

                }

                string txref = "null";
                if (checkBoxTaskXRef.Checked)
                    txref = "'" + this.comboBoxTXRef.GetItemText(this.comboBoxTXRef.SelectedItem).Split(' ')[0] + "'";

                string exref = "null";
                if (checkBoxExpXRef.Checked)
                    exref = "'" + this.comboBoxEXRef.GetItemText(this.comboBoxEXRef.SelectedItem).Split(' ')[0] + "'";

                string inclExp = ((bool?)checkBoxIncludeExp.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string tax1 = ((bool?)checkBoxTax1.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string tax2 = ((bool?)checkBoxTax2.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string tax3 = ((bool?)checkBoxTax3.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string budg = ((bool?)checkBoxBudget.Checked) == true ? '1'.ToString() : '0'.ToString();
                string reqTask = ((bool?)checkBoxReqTaskCodes.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string reqAct = ((bool?)checkBoxReqActCodes.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string reqTaskOnExp = ((bool?)checkBoxReqTaskOnExp.Checked) == true ? 'Y'.ToString() : 'N'.ToString();



                string retType = ((bool?)comboBoxRetainerType.Visible) == true ? this.comboBoxRetainerType.GetItemText(this.comboBoxRetainerType.SelectedItem).Split(' ')[0] : string.Empty;


                //this.comboBoxBT.GetItemText(this.comboBoxBT.SelectedItem).Split(' ')[0] 

                //add addy
                // (ensure no conflict)
                //add billto (save id )
                //add billcopy
                int billtoid = createAddy();

                if (billtoid != 0)
                {
                    //add matter
                    //add orig

                    string sql = "Insert into Matter(MatSysNbr,MatCliNbr,MatBillTo,MatCode,MatNickName,MatReportingName,MatDescription, " +
                        " MatRemarks,MatPhoneNbr,MatFaxNbr,MatContactName,MatDateOpened,MatStatusFlag,MatLockFlag, "
       + "  MatDateClosed,MatOfficeCode,MatPracticeClass,MatFeeSch,MatTaskCodeXref,MatExpSch,MatExpCodeXref,MatQuickAction, " +
       " MatBillAgreeCode,MatFlatFeeIncExp,MatRetainerType,MatFltFeeOrRetainer,MatExpFreqCode,MatFeeFreqCode,MatBillMonth,MatBillCycle,"
      + "   MatExpThreshold,MatFeeThreshold,MatInterestPcnt,MatInterestDays,MatDiscountOption,MatDiscountPcnt,MatSurchargeOption,MatSurchargePcnt,MatSplitMethod,MatSplitThreshold,"
       + "  MatSplitPriorAmtBld,MatBudgetOption,MatBudgetPhase,MatReqPhaseOnTrans,MatReqTaskCdOnTime,MatReqActyCdOnTime,MatReqTaskCdOnExp,MatTax1Exempt,MatTax2Exempt,MatTax3Exempt,MatDateLastWork,MatDateLastExp"
       + "  , MatDateLastBill,MatDateLastStmt,MatDateLastPaymt,MatLastPaymtAmt,MatARLastBill,MatPaySinceLastBill,MatAdjSinceLastBill,MatPPDBalance, " +
       " MatVisionAddr,MatThresholdOption,MatType,MatBillingField01,MatBillingField02,"
      + "   MatBillingField03,MatBillingField04,MatBillingField05,MatBillingField06,MatBillingField07,MatBillingField08,MatBillingField09,MatBillingField10,MatBillingField11,MatBillingField12,MatBillingField13,MatBillingField14,MatBillingField15,MatBillingField16,"
       + "  MatBillingField17,MatBillingField18,MatBillingField19,MatBillingField20,MatCTerms,MatCStatus,MatCStatus2) "
       + "     values( case when (select max(MatSysNbr) from matter) is null then 1 else ((select max(MatSysNbr) from matter) + 1) end, " + clisysnbr + ", " + billtoid.ToString() + ",  "
       + "       " + codesql + ", '" + textBoxNName.Text.Trim() + "', '" + textBoxRName.Text.Trim() + "',  '" + textBoxDesc.Text.Trim() + "', " +
       " '', '" + textBoxPhone.Text.Trim() + "', '" + textBoxFax.Text.Trim() + "', '" + textBoxContact.Text.Trim() + "', '" + dateTimePickerOpened.Value.ToString("MM/dd/yyyy") + "','O' ,'0', "
     + " '01/01/1900','" + this.comboBoxOffice.GetItemText(this.comboBoxOffice.SelectedItem).Split(' ')[0] + "','" + this.comboBoxPC.GetItemText(this.comboBoxPC.SelectedItem).Split(' ')[0] + "','" + this.comboBoxFeeSched.GetItemText(this.comboBoxFeeSched.SelectedItem).Split(' ')[0] + "'," + txref + ",'" + this.comboBoxExpSched.GetItemText(this.comboBoxExpSched.SelectedItem).Split(' ')[0] + "'," + exref + ",0, " 
      +     "'" +  this.comboBoxBAgree.GetItemText(this.comboBoxBAgree.SelectedItem).Split(' ')[0] + "','" + inclExp + "','" + retType + "', 0.00, '" + this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0] + "', '" + this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0] + "' ," + textBoxMonth.Text + "," + textBoxCycle.Text + ", " 
 + textBoxExpThresh.Text + "," + textBoxFeeThresh.Text + "," + textBoxIntPct.Text + "," + textBoxIntDays.Text + "," + this.comboBoxDisc.GetItemText(this.comboBoxDisc.SelectedItem).Split(' ')[0] + "," + textBoxDiscPct.Text + ", " + this.comboBoxSurcharge.GetItemText(this.comboBoxSurcharge.SelectedItem).Split(' ')[0] + ", " + textBoxSurPct.Text + ", 0, 0.00,"
      + "0.00," + budg + ",0, 'N','" + reqTask + "','" + reqAct + "','" + reqTaskOnExp + "','" + tax1 + "','" + tax2 + "','" + tax3 + "',"

    + " '01/01/1900','01/01/1900','01/01/1900','01/01/1900','01/01/1900',0.00,0.00,0.00,0.00,0.00,0,0,0,"
     + " '','','','','','','', '','','','','','','','','', '', '', '', '', 0, 0, '')";

                     TextWriter ss = new StreamWriter(@"c:\intel\sql1.txt");
                     ss.Write(sql);
                     ss.Flush();
                     ss.Close();
                    _jurisUtility.ExecuteNonQuery(0, sql);

                    sql = "update sysparam set spnbrvalue = (select max(matsysnbr) from matter) where spname = 'LastSysNbrMatter'";
                    _jurisUtility.ExecuteNonQuery(0, sql);


                    addOrig(codesql);

                    if (presetID != 0)
                        loadDfaultPreset();
                }
            }


            // TextWriter ss = new StreamWriter(@"c:\intel\sql1.txt");
            // ss.Write(sql);
            // ss.Flush();
            // ss.Close();


        }

        private int createAddy()
        {
            //get clisysbvr if we dont have it yet (clicked on matter only)

            if (clisysnbr == 0)
            {
                string sql = "select clisysnbr from client where dbo.jfn_FormatClientCode(clicode) = '" + textBoxCode.Text + "'";
                DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        clisysnbr = Convert.ToInt32(dr[0].ToString());
                    }

                }



            }

            if (clisysnbr == 0)
            {
                MessageBox.Show("Client " + textBoxCode.Text + " does not exist. Enter a valid client code." + "\r\n" + "Keep in mind it must match exactly as it appears in Juris including leading zeroes", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            else
            {
                //see if matter number exists
                int matsys = 0;
                string sql = "select matsysnbr from matter where matclinbr = " + clisysnbr.ToString() + " and dbo.jfn_FormatMatterCode(matcode) = '" + textBoxMatterCode.Text + "'";
                DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        matsys = Convert.ToInt32(dr[0].ToString());
                    }

                }
                if (matsys != 0)
                {
                    MessageBox.Show("That matter is already used for that client" + "\r\n" + "Enter a new and unique matter code", "Constraint Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
                else
                {
                    //see if this combo os nickname/clisys exists
                    string test = "select BilAdrSysNbr from BillingAddress where BilAdrNickName = '" + textBoxBANName.Text + "' and BilAdrCliNbr = " + clisysnbr.ToString(); ;
                    DataSet dds1 = _jurisUtility.RecordsetFromSQL(test);
                    if (dds1 != null && dds1.Tables.Count > 0 && dds1.Tables[0].Rows.Count != 0)
                    {
                        MessageBox.Show("That nickname is already used for that client" + "\r\n" + "Enter a new and unique nickname", "Constraint Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 0;
                    }
                    else
                    {

                        string addy = richTextBoxBAAddy.Text.Replace("\r", "|").Replace("\n", "|");
                        addy = addy.Replace("||", "|");


                        sql = "Insert into BillingAddress(BilAdrSysNbr, BilAdrCliNbr, BilAdrUsageFlg, BilAdrNickName, BilAdrPhone, " +
                            " BilAdrFax, BilAdrContact, BilAdrName, BilAdrAddress, BilAdrCity, BilAdrState, BilAdrZip, BilAdrCountry, BilAdrType, BilAdrEmail) " +
                            " values (case when(select max(biladrsysnbr) from billingaddress) is null then 1 else ((select max(biladrsysnbr) from billingaddress) +1) end, " + clisysnbr + ", " +
                            " 'C', '" + textBoxBANName.Text + "', '" + textBoxBAPhone.Text + "', "
                             + "  '" + textBoxBAFax.Text + "', '" + textBoxBAContact.Text + "', " +
                            " '" + textBoxBAName.Text + "', " +
                            "replace('" + addy + "', '|', char(13) + char(10)), "
                            + " '" + textBoxBACity.Text + "', '" + textBoxBAState.Text + "', '" + textBoxBAZip.Text + "','" + textBoxBACountry.Text + "', 0, '" + textBoxBAEmail.Text + "')";

                        _jurisUtility.ExecuteNonQuery(0, sql);


                        sql = "update sysparam set spnbrvalue = (select max(biladrsysnbr) from billingaddress) where spname = 'LastSysNbrBillAddress'";
                        _jurisUtility.ExecuteNonQuery(0, sql);

                        sql = "select max(biladrsysnbr) from billingaddress";
                        dds.Clear();
                        int addyid = 0;
                        dds = _jurisUtility.RecordsetFromSQL(sql);
                        if (dds != null && dds.Tables.Count > 0)
                        {
                            foreach (DataRow dr in dds.Tables[0].Rows)
                            {
                                addyid = Convert.ToInt32(dr[0].ToString());
                            }

                        }

                        string resp = "null";
                        if (checkBoxRT.Checked)
                            resp = " (select empsysnbr from employee where empid = '" + this.comboBoxRT.GetItemText(this.comboBoxRT.SelectedItem).Split(' ')[0] + "')";

                        //create billto
                        sql = "Insert into BillTo (BillToSysNbr,BillToCliNbr,BillToUsageFlg,BillToNickName,BillToBillingAtty,BillToBillFormat,BillToEditFormat,BillToRespAtty) " +
                            "values (case when(select max(billtosysnbr) from billto) is null then 1 else ((select max(billtosysnbr) from billto) +1) end, " + clisysnbr.ToString() +
                            ",  'M', '" + textBoxBANName.Text + "', (select empsysnbr from employee where empid = '" + this.comboBoxBT.GetItemText(this.comboBoxBT.SelectedItem).Split(' ')[0] + "'), " +
                            " '" + this.comboBoxBillLayout.GetItemText(this.comboBoxBillLayout.SelectedItem).Split(' ')[0] + "', '" + this.comboBoxPreBillLayout.GetItemText(this.comboBoxPreBillLayout.SelectedItem).Split(' ')[0] + "', " + resp + ")";

                        _jurisUtility.ExecuteNonQuery(0, sql);

                        sql = "select max(billtosysnbr) from billto";
                        dds.Clear();
                        int billto = 0;
                        dds = _jurisUtility.RecordsetFromSQL(sql);
                        if (dds != null && dds.Tables.Count > 0)
                        {
                            foreach (DataRow dr in dds.Tables[0].Rows)
                            {
                                billto = Convert.ToInt32(dr[0].ToString());
                            }

                        }

                        sql = "update sysparam set spnbrvalue = (select max(billtosysnbr) from billto) where spname = 'LastSysNbrBillTo'";
                        _jurisUtility.ExecuteNonQuery(0, sql);

                        //billcopy
                        sql = "Insert into BillCopy(BilCpyBillTo,BilCpyBilAdr,BilCpyComment,BilCpyNbrOfCopies,BilCpyPrintFormat,BilCpyEmailFormat,BilCpyExportFormat,BilCpyARFormat) "
                        + " values ( " + billto.ToString() + ", " + addyid.ToString() + " ,'',1,1,0,0,0 )";

                        _jurisUtility.ExecuteNonQuery(0, sql);

                        return billto;
                    }
                }
            }

        }

        private void addOrig(string matcode)
        {
            string sql = "";


            sql = "select matsysnbr from matter where matclinbr = " + clisysnbr.ToString() + " and matcode = " + matcode;
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            if (dds != null && dds.Tables.Count > 0)
            {
                foreach (DataRow dr in dds.Tables[0].Rows)
                {
                    matsysnbr = Convert.ToInt32(dr[0].ToString());
                }

            }

            if (!textBoxOTPct1.Text.Equals("0"))
            {
                sql = "insert into MatOrigAtty (MOrigMat, MOrigAtty, MOrigPcnt) values (" + matsysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT1.GetItemText(this.comboBoxOT1.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct1.Text + " as decimal(7,4)))";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }
            if (!textBoxOTPct2.Text.Equals("0"))
            {
                sql = "insert into MatOrigAtty (MOrigMat, MOrigAtty, MOrigPcnt) values (" + matsysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT2.GetItemText(this.comboBoxOT2.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct2.Text + " as decimal(7,4)))";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }
            if (!textBoxOTPct3.Text.Equals("0"))
            {
                sql = "insert into MatOrigAtty (MOrigMat, MOrigAtty, MOrigPcnt) values (" + matsysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT3.GetItemText(this.comboBoxOT3.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct3.Text + " as decimal(7,4)))";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }
            if (!textBoxOTPct4.Text.Equals("0"))
            {
                sql = "insert into MatOrigAtty (MOrigMat, MOrigAtty, MOrigPcnt) values (" + matsysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT4.GetItemText(this.comboBoxOT4.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct4.Text + " as decimal(7,4)))";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }
            if (!textBoxOTPct5.Text.Equals("0"))
            {
                sql = "insert into MatOrigAtty (MOrigMat, MOrigAtty, MOrigPcnt) values (" + matsysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT5.GetItemText(this.comboBoxOT5.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct5.Text + " as decimal(7,4)))";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }




            DialogResult fc = MessageBox.Show("Matter " + textBoxCode.Text + "/" + textBoxMatterCode.Text + " was added successfully." + "\r\n" + "Would you like to add another Matter to this Client?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (fc == DialogResult.Yes)
            {
                MatterForm cleared = new MatterForm(_jurisUtility, presetID, false, clisysnbr, textBoxCode.Text);
                cleared.Show();
                //move data over
                this.Close();



            }
        }

        private bool testOrigPct()
        {

            if (isNumeric(textBoxOTPct1.Text) && isNumeric(textBoxOTPct2.Text) && isNumeric(textBoxOTPct3.Text) && isNumeric(textBoxOTPct4.Text) && isNumeric(textBoxOTPct5.Text) && (Convert.ToInt32(textBoxOTPct1.Text) + Convert.ToInt32(textBoxOTPct2.Text) + Convert.ToInt32(textBoxOTPct3.Text) + Convert.ToInt32(textBoxOTPct4.Text) + Convert.ToInt32(textBoxOTPct5.Text) == 100))
                    return true;
            else
            {
                MessageBox.Show("All 5 percentages for Originators must be numeric and add to 100." + "\r\n" + "Resetting percentages to default. Please adjust if needed.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxOTPct1.Text = "100";
                textBoxOTPct2.Text = "0";
                textBoxOTPct3.Text = "0";
                textBoxOTPct4.Text = "0";
                textBoxOTPct5.Text = "0";
                return false;
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ExitDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxMatterCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxCode_MouseLeave(object sender, EventArgs e)
        {

        }

        private void textBoxCode_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxCode.Text) && clisysnbr == 0)
            {
                string sql = "select clisysnbr from client where dbo.jfn_FormatClientCode(clicode) = '" + textBoxCode.Text + "'";
                DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        clisysnbr = Convert.ToInt32(dr[0].ToString());
                    }
                }
                else
                    MessageBox.Show("That client does not exist. Re-enter a client that exists" + "\r\n" + "and remember that the code must mstch exactly as it appears in Juris including leading zeros", "Client Error", MessageBoxButtons.OK, MessageBoxIcon.Error);




                if (clisysnbr != 0)
                {
                    string sysparam = "  select SpTxtValue from sysparam where SpName = 'FldMatter'";
                    DataSet dds2 = _jurisUtility.RecordsetFromSQL(sysparam);
                    int clientLength = 5;
                    string cell = "";
                    if (dds2 != null && dds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in dds2.Tables[0].Rows)
                        {
                            cell = dr[0].ToString();
                        }

                    }
                    string[] test = cell.Split(',');
                    string codesql = "";


                    sql = " SELECT top 1 matcode, matsysnbr" +
                       "   FROM Matter" +
                       "   where matclinbr = " + clisysnbr +
                        "  order by matsysnbr desc";
                    DataSet dds1 = _jurisUtility.RecordsetFromSQL(sql);
                    if (dds1 != null && dds1.Tables.Count > 0)
                    {
                        foreach (DataRow dr in dds1.Tables[0].Rows)
                        {
                            if (isNumeric(dr[0].ToString()))
                            {
                                if (test[1].Equals("C"))
                                {
                                    clientLength = Convert.ToInt32(test[2]);
                                    codesql = "000000000000" + (Convert.ToInt32(dr[0].ToString()) + 1).ToString();
                                    codesql = codesql.Substring(codesql.Length - clientLength, clientLength);
                                }
                                else
                                {

                                    codesql = "000000000000" + (Convert.ToInt32(dr[0].ToString()) + 1).ToString();
                                    codesql = codesql.Substring(codesql.Length - 12, 12);
                                }



                                textBoxMatterCode.Text = codesql;
                            }
                        }
                    }


                }



            }
        }
    }
}
