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

namespace BillsToExcel
{
    public partial class Form1 : Form
    {
        private List<string> billFiles = new List<string>();
        private List<Hashtable> billData = new List<Hashtable>();

        public Form1()
        {
            InitializeComponent();
        }

        private void uploadControl1_OnFilePathChanged(string path)
        {
            // read bill files here

            billFiles.Clear();
            billFiles.AddRange(Directory.GetFiles(path, "*.bill"));
            billFiles.Sort();

            loadBillsAndGetFiledNames();
            // billFiles
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // open save dialog here
            if (saveFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
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
                                    cleanProps["BILL." + billEntryPropItem.Key] = billEntryPropItem.Value;
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
            this.checkedListBox1.Items.Clear();
            this.checkedListBox1.Sorted = true;

            if (billData.Count > 0)
            {
                Hashtable billFirstEntry = billData[0];
                IEnumerator billKeysEnumerator = billFirstEntry.Keys.GetEnumerator();
                while (billKeysEnumerator.MoveNext())
                    this.checkedListBox1.Items.Add(billKeysEnumerator.Current);
            }


            List<string> fields = new List<string>();
            return fields;
        }

        private DataTable billsToExcel()
        {
            DataTable billsInfo = new DataTable();

            // set checked columns
            foreach (object selectedItem in this.checkedListBox1.CheckedItems)
            {
                billsInfo.Columns.Add(selectedItem.ToString());
            }

            if (billsInfo.Columns.Count == 0)
                return null;

            foreach (Hashtable billentry in billData)
            {
                DataRow dRow = billsInfo.NewRow();
                foreach (DataColumn column in billsInfo.Columns)
                    dRow[column.ColumnName] = billentry[column.ColumnName];
                billsInfo.Rows.Add(dRow);
            }

            return billsInfo;
        }
    }

}
