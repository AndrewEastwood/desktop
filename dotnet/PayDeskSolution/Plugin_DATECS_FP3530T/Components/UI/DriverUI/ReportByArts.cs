using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    public partial class ReportByArts : Form
    {
        private string _pwd;
        private string _mode;

        //Constructors
        public ReportByArts()
        {
            InitializeComponent();
        }
        public ReportByArts(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public ReportByArts(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        //Events
        private void ReportByArts_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            _mode = "S";
        }
        private void ReportByArts_KeyDown(object sender, KeyEventArgs e)
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
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            switch (((RadioButton)sender).Tag.ToString())
            {
                case "1":
                    {
                        _mode = "S";
                        break;
                    }
                case "2":
                    {
                        _mode = "P";
                        break;
                    }
                case "3":
                    {
                        _mode = "P#";
                        break;
                    }
                case "4":
                    {
                        _mode = "G";
                        break;
                    }
                case "5":
                    {
                        _mode = "G#";
                        break;
                    }
            }
        }

        //Properties
        public string Password { get { return _pwd; } }
        public string ReportMode { get { return _mode; } }
    }
}