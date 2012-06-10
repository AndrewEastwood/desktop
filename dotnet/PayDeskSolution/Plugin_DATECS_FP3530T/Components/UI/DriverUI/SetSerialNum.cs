using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    internal partial class SetSerialNum : Form
    {
        private byte _ccode;
        private string _scode;

        // Constructor
        public SetSerialNum()
        {
            InitializeComponent();
        }
        public SetSerialNum(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetSerialNum(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void SetPrintVer_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            _ccode = 2;
            _scode = "";
        }
        private void SetPrintVer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (maskedTextBox1.Text.Length != 10)
                    return;

                _scode = maskedTextBox1.Text;
                _ccode = (byte)numericUpDown1.Value;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }
    
        // Properties
        public byte CountryCode { get { return _ccode; } }
        public string SerialNumber { get { return _scode; } }
    }
}