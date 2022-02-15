using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using Gizmox.Controls;
using System.Runtime.InteropServices;


namespace JurisUtilityBase
{
    public partial class ClientForm : Form
    {
        public ClientForm(JurisUtility jutil, int preID, bool modify, System.Drawing.Point ppt, int empsys)
        {
            InitializeComponent();
            _jurisUtility = jutil;
            presetID = preID;
            isModification = modify;
            pt = ppt;
            empsysnbr = empsys;
        }

        JurisUtility _jurisUtility;
        int presetID = 0;
        bool isModification = false;
        public List<string> errorList = new List<string>();
        bool codeIsNumeric = false;
        int clisysnbr = 0;
        bool isError = false;
        int lengthOfCode = 4;
        private System.Drawing.Point pt;
        string noteName = "";
        string noteText = "";
        int empsysnbr = 0;
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
                System.Environment.Exit(0);
            }
            this.Location = pt;
            dateTimePickerOpened.Value = DateTime.Now; //OpenedDate

            //see if a default exists and keep the ID for later use
            checkForTables();
            string sql = "select id from defaults where IsStandard = 'Y' and userid = " + empsysnbr.ToString();
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            if (presetID == 0) // if there is no preset passe by presetmamager then use default
            {
                if (dds != null && dds.Tables.Count > 0 && dds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                        presetID = Convert.ToInt32(dr[0].ToString());
                } //else its not there so add it
            }
            DataSet myRSPC2 = new DataSet();
            //if clicode is Numeric then increment by 1
            getSettings();

            getNextClientNumber();


            //get number of originators
            string cell = "";
            sql = "  select SpTxtValue from sysparam where SpName = 'CfgTkprOpts'";
            dds.Clear();
            dds = _jurisUtility.RecordsetFromSQL(sql); //the first character should be a number...if not, do nothing
            int numOfOrig = 5;
            string[] temp = null;
            try
            {
                if (dds != null && dds.Tables.Count > 0 && dds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        cell = dr[0].ToString();
                    }
                    temp = cell.Split(',');
                    numOfOrig = Convert.ToInt32(temp[0]);
                }
            }
            catch (Exception)   { }

            hideOrShowOriginators(numOfOrig);

            //Office
            comboBoxOffice.ClearItems();
            myRSPC2.Clear();
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

            //pract Class
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
                System.Environment.Exit(1);
            }
            else
            {
                if (presetID != 0)
                    loadDfaultPreset();
                if (isModification)
                {
                    buttonCreateClient.Text = "Save Template";
                    buttonCreateClient.Click -=  button1_Click;
                    buttonCreateClient.Click += buttonModify;
                }
            }
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

        private void getSettings()
        {
            string sql = "  select SpTxtValue from sysparam where SpName = 'FldClient'";
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            string cell = "";
            if (dds != null && dds.Tables.Count > 0)
            {
                foreach (DataRow dr in dds.Tables[0].Rows)
                    cell = dr[0].ToString();
            }

            string[] test = cell.Split(',');
            lengthOfCode = Convert.ToInt32(test[2]);


            if (test[1].Equals("C"))
                codeIsNumeric = false;
            else
                codeIsNumeric = true;

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

        private void loadDfaultPreset()
        {
            checkForTables();
            //make sure default numbers are set 
            textBoxOTPct1Opt.Text = "100";
            textBoxOTPct2Opt.Text = "0";
            textBoxOTPct3Opt.Text = "0";
            textBoxOTPct4Opt.Text = "0";
            textBoxOTPct5Opt.Text = "0";
            textBoxMonthOpt.Text = "1";
            textBoxCycleOpt.Text = "1";
            textBoxIntDaysOpt.Text = "0";
            textBoxIntPctOpt.Text = "0.00";
            textBoxDiscPctOpt.Text = "0.00";
            textBoxSurPctOpt.Text = "0.00";
            textBoxBANName.Text = "MAIN";
            richTextBoxBAAddy.Text = "";

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


        private void loadDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ds = MessageBox.Show("This will clear anything already on the Client form. Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            DataSet ds1;
            if (ds == DialogResult.Yes)
            {
                checkForTables();
                string sql = "select ID, name as [Default Name],  convert(varchar,CreationDate, 101) as [Creation Date], isStandard as [Default] from Defaults where id < 999990 and  userid =  " + empsysnbr.ToString();
                ds1 = _jurisUtility.RecordsetFromSQL(sql);
                pt = this.Location;
                PresetManager DM = new PresetManager(ds1, _jurisUtility, pt, empsysnbr); 
                DM.Show();
                this.Hide();
                this.Close();


                
            }
        }

        private void clearFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pt = this.Location;
            ClientForm cleared = new ClientForm(_jurisUtility, 0, false, pt, empsysnbr);
            cleared.Show();
            this.Hide();
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

        private void clearFieldsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pt = this.Location;
            MatterForm cleared = new MatterForm(_jurisUtility,  0, "", 0, pt, empsysnbr);
            cleared.Show();
            this.Hide();
            this.Close();
        }

        private void saveAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkForTables();
            checkDefaultName();
        }

        private void checkDefaultName()
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Name of new Template", "Template Name", "New Template");
            if (!string.IsNullOrEmpty(name))
            {
                //see if default name already exists
                checkForTables();
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
                    MessageBox.Show("Names must be unique and that name already exists." + "\r\n" + "Template not added", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("A valid name is required. Template not added", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void createDefault(string name)
        {
            checkForTables();
            string sql = "insert into defaults (ID, name, userid, CreationDate, IsStandard, AllData ) " +
                " values ((case when (select max(ID) from defaults where id < 999990) is null then 1 else ((select max(ID) from defaults where id < 999990) + 1) end), '" + name + "', " + empsysnbr.ToString() + ", getdate(), 'N', '')";

            _jurisUtility.ExecuteNonQuery(0, sql);

            sql = "select max(id) from defaults where id < 999990 and userid = " + empsysnbr.ToString();
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            int defID = 0;
            if (dds != null && dds.Tables.Count > 0)
            {
                foreach (DataRow dr in dds.Tables[0].Rows)
                    defID = Convert.ToInt32(dr[0].ToString());
            } //else its not there so add it

            foreach (var textbox in this.Controls.OfType<TextBox>())
            {
                if (!string.IsNullOrEmpty(textbox.Text) && !textbox.Name.Equals("textBoxCode") )
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
            buttonCreateClient.Enabled = false;
            if (codeIsNumeric) // is the sysparam setting a number?
            {
                if (isNumeric(textBoxCode.Text)) // if so, did they enter a number?
                {
                    if (textBoxCode.Text.Length > lengthOfCode) // is it too many characters?
                        MessageBox.Show("Client Code" + textBoxCode.Text + " is too long. " + "\r\n" + "Your settings only allow for up to " + lengthOfCode.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        createClient();
                }
                else
                    MessageBox.Show("Client Code" + textBoxCode.Text + " is not numeric." + "\r\n" + "Your settings require a number", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else // is it aplha? if so, we only care if its too long
            {
                if (textBoxCode.Text.Length > lengthOfCode)
                    MessageBox.Show("Client Code" + textBoxCode.Text + " is too long. " + "\r\n" + "Your settings only allow for up to " + lengthOfCode.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    createClient();

            }
        }

        private void buttonModify(object sender, EventArgs e)
        {
            checkForTables();
            string sql = "delete from DefaultSettings where defaultid = " + presetID.ToString();
            _jurisUtility.ExecuteNonQuery(0, sql);

            foreach (var textbox in this.Controls.OfType<TextBox>())
            {
                if (!string.IsNullOrEmpty(textbox.Text) && !textbox.Name.Equals("textBoxCode"))
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
            sql = "select ID, name as [Default Name],  convert(varchar,CreationDate, 101) as [Creation Date], isStandard as [Default] from Defaults where userid = " + empsysnbr.ToString();
            DataSet ds1 = _jurisUtility.RecordsetFromSQL(sql);
            pt = this.Location;
            PresetManager DM = new PresetManager(ds1, _jurisUtility, pt, empsysnbr);
            DM.Show();
            this.Hide();
            this.Close();

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

            }
            else
            {
                checkBoxIncludeExp.Visible = false;
            }
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
            catch (Exception) { return false; }
        }


        private string formatClientCode(string code)
        {
            string formattedCode = "";
            if (codeIsNumeric)
            {
                formattedCode = "000000000000" + code;
                formattedCode = formattedCode.Substring(formattedCode.Length - 12, 12);
            }
            else
                formattedCode = code;
            return formattedCode;

        }


        private bool checkFields()
        {
            if (clisysnbr == 0)
            {
                if (textBoxCode.Text.Length > lengthOfCode)
                {
                    MessageBox.Show("Client Code is longer than allowed." + "\r\n" + "Your settings allow for " + lengthOfCode.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonCreateClient.Enabled = true;
                    return false;

                }
                if (codeIsNumeric && !isNumeric(textBoxCode.Text))
                {
                    MessageBox.Show("Client Code is not numeric." + "\r\n" + "Your settings require a numeric code.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonCreateClient.Enabled = true;
                    return false;
                }
                string code = formatClientCode(textBoxCode.Text);
                string sql = "select clisysnbr from client where clicode = '" + code + "'";
                DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows) //client already exists
                    {
                        MessageBox.Show("Client " + textBoxCode.Text + " already exists. Enter a valid client code." + "\r\n" + "Codes must match the format in which they appear in Juris." + "\r\n" + "This includes leading zeroes", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        buttonCreateClient.Enabled = true;
                        return false;
                        
                    }

                }
                if (!checkForRequiredUDFs())
                {
                    buttonCreateClient.Enabled = true;
                    return false;
                }

            }

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


            //ensure no apostrophes or double quotes as they break sql
            foreach (var textbox in this.Controls.OfType<TextBox>())
                    textbox.Text = textbox.Text.Replace("'", "").Replace("\"", "").Replace(@"\", " ").Replace("%", "").Replace("[", "").Replace("]", "").Replace("_", " ").Replace("^", ""); 

                foreach (var textbox in this.Controls.OfType<RichTextBox>())
                    textbox.Text = textbox.Text.Replace("'", "").Replace("\"", "").Replace(@"\", " ").Replace("%", "").Replace("[", "").Replace("]", "").Replace("_", " ").Replace("^", ""); 

                if (incorrectFields.Count == 0)
                {
                    if (string.IsNullOrEmpty(richTextBoxBAAddy.Text))
                    {
                        MessageBox.Show("All fields in black text are required." + "\r\n" + "Please correct this issue and retry", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonCreateClient.Enabled = true;
                    return false;
                    }
                    else
                    {
                        foreach (var textbox in this.Controls.OfType<TextBox>())
                        {
                            if (string.IsNullOrEmpty(textbox.Text)) //if there is nothing in it, is it required?
                            {
                                if (!textbox.Name.EndsWith("Opt") && !textbox.Name.Equals("textBoxConsolidation"))
                                {
                                MessageBox.Show(textbox.Name);
                                    MessageBox.Show("All fields in black text are required." + "\r\n" + "Please correct this issue and retry", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    buttonCreateClient.Enabled = true;
                                    return false;
                                }

                            }

                        }   
                    }
                    if (Convert.ToInt32(textBoxMonthOpt.Text) < 1 || Convert.ToInt32(textBoxMonthOpt.Text) > 12 && ( this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("A") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("A")))
                    {
                    MessageBox.Show("When Annual is selected, the Month must be 1 through 12.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonCreateClient.Enabled = true; 
                        return false;
                    }
                    if (Convert.ToInt32(textBoxMonthOpt.Text) < 1 || Convert.ToInt32(textBoxMonthOpt.Text) > 6 && (this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("S") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("S")))
                    {
                        MessageBox.Show("When Semi Annual is selected, the Month must be 1 through 6.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                        buttonCreateClient.Enabled = true;
                        return false;
                    }
                    if (Convert.ToInt32(textBoxMonthOpt.Text) < 1 || Convert.ToInt32(textBoxMonthOpt.Text) > 4 && (this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("Q") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("Q")))
                    {
                        MessageBox.Show("When Quarterly is selected, the Month must be 1 through 4.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        buttonCreateClient.Enabled = true;
                        return false;
                    }
                    if (Convert.ToInt32(textBoxCycleOpt.Text) < 1 || Convert.ToInt32(textBoxCycleOpt.Text) > 999 && (this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0].Equals("C") || this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0].Equals("C")))
                    {
                        MessageBox.Show("When Cycle is selected, the Month must be 1 through 999.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        buttonCreateClient.Enabled = true;
                        return false;
                    } //textBoxIntDaysOpt
                      //textBoxCycleOpt.Text
                    if (!isInteger(textBoxCycleOpt.Text)  || !isInteger(textBoxIntDaysOpt.Text) || !isInteger(textBoxMonthOpt.Text))
                    {
                        MessageBox.Show("Cycle, Interest Days and Month must be integers.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBoxMonthOpt.Text = "1";
                        textBoxCycleOpt.Text = "1";
                        textBoxIntDaysOpt.Text = "0";
                        buttonCreateClient.Enabled = true;
                        return false;
                    }
                    if (checkBoxConsolidation.Checked && string.IsNullOrEmpty(textBoxConsolidation.Text))
                {
                    MessageBox.Show("A Consolidation name is required. Please add a name or uncheck the box", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            string sysparam = " SELECT SpTxtValue, SpName FROM SysParam where spname like 'FldClientUDF%' and sptxtvalue not like 'C UDF%' ";

            DataSet dds2 = _jurisUtility.RecordsetFromSQL(sysparam);
            if (dds2 != null && dds2.Tables.Count > 0 && dds2.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dds2.Tables[0].Rows) // if any defined UDF fields are 'R', see if they actually added them through UDF button
                {
                    string[] test = dr[0].ToString().Split(',');
                    if (test[3].ToString().Equals("R"))
                    {
                        sysparam = "select * from DefaultSettings where DefaultID = 999994 and [name] = '" + test[0].ToString().Replace(" ", "") + "'";
                        DataSet dds3 = _jurisUtility.RecordsetFromSQL(sysparam);
                        if (dds3 == null || dds3.Tables.Count == 0 || dds3.Tables[0].Rows.Count == 0)
                        {
                            MessageBox.Show("At least 1 UDF field is required. Please populate the required UDF field(s).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void createClient()
        {
            int addyid = 0;
            if (checkFields())
            {
                string txref = "null";
                if (checkBoxTaskXRef.Checked)
                    txref = "'" + this.comboBoxTXRef.GetItemText(this.comboBoxTXRef.SelectedItem).Split(' ')[0] + "'";

                string exref = "null";
                if (checkBoxExpXRef.Checked)
                    exref = "'" + this.comboBoxEXRef.GetItemText(this.comboBoxEXRef.SelectedItem).Split(' ')[0] + "'";

                string resp = "null";
                if (checkBoxRT.Checked)
                    resp = " (select empsysnbr from employee where empid = '" + this.comboBoxRT.GetItemText(this.comboBoxRT.SelectedItem).Split(' ')[0] + "')";

                string inclExp = ((bool?)checkBoxIncludeExp.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string tax1 = ((bool?)checkBoxTax1.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string tax2 = ((bool?)checkBoxTax2.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string tax3 = ((bool?)checkBoxTax3.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string budg = ((bool?)checkBoxBudget.Checked) == true ? '1'.ToString() : '0'.ToString();
                string reqTask = ((bool?)checkBoxReqTaskCodes.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                string reqAct = ((bool?)checkBoxReqActCodes.Checked) == true ? 'Y'.ToString() : 'N'.ToString();

                string retType = ((bool?)comboBoxRetainerType.Visible) == true ? this.comboBoxRetainerType.GetItemText(this.comboBoxRetainerType.SelectedItem).Split(' ')[0] : string.Empty;

                string sql = "Insert into Client(CliSysNbr,CliCode,CliNickName,CliReportingName,CliSourceOfBusiness, " +
                  " CliPhoneNbr,CliFaxNbr,CliContactName,CliDateOpened,CliOfficeCode,CliBillingAtty,CliPracticeClass, "
    + " CliFeeSch,CliTaskCodeXref,CliExpSch,CliExpCodeXref,CliBillFormat,CliBillAgreeCode,CliFlatFeeIncExp,CliRetainerType,CliExpFreqCode,CliFeeFreqCode,CliBillMonth,CliBillCycle, "
    + " CliExpThreshold,CliFeeThreshold,CliInterestPcnt,CliInterestDays,CliDiscountOption,CliDiscountPcnt,CliSurchargeOption,CliSurchargePcnt, " +
    " CliTax1Exempt,CliTax2Exempt,CliTax3Exempt,CliBudgetOption,CliReqPhaseOnTrans, "
    + " CliReqTaskCdOnTime,CliReqActyCdOnTime,CliReqTaskCdOnExp,CliPrimaryAddr,CliType,CliEditFormat,CliThresholdOption,CliRespAtty," +
    "CliBillingField01,CliBillingField02,CliBillingField03,CliBillingField04,CliBillingField05, CliBillingField06,CliBillingField07,CliBillingField08,CliBillingField09,CliBillingField10,CliBillingField11,CliBillingField12,CliBillingField13,CliBillingField14,CliBillingField15,CliBillingField16,CliBillingField17,CliBillingField18,CliBillingField19, CliBillingField20, "
    + " CliCTerms,CliCStatus,CliCStatus2)  "
    + " values( case when (select max(clisysnbr) from client) is null then 1 else ((select max(clisysnbr) from client) + 1) end, '" + formatClientCode(textBoxCode.Text) + "', '" + textBoxNName.Text.Trim() + "', '" + textBoxRName.Text.Trim() + "', '" + textBoxSoBOpt.Text.Trim() + "', "
    + " '" + textBoxPhoneOpt.Text.Trim() + "', '" + textBoxFaxOpt.Text.Trim() + "', '" + textBoxContactOpt.Text.Trim() + "', '" + dateTimePickerOpened.Value.ToString("MM/dd/yyyy") + "', '" + this.comboBoxOffice.GetItemText(this.comboBoxOffice.SelectedItem).Split(' ')[0] + "', "
    + " (select empsysnbr from employee where empid = '" + this.comboBoxBT.GetItemText(this.comboBoxBT.SelectedItem).Split(' ')[0] + "'), "
    + "'" + this.comboBoxPC.GetItemText(this.comboBoxPC.SelectedItem).Split(' ')[0] + "', "
    + " '" + this.comboBoxFeeSched.GetItemText(this.comboBoxFeeSched.SelectedItem).Split(' ')[0] + "'," + txref + ",'" + this.comboBoxExpSched.GetItemText(this.comboBoxExpSched.SelectedItem).Split(' ')[0] + "'," + exref + ", "
    + " '" + this.comboBoxBillLayout.GetItemText(this.comboBoxBillLayout.SelectedItem).Split(' ')[0] + "','" + this.comboBoxBAgree.GetItemText(this.comboBoxBAgree.SelectedItem).Split(' ')[0] + "','" + inclExp + "','" + retType + "', '" + this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0] + "', '" + this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0] + "' ," + textBoxMonthOpt.Text + "," + textBoxCycleOpt.Text + ", "
    + " 0.00,0.00," + textBoxIntPctOpt.Text + "," + textBoxIntDaysOpt.Text + "," + this.comboBoxDisc.GetItemText(this.comboBoxDisc.SelectedItem).Split(' ')[0] + "," + textBoxDiscPctOpt.Text + ", " + this.comboBoxSurcharge.GetItemText(this.comboBoxSurcharge.SelectedItem).Split(' ')[0] + ", " + textBoxSurPctOpt.Text + ", "
    + " '" + tax1 + "','" + tax2 + "','" + tax3 + "'," + budg + ",'N','" + reqTask + "','" + reqAct + "','N',null,0,'" + this.comboBoxPreBillLayout.GetItemText(this.comboBoxPreBillLayout.SelectedItem).Split(' ')[0] + "', "
    + " 0," + resp + ",'','','','','','','','','','','','', "
    + " '','','','','','','','',0,0,'')";

               isError =  _jurisUtility.ExecuteNonQuery(0, sql);
                if (!isError) //was there an error adding the client? if so, we didnt make any changes so we are good
                {
                    clisysnbr = getClisysnbr();
                    if (clisysnbr != 0)
                    {
                        //add log
                        DateTime nn = DateTime.Now;
                        string pcName = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
                        sql = "Insert into Client_log([DateTimeStamp] ,[HostMachine],[Application] ,[JurisUser] ,[RecordType] ,[SourceTable],[Notes],CliSysNbr,CliCode,CliNickName,CliReportingName,CliSourceOfBusiness, " +
                      " CliPhoneNbr,CliFaxNbr,CliContactName,CliDateOpened,CliOfficeCode,CliBillingAtty,CliPracticeClass, "
                        + " CliFeeSch,CliTaskCodeXref,CliExpSch,CliExpCodeXref,CliBillFormat,CliBillAgreeCode,CliFlatFeeIncExp,CliRetainerType,CliExpFreqCode,CliFeeFreqCode,CliBillMonth,CliBillCycle, "
                        + " CliExpThreshold,CliFeeThreshold,CliInterestPcnt,CliInterestDays,CliDiscountOption,CliDiscountPcnt,CliSurchargeOption,CliSurchargePcnt, " +
                        " CliTax1Exempt,CliTax2Exempt,CliTax3Exempt,CliBudgetOption,CliReqPhaseOnTrans, "
                        + " CliReqTaskCdOnTime,CliReqActyCdOnTime,CliReqTaskCdOnExp,CliPrimaryAddr,CliType,CliEditFormat,CliThresholdOption,CliRespAtty," +
                        "CliBillingField01,CliBillingField02,CliBillingField03,CliBillingField04,CliBillingField05, CliBillingField06,CliBillingField07,CliBillingField08,CliBillingField09,CliBillingField10,CliBillingField11,CliBillingField12,CliBillingField13,CliBillingField14,CliBillingField15,CliBillingField16,CliBillingField17,CliBillingField18,CliBillingField19, CliBillingField20, "
                        + " CliCTerms,CliCStatus,CliCStatus2)  "
                        + " values( '" + nn.ToString("yyyy-MM-dd HH:mm") + "', left('" + pcName + "', 30), 'CMI Tool',1, 1,40,null,"
                        + clisysnbr + ", '" + formatClientCode(textBoxCode.Text) + "', '" + textBoxNName.Text.Trim() + "', '" + textBoxRName.Text.Trim() + "', '" + textBoxSoBOpt.Text.Trim() + "', "
                        + " '" + textBoxPhoneOpt.Text.Trim() + "', '" + textBoxFaxOpt.Text.Trim() + "', '" + textBoxContactOpt.Text.Trim() + "', '" + dateTimePickerOpened.Value.ToString("MM/dd/yyyy") + "', '" + this.comboBoxOffice.GetItemText(this.comboBoxOffice.SelectedItem).Split(' ')[0] + "', "
                        + " (select empsysnbr from employee where empid = '" + this.comboBoxBT.GetItemText(this.comboBoxBT.SelectedItem).Split(' ')[0] + "'), "
                        + "'" + this.comboBoxPC.GetItemText(this.comboBoxPC.SelectedItem).Split(' ')[0] + "', "
                        + " '" + this.comboBoxFeeSched.GetItemText(this.comboBoxFeeSched.SelectedItem).Split(' ')[0] + "'," + txref + ",'" + this.comboBoxExpSched.GetItemText(this.comboBoxExpSched.SelectedItem).Split(' ')[0] + "'," + exref + ", "
                        + " '" + this.comboBoxBillLayout.GetItemText(this.comboBoxBillLayout.SelectedItem).Split(' ')[0] + "','" + this.comboBoxBAgree.GetItemText(this.comboBoxBAgree.SelectedItem).Split(' ')[0] + "','" + inclExp + "','" + retType + "', '" + this.comboBoxExpFreq.GetItemText(this.comboBoxExpFreq.SelectedItem).Split(' ')[0] + "', '" + this.comboBoxFeeFreq.GetItemText(this.comboBoxFeeFreq.SelectedItem).Split(' ')[0] + "' ," + textBoxMonthOpt.Text + "," + textBoxCycleOpt.Text + ", "
                        + " 0.00,0.00," + textBoxIntPctOpt.Text + "," + textBoxIntDaysOpt.Text + "," + this.comboBoxDisc.GetItemText(this.comboBoxDisc.SelectedItem).Split(' ')[0] + "," + textBoxDiscPctOpt.Text + ", " + this.comboBoxSurcharge.GetItemText(this.comboBoxSurcharge.SelectedItem).Split(' ')[0] + ", " + textBoxSurPctOpt.Text + ", "
                        + " '" + tax1 + "','" + tax2 + "','" + tax3 + "'," + budg + ",'N','" + reqTask + "','" + reqAct + "','N',null,0,'" + this.comboBoxPreBillLayout.GetItemText(this.comboBoxPreBillLayout.SelectedItem).Split(' ')[0] + "', "
                        + " 0," + resp + ",'','','','','','','','','','','','', "
                        + " '','','','','','','','',0,0,'')";
                        isError = _jurisUtility.ExecuteNonQuery(0, sql);
                        if (!isError)
                        {

                            if (!resp.Equals("null"))
                                addRespToTable(resp); //you have to add it twice for some reason...in client and ClientResponsibleTimekeeper
                            if (!isError) // was there an issue adding resp tkpr? if so, undo client
                            {
                                sql = "update ClientResponsibleTimekeeper_Log set jurisuser = " + empsysnbr.ToString() + " where jurisuser is null and convert(varchar,DateTimeStamp, 101) = convert(varchar,getdate(), 101)";
                                _jurisUtility.ExecuteNonQuery(0, sql);
                                isError = createAddy();
                                if (!isError) //was there an issue adding the address? If so, we need to remove the client and resp
                                {
                                    sql = "update BillingAddress_Log set jurisuser = " + empsysnbr.ToString() + " where jurisuser is null and convert(varchar,DateTimeStamp, 101) = convert(varchar,getdate(), 101)";
                                    _jurisUtility.ExecuteNonQuery(0, sql);

                                    sql = "select max(biladrsysnbr) from billingaddress where BilAdrCliNbr = " + clisysnbr.ToString();
                                    DataSet dds = new DataSet();
                                    
                                    dds = _jurisUtility.RecordsetFromSQL(sql);
                                    if (dds != null && dds.Tables.Count > 0)
                                    {
                                        foreach (DataRow dr in dds.Tables[0].Rows)
                                            addyid = Convert.ToInt32(dr[0].ToString());

                                    }
                                    isError = addOrig();
                                    if (!isError)
                                    {
                                        sql = "update CliOrigAtty_log set jurisuser = " + empsysnbr.ToString() + " where jurisuser is null and convert(varchar,DateTimeStamp, 101) = convert(varchar,getdate(), 101)";
                                        _jurisUtility.ExecuteNonQuery(0, sql);
                                        isError = loadClientBillFields();
                                        if (!isError)
                                        {
                                            isError = loadClientUDFFields();
                                            if (!isError)
                                            {
                                                isError = createConsolidation(resp);
                                                if (!isError)
                                                {
                                                    //handles making of client and editing it for billing fields/udfs
                                                    sql = "update client_log set jurisuser = " + empsysnbr.ToString() + " where jurisuser is null and convert(varchar,DateTimeStamp, 101) = convert(varchar,getdate(), 101)";
                                                    _jurisUtility.ExecuteNonQuery(0, sql);

                                                    string SQL = "Insert into DocumentTree(dtdocid, dtsystemcreated, dtdocclass,dtdoctype,  dtparentid, dttitle, dtkeyl) "
                                                    + " select (select max(dtdocid)  from documenttree) + 1 , 'Y',4200,'R', 22, Clireportingname, Clisysnbr "
                                                    + " from Client where clisysnbr = " + clisysnbr.ToString();
                                                    _jurisUtility.ExecuteNonQuery(0, SQL);

                                                    SQL = " Update sysparam set spnbrvalue=(select max(dtdocid) from documenttree) where spname='LastSysNbrDocTree'";
                                                    _jurisUtility.ExecuteNonQuery(0, SQL);

                                                    SQL = " update sysparam set spnbrvalue = (select max(CliSysNbr) from client) where spname = 'LastSysNbrClient'";
                                                    _jurisUtility.ExecuteNonQuery(0, SQL);

                                                    sql = "update sysparam set spnbrvalue = (select max(biladrsysnbr) from billingaddress) where spname = 'LastSysNbrBillAddress'";
                                                    _jurisUtility.ExecuteNonQuery(0, sql);

                                                    sql = "update sysparam set spnbrvalue = (select max(billtosysnbr) from billto) where spname = 'LastSysNbrBillTo'";
                                                    _jurisUtility.ExecuteNonQuery(0, sql);

                                                    //add notecard if thye selected it
                                                    sql = "insert into [ClientNote] ([CNClient] ,[CNNoteIndex],[CNObject],[CNNoteText],[CNNoteObject]) values(" + clisysnbr.ToString() + ", replace('" + noteName + "', '|', char(13) + char(10)), '', replace('" + noteText + "', '|', char(13) + char(10)), null)";
                                                    _jurisUtility.ExecuteNonQuery(0, sql);

                                                    sql = "update ClientNote_Log set jurisuser = " + empsysnbr.ToString() + " where jurisuser is null and convert(varchar,DateTimeStamp, 101) = convert(varchar,getdate(), 101)";
                                                    _jurisUtility.ExecuteNonQuery(0, sql);

                                                    //after adding the client, load the preset back in
                                                    if (presetID != 0)
                                                        checkForTables();

                                                    DialogResult fc = MessageBox.Show("Client " + textBoxCode.Text + " was added successfully." + "\r\n" + "Would you like to Add a Matter to this Client?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                                    if (fc == DialogResult.Yes)
                                                    {
                                                        //save info to move over to matter
                                                        saveInfoToMoveToMatter();
                                                        pt = this.Location;
                                                        MatterForm cleared = new MatterForm(_jurisUtility, clisysnbr, textBoxCode.Text, addyid, pt, empsysnbr);
                                                        cleared.Show();
                                                        this.Hide();
                                                        //move data over
                                                        this.Close();
                                                    }
                                                    else
                                                    {
                                                        sql = "delete from DefaultSettings where defaultid = 999998 and empsys = " + empsysnbr.ToString(); //stored BF info
                                                        _jurisUtility.ExecuteNonQuery(0, sql);
                                                        sql = "delete from Defaults where id = 999998 and userid = " + empsysnbr.ToString();
                                                        _jurisUtility.ExecuteNonQuery(0, sql);
                                                        sql = "delete from DefaultSettings where defaultid = 999994 and empsys = " + empsysnbr.ToString(); //stored UDF info
                                                        _jurisUtility.ExecuteNonQuery(0, sql);
                                                        sql = "delete from Defaults where id = 999994  and userid = " + empsysnbr.ToString();
                                                        _jurisUtility.ExecuteNonQuery(0, sql);
                                                        pt = this.Location;
                                                        ClientForm newClient = new ClientForm(_jurisUtility, presetID, false, pt, empsysnbr);
                                                        newClient.Show();
                                                        this.Hide();
                                                        this.Close();
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("There was an issue adding the consolidation." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    isError = false;
                                                    undoOrig();
                                                    undoResp();
                                                    undoAddy(addyid);
                                                    undoClient();
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("There was an issue adding the UDF Fields." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                isError = false;
                                                undoOrig();
                                                undoResp();
                                                undoAddy(addyid);
                                                undoClient();
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("There was an issue adding the Billing Fields." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            isError = false;
                                            undoOrig();
                                            undoResp();
                                            undoAddy(addyid);
                                            undoClient();
                                        }

                                    }
                                    else
                                    {
                                        MessageBox.Show("There was an issue adding the Originator." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        isError = false; 
                                        undoResp();
                                        undoAddy(addyid);
                                        undoClient();
                                        
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("There was an issue adding the address." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    isError = false; 
                                    undoResp();
                                    undoClient();
                                }
                            }
                            else
                            {
                                MessageBox.Show("There was an issue adding the Responsible Timekeepers." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                isError = false; 
                                undoClient();
                            }
                        }
                        else
                        {
                            MessageBox.Show("There was an issue adding the Client." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                            
                        }
                    }
                }
                else
                {
                    MessageBox.Show("There was an issue adding the client." + "\r\n" + "No changes were made to your database" + "\r\n" + _jurisUtility.errorMessage, "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    clisysnbr = 0;

                     //TextWriter ss = new StreamWriter(@"c:\intel\sql1.txt");
                     //ss.Write(_jurisUtility.errorMessage);
                     //ss.Flush();
                     //ss.Close();
                    isError = false;
                }
            }
        }

        private void undoClient()
        {
            try
            {
                _jurisUtility.errorMessage = "";
                string sql = "delete from client where clisysnbr = " + clisysnbr.ToString();
                _jurisUtility.ExecuteNonQuery(0, sql);
                clisysnbr = 0;
                isError = false;
            }
            catch (Exception exc)  { }

        }



        private void undoAddy(int addyID)
        {
            try
            {
                string sql = "";
                sql = "delete from billingaddress where biladrsysnbr = " + addyID.ToString();
                _jurisUtility.ExecuteNonQuery(0, sql);

                isError = false;
            }
            catch (Exception exa) {  }

        }

        private void undoResp()
        {
            try
            {
                string sql = "";
                sql = "delete from ClientResponsibleTimekeeper where CRTClientID = " + clisysnbr.ToString();
                _jurisUtility.ExecuteNonQuery(0, sql);
                isError = false;
            }
            catch (Exception exr) { }

        }

        private void undoOrig()
        {
            try
            {
                string sql = "";
                sql = "delete from CliOrigAtty where COrigCli = " + clisysnbr.ToString();
                _jurisUtility.ExecuteNonQuery(0, sql);

                isError = false;
            }
            catch (Exception exo) { }
        }

        private bool addRespToTable(string empsys)
        {
            isError = false;
            DateTime nn = DateTime.Now;
            string pcName = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
            string sql = "insert into ClientResponsibleTimekeeper (CRTClientID, CRTEmployeeID, CRTPercent) values ( " + 
                   clisysnbr.ToString() + ", " + empsys + ", 100.0000 )" ;
           isError =  _jurisUtility.ExecuteNonQuery(0, sql);
            return isError;

        }

        private bool createConsolidation(string respAtty)
        {
            if (checkBoxConsolidation.Checked)
            {
                string sql = "insert into billto ([BillToSysNbr] ,[BillToCliNbr] ,[BillToUsageFlg] ,[BillToNickName] ,[BillToBillingAtty] ,[BillToBillFormat],[BillToEditFormat] ,[BillToRespAtty]) " +
                    "  values (case when (select max(BillToSysNbr) from billto) is null then 1 else ((select max(BillToSysNbr) from billto) + 1) end, " + clisysnbr.ToString() + ", 'C', '" + textBoxConsolidation.Text + "', " +
                    "(select empsysnbr from employee where empid = '" + this.comboBoxBT.GetItemText(this.comboBoxBT.SelectedItem).Split(' ')[0] + "'), '" + this.comboBoxBillLayout.GetItemText(this.comboBoxBillLayout.SelectedItem).Split(' ')[0] + "', " +
                    "'" + this.comboBoxPreBillLayout.GetItemText(this.comboBoxPreBillLayout.SelectedItem).Split(' ')[0] + "', " + respAtty + ")";
                isError = _jurisUtility.ExecuteNonQuery(0, sql); //did we enconter an error creating billto?
                if (!isError)
                {
                    sql = "select max(billtosysnbr) from billto where billtoclinbr = " + clisysnbr.ToString();
                    int billto = 0;
                    DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                    if (dds != null && dds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in dds.Tables[0].Rows)
                        {
                            billto = Convert.ToInt32(dr[0].ToString());
                        }

                    }
                    int addyid = 0;
                    sql = "select max(biladrsysnbr) from billingaddress where BilAdrCliNbr = " + clisysnbr.ToString();
                    dds.Clear();

                    dds = _jurisUtility.RecordsetFromSQL(sql);
                    if (dds != null && dds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in dds.Tables[0].Rows)
                            addyid = Convert.ToInt32(dr[0].ToString());

                    }


                    //billcopy
                    sql = "Insert into BillCopy(BilCpyBillTo,BilCpyBilAdr,BilCpyComment,BilCpyNbrOfCopies,BilCpyPrintFormat,BilCpyEmailFormat,BilCpyExportFormat,BilCpyARFormat) "
                    + " values ( " + billto.ToString() + ", " + addyid.ToString() + " ,'',1,1,0,0,0 )";

                    isError = _jurisUtility.ExecuteNonQuery(0, sql);
                    
                }
                return isError;
            }
            else
                return false;

        }

        private bool createAddy()
        {
            bool isError = false;
            DateTime nn = DateTime.Now;
            string pcName = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
            string addy = richTextBoxBAAddy.Text.Replace("\r", "|").Replace("\n", "|");
            addy = addy.Replace("||", "|");

            string sql = "Insert into BillingAddress(BilAdrSysNbr, BilAdrCliNbr, BilAdrUsageFlg, BilAdrNickName, BilAdrPhone, " +
                " BilAdrFax, BilAdrContact, BilAdrName, BilAdrAddress, BilAdrCity, BilAdrState, BilAdrZip, BilAdrCountry, BilAdrType, BilAdrEmail) " +
    " values (case when(select max(biladrsysnbr) from billingaddress) is null then 1 else ((select max(biladrsysnbr) from billingaddress) +1) end, " + clisysnbr.ToString() + ", " +
    " 'C', '" + textBoxBANName.Text + "', '" + textBoxBAPhoneOpt.Text + "', "
     + "  '" + textBoxBAFaxOpt.Text + "', '" + textBoxBAContactOpt.Text + "', " +
    " '" + textBoxBANameOpt.Text + "', " +
    "replace('" + addy + "', '|', char(13) + char(10)), "
    + " '" + textBoxBACityOpt.Text + "', '" + textBoxBAStateOpt.Text + "', '" + textBoxBAZipOpt.Text + "','" + textBoxBACountryOpt.Text + "', 0, '" + textBoxBAEmailOpt.Text + "')";

            isError = _jurisUtility.ExecuteNonQuery(0, sql);
            if (!isError)
            {
                int addyid = 0;
                sql = "select max(biladrsysnbr) from billingaddress where BilAdrCliNbr = " + clisysnbr.ToString(); ;
                DataSet dds = new DataSet();

                dds = _jurisUtility.RecordsetFromSQL(sql);
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                        addyid = Convert.ToInt32(dr[0].ToString());

                }

            }

            return isError;
        }


        private int getClisysnbr()
        {
            int sysnbr = 0;
            string sql = "select clisysnbr from client where clicode = '" + formatClientCode(textBoxCode.Text) + "'";
            DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
            if (dds != null && dds.Tables.Count > 0)
            {
                foreach (DataRow dr in dds.Tables[0].Rows)
                    sysnbr = Convert.ToInt32(dr[0].ToString());
            }
            return sysnbr;
        }

        private bool addOrig()
        {
            string sql = "";
                
                if (!textBoxOTPct1Opt.Text.Equals("0"))
                {
                    sql = "insert into CliOrigAtty (COrigCli, COrigAtty, COrigPcnt) values (" + clisysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT1.GetItemText(this.comboBoxOT1.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct1Opt.Text + " as decimal(7,4)))";
                    if (_jurisUtility.ExecuteNonQuery(0, sql))
                        return true;

                }
                if (!textBoxOTPct2Opt.Text.Equals("0"))
                {
                    sql = "insert into CliOrigAtty (COrigCli, COrigAtty, COrigPcnt) values (" + clisysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT2.GetItemText(this.comboBoxOT2.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct2Opt.Text + " as decimal(7,4)))";
                    if (_jurisUtility.ExecuteNonQuery(0, sql))
                        return true;

            }
                if (!textBoxOTPct3Opt.Text.Equals("0"))
                {
                    sql = "insert into CliOrigAtty (COrigCli, COrigAtty, COrigPcnt) values (" + clisysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT3.GetItemText(this.comboBoxOT3.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct3Opt.Text + " as decimal(7,4)))";
                    if (_jurisUtility.ExecuteNonQuery(0, sql))
                        return true;

            }
                if (!textBoxOTPct4Opt.Text.Equals("0"))
                {
                    sql = "insert into CliOrigAtty (COrigCli, COrigAtty, COrigPcnt) values (" + clisysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT4.GetItemText(this.comboBoxOT4.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct4Opt.Text + " as decimal(7,4)))";
                    if (_jurisUtility.ExecuteNonQuery(0, sql))
                        return true;

            }
                if (!textBoxOTPct5Opt.Text.Equals("0"))
                {
                    sql = "insert into CliOrigAtty (COrigCli, COrigAtty, COrigPcnt) values (" + clisysnbr.ToString() + ", (select empsysnbr from employee where empid = '" + this.comboBoxOT5.GetItemText(this.comboBoxOT5.SelectedItem).Split(' ')[0] + "'), cast(" + textBoxOTPct5Opt.Text + " as decimal(7,4)))";
                    if (_jurisUtility.ExecuteNonQuery(0, sql))
                        return true;

            }
            return false;

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
                MessageBox.Show("There are duplicate Originators. Originators with " + "\r\n" + "0 percent are not taken into consideration", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void saveInfoToMoveToMatter()
        {
            checkForTables();
            string sql = "insert into defaults (ID, name, userid, CreationDate, IsStandard, AllData ) " +
                " values (999999, 'MoveToMatter', " + empsysnbr.ToString() + ", getdate(), 'N', '')";

            _jurisUtility.ExecuteNonQuery(0, sql);


            foreach (var textbox in this.Controls.OfType<TextBox>())
            {
                if (!string.IsNullOrEmpty(textbox.Text) && !textbox.Name.Equals("textBoxCode"))
                {
                    sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType, empsys) values (999999, '" + textbox.Name + "', '" + textbox.Text + "', 'textBox', " + empsysnbr.ToString() + " )";
                    _jurisUtility.ExecuteNonQuery(0, sql);

                }
            }
            foreach (var cbox in this.Controls.OfType<ComboBox>())
            {
                sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType, empsys) values (999999, '" + cbox.Name + "', '" + cbox.GetItemText(cbox.SelectedItem) + "', 'comboBox', " + empsysnbr.ToString() + " )";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }

            foreach (var textbox in this.Controls.OfType<CheckBox>())
            {
                string isChecked = ((bool?)textbox.Checked) == true ? 'Y'.ToString() : 'N'.ToString();
                sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType, empsys) values (999999, '" + textbox.Name + "', '" + isChecked + "', 'checkBox', " + empsysnbr.ToString() + " )";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }

            foreach (var textbox in this.Controls.OfType<RichTextBox>())
            {
                sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType, empsys) values (999999, '" + textbox.Name + "', '" + textbox.Text + "', 'richTextBox', " + empsysnbr.ToString() + " )";
                _jurisUtility.ExecuteNonQuery(0, sql);
            }
        }

        private bool testOrigPct()
        {

            if (isNumeric(textBoxOTPct1Opt.Text) && isNumeric(textBoxOTPct2Opt.Text) && isNumeric(textBoxOTPct3Opt.Text) && isNumeric(textBoxOTPct4Opt.Text) && isNumeric(textBoxOTPct5Opt.Text) && (Convert.ToDecimal(textBoxOTPct1Opt.Text) + Convert.ToDecimal(textBoxOTPct2Opt.Text) + Convert.ToDecimal(textBoxOTPct3Opt.Text) + Convert.ToDecimal(textBoxOTPct4Opt.Text) + Convert.ToDecimal(textBoxOTPct5Opt.Text) == 100))
                if (checkForDupeOriginators())
                    return true;
                else
                    return false;
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

        private void textBoxNName_Leave(object sender, EventArgs e)
        {
            textBoxRName.Text = textBoxNName.Text;
        }

        private void getNextClientNumber()
        {

                string sql = "SELECT distinct number FROM master..spt_values " +
                            " WHERE number BETWEEN (SELECT min(cast(clicode as int)) from client) and (SELECT max(cast(clicode as int)) + 1 FROM client)  " +
                            " AND number NOT IN(SELECT cast(clicode as int) FROM client)";
                DataSet dds1 = _jurisUtility.RecordsetFromSQL(sql);
                string nextcode = "";
                if (dds1 != null && dds1.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds1.Tables[0].Rows)
                    {
                        if (codeIsNumeric || isNumeric(dr[0].ToString()))
                        {
                            nextcode = "000000000000" + dr[0].ToString().ToString();
                            nextcode = nextcode.Substring(nextcode.Length - lengthOfCode, lengthOfCode);
                            textBoxCode.Text = nextcode;
                            break;
                        }
                    }
                }

        }


        private bool loadClientBillFields()
        {
            if (clisysnbr != 0)
            {
                string sql = "select name, data from DefaultSettings where defaultid = 999998";
                DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        sql = "update client set " + dr[0].ToString() + " = replace('" + dr[1].ToString() + "', '|', char(13) + char(10)) where clisysnbr = " + clisysnbr.ToString();
                        if (_jurisUtility.ExecuteNonQuery(0, sql))
                            return true;
                    }
                } //else its not there so add it
                return false;
            } //we dont have a valid client so do nothing
            else
                return false;
        }

        private bool loadClientUDFFields()
        {
            if (clisysnbr != 0)
            {
                string sql = "select name, data, entrytype from DefaultSettings where defaultid = 999994 and empsys = " + empsysnbr.ToString();
                DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                if (dds != null && dds.Tables.Count > 0)
                {
                    foreach (DataRow dr in dds.Tables[0].Rows)
                    {
                        if (dr[1].ToString().Equals("int"))
                        sql = "update client set [" + dr[0].ToString() + "] = " + dr[1].ToString() + " where clisysnbr = " + clisysnbr.ToString();
                        else
                        sql = "update client set [" + dr[0].ToString() + "] = '" + dr[1].ToString() + "' where clisysnbr = " + clisysnbr.ToString();
                        if (_jurisUtility.ExecuteNonQuery(0, sql))
                        {
                            MessageBox.Show(_jurisUtility.errorMessage);
                            return true;
                        }
                    }
                } //else its not there so add it
                return false;
            } //we dont have a valid client so do nothing
            else
                return false;
        }

        private void textBoxCode_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxCode.Text))
            {
                if (textBoxCode.Text.Length > lengthOfCode)
                    MessageBox.Show("Client Code is longer than allowed." + "\r\n" + "Your settings allow for " + lengthOfCode.ToString() + " characters.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                else if (codeIsNumeric && !isNumeric(textBoxCode.Text))
                    MessageBox.Show("Client Code is not numeric. Your settings require a numeric code.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    string code = formatClientCode(textBoxCode.Text);
                    string sql = "select clisysnbr from client where dbo.jfn_FormatClientCode(clicode) = '" + code + "'";
                    DataSet dds = _jurisUtility.RecordsetFromSQL(sql);
                    if (dds != null && dds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in dds.Tables[0].Rows) //client already exists
                        {
                            MessageBox.Show("Client " + textBoxCode.Text + " already exists. Enter a valid client code." + "\r\n" + "Codes must match the format in which they appear in Juris." + "\r\n" + "This includes leading zeroes", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                }
            }

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Visible)
                this.Hide();
        }

        private void buttonAddNoteCard_Click(object sender, EventArgs e)
        {
            this.Hide();
            AddNoteCard adn = new AddNoteCard(pt, "Add Client Note Card");
            adn.ShowDialog();
            noteName = adn.name;
            noteText = adn.text;
            this.Show();
        }


        private void buttonCliBilling_Click(object sender, EventArgs e)
        {
            CliBillingForm cliB = new CliBillingForm(_jurisUtility, empsysnbr);
            if (cliB.loadFields())
                cliB.ShowDialog();
            else
                cliB.Close();
        }

        private void buttonCliUDF_Click(object sender, EventArgs e)
        {
            CliUDFFields cliB = new CliUDFFields(_jurisUtility, empsysnbr);
            if (cliB.loadFields())
                cliB.ShowDialog();
            else
                cliB.Close();
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
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

        public int MakeLong(short lowPart, short highPart) // to catch clicking the Red X to close
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }
    }
}
