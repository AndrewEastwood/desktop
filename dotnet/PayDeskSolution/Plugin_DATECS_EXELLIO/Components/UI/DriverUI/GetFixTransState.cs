using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    internal partial class GetFixTransState : Form
    {
        private char _param;

        //Constructors
        public GetFixTransState()
        {
            InitializeComponent();
        }
        public GetFixTransState(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public GetFixTransState(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        //Events
        private void GetFixTransState_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void GetFixTransState_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked)
                    _param = 'T';
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }
    
        // Properties
        public char TransParam { get { return _param; } }
    }
}