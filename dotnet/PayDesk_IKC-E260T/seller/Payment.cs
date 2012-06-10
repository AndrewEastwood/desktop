using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mdcore;

namespace PayDesk
{
    public partial class Payment : Form
    {
        private double suma;
        private List<double> cash = new List<double>();
        private double totCash;
        private double rest;
        private List<byte> type = new List<byte>();
        private bool autoClose;
        private int i = 0;

        //const
        public Payment(double suma)
        {
            InitializeComponent();
            this.suma = suma;
        }

        //events
        private void Pay_Load(object sender, EventArgs e)
        {
            winapi.API.OutputDebugString("Payment loaded");
            textBox0.Enabled = UserStruct.Properties[21];//card
            textBox1.Enabled = UserStruct.Properties[19];//credit
            textBox2.Enabled = UserStruct.Properties[20];//cheque
            textBox3.Enabled = UserStruct.Properties[18];//cash

            if (!textBox0.Enabled && !textBox1.Enabled &&
                !textBox2.Enabled && !textBox3.Enabled)
                button1.Enabled = false;

            label5.Text += string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", suma);
            label6.Text = string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", 0);

            //button1.PerformClick();//)
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            //Disable all other payments types

            if (Program.Service.UseEKKR)
            {
                textBox3.Enabled = UserStruct.Properties[18];//cash

                /*if (((TextBox)sender).Tag.ToString() != "3")
                {*/
                    textBox0.Enabled = textBox1.Text == "" && textBox2.Text == "" && UserStruct.Properties[21];//card
                    textBox1.Enabled = textBox0.Text == "" && textBox2.Text == "" && UserStruct.Properties[19];//credit
                    textBox2.Enabled = textBox0.Text == "" && textBox1.Text == "" && UserStruct.Properties[20];//cheque
                /*}
                else
                {
                    textBox0.Enabled = UserStruct.Properties[21];//card
                    textBox1.Enabled = UserStruct.Properties[19];//credit
                    textBox2.Enabled = UserStruct.Properties[20];//cheque
                }*/
            }
            else
            {
                textBox0.Enabled = ((TextBox)sender).Text.Length == 0 && UserStruct.Properties[21];//card
                textBox1.Enabled = ((TextBox)sender).Text.Length == 0 && UserStruct.Properties[19];//credit
                textBox2.Enabled = ((TextBox)sender).Text.Length == 0 && UserStruct.Properties[20];//cheque
                textBox3.Enabled = ((TextBox)sender).Text.Length == 0 && UserStruct.Properties[18];//cash
            }

            //Enable only current payment type
            ((TextBox)sender).Enabled = true;
            ((TextBox)sender).Focus();

            //Calculate sum for current type
            cash.Clear();
            type.Clear();

            totCash = AppFunc.GetDouble(textBox3.Text);//cash - 3
            if (totCash != 0)
            {
                cash.Add(totCash);
                type.Add(3);
            }
            totCash = AppFunc.GetDouble(textBox0.Text);//card - 0
            if (totCash != 0)
            {
                cash.Add(totCash);
                type.Add(0);
            }
            totCash = AppFunc.GetDouble(textBox1.Text);//credit - 1
            if (totCash != 0)
            {
                cash.Add(totCash);
                type.Add(1);
            }
            totCash = AppFunc.GetDouble(textBox2.Text);//cheque - 2
            if (totCash != 0)
            {
                cash.Add(totCash);
                type.Add(2);
            }
            totCash = 0;
            for (i = 0; i < cash.Count; i++)
                totCash += cash[i];

            if (totCash > suma && type[0] == 3 && type.Count > 1)
            {
                cash[0] = suma;
                for (i = 1; i < cash.Count; i++)
                    cash[0] -= cash[i];
                if (cash[0] < 0)
                    cash[0] = 0;
            }

            totCash = AppFunc.GetRoundedMoney(totCash);
            rest = totCash - suma;
            rest = AppFunc.GetRoundedMoney(rest);

            //Show rest
            label6.Text = string.Format("{0:F" + AppConfig.APP_MoneyDecimals + "}", rest >= 0.0 ? rest : 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (totCash > 9999999)
            {
                MMessageBox.Show("Помилка введення", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (totCash >= suma)
            {
                if (!type.Contains(3))
                    cash[0] = suma;
                DialogResult = DialogResult.OK;
                Close();
            }

            if (totCash == 0.0)
            {
                totCash = suma;
                rest = 0.0;
                autoClose = true;
                cash.Add(totCash);
                type.Add(3);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void Pay_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs n_one = new KeyEventArgs(Keys.NumPad1);
            KeyEventArgs n_two = new KeyEventArgs(Keys.NumPad2);
            KeyEventArgs n_three = new KeyEventArgs(Keys.NumPad3);
            KeyEventArgs n_four = new KeyEventArgs(Keys.NumPad4);

            KeyEventArgs dn = new KeyEventArgs(Keys.Down);
            KeyEventArgs up = new KeyEventArgs(Keys.Up);
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                Close();
                return;
            }

            if ((e.KeyValue == n_one.KeyValue || e.KeyValue == 49) && e.Alt)
            {
                textBox3.Focus();
                textBox3.SelectAll();
                return;
            }
            if ((e.KeyValue == n_two.KeyValue || e.KeyValue == 50) && e.Alt)
            {
                textBox0.Focus();
                textBox0.SelectAll();
                return;
            }
            if ((e.KeyValue == n_three.KeyValue || e.KeyValue == 51) && e.Alt && Program.Service.UseEKKR)
            {
                textBox1.Focus();
                textBox1.SelectAll();
                return;
            }
            if ((e.KeyValue == n_four.KeyValue || e.KeyValue == 52) && e.Alt && Program.Service.UseEKKR)
            {
                textBox2.Focus();
                textBox2.SelectAll();
                return;
            }

            if (e.KeyValue == dn.KeyValue)
            {
                if (textBox3.Focused)
                {
                    textBox0.Focus();
                    textBox0.SelectAll();
                    return;
                }
                if (textBox0.Focused)
                {
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return;
                }
                if (textBox1.Focused)
                {
                    textBox2.Focus();
                    textBox2.SelectAll();
                    return;
                }
            }
            if (e.KeyValue == up.KeyValue)
            {
                if (textBox2.Focused)
                {
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return;
                }
                if (textBox1.Focused)
                {
                    textBox0.Focus();
                    textBox0.SelectAll();
                    return;
                }
                if (textBox0.Focused)
                {
                    textBox3.Focus();
                    textBox3.SelectAll();
                    return;
                }
            }

        }

        public List<double> ItemsCash
        {
            get
            {
                return cash;
            }
        }

        public List<byte> Type
        {
            get
            {
                return type;
            }
        }

        public double CashSum
        {
            get
            {
                return totCash;
            }
        }

        public double Rest
        {
            get
            {
                return rest;
            }
        }

        public bool Autoclose
        {
            get
            {
                return autoClose;
            }
        }

    }
}