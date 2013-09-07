using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MINI_FP6.Components.UI.DriverUI
{
    public partial class SetString : Form
    {
        public string[] lines = new string[2];

        public SetString()
        {
            InitializeComponent();
        }
        public SetString(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetString(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void SetString_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                lines[0] = textBox1.Text.Replace('³', 'i').Replace('²', 'I');
                lines[1] = textBox2.Text.Replace('³', 'i').Replace('²', 'I');
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void SetString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}