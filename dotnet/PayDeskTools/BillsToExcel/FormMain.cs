using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using components.Components.ExcelDataWorker;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Configuration;
using components.Public;

namespace BillsToExcel
{
    public partial class FormMain : Form
    {
        private List<string> billFiles = new List<string>();
        private List<Hashtable> billData = new List<Hashtable>();
        private const int FIELD_KEY_NAME = 1;
        private const int FIELD_KEY_TITLE = 2;
        // private components.Components.XmlDocumentParser.Com_XmlDocumentParser xmlParser = new components.Components.XmlDocumentParser.Com_XmlDocumentParser();

        public FormMain()
        {
            InitializeComponent();
            // xmlParser.Settings.ConfigDirectoryNameDefault = string.Empty;
        }

        private void uploadControl1_OnFilePathChanged(string path)
        {
            // read bill files here
            billFiles.Clear();
            billFiles.AddRange(Directory.GetFiles(path, "*.bill"));
            billFiles.Sort();

            loadBillsAndGetFiledNames();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // open save dialog here
            if (saveFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                saveSettings();
                new ExcelDataWorker().FileWrite(saveFileDialog1.FileName, billsToExcel());
                MessageBox.Show("Completed! Bills are saved");
            }
        }

        private List<string> loadBillsAndGetFiledNames()
        {
            // step 1. load all bills
            BinaryFormatter binF = new BinaryFormatter();
            billData.Clear();

            foreach (string pathToBillFile in billFiles)
            {
                using (FileStream stream = new FileStream(pathToBillFile, FileMode.Open, FileAccess.Read))
                {
                    object[] billObj = (object[])binF.Deserialize(stream);
                    try
                    {
                        if (billObj[1] != null)
                        {
                            Hashtable billAllProps = (Hashtable)billObj[1];
                            Hashtable cleanProps = (Hashtable)billAllProps.Clone();
                            if (billAllProps.ContainsKey("BILL"))
                            {
                                Dictionary<string, object> billEntryProps = (Dictionary<string, object>)billAllProps["BILL"];
                                foreach (KeyValuePair<string, object> billEntryPropItem in billEntryProps)
                                    cleanProps["BILL_" + billEntryPropItem.Key] = billEntryPropItem.Value;
                                // remove dictionary
                                cleanProps.Remove("BILL");
                            }
                            billData.Add(cleanProps);
                        }
                    }
                    catch { }
                }
            }

            this.label_count.Text = billData.Count.ToString();

            // step 2. extract bill fileds
            this.listView1.Items.Clear();
            this.listView1.Sort();

            if (billData.Count > 0)
            {
                Hashtable billFirstEntry = billData[0];
                IEnumerator billKeysEnumerator = billFirstEntry.Keys.GetEnumerator();
                while (billKeysEnumerator.MoveNext())
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = billKeysEnumerator.Current.ToString();
                    // item.SubItems.Add("");
                    item.SubItems.Add(item.Name);
                    item.SubItems.Add(getItemHumanFrendlyLabel(item.Name));
                    item.Checked = getItemCheckedState(item.Name);

                    this.listView1.Items.Add(item);
                }
            }


            List<string> fields = new List<string>();
            return fields;
        }

        private bool getItemCheckedState(string itemName)
        {
            return ApplicationConfiguration.Instance.GetValueByPath<bool>("General.States." + itemName);
        }

        private string getItemHumanFrendlyLabel(string itemName)
        {
            string friendlyName = ApplicationConfiguration.Instance.GetValueByPath<string>("General.Titles." + itemName);
            if (friendlyName.Length == 0)
                friendlyName = itemName;
            return friendlyName;
        }

        private DataTable billsToExcel()
        {
            DataTable billsInfo = new DataTable();

            // set checked columns
            foreach (ListViewItem item in this.listView1.CheckedItems)
            {
                DataColumn dCol = new DataColumn(item.Name);
                dCol.Caption = getItemHumanFrendlyLabel(item.Name);
                billsInfo.Columns.Add(dCol);
            }

            if (billsInfo.Columns.Count == 0)
                return null;

            foreach (Hashtable billentry in billData)
            {
                DataRow dRow = billsInfo.NewRow();
                foreach (DataColumn column in billsInfo.Columns)
                    switch (column.ColumnName)
                    {
                        case "BILL_DELETED_ROWS":
                            {
                                int deletedRowsCounter = 0;
                                try
                                {
                                    Dictionary<string, object[]> deletedRows = (Dictionary<string, object[]>)billentry[column.ColumnName];
                                    deletedRowsCounter = deletedRows.Count;
                                }
                                catch { }
                                dRow[column.ColumnName] = deletedRowsCounter;
                                break;
                            }
                        default:
                            {
                                dRow[column.ColumnName] = billentry[column.ColumnName];
                                break;
                            }
                    }
                billsInfo.Rows.Add(dRow);
            }

            return billsInfo;
        }

        private void saveSettings()
        {
            // ConfigurationManager.AppSettings.Clear();
            Hashtable configTitles = new Hashtable();
            Hashtable configStates = new Hashtable();

            foreach (ListViewItem item in this.listView1.Items)
            {
                // save titles
                configTitles[item.Name] = item.SubItems[FIELD_KEY_TITLE].Text;
                // save states
                if (item.Checked)
                    configStates[item.Name] = true;
            }

            Hashtable config = new Hashtable() { 
                {"Titles", configTitles},
                {"States", configStates}
            };

            // Hashtable configWrapper = new Hashtable() { { "GENERAL", config } };
            ApplicationConfiguration.Instance.XmlParser.SetXmlData(new Hashtable() { { "General", config } });
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                textBox1.SuspendLayout();
                textBox1.Text = listView1.SelectedItems[0].SubItems[FIELD_KEY_TITLE].Text;
                textBox1.ResumeLayout();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.SelectedItems[0].SubItems[FIELD_KEY_TITLE].Text = textBox1.Text;
            }
        }

    }

}
