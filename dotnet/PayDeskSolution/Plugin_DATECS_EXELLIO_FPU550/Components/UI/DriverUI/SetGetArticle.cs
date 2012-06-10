using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO_FPU550.DriverUI
{
    partial class SetGetArticle : Form
    {
        private char _option;
        private object[] _param;

        //Constructors
        public SetGetArticle()
        {
            InitializeComponent();
        }
        public SetGetArticle(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetGetArticle(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void SetGetArticle_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            // select first item
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }
        private void SetGetArticle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                switch (_option)
                {
                    case 'I':
                        {
                            switch (comboBox2.SelectedIndex)
                            {
                                case 0:
                                    {
                                        _option = 'I';
                                        break;
                                    }
                                case 1:
                                    {
                                        _option = 'F';
                                        break;
                                    }
                                case 2:
                                    {
                                        _option = 'N';
                                        break;
                                    }
                            }

                            _param = new object[0];
                            break;
                        }
                    case 'P':
                        {
                            _param = new object[10];

                            _param[0] = comboBox3.Text;
                            _param[1] = uint.Parse(maskedTextBox1.Text);
                            _param[2] = ',';
                            _param[3] = maskedTextBox2.Text;
                            _param[4] = ',';
                            _param[5] = textBox2.Text;
                            _param[6] = ',';
                            _param[7] = textBox6.Text;
                            _param[8] = ',';
                            _param[9] = textBox5.Text;

                            break;
                        }
                    case 'D':
                        {
                            _param = new object[3];

                            if (comboBox1.SelectedIndex == 0)
                                _param[0] = maskedTextBox7.Text;
                            else
                                _param[0] = 'A';
                            _param[1] = ',';
                            _param[2] = textBox1.Text;
                            break;
                        }
                    case 'R':
                        {
                            _param = new object[1];
                            _param[0] = maskedTextBox4.Text;
                            break;
                        }
                    case 'C':
                        {
                            _param = new object[5];
                            _param[0] = maskedTextBox5.Text;
                            _param[1] = ',';
                            _param[2] = maskedTextBox6.Text;
                            _param[3] = ',';
                            _param[4] = textBox10.Text;
                            break;
                        }
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        //Properties
        public char Option { get { return _option; } }
        public object[] Param { get { return _param; } }

        private void checkBoxes_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox _cbo = ((CheckBox)sender);

            if (_cbo.Tag.ToString() == "1" && _cbo.Checked)
            {
                _option = 'P';
                checkBox2.Checked = checkBox3.Checked = checkBox4.Checked = checkBox5.Checked = false;
                return;
            }
            if (_cbo.Tag.ToString() == "2" && _cbo.Checked)
            {
                _option = 'D';
                checkBox1.Checked = checkBox3.Checked = checkBox4.Checked = checkBox5.Checked = false;
                return;
            }
            if (_cbo.Tag.ToString() == "3" && _cbo.Checked)
            {
                _option = 'R';
                checkBox1.Checked = checkBox2.Checked = checkBox4.Checked = checkBox5.Checked = false;
                return;
            }
            if (_cbo.Tag.ToString() == "4" && _cbo.Checked)
            {
                _option = 'C';
                checkBox1.Checked = checkBox2.Checked = checkBox3.Checked = checkBox5.Checked = false;
                return;
            } 
            if (_cbo.Tag.ToString() == "5" && _cbo.Checked)
            {
                _option = 'I';
                checkBox1.Checked = checkBox2.Checked = checkBox3.Checked = checkBox4.Checked = false;
                return;
            }

            if (!checkBox1.Checked &&
                !checkBox2.Checked &&
                !checkBox3.Checked &&
                !checkBox4.Checked &&
                !checkBox5.Checked)
                _cbo.Checked = true;
        }
    }
}