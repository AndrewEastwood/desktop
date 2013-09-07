using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MINI_FP6.Components.UI.DriverUI
{
    public partial class PayMoney : Form
    {
        public object[] articles = new object[0];
        public bool dontPrintOne = false;

        public PayMoney()
        {
            InitializeComponent();
        }
        public PayMoney(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public PayMoney(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void PayMoney_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                object[] article = new object[6];

                article[0] = Convert.ToDouble(maskedTextBox1.Text);//total
                article[1] = byte.Parse(comboBox2.Text);//0..3
                article[2] = Convert.ToDouble(textBox1.Text);//price
                article[3] = comboBox1.Text;//pdv
                article[4] = richTextBox1.Text.Replace('³', 'i').Replace('²', 'I');//name
                article[5] = textBox2.Text;//id

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

        private void PayMoney_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}