using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using mdcore;

namespace PayDesk
{
    public partial class UnitFilter : Form
    {
        private DataTable dTable;

        public UnitFilter(DataTable dTable)
        {
            InitializeComponent();
            this.dTable = dTable;

            try
            {
                for (byte i = 0; i < ((string[])AppConfig.APP_UnitFilter[0]).Length; i++)
                    checkedListBox1.Items.Add(
                        ((string[])AppConfig.APP_UnitFilter[0])[i],
                        ((bool[])AppConfig.APP_UnitFilter[1])[i]);
            }
            catch { }
        }

        private void Filter_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                Close();
                return;
            }
        }
        private void updateValuesButton_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();

            for (int i = 0; i < dTable.Rows.Count; i++)
                if (checkedListBox1.Items.IndexOf(dTable.Rows[i]["UNIT"].ToString().ToLower().Trim()) == -1)
                    checkedListBox1.Items.Add(dTable.Rows[i]["UNIT"].ToString().ToLower().Trim());
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                AppConfig.APP_UnitFilter = new object[2] { 
                    new string[checkedListBox1.Items.Count], 
                    new bool[checkedListBox1.Items.Count] };

                using (StreamWriter sw = File.CreateText(AppConfig.Path_Units))
                {
                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        sw.Write(checkedListBox1.Items[i].ToString() == "" ? " " : checkedListBox1.Items[i]);
                        sw.Write("_");
                        sw.WriteLine(checkedListBox1.GetItemChecked(i) ? "1" : "0");
                        //Save to config
                        ((string[])AppConfig.APP_UnitFilter[0])[i] = checkedListBox1.Items[i].ToString();
                        ((bool[])AppConfig.APP_UnitFilter[1])[i] = checkedListBox1.GetItemChecked(i);
                    }
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch { return; }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}