using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IKC_OP2.Components.UI.DriverUI
{
    public partial class PeriodicReport2 : Form
    {
        public uint startNo;
        public uint endNo;

        public PeriodicReport2()
        {
            InitializeComponent();
        }
        public PeriodicReport2(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public PeriodicReport2(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void PeriodicReport2_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                startNo = (uint)numericUpDown1.Value;
                endNo = (uint)numericUpDown2.Value;
            }
            catch { return; }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void PeriodicReport2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}