using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    public partial class ResetUserData : Form
    {
        private string _pwd;
        private byte _userno;

        // Counstructrs
        public ResetUserData()
        {
            InitializeComponent();
        }
        public ResetUserData(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public ResetUserData(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void ResetUserData_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            comboBox1.SelectedIndex = 0;
        }
        private void ResetUserData_KeyDown(object sender, KeyEventArgs e)
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