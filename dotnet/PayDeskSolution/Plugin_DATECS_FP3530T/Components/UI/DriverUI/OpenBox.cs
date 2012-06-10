using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    internal partial class OpenBox : Form
    {
        private byte _impulse;

        // Constructors
        public OpenBox()
        {
            InitializeComponent();
        }
        public OpenBox(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public OpenBox(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void OpenBox_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            _impulse = 20;
        }
        private void OpenBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _impulse = Convert.ToByte(textBox1.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        // Properties
        public byte Impulse { get { return _impulse; } }
    }
}