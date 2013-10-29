using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    public partial class ReportByDateShort : Form
    {
        private string _pwd;
        private DateTime _startDate;
        private DateTime _endDate;

        //Constructors
        public ReportByDateShort()
        {
            InitializeComponent();
        }
        public ReportByDateShort(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public ReportByDateShort(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        //Events
        private void ReportByDateShort_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void ReportByDateShort_KeyDown(object sender, KeyEventArgs e)
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

        public void SetPassword(string password)
        {
            this.textBox1.Enabled = !(password != null && password.Length > 0);
            this.textBox1.Text = password;
        }

        //Properties
        public string Password { get { return _pwd; } }
        public DateTime StartDate { get { return _startDate; } }
        public DateTime EndDate { get { return _endDate; } }
    }
}