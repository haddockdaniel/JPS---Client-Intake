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
    public partial class CliBillingForm : Form
    {
        public CliBillingForm(JurisUtility _JU)
        {
            InitializeComponent();
            JU = _JU;
        }

        JurisUtility JU;
        List<BillingField> bfList = new List<BillingField>();
        BillingField bf = null;

        public bool loadFields()
        {
            //small = 395, 663
            //large = 765, 663
            //x large = 1153, 663
            string sysparam = " SELECT SpTxtValue, SpName FROM SysParam where spname like 'FldClientUDF%' and sptxtvalue not like 'C UDF%' " + 
                                  " union all " +
                                  "SELECT SpTxtValue, SpName FROM SysParam where spname like 'FldClientBF%' and sptxtvalue not like 'Billing Field %'";
            
            DataSet dds2 = JU.RecordsetFromSQL(sysparam);
            if (dds2 != null && dds2.Tables.Count > 0)
            {
                int numOfFields = dds2.Tables[0].Rows.Count;
                if  (numOfFields == 0)
                {
                    MessageBox.Show("There are no defined Billing or UDF Fields in your data.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (numOfFields < 11)
                {
                    this.Size = new Size(395, 663);
                    buttonAddData.TabIndex = numOfFields + 1;
                    buttonCancel.TabIndex = numOfFields + 2;
                    richTextBox11.TabIndex = 250;
                    richTextBox12.TabIndex = 251;
                }
                else if (numOfFields < 21)
                {
                    this.Size = new Size(765, 663);
                    buttonAddData.TabIndex = numOfFields + 1;
                    buttonCancel.TabIndex = numOfFields + 2;
                    richTextBox21.TabIndex = 250;
                    richTextBox22.TabIndex = 251;
                }
                int rowNum = 1;
                foreach (DataRow dr in dds2.Tables[0].Rows)
                {
                    string[] test = dr[0].ToString().Split(',');
                    string  fieldType = dr[1].ToString();

                    bf = new BillingField();
                    if (dr[1].ToString().Contains("FldClientBF")) //billing field
                    {
                        bf.length = Convert.ToInt32(test[2].ToString());
                        bf.name = "CliBillingField" + dr[1].ToString().Replace("FldClientBF", "");
                        bf.whichBox = "richTextBox" + rowNum.ToString();
                        bf.text = ""; // save for when they type text in
                        bf.isRequired = false;
                        bf.UDFtype = "";
                    }
                    else //UDF
                    {
                        bf.length = Convert.ToInt32(test[2].ToString());
                        bf.name = test[0].ToString().Replace(" ", "");
                        bf.whichBox = "richTextBox" + rowNum.ToString();
                        bf.text = ""; // save for when they type text in
                        if (test[3].ToString().Equals("Y"))
                            bf.isRequired = true;
                        else
                            bf.isRequired = false;
                        bf.UDFtype = test[1].ToString();
                    }



                    foreach (var label in this.Controls.OfType<Label>())
                    {
                        if (label.Name.Equals("label" + rowNum.ToString()))
                        {
                            label.Text = test[0].ToString();
                            label.Visible = true;
                        }
                            
                    }
                     foreach (var textbox in this.Controls.OfType<RichTextBox>())
                     {
                        if (textbox.Name.Equals("richTextBox" + rowNum.ToString()))
                        {
                            textbox.MaxLength = Convert.ToInt32(test[2].ToString());
                            textbox.Visible = true;
                        }
                     }

                    bfList.Add(bf);
                    rowNum++;


                }
                return true;
            }
            else
            {
                MessageBox.Show("There are no defined Billing or UDF Fields in your data.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }




        }

        public void getData()
        {
            foreach (var textbox in this.Controls.OfType<RichTextBox>())
            {
                if (!string.IsNullOrEmpty(textbox.Text))
                {
                        textbox.Text = textbox.Text.Replace("'", "").Replace("\"", "").Replace(@"\", " ").Replace("%", "").Replace("[", "").Replace("]", "").Replace("_", " ").Replace("^", "");
                    textbox.Text = textbox.Text.Replace("\r", "|").Replace("\n", "|");
                    textbox.Text = textbox.Text.Replace("||", "|");
                    foreach (BillingField bb in bfList)
                    {
                        if (bb.whichBox.Equals(textbox.Name))
                        {
                            bb.text = textbox.Text;
                            break;
                        }
                    }
                }
                
            }
        }

        private void saveData()
        {

            string sql = "insert into defaults (ID, name, PopulateMatter, CreationDate, IsStandard, DefType ) " +
        " values (999998, 'BFClient', 'N', getdate(), 'N', 'R')";

            JU.ExecuteNonQuery(0, sql);


            foreach (BillingField bb in bfList)
            {
                if (!string.IsNullOrEmpty(bb.text))
                {
                    sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (999998, '" + bb.name.Replace(" ", "") + "', '" + bb.text + "', 'richTextBox' )";
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
                    getData();
                    saveData();
                    this.Close();
                }
            }
        }

        private bool validateUDFTypes()
        {
            foreach (BillingField bb in bfList)
            {
                foreach (var textbox in this.Controls.OfType<RichTextBox>())
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
                foreach (var textbox in this.Controls.OfType<RichTextBox>())
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
