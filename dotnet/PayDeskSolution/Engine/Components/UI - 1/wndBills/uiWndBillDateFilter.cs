using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PayDesk.Components.UI.wndBills
{
    public partial class uiWndBillDateFilter : Form
    {
        public uiWndBillDateFilter()
        {
            InitializeComponent();
            this.dateTimePicker_from.Value = this.dateTimePicker_from.Value.AddDays(-1.0);
        }

        public DateTime Filter_FromDate
        {
            get { return this.dateTimePicker_from.Value; }
        }

        public DateTime Filter_ToDate
        {
            get { return this.dateTimePicker_to.Value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
