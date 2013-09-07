using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using components.Lib;

namespace MINI_FP6.Components.UI.DriverUI
{
    public partial class Discount : Form
    {
        public object[] discInfo = new object[5];

        public Discount()
        {
            InitializeComponent();
        }
        public Discount(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public Discount(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void Discount_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;

            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox1.Enabled = checkBox1.Checked;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                label1.Text = "Значення в %";
                //numericUpDown1.Enabled = true;
                textBox1.MaxLength = 8;
            }
            else
            {
                label1.Text = "Значення в вал";
                //numericUpDown1.Enabled = false;
                textBox1.MaxLength = 10;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double value = 0;
            byte ppt = 0;

            try
            {
                CoreLib fn = new CoreLib();
                value = fn.GetDouble(textBox1.Text);

                switch (comboBox1.SelectedIndex)
                {
                    case 1: { value = -value; break; }
                }

                switch (comboBox2.SelectedIndex)
                {
                    case 0:
                        {
                            try
                            {
                                ppt = byte.Parse(numericUpDown1.Value.ToString());
                                if (ppt > 127)//7bit
                                    ppt = 127;
                            }
                            catch { return; }

                            if (value > 16777215)//3 byte
                                value = 16777215;
                            if (value < -16777215)//3 byte
                                value = -16777215;

                            switch (comboBox3.SelectedIndex)
                            {
                                case 0:
                                    {
                                        discInfo[0] = (byte)0;
                                        break;
                                    }
                                case 1:
                                    {
                                        discInfo[0] = (byte)2;
                                        break;
                                    }
                            }
                            break;
                        }
                    case 1:
                        {
                            if ((int)(value*100) > 2147483647)//31bit
                                value = 2147483647;
                            if ((int)(value * 100) < -2147483647)//31bit
                                value = -2147483647;

                            switch (comboBox3.SelectedIndex)
                            {
                                case 0:
                                    {
                                        discInfo[0] = (byte)1;
                                        break;
                                    }
                                case 1:
                                    {
                                        discInfo[0] = (byte)3;
                                        break;
                                    }
                            }
                            break;
                        }
                }

                discInfo[1] = value;
                discInfo[2] = ppt;
                discInfo[3] = richTextBox1.Text;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { return; }
        }

        private void Discount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}