using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol.IKSE260T
{
    public partial class SetCod : Form
    {
        public uint oldPass;
        public byte nom;
        public uint newPass;

        public SetCod()
        {
            InitializeComponent();
        }
        public SetCod(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetCod(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void SetCod_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                oldPass = uint.Parse(textBox1.Text);
                nom = (byte)comboBox1.SelectedIndex;
                newPass = uint.Parse(textBox2.Text);

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void SetCod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}