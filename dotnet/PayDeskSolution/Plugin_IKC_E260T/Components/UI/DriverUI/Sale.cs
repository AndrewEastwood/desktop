using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using components.Lib;

namespace IKC_E260T.Components.UI.DriverUI
{
    public partial class Sale : Form
    {
        public object[] articles = new object[0];
        public bool dontPrintOne = false;

        public Sale()
        {
            InitializeComponent();
        }
        public Sale(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public Sale(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void Sale_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.Text == string.Empty)
                    return;

                CoreLib fn = new CoreLib();
                object[] article = new object[7];

                article[0] = fn.GetDouble(maskedTextBox1.Text);//total
                article[1] = byte.Parse(comboBox2.Text);//dose_ppt 0..15
                article[2] = Convert.ToDouble(textBox1.Text);//price
                article[3] = comboBox1.Text;//pdv
                article[4] = richTextBox1.Text.Replace('³', 'i').Replace('²', 'I');//name
                article[5] = textBox2.Text;//id
                article[6] = byte.Parse(comboBox3.Text);//money_ppt 0..15

                Array.Resize<object>(ref articles, articles.Length + 1);
                articles[articles.Length - 1] = article;

                ClearAll();
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dontPrintOne = checkBox1.Checked;//printOne
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ClearAll()
        {
            richTextBox1.Clear();
            maskedTextBox1.Clear();
            textBox1.Clear();
            textBox2.Clear();
            checkBox1.Checked = false;
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