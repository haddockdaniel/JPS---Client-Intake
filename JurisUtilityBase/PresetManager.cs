﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using JDataEngine;
using JurisAuthenticator;
using JurisUtilityBase.Properties;
using System.Data.OleDb;
using Microsoft.Win32;
using JurisSVR.ExpenseAttachments;

namespace JurisUtilityBase
{
    public partial class PresetManager : Form
    {
        public PresetManager(DataSet ds, JurisUtility jutil, string CliMat)
        {
            InitializeComponent();
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].Width = 250;
            dataGridView1.Columns[2].Width = 75;
            dataGridView1.Columns[3].Width = 100;
            dataGridView1.Columns[4].Width = 60;
            _jurisUtility = jutil;
            ClientOrMatter = CliMat;
        }

        JurisUtility _jurisUtility;
        string ClientOrMatter = "";

        private void buttonBack_Click(object sender, EventArgs e)
        {
            ClientForm cleared = new ClientForm(_jurisUtility, 0, false);
            cleared.Show();
            this.Close();
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            int id = 0;
            if (dataGridView1.SelectedRows.Count == 0 || dataGridView1.SelectedRows.Count > 1)
                MessageBox.Show("One and only one Preset can be renamed at a time", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
                checkDefaultName(id);

            }


        }


        private void checkDefaultName(int ID)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("Enter the New Name", "Default Name", "New Default");
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
                {
                    sql = "update defaults set name = '" + name + "' where id = " + ID.ToString();
                    _jurisUtility.ExecuteSqlCommand(0, sql);
                    sql = "select ID, name as [Default Name], PopulateMatter as [Populate Matter],  convert(varchar,CreationDate, 101) as [Creation Date], isStandard as [Default] from Defaults where DefType = '" + ClientOrMatter + "'";
                    DataSet ds = _jurisUtility.RecordsetFromSQL(sql);
                    PresetManager DM = new PresetManager(ds, _jurisUtility, ClientOrMatter);
                    DM.Show();
                    this.Close();
                }
                else
                    MessageBox.Show("Names must be unique and that name already exists. Default not added", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("A valid name is required. Default not updated", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int id = 0;
            string sql = "";

            if (dataGridView1.SelectedRows.Count == 0)
                MessageBox.Show("At least one Preset must be selected", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                {
                    id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
                    sql = "delete from defaultsettings where defaultid = " + id.ToString();
                    _jurisUtility.ExecuteSqlCommand(0, sql);
                    sql = "delete from defaults where id = " + id.ToString();
                    _jurisUtility.ExecuteSqlCommand(0, sql);
                }
                sql = "select ID, name as [Default Name], PopulateMatter as [Populate Matter],  convert(varchar,CreationDate, 101) as [Creation Date], isStandard as [Default] from Defaults where DefType = '" + ClientOrMatter + "'";
                DataSet ds = _jurisUtility.RecordsetFromSQL(sql);
                PresetManager DM = new PresetManager(ds, _jurisUtility, ClientOrMatter);
                DM.Show();
                this.Close();
            }
            
        }

        private void buttonStandard_Click(object sender, EventArgs e)
        {
            string sql = "";
            int id = 0;
            if (dataGridView1.SelectedRows.Count == 0 || dataGridView1.SelectedRows.Count > 1)
                MessageBox.Show("One and only one Preset can be default at a time", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
                sql = "update defaults set IsStandard = 'N' where DefType = '" + ClientOrMatter + "'";
                _jurisUtility.ExecuteSqlCommand(0, sql);
                sql = "update defaults set IsStandard = 'Y' where id = " + id.ToString();
                _jurisUtility.ExecuteSqlCommand(0, sql);
                sql = "select ID, name as [Default Name], PopulateMatter as [Populate Matter],  convert(varchar,CreationDate, 101) as [Creation Date], isStandard as [Default] from Defaults where DefType = 'C'";
                DataSet ds = _jurisUtility.RecordsetFromSQL(sql);
                PresetManager DM = new PresetManager(ds, _jurisUtility, ClientOrMatter);
                DM.Show();
                this.Close();

            }



        }

        private void buttonModify_Click(object sender, EventArgs e)
        {
            int id = 0;
            if (dataGridView1.SelectedRows.Count == 0 || dataGridView1.SelectedRows.Count > 1)
                MessageBox.Show("One and only one Preset can be Modified at a time", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
                ClientForm cleared = new ClientForm(_jurisUtility, id, true);
                cleared.Show();
                this.Close();
            }





        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            int id = 0;
            if (dataGridView1.SelectedRows.Count == 0 || dataGridView1.SelectedRows.Count > 1)
                MessageBox.Show("One and only one Preset can be loaded at a time", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value);
                ClientForm cleared = new ClientForm(_jurisUtility, id, false);
                cleared.Show();
                this.Close();
            }
        }
    }
}
