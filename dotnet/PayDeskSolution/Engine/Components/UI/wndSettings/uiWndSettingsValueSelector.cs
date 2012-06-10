using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PayDesk.Components.UI.wndSettings
{
    public partial class uiWndSettingsValueSelector : Form
    {

        private object v;

        public uiWndSettingsValueSelector()
        {
            InitializeComponent();
        }


        private void uiWndSettingsValueSelector_Load(object sender, EventArgs e)
        {
            this.comboBox1.Text = v.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void uiWndSettingsNumberRequest_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();
            }
        }

        public object Value { get { return this.comboBox1.Text; } set { v = value; } }

        public void addValues(object[] values)
        {
            this.comboBox1.Items.AddRange(values);
        }

        public void resetRange()
        {
            this.comboBox1.Items.Clear();
        }
    }
}
