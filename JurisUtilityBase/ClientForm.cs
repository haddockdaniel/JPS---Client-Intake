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

namespace JurisUtilityBase
{
    public partial class ClientForm : Form
    {
        public ClientForm(JurisUtility jutil)
        {
            InitializeComponent();
            _jurisUtility = jutil;
        }


        JurisUtility _jurisUtility;
        public List<ExceptionHandler> errorList = new List<ExceptionHandler>();
        ExceptionHandler error = null;

        //load all default items
        private void ClientForm_Load(object sender, EventArgs e)
        {
            dateTimePickerOpened.Value = DateTime.Now; //OpenedDate

            //Office
            comboBoxOffice.ClearItems();
            string SQLPC2 = "select OfcOfficeCode as OfficeCode from OfficeCode order by OfcOfficeCode";
            DataSet myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

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













            //                dtOpen.Visible = checkBoxSetDate.Checked;
            //NewDR = dtOpen.Value.Date.ToString("MM/dd/yyyy");
            //if (cbOT.SelectedIndex > 0)
            //  OT = this.cbOT.GetItemText(this.cbOT.SelectedItem).Split(' ')[0];
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sql = "select ID, name as [Default Name], PopulateMatter as [Populate Matter], Employee.empname as Creator, CreationDate as [Creation Date], isStandard as [Default] from Defaults inner join employee on empsysnbr = Creator";
            DataSet ds = _jurisUtility.RecordsetFromSQL(sql);
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
    }
}
