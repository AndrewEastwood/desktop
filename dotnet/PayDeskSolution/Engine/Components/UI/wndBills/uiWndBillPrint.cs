using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using driver.Lib;
using driver.Common;
using driver.Config;
//0using ;
//0using ;

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

            // restore position
            try
            {
                this.Location = ((Point)ConfigManager.Instance.CommonConfiguration.WP_ALL["BILL_PRN"]);
                this.StartPosition = FormStartPosition.Manual;
            }
            catch
            {
                if (ConfigManager.Instance.CommonConfiguration.WP_ALL == null)
                    ConfigManager.Instance.CommonConfiguration.WP_ALL = new System.Collections.Hashtable();
                // saving position
                ConfigManager.Instance.CommonConfiguration.WP_ALL["BILL_PRN"] = this.Location;
            }

            bool billIsLocked = (bool)DataWorkShared.ExtractBillProperty(this.billEntry, CoreConst.IS_LOCKED, false);
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
            //Dictionary<string, object> billInfo = DataWorkShared.GetBillStructure(this.billEntry);
            DataWorkOutput.Print(Enums.PrinterType.BillKitchen, this.billEntry);
            //.CoreLib.Print(new object[] { this.billEntry }, "kitchen", 1);
           DataWorkBill.BillUpdatePrintedCount(this.billEntry);

            // reset deleted rows
            DataWorkShared.ResetBillProperty(this.billEntry, CoreConst.DELETED_ROWS);


            //.CoreLib.SaveBill(false, 0, "", this.billEntry);
           DataWorkBill.SaveBillToFile(this.billEntry);
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
            //Dictionary<string, object> billInfo = DataWorkShared.GetBillStructure(this.billEntry);
            DataWorkOutput.Print(Enums.PrinterType.BillCustomer, this.billEntry);
            DataWorkBill.BillUpdatePrintedCount(this.billEntry);

            // reset deleted rows
            DataWorkShared.ResetBillProperty(this.billEntry, CoreConst.DELETED_ROWS);

            //.CoreLib.SaveBill(false, "", "", this.billEntry, "");
            DataWorkBill.LockBill(this.billEntry);
            //.CoreLib.LockBill(this.billEntry);
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

        private void uiWndBillPrint_FormClosing(object sender, FormClosingEventArgs e)
        {
            // saving position
            ConfigManager.Instance.CommonConfiguration.WP_ALL["BILL_PRN"] = this.Location;
        }
    }
}
