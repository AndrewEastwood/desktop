using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mdcore;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using mdcore.Lib;

namespace PayDesk.Components.UI.wndBills
{
    public partial class BillSave : Form
    {
        //таблиця рахунку  
        private DataTable dtBill;
        private Dictionary<string, object> billInfoStructure;
        //номер рахунку
        private string billNo;
        //Якщо true то рахунок є новим інакше рахунок вже був збережений
        private bool isNewBill;
        private bool needCleanup;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dTable">Таблиця рахунку</param>
        /// 
        public BillSave(DataTable dTable)
        {
            InitializeComponent();

            isNewBill = !dTable.ExtendedProperties.Contains("BILL") || dTable.ExtendedProperties["BILL"] == null;
            if (isNewBill)
                billNo = DataWorkBill.GetNextBillID();
            else
            {
                billNo = ((Dictionary<string , object>)dTable.ExtendedProperties["BILL"])["BILL_NO"].ToString();
                richTextBox1.Text = ((Dictionary<string, object>)dTable.ExtendedProperties["BILL"])["COMMENT"].ToString();
            }
            this.dtBill = dTable.Copy();
            this.needCleanup = false;
            Text += " " + billNo.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dTable"></param>
        /// <param name="clearInfo"></param>
        public BillSave(DataTable dTable, bool clearInfo)
            : this(dTable)
        {
            this.needCleanup = clearInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dTable"></param>
        /// <param name="billInfo"></param>
        public BillSave(DataTable dTable, Dictionary<string, object> billInfo)
            : this(dTable)
        {
            this.billInfoStructure = billInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dTable"></param>
        /// <param name="clearInfo"></param>
        /// <param name="billInfo"></param>
        public BillSave(DataTable dTable, bool clearInfo, Dictionary<string ,object> billInfo)
            : this(dTable, clearInfo)
        {
            this.billInfoStructure = billInfo;
        }

        /// <summary>
        /// Виконує збереження рахунку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length == 0)
            {
                mdcore.Components.UI.MMessageBox.Show("Введіть коментар рахунку", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DataWorkBill.SaveBill(isNewBill, billNo, richTextBox1.Text, ref this.dtBill))
            {/*
                if (this.needCleanup)
                    dtBill.ExtendedProperties.Clear();*/
                DialogResult = DialogResult.OK;
            }

            Close();
        }

        /// <summary>
        /// Обробник клавіатури
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BillRequets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                Close();
            }
            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
            {
                button1.PerformClick();
            }
        }
    
    
        /* Properties */
        /// <summary>
        /// 
        /// </summary>
        public string GetNewBillNo { get { return this.billNo.PadLeft(5, '0'); } }
        /// <summary>
        /// 
        /// </summary>
        public bool IsNewBill { get { return this.isNewBill; } }
        /// <summary>
        /// Saved bill object
        /// </summary>
        public DataTable SavedBill { get { return this.dtBill; } }
        public object SavedBillInfoStructure { get { return this.dtBill.ExtendedProperties; } }
    }
}