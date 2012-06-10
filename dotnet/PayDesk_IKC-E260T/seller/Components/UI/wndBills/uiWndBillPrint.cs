using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mdcore.Lib;

namespace PayDesk.Components.UI.wndBills
{
    public partial class uiWndBillPrint : Form
    {
        private DataTable billEntry;
        private object[] pData;

        public uiWndBillPrint(DataTable bill)
        {
            this.billEntry = bill;
            InitializeComponent();
            bool billIsLocked = (bool)DataWorkShared.ExtractBillProperty(this.billEntry, mdcore.Common.CoreConst.IS_LOCKED, false);
            button1.Enabled = !billIsLocked;
        }
        /// <summary>
        /// Print bill for kitchen. Strandart function.
        /// If bill is new all items will be printed, otherwise will be printed 
        /// new items only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //Dictionary<string, object> billInfo = mdcore.Lib.DataWorkShared.GetBillStructure(this.billEntry);
            mdcore.Lib.DataWorkOutput.Print(mdcore.Lib.DataWorkOutput.PrinterType.BillKitchen, this.billEntry);
            //mdcore.Lib.CoreLib.Print(new object[] { this.billEntry }, "kitchen", 1);
            mdcore.Lib.DataWorkBill.BillUpdatePrintedCount(this.billEntry);

            // reset deleted rows
            mdcore.Lib.DataWorkShared.ResetBillProperty(this.billEntry, mdcore.Common.CoreConst.DELETED_ROWS);


            //mdcore.Lib.CoreLib.SaveBill(false, 0, "", this.billEntry);
            mdcore.Lib.DataWorkBill.SaveBillToFile(this.billEntry);
            DialogResult = DialogResult.None;
            this.Close();
        }

        /// <summary>
        /// Print bill for client. Standart function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //Dictionary<string, object> billInfo = mdcore.Lib.DataWorkShared.GetBillStructure(this.billEntry);
            mdcore.Lib.DataWorkOutput.Print(mdcore.Lib.DataWorkOutput.PrinterType.Bill, this.billEntry);
            mdcore.Lib.DataWorkBill.BillUpdatePrintedCount(this.billEntry);

            // reset deleted rows
            mdcore.Lib.DataWorkShared.ResetBillProperty(this.billEntry, mdcore.Common.CoreConst.DELETED_ROWS);

            //mdcore.Lib.CoreLib.SaveBill(false, "", "", this.billEntry, "");
            mdcore.Lib.DataWorkBill.LockBill(this.billEntry);
            //mdcore.Lib.CoreLib.LockBill(this.billEntry);
            DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Close current print bill window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_printBill_closeWindow_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.None;
            this.Close();
        }

        private void uiWndBillPrint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                DialogResult = DialogResult.None;
                this.Close();
            }
        }
    }
}
