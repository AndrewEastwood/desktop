using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO_FPU550.DriverUI
{
    public partial class ReportByNoFull : Form
    {
        private string _pwd;
        private uint _startNo;
        private uint _endNo;

        //Constructors
        public ReportByNoFull()
        {
            InitializeComponent();
        }
        public ReportByNoFull(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public ReportByNoFull(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        //Events
        private void ReportByNoFull_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void ReportByNoFull_KeyDown(object sender, KeyEventArgs e)
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
                _startNo = (uint)numericUpDown1.Value;
                _endNo = (uint)numericUpDown2.Value;
            }
            catch { return; }
            DialogResult = DialogResult.OK;
            Close();
        }

        //Properties
        public string Password { get { return _pwd; } }
        public uint StartNo { get { return _startNo; } }
        public uint EndNo { get { return _endNo; } }


    }
}