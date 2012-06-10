using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    internal partial class SetTaxNum : Form
    {
        private string _tcode;
        private char _ttype;

        // Constructor
        public SetTaxNum()
        {
            InitializeComponent();
        }
        public SetTaxNum(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetTaxNum(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void SetTaxNum_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            _tcode = "";
        }
        private void SetTaxNum_KeyDown(object sender, KeyEventArgs e)
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

                _tcode = maskedTextBox1.Text;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            _ttype = char.Parse(((RadioButton)sender).Tag.ToString());
        }    

        // Properties
        public string TaxNumber { get { return _tcode; } }
        public char TaxType { get { return _ttype; } }
    }
}