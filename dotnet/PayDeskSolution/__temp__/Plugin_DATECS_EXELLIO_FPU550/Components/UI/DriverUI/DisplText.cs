using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO_FPU550.DriverUI
{
    internal partial class DisplText : Form
    {
        private string textline = "";

        public DisplText()
        {
            InitializeComponent();
        }
        public DisplText(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public DisplText(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void DisplText_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textline = textBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void DisplText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    
        // Properties
        public string TextLine { get { return this.textline; } }
    
    }
}