using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IKC_OP2.Components.UI.DriverUI
{
    public partial class SetHeadLine : Form
    {
        public ushort pass;
        public string line1;
        public string line2;
        public string line3;
        public string line4;

        public SetHeadLine()
        {
            InitializeComponent();
        }
        public SetHeadLine(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetHeadLine(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                pass = ushort.Parse(textBox1.Text);
                line1 = textBox2.Text.Replace('³', 'i').Replace('²', 'I');
                line2 = textBox3.Text.Replace('³', 'i').Replace('²', 'I');
                line3 = textBox4.Text.Replace('³', 'i').Replace('²', 'I');
                line4 = textBox5.Text.Replace('³', 'i').Replace('²', 'I');
            }
            catch { }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Avans_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}