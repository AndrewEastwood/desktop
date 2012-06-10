using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.IKSE260T
{
    public partial class ProgArt : Form
    {
        public ushort pass;
        public object[] articles = new object[0];

        public ProgArt()
        {
            InitializeComponent();
        }
        public ProgArt(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public ProgArt(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void ProgArt_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        //to memory
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                object[] article = new object[5];

                article[0] = byte.Parse(comboBox2.Text);//0..3
                article[1] = Convert.ToDouble(textBox1.Text);//price
                article[2] = comboBox1.Text;//pdv
                article[3] = richTextBox1.Text.Replace('³', 'i').Replace('²', 'I');//name
                article[4] = uint.Parse(textBox2.Text);//id

                Array.Resize<object>(ref articles, articles.Length + 1);
                articles[articles.Length - 1] = article;

                ClearAll();
            }
            catch { }
        }
        //ok
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                pass = ushort.Parse(textBox3.Text);
            }
            catch { return; }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ClearAll()
        {
            richTextBox1.Clear();
            textBox1.Clear();
            textBox2.Clear();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;

            richTextBox1.Select();
        }

        private void Sale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}