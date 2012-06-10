using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IKC_E260T.Components.UI.DriverUI
{
    partial class SendCustomer : Form
    {
        public string[] lines;
        public bool[] show;

        public SendCustomer()
        {
            InitializeComponent();
        }
        public SendCustomer(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SendCustomer(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void SendCustomer_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                lines = new string[2];
                show = new bool[2];
                lines[0] = textBox1.Text.Replace('³', 'i').Replace('²', 'I');
                lines[1] = textBox2.Text.Replace('³', 'i').Replace('²', 'I');
                show[0] = checkBox1.Checked;
                show[1] = checkBox2.Checked;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = checkBox2.Checked;
        }

        private void SendCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}