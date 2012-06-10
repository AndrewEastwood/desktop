using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    public partial class SetUserName : Form
    {
        private string _pwd;
        private string _name;
        private byte _userno;

        // Counstructrs
        public SetUserName()
        {
            InitializeComponent();
        }
        public SetUserName(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetUserName(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void SetUserName_Load(object sender, EventArgs e)
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

                _pwd=textBox1.Text;
                _name = textBox2.Text;

                _userno = (byte)(comboBox1.SelectedIndex + 1);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        // Properties
        public string Password { get { return this._pwd; } }
        public byte UserNo { get { return this._userno; } }
        public string UserName { get { return this._name; } }
    }
}