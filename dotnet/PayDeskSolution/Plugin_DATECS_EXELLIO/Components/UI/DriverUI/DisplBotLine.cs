using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    internal partial class DisplBotLine : Form
    {
        private string btnline = "";

        public DisplBotLine()
        {
            InitializeComponent();
        }
        public DisplBotLine(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public DisplBotLine(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void DisplBotLine_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                btnline = textBox1.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void DisplBotLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    
        // Properties
        public string BottomLine { get { return this.btnline; } }
    
    }
}