using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using Gizmox.Controls;
using System.Runtime.InteropServices;

namespace JurisUtilityBase
{
    public partial class MatterForm : Form
    {
        public MatterForm(JurisUtility jutil, int clisys, string cc, int adrSys, System.Drawing.Point ppt, int empsys)
        {
            InitializeComponent();
            _jurisUtility = jutil;
            addySysNbr = adrSys;
            clisysnbr = clisys;
            clicode = cc;
            pt = ppt;
            empsysnbr = empsys;
        }

        private System.Drawing.Point pt;
        JurisUtility _jurisUtility;

        string clicode = "";
        int addySysNbr = 0;
        public List<string> errorList = new List<string>();
        int clisysnbr = 0;
        bool isError = false;
        bool removeAddy = false;
        bool codeIsNumericClient = false;
        bool codeIsNumericMatter = false;
        int lengthOfCodeClient = 4;
        int lengthOfCodeMatter = 4;
        int numOfOrig = 5;
        int matsysnbr = 0;
        string noteName = "";
        string noteText = "";
        int empsysnbr = 0;
        bool addNoteCard = false;

        bool exitToMain = false;
        public const uint WM_NCHITTEST = 0x0084;
        public const int HTCLOSE = 20;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);


        //load all default items
        private void ClientForm_Load(object sender, EventArgs e)
        {
            if (!checkIfLoginInfoIsStillInDB())
            {
                MessageBox.Show("An Admin cleared all logins so you must log in again." + "\r\n" + "The application wil now close. Please reopen and log in.", "Credential Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                exitToMain = true;
                this.Close();
            }
            dateTimePickerOpened.Value = DateTime.Now; //OpenedDate
            this.Location = pt;
            textBoxCode.Text = clicode;

            if (addySysNbr == 0 || clisysnbr == 0)
            {
                checkBoxChooseAddy.Checked = false;
                checkBoxChooseAddy.Enabled = false;
                comboBoxAddyChoose.Enabled = false;
            }
            else
                loadAddys();

            getDefaultsForClientMatter(); // we need to know if they are numeric or alpha an how long they can be

            //if clicode is Numeric then increment by 1
            getNextMatterNumber();

            //get number of originators
            string sysparam = "  select SpTxtValue from sysparam where SpName = 'CfgTkprOpts'";
            DataSet dds2 = _jurisUtility.RecordsetFromSQL(sysparam); //the first character should be a number...if not, do nothing
            
            string[] temp = null;
            string cell = "";
            try
            {

                if (dds2 != null && dds2.Tables.Count > 0 && dds2.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dds2.Tables[0].Rows)
                    {
                        cell = dr[0].ToString();
                    }
                    temp = cell.Split(',');
                    numOfOrig = Convert.ToInt32(temp[0]);
                }



            }
            catch (Exception) { }

            hideOrShowOriginators(numOfOrig);


            //Office
            comboBoxOffice.ClearItems();
            DataSet myRSPC2 = new DataSet();
            string SQLPC2 = "select OfcOfficeCode + '    ' + right(OfcDesc, 30) as OfficeCode from OfficeCode order by OfcOfficeCode";
            myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2 == null || myRSPC2.Tables.Count == 0 || myRSPC2.Tables[0].Rows.Count == 0)
                errorList.Add("There are no Office Codes. Correct and run the tool again");
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                    comboBoxOffice.Items.Add(dr["OfficeCode"].ToString());
                comboBoxOffice.SelectedIndex = 0;
            }

            //pract Class00
            comboBoxPC.ClearItems();
            myRSPC2.Clear();
            SQLPC2 = "select PrctClsCode  + '    ' + right(PrctClsDesc, 30) as PC from PracticeClass order by PrctClsCode";
            myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

            if (myRSPC2 == null || myRSPC2.Tables.Count == 0 || myRSPC2.Tables[0].Rows.Count == 0)
                errorList.Add("There are no Practice Classes. Correct and run the tool again");
            else
            {
                foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                    comboBoxPC.Items.Add(dr["PC"].ToString());
                comboBoxPC.SelectedIndex = 0;
            }

            //addresses
            if (clisysnbr != 0)
            {
                loadAddys();
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

            if (myRSPC2 == null || myRSPC2.Tables.Count == 0 || myRSPC2.Tables[0].Rows.Count == 0)
                errorList.Add("There are no valid Timekeepers. Correct and run the tool again");
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
            if (myRSPC2 == null || myRSPC2.Tables.Count == 0 || myRSPC2.Tables[0].Rows.Count == 0)
                errorList.Add("Fee or Exp Schedule Standard in sysparam invalid (CfgTransOpts). Correct and run the tool again");
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

            if (myRSPC2 == null || myRSPC2.Tables.Count == 0 || myRSPC2.Tables[0].Rows.Count == 0)
                errorList.Add("There are no Fee Schedules. Correct and run the tool again");
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

            if (myRSPC2 == null || myRSPC2.Tables.Count == 0 || myRSPC2.Tables[0].Rows.Count == 0)
                errorList.Add("There are no Expense Schedules. Correct and run the tool again");
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

            if (myRSPC2 == null || myRSPC2.Tables.Count == 0 || myRSPC2.Tables[0].Rows.Count == 0)
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

            if (myRSPC2 == null || myRSPC2.Tables.Count == 0 || myRSPC2.Tables[0].Rows.Count == 0)
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

            if (myRSPC2 == null || myRSPC2.Tables.Count == 0 || myRSPC2.Tables[0].Rows.Count == 0)
                errorList.Add("There are no Bill Layouts. Correct and run the tool again");
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
                foreach (string ee in errorList)
                    allErrors = allErrors + ee + "\r\n";
                MessageBox.Show("There were issues loading the Form. See below for details:" + "\r\n" + allErrors, "Form Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                exitToMain = true;
                this.Close();
            }
            else
                //load client values if present
                loadClientInfoForMatter();




            //                dtOpen.Visible = checkBoxSetDate.Checked;
            //NewDR = dtOpen.Value.Date.ToString("MM/dd/yyyy");
            //if (cbOT.SelectedIndex > 0)
            //  OT = this.cbOT.GetItemText(this.cbOT.SelectedItem).Split(' ')[0];
        }

        public bool checkIfLoginInfoIsStillInDB()
        {
            string sql = "select name from defaults where id = 999993 and userid = " + empsysnbr.ToString();
            DataSet myRSPC2 = _jurisUtility.RecordsetFromSQL(sql);

            if (myRSPC2 == null || myRSPC2.Tables.Count == 0 || myRSPC2.Tables[0].Rows.Count == 0) //if no record exists...they shouldnt be usin the tool - catch if they are still in the tool when someone clears the logins
            {
                return false; 
            }
            else 
            {

                return true; //record exists still so they are allowed to keep working
            }


        }

        private void loadAddys()
        {
            if (clisysnbr != 0)
            {
                comboBoxAddyChoose.Enabled = true;
                checkBoxChooseAddy.Enabled = true;
                comboBoxAddyChoose.ClearItems();
                string SQLPC2 = "select BilAdrNickName as PC from BillingAddress where BilAdrCliNbr = " + clisysnbr.ToString() + " order by BilAdrNickName";
                DataSet myRSPC2 = _jurisUtility.RecordsetFromSQL(SQLPC2);

                if (myRSPC2.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow dr in myRSPC2.Tables[0].Rows)
                        comboBoxAddyChoose.Items.Add(dr["PC"].ToString());
                    comboBoxAddyChoose.SelectedIndex = 0;
                    checkBoxChooseAddy.Checked = true;
                }
            }
        }

        private void hideOrShowOriginators(int number)
        {
            if (number > 1)
            {
                comboBoxOT2.Visible = true;
                textBoxOTPct2Opt.Visible = true;
            }
            if (number > 2)
            {
                comboBoxOT3.Visible = true;
                textBoxOTPct3Opt.Visible = true;
            }
            if (number > 3)
            {
                comboBoxOT4.Visible = true;
                textBoxOTPct4Opt.Visible = true;
            }
            if (number > 4)
            {
                comboBoxOT5.Visible = true;
                textBoxOTPct5Opt.Visible = true;
            }

        }

        private void loadInfoFromClientForm()
        {
            checkForTables();
            string sql = "select name, data, entrytype from DefaultSettings where defaultid = 999999 and empsys = " + empsysnbr.ToString();
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
            loadConsolidations();

        }

        private void clearFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pt = this.Location;
            MatterForm cleared = new MatterForm(_jurisUtility, 0, "", 0, pt, empsysnbr);
            cleared.Show();
            this.Close();
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

        private void button1_Click(object sender, EventArgs e)
        {
            buttonCreateClient.Enabled = false;
            if (testClientCode() && testMatterCode())
            {
                createMatter();
                buttonCreateClient.Enabled = true;
            }
            else
                buttonCreateClient.Enabled = true;
        }

        public bool testClientCode()
        {

            if (codeIsNumericClient) // is the sysparam setting a number?
            {
                if (isNumeric(textBoxCode.Text)) // if so, did they enter a number?
                {
                    if (textBoxCode.Text.Length > lengthOfCodeClient) // is it too many characters?
                    {
                        MessageBox.Show("Client Code " + textBoxCode.Text + " is too long. " + "\r\n" + "Your settings only allow for up to " + lengthOfCodeClient.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                    {
                        string code = formatClientCode(textBoxCode.Text);
                        return true;

                    }
                }
                else
                {
                    MessageBox.Show("Client Code " + textBoxCode.Text + " is not numeric. Your settings require a number", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

            }
            else // is it aplha? if so, we only care if its too long
            {
                if (textBoxCode.Text.Length > lengthOfCodeClient)
                {
                    MessageBox.Show("Client Code " + textBoxCode.Text + " is too long. " + "\r\n" + "Your settings only allow for up to " + lengthOfCodeClient.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    string code = formatClientCode(textBoxCode.Text);
                    return true;

                }

            }

        }

        private bool testMatterCode()
        {
            if (codeIsNumericMatter) // is the sysparam setting a number?
            {
                if (isNumeric(textBoxMatterCode.Text)) // if so, did they enter a number?
                {
                    if (textBoxMatterCode.Text.Length > lengthOfCodeMatter) // is it too many characters?
                    {
                        MessageBox.Show("Matter Code " + textBoxMatterCode.Text + " is too long. " + "\r\n" + "Your settings only allow for up to " + lengthOfCodeMatter.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else
                    {
                        string code = formatMatterCode(textBoxMatterCode.Text);
                        return true;

                    }
                }
                else
                {
                    MessageBox.Show("Matter Code " + textBoxMatterCode.Text + " is not numeric. Your settings require a number", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

            }
            else // is it aplha? if so, we only care if its too long
            {
                if (textBoxMatterCode.Text.Length > lengthOfCodeMatter)
                {
                    MessageBox.Show("Matter Code " + textBoxMatterCode.Text + " is too long. " + "\r\n" + "Your settings only allow for up to " + lengthOfCodeMatter.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    string code = formatMatterCode(textBoxMatterCode.Text);
                    return true;

                }

            }

        }



        private void buttonExit_Click(object sender, EventArgs e)
        {
            exitToMain = true;
            this.Close();
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
            {
                checkBoxIncludeExp.Visible = true;
                textBoxFlatRetAmtOpt.Visible = true;
                label34.Visible = true;
            }
            else
            {
                checkBoxIncludeExp.Visible = false;
            }
            if (this.comboBoxBAgree.GetItemText(this.comboBoxBAgree.SelectedItem).Split(' ')[0].Equals("R")) //retainer requires retainer type
            {
                labelRet.Visible = true;
                comboBoxRetainerType.Visible = true;
                textBoxFlatRetAmtOpt.Visible = true;
                label34.Visible = true;
            }
            else
            {
                labelRet.Visible = false;
                comboBoxRetainerType.Visible = false;
            }

            if (!this.comboBoxBAgree.GetItemText(this.comboBoxBAgree.SelectedItem).Split(' ')[0].Equals("R") && !this.comboBoxBAgree.GetItemText(this.comboBoxBAgree.SelectedItem).Split(' ')[0].Equals("F"))
            {
                textBoxFlatRetAmtOpt.Visible = false;
                label34.Visible = false;

            }
        }


        private void comboBoxFeeFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            showHideMonthCycleBoxes();
        }

        private void comboBoxExpFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            showHideMonthCycleBoxes();
        }

        private void showHideMonthCycleBoxes()
        {
            if (this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("C") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("C")) //task billing requires task codes
            {
                labelCycle.Visible = true;
                textBoxCycleOpt.Visible = true;
            }
            else
            {
                labelCycle.Visible = false;
                textBoxCycleOpt.Visible = false;
            }
            if (this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("Q") || this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("S") || this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("A") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("Q") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("S") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("A"))
            {
                label39.Visible = true;
                textBoxMonthOpt.Visible = true;

            }
            else
            {
                label39.Visible = false;
                textBoxMonthOpt.Visible = false;
            }

        }


        private void comboBoxDisc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.comboBoxDisc.GetItemText(this.comboBoxDisc.SelectedItem).Split(' ')[0].Equals("0")) //if discount option selected (not 0)
            {
                labelDPct.Visible = true;
                textBoxDiscPctOpt.Visible = true;
            }
            else
            {
                labelDPct.Visible = false;
                textBoxDiscPctOpt.Visible = false;
            }
        }

        private void comboBoxSurcharge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.comboBoxSurcharge.GetItemText(this.comboBoxSurcharge.SelectedItem).Split(' ')[0].Equals("0")) //if surcharge option selected (not 0)
            {
                labelSPct.Visible = true;
                textBoxSurPctOpt.Visible = true;
            }
            else
            {
                labelSPct.Visible = false;
                textBoxSurPctOpt.Visible = false;
            }
        }

        private bool isNumeric(string value)
        {
            try
            {
                decimal test = Convert.ToDecimal(value);
                return true;
            }
            catch (Exception ) {  return false;  }

        }


        private bool checkFields()
        {
            List<string> incorrectFields = new List<string>();
            if (!isNumeric(textBoxMonthOpt.Text))
            {
                textBoxMonthOpt.Text = "1";
                incorrectFields.Add("Month");
            }
            if (!isNumeric(textBoxCycleOpt.Text))
            {
                textBoxCycleOpt.Text = "1";
                incorrectFields.Add("Cycle");
            }
            if (!isNumeric(textBoxIntDaysOpt.Text))
            {
                textBoxIntDaysOpt.Text = "0";
                incorrectFields.Add("Interest Days");
            }
            if (!isNumeric(textBoxIntPctOpt.Text))
            {
                textBoxIntPctOpt.Text = "0.00";
                incorrectFields.Add("Interest Pct");
            }
            if (!isNumeric(textBoxDiscPctOpt.Text))
            {
                textBoxDiscPctOpt.Text = "0.00";
                incorrectFields.Add("Discount Pct");
            }
            if (!isNumeric(textBoxSurPctOpt.Text))
            {
                textBoxSurPctOpt.Text = "0.00";
                incorrectFields.Add("Surcharge Pct");
            }
            if (!isNumeric(textBoxFlatRetAmtOpt.Text))
            {
                textBoxFlatRetAmtOpt.Text = "100";
                incorrectFields.Add("Flat Fee/Retainer Amt");
            }
            //retry if matter exists


            //ensure no apostrophes or double quotes as they break sql
            foreach (var textbox in this.Controls.OfType<TextBox>())
                textbox.Text = textbox.Text.Replace("'", "").Replace("\"", "").Replace(@"\", " ").Replace("%", "").Replace("[", "").Replace("]", "").Replace("_", " ").Replace("^", "");

            foreach (var textbox in this.Controls.OfType<RichTextBox>())
                textbox.Text = textbox.Text.Replace("'", "").Replace("\"", "").Replace(@"\", " ").Replace("%", "").Replace("[", "").Replace("]", "").Replace("_", " ").Replace("^", "");

            //ensure that box isnt checked if there are no valid addresses selected or loaded
            if (comboBoxAddyChoose.SelectedIndex == -1 || string.IsNullOrEmpty(comboBoxAddyChoose.Text))
                checkBoxChooseAddy.Checked = false;

            if (textBoxCode.Text.Length > lengthOfCodeClient)
            {
                MessageBox.Show("Client Code is longer than allowed. Your settings allow for " + lengthOfCodeClient.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonCreateClient.Enabled = true;
                return false;

            }
            if (codeIsNumericClient && !isNumeric(textBoxCode.Text))
            {
                MessageBox.Show("Client Code is not numeric. Your settings require a numeric code.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonCreateClient.Enabled = true;
                return false;
            }

            if (textBoxMatterCode.Text.Length > lengthOfCodeMatter)
            {
                MessageBox.Show("Matter Code is longer than allowed. Your settings allow for " + lengthOfCodeMatter.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                buttonCreateClient.Enabled = true;
                return false;

            }
            if (codeIsNumericMatter && !isNumeric(textBoxMatterCode.Text))
            {
                MessageBox.Show("Matter Code is not numeric. Your settings require a numeric code.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonCreateClient.Enabled = true;
                return false;
            }
            if (!checkForRequiredUDFs())
            {
                buttonCreateClient.Enabled = true;
                return false;
            }

            clisysnbr = getCliSysNbr();

            if (!checkBoxChooseAddy.Checked)
            {
                string test = "select BilAdrSysNbr from BillingAddress where BilAdrNickName = '" + textBoxBANName.Text + "' and BilAdrCliNbr = " + clisysnbr.ToString();
                DataSet dds1 = _jurisUtility.RecordsetFromSQL(test);
                if (dds1 != null && dds1.Tables.Count > 0 && dds1.Tables[0].Rows.Count != 0)
                {
                    MessageBox.Show("That address nickname is already used for that client" + "\r\n" + "Enter a new and unique nickname", "Constraint Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonCreateClient.Enabled = true;
                    return false;
                }
            }

            if (incorrectFields.Count == 0)
            {
                if (!checkBoxChooseAddy.Checked && (string.IsNullOrEmpty(richTextBoxBAAddy.Text) || string.IsNullOrEmpty(textBoxBANName.Text)))
                {
                    MessageBox.Show("When an existing address is not selected, the Nickname and Address field are required." + "\r\n" + "Please correct this issue and retry", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonCreateClient.Enabled = true;
                    return false;
                }
                foreach (var textbox in this.Controls.OfType<TextBox>())
                {
                    if (string.IsNullOrEmpty(textbox.Text)) //if there is nothing in it, is it required?
                    {
                        if (!textbox.Name.Equals("textBoxBANName"))
                        {
                            if (!textbox.Name.EndsWith("Opt"))
                            {
                                MessageBox.Show("All fields in black text are required. Please correct this issue and retry", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                buttonCreateClient.Enabled = true;
                                return false;
                            }
                        }

                    }

                }
                if ((Convert.ToInt32(textBoxMonthOpt.Text) < 1 || Convert.ToInt32(textBoxMonthOpt.Text) > 12) && (this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("M") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("M") || this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("A") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("A")))
                {
                    MessageBox.Show("When Monthly or Annual is selected, the Month must be 1 through 12.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonCreateClient.Enabled = true;
                    return false;
                }
                if ((Convert.ToInt32(textBoxMonthOpt.Text) < 1 || Convert.ToInt32(textBoxMonthOpt.Text) > 6) && (this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("S") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("S")))
                {
                    MessageBox.Show("When Semi Annual is selected, the Month must be 1 through 6.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonCreateClient.Enabled = true;
                    return false;
                }
                if ((Convert.ToInt32(textBoxMonthOpt.Text) < 1 || Convert.ToInt32(textBoxMonthOpt.Text) > 12) && (this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("Q") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("Q")))
                {
                    MessageBox.Show("When Quarterly is selected, the Month must be 1 through 4.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonCreateClient.Enabled = true;
                    return false;
                }
                if ((Convert.ToInt32(textBoxCycleOpt.Text) < 1 || Convert.ToInt32(textBoxCycleOpt.Text) > 999) && (this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("C") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("C")))
                {
                    MessageBox.Show("When Cycle is selected, the Month must be 1 through 999.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonCreateClient.Enabled = true;
                    return false;
                } //textBoxIntDaysOpt
                  //textBoxCycleOpt.Text
                if (!isInteger(textBoxCycleOpt.Text) || !isInteger(textBoxIntDaysOpt.Text) || !isInteger(textBoxMonthOpt.Text))
                {
                    MessageBox.Show("Cycle, Interest Days and Month must be integers.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxMonthOpt.Text = "1";
                    textBoxCycleOpt.Text = "1";
                    textBoxIntDaysOpt.Text = "0";
                    buttonCreateClient.Enabled = true;
                    return false;
                }
                if (testOrigPct())
                    return true;
                else
                {
                    buttonCreateClient.Enabled = true;
                    return false;
                }

            }

            else
            {
                string items = "";
                foreach (string dd in incorrectFields)
                    items = items + dd + " ";
                MessageBox.Show("All numeric fields must have a number in them." + "\r\n" + "The following fields are invalid and will be reset" + "\r\n" + items + "\r\n" + "Please adjust if needed and continue.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonCreateClient.Enabled = true;

                return false;
            }

        }

        private bool checkForRequiredUDFs()
        {
            string sysparam = " SELECT SpTxtValue, SpName FROM SysParam where spname like 'FldMatterUDF%' and sptxtvalue not like 'M UDF%' ";

            DataSet dds2 = _jurisUtility.RecordsetFromSQL(sysparam);
            if (dds2 != null && dds2.Tables.Count > 0 && dds2.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dds2.Tables[0].Rows) // if any defined UDF fields are 'R', see if they actually added them through UDF button
                {

                    string[] test = dr[0].ToString().Split(',');
                    if (test[3].ToString().Equals("R"))
                    {
                        sysparam = "select * from DefaultSettings where defaultid = 999996 and [name] = '" + test[0].ToString().Replace(" ", "") + "' and empsys = " + empsysnbr.ToString();
                        DataSet dd3 = _jurisUtility.RecordsetFromSQL(sysparam);
                        if (dd3 == null || dd3.Tables.Count == 0 || dd3.Tables[0].Rows.Count == 0)
                        {
                            MessageBox.Show("At least 1 UDF field is required. Please populate the required UDF field(s).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            sysparam = "delete from DefaultSettings where DefaultID = 999996 and empsys = " + empsysnbr.ToString();
                            _jurisUtility.ExecuteNonQuery(0, sysparam);
                            sysparam = "delete from Defaults where ID = 999996 and userid = " + empsysnbr.ToString();
                            _jurisUtility.ExecuteNonQuery(0, sysparam);

                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool isInteger(string test)
        {
            try
            {
                if (test.Contains(".")) //integers dont have decimals
                    return false;
                else
                {
                    int f = Convert.ToInt32(test); // does it even parse?
                    return true;
                }

            }
            catch (Exception e)
            {
                return false;
            }

        }


        private bool checkForDupeOriginators()
        {
            List<ComboBox> cboxList = new List<ComboBox>();
            foreach (var cbox in this.Controls.OfType<ComboBox>())
            {//check to see if they have a percentage...if they dont, we dont care fi they dupe as they arent used
                if ((cbox.Name.Equals("comboBoxOT1") && Convert.ToDecimal(textBoxOTPct1Opt.Text) != 0) || (cbox.Name.Equals("comboBoxOT2") && Convert.ToDecimal(textBoxOTPct2Opt.Text) != 0) ||
                    (cbox.Name.Equals("comboBoxOT3") && Convert.ToDecimal(textBoxOTPct3Opt.Text) != 0) || (cbox.Name.Equals("comboBoxOT4") && Convert.ToDecimal(textBoxOTPct4Opt.Text) != 0) ||
                    (cbox.Name.Equals("comboBoxOT5") && Convert.ToDecimal(textBoxOTPct5Opt.Text) != 0))
                    cboxList.Add(cbox);
            }

            
        int vv = cboxList.Select(box => box.SelectedIndex).Distinct().Count(); //how many unique empids do we have?

            if (cboxList.Count == vv)
                return true;
            else
            {
                MessageBox.Show("There are duplicate Originators. Either adjust the percentages or Originators." + "\r\n" + "Originators with 0 percent are ignored and not taken into consideration", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void createMatter()
        {
            if (!doesMatterExist())
            {
                if (checkFields())
                {



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


                    string resp = "Empty";
                    if (checkBoxRT.Checked)
                        resp = " (select empsysnbr from employee where empid = '" + this.comboBoxRT.GetItemText(this.comboBoxRT.SelectedItem).Split(' ')[0] + "')";

                    string retType = ((bool?)comboBoxRetainerType.Visible) == true ? this.comboBoxRetainerType.GetItemText(this.comboBoxRetainerType.SelectedItem).Split(' ')[0] : string.Empty;

                    int billto = 0;
                    if (checkBoxConsolidation.Checked)
                    {
                        var len = this.comboBoxConsolidation.GetItemText(this.comboBoxConsolidation.SelectedItem).Split(' ').Length - 1;
                        billto = Convert.ToInt32(this.comboBoxConsolidation.GetItemText(this.comboBoxConsolidation.SelectedItem).Split(' ')[len]);

                    }
                    else
                        billto = createAddy();

                    if (billto != 0)
                    {
                        string formattedMatCode = formatMatterCode(textBoxMatterCode.Text);
                        string desc = richTextBoxDescOpt.Text.Replace("\r", "|").Replace("\n", "|");
                        desc = desc.Replace("||", "|");
                        desc = desc.Replace("'", "").Replace("\"", "").Replace(@"\", " ").Replace("%", "").Replace("[", "").Replace("]", "").Replace("_", " ").Replace("^", "");
                        string rem = richTextBoxRemarksOpt.Text.Replace("\r", "|").Replace("\n", "|");
                        rem = rem.Replace("||", "|");
                        rem = rem.Replace("'", "").Replace("\"", "").Replace(@"\", " ").Replace("%", "").Replace("[", "").Replace("]", "").Replace("_", " ").Replace("^", "");

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
                           + "     values( case when (select max(matsysnbr) from matter) is null then 1 else ((select max(matsysnbr) from matter) + 1) end, " + clisysnbr + ", " + billto.ToString() + ",  "
                           + "       '" + formattedMatCode + "', '" + textBoxNName.Text.Trim() + "', '" + textBoxRName.Text.Trim() + "', replace('" + desc + "', '|', char(13) + char(10)), " +
                           " replace('" + rem + "', '|', char(13) + char(10)),'" + textBoxPhoneOpt.Text.Trim() + "', '" + textBoxFaxOpt.Text.Trim() + "', '" + textBoxContactOpt.Text.Trim() + "', '" + dateTimePickerOpened.Value.ToString("MM/dd/yyyy") + "','O' ,'0', "
                         + " '01/01/1900','" + this.comboBoxOffice.GetItemText(this.comboBoxOffice.SelectedItem).Split(' ')[0] + "','" + this.comboBoxPC.GetItemText(this.comboBoxPC.SelectedItem).Split(' ')[0] + "','" + this.comboBoxFeeSched.GetItemText(this.comboBoxFeeSched.SelectedItem).Split(' ')[0] + "'," + txref + ",'" + this.comboBoxExpSched.GetItemText(this.comboBoxExpSched.SelectedItem).Split(' ')[0] + "'," + exref + ",0, "
                          + "'" + this.comboBoxBAgree.GetItemText(this.comboBoxBAgree.SelectedItem).Split(' ')[0] + "','" + inclExp + "','" + retType + "', " + textBoxFlatRetAmtOpt.Text + ", '" + this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0] + "', '" + this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0] + "' ," + textBoxMonthOpt.Text + "," + textBoxCycleOpt.Text + ", "
                         + " 0.00,0.00," + textBoxIntPctOpt.Text + "," + textBoxIntDaysOpt.Text + "," + this.comboBoxDisc.GetItemText(this.comboBoxDisc.SelectedItem).Split(' ')[0] + "," + textBoxDiscPctOpt.Text + ", " + this.comboBoxSurcharge.GetItemText(this.comboBoxSurcharge.SelectedItem).Split(' ')[0] + ", " + textBoxSurPctOpt.Text + ", 0, 0.00,"
                          + "0.00," + budg + ",0, 'N','" + reqTask + "','" + reqAct + "','N','" + tax1 + "','" + tax2 + "','" + tax3 + "',"

                        + " '01/01/1900','01/01/1900','01/01/1900','01/01/1900','01/01/1900',0.00,0.00,0.00,0.00,0.00,0,0,0,"
                         + " '','','','','','','', '','','','','','','','','', '', '', '', '', 0, 0, '')";


                        isError = _jurisUtility.ExecuteNonQuery(0, sql);
                        if (!isError) //error adding matter
                        {
                            sql = "update BillingAddress_Log set jurisuser = " + empsysnbr.ToString() + " where jurisuser is null and convert(varchar,DateTimeStamp, 101) = convert(varchar,getdate(), 101)";
                            _jurisUtility.ExecuteNonQuery(0, sql);

                            sql = "update Billto_Log set jurisuser = " + empsysnbr.ToString() + " where jurisuser is null and convert(varchar,DateTimeStamp, 101) = convert(varchar,getdate(), 101)";
                            _jurisUtility.ExecuteNonQuery(0, sql);

                            sql = "update BillCopy_Log set jurisuser = " + empsysnbr.ToString() + " where jurisuser is null and convert(varchar,DateTimeStamp, 101) = convert(varchar,getdate(), 101)";
                            _jurisUtility.ExecuteNonQuery(0, sql);

                            matsysnbr = getMatSysNbr();
                            if (!resp.Equals("Empty"))
                                isError = addRespToTable(resp);
                            if (!isError) //error adding resp atty
                            {
                                sql = "update MatterResponsibleTimekeeper_Log set jurisuser = " + empsysnbr.ToString() + " where jurisuser is null and convert(varchar,DateTimeStamp, 101) = convert(varchar,getdate(), 101)";
                                _jurisUtility.ExecuteNonQuery(0, sql);
                                isError = addOrig();
                                if (!isError)//error adding originators
                                {
                                    sql = "update MatOrigAtty_Log set jurisuser = " + empsysnbr.ToString() + " where jurisuser is null and convert(varchar,DateTimeStamp, 101) = convert(varchar,getdate(), 101)";
                                    _jurisUtility.ExecuteNonQuery(0, sql);
                                    isError = loadMatterBillFields();
                                    if (!isError)
                                    {
                                        isError = loadMatterUDFs();
                                        if (!isError)
                                        {
                                            //handles making of matter and editing it for billing fields/udfs
                                            sql = "update matter_log set jurisuser = " + empsysnbr.ToString() + " where matsysnbr = " + matsysnbr.ToString();
                                            _jurisUtility.ExecuteNonQuery(0, sql);

                                            sql = "update matter_log set [Application] = 'CMI Tool' where matsysnbr = " + matsysnbr.ToString();
                                            _jurisUtility.ExecuteNonQuery(0, sql);

                                            sql = "update sysparam set spnbrvalue = " + matsysnbr.ToString() + " where spname = 'LastSysNbrMatter'";
                                            _jurisUtility.ExecuteNonQuery(0, sql);

                                            sql = "update sysparam set spnbrvalue = (select max(billtosysnbr) from billto) where spname = 'LastSysNbrBillTo'";
                                            _jurisUtility.ExecuteNonQuery(0, sql);

                                            sql = "update sysparam set spnbrvalue = (select max(biladrsysnbr) from billingaddress) where spname = 'LastSysNbrBillAddress'";
                                            _jurisUtility.ExecuteNonQuery(0, sql);

                                            //if they added a notecard
                                            if (addNoteCard)
                                            {
                                                sql = "insert into [matterNote] ([MNMatter] ,[mNNoteIndex],[mNObject],[mNNoteText],[mNNoteObject]) values(" + matsysnbr.ToString() + ", replace('" + noteName + "', '|', char(13) + char(10)), '', replace('" + noteText + "', '|', char(13) + char(10)), null)";
                                                _jurisUtility.ExecuteNonQuery(0, sql);

                                                sql = "update MatterNote_Log set jurisuser = " + empsysnbr.ToString() + ", [Application] = 'CMI Tool' where MNMatter = " + matsysnbr.ToString();
                                                _jurisUtility.ExecuteNonQuery(0, sql);
                                            }

                                            DialogResult fc = MessageBox.Show("Matter " + textBoxCode.Text + "/" + textBoxMatterCode.Text + " was added successfully." + "\r\n" + "Would you like to add another Matter to this Client?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                            if (fc == DialogResult.Yes)
                                            {
                                                pt = this.Location;
                                                sql = "delete from DefaultSettings where defaultid = 999997 and empsys = " + empsysnbr.ToString(); //stored BF info
                                                _jurisUtility.ExecuteNonQuery(0, sql);
                                                sql = "delete from Defaults where id = 999997  and userid = " + empsysnbr.ToString();
                                                _jurisUtility.ExecuteNonQuery(0, sql);
                                                sql = "delete from DefaultSettings where defaultid = 999996 and empsys = " + empsysnbr.ToString(); //stored UDF info
                                                _jurisUtility.ExecuteNonQuery(0, sql);
                                                sql = "delete from Defaults where id = 999996  and userid = " + empsysnbr.ToString();
                                                _jurisUtility.ExecuteNonQuery(0, sql);
                                                MatterForm cleared = new MatterForm(_jurisUtility, clisysnbr, textBoxCode.Text, addySysNbr, pt, empsysnbr);
                                                cleared.Show();
                                                //move data over
                                                this.Close();

                                            }
                                            else
                                            {
                                                sql = "delete from DefaultSettings where defaultid = 999997 and empsys = " + empsysnbr.ToString(); //stored BF info
                                                _jurisUtility.ExecuteNonQuery(0, sql);
                                                sql = "delete from Defaults where id = 999997  and userid = " + empsysnbr.ToString();
                                                _jurisUtility.ExecuteNonQuery(0, sql);
                                                sql = "delete from DefaultSettings where defaultid = 999996 and empsys = " + empsysnbr.ToString(); //stored UDF info
                                                _jurisUtility.ExecuteNonQuery(0, sql);
                                                sql = "delete from Defaults where id = 999996  and userid = " + empsysnbr.ToString();
                                                _jurisUtility.ExecuteNonQuery(0, sql);
                                                exitToMain = true;
                                                this.Close();
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("There was an issue adding the UDF Fields." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            isError = false;
                                            undoOrig();
                                            undoResp();
                                            undoMatter();
                                            undoBillCopy(billto);
                                            undoBillTo(billto);

                                            if (removeAddy)
                                            {
                                                undoAddy(addySysNbr);
                                                addySysNbr = 0;
                                                removeAddy = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("There was an issue adding the Billing Fields." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        isError = false;
                                        undoOrig();
                                        undoResp();
                                        undoMatter();
                                        undoBillCopy(billto);
                                        undoBillTo(billto);

                                        if (removeAddy)
                                        {
                                            undoAddy(addySysNbr);
                                            addySysNbr = 0;
                                            removeAddy = false;
                                        }
                                    }
                                }
                                else //error adding rig attys
                                {
                                    MessageBox.Show("There was an issue adding the Originating Attys." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    isError = false;
                                    undoResp();
                                    undoMatter();
                                    undoBillCopy(billto);
                                    undoBillTo(billto);


                                    if (removeAddy)
                                    {
                                        undoAddy(addySysNbr);
                                        addySysNbr = 0;
                                        removeAddy = false;
                                    }
                                }
                            }
                            else //error adding resp attys
                            {
                                MessageBox.Show("There was an issue adding the Responsible Attys." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                isError = false;
                                undoBillCopy(billto);
                                undoMatter();
                                undoBillTo(billto);

                                if (removeAddy)
                                {
                                    undoAddy(addySysNbr);
                                    addySysNbr = 0;
                                    removeAddy = false;
                                }
                            }
                        }
                        else //error adding the matter
                        {
                            MessageBox.Show("There was an issue adding the matter." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            isError = false;
                            undoBillCopy(billto);
                            undoBillTo(billto);

                            if (removeAddy)
                            {
                                undoAddy(addySysNbr);
                                addySysNbr = 0;
                                removeAddy = false;
                            }
                        }


                    }
                }
            }
            else
                MessageBox.Show("That matter code already exists for this client. " + "\r\n" + " Please update the matter ode so it is unique", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool addRespToTable(string empsys)
        {
            try
            {
                string sql = "";

                sql = "insert into MatterResponsibleTimekeeper (MRTMatterID, MRTEmployeeID, MRTPercent) values ( " +
                        matsysnbr.ToString() + ", " + empsys + ", 100.0000 )";
                return _jurisUtility.ExecuteNonQuery(0, sql);
            }
            catch (Exception cc)
            {
                return true;
            }

        }

        private void undoBillCopy(int billto)
        {
            try
            {
                string sql = "delete from BillCopy where BilCpyBillTo = " + billto.ToString() + " and  BilCpyBilAdr = " + addySysNbr.ToString();
                _jurisUtility.ExecuteNonQuery(0, sql);
                isError = false;

            }
            catch (Exception)   { }


        }

        private void undoMatter()
        {
            try
            {
                string sql = "delete from matter where matsysnbr = " + matsysnbr.ToString();
                _jurisUtility.ExecuteNonQuery(0, sql);
                isError = false;

            }
            catch (Exception ex)   { MessageBox.Show(ex.Message); }
        }

        private void undoResp()
        {
            try
            {
                string sql = "delete from MatterResponsibleTimekeeper where MRTMatterID = " + matsysnbr.ToString() ;
                _jurisUtility.ExecuteNonQuery(0, sql);
                isError = false;

            }
            catch (Exception)   { }
        }

        private void undoOrig()
        {
            try
            {
                string sql = "delete from MatOrigAtty where MOrigMat = " + matsysnbr.ToString();
                _jurisUtility.ExecuteNonQuery(0, sql);
                isError = false;

            }
            catch (Exception)  { }
        }

        private int createAddy() // returns billto which is required to add the matter
        {
            //get clisysnbr if we dont have it yet (clicked on matter only)


            try
            {
                if (clisysnbr == 0)
                {
                    MessageBox.Show("Client " + textBoxCode.Text + " does not exist. Enter a valid client code." + "\r\n" + "It must match exactly as it appears in Juris including leading zeroes", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
                else
                {
                    //see if matter number exists
                    int matsys = 0;
                    string code = formatMatterCode(textBoxMatterCode.Text);
                    string sql = "select matsysnbr from matter where matclinbr = " + clisysnbr.ToString() + " and matcode = '" + code + "'";
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


                        if (checkBoxChooseAddy.Checked) //if checked we only add billto and billcopy
                        {
                            removeAddy = false;
                            if (addySysNbr == 0)
                                addySysNbr = getAddyID();
                            if (addySysNbr != 0)
                            {
                                string resp = "0";
                                if (checkBoxRT.Checked)
                                    resp = " (select empsysnbr from employee where empid = '" + this.comboBoxRT.GetItemText(this.comboBoxRT.SelectedItem).Split(' ')[0] + "')";

                                //create billto
                                sql = "Insert into BillTo (BillToSysNbr,BillToCliNbr,BillToUsageFlg,BillToNickName,BillToBillingAtty,BillToBillFormat,BillToEditFormat,BillToRespAtty) " +
                                    "values (case when(select max(billtosysnbr) from billto) is null then 1 else ((select max(billtosysnbr) from billto) +1) end, " + clisysnbr.ToString() +
                                    ",  'M', '" + textBoxMatterCode.Text + "', (select empsysnbr from employee where empid = '" + this.comboBoxBT.GetItemText(this.comboBoxBT.SelectedItem).Split(' ')[0] + "'), " +
                                    " '" + this.comboBoxBillLayout.GetItemText(this.comboBoxBillLayout.SelectedItem).Split(' ')[0] + "', '" + this.comboBoxPreBillLayout.GetItemText(this.comboBoxPreBillLayout.SelectedItem).Split(' ')[0] + "', " + resp + ")";

                                isError = _jurisUtility.ExecuteNonQuery(0, sql); //did we enconter an error creating billto?
                                if (!isError)
                                {
                                    sql = "select max(billtosysnbr) from billto where BillToCliNbr = " + clisysnbr.ToString();
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



                                    //billcopy
                                    sql = "Insert into BillCopy(BilCpyBillTo,BilCpyBilAdr,BilCpyComment,BilCpyNbrOfCopies,BilCpyPrintFormat,BilCpyEmailFormat,BilCpyExportFormat,BilCpyARFormat) "
                                    + " values ( " + billto.ToString() + ", " + addySysNbr.ToString() + " ,'',1,1,0,0,0 )";

                                    isError = _jurisUtility.ExecuteNonQuery(0, sql);
                                    if (!isError)
                                    { return billto; }
                                    else
                                    {
                                        MessageBox.Show("There was an issue adding Billing Reference (billcopy-Existing)." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        isError = false;
                                        undoBillTo(billto);
                                        return 0;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("There was an issue adding Billing Reference (billto-Existing)." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    isError = false;
                                    return 0;
                                }
                            }
                            else //they picked from the list but we didnt find it...?
                            {
                                MessageBox.Show("We could not find that address. Please try another.", "LookUp Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return 0;
                            }


                        }
                        else //we add address as well
                        {
                            removeAddy = true;
                            string addy = richTextBoxBAAddy.Text.Replace("\r", "|").Replace("\n", "|");
                            addy = addy.Replace("||", "|");
                            addy = addy.Replace("'", "").Replace("\"", "").Replace(@"\", " ").Replace("%", "").Replace("[", "").Replace("]", "").Replace("_", " ").Replace("^", "");

                            sql = "Insert into BillingAddress(BilAdrSysNbr, BilAdrCliNbr, BilAdrUsageFlg, BilAdrNickName, BilAdrPhone, " +
                                " BilAdrFax, BilAdrContact, BilAdrName, BilAdrAddress, BilAdrCity, BilAdrState, BilAdrZip, BilAdrCountry, BilAdrType, BilAdrEmail) " +
                                " values (case when(select max(biladrsysnbr) from billingaddress) is null then 1 else ((select max(biladrsysnbr) from billingaddress) +1) end, " + clisysnbr + ", " +
                                " 'M', '" + textBoxBANName.Text + "', '" + textBoxBAPhoneOpt.Text + "', "
                                 + "  '" + textBoxBAFaxOpt.Text + "', '" + textBoxBAContactOpt.Text + "', " +
                                " '" + textBoxBANameOpt.Text + "', " +
                                "replace('" + addy + "', '|', char(13) + char(10)), "
                                + " '" + textBoxBACityOpt.Text + "', '" + textBoxBAStateOpt.Text + "', '" + textBoxBAZipOpt.Text + "','" + textBoxBACountryOpt.Text + "', 0, '" + textBoxBAEmailOpt.Text + "')";

                            isError = _jurisUtility.ExecuteNonQuery(0, sql);


                            if (!isError)
                            {
                                sql = "select max(biladrsysnbr) from billingaddress where BilAdrCliNbr = " + clisysnbr.ToString();
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

                                string resp = "0";
                                if (checkBoxRT.Checked)
                                    resp = " (select empsysnbr from employee where empid = '" + this.comboBoxRT.GetItemText(this.comboBoxRT.SelectedItem).Split(' ')[0] + "')";

                                //create billto
                                sql = "Insert into BillTo (BillToSysNbr,BillToCliNbr,BillToUsageFlg,BillToNickName,BillToBillingAtty,BillToBillFormat,BillToEditFormat,BillToRespAtty) " +
                                    "values (case when(select max(billtosysnbr) from billto) is null then 1 else ((select max(billtosysnbr) from billto) +1) end, " + clisysnbr.ToString() +
                                    ",  'M', '" + textBoxBANName.Text + "', (select empsysnbr from employee where empid = '" + this.comboBoxBT.GetItemText(this.comboBoxBT.SelectedItem).Split(' ')[0] + "'), " +
                                    " '" + this.comboBoxBillLayout.GetItemText(this.comboBoxBillLayout.SelectedItem).Split(' ')[0] + "', '" + this.comboBoxPreBillLayout.GetItemText(this.comboBoxPreBillLayout.SelectedItem).Split(' ')[0] + "', " + resp + ")";

                                isError = _jurisUtility.ExecuteNonQuery(0, sql); //did we enconter an error creating billto?
                                if (!isError)
                                {
                                    sql = "select max(billtosysnbr) from billto where BillToCliNbr = " + clisysnbr.ToString();
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



                                    //billcopy
                                    sql = "Insert into BillCopy(BilCpyBillTo,BilCpyBilAdr,BilCpyComment,BilCpyNbrOfCopies,BilCpyPrintFormat,BilCpyEmailFormat,BilCpyExportFormat,BilCpyARFormat) "
                                    + " values ( " + billto.ToString() + ", " + addySysNbr.ToString() + " ,'" + textBoxMatterCode.Text + "',1,1,0,0,0 )";

                                    isError = _jurisUtility.ExecuteNonQuery(0, sql);
                                    if (!isError)
                                    {
                                        addySysNbr = addyid;
                                        return billto;
                                    }
                                    else
                                    {
                                        MessageBox.Show("There was an issue adding Billing Reference (billcopy)." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        isError = false;
                                        undoBillTo(billto);
                                        undoAddy(addyid);
                                        return 0;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("There was an issue adding Billing Reference (billto)." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    isError = false;
                                    undoAddy(addyid);
                                    return 0;
                                }
                            }
                            else
                            {
                                MessageBox.Show("There was an issue adding the Address." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                isError = false;
                                return 0;
                            }
                        }

                    }
                }
            }
            catch (Exception vv)
            { 
                isError =  false;
                return 0;
            }

        }

        private void undoBillTo(int billto)
        {
            try
            {
                string sql = "delete from billto where billtosysnbr = " + billto.ToString();
                _jurisUtility.ExecuteNonQuery(0, sql);
                isError = false;

            }
            catch (Exception)
            { }

        }


        private void undoAddy(int addyid)
        {
            try
            {
                string sql = "delete from BillingAddress where BilAdrSysNbr = " + addyid.ToString();
                _jurisUtility.ExecuteNonQuery(0, sql);
                isError = false;

            }
            catch (Exception)
            { }

        }

        private bool addOrig()
        {
            try
            {
                string sql = "";
                if (matsysnbr == 0)
                    getMatSysNbr();
                if (!textBoxOTPct1Opt.Text.Equals("0"))
                {
                    sql = "insert into MatOrigAtty (MOrigMat, MOrigAtty, MOrigPcnt) values (" + matsysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT1.GetItemText(this.comboBoxOT1.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct1Opt.Text + " as decimal(7,4)))";
                    if (_jurisUtility.ExecuteNonQuery(0, sql))
                        return true;
                }
                if (!textBoxOTPct2Opt.Text.Equals("0"))
                {
                    sql = "insert into MatOrigAtty (MOrigMat, MOrigAtty, MOrigPcnt) values (" + matsysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT2.GetItemText(this.comboBoxOT2.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct2Opt.Text + " as decimal(7,4)))";
                    if (_jurisUtility.ExecuteNonQuery(0, sql))
                        return true;
                }
                if (!textBoxOTPct3Opt.Text.Equals("0"))
                {
                    sql = "insert into MatOrigAtty (MOrigMat, MOrigAtty, MOrigPcnt) values (" + matsysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT3.GetItemText(this.comboBoxOT3.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct3Opt.Text + " as decimal(7,4)))";
                    if (_jurisUtility.ExecuteNonQuery(0, sql))
                        return true;
                }
                if (!textBoxOTPct4Opt.Text.Equals("0"))
                {
                    sql = "insert into MatOrigAtty (MOrigMat, MOrigAtty, MOrigPcnt) values (" + matsysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT4.GetItemText(this.comboBoxOT4.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct4Opt.Text + " as decimal(7,4)))";
                    if (_jurisUtility.ExecuteNonQuery(0, sql))
                        return true;
                }
                if (!textBoxOTPct5Opt.Text.Equals("0"))
                {
                    sql = "insert into MatOrigAtty (MOrigMat, MOrigAtty, MOrigPcnt) values (" + matsysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT5.GetItemText(this.comboBoxOT5.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct5Opt.Text + " as decimal(7,4)))";
                    if (_jurisUtility.ExecuteNonQuery(0, sql))
                        return true;
                }

                return false;
            }
            catch (Exception b)
            {
                return true;
            }
        }

        private bool testOrigPct()
        {

            if (isNumeric(textBoxOTPct1Opt.Text) && isNumeric(textBoxOTPct2Opt.Text) && isNumeric(textBoxOTPct3Opt.Text) && isNumeric(textBoxOTPct4Opt.Text) && isNumeric(textBoxOTPct5Opt.Text) && (Convert.ToDecimal(textBoxOTPct1Opt.Text) + Convert.ToDecimal(textBoxOTPct2Opt.Text) + Convert.ToDecimal(textBoxOTPct3Opt.Text) + Convert.ToDecimal(textBoxOTPct4Opt.Text) + Convert.ToDecimal(textBoxOTPct5Opt.Text) == 100))
            {
                if (checkForDupeOriginators())
                    return true;
                else
                    return false;
            }
            else
            {
                MessageBox.Show("All 5 percentages for Originators must be numeric and add to 100." + "\r\n" + "Resetting percentages to default. Please adjust if needed.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxOTPct1Opt.Text = "100";
                textBoxOTPct2Opt.Text = "0";
                textBoxOTPct3Opt.Text = "0";
                textBoxOTPct4Opt.Text = "0";
                textBoxOTPct5Opt.Text = "0";
                return false;
            }

        }


        private void ExitDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exitToMain = true;
            this.Close();
        }

        private int getAddyID()
        {
            int addyid = 0;
            string test = "select BilAdrSysNbr from BillingAddress where BilAdrNickName = '" + this.comboBoxAddyChoose.GetItemText(this.comboBoxAddyChoose.SelectedItem) + "' and BilAdrCliNbr = " + clisysnbr.ToString();
            DataSet dds1 = _jurisUtility.RecordsetFromSQL(test);
            if (dds1 != null && dds1.Tables.Count > 0)
            {
                foreach (DataRow dr in dds1.Tables[0].Rows)
                {
                    addyid = Convert.ToInt32(dr[0].ToString());
                    return addyid;
                }
            }
            return addyid;
        }


        private void textBoxCode_Leave(object sender, EventArgs e)
        {
            textBoxNName.Text = "";
            textBoxRName.Text = "";
            if (!string.IsNullOrEmpty(textBoxCode.Text.Trim()))
            {
                if (textBoxCode.Text.Length > lengthOfCodeClient)
                    MessageBox.Show("Client Code is longer than allowed. Your settings allow for " + lengthOfCodeClient.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    if (codeIsNumericClient && !isNumeric(textBoxCode.Text))
                        MessageBox.Show("Client Code is not numeric. Your settings require a numeric code.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        verifyAndLoadClient();
                    }
                }
            }

        }

        private void verifyAndLoadClient()
        {
            if (!string.IsNullOrEmpty(textBoxCode.Text))
            {
                clisysnbr = getCliSysNbr();
                if (clisysnbr == 0)
                {
                    MessageBox.Show("That client does not exist. Re-enter a client that exists." + "\r\n" + "The code must match exactly as it appears in Juris including leading zeros", "Client Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    clisysnbr = 0;
                }
                else
                {
                    loadClientInfoForMatter();
                    getNextMatterNumber();
                    loadAddys();
                    loadConsolidations();
                }
            }
            else if (clisysnbr != 0)
            {
                getNextMatterNumber();
                loadAddys();
                loadConsolidations();
            }

        }


        private void loadConsolidations()
        {
            comboBoxConsolidation.ClearItems();
            if (clisysnbr != 0)
            {
                string sql = "select BillToNickName + '                                                                      ' + cast(BillToSysNbr as varchar(15)) as id from billto where BillToCliNbr = " + clisysnbr.ToString() + " and BillToUsageFlg = 'C'";
                DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                if (dds != null && dds.Tables.Count > 0 && dds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                        comboBoxConsolidation.Items.Add(dr["id"].ToString());

                    checkBoxConsolidation.Enabled = true;
                    comboBoxConsolidation.Enabled = true;
                    comboBoxConsolidation.SelectedIndex = 0;
                    
                }
                else
                {
                    comboBoxConsolidation.Enabled = false;
                    checkBoxConsolidation.Enabled = false;
                }
            }
            else
            {
                comboBoxConsolidation.Enabled = false;
                checkBoxConsolidation.Enabled = false;
            }

        }

        private int getCliSysNbr()
        {
            clisysnbr = 0;
            string code = formatClientCode(textBoxCode.Text);
            if (codeIsNumericClient)
                textBoxCode.Text = code.Substring(code.Length - lengthOfCodeClient, lengthOfCodeClient);
            string sql = "select clisysnbr from client where clicode = '" + code + "'";
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            if (dds != null && dds.Tables.Count > 0)
            {
                foreach (DataRow dr in dds.Tables[0].Rows)
                {
                    clisysnbr = Convert.ToInt32(dr[0].ToString());
                }
            }
            return clisysnbr;
        }

        private int getMatSysNbr()
        {

            matsysnbr = 0;
            if (clisysnbr == 0)
                clisysnbr = getCliSysNbr();
            string code = formatMatterCode(textBoxMatterCode.Text);
            if (codeIsNumericMatter)
                textBoxMatterCode.Text = code.Substring(code.Length - lengthOfCodeMatter, lengthOfCodeMatter);
            string sql = "select matsysnbr from matter where matclinbr = " + clisysnbr.ToString() + " and matcode = '" + code + "'";
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            if (dds != null && dds.Tables.Count > 0)
            {
                foreach (DataRow dr in dds.Tables[0].Rows)
                {
                    matsysnbr = Convert.ToInt32(dr[0].ToString());
                    break;
                }

            }
            return matsysnbr;
        }

        private bool doesMatterExist()
        {
            int ms = 0;
            if (clisysnbr == 0)
                clisysnbr = getCliSysNbr();
            string code = formatMatterCode(textBoxMatterCode.Text);
            if (codeIsNumericMatter)
                textBoxMatterCode.Text = code.Substring(code.Length - lengthOfCodeMatter, lengthOfCodeMatter);
            string sql = "select matsysnbr from matter where matclinbr = " + clisysnbr.ToString() + " and matcode = '" + code + "'";
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            if (dds != null && dds.Tables.Count > 0)
            {
                foreach (DataRow dr in dds.Tables[0].Rows)
                {
                    ms = Convert.ToInt32(dr[0].ToString());
                    break;
                }

            }
            if (ms == 0)
                return false;
            else
                return true;
        }

        private void getDefaultsForClientMatter()
        {
            //matter
            string sysparam = "  select SpTxtValue from sysparam where SpName = 'FldMatter'";
            DataSet dds2 = _jurisUtility.RecordsetFromSQL(sysparam);
            string cell = "";
            if (dds2 != null && dds2.Tables.Count > 0)
            {
                foreach (DataRow dr in dds2.Tables[0].Rows)
                {
                    cell = dr[0].ToString();
                }

            }
            string[] test = cell.Split(',');
            lengthOfCodeMatter = Convert.ToInt32(test[2]);

            if (test[1].Equals("C"))
                codeIsNumericMatter = false;
            else
                codeIsNumericMatter = true;

            //client
            sysparam = "  select SpTxtValue from sysparam where SpName = 'FldClient'";
            dds2.Clear();
            dds2 = _jurisUtility.RecordsetFromSQL(sysparam);
            if (dds2 != null && dds2.Tables.Count > 0)
            {
                foreach (DataRow dr in dds2.Tables[0].Rows)
                {
                    cell = dr[0].ToString();
                }

            }
            string[] test1 = cell.Split(',');
            lengthOfCodeClient = Convert.ToInt32(test1[2]);

            if (test1[1].Equals("C"))
                codeIsNumericClient = false;
            else
                codeIsNumericClient = true;


        }

        private string formatClientCode(string code)
        {
            string formattedCode = "";
            if (codeIsNumericClient)
            {
                formattedCode = "000000000000" + code;
                formattedCode = formattedCode.Substring(formattedCode.Length - 12, 12);
            }
            else
                formattedCode = code;
            return formattedCode;



        }

        private string formatMatterCode(string code)
        {
            string formattedCode = "";
            if (codeIsNumericMatter)
            {
                formattedCode = "000000000000" + code;
                formattedCode = formattedCode.Substring(formattedCode.Length - 12, 12);
            }
            else
                formattedCode = code;
            return formattedCode;

        }

        private void getNextMatterNumber()
        {
            if (clisysnbr != 0)
            {
                string sql = "SELECT distinct top 1 number FROM master..spt_values " +
                            "WHERE number BETWEEN 1 and (SELECT max(cast(matcode as int)) + 1 FROM matter where matclinbr = " + clisysnbr.ToString() + ") " +
                            "AND number NOT IN (SELECT cast(matcode as int) FROM matter where matclinbr = " + clisysnbr.ToString() + ") order by number";
                DataSet dds1 = _jurisUtility.RecordsetFromSQL(sql);
                string nextcode = "";
                if (dds1 != null && dds1.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds1.Tables[0].Rows)
                    {
                        if (codeIsNumericMatter || isNumeric(dr[0].ToString()))
                        {
                            nextcode = "000000000000" + dr[0].ToString().ToString();
                            nextcode = nextcode.Substring(nextcode.Length - lengthOfCodeMatter, lengthOfCodeMatter);
                            textBoxMatterCode.Text = nextcode;
                        }
                    }
                }


            }

        }


        private void comboBoxAddyChoose_SelectedIndexChanged(object sender, EventArgs e)
        {
            //fill in the addy when they choose an address
            addySysNbr = getAddyID();
        }


        private void textBoxNName_Leave(object sender, EventArgs e)
        {
            textBoxRName.Text = textBoxNName.Text;
        }

        private void loadClientInfoForMatter()
        {
            //get client info to load into form
            //add resp and orig
            string sql = "SELECT   CliNickName ,CliReportingName ,CliPhoneNbr  ,CliFaxNbr ,CliContactName  ,OfcOfficeCode + '    ' + right(OfcDesc, 30)  , " +
                " empid + '    ' + empname ,PrctClsCode  + '    ' + right(PrctClsDesc, 30)  ,CliFeeSch " +
                " ,case when CliTaskCodeXref is null then 'null' else CliTaskCodeXref end as CliTaskCodeXref ,CliExpSch  ,case when CliExpCodeXref is null then 'null' else CliExpCodeXref end as CliExpCodeXref "
                + ",CliBillFormat  ,CliBillAgreeCode ,CliFlatFeeIncExp  ,CliRetainerType ,CliExpFreqCode  ,CliFeeFreqCode  ,CliBillMonth ,CliBillCycle ,CliInterestPcnt " +
                " ,CliInterestDays ,CliDiscountOption ,CliDiscountPcnt ,CliSurchargeOption ,CliSurchargePcnt ,CliTax1Exempt ,CliTax2Exempt ,CliTax3Exempt ,CliBudgetOption ,CliReqTaskCdOnTime ,CliReqActyCdOnTime ," +
                " CliEditFormat FROM Client inner join officecode on OfcOfficeCode = CliOfficeCode inner join PracticeClass on PrctClsCode = CliPracticeClass " +
                " inner join employee on empsysnbr = CliBillingAtty where clisysnbr = " + clisysnbr.ToString();
            DataSet client = _jurisUtility.RecordsetFromSQL(sql);
            if (client != null && client.Tables.Count > 0)
            {
                foreach (DataRow dr in client.Tables[0].Rows)
                {
                    //hard code every field...ugh
                    //textBoxNName.Text = dr[0].ToString();
                    //textBoxRName.Text = dr[1].ToString();
                    textBoxPhoneOpt.Text = dr[2].ToString();
                    textBoxFaxOpt.Text = dr[3].ToString();
                    textBoxContactOpt.Text = dr[4].ToString();
                    comboBoxOffice.SelectedIndex = comboBoxOffice.FindStringExact(dr[5].ToString());
                    comboBoxBT.SelectedIndex = comboBoxBT.FindStringExact(dr[6].ToString());
                    comboBoxPC.SelectedIndex = comboBoxPC.FindStringExact(dr[7].ToString());
                    comboBoxFeeSched.SelectedIndex = comboBoxFeeSched.FindStringExact(dr[8].ToString().Split(' ')[0]);
                    if (!dr[9].ToString().Equals("null"))
                        checkBoxTaskXRef.Checked = true;
                    else
                        checkBoxTaskXRef.Checked = false;
                    if (!dr[9].ToString().Equals("null"))
                        comboBoxTXRef.SelectedIndex = comboBoxTXRef.FindStringExact(dr[9].ToString().Split(' ')[0]);
                    comboBoxExpSched.SelectedIndex = comboBoxExpSched.FindStringExact(dr[10].ToString().Split(' ')[0]);
                    if (!dr[11].ToString().Equals("null"))
                        checkBoxExpXRef.Checked = true;
                    else
                        checkBoxExpXRef.Checked = false;
                    if (!dr[11].ToString().Equals("null"))
                        comboBoxEXRef.SelectedIndex = comboBoxEXRef.FindStringExact(dr[11].ToString().Split(' ')[0]);
                    comboBoxBillLayout.SelectedIndex = comboBoxBillLayout.FindStringExact(dr[12].ToString().Split(' ')[0]);
                    comboBoxBAgree.SelectedIndex = comboBoxBAgree.FindString(dr[13].ToString()); //fix
                    if (dr[14].ToString().Equals("Y"))
                        checkBoxIncludeExp.Checked = true;
                    else
                        checkBoxIncludeExp.Checked = false;
                    comboBoxRetainerType.SelectedIndex = comboBoxRetainerType.FindString(dr[15].ToString());//fix
                    comboBoxExpFreq.SelectedIndex = comboBoxExpFreq.FindString(dr[16].ToString());//fix
                    comboBoxFeeFreq.SelectedIndex = comboBoxFeeFreq.FindString(dr[17].ToString());//fix
                    textBoxMonthOpt.Text = dr[18].ToString();
                    textBoxCycleOpt.Text = dr[19].ToString();
                    textBoxIntPctOpt.Text = dr[20].ToString();
                    textBoxIntDaysOpt.Text = dr[21].ToString();
                    comboBoxDisc.SelectedIndex = comboBoxDisc.FindString(dr[22].ToString());//fix
                    textBoxDiscPctOpt.Text = dr[23].ToString();
                    comboBoxSurcharge.SelectedIndex = comboBoxSurcharge.FindString(dr[24].ToString());//fix
                    textBoxSurPctOpt.Text = dr[25].ToString();
                    if (dr[26].ToString().Equals("Y"))
                        checkBoxTax1.Checked = true;
                    else
                        checkBoxTax1.Checked = false;
                    if (dr[27].ToString().Equals("Y"))
                        checkBoxTax2.Checked = true;
                    else
                        checkBoxTax2.Checked = false;
                    if (dr[28].ToString().Equals("Y"))
                        checkBoxTax3.Checked = true;
                    else
                        checkBoxTax3.Checked = false;
                    if (dr[29].ToString().Equals("Y"))
                        checkBoxBudget.Checked = true;
                    else
                        checkBoxBudget.Checked = false;
                    if (dr[30].ToString().Equals("Y"))
                        checkBoxReqTaskCodes.Checked = true;
                    else
                        checkBoxReqTaskCodes.Checked = false;
                    if (dr[31].ToString().Equals("Y"))
                        checkBoxReqActCodes.Checked = true;
                    else
                        checkBoxReqActCodes.Checked = false;
                    comboBoxPreBillLayout.SelectedIndex = comboBoxPreBillLayout.FindStringExact(dr[32].ToString().Split(' ')[0]);
                }

            }

            //dont forget about resp and orig
            sql = "SELECT TOP 1 empid + '    ' + empname FROM ClientResponsibleTimekeeper inner join employee on empsysnbr = CRTEmployeeID where CRTClientID = " + clisysnbr.ToString();
            client.Clear();
            client = _jurisUtility.RecordsetFromSQL(sql);
            if (client != null && client.Tables.Count > 0)
            {
                foreach (DataRow dr in client.Tables[0].Rows)
                {
                    checkBoxRT.Checked = true;
                    comboBoxRT.SelectedIndex = comboBoxRT.FindStringExact(dr[0].ToString());
                }
            }
            else
                checkBoxRT.Checked = false;

            sql = "SELECT empid + '    ' + empname, COrigPcnt FROM CliOrigAtty inner join employee on empsysnbr = COrigAtty where COrigCli = " + clisysnbr.ToString();
            client.Clear();
            int row = 1;
            client = _jurisUtility.RecordsetFromSQL(sql);
            if (client != null && client.Tables.Count > 0)
            {
                foreach (DataRow dr in client.Tables[0].Rows)
                {
                    if (row == 1)
                    {
                        comboBoxOT1.SelectedIndex = comboBoxOT1.FindString(dr[0].ToString());
                        textBoxOTPct1Opt.Text = dr[1].ToString();
                    }
                    if (row == 2)
                    {
                        comboBoxOT2.SelectedIndex = comboBoxOT2.FindString(dr[0].ToString());
                        textBoxOTPct2Opt.Text = dr[1].ToString();
                    }
                    if (row == 3)
                    {
                        comboBoxOT3.SelectedIndex = comboBoxOT3.FindString(dr[0].ToString());
                        textBoxOTPct3Opt.Text = dr[1].ToString();
                    }
                    if (row == 4)
                    {
                        comboBoxOT4.SelectedIndex = comboBoxOT4.FindString(dr[0].ToString());
                        textBoxOTPct4Opt.Text = dr[1].ToString();
                    }
                    if (row == 5)
                    {
                        comboBoxOT5.SelectedIndex = comboBoxOT5.FindString(dr[0].ToString());
                        textBoxOTPct5Opt.Text = dr[1].ToString();
                    }



                    row++;
                }
            }
            else
                checkBoxRT.Checked = false;
        }





        private bool loadMatterBillFields()
        {
            getMatSysNbr();
            if (matsysnbr != 0)
            {
                string sql = "select name, data from DefaultSettings where defaultid = 999997  and empsys = " + empsysnbr.ToString();
                DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        sql = "update matter set " + dr[0].ToString() + " = replace('" + dr[1].ToString() + "', '|', char(13) + char(10)) where matsysnbr = " + matsysnbr.ToString();
                        if (_jurisUtility.ExecuteNonQuery(0, sql))
                            return true;
                    }
                } //else its not there so add it
                return false;
            } //we dont have a valid client so do nothing
            else
                return false;
        }

        private bool loadMatterUDFs()
        {
            getMatSysNbr();
            if (matsysnbr != 0)
            {
                string sql = "select name, entryType, data from DefaultSettings where defaultid = 999996  and empsys = " + empsysnbr.ToString();
                DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        if (dr[1].ToString().Equals("int"))
                            sql = "update matter set [" + dr[0].ToString() + "] = " + dr[2].ToString() + " where matsysnbr = " + matsysnbr.ToString();
                        else
                            sql = "update matter set [" + dr[0].ToString() + "] = '" + dr[2].ToString() + "' where matsysnbr = " + matsysnbr.ToString();
                        if (_jurisUtility.ExecuteNonQuery(0, sql))
                        {
                            return true;
                        }
                    }
                } //else its not there so add it
                return false;
            } //we dont have a valid matter so do nothing
            else
                return false;
        }

        private void buttonCliBilling_Click(object sender, EventArgs e)
        {
            MatBillingForm matB = new MatBillingForm(_jurisUtility, empsysnbr);
            if (matB.loadFields())
            {
                matB.ShowDialog();
            }
            else
                matB.Close();
        }

        private void textBoxMatterCode_Leave(object sender, EventArgs e)
        {

            if (textBoxMatterCode.Text.Length > lengthOfCodeMatter)
                MessageBox.Show("Matter Code is longer than allowed. Your settings allow for " + lengthOfCodeMatter.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (codeIsNumericMatter && !isNumeric(textBoxMatterCode.Text))
                MessageBox.Show("Matter Code is not numeric. Your settings require a numeric code.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string code = formatMatterCode(textBoxMatterCode.Text);
                if (codeIsNumericMatter)
                    textBoxMatterCode.Text = code.Substring(code.Length - lengthOfCodeMatter, lengthOfCodeMatter);
                else
                    textBoxMatterCode.Text = code;
            }
        }

        private void MatterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            int nX = System.Windows.Forms.Cursor.Position.X;
            int nY = System.Windows.Forms.Cursor.Position.Y;
            if (SendMessage(this.Handle, WM_NCHITTEST, 0, MakeLong((short)nX, (short)nY)) == HTCLOSE)
            {
                exitToMain = true;
            }
            if (exitToMain)
            {
                string sql = "delete from Defaults where id in (999993) and userid = " + empsysnbr.ToString();
                _jurisUtility.ExecuteNonQuery(0, sql);
                System.Environment.Exit(1);

            }
        }

        private void closeAndCreateClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pt = this.Location;
            ClientForm cleared = new ClientForm(_jurisUtility, 0, false, pt, empsysnbr);
            cleared.Show();
            this.Close();
        }

        private void buttonNoteCard_Click(object sender, EventArgs e)
        {
            this.Hide();
            AddNoteCard adn = new AddNoteCard(pt, "Add Matter Note Card");
            adn.ShowDialog();
            noteName = adn.name;
            noteText = adn.text;
            adn.Close();
            if (string.IsNullOrEmpty(noteName) && string.IsNullOrEmpty(noteText))
                addNoteCard = false;
            else
                addNoteCard = true;
            this.Show();
        }

        private void buttonCliLookUp_Click(object sender, EventArgs e)
        {
            this.Hide();
            ClientLookUp cl = new ClientLookUp(_jurisUtility, pt);
            cl.ShowDialog();
            if (cl.clientSelected)
            {
                clisysnbr = cl.clisysnbr;
                textBoxCode.Text = cl.clicode;
            }
            cl.Close();
            loadClientInfoForMatter();
            getNextMatterNumber();
            loadAddys();
            loadConsolidations();
            cl.Close();
            this.Show(); //billto name based on flag = c
        }

        private void textBoxMatterCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (clisysnbr != 0)
            {
                this.Hide();
                MatLookUp cl = new MatLookUp(_jurisUtility, pt, clisysnbr);
                cl.ShowDialog();
                cl.Close();
                this.Show();
            }
            else
            {
                MessageBox.Show("A client must be selected first", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            MatUDFFields matB = new MatUDFFields(_jurisUtility, empsysnbr);
            if (matB.loadFields())
            {
                matB.ShowDialog();
            }
            else
                matB.Close();
        }

        private void checkBoxConsolidation_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxConsolidation.Checked)
            {
                checkBoxChooseAddy.Enabled = false;
                comboBoxAddyChoose.Enabled = false;
                textBoxBANName.Enabled = false;
                richTextBoxBAAddy.Enabled = false;
                textBoxBAPhoneOpt.Enabled = false;
                textBoxBAFaxOpt.Enabled = false;
                textBoxBAContactOpt.Enabled = false;
                textBoxBANameOpt.Enabled = false;
                textBoxBACityOpt.Enabled = false;
                textBoxBAStateOpt.Enabled = false;
                textBoxBAZipOpt.Enabled = false;
                textBoxBACountryOpt.Enabled = false;
                textBoxBAEmailOpt.Enabled = false;
            }
            else
            {
                checkBoxChooseAddy.Enabled = true;
                comboBoxAddyChoose.Enabled = true;
                textBoxBANName.Enabled = true;
                richTextBoxBAAddy.Enabled = true;
                textBoxBAPhoneOpt.Enabled = true;
                textBoxBAFaxOpt.Enabled = true;
                textBoxBAContactOpt.Enabled = true;
                textBoxBANameOpt.Enabled = true;
                textBoxBACityOpt.Enabled = true;
                textBoxBAStateOpt.Enabled = true;
                textBoxBAZipOpt.Enabled = true;
                textBoxBACountryOpt.Enabled = true;
                textBoxBAEmailOpt.Enabled = true;
            }
        }

        public int MakeLong(short lowPart, short highPart) // to catch clicking the Red X to close
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }

        private void comboBoxConsolidation_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }


}