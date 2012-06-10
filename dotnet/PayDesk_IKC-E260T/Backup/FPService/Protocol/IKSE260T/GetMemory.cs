using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.IKSE260T
{
    public partial class GetMemory : Form
    {
        public byte page;
        public string block;
        public byte size;

        public GetMemory()
        {
            InitializeComponent();
        }
        public GetMemory(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public GetMemory(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void GetMemory_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Block Adress uint
                page = byte.Parse(textBox1.Text);
                //Page Nom byte
                if (textBox2.Text != string.Empty)
                    block = textBox2.Text;
                else
                    return;
                //Block Size byte
                size = byte.Parse(textBox3.Text);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void GetMemory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}