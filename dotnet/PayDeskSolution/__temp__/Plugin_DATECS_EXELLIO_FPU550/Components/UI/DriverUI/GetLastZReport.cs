using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO_FPU550.DriverUI
{
    public partial class GetLastZReport : Form
    {
        private byte _mode;

        //Constructors
        public GetLastZReport()
        {
            InitializeComponent();
        }
        public GetLastZReport(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public GetLastZReport(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        //Events
        private void GetLastZReport_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            _mode = 0;
        }
        private void GetLastZReport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            _mode = byte.Parse(((RadioButton)sender).Tag.ToString());
        }

        //Properties
        public byte ReportMode { get { return _mode; } }
    }
}