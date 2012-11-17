using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using driver.Config;
using driver.Lib;
//using mdcore;
//using mdcore.Config;

namespace PayDesk.Components.UI
{
    public partial class uiWndUnitFilter : Form
    {
        private DataTable dTable;

        public uiWndUnitFilter(DataTable dTable)
        {
            InitializeComponent();
            this.dTable = dTable;

            try
            {
                for (byte i = 0; i < ((string[])ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[0]).Length; i++)
                {
                    checkedListBox1.Items.Add(
                        ((string[])ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[0])[i],
                        ((bool[])ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[1])[i]);
                    // use scale product weight for this item unit.
                    checkedListBox2.Items.Add("", ((bool[])ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[2])[i]);

                }
            }
            catch
            {
                checkedListBox1.Items.Clear();
                checkedListBox2.Items.Clear();
            }
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
            checkedListBox2.Items.Clear();

            for (int i = 0; i < dTable.Rows.Count; i++)
                if (checkedListBox1.Items.IndexOf(dTable.Rows[i]["UNIT"].ToString().ToLower().Trim()) == -1)
                {
                    checkedListBox1.Items.Add(dTable.Rows[i]["UNIT"].ToString().ToLower().Trim());
                    checkedListBox2.Items.Add("");
                }
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.Instance.CommonConfiguration.APP_UnitFilter = new object[3] { 
                    new string[checkedListBox1.Items.Count], 
                    new bool[checkedListBox1.Items.Count], 
                    new bool[checkedListBox1.Items.Count] };

                using (StreamWriter sw = File.CreateText(ConfigManager.Instance.CommonConfiguration.Path_Units))
                {
                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        sw.Write(checkedListBox1.Items[i].ToString() == "" ? " " : checkedListBox1.Items[i]);
                        sw.Write("_");
                        sw.WriteLine(checkedListBox1.GetItemChecked(i) ? "1" : "0");
                        sw.WriteLine("_");
                        sw.WriteLine(checkedListBox2.GetItemChecked(i) ? "1" : "0");
                        //Save to config
                        ((string[])ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[0])[i] = checkedListBox1.Items[i].ToString();
                        ((bool[])ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[1])[i] = checkedListBox1.GetItemChecked(i);
                        ((bool[])ConfigManager.Instance.CommonConfiguration.APP_UnitFilter[2])[i] = checkedListBox2.GetItemChecked(i);
                    }
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (Exception ex)
            {
                CoreLib.WriteLog(ex, "PayDesk.Components.UI.uiWndUnitFilter.saveButton_Click");

                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox2.SelectedIndex = checkedListBox1.SelectedIndex;
        }
    }
}