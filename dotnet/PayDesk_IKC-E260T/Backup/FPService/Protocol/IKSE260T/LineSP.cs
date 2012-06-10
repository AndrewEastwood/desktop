using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.IKSE260T
{
    public partial class LineSP : Form
    {
        public byte lsp;

        public LineSP()
        {
            InitializeComponent();
        }
        public LineSP(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public LineSP(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void LineSP_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                lsp = (byte)numericUpDown1.Value;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void LineSP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}