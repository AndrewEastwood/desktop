using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using components.Lib;

namespace IKC_E260T.Components.UI.DriverUI
{
    partial class Give : Form
    {
        public uint copecks = 0;

        public Give()
        {
            InitializeComponent();
        }
        public Give(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public Give(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void Give_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CoreLib fn = new CoreLib();
                copecks = Convert.ToUInt32(fn.GetDouble(textBox1.Text) * 100);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void Give_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}