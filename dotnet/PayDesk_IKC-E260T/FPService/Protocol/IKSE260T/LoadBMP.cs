using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.IKSE260T
{
    public partial class LoadBMP : Form
    {
        private bool allowToDraw;
        public object[] imageInfo = new object[2];

        public LoadBMP()
        {
            InitializeComponent();
        }
        public LoadBMP(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public LoadBMP(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            textBox2.Text = openFileDialog1.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ushort pass = 0;
            try
            {
                pass = ushort.Parse(textBox1.Text);
            }
            catch { return; }

            imageInfo[0] = pass;
            if (textBox2.Text == "")
                return;
            imageInfo[1] = textBox2.Text;//image

            DialogResult = DialogResult.OK;
            Close();
        }

        private void LoadBMP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}