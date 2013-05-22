using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IKC_OP2.Components.UI.DriverUI
{
    public partial class ChangeRate : Form
    {
        public byte rate;

        public ChangeRate()
        {
            InitializeComponent();
        }
        public ChangeRate(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public ChangeRate(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void ChangeRate_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rate = (byte)comboBox1.SelectedIndex;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ChangeRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}