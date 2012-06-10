using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    internal partial class PrintNText : Form
    {
        private string _ntext = "";

        public PrintNText()
        {
            InitializeComponent();
        }
        public PrintNText(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public PrintNText(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void PrintNText_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _ntext = textBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void PrintNText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    
        // Properties
        public string NonFixText { get { return this._ntext; } }
    
    }
}