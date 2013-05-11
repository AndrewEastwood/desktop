using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO_FPU550.DriverUI
{
    internal partial class GetFixMem : Form
    {
        private string _fno;
        private byte _retype;
        private string _sfno;

        //Constructors
        public GetFixMem()
        {
            InitializeComponent();
        }
        public GetFixMem(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public GetFixMem(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        //Events
        private void GetFixMem_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            _fno = "0";
            comboBox1.SelectedIndex = 0;
        }
        private void GetFixMem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Length == 0)
                    return;

                if (textBox2.Enabled && textBox2.Text.Length == 0)
                    return;

                _fno = textBox1.Text;
                _sfno = textBox2.Text;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 4:
                case 5:
                case 6: { textBox2.Enabled = textBox2.ReadOnly = true; break; }
                default: { textBox2.Enabled = textBox2.ReadOnly = false; break; }
            }
            _retype = (byte)comboBox1.SelectedIndex;
        }
    
        // Properties
        public string FixNo { get { return _fno; } }
        public byte ReturnType { get { return _retype; } }
        public string SecondaryParam { get { return _sfno; } }
    }
}