
namespace JurisUtilityBase
{
    partial class ClientLookUp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientLookUp));
            this.radioButtonByName = new System.Windows.Forms.RadioButton();
            this.radioButtonByCliNum = new System.Windows.Forms.RadioButton();
            this.textBoxClient = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonCreateClient = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButtonByName
            // 
            this.radioButtonByName.AutoSize = true;
            this.radioButtonByName.Checked = true;
            this.radioButtonByName.Location = new System.Drawing.Point(28, 32);
            this.radioButtonByName.Name = "radioButtonByName";
            this.radioButtonByName.Size = new System.Drawing.Size(104, 17);
            this.radioButtonByName.TabIndex = 0;
            this.radioButtonByName.TabStop = true;
            this.radioButtonByName.Text = "Search by Name";
            this.radioButtonByName.UseVisualStyleBackColor = true;
            // 
            // radioButtonByCliNum
            // 
            this.radioButtonByCliNum.AutoSize = true;
            this.radioButtonByCliNum.Location = new System.Drawing.Point(160, 32);
            this.radioButtonByCliNum.Name = "radioButtonByCliNum";
            this.radioButtonByCliNum.Size = new System.Drawing.Size(113, 17);
            this.radioButtonByCliNum.TabIndex = 1;
            this.radioButtonByCliNum.Text = "Search by Number";
            this.radioButtonByCliNum.UseVisualStyleBackColor = true;
            // 
            // textBoxClient
            // 
            this.textBoxClient.Location = new System.Drawing.Point(28, 72);
            this.textBoxClient.Name = "textBoxClient";
            this.textBoxClient.Size = new System.Drawing.Size(245, 20);
            this.textBoxClient.TabIndex = 2;
            this.textBoxClient.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(28, 108);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(245, 150);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // buttonCreateClient
            // 
            this.buttonCreateClient.BackColor = System.Drawing.Color.LightGray;
            this.buttonCreateClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCreateClient.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonCreateClient.Location = new System.Drawing.Point(28, 295);
            this.buttonCreateClient.Name = "buttonCreateClient";
            this.buttonCreateClient.Size = new System.Drawing.Size(90, 36);
            this.buttonCreateClient.TabIndex = 58;
            this.buttonCreateClient.Text = "Next";
            this.buttonCreateClient.UseVisualStyleBackColor = false;
            this.buttonCreateClient.Click += new System.EventHandler(this.buttonCreateClient_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.BackColor = System.Drawing.Color.LightGray;
            this.buttonExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonExit.Location = new System.Drawing.Point(180, 295);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(93, 36);
            this.buttonExit.TabIndex = 59;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = false;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 265);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(247, 13);
            this.label1.TabIndex = 60;
            this.label1.Text = "* Double click one client and click Next to proceed";
            // 
            // ClientLookUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 348);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCreateClient);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.textBoxClient);
            this.Controls.Add(this.radioButtonByCliNum);
            this.Controls.Add(this.radioButtonByName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ClientLookUp";
            this.Text = "Client Look Up";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonByName;
        private System.Windows.Forms.RadioButton radioButtonByCliNum;
        private System.Windows.Forms.TextBox textBoxClient;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonCreateClient;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Label label1;
    }
}