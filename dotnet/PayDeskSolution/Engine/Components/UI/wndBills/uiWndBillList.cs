using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//0using mdcore;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
//0using ;
//0using mdcore.Components.UI;
//0using mdcore.Config;
using Microsoft.VisualBasic.FileIO;
using PayDesk.Components;
//0using mdcore.Common;
using System.Collections;
using driver.Config;
using driver.Lib;
using driver.Common;
using driver.Components.UI;
using components.Components.MMessageBox;

namespace PayDesk.Components.UI.wndBills
{
    public partial class uiWndBillList : Form
    {
        //private FileStream stream; .Components.UI.wndBills
        //private BinaryFormatter binF = new BinaryFormatter();
        private DataTable dTBill = new DataTable();
        private DataTable dTList = new DataTable();
        private string loadedBillNo;
        //private double billListSuma;
        private string[] bills;
        private Dictionary<string, string> billFileList = new Dictionary<string, string>();
        private int currentActiveRowIndex = -1;
        private string currentActiveBillOID = string.Empty;
        private DateTime currentDateTimeFilter = new DateTime();

        /* CONSTRUCTORS */

        /// <summary>
        /// 
        /// </summary>
        public uiWndBillList()
        {
            InitializeComponent();

            // restore position
            try
            {
                this.Location = ((Point)ConfigManager.Instance.CommonConfiguration.WP_ALL["BILL_VIEW"]);
                this.StartPosition = FormStartPosition.Manual;
            }
            catch
            {
                if (ConfigManager.Instance.CommonConfiguration.WP_ALL == null)
                    ConfigManager.Instance.CommonConfiguration.WP_ALL = new System.Collections.Hashtable();
                // saving position
                ConfigManager.Instance.CommonConfiguration.WP_ALL["BILL_VIEW"] = this.Location;
            }

            this.loadedBillNo = string.Empty;
            //this.billListSuma = 0.0;
            this.Font = ConfigManager.Instance.CommonConfiguration.STYLE_BillWindow;
            //this.listGrid.Font = new Font(ConfigManager.Instance.CommonConfiguration.STYLE_BillWindow.FontFamily, ConfigManager.Instance.CommonConfiguration.STYLE_BillWindow.Size + 2F, ConfigManager.Instance.CommonConfiguration.STYLE_BillWindow.Style);
            this.listGrid.Font = ConfigManager.Instance.CommonConfiguration.STYLE_BillWindowEntry;
            this.billGrid.Font = ConfigManager.Instance.CommonConfiguration.STYLE_BillWindowEntryItems;
            this.listGrid.RowTemplate.Height = ConfigManager.Instance.CommonConfiguration.STYLE_Misc_BillItemsRowHeight;
            this.billGrid.RowTemplate.Height = ConfigManager.Instance.CommonConfiguration.STYLE_Misc_BillItemProductsRowHeight;
            

            this.fileSystemWatcher_billIFolder.Path = ConfigManager.Instance.CommonConfiguration.Path_Bills;

            // hide total sum
            this.label5.Visible = this.label6.Visible = ConfigManager.Instance.CommonConfiguration.Content_Bills_ShowBillTotalSum;

            // hide column
            this.sum.Visible = ConfigManager.Instance.CommonConfiguration.Content_Bills_ShowBillSumColumn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadedBillNo"></param>
        public uiWndBillList(string loadedBillNo)
            : this()
        {
            if (loadedBillNo != new object().ToString())
                this.loadedBillNo = loadedBillNo;
        }

        /* EVENTS */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BillsList_Load(object sender, EventArgs e)
        {
            this.LoadDayBillsWithClear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BillsList_KeyDown(object sender, KeyEventArgs e)
        {
            // Find
            if (e.KeyValue == new KeyEventArgs(Keys.F).KeyValue && e.Alt)
            {
                textBox_billsList_billNoSearch.Select();
                textBox_billsList_billNoSearch.SelectAll();
                return;
            }
            // Print
            if (e.KeyValue == new KeyEventArgs(Keys.P).KeyValue && e.Alt)
            {
                button_billsList_Print.PerformClick();
                return;
            }
            // Delete
            if (e.KeyValue == new KeyEventArgs(Keys.D).KeyValue && e.Alt)
            {
                button_billsList_Delete.PerformClick();
                return;
            }
            // Load selected bill
            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
            {
                button_billsList_Open.PerformClick();
                return;
            }
            // Exit
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                if (textBox_billsList_billNoSearch.Focused)
                {
                    textBox_billsList_billNoSearch.ResetText();
                    listGrid.Focus();
                    listGrid.Select();
                    for (int i = 0; i < this.listGrid.RowCount; i++)
                        this.listGrid.Rows[i].Visible = true;
                }
                else
                    Close();
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (DialogResult == DialogResult.OK)
                return;

            if (listGrid.SelectedRows.Count != 0 && listGrid.SelectedRows[0] != null)
            {
                int rIdx = listGrid.SelectedRows[0].Index;
                try
                {
                    dTBill = LoadActiveBill();
                    billGrid.DataSource = dTBill;
                    this.currentActiveBillOID = DataWorkShared.ExtractBillProperty(this.dTBill, CoreConst.OID).ToString();
                    Dictionary<string, object> billInfo = DataWorkShared.GetBillInfo(this.dTBill);
                    Dictionary<string, object> orderInfo = DataWorkShared.GetOrderInfo(this.dTBill);

                    for (int i = 0; i < billGrid.ColumnCount; i++)
                        switch (billGrid.Columns[i].Name)
                        {
                            case "NAME":
                                billGrid.Columns[i].HeaderText = "Назва";
                                billGrid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                break;
                            case "TOT":
                                billGrid.Columns[i].HeaderText = "К-сть";
                                billGrid.Columns[i].Width = 40;
                                break;
                            case "PRICE":
                                billGrid.Columns[i].HeaderText = "Ціна";
                                billGrid.Columns[i].Width = 50;
                                break;
                            case "SUM":
                                billGrid.Columns[i].HeaderText = "Сума";
                                billGrid.Columns[i].Width = 70;
                                break;
                            default:
                                billGrid.Columns[i].Visible = false;
                                break;
                        }


                    //double chqSUMA = (double)dTBill.Compute("sum(SUM)", "");
                    //double chqSUMA = (double)dTBill.ExtendedProperties[CoreConst.ORDER_REAL_SUMA];
                    //label2.Text = string.Format("{0} {1} {2} {3:0.00}{4}", "Перегляд рахунку №", billInfo[CoreConst.BILL_NO], "на суму:", chqSUMA, "грн.");

                    bool billIsLoaded = DataWorkShared.ExtractBillProperty(dTBill, CoreConst.BILL_NO, string.Empty).ToString() == this.loadedBillNo;
                    bool billIsClosed = DataWorkShared.ExtractOrderProperty(dTBill, CoreConst.ORDER_NO, string.Empty, false).ToString() != string.Empty;
                    bool billIsLocked =(bool)DataWorkShared.ExtractBillProperty(dTBill, CoreConst.IS_LOCKED, false);

                    button_billsList_Open.Enabled = !billIsClosed && !billIsLoaded;
                    button_billsList_Print.Enabled = !billIsClosed;// && !billIsLocked;
                    button_billsList_Delete.Enabled = !billIsClosed && !billIsLocked && !billIsLoaded;
                    button_billsList_madecopy.Enabled = !billIsClosed && billIsLocked && !billIsLoaded;
                    button_billsList_unlock.Visible = false;
                    string orderNo = DataWorkShared.ExtractOrderProperty(dTBill, CoreConst.ORDER_NO, string.Empty).ToString();
                    string billState = string.Empty;
                    switch (orderNo)
                    {
                        case "null": { billState = "Рахунок АНУЛЬОВАНИЙ"; orderNo = "Нема"; break; }
                        case "copy": { billState = "Створена копія рахунку."; orderNo = "Нема"; break; }
                    }

                    if (orderNo == string.Empty)
                        orderNo = "Нема";


                    if (billIsClosed)
                        billState += "Рахунок закритий.";
                    else
                        if (billIsLocked)
                            billState += "Рахунок надрукований клієнту. Зробіть чек!";
                        else
                            billState += "Доступний для редагування.";

                    if (billIsLoaded)
                        billState += "   Відкритий в основному вікні.";


                    label2.Text = billState.Trim();

                    label_orderInfo_suma.Text = string.Format("{0}{1} {2}", "Сума без знижок", ":", DataWorkShared.ExtractOrderProperty(dTBill, CoreConst.ORDER_SUMA));
                    label_orderInfo_realSuma.Text = string.Format("{0}{1} {2}", "СУМА", ":", DataWorkShared.ExtractOrderProperty(dTBill, CoreConst.ORDER_REAL_SUMA));
                    label_orderInfo_orderNo.Text = string.Format("{0}{1} {2}", "Номер чеку", ":", orderNo);
                    label_orderInfo_discount.Text = string.Empty; 
                    /*if (billIsClosed || billIsLocked)
                    {*/
                        // bill is closed forever
                        /*button_billsList_Open.Enabled = !billIsClosed;
                        button_billsList_Print.Enabled = false;
                        button_billsList_Delete.Enabled = false;
                        button_billsList_madecopy.Enabled = !billIsClosed && billIsLocked;
                        button_billsList_unlock.Visible = false;*/
                   /* }
                    else*/
                    //{
                    /*
                        // bill is not closed but is was printed, so it locked
                        if ((dTBill.ExtendedProperties.ContainsKey("LOCK") && bool.Parse(dTBill.ExtendedProperties["LOCK"].ToString())) || billIsLoaded)
                        {
                            // if this bill is loaded we couldn't load it again.
                            button_billsList_Open.Enabled = !billIsLoaded;
                            // lock PRINT, DELETE, COPY operations
                            button_billsList_Print.Enabled = false;
                            button_billsList_Delete.Enabled = false;
                            button_billsList_madecopy.Enabled = false;
                            // if this bill is loaded we couldn't ulock it
                            button_billsList_unlock.Visible = !billIsLoaded;
                        }
                        else
                        {*/
                            // bill is not closed and locked too. Normal bill.
                           /* button_billsList_Open.Enabled = true;
                            button_billsList_Print.Enabled = true;
                            button_billsList_Delete.Enabled = true;
                            button_billsList_madecopy.Enabled = true;
                            button_billsList_unlock.Visible = false;*/
                        //}
                    //}
                    


                }
                catch { }

                // shif v-scrollbar
                //this.listGrid.Rows[ridx].Selected = true;
                vScrollBar1.Value = rIdx;


            }
            else
            {
                this.ActivityControls(false);
                button_billsList_unlock.Visible = false;
            }

            //this.listGrid.BackgroundColor = this.listGrid.BackgroundColor;
            //this.listGrid.Parent.BackColor = this.listGrid.BackgroundColor;
            //this.listGrid.Refresh();
            //this.listGrid.Parent.Refresh();
            //this.listGrid.Update();
            //this.listGrid.Invalidate();
            this.listGrid.Focus();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listGrid_Leave(object sender, EventArgs e)
        {
            listGrid.DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.GrayText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listGrid_Enter(object sender, EventArgs e)
        {
            listGrid.DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.Highlight);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_billsList_Print_Click(object sender, EventArgs e)
        {
            PayDesk.Components.UI.wndBills.uiWndBillPrint bp = new PayDesk.Components.UI.wndBills.uiWndBillPrint(dTBill);
            DialogResult printResult = bp.ShowDialog(this);
            bp.Dispose();
            /*
            CoreLib.Print(new object[] { dTBill }, "kitchen", 1);

            for (int i = 0; i < dTBill.Rows.Count; i++)
                dTBill.Rows[i]["PRINTCOUNT"] = Convert.ToDouble(dTBill.Rows[i]["TOT"]);

            CoreLib.SaveBill(false, 0, "", dTBill);*/

            this.LoadDayBillsWithClear();
            //this.FilterByDate(this.dateTimePicker1.Value, this.currentActiveBillOID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_billsList_Delete_Click(object sender, EventArgs e)
        {
            if (this.loadedBillNo == DataWorkShared.ExtractBillProperty(this.dTBill, CoreConst.BILL_NO, string.Empty).ToString())
                MMessageBox.Show("Неможливо видалити рахунок № " + this.loadedBillNo + "\r\nВін є відкритий в основному вікні",
                      Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {/*
                if (!UserConfig.Properties[24])
                    if (new Admin().ShowDialog(this) != DialogResult.OK)
                        return;*/
                DataWorkBill.LockBill(dTBill, "null");
                this.LoadDayBillsWithClear();
                /*
                if ()
                {
                    for (int i = 0, j = 0; j < bills.Length; i++, j++)
                    {
                        if (i == listGrid.CurrentRow.Index)
                            j++;
                        if (j < bills.Length)
                            bills[i] = bills[j];
                    }
                    Array.Resize<string>(ref bills, bills.Length - 1);
                    listGrid.Rows.Remove(listGrid.CurrentRow);
                    if (listGrid.RowCount == 0)
                        this.button_billsList_ShowAllBills.Enabled = false;
                }*/
            }

        }
        /// <summary>
        /// Завантаження рахунку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_billsList_Open_Click(object sender, EventArgs e)
        {
            if (this.loadedBillNo != string.Empty)
            {
                MMessageBox.Show("Неможливо відкрити рахунок № " + DataWorkShared.ExtractBillProperty(this.dTBill, CoreConst.BILL_NO, string.Empty) + "\r\nВ основному вікні завантажений інший рахунок № " + this.loadedBillNo,
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listGrid.Focused || button_billsList_Open.Focused)
            {
                if (listGrid.RowCount != 0)
                {
                    dTBill = LoadActiveBill();
                    DialogResult = DialogResult.OK;
                }
                else
                    DialogResult = DialogResult.None;
                listGrid.SelectionChanged -= listGrid_SelectionChanged;
                Close();
            }
            else
                this.FilterByNo(textBox_billsList_billNoSearch.Text);
        }
        /// <summary>
        /// Refresh (read) all bill files from folder and apply latest filter by date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_billsList_Refresh_Click(object sender, EventArgs e)
        {
            this.textBox_billsList_billNoSearch.Text = this.textBox_billsList_billNoSearch.Tag.ToString();
            this.textBox_billsList_billNoSearch.Refresh();
            this.billGrid.Select();
            this.billGrid.Update();
            this.LoadDayBillsWithClear();
        }
        /// <summary>
        /// Unlock selected bill.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_billsList_unlock_Click(object sender, EventArgs e)
        {
            if (!UserConfig.Properties[24])
                if (new uiWndAdmin().ShowDialog(this) != DialogResult.OK)
                    return;

            DataWorkBill.UnlockBill(this.LoadedBill);
            this.LoadDayBills();
            //this.FilterByDate(this.dateTimePicker1.Value, this.currentActiveBillOID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_billsList_ShowAllBills_Click(object sender, EventArgs e)
        {
            PayDesk.Components.UI.wndBills.uiWndBillDateFilter dateFilter = new PayDesk.Components.UI.wndBills.uiWndBillDateFilter();
            if (dateFilter.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                //this.FilterReset(this.currentActiveBillOID);
                this.LoadRangeBills(dateFilter.Filter_FromDate ,dateFilter.Filter_ToDate);
 
            }
            dateFilter.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_billsList_madecopy_Click(object sender, EventArgs e)
        {
            if (this.loadedBillNo == DataWorkShared.ExtractBillProperty(this.dTBill, CoreConst.BILL_NO, string.Empty).ToString())
            {
                MMessageBox.Show("Неможливо зробити копію рахунку № " + this.loadedBillNo + "\r\nВін є відкритий в основному вікні",
                      Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            /*
            if (!UserConfig.Properties[24])
                if (new Admin().ShowDialog(this) != DialogResult.OK)
                    return;
            */
            DataTable dTable = this.LoadActiveBill();
            DataTable dNewTable = this.LoadActiveBill();
            DataWorkShared.SetBillProperty(dNewTable, CoreConst.IS_LOCKED, false);
            //DataTable dTableCopy = dTable.Copy();
            DataWorkBill.MadeBillCopy(dNewTable);
            DataWorkBill.LockBill(dTable, "copy");
            this.currentActiveBillOID = DataWorkShared.ExtractBillProperty(dNewTable, CoreConst.OID, string.Empty).ToString();
            //CoreLib.MadeBillCopy(dTableCopy);
            //CoreLib.LockBill(dTable, "-1");
            //this.currentActiveBillOID = dTableCopy.ExtendedProperties["NOM"].ToString();
            this.LoadDayBillsWithClear();
            //this.FilterByDate(this.dateTimePicker1.Value, this.currentActiveBillOID);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_billsList_Search_Click(object sender, EventArgs e)
        {/*
            if (this.dateTimePicker1.Checked)
            {
                this.FilterAll(this.dateTimePicker1.Value, this.textBox1.Text, this.currentActiveBillOID);
            }
            else*/
            if (this.textBox_billsList_billNoSearch.Text != string.Empty && this.textBox_billsList_billNoSearch.Text != this.textBox_billsList_billNoSearch.Tag.ToString())
                this.FilterByNo(this.textBox_billsList_billNoSearch.Text);
            else
                this.LoadDayBillsWithClear();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_billsList_billNoSearch_Validated(object sender, EventArgs e)
        {
            if (this.textBox_billsList_billNoSearch.Text == string.Empty)
                this.textBox_billsList_billNoSearch.Text = this.textBox_billsList_billNoSearch.Tag.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_billsList_billNoSearch_Enter(object sender, EventArgs e)
        {
            if (this.textBox_billsList_billNoSearch.Text == this.textBox_billsList_billNoSearch.Tag.ToString())
                this.textBox_billsList_billNoSearch.Text = string.Empty;

        }

        /* METHODS */

        /// <summary>
        /// Load active (selected) bill from bill's list
        /// </summary>
        /// <returns></returns>
        private DataTable LoadActiveBill()
        {
            string index = listGrid.SelectedRows[0].Cells["oid"].Value.ToString();
            //return DataWorkShared.CombineDataObject(DataWorkBill.LoadBillByPath(this.billFileList[index]));
            return DataWorkBill.LoadCombinedBill(this.billFileList[index]);
        }
        /// <summary>
        /// Load all bills from bill's directory
        /// </summary>
        private double LoadDayBills()
        {
            return this.LoadDayBills(DateTime.Now);
        }
        private double LoadDayBills(DateTime selectedDay)
        {
            Dictionary<string, object> items = DataWorkBill.LoadDayBills(selectedDay, ConfigManager.Instance.CommonConfiguration.Path_Bills, ConfigManager.Instance.CommonConfiguration.APP_SubUnit);
            DataTable currentBill = new DataTable();
            PropertyCollection props = new PropertyCollection();
            Dictionary<string, object> billInfo = new Dictionary<string, object>();
            double generalSuma = 0.0;
            try
            {
                foreach (KeyValuePair<string, object> billEntry in items)
                {
                    currentBill = (DataTable)((object[])billEntry.Value)[0];
                    props = (PropertyCollection)((object[])billEntry.Value)[1];
                    billInfo = ((Dictionary<string, object>)props[CoreConst.BILL]);
                    this.billFileList.Add(billInfo[CoreConst.OID].ToString(), billEntry.Key);

                    listGrid.Rows.Add(
                        new object[] {
                            billInfo[CoreConst.OID], 
                            billInfo[CoreConst.BILL_NO],
                            billInfo[CoreConst.DATETIME],
                            (billInfo[CoreConst.COMMENT] != null)?billInfo[CoreConst.COMMENT].ToString().Replace("%20", " "):"", 
                            (double)props[CoreConst.ORDER_REAL_SUMA], 
                            bool.Parse(billInfo[CoreConst.IS_LOCKED].ToString()), 
                            props[CoreConst.ORDER_NO],
                            (billInfo.ContainsKey(CoreConst.DATETIME_LOCK)?billInfo[CoreConst.DATETIME_LOCK]:"-")
                        }
                    );

                    generalSuma += (double)props[CoreConst.ORDER_REAL_SUMA];
                    if (props.ContainsKey(CoreConst.ORDER_NO) && props[CoreConst.ORDER_NO] != null && props[CoreConst.ORDER_NO].ToString() != string.Empty)
                    {
                        Font extFont = listGrid.Font;
                        listGrid.Rows[listGrid.Rows.Count - 1].DefaultCellStyle.Font = new Font(extFont, FontStyle.Strikeout);
                    }
                    else
                        listGrid.Rows[listGrid.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightPink;

                }
            }
            catch { }


            /*
            string item = string.Empty;
            bills = Directory.GetFiles(ConfigManager.Instance.CommonConfiguration.Path_Bills, string.Format("{0:X2}_N*_{1}.bill", ConfigManager.Instance.CommonConfiguration.APP_SubUnit, selectedDay.ToString("ddMMyy")));
            Array.Sort(bills);
            double billListSuma = 0.0;
            double billSuma = 0.0;
            object[] billEntry = new object[2];
            PropertyCollection props = new PropertyCollection();
            Dictionary<string, object> billInfo = new Dictionary<string, object>();
            object orderNo = new object();
            for (int i = 0; i < bills.Length; i++, item = string.Empty)
            {
                try
                {
                    stream = new FileStream(bills[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                    billEntry = (object[])binF.Deserialize(stream);
                    //dTBill = (DataTable)binF.Deserialize(stream);
                    dTBill = (DataTable)billEntry[0];
                    props = (PropertyCollection)billEntry[1];
                    billInfo = ((Dictionary<string, object>)props[CoreConst.BILL]);

                    stream.Close();
                    stream.Dispose();

                    //Adding item
                    billFileList.Add(billInfo[CoreConst.OID].ToString(), bills[i]);
                    billSuma = (double)props[CoreConst.ORDER_SUMA];
                    orderNo = props[CoreConst.ORDER_NO];
                    // hide special flags
                    //if (orderNo != null && (string.Compare(orderNo.ToString(), "null") == 0 || string.Compare(orderNo.ToString(), "k") == 0))
                    //    orderNo = string.Empty;
                    listGrid.Rows.Add(
                        new object[] {
                            billInfo[CoreConst.OID], 
                            billInfo[CoreConst.BILL_NO],
                            billInfo[CoreConst.DATETIME],
                            billInfo[CoreConst.COMMENT], 
                            billSuma, 
                            bool.Parse(billInfo[CoreConst.IS_LOCKED].ToString()), 
                            orderNo
                        }
                    );
                    billListSuma += billSuma;
                    if (props.ContainsKey(CoreConst.ORDER_NO) && props[CoreConst.ORDER_NO] != null && props[CoreConst.ORDER_NO].ToString() != string.Empty)
                    {
                        Font extFont = listGrid.Font;
                        listGrid.Rows[listGrid.Rows.Count - 1].DefaultCellStyle.Font = new Font(extFont, FontStyle.Strikeout);
                    }
                }
                catch (Exception ex) { CoreLib.WriteLog(ex, "LoadDayBills(DateTime selectedDay); Unable to load bill file: " + bills[i]); }

            }*/

            return generalSuma;
        }
        private void LoadDayBillsBeforeHook()
        {
            if (this.listGrid.Rows.Count > 0 || this.billFileList.Count > 0)
            {
                listGrid.SelectionChanged -= listGrid_SelectionChanged;
                this.billFileList.Clear();
                this.listGrid.Rows.Clear();
                //firstLoad = false;
            }
        }
        private void LoadDayBillsAfterHook(Hashtable param)
        {
            this.label_allBills_TotalRecords.Text = this.listGrid.RowCount.ToString();
            this.label6.Text = string.Format("{0:0.00}", (double)param["SUMA"]);
            listGrid.SelectionChanged += listGrid_SelectionChanged;
            if (this.listGrid.RowCount != 0)
            {
                bool wasSelected = false;
                int index = 0;
                if (this.currentActiveBillOID != string.Empty)
                {
                    for (int i = 0; i < listGrid.RowCount; i++)
                        if (listGrid["OID", i].Value.ToString() == this.currentActiveBillOID)
                        {
                            wasSelected = true;
                            this.listGrid.Rows[i].Selected = true;
                            this.listGrid.FirstDisplayedScrollingRowIndex = i;
                            index = i;
                            break;
                        }
                }
                if (!wasSelected)
                {
                    index = this.listGrid.Rows.GetLastRow(DataGridViewElementStates.None);
                    this.listGrid.Rows[index].Selected = true;

                    //index = this.listGrid.Rows[this.listGrid.Rows.GetLastRow(DataGridViewElementStates.None)].Index-1;
                    //this.listGrid.FirstDisplayedScrollingRowIndex = index;
                }


                //this.listGrid_CellMouseClick(this.listGrid, new DataGridViewCellMouseEventArgs(1, index, 20, 20, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 2, 10, 10, 1)));
                  // this.listGrid.Focus();
                  // this.listGrid.Select();
                //this.listGrid.InvalidateRow(this.listGrid.SelectedRows[0].Index);
                this.button_billsList_Open.Enabled = true;
                this.listGrid.CurrentCell = this.listGrid[1, index];
            }
            else
            {
                this.button_billsList_Open.Enabled = false;
            }

            this.listGrid.Focus();
            this.listGrid.Select();


            listGrid_SelectionChanged(listGrid, EventArgs.Empty);

            /* USING FOR SCROLL AT FIRST DISPLAYED ROW ONLY
            // correcting number (total visible rows in list)
            this.listGrid.FirstDisplayedScrollingRowIndex = this.listGrid.RowCount - 1;
            if (this.listGrid.FirstDisplayedScrollingRowIndex != 0)
            {
                this.vScrollBar1.Visible = true;
                this.vScrollBar1.Maximum = this.listGrid.FirstDisplayedScrollingRowIndex * 5 + 4;
                this.vScrollBar1.SmallChange = 5;
                this.vScrollBar1.LargeChange = 5;
            }
            else
                this.vScrollBar1.Visible = false;
            this.listGrid.FirstDisplayedScrollingRowIndex = 0;
            */

            /* USED FOR SCROLLING BY ALL ROWS WITH SELECTION */
            // correcting number (total visible rows in list)
            if (this.listGrid.RowCount != 0)
                this.listGrid.FirstDisplayedScrollingRowIndex = this.listGrid.RowCount - 1;
            if (this.listGrid.FirstDisplayedScrollingRowIndex > 0)
            {
                this.vScrollBar1.Maximum = this.listGrid.RowCount - 1;
                this.vScrollBar1.SmallChange = 1;
                this.vScrollBar1.LargeChange = 1;
                this.vScrollBar1.Value = this.vScrollBar1.Maximum;
                this.vScrollBar1.Visible = true;
            }
            else
                this.vScrollBar1.Visible = false;


            //listGrid.Select();
            //listGrid.Focus();
            //this.listGrid.PerformLayout();
        }
        private void LoadRangeBills(DateTime dateFrom)
        {
            DateTime dateTo = DateTime.Now;
            this.LoadRangeBills(dateFrom, dateTo);
        }
        private void LoadRangeBills(DateTime dateFrom, DateTime dateTo)
        {
            this.LoadDayBillsBeforeHook();
            double allSuma = 0.0;
            while (dateFrom < dateTo)
            {
                allSuma += this.LoadDayBills(dateFrom);
                dateFrom = dateFrom.AddDays(1.0);
            }
            Hashtable param = new Hashtable();
            param.Add("SUMA", allSuma);
            this.LoadDayBillsAfterHook(param);
        }
        private void LoadDayBillsWithClear(DateTime selectedDay)
        {
            this.LoadDayBillsBeforeHook();
            double allSuma = this.LoadDayBills(selectedDay);
            Hashtable param = new Hashtable();
            param.Add("SUMA", allSuma);
            this.LoadDayBillsAfterHook(param);
        }
        private void LoadDayBillsWithClear()
        {
            this.LoadDayBillsBeforeHook();
            double allSuma = this.LoadDayBills(DateTime.Now);
            Hashtable param = new Hashtable();
            param.Add("SUMA", allSuma);
            this.LoadDayBillsAfterHook(param);
        }
        /// <summary>
        /// Set activity mode to all needle controls
        /// </summary>
        /// <param name="isActive"></param>
        private void ActivityControls(bool isActive)
        {
            if (!isActive)
                billGrid.DataSource = null;

            label2.ResetText();
            button_billsList_Open.Enabled = isActive;
            button_billsList_Print.Enabled = isActive;
            button_billsList_Delete.Enabled = isActive;
            if (!isActive)
            {
                label_orderInfo_suma.Text = string.Empty;
                label_orderInfo_realSuma.Text = string.Empty;
                label_orderInfo_orderNo.Text = string.Empty;
                label_orderInfo_discount.Text = string.Empty;
            }
            //button_billsList_madecopy.Enabled = isActive;
        }
        private void FilterByDate(DateTime filterDateTime, string activeBillOID)
        {
            int hiddenRows = 0;
            int firstVisibleRowNo = -1;
            int activeVisibleRowNo = -1;
            string dateFilter = filterDateTime.ToShortDateString();
            for (int i = 0; i < listGrid.RowCount; i++)
                if (dateFilter == listGrid["DT", i].Value.ToString())
                {
                    //listGrid.Rows[i].Selected = true;
                    listGrid.Rows[i].Visible = true;
                    if (firstVisibleRowNo < 0)
                        firstVisibleRowNo = i;
                    if (activeVisibleRowNo < 0 && listGrid["OID", i].Value.ToString() == activeBillOID)
                    {
                        activeVisibleRowNo = i;
                    }
                    //listGrid.CurrentCell = listGrid["NOM", i];
                }
                else
                {
                    listGrid.Rows[i].Visible = false;
                    hiddenRows++;
                }

            if (firstVisibleRowNo >= 0)
            {
                if (activeVisibleRowNo >= 0)
                {
                    listGrid.Rows[activeVisibleRowNo].Selected = false;
                    listGrid.Rows[activeVisibleRowNo].Selected = true;
                }
                else
                {
                    listGrid.Rows[firstVisibleRowNo].Selected = false;
                    listGrid.Rows[firstVisibleRowNo].Selected = true;
                }
            }

            this.FilterResultHook();
            /*if (hiddenRows == this.listGrid.RowCount)
                this.ActivityControls(false);*/
        }
        private void FilterByNo(string billNumber)
        {
            if (billNumber == string.Empty)
                return;

            int hiddenRows = 0;
            int firstVisibleRowNo = -1;
            for (int i = 0; i < listGrid.RowCount; i++)
                if (listGrid["NOM", i].Value.ToString().Contains(billNumber))
                {
                    //listGrid.Rows[i].Selected = true;
                    listGrid.Rows[i].Visible = true;
                    if (firstVisibleRowNo < 0)
                        firstVisibleRowNo = i;
                    //listGrid.CurrentCell = listGrid["NOM", i];
                }
                else
                {
                    listGrid.Rows[i].Visible = false;
                    hiddenRows++;
                }
            if (firstVisibleRowNo >= 0)
                listGrid.Rows[firstVisibleRowNo].Selected = true;

            this.FilterResultHook();
           /* if (hiddenRows == this.listGrid.RowCount)
                this.ActivityControls(false);*/
        }
        private void FilterReset(string activeBillOID)
        {
            int activeVisibleRowNo = -1;
            for (int i = 0; i < listGrid.RowCount; i++)
            {
                if (activeVisibleRowNo < 0 && listGrid["NOM", i].Value.ToString() == activeBillOID)
                    activeVisibleRowNo = i;
                listGrid.Rows[i].Visible = true;
            }
            if (activeVisibleRowNo >= 0)
                listGrid.Rows[activeVisibleRowNo].Selected = true;
        }
        private void FilterResultHook()
        {
            if (this.listGrid.Rows.GetRowCount(DataGridViewElementStates.Visible) == 0)
                this.ActivityControls(false);
        }

        /* PROPERTIES */

        /// <summary>
        /// Завантажений рахунок
        /// </summary>
        public DataTable LoadedBill { get { return dTBill; } }


        private void fileSystemWatcher_billIFolder_Changed(object sender, FileSystemEventArgs e)
        {
            if (!this.IsDisposed)
            {
                //MMessageBox.Show(this, "Були внесені змінити в базу рахунків\r\nВідбудеться оновлення списку. Натисніть ОК для продовження.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.button_billsList_Refresh.PerformClick();
            }             
        }

        private void uiWndBillList_FormClosing(object sender, FormClosingEventArgs e)
        {
            // saving position
            ConfigManager.Instance.CommonConfiguration.WP_ALL["BILL_VIEW"] = this.Location;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {/*
            if (e.NewValue == 0)
            {
                this.listGrid.FirstDisplayedScrollingRowIndex = e.NewValue;
                this.listGrid.Rows[0].Selected = true;

                return;
            }

            this.listGrid.FirstDisplayedScrollingRowIndex = e.NewValue / 5;
            this.listGrid.Rows[this.listGrid.FirstDisplayedScrollingRowIndex].Selected = true;

            if (e.NewValue >= this.vScrollBar1.Maximum)
            {
                this.listGrid.FirstDisplayedScrollingRowIndex = this.listGrid.RowCount - 1;
                this.listGrid.Rows[this.listGrid.RowCount - 1].Selected = true;
                return;
            }
            */


            this.listGrid.FirstDisplayedScrollingRowIndex = e.NewValue;
            this.listGrid.Rows[e.NewValue].Selected = true;
            this.listGrid[1, e.NewValue].Selected = true;
            //this.listGrid.Rows[e.NewValue]. = DataGridViewElementStates.Selected;

            /*
            if (e.NewValue > e.OldValue && (this.listGrid.FirstDisplayedScrollingRowIndex + 1) < this.listGrid.RowCount)
                this.listGrid.FirstDisplayedScrollingRowIndex++;
            else
                if (e.NewValue < e.OldValue && (this.listGrid.FirstDisplayedScrollingRowIndex - 1) >= 0)
                    this.listGrid.FirstDisplayedScrollingRowIndex--;*/
        }
    }
}