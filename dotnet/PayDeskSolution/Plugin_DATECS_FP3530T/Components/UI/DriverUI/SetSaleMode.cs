using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    public partial class SetSaleMode : Form
    {
        private byte _mode;

        // Counstructrs
        public SetSaleMode()
        {
            InitializeComponent();
        }
        public SetSaleMode(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetSaleMode(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void SetSaleMode_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            comboBox1.SelectedIndex = 0;
        }
        private void SetSaleMode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _mode = (byte)comboBox1.SelectedIndex;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        // Properties
        public byte SaleMode { get { return _mode; } }
    }
}