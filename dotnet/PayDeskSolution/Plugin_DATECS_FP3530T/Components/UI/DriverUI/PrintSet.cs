using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    public partial class PrintSet : Form
    {
        private char _value;
        private string _text;

        // Constructors
        public PrintSet()
        {
            InitializeComponent();
        }
        public PrintSet(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public PrintSet(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void PrintSet_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void PrintSet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }
        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            switch (((RadioButton)sender).Tag.ToString())
            {
                case "1":
                    {
                        _value = '7';
                        _text = richTextBox1.Text + "\r\n" + richTextBox2.Text;
                        break;
                    }
                case "2":
                    {
                        _value = 'C';
                        _text = "1";
                        break;
                    }
                case "3":
                    {
                        _value = 'C';
                        _text = "0";
                        break;
                    }
                case "4":
                    {
                        _value = 'J';
                        _text = "1";
                        break;
                    }
                case "5":
                    {
                        _value = 'J';
                        _text = "0";
                        break;
                    }
                case "6":
                    {
                        _value = 'L';
                        _text = "1";
                        break;
                    }
                case "7":
                    {
                        _value = 'L';
                        _text = "0";
                        break;
                    }
            }
        }
        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            if (((RichTextBox)sender).Text.Length % 36 == 0)
                ((RichTextBox)sender).Text += "\r\n";
        }

        //Properties
        public char PrnValue { get { return _value; } }
        public string PrnText { get { return _text; } }


    }
}