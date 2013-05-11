using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO_FPU550.DriverUI
{
    public partial class ResetUserPass : Form
    {
        private string _pwd;
        private byte _userno;

        // Counstructrs
        public ResetUserPass()
        {
            InitializeComponent();
        }
        public ResetUserPass(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public ResetUserPass(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void ResetUserPass_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            comboBox1.SelectedIndex = 0;
        }
        private void ResetUserPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Length != 8)
                    return;

                _pwd = textBox1.Text;
                _userno = (byte)(comboBox1.SelectedIndex + 1);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        // Properties
        public string Password { get { return this._pwd; } }
        public byte UserNo { get { return this._userno; } }
    }
}