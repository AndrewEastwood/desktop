using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.DATECSFP2530T
{
    public partial class PeriodicReportShort : Form
    {
        public DateTime startDate;
        public DateTime endDate;

        public PeriodicReportShort()
        {
            InitializeComponent();
        }
        public PeriodicReportShort(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public PeriodicReportShort(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void PeriodicReportShort_Load(object sender, EventArgs e)
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

        private void PeriodicReportShort_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}