using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    internal partial class DisplTopLine : Form
    {
        private string topline = "";

        public DisplTopLine()
        {
            InitializeComponent();
        }
        public DisplTopLine(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public DisplTopLine(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void DisplTopLine_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                topline = textBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void DisplTopLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    
        // Properties
        public string TopLine { get { return this.topline; } }
    
    }
}