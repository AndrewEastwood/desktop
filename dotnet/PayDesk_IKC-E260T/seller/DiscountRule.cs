using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PayDesk
{
    public partial class DiscountRule : Form
    {
        private string newRule;

        public DiscountRule(string cmd)
        {
            InitializeComponent();
            Text = cmd;

            if (cmd != "")
                Translate(cmd);
            else
            {
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
                numericUpDown2.Text = "0.00";
                numericUpDown3.Text = "0";
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox3.Enabled = comboBox2.Enabled = numericUpDown3.Enabled = checkBox2.Checked;
            label4.Text = MakeHelpString();
        }
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            label4.Text = MakeHelpString();
        }
        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            label4.Text = MakeHelpString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            newRule = MakeInfo();
            Close();
        }
        private void BillRule_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                Close();
                return;
            }
        }

        //Methods
        private void Translate(string text)
        {
            string[] _rrr = text.Split(';');

            comboBox1.SelectedIndex = int.Parse(_rrr[0]);
            numericUpDown2.Text = _rrr[1];

            if (_rrr[2] == "?")
            {
                checkBox2.Checked = false;
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
            }
            else
            {
                checkBox2.Checked = true;
                if (_rrr[2] == "|")
                    comboBox3.SelectedIndex = 0;
                else
                    comboBox3.SelectedIndex = 1;
            }

            if (_rrr[3] != "N")
                comboBox2.SelectedIndex = int.Parse(_rrr[3]);

            if (_rrr[3] != "N")
                numericUpDown3.Text = _rrr[4];

            numericUpDown1.Value = int.Parse(_rrr[5]);

        }
        private string MakeInfo()
        {
            string value="";

            value += comboBox1.SelectedIndex.ToString() + ';';
            value += numericUpDown2.Text + ";";

            if (checkBox2.Checked == true)
            {
                if (comboBox3.SelectedIndex == 0)
                    value += "|;";
                else
                    value += "&;";

                value += comboBox2.SelectedIndex.ToString() + ';';
                value += numericUpDown3.Text + ";";
            }
            else
                value += "?;N;N;";

            value += numericUpDown1.Value.ToString();

            return value;
        }
        private string MakeHelpString()
        {
            string help = "Якщо сума товару є ";

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {
                        help += "більша за ";
                        break;
                    }
                case 1:
                    {
                        help += "меньша за ";
                        break;
                    }
                case 2:
                    {
                        help += "більша і рівна за ";
                        break;
                    }
                case 3:
                    {
                        help += "меньша і рівна за ";
                        break;
                    }
            }

            help += numericUpDown2.Value.ToString() + " ";

            if (checkBox2.Checked)
            {
                switch (comboBox3.SelectedIndex)
                {
                    case 0:
                        {
                            help += "і кількість товару є ";
                            break;
                        }
                    case 1:
                        {
                            help += "або кількість товару є ";
                            break;
                        }
                }

                switch (comboBox2.SelectedIndex)
                {
                    case 0:
                        {
                            help += "більша за ";
                            break;
                        }
                    case 1:
                        {
                            help += "меньша за ";
                            break;
                        }
                    case 2:
                        {
                            help += "нерівна ";
                            break;
                        }
                    case 3:
                        {
                            help += "більша і рівна за ";
                            break;
                        }
                    case 4:
                        {
                            help += "меньша і рівна за ";
                            break;
                        }
                }

                help += numericUpDown3.Value.ToString() + " ";
            }

            help += "то надати знижку " + numericUpDown1.Value.ToString() + "%";

            return help;
        }

        //Properties
        public string NewRule
        {
            get { return newRule; }
        }
    }
}