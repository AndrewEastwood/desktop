using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using components.Lib;

namespace DATECS_EXELLIO.DriverUI
{
    partial class CustomSetMoney : Form
    {
        private double _money;

        //Constructors
        public CustomSetMoney()
        {
            InitializeComponent();
        }
        public CustomSetMoney(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public CustomSetMoney(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void SetGetMoney_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void SetGetMoney_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CoreLib fn = new CoreLib();
                _money = fn.GetDouble(textBox1.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        //Properties
        public double Money { get { return this._money; } }

    }
}