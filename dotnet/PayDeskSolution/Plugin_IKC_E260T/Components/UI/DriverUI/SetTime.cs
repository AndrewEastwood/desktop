using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IKC_E260T.Components.UI.DriverUI
{
    partial class SetTime : Form
    {
        public DateTime time;

        public SetTime()
        {
            InitializeComponent();
        }
        public SetTime(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetTime(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void SetTime_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                time = dateTimePicker1.Value;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void SetTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}