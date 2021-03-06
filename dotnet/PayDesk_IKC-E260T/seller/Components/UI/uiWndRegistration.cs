using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
/* internal */
using mdcore.Components.UI;
using mdcore.Lib;

namespace PayDesk.Components.UI
{
    public partial class uiWndRegistration : Form
    {
        public uiWndRegistration()
        {
            InitializeComponent();
            maskedTextBox_uiWndReg_PublicCode.Text = new sgmode.ClassMode().getPublicNumber();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new sgmode.ClassMode().setClientCode(maskedTextBox_uiWndReg_ClientCode.Text);
            if (maskedTextBox_uiWndReg_ClientCode.Text != string.Empty)
                DialogResult = DialogResult.OK;
            else
                DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}