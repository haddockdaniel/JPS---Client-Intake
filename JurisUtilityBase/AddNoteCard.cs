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
    public partial class AddNoteCard : Form
    {
        public AddNoteCard(System.Drawing.Point ppt, string formName)
        {
            InitializeComponent();
            this.Location = ppt;
            this.Text = formName;
        }

        public string name = "";
        public string text = "";

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            name = "";
            text = "";
            this.Hide();
        }

        private void buttonAddData_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxName.Text) && !string.IsNullOrEmpty(richTextBoxText.Text))
            {
                textBoxName.Text = textBoxName.Text.Replace("'", "").Replace("\"", "").Replace(@"\", " ").Replace("%", "").Replace("[", "").Replace("]", "").Replace("_", " ").Replace("^", "");
                name = textBoxName.Text;
                richTextBoxText.Text = richTextBoxText.Text.Replace("'", "").Replace("\"", "").Replace(@"\", " ").Replace("%", "").Replace("[", "").Replace("]", "").Replace("_", " ").Replace("^", "");
                richTextBoxText.Text = richTextBoxText.Text.Replace("\r", "|").Replace("\n", "|");
                richTextBoxText.Text = richTextBoxText.Text.Replace("||", "|");
                text = richTextBoxText.Text;
                this.Hide();
            }
            else
                MessageBox.Show("Both Name and Text are required", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
}
