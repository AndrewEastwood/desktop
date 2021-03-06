﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mdcore.Lib;
using mdcore.Config;
using mdcore.Common;

namespace PayDesk.Components.UI.wndBills
{
    public partial class uiWndBillManagercs : Form
    {
        private Dictionary<string, string> billFileList = new Dictionary<string, string>();

        public uiWndBillManagercs()
        {
            InitializeComponent();
            
            billGrid.DataSource = null;
            label_billInfo_State.Text = "-";
            label_orderInfo_suma.Text = "-";
            label_orderInfo_realSuma.Text = "-";
            label_orderInfo_orderNo.Text = "-";
            label_orderInfo_discount.Text = "-";
            
            ShowBills(DateTime.Now, DateTime.Now);

        }

        /* EVENTS */

        // date filtering
        private void dateTimePicker_DateFilter_ValueChanged(object sender, EventArgs e)
        {
            ShowBills(dateTimePicker_StartDate.Value, dateTimePicker_EndDate.Value);
        }

        private void listGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (DialogResult == DialogResult.OK)
                return;
            
            if (listGrid.SelectedRows.Count != 0 && listGrid.SelectedRows[0] != null)
            {
                try
                {
                    DataTable dTBill = DataWorkBill.LoadCombinedBill(listGrid.SelectedRows[0].Cells["ColumnPath"].Value.ToString());
                    billGrid.DataSource = dTBill;
                    //string currentActiveBillOID = DataWorkShared.ExtractBillProperty(dTBill, mdcore.Common.CoreConst.OID).ToString();
                    Dictionary<string, object> billInfo = DataWorkShared.GetBillInfo(dTBill);
                    Dictionary<string, object> orderInfo = DataWorkShared.GetOrderInfo(dTBill);
                    
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
                    //double chqSUMA = (double)dTBill.ExtendedProperties[mdcore.Common.CoreConst.ORDER_REAL_SUMA];
                    //label2.Text = string.Format("{0} {1} {2} {3:0.00}{4}", "Перегляд рахунку №", billInfo[mdcore.Common.CoreConst.BILL_NO], "на суму:", chqSUMA, "грн.");
                    this.billGrid.CurrentCell = null;
                    bool billIsClosed = DataWorkShared.ExtractOrderProperty(dTBill, mdcore.Common.CoreConst.ORDER_NO, string.Empty, false).ToString() != string.Empty;
                    bool billIsLocked = (bool)DataWorkShared.ExtractBillProperty(dTBill, mdcore.Common.CoreConst.IS_LOCKED, false);
                    string orderNo = DataWorkShared.ExtractOrderProperty(dTBill, mdcore.Common.CoreConst.ORDER_NO, string.Empty).ToString();
                    string billState = string.Empty;
                    switch (orderNo)
                    {
                        case "null": { billState = "Рахунок АНУЛЬОВАНИЙ"; orderNo = "Нема"; break; }
                        case "copy": { billState = "Створена копія рахунку."; orderNo = "Нема"; break; }
                    }

                    if (orderNo == string.Empty)
                        orderNo = "Нема";

                    if (billIsLocked && !billIsClosed)
                        billState += "   Рахунок надрукований клієнту. Зробіть чек! ";

                    if (!billIsLocked && !billIsClosed)
                        billState += "   Доступний для редагування.";
                    else
                        billState += "   Рахунок закритий.";

                    label_billInfo_State.Text = billState.Trim();
                    label_orderInfo_suma.Text = string.Format("{0}{1} {2}", "Сума без знижок", ":", DataWorkShared.ExtractOrderProperty(dTBill, mdcore.Common.CoreConst.ORDER_SUMA));
                    label_orderInfo_realSuma.Text = string.Format("{0}{1} {2}", "СУМА", ":", DataWorkShared.ExtractOrderProperty(dTBill, mdcore.Common.CoreConst.ORDER_REAL_SUMA));
                    label_orderInfo_orderNo.Text = string.Format("{0}{1} {2}", "Номер чеку", ":", orderNo);
                    label_orderInfo_discount.Text = string.Empty;
                }
                catch { }
            }
            else
            {
                billGrid.DataSource = null;
                label_billInfo_State.Text = "-";
                label_orderInfo_suma.Text = "-";
                label_orderInfo_realSuma.Text = "-";
                label_orderInfo_orderNo.Text = "-";
                label_orderInfo_discount.Text = "-";
            }

        }


        private void listGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        // enabling context menu for rows only
        private void listGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.Button == MouseButtons.Right)
                    this.contextMenuStrip1.Show(Control.MousePosition);
                if (e.Button == MouseButtons.Left && e.ColumnIndex == this.listGrid.Columns["ColumnSelect"].Index)
                    this.listGrid["ColumnSelect", e.RowIndex].Value = !(bool)this.listGrid["ColumnSelect", e.RowIndex].Value;
                (sender as DataGridView).CurrentCell = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
            }
        }

        // context menu event handler
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null) return;
            switch (e.ClickedItem.Tag.ToString())
            {
                case "ItemSelectOne":
                    {
                        try
                        {
                            this.listGrid.SelectedRows[0].Cells["ColumnSelect"].Value = true;
                        }
                        catch { }
                        break;
                    }
                case "ItemDeselectOne":
                    {
                        try
                        {
                            this.listGrid.SelectedRows[0].Cells["ColumnSelect"].Value = false;
                        }
                        catch { }
                        break;
                    }
                case "ItemDelete":
                    {
                        try
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(this.listGrid.SelectedRows[0].Cells["ColumnPath"].Value.ToString(), Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                            ShowBills(dateTimePicker_StartDate.Value, dateTimePicker_EndDate.Value);
                        }
                        catch { }
                        break;
                    }
                case "ItemDeleteChecked":
                    {
                        for (int i = 0; i < this.listGrid.RowCount; i++)
                        {
                            try
                            {
                                if ((bool)this.listGrid["ColumnSelect", i].Value)
                                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(this.listGrid["ColumnPath", i].Value.ToString(), Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                            }
                            catch { }
                        }
                        ShowBills(dateTimePicker_StartDate.Value, dateTimePicker_EndDate.Value);
                        break;
                    }
                case "ItemCheckAll":
                    {
                        for (int i = 0; i < this.listGrid.RowCount; i++)
                        {
                            this.listGrid["ColumnSelect", i].Value = true;
                            //this.listGrid.Rows[i].Selected = true;
                        }
                        break;
                    }
                case "ItemDeselectAll":
                    {
                        for (int i = 0; i < this.listGrid.RowCount; i++)
                        {
                            this.listGrid["ColumnSelect", i].Value = false;
                            //this.listGrid.Rows[i].Selected = false;
                        }
                        break;
                    }
            }
        }

        /* METHODS */
        private double ShowBills(DateTime fromDay, DateTime toDay)
        {
            Dictionary<string, object> items = DataWorkBill.LoadRangeBills(fromDay, toDay, AppConfig.Path_Bills, AppConfig.APP_SubUnit);
            DataTable currentBill = new DataTable();
            PropertyCollection props = new PropertyCollection();
            Dictionary<string, object> billInfo = new Dictionary<string, object>();
            double generalSuma = 0.0;
            //this.billFileList.Clear();
            listGrid.Rows.Clear();
            foreach (KeyValuePair<string, object> billEntry in items)
            {
                currentBill = (DataTable)((object[])billEntry.Value)[0];
                props = (PropertyCollection)((object[])billEntry.Value)[1];
                billInfo = ((Dictionary<string, object>)props[CoreConst.BILL]);
                //this.billFileList.Add(billInfo[CoreConst.OID].ToString(), billEntry.Key);

                if (props[CoreConst.ORDER_NO] == null)
                    continue;

                listGrid.Rows.Add(
                    new object[] {
                            billInfo[CoreConst.OID],
                            billEntry.Key,
                            false, 
                            billInfo[CoreConst.BILL_NO],
                            billInfo[CoreConst.DATETIME],
                            billInfo[CoreConst.COMMENT], 
                            (double)props[mdcore.Common.CoreConst.ORDER_SUMA], 
                            bool.Parse(billInfo[CoreConst.IS_LOCKED].ToString()), 
                            props[CoreConst.ORDER_NO]
                        }
                );
                generalSuma += (double)props[mdcore.Common.CoreConst.ORDER_SUMA];
                /*if (props.ContainsKey(CoreConst.ORDER_NO) && props[CoreConst.ORDER_NO] != null && props[CoreConst.ORDER_NO].ToString() != string.Empty)
                {
                    Font extFont = listGrid.Font;
                    listGrid.Rows[listGrid.Rows.Count - 1].DefaultCellStyle.Font = new Font(extFont, FontStyle.Strikeout);
                }*/
            }

            this.label_orderInfo_General.Text = string.Format("Всього {0} запис(ів) на суму {1:0.00}{2}", listGrid.RowCount, generalSuma, "грн");

            return generalSuma;
        }

    }
}
