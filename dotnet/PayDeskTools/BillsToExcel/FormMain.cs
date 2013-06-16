using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using components.Components.CSVObject;
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
            uploadControl1.initSelectedPath(ApplicationConfiguration.Instance.GetValueByPath<string>("General.Paths.pathToBills"));
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
            if (folderBrowserDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                saveSettings();
                ApplicationConfiguration.Instance.ReloadConfigurationData();
                new CSVObject().Export(billsToExcel(), folderBrowserDialog1.SelectedPath, "\t");
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
                    if (billObj.Length == 2 && billObj[0] != null && billObj[1] != null)
                        try
                        {
                            Hashtable billAllProps = (Hashtable)billObj[1];
                            Hashtable info = (Hashtable)billAllProps.Clone();
                            Hashtable deletedData = new Hashtable();

                            if (billAllProps.ContainsKey("BILL"))
                            {
                                Dictionary<string, object> billEntryProps = (Dictionary<string, object>)billAllProps["BILL"];
                                foreach (KeyValuePair<string, object> billEntryPropItem in billEntryProps)
                                    info["BILL_" + billEntryPropItem.Key] = billEntryPropItem.Value;
                                // remove dictionary
                                info.Remove("BILL");
                            }

                            if (info.ContainsKey("BILL_DELETED_ROWS"))
                            {
                                int delCount = 0;
                                try
                                {
                                    Dictionary<string, object[]> deletedRows = (Dictionary<string, object[]>)info["BILL_DELETED_ROWS"];
                                    delCount = deletedRows.Count;
                                    if (delCount > 0)
                                    {
                                        foreach (KeyValuePair<string, object[]> deletedRow in deletedRows)
                                            deletedData[deletedRow.Key] = deletedRow.Value;
                                    }
                                }
                                catch { }
                                finally
                                {
                                    info.Remove("BILL_DELETED_ROWS");
                                    info["BILL_DELETED_ROWS"] = delCount;
                                }
                            }

                            Hashtable transformBillData = new Hashtable()
                            {
                                {"DATA", (DataTable)billObj[0]},
                                {"INFO", info},
                                {"DELETED", deletedData}
                            };

                            billData.Add(transformBillData);
                        }
                        catch { }
                }
            }

            this.label_count.Text = billData.Count.ToString();

            // step 2. extract bill fileds
            this.listViewGeneral.Items.Clear();
            this.listViewGeneral.Sort();
            this.listViewProducts.Items.Clear();
            this.listViewProducts.Sort();

            if (billData.Count > 0)
            {
                Hashtable billFirstEntry = billData[0];

                IEnumerator billKeysEnumerator = ((Hashtable)billFirstEntry["INFO"]).Keys.GetEnumerator();
                while (billKeysEnumerator.MoveNext())
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = billKeysEnumerator.Current.ToString();
                    // item.SubItems.Add("");
                    item.SubItems.Add(item.Name);
                    item.SubItems.Add(getItemHumanFrendlyLabel("Info." + item.Name));
                    item.Checked = getItemCheckedState(item.Name);

                    this.listViewGeneral.Items.Add(item);
                }

                foreach (DataColumn dCol in ((DataTable)billFirstEntry["DATA"]).Columns)
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = dCol.ColumnName;
                    // item.SubItems.Add("");
                    item.SubItems.Add(dCol.ColumnName);
                    item.SubItems.Add(getItemHumanFrendlyLabel("Product." + dCol.ColumnName));
                    item.Checked = getItemCheckedState(dCol.ColumnName);

                    this.listViewProducts.Items.Add(item);
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

        private DataSet billsToExcel()
        {
            DataTable productSold = new DataTable("Sold");
            DataTable productRemoved = new DataTable("Removed");
            DataTable billsInfo = new DataTable("Info");

            // add info columns
            foreach (ListViewItem item in this.listViewGeneral.CheckedItems)
            {
                switch (item.Name)
                {
                    case "BILL_COMMENT":
                        {
                            string caption = getItemHumanFrendlyLabel("Info." + item.Name);
                            string[] fieldsToInclude = caption.Split(';');
                            int idx = 0;
                            foreach (string fldCap in fieldsToInclude)
                            {
                                DataColumn dCol = new DataColumn(item.Name + "_" + (idx));
                                dCol.Caption = fldCap.Trim();
                                billsInfo.Columns.Add(dCol);
                                dCol.ExtendedProperties.Add("PARENT", item.Name);
                                dCol.ExtendedProperties.Add("DATA_INDEX", idx);
                                idx++;
                            }
                            break;
                        }
                    default:
                        {
                            DataColumn dCol = new DataColumn(item.Name);
                            dCol.Caption = getItemHumanFrendlyLabel("Info." + item.Name);
                            billsInfo.Columns.Add(dCol);
                            dCol.ExtendedProperties.Add("PARENT", item.Name);
                            break;
                        }
                }
            }

            // add product columns
            foreach (ListViewItem item in this.listViewProducts.CheckedItems)
            {
                DataColumn dColS = new DataColumn(item.Name);
                dColS.Caption = getItemHumanFrendlyLabel("Product." + item.Name);
                productSold.Columns.Add(dColS);

                DataColumn dColD = new DataColumn(item.Name);
                dColD.Caption = getItemHumanFrendlyLabel("Product." + item.Name);
                productRemoved.Columns.Add(dColD);
            }


            if (billsInfo.Columns.Count == 0)
                return null;

            // Add data relation column
            billsInfo.Columns.Add("_ID");
            productSold.Columns.Add("_BillID");
            productRemoved.Columns.Add("_BillID");


            int billFakeID = 0;

            // loop through bills
            foreach (Hashtable billentry in billData)
            {

                Hashtable billentryInfo = (Hashtable)billentry["INFO"];

                // add bill info row
                DataRow dRow = billsInfo.NewRow();
                // add relation index
                dRow["_ID"] = billFakeID;

                foreach (DataColumn column in billsInfo.Columns)
                {
                    string parentName = column.ColumnName;
                    if (column.ExtendedProperties.ContainsKey("PARENT"))
                        parentName = column.ExtendedProperties["PARENT"].ToString();
                    switch (parentName)
                    {
                        case "_ID":
                            {
                                break;
                            }
                        case "DISCOUNT":
                            {
                                string disc = string.Empty;
                                try
                                {
                                    Hashtable discount = (Hashtable)billentryInfo["DISCOUNT"];

                                    // add discount in the percnete
                                    if (discount.ContainsKey("DISC_FINAL_PERCENT"))
                                        disc += discount["DISC_FINAL_PERCENT"] + "%";

                                    // add discount in cash value
                                    if (discount.ContainsKey("DISC_FINAL_CASH"))
                                        disc += " (" + discount["DISC_FINAL_CASH"] + "грн.)";
                                }
                                catch { }
                                dRow[column.ColumnName] = disc;
                                break;
                            }
                        case "BILL_COMMENT":
                            {
                                string fullComment = billentryInfo[parentName].ToString();
                                string[] exploded = fullComment.Split(' ');
                                int dataIndex = 0;
                                if (column.ExtendedProperties.ContainsKey("DATA_INDEX"))
                                    dataIndex = int.Parse(column.ExtendedProperties["DATA_INDEX"].ToString());
                                if (exploded.Length > dataIndex && exploded[dataIndex] != null)
                                    dRow[column.ColumnName] = exploded[dataIndex].Replace("%20", "");
                                break;
                            }
                        case "PAYMENT":
                            {
                                StringBuilder payment = new StringBuilder();
                                try
                                {
                                    Dictionary<string, object> payInfo = (Dictionary<string, object>)billentryInfo["PAYMENT"];

                                    try
                                    {
                                        payment.Append("Оплата: ");
                                        if (payInfo.ContainsKey("TYPE"))
                                            switch (((List<byte>)payInfo["TYPE"])[0])
                                            {
                                                case 0: { payment.Append("Картка"); break; }
                                                case 1: { payment.Append("Кредит");  break; }
                                                case 2: { payment.Append("Чек"); break; }
                                                case 3: { payment.Append("Готівка"); break; }
                                            }
                                    }
                                    catch { }

                                    try
                                    {
                                        payment.Append(" ; ");
                                        payment.Append("Гроші: ");
                                        if (payInfo.ContainsKey("SUMA"))
                                            payment.Append(payInfo["SUMA"]);
                                    }
                                    catch { }

                                    try
                                    {
                                        payment.Append(" ; ");
                                        payment.Append("Решта: ");
                                        if (payInfo.ContainsKey("REST"))
                                            payment.Append(payInfo["REST"]);
                                    }
                                    catch { }

                                }
                                catch { }
                                dRow[column.ColumnName] = payment.ToString().Trim();
                                break;
                            }
                        default:
                            {
                                dRow[column.ColumnName] = billentryInfo[column.ColumnName];
                                break;
                            }
                    }
                }
                billsInfo.Rows.Add(dRow);

                // add sold products
                DataTable content = (DataTable)billentry["DATA"];
                foreach (DataRow dSoldSrcRow in content.Rows)
                {
                    DataRow dSoldDestRow = productSold.NewRow();
                    // add relation index
                    dSoldDestRow["_BillID"] = billFakeID;
                    // loop through visible fileds
                    foreach (DataColumn dColSold in productSold.Columns)
                        if (dColSold.ColumnName != "_BillID")
                            dSoldDestRow[dColSold.ColumnName] = dSoldSrcRow[dColSold.ColumnName];

                    productSold.Rows.Add(dSoldDestRow);
                }

                // add removed products
                try
                {
                    Hashtable deletedRows = (Hashtable)billentry["DELETED"];
                    foreach (DictionaryEntry dDelSrcRow in deletedRows)
                    {
                        DataRow dDelDestRow = productRemoved.NewRow();
                        object[] removedItemsArray = (object[])dDelSrcRow.Value;
                        // we use different approach because we do not know column name, just item position
                        // it is the same as column name position in the list
                        foreach (ListViewItem listItemProd in listViewProducts.Items)
                        {
                            // use selected items only
                            if (listItemProd.Checked && productRemoved.Columns.IndexOf(listItemProd.Name) >= 0 && removedItemsArray.Length > listItemProd.Index)
                                dDelDestRow[listItemProd.Name] = removedItemsArray[listItemProd.Index];
                        }
                        // add relation index
                        dDelDestRow["_BillID"] = billFakeID;

                        productRemoved.Rows.Add(dDelDestRow);
                    }
                }
                catch { }

                billFakeID++;
            }

            DataSet ds = new DataSet();

            ds.Tables.Add(productSold);
            ds.Tables.Add(productRemoved);
            ds.Tables.Add(billsInfo);

            return ds;
        }

        private void saveSettings()
        {
            // ConfigurationManager.AppSettings.Clear();
            Hashtable path = new Hashtable() { { "pathToBills", uploadControl1.FilePath } };
            Hashtable configInfoTitles = new Hashtable();
            Hashtable configProductTitles = new Hashtable();
            Hashtable configStates = new Hashtable();

            foreach (ListViewItem item in this.listViewGeneral.Items)
            {
                // save titles
                configInfoTitles[item.Name] = item.SubItems[FIELD_KEY_TITLE].Text;
                // save states
                if (item.Checked)
                    configStates[item.Name] = true;
            }

            foreach (ListViewItem item in this.listViewProducts.Items)
            {
                // save titles
                configProductTitles[item.Name] = item.SubItems[FIELD_KEY_TITLE].Text;
                // save states
                if (item.Checked)
                    configStates[item.Name] = true;
            }

            Hashtable config = new Hashtable() { 
                {"Paths", path},
                {"Titles", new Hashtable()
                    {
                        {"Info", configInfoTitles},
                        {"Product", configProductTitles}
                    }
                },
                {"States", configStates}
            };

            // Hashtable configWrapper = new Hashtable() { { "GENERAL", config } };
            ApplicationConfiguration.Instance.XmlParser.SetXmlData(new Hashtable() { { "General", config } });
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView list = (ListView)sender;

            if (list.SelectedItems.Count > 0)
            {
                textBox1.SuspendLayout();
                textBox1.Tag = list.Name;
                textBox1.Text = list.SelectedItems[0].SubItems[FIELD_KEY_TITLE].Text;
                textBox1.ResumeLayout();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Control[] controls = (Control[])Controls.Find(textBox1.Tag.ToString(), true);
            if (controls.Length > 0)
            {
                try
                {
                    ListView list = (ListView)controls[0];
                    if (list != null && list.SelectedItems.Count > 0)
                    {
                        list.SelectedItems[0].SubItems[FIELD_KEY_TITLE].Text = textBox1.Text;
                    }
                }
                catch { }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.saveSettings();
        }

    }

}
