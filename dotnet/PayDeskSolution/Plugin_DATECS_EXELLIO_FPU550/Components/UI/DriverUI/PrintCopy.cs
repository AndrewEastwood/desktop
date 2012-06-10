using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO_FPU550.DriverUI
{
    internal partial class PrintCopy : Form
    {
        private byte _copies = 1;

        public PrintCopy()
        {
            InitializeComponent();
        }
        public PrintCopy(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public PrintCopy(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void PrintCopy_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _copies = (byte)numericUpDown1.Value;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void PrintCopy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    
        // Properties
        public byte Copies { get { return this._copies; } }
    
    }
}