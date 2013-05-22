using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IKC_OP2.Components.UI.DriverUI
{
    public partial class SetCashier : Form
    {
        public string id;
        public byte nom;
        public uint pass;

        public SetCashier()
        {
            InitializeComponent();
        }
        public SetCashier(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetCashier(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void SetCashier_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (maskedTextBox1.Text != "")
                {
                    id = textBox1.Text;
                    nom = (byte)comboBox1.SelectedIndex;
                    pass = uint.Parse(maskedTextBox1.Text);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                    maskedTextBox1.Select();
            }
            catch { }
        }

        private void SetCashier_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}