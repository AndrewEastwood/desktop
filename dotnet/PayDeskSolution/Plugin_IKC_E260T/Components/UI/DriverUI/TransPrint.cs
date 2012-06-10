using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IKC_E260T.Components.UI.DriverUI
{
    public partial class TransPrint : Form
    {
        public string text;
        public bool endPrint;

        public TransPrint()
        {
            InitializeComponent();
        }
        public TransPrint(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public TransPrint(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void TransPrint_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                text = richTextBox1.Text.Replace('³', 'i').Replace('²', 'I');
                endPrint = checkBox1.Checked;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void TransPrint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}