using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PayDesk.Components.UI.wndSettings
{
    public partial class uiWndSettingsNumberRequest : Form
    {
        private int v;

        public uiWndSettingsNumberRequest()
        {
            InitializeComponent();
            v = 0;
        }

        private void uiWndSettingsNumberRequest_Load(object sender, EventArgs e)
        {
            this.numericUpDown1.Value = v;
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

        public int Value { get { return (int)this.numericUpDown1.Value; } set { v = value; } }
    }
}
