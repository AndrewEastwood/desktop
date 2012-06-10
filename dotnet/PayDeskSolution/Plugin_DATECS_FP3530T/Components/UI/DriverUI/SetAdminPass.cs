using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    public partial class SetAdminPass : Form
    {
        private string _cpwd;
        private string _npwd;

        // Counstructrs
        public SetAdminPass()
        {
            InitializeComponent();
        }
        public SetAdminPass(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetAdminPass(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void SetAdminPass_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void SetAdminPass_KeyDown(object sender, KeyEventArgs e)
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
                if (textBox2.Text.Length != 8)
                    return;

                _cpwd = textBox1.Text;
                _npwd = textBox2.Text;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        // Properties
        public string OldPassword { get { return this._cpwd; } }
        public string NewPassword { get { return this._npwd; } }
    }
}