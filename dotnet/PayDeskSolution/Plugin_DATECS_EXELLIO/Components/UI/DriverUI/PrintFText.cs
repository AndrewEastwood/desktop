using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    internal partial class PrintFText : Form
    {
        private string _ftext = "";

        public PrintFText()
        {
            InitializeComponent();
        }
        public PrintFText(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public PrintFText(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void PrintFText_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _ftext = textBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void PrintFText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    
        // Properties
        public string FixText { get { return this._ftext; } }
    
    }
}