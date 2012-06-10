using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    internal partial class SetFixNum : Form
    {
        private string _fcode;

        // Constructor
        public SetFixNum()
        {
            InitializeComponent();
        }
        public SetFixNum(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetFixNum(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void SetFixNum_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            _fcode = "";
        }
        private void SetFixNum_KeyDown(object sender, KeyEventArgs e)
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

                _fcode = maskedTextBox1.Text;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }
    
        // Properties
        public string FiscalNumber { get { return _fcode; } }
    }
}