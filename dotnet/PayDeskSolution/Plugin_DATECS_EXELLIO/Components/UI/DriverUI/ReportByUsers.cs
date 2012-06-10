using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    public partial class ReportByUsers : Form
    {
        private string _pwd;

        //Constructors
        public ReportByUsers()
        {
            InitializeComponent();
        }
        public ReportByUsers(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public ReportByUsers(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        //Events
        private void ReportByUsers_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void ReportByUsers_KeyDown(object sender, KeyEventArgs e)
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
            }
            catch { return; }
            DialogResult = DialogResult.OK;
            Close();
        }

        //Properties
        public string Password { get { return _pwd; } }
    }
}