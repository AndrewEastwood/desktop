using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    public partial class ReportByDateFull : Form
    {
        private string _pwd;
        private DateTime _startDate;
        private DateTime _endDate;

        //Constructors
        public ReportByDateFull()
        {
            InitializeComponent();
        }
        public ReportByDateFull(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public ReportByDateFull(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        //Events
        private void ReportByDateFull_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void ReportByDateFull_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Length < 4)
                    return;

                _pwd = textBox1.Text;
                _startDate = dateTimePicker1.Value;
                _endDate = dateTimePicker2.Value;
            }
            catch { return; }
            DialogResult = DialogResult.OK;
            Close();
        }

        //Properties
        public string Password { get { return _pwd; } }
        public DateTime StartDate { get { return _startDate; } }
        public DateTime EndDate { get { return _endDate; } }
    }
}