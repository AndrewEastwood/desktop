using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    internal partial class LineFeed : Form
    {
        private byte lines = 0;

        // Constructor
        public LineFeed()
        {
            InitializeComponent();
        }
        public LineFeed(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public LineFeed(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void LineFeed_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void LineFeed_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                lines = Convert.ToByte(textBox1.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }
    
        // Properties
        public byte Lines { get { return this.lines; } }
    }
}