namespace JurisUtilityBase
{
    partial class UtilityBaseMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UtilityBaseMain));
            this.JurisLogoImageBox = new System.Windows.Forms.PictureBox();
            this.LexisNexisLogoPictureBox = new System.Windows.Forms.PictureBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.listBoxCompanies = new System.Windows.Forms.ListBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.OpenFileDialogOpen = new System.Windows.Forms.OpenFileDialog();
            this.buttonReport = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonMatOnly = new System.Windows.Forms.RadioButton();
            this.radioButtonCliOnly = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.JurisLogoImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LexisNexisLogoPictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // JurisLogoImageBox
            // 
            this.JurisLogoImageBox.Image = ((System.Drawing.Image)(resources.GetObject("JurisLogoImageBox.Image")));
            this.JurisLogoImageBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("JurisLogoImageBox.InitialImage")));
            this.JurisLogoImageBox.Location = new System.Drawing.Point(0, 1);
            this.JurisLogoImageBox.Name = "JurisLogoImageBox";
            this.JurisLogoImageBox.Size = new System.Drawing.Size(104, 336);
            this.JurisLogoImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.JurisLogoImageBox.TabIndex = 0;
            this.JurisLogoImageBox.TabStop = false;
            // 
            // LexisNexisLogoPictureBox
            // 
            this.LexisNexisLogoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("LexisNexisLogoPictureBox.Image")));
            this.LexisNexisLogoPictureBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("LexisNexisLogoPictureBox.InitialImage")));
            this.LexisNexisLogoPictureBox.Location = new System.Drawing.Point(8, 343);
            this.LexisNexisLogoPictureBox.Name = "LexisNexisLogoPictureBox";
            this.LexisNexisLogoPictureBox.Size = new System.Drawing.Size(96, 28);
            this.LexisNexisLogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.LexisNexisLogoPictureBox.TabIndex = 1;
            this.LexisNexisLogoPictureBox.TabStop = false;
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 374);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(492, 22);
            this.statusStrip.TabIndex = 2;
            // 
            // listBoxCompanies
            // 
            this.listBoxCompanies.FormattingEnabled = true;
            this.listBoxCompanies.Location = new System.Drawing.Point(111, 1);
            this.listBoxCompanies.Name = "listBoxCompanies";
            this.listBoxCompanies.Size = new System.Drawing.Size(185, 69);
            this.listBoxCompanies.TabIndex = 0;
            this.listBoxCompanies.SelectedIndexChanged += new System.EventHandler(this.listBoxCompanies_SelectedIndexChanged);
            // 
            // labelDescription
            // 
            this.labelDescription.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelDescription.Location = new System.Drawing.Point(302, 1);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(174, 69);
            this.labelDescription.TabIndex = 1;
            this.labelDescription.Text = "Imports new clients and matters based on User\'s entries";
            // 
            // OpenFileDialogOpen
            // 
            this.OpenFileDialogOpen.FileName = "openFileDialog1";
            // 
            // buttonReport
            // 
            this.buttonReport.BackColor = System.Drawing.Color.LightGray;
            this.buttonReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonReport.ForeColor = System.Drawing.Color.MidnightBlue;
            this.buttonReport.Location = new System.Drawing.Point(124, 299);
            this.buttonReport.Name = "buttonReport";
            this.buttonReport.Size = new System.Drawing.Size(105, 38);
            this.buttonReport.TabIndex = 16;
            this.buttonReport.Text = "Exit";
            this.buttonReport.UseVisualStyleBackColor = false;
            this.buttonReport.Click += new System.EventHandler(this.buttonReport_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LightGray;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.button1.Location = new System.Drawing.Point(330, 299);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 38);
            this.button1.TabIndex = 17;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.BackColor = System.Drawing.Color.LightGray;
            this.buttonBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowse.ForeColor = System.Drawing.Color.MidnightBlue;
            this.buttonBrowse.Location = new System.Drawing.Point(175, 188);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(197, 38);
            this.buttonBrowse.TabIndex = 19;
            this.buttonBrowse.Text = "Browse for Excel File";
            this.buttonBrowse.UseVisualStyleBackColor = false;
            this.buttonBrowse.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonMatOnly);
            this.groupBox1.Controls.Add(this.radioButtonCliOnly);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.groupBox1.Location = new System.Drawing.Point(175, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 75);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // radioButtonMatOnly
            // 
            this.radioButtonMatOnly.AutoSize = true;
            this.radioButtonMatOnly.Location = new System.Drawing.Point(6, 47);
            this.radioButtonMatOnly.Name = "radioButtonMatOnly";
            this.radioButtonMatOnly.Size = new System.Drawing.Size(116, 20);
            this.radioButtonMatOnly.TabIndex = 23;
            this.radioButtonMatOnly.Text = "Import Matter";
            this.radioButtonMatOnly.UseVisualStyleBackColor = true;
            // 
            // radioButtonCliOnly
            // 
            this.radioButtonCliOnly.AutoSize = true;
            this.radioButtonCliOnly.Checked = true;
            this.radioButtonCliOnly.Location = new System.Drawing.Point(6, 21);
            this.radioButtonCliOnly.Name = "radioButtonCliOnly";
            this.radioButtonCliOnly.Size = new System.Drawing.Size(112, 20);
            this.radioButtonCliOnly.TabIndex = 22;
            this.radioButtonCliOnly.TabStop = true;
            this.radioButtonCliOnly.Text = "Import Client";
            this.radioButtonCliOnly.UseVisualStyleBackColor = true;
            // 
            // UtilityBaseMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(492, 396);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonReport);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.listBoxCompanies);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.LexisNexisLogoPictureBox);
            this.Controls.Add(this.JurisLogoImageBox);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UtilityBaseMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client Matter Intake";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.JurisLogoImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LexisNexisLogoPictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox JurisLogoImageBox;
        private System.Windows.Forms.PictureBox LexisNexisLogoPictureBox;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ListBox listBoxCompanies;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.OpenFileDialog OpenFileDialogOpen;
        private System.Windows.Forms.Button buttonReport;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonMatOnly;
        private System.Windows.Forms.RadioButton radioButtonCliOnly;
    }
}

