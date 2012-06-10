using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.IKSE260T
{
    public partial class PeriodicReport : Form
    {
        public DateTime startDate;
        public DateTime endDate;

        public PeriodicReport()
        {
            InitializeComponent();
        }
        public PeriodicReport(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public PeriodicReport(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void PeriodicReport_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                startDate = dateTimePicker1.Value;
                endDate = dateTimePicker2.Value;
            }
            catch { return; }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void PeriodicReport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}