using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    partial class CustomSetTime : Form
    {
        private DateTime _datetime;

        public CustomSetTime()
        {
            InitializeComponent();
        }

        public CustomSetTime(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public CustomSetTime(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void SetDate_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _datetime = new DateTime(DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                dateTimePicker1.Value.Hour,
                dateTimePicker1.Value.Minute,
            dateTimePicker1.Value.Second);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SetDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }

        public DateTime NewDateTime { get { return this._datetime; } }
    }
}