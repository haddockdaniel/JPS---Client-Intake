using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gizmox.Controls;

namespace JurisUtilityBase
{
    public partial class CliUDFFields : Form
    {
        public CliUDFFields()
        {
            InitializeComponent();
        }

        public CliUDFFields(JurisUtility _JU, int empsys)
        {
            InitializeComponent();
            JU = _JU;
            empsysnbr = empsys;
        }

        JurisUtility JU;
        List<BillingField> bfList = new List<BillingField>();
        BillingField bf = null;
        int empsysnbr = 0;

        public bool loadFields()
        {

            string sysparam = " SELECT SpTxtValue, SpName FROM SysParam where spname like 'FldClientUDF%' and sptxtvalue not like 'C UDF%' ";

            DataSet dds2 = JU.RecordsetFromSQL(sysparam);
            if (dds2 != null && dds2.Tables.Count > 0)
            {
                int numOfFields = dds2.Tables[0].Rows.Count;
                if (numOfFields == 0)
                {
                    MessageBox.Show("There are no defined UDF Fields in your data.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //hide all items not defined

                foreach (var label in this.Controls.OfType<Label>())
                {
                    int num = Convert.ToInt32(label.Name.Replace("label", ""));
                    if (num > numOfFields)
                        label.Visible = false;

                }

                foreach (var cb in this.Controls.OfType<ComboBox>())
                {
                    int num = Convert.ToInt32(cb.Name.Replace("comboBox", ""));
                    if (num > numOfFields)
                        cb.Visible = false;

                }

                foreach (var tb in this.Controls.OfType<TextBox>())
                {
                    int num = Convert.ToInt32(tb.Name.Replace("textBox", ""));
                    if (num > numOfFields)
                        tb.Visible = false;

                }
                int rowNum = 1;
                foreach (DataRow dr in dds2.Tables[0].Rows)
                {
                    string[] test = dr[0].ToString().Split(',');

                    bf = new BillingField();
                    bf.delete = false;
                    bf.length = Convert.ToInt32(test[2].ToString());
                    bf.name = test[0].ToString().Replace(" ", "");
                    bf.UDFtype = test[1].ToString();
                    if (test[3].ToString().Equals("R") || test[3].ToString().Equals("N"))
                    {
                        bf.whichBox = "textBox" + rowNum.ToString();
                        bf.text = ""; // save for when they type text in
                        if (test[3].ToString().Equals("Y"))
                            bf.isRequired = true;
                        else
                            bf.isRequired = false;
                        
                    }
                    else
                    {
                        if (test[3].ToString().Equals("T")) //timekeeper
                        {
                            foreach (var cb in this.Controls.OfType<ComboBox>())
                            {
                                if (cb.Name.Equals("comboBox" + rowNum.ToString()))
                                {
                                    string SQLPC2 = "select empinitials,empid + '    ' + empname as emp from employee where empvalidastkpr='Y' order by empinitials, empid";
                                    DataSet myRSPC2 = JU.RecordsetFromSQL(SQLPC2);
                                    cb.Items.Add("Blank/None        ");
                                    if (myRSPC2.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow dd in myRSPC2.Tables[0].Rows)
                                            cb.Items.Add(dd["emp"].ToString());
                                        cb.SelectedIndex = 0;
                                    }
                                }
                            }
                        }
                        else if (test[3].ToString().Equals("P")) // practice class
                        {
                            foreach (var cb in this.Controls.OfType<ComboBox>())
                            {
                                if (cb.Name.Equals("comboBox" + rowNum.ToString()))
                                {
                                    string SQLPC2 = "select PrctClsCode  + '    ' + right(PrctClsDesc, 30) as PC from PracticeClass order by PrctClsCode";
                                    DataSet myRSPC2 = JU.RecordsetFromSQL(SQLPC2);
                                    cb.Items.Add("Blank/None        ");
                                    if (myRSPC2.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow dd in myRSPC2.Tables[0].Rows)
                                            cb.Items.Add(dd["PC"].ToString());
                                        cb.SelectedIndex = 0;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var label in this.Controls.OfType<Label>())
                    {
                        if (label.Name.Equals("label" + rowNum.ToString()))
                        {
                            label.Text = test[0].ToString();
                        }

                    }

                    bfList.Add(bf);
                    rowNum++;


                }
                return true;
            }
            else
            {
                MessageBox.Show("There are no defined UDF Fields in your data.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }




        }

        private void saveData()
        {

            string sql = "insert into defaults (ID, name, userid, CreationDate, IsStandard, AllData ) " +
        " values (999994, 'UDFClient', 'N', getdate(), " + empsysnbr.ToString() + ", '')";

            JU.ExecuteNonQuery(0, sql);


            foreach (BillingField bb in bfList)
            {
                if (!string.IsNullOrEmpty(bb.text) && !bb.delete)
                {
                    sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (999994, '" + bb.name.Replace(" ", "") + "', '" + bb.text + "', 'richTextBox' )";
                    JU.ExecuteNonQuery(0, sql);
                }
            }
        }


        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonAddData_Click(object sender, EventArgs e)
        {
            if (validateUDFTypes())
            {
                if (validateUDFRequired())
                {
                    if (verifyDropDowns())

                }


            }
            //this.comboBoxPC.GetItemText(this.comboBoxPC.SelectedItem).Split(' ')[0]
        }

        private bool validateUDFTypes()
        {
            foreach (BillingField bb in bfList)
            {
                foreach (var textbox in this.Controls.OfType<TextBox>())
                {
                    if (textbox.Name.Equals(bb.whichBox))
                    {
                        if (bb.UDFtype == "N" && !isNumber(textbox.Text))
                        {
                            MessageBox.Show("UDF Field " + bb.name + " is set to Numeric. Please ensure the data is numeric", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;

                        }
                        if (bb.UDFtype == "D" && !isDate(textbox.Text))
                        {
                            MessageBox.Show("UDF Field " + bb.name + " is set to Date. Please ensure the data is a valid date", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;

                        }
                    }
                }

            }
            return true;
        }

        private bool validateUDFRequired()
        {
            foreach (BillingField bb in bfList)
            {
                foreach (var textbox in this.Controls.OfType<TextBox>())
                {
                    if (textbox.Name.Equals(bb.whichBox) && bb.isRequired && string.IsNullOrEmpty(textbox.Text))
                    {
                        MessageBox.Show("UDF Field " + bb.name + " is set to Required. Please add data", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                }

            }
            return true;
        }

        private bool verifyDropDowns()
        {
            foreach (BillingField bb in bfList) // did they leave a drop down blank? If so, ignore it
            {



            }

                return true;
        }

        private bool isNumber(string test)
        {
            try
            {
                Decimal ff = Convert.ToDecimal(test);
                return true;
            }
            catch
            { return false; }
        }

        private bool isDate(string test)
        {
            try
            {
                DateTime ff = Convert.ToDateTime(test);
                return true;
            }
            catch
            { return false; }
        }
    }
}
