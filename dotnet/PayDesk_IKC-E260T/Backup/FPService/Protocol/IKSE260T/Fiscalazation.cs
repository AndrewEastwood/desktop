using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.IKSE260T
{
    public partial class Fiscalazation : Form
    {
        public ushort pass;
        public string fn;

        public Fiscalazation()
        {
            InitializeComponent();
        }
        public Fiscalazation(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public Fiscalazation(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                pass = ushort.Parse(textBox2.Text);
                fn = textBox1.Text;
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