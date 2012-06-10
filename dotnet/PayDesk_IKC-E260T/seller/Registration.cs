using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PayDesk
{
    public partial class Registration : Form
    {
        public Registration()
        {
            InitializeComponent();
            textBox6.Text = mdcore.Active.MakeSerial();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mdcore.Active.SetGetState(textBox1.Text);
            if (textBox1.Text != string.Empty)
                DialogResult = DialogResult.OK;
            else
                DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}