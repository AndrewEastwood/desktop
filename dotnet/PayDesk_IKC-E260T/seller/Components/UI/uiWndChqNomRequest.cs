using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PayDesk.Components.UI
{
    public partial class uiWndChqNomRequest : Form
    {
        public uiWndChqNomRequest()
        {
            InitializeComponent();
            this.Text = Application.ProductName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            uint value = 0;
            if (!uint.TryParse(maskedTextBox1.Text, out value))
                return;

            DialogResult = DialogResult.Yes;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        public uint ChequeNumber
        {
            get
            {
                return uint.Parse(maskedTextBox1.Text);
            }
        }


    }
}