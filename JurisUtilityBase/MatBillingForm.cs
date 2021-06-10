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
    public partial class MatBillingForm : Form
    {
        public MatBillingForm(JurisUtility _JU)
        {
            InitializeComponent();
            JU = _JU;
        }

        JurisUtility JU;
        List<BillingField> bfList = new List<BillingField>();
        BillingField bf = null;

        public bool loadFields()
        {
            //small = 383, 663
            //large = 765, 663
            string sysparam = "SELECT SpTxtValue, SpName FROM SysParam where spname like 'FldMatterBF%' and sptxtvalue not like 'Billing Field %'";
            DataSet dds2 = JU.RecordsetFromSQL(sysparam);
            if (dds2 != null && dds2.Tables.Count > 0)
            {
                int numOfFields = dds2.Tables[0].Rows.Count;
                if (numOfFields == 0)
                {
                    MessageBox.Show("There are no defined Matter Billing Fields in your data.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (numOfFields < 11)
                {
                    this.Size = new Size(383, 663);
                    buttonAddData.TabIndex = 11;
                    buttonCancel.TabIndex = 11;
                    richTextBox11.TabIndex = 30;
                }
                int rowNum = 1;
                foreach (DataRow dr in dds2.Tables[0].Rows)
                {
                    string[] test = dr[0].ToString().Split(',');

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
                    bf = new BillingField();
                    bf.length = Convert.ToInt32(test[2].ToString());
                    bf.name = "MatBillingField" + dr[1].ToString().Replace("FldMatterBF", "");
                    bf.whichBox = "richTextBox" + rowNum.ToString();
                    bf.text = ""; // save for when they type text in
                    bfList.Add(bf);
                    rowNum++;


                }
                
                return true;
            }
            else
            {
                MessageBox.Show("There are no defined Matter Billing Fields in your data.", "Form Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        " values (999997, 'BFMatter', 'N', getdate(), 'N', 'R')";

            JU.ExecuteNonQuery(0, sql);


            foreach (BillingField bb in bfList)
            {
                if (!string.IsNullOrEmpty(bb.text))
                {
                    sql = "insert into DefaultSettings (DefaultID, [name], [data], entryType) values (999997, '" + bb.name + "', '" + bb.text + "', 'richTextBox' )";
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
            getData();
            saveData();
            this.Close();
        }

    }
}
