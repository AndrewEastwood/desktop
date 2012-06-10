using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.IKSE260T
{
    public partial class Payment : Form
    {
        public object[] payInfo = new object[1 + 1 + 1 + 1 + 1];

        public Payment()
        {
            InitializeComponent();
        }
        public Payment(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public Payment(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void Payment_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = !checkBox2.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 3:
                    {
                        textBox1.Enabled = true;
                        checkBox2.Enabled = true;
                        label2.Enabled = true;
                        break;
                    }
                default:
                    {
                        textBox1.Enabled = false;
                        checkBox2.Enabled = false;
                        label2.Enabled = false;
                        break;
                    }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                payInfo[0] = (byte)comboBox1.SelectedIndex;
                payInfo[1] = checkBox1.Checked;
                if (checkBox2.Checked || comboBox1.SelectedIndex != 3)
                    payInfo[2] = Convert.ToDouble("0");
                else
                {
                    if (textBox1.Text != "")
                        payInfo[2] = Convert.ToDouble(textBox1.Text);
                    else
                        return;
                }
                payInfo[3] = checkBox2.Checked;
                payInfo[4] = textBox2.Text;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void Payment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}