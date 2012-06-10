using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO_FPU550.DriverUI
{
    public partial class SetUserPass : Form
    {
        private string _cpwd;
        private string _npwd;
        private byte _userno;

        // Counstructrs
        public SetUserPass()
        {
            InitializeComponent();
        }
        public SetUserPass(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetUserPass(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void SetUserPwd_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            comboBox1.SelectedIndex = 0;
        }
        private void SetUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Length < 4)
                    return;

                _cpwd=textBox1.Text;
                _npwd = textBox2.Text;

                _userno = (byte)(comboBox1.SelectedIndex + 1);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        // Properties
        public string OldPassword { get { return this._cpwd; } }
        public byte UserNo { get { return this._userno; } }
        public string NewPassword { get { return this._npwd; } }
    }
}