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
using components.Public;

namespace PayDesk.Components.UI.wndBills
{
    public partial class uiWndBillList : Form
    {

        private Hashtable _localConfig;
        //private FileStream stream; .Components.UI.wndBills
        //private BinaryFormatter binF = new BinaryFormatter();
        private DataTable dTBill = new DataTable();
        private DataTable dTList = new DataTable();
        private string loadedBillNo;
        //private double billListSuma;
        //private string[] bills;
        private Dictionary<string, string> billFileList = new Dictionary<string, string>();
        //private int currentActiveRowIndex = -1;
        private string currentActiveBillOID = string.Empty;
        //private DateTime currentDateTimeFilter = new DateTime();
        bool _blockFilterAction;

        /* CONSTRUCTORS */

        /// <summary>
        /// 
        /// </summary>
        public uiWndBillList()
        {
            InitializeComponent();

            _localConfig = ApplicationConfiguration.Instance.GetValueByKey<Hashtable>("dinningRoom");

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

            // update filter dropdowns
            string pattern = _localConfig["patternBarOrderComment"].ToString();
            string[] sections = pattern.Split(new char[] { ' ' });
            foreach (string s in sections)
            {
                try
                {// 
                    List<string> _filterOptions = new List<string>();
                    foreach (DictionaryEntry section in (Hashtable)_localConfig[s])
                    {
                        try
                        {
                            if (section.Key.ToString() == "items")
                                foreach (DictionaryEntry singleItem in (Hashtable)section.Value)
                                    _filterOptions.Add(singleItem.Value.ToString());
                            if (section.Key.ToString() == "active" && bool.Parse(section.Value.ToString()))
                            {
                                switch (s.ToLower())
                                {
                                    case "waiters":
                                        filterBy_billWaiter.Items.AddRange(_filterOptions.ToArray());
                                        filterBy_billWaiter.Enabled = true;
                                        break;
                                    case "desknumbers":
                                        filterBy_billDesk.Items.AddRange(_filterOptions.ToArray());
                                        filterBy_billDesk.Enabled = true;
                                        break;
                                    case "orderumbers":
                                        filterBy_billTag.Items.AddRange(_filterOptions.ToArray());
                                        filterBy_billTag.Enabled = true;
                                        break;
                                }
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
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
            // Print
            if (e.KeyValue == new KeyEventArgs(Keys.P).KeyValue && e.Alt)
            {
                button_billsList_Print.PerformClick();
                return;
            }
            // Delete
            if (e.KeyValue == new KeyEventArgs(Keys.D).KeyValue && e.Alt)
            {
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

                    bool billIsLoaded = DataWorkShared.ExtractBillProperty(dTBill, CoreConst.BILL_NO, string.Empty).ToString() == this.loadedBillNo;
                    bool billIsClosed = DataWorkShared.ExtractOrderProperty(dTBill, CoreConst.ORDER_NO, string.Empty, false).ToString() != string.Empty;
                    bool billIsLocked =(bool)DataWorkShared.ExtractBillProperty(dTBill, CoreConst.IS_LOCKED, false);

                    button_billsList_Open.Enabled = !billIsClosed && !billIsLoaded;
                    button_billsList_Print.Enabled = !billIsClosed;// && !billIsLocked;
                    button_billsList_Delete.Enabled = !billIsClosed && !billIsLocked && !billIsLoaded;
                    button_billsList_madecopy.Enabled = !billIsClosed && billIsLocked && !billIsLoaded;
                    //button_billsList_unlock.Visible = false;
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
                }
                catch { }

                // shif v-scrollbar
                try
                {
                    if (vScrollBar1.Maximum < rIdx)
                        vScrollBar1.Value = vScrollBar1.Maximum;
                    else
                        vScrollBar1.Value = rIdx;
                }
                catch { }

            }
            else
            {
                this.ActivityControls(false);
            }

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
            this.LoadDayBillsWithClear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_billsList_Delete_Click(object sender, EventArgs e)
        {
            if (this.loadedBillNo == DataWorkShared.ExtractBillProperty(this.dTBill, CoreConst.BILL_NO, string.Empty).ToString())
                MMessageBox.Show("Неможливо анулювати рахунок № " + this.loadedBillNo + "\r\nВін є відкритий в основному вікні",
                      Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                DataWorkBill.LockBill(dTBill, "null");
                this.LoadDayBillsWithClear();
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
                ;// this.FilterByNo(textBox_billsList_billNoSearch.Text);
        }
        /// <summary>
        /// Refresh (read) all bill files from folder and apply latest filter by date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_billsList_Refresh_Click(object sender, EventArgs e)
        {
            //this.textBox_billsList_billNoSearch.Text = this.textBox_billsList_billNoSearch.Tag.ToString();
            //this.textBox_billsList_billNoSearch.Refresh();
            this.billGrid.Select();
            this.billGrid.Update();
            this.LoadDayBillsWithClear();

            _blockFilterAction = true;
            filterBy_billNo.ResetText();
            filterBy_billDesk.ResetText();
            filterBy_billWaiter.ResetText();
            filterBy_billTag.ResetText();
            _blockFilterAction = false;
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
            this._loadDayBills();
            //this.FilterByDate(this.dateTimePicker1.Value, this.currentActiveBillOID);
        }
        /// <summary>
        /// Download another bills (will open date range picker before)
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
        /// Do 
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
        /// Watch bill folder for changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileSystemWatcher_billIFolder_Changed(object sender, FileSystemEventArgs e)
        {
            if (!this.IsDisposed)
            {
                //MMessageBox.Show(this, "Були внесені змінити в базу рахунків\r\nВідбудеться оновлення списку. Натисніть ОК для продовження.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.button_billsList_Refresh.PerformClick();
            }
        }
        /// <summary>
        /// Do smth before window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiWndBillList_FormClosing(object sender, FormClosingEventArgs e)
        {
            // saving position
            ConfigManager.Instance.CommonConfiguration.WP_ALL["BILL_VIEW"] = this.Location;
        }
        /// <summary>
        /// Controls vertical scroll bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            this.listGrid.FirstDisplayedScrollingRowIndex = e.NewValue;
            this.listGrid.Rows[e.NewValue].Selected = true;
            this.listGrid[1, e.NewValue].Selected = true;
        }
        /// <summary>
        /// Manage filtering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filterBy_TextChanged(object sender, EventArgs e)
        {
            if (_blockFilterAction)
                return;

            string _filterStrBy_billNo = filterBy_billNo.Text;
            string _filterStrBy_billComment = "";

            string pattern = _localConfig["patternBarOrderComment"].ToString();
            string[] sections = pattern.Split(new char[] { ' ' });
            foreach (string s in sections)
            {
                switch (s.ToLower())
                {
                    case "waiters":
                        _filterStrBy_billComment += filterBy_billWaiter.Text;
                        break;
                    case "desknumbers":
                        _filterStrBy_billComment += filterBy_billDesk.Text;
                        break;
                    case "orderumbers":
                        _filterStrBy_billComment += filterBy_billTag.Text;
                        break;
                }
                _filterStrBy_billComment += " ";
            }

            _filterStrBy_billComment = _filterStrBy_billComment.Trim();
            _filterStrBy_billNo = _filterStrBy_billNo.Trim();


            FilterByNo(_filterStrBy_billNo);
            FilterByComment(_filterStrBy_billComment, true);

            //switch (((Control)sender).Tag.ToString())
            //{
            //    case "filterBy_billNo":
            //        break;
            //    case "filterBy_billWaiter":
            //        break;
            //    case "filterBy_billDesk":
            //        break;
            //    case "filterBy_billTag":
            //        break;
            //}
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
        /// Load all bills from bill's directory. It is the basic method for "range" and 'clear day" bills loaders
        /// </summary>
        private double _loadDayBills()
        {
            return this._loadDayBills(DateTime.Now);
        }
        private double _loadDayBills(DateTime selectedDay)
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
                            ((DateTime)billInfo[CoreConst.DATETIME]).ToString("mm.dd.yy hh:mm"),
                            (billInfo[CoreConst.COMMENT] != null)?billInfo[CoreConst.COMMENT].ToString().Replace("%20", " "):"", 
                            (double)props[CoreConst.ORDER_REAL_SUMA], 
                            bool.Parse(billInfo[CoreConst.IS_LOCKED].ToString()), 
                            props[CoreConst.PAYDESK_NO], 
                            props[CoreConst.ORDER_NO],
                            (billInfo.ContainsKey(CoreConst.DATETIME_LOCK) && billInfo[CoreConst.DATETIME_LOCK] is DateTime ? ((DateTime)billInfo[CoreConst.DATETIME_LOCK]).ToString("mm.dd.yy hh:mm") : "-")
                        }
                    );

                    generalSuma += (double)props[CoreConst.ORDER_REAL_SUMA];
                    if (props.ContainsKey(CoreConst.ORDER_NO) && props[CoreConst.ORDER_NO] != null && props[CoreConst.ORDER_NO].ToString() != string.Empty)
                    {
                        Font extFont = listGrid.Font;
                        listGrid.Rows[listGrid.Rows.Count - 1].DefaultCellStyle.Font = new Font(extFont, FontStyle.Strikeout);
                        listGrid.Rows[listGrid.Rows.Count - 1].DefaultCellStyle.BackColor = Color.GreenYellow;
                    } else if (bool.Parse(billInfo[CoreConst.IS_LOCKED].ToString()))
                        listGrid.Rows[listGrid.Rows.Count - 1].DefaultCellStyle.BackColor = Color.DarkOrange;
                    else
                        listGrid.Rows[listGrid.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightPink;

                }
            }
            catch { }

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
                // update filter box
                this.filterBy_billNo.Items.Clear();
                this.filterBy_billNo.Enabled = true;
                for (int i = 0; i < listGrid.RowCount; i++)
                    this.filterBy_billNo.Items.Add(listGrid["NOM", i].Value.ToString().Trim('0'));

                // select active bill
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
                }

                this.button_billsList_Open.Enabled = true;
                this.listGrid.CurrentCell = this.listGrid[1, index];
            }
            else
            {
                this.button_billsList_Open.Enabled = false;
                // disable filter box by num
                this.filterBy_billNo.Enabled = false;
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
                allSuma += this._loadDayBills(dateFrom);
                dateFrom = dateFrom.AddDays(1.0);
            }
            Hashtable param = new Hashtable();
            param.Add("SUMA", allSuma);
            this.LoadDayBillsAfterHook(param);
        }
        private void LoadDayBillsWithClear(DateTime selectedDay)
        {
            this.LoadDayBillsBeforeHook();
            double allSuma = this._loadDayBills(selectedDay);
            Hashtable param = new Hashtable();
            param.Add("SUMA", allSuma);
            this.LoadDayBillsAfterHook(param);
        }
        private void LoadDayBillsWithClear()
        {
            LoadDayBillsWithClear(DateTime.Now);
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
            FilterByNo(billNumber, false);
        }
        private void FilterByNo(string billNumber, bool isCascadeFilter)
        {
            int hiddenRows = 0;
            int firstVisibleRowNo = -1;
            for (int i = 0; i < listGrid.RowCount; i++)
                if (billNumber == string.Empty || listGrid["NOM", i].Value.ToString().Contains(billNumber))
                {
                    //listGrid.Rows[i].Selected = true;
                    if (isCascadeFilter && !listGrid.Rows[i].Visible)
                        continue;

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
            //if (firstVisibleRowNo >= 0)
            //    listGrid.Rows[firstVisibleRowNo].Selected = true;

            this.FilterResultHook();
           /* if (hiddenRows == this.listGrid.RowCount)
                this.ActivityControls(false);*/
        }
        private void FilterByComment(string partial)
        {
            FilterByComment(partial, false);
        }
        private void FilterByComment(string partial, bool isCascadeFilter)
        {
            int hiddenRows = 0;
            int firstVisibleRowNo = -1;
            for (int i = 0; i < listGrid.RowCount; i++)
                if (partial == string.Empty || listGrid["CMT", i].Value.ToString().Contains(partial))
                {
                    if (isCascadeFilter && !listGrid.Rows[i].Visible)
                        continue;
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
            //if (firstVisibleRowNo >= 0)
            //    listGrid.Rows[firstVisibleRowNo].Selected = true;

            this.FilterResultHook();
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
    }
}