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
    partial class SetGetMoney : Form
    {
        private double _money;

        //Constructors
        public SetGetMoney()
        {
            InitializeComponent();
        }
        public SetGetMoney(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetGetMoney(string caption, string desc)
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
                string moneyText = textBox1.Text;
                bool isNegative = moneyText[0] == '-';
                CoreLib fn = new CoreLib();

                if (!Char.IsDigit(moneyText[0]))
                    moneyText = moneyText.Substring(1);

                _money = fn.GetDouble(moneyText);

                if (isNegative)
                    _money = -_money;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        //Properties
        public double Money { get { return this._money; } }

    }
}