using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    public partial class GetUserInfo : Form
    {
        private byte _userno;

        // Counstructrs
        public GetUserInfo()
        {
            InitializeComponent();
        }
        public GetUserInfo(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public GetUserInfo(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void GetUserInfo_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            comboBox1.SelectedIndex = 0;
        }
        private void GetUserInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _userno = (byte)(comboBox1.SelectedIndex + 1);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        // Properties
        public byte UserNo { get { return this._userno; } }
    }
}