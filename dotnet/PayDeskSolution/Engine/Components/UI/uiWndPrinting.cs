using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//0using mdcore;
using System.IO;
using driver.Config;
using driver.Components.UI;
using components.Components.MMessageBox;
//0using mdcore.Config;
//0using mdcore.Components.UI;

namespace PayDesk.Components.UI
{
    public partial class uiWndPrinting : Form
    {

        private int loadedRowsCount;
        public uiWndPrinting()
        {
            InitializeComponent();
        }

        // for print: name is key
        // printer exe file
        // binded template
        // printer type (used in program)

        // startup hook
        private void Printing_Load(object sender, EventArgs e)
        {
            //checkBox_useCommonPrinting.Checked = ConfigManager.Instance.CommonConfiguration.APP_UseCommonPrinting;

            // Printers
            /*if (ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks != null && ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[0] != null)
                for (int i = 0; i < ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[0].Length; i++)
                {
                    try
                    {
                        object valItem = ((DataGridViewComboBoxColumn)printersGrid.Columns["ColumnPrnType"]).Items[int.Parse(ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[1][i])];
                        printersGrid.Rows.Add(new object[] { ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[0][i], valItem, ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[2][i], "Видалити" });
                    }
                    catch (Exception ex) { .CoreLib.WriteLog(ex, "Printing_Load"); }
                }*/


            if (ConfigManager.Instance.CommonConfiguration.Path_Printers != null && ConfigManager.Instance.CommonConfiguration.Path_Printers.Count != 0)
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> prnItem in ConfigManager.Instance.CommonConfiguration.Path_Printers)
                {
                    try
                    {
                        printersGrid.Rows.Add(new object[] { prnItem.Key, prnItem.Value["PRN"], prnItem.Value["TPL"] });
                        controlGrid.Rows.Add(new object[] { "Видалити", "...", prnItem.Value["ACTIVE"], this.GetPrnValueByIndex(prnItem.Value["TYPE"]) });
                    
                    }
                    catch { }
                }


            }/*
            this.RefreshPrinterList();
            //Templates
            textBox1.Text = ConfigManager.Instance.CommonConfiguration.Path_Tpl_1;
            textBox2.Text = ConfigManager.Instance.CommonConfiguration.Path_Tpl_2;
            textBox3.Text = ConfigManager.Instance.CommonConfiguration.Path_Tpl_3;*/
            this.loadedRowsCount = this.printersGrid.Rows.Count;
        }

        // key handler
        private void Printing_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                Close();
                return;
            }
        }

        // add new printer
        private void button_addPrinter_Click(object sender, EventArgs e)
        {
            this.AddOrUpdateRow();
        }
        
        // save and close window
        private void button_SaveAndClose_Click(object sender, EventArgs e)
        {
            this.PrintListSaveUI();
            DialogResult = DialogResult.OK;
        }

        // save only
        private void button_savePrnOnly_Click(object sender, EventArgs e)
        {
            this.PrintListSaveUI();
        }

        // control grid
        private void controlGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 0:
                    {
                        this.DeletePrnRow(e.RowIndex);
                        break;
                    }
                case 1:
                    {
                        this.AddOrUpdateRow(e.RowIndex);
                        break;
                    }
            }
        }

        /* Methods */

        // save
        // return -1 = double prnter's name
        private int PrnListSave()
        {
            if (this.loadedRowsCount != 0 && this.printersGrid.Rows.Count == 0)
            {
                DialogResult delRez = MMessageBox.Show(this, "Ви дійсно бажаєте видалити всі принтери", Application.ProductName,
    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (delRez != System.Windows.Forms.DialogResult.Yes)
                    return 0;
            }

            int retErr = 0;
            string prnName = string.Empty;
            ConfigManager.Instance.CommonConfiguration.Path_Printers.Clear();
            for (int i = 0; i < printersGrid.RowCount; i++)
            {
                prnName = printersGrid["ColumnPrnName", i].Value.ToString();
                if (prnName == string.Empty)
                    return -2;

                Dictionary<string, string> prnItem = new Dictionary<string, string>();
                prnItem.Add("PRN", string.Format("{0}", printersGrid["ColumnPrnPath", i].Value));
                prnItem.Add("TYPE", this.GetPrnTypeIndex(controlGrid["ColumnCtrlType", i].Value.ToString()).ToString());
                prnItem.Add("TPL", string.Format("{0}", printersGrid["ColumnPrnTpl", i].Value.ToString()));
                prnItem.Add("ACTIVE", string.Format("{0}", ((DataGridViewCheckBoxCell)controlGrid["ColumnCtrlActive", i]).FormattedValue.ToString()));

                if (!ConfigManager.Instance.CommonConfiguration.Path_Printers.ContainsKey(prnName))
                {
                    ConfigManager.Instance.CommonConfiguration.Path_Printers.Add(prnName, prnItem); 
                }
                else
                {
                    ConfigManager.Instance.CommonConfiguration.Path_Printers[prnName] = prnItem;
                    //retErr = -1;
                }
            }



            /*

            ConfigManager.Instance.CommonConfiguration.Path_PrinterTempleates.Clear();
            for (int i = 0; i < printersGrid.RowCount; i++)
            {
                if (!ConfigManager.Instance.CommonConfiguration.Path_PrinterTempleates.ContainsKey(printersGrid["ColumnPrnName", i].Value.ToString()))
                    ConfigManager.Instance.CommonConfiguration.Path_PrinterTempleates[printersGrid["ColumnPrnName", i].Value.ToString()] = string.Empty;
                else
                    return -1;
            }

            ConfigManager.Instance.CommonConfiguration.APP_UseCommonPrinting = checkBox1.Checked;

            if (ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks == null)
                ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks = new string[3][];
            ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[0] = new string[printersGrid.RowCount];
            ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[1] = new string[printersGrid.RowCount];
            ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[2] = new string[printersGrid.RowCount];

            for (int i = 0; i < printersGrid.RowCount; i++)
            {
                ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[0][i] = printersGrid["ColumnPrnPath", i].Value.ToString();
                object valIndex = ((DataGridViewComboBoxCell)printersGrid["ColumnPrnType", i]).Value;
                ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[1][i] = ((DataGridViewComboBoxCell)printersGrid["ColumnPrnType", i]).Items.IndexOf(valIndex).ToString();
                ConfigManager.Instance.CommonConfiguration.APP_PrintersLinks[2][i] = printersGrid["ColumnPrnName", i].Value.ToString();


            }

            ConfigManager.Instance.CommonConfiguration.Path_Tpl_1 = textBox1.Text;
            ConfigManager.Instance.CommonConfiguration.Path_Tpl_2 = textBox2.Text;
            ConfigManager.Instance.CommonConfiguration.Path_Tpl_3 = textBox3.Text;
            */
            /*
            if (Program.MainArgs.ContainsKey("-c") && Program.MainArgs["-c"] != null)
                ConfigManager.Instance.CommonConfiguration.SaveData(Program.MainArgs["-c"].ToString());
            else*/
            ConfigManager.SaveConfiguration();

            return retErr;
        }
        private void PrintListSaveUI()
        {
            try
            {
                int r = this.PrnListSave();
                if (r < 0)
                {
                    if (r == -1)
                        MMessageBox.Show(this, "В списку є принтери з однаковою назвою", Application.ProductName,
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    if (r == -2)
                        MMessageBox.Show(this, "В списку є принтери без назви", Application.ProductName,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }
        private int GetPrnTypeIndex(string value)
        {
            int idx = 0;
            try
            {
                idx = ((DataGridViewComboBoxColumn)controlGrid.Columns["ColumnCtrlType"]).Items.IndexOf(value);
            }
            catch { }

            return idx;
        }
        private string GetPrnValueByIndex(string index)
        {
            string item = string.Empty;
            try
            {
                item = ((DataGridViewComboBoxColumn)controlGrid.Columns["ColumnCtrlType"]).Items[int.Parse(index)].ToString();
            }
            catch { }

            return item;
        }
        private string GetPrnValueByIndex() { return this.GetPrnValueByIndex("0"); }
        private void AddOrUpdateRow(int RowIndex)
        {
            string[] prnPaths = new string[2];
            DialogResult prnPath = openFileDialog2.ShowDialog(this);
            if (prnPath == DialogResult.OK)
                prnPaths[0] = openFileDialog2.FileName;
            DialogResult tplPath = openFileDialog1.ShowDialog(this);
            if (tplPath == DialogResult.OK)
                prnPaths[1] = openFileDialog1.FileName;

            if (RowIndex >= 0)
            {
                if (prnPath == DialogResult.OK)
                    printersGrid["ColumnPrnPath", RowIndex].Value = prnPaths[0];
                if (tplPath == DialogResult.OK)
                    printersGrid["ColumnPrnTpl", RowIndex].Value = prnPaths[1];
            }
            else
            {
                //printersGrid.Rows.Add(new object[] { "Видалити", prnPaths[0], this.GetPrnValueByIndex(), "принтер 1", prnPaths[1], "..." });



                printersGrid.Rows.Add(new object[] { "принтер " + (printersGrid.RowCount + 1), prnPaths[0], prnPaths[1] });
                controlGrid.Rows.Add(new object[] { "Видалити", "...", true, this.GetPrnValueByIndex() });
            }
        }
        private void AddOrUpdateRow() { this.AddOrUpdateRow(-1); }
        private void DeletePrnRow(int RowIndex)
        {
            string pName = printersGrid["ColumnPrnName", RowIndex].Value.ToString();
            string pType = controlGrid["ColumnCtrlType", RowIndex].Value.ToString();
            DialogResult delRez = MMessageBox.Show(this, "Видалити принтер \"" + pType + " " + pName + "\"", Application.ProductName,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (delRez == DialogResult.Yes)
            {
                printersGrid.Rows.RemoveAt(RowIndex);
                controlGrid.Rows.RemoveAt(RowIndex);
                this.PrnListSave();
            }
        }
        private void RefreshPrinterList()
        {/*
            this.listBox_printers.Items.Clear();
            for (int i = 0; i < printersGrid.RowCount; i++)
            {
                string prnName = printersGrid["ColumnPrnName", i].Value.ToString();
                string prnType = printersGrid["ColumnPrnType", i].Value.ToString();

                this.listBox_printers.Items.Add(prnName + " - " + prnType);
            }
            this.groupBox1.Text = this.groupBox1.Tag.ToString();
            if (this.listBox_printers.Items.Count > 0)
                this.listBox_printers.SelectedIndex = 0;*/
        }

        /* old
        private void PathSelection_Click(object sender, EventArgs e)
        {
            byte idx = byte.Parse(((Button)sender).Tag.ToString());
            openFileDialog1.Title = tabControl1.TabPages[1].Controls["label" + idx].Text;
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                tabControl1.TabPages[1].Controls["textBox" + idx].Text = openFileDialog1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = listBox_printers.SelectedItem.ToString();
            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                this.textBox_currentTemplate.Text = openFileDialog1.FileName;
        }

        private void listBox_printers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ConfigManager.Instance.CommonConfiguration.Path_PrinterTempleates.ContainsKey(listBox_printers.SelectedItem.ToString()))
            {
                this.groupBox1.Text = this.groupBox1.Tag.ToString() + " " + listBox_printers.SelectedItem;
                this.textBox_currentTemplate.Text = ConfigManager.Instance.CommonConfiguration.Path_PrinterTempleates[listBox_printers.SelectedItem.ToString()];
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (ConfigManager.Instance.CommonConfiguration.Path_PrinterTempleates.ContainsKey(listBox_printers.SelectedItem.ToString()))
            {
                ConfigManager.Instance.CommonConfiguration.Path_PrinterTempleates[listBox_printers.SelectedItem.ToString()] = this.textBox_currentTemplate.Text;
            }
        }


        */


    }
}