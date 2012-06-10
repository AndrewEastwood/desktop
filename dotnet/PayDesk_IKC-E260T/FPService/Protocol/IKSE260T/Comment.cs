using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.IKSE260T
{
    public partial class Comment : Form
    {
        public string commentLine;
        public bool retCheque;

        public Comment()
        {
            InitializeComponent();
        }
        public Comment(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public Comment(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void Comment_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                commentLine = textBox1.Text.Replace('³', 'i').Replace('²', 'I');
                retCheque = checkBox1.Checked;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void Comment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}