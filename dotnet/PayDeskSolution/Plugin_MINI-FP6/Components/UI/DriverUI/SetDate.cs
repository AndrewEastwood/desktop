using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MINI_FP6.Components.UI.DriverUI
{
    partial class SetDate : Form
    {
        public DateTime date;

        public SetDate()
        {
            InitializeComponent();
        }

        public SetDate(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetDate(string caption, string desc)
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
            date = monthCalendar1.SelectionStart;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SetDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }   
    }
}