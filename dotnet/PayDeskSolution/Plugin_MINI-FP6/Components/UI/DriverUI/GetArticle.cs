using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MINI_FP6.Components.UI.DriverUI
{
    public partial class GetArticle : Form
    {
        public uint articleID = 0;
        public GetArticle()
        {
            InitializeComponent();
        }
        public GetArticle(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public GetArticle(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void GetArticle_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void GetArticle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                articleID = uint.Parse(textBox1.Text);
            }
            catch { return; }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}