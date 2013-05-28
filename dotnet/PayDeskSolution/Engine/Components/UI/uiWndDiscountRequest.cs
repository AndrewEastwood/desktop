using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using driver.Lib;
using driver.Config;
using components.Shared.Attributes;
using driver.Components.UI;
using components.Components.MMessageBox;
//using ;
//using mdcore.Config;
//using mdcore.Components.UI;

namespace PayDesk.Components.UI
{
    public partial class uiWndDiscountRequest : Form
    {
        private bool type = false;
        private double suma = 0.0;
        private double dsc = 0.0;
        private double cdisc = 0.0;

        public uiWndDiscountRequest(double suma, bool type)
        {
            InitializeComponent();
            this.type = type;
            if (type)
                Text = "Знижка";
            else
                Text = "Націнка";
            this.suma = suma;
        }

        public uiWndDiscountRequest()
        {
            InitializeComponent();
        }

        private void Discount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Up).KeyValue)
            {
                textBox1.Select();
                textBox1.SelectAll();
                radioButton1.Checked = true;
                return;
            }

            if (e.KeyValue == new KeyEventArgs(Keys.Down).KeyValue)
            {
                textBox2.Select();
                textBox2.SelectAll();
                radioButton2.Checked = true;
                return;
            }

            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
            {
                button1.PerformClick();
                return;
            }

            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                Close();
                return;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            cdisc = 0.0;
            dsc = 0.0;
            
            try
            {
                if (radioButton1.Checked)
                {
                    dsc = MathLib.GetDouble(textBox1.Text);
                    if (dsc > 99.99)
                        dsc = 99.99;
                    if (dsc < 0.0)
                        dsc = 0.0;
                }
                else
                {
                    cdisc = MathLib.GetDouble(textBox2.Text);
                    if (cdisc > suma)
                    {
                        MMessageBox.Show(this, "Сума чеку або товарів, на яких дозволена знижка є меньша, ніж сума знижки", Application.ProductName,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    //dsc = (cdisc * 100) / suma;

                    //if (double.IsInfinity(dsc))
                    //{
                    //    dsc = 0.0;
                    //    cdisc = 0.0;
                    //}
                }

                dsc = Math.Round(dsc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, MidpointRounding.AwayFromZero);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch
            {
                return;
            }
        }

        private void SetVariant(int no)
        {
            if (no == 1)
            {
                this.textBox1.Focus();
                this.textBox1.Select();
                this.textBox1.SelectAll();
                this.radioButton1.Checked = true;
                this.radioButton2.Checked = false;
            }
            else
            {
                this.textBox2.Focus();
                this.textBox2.Select();
                this.textBox2.SelectAll();
                this.radioButton1.Checked = false;
                this.radioButton2.Checked = true;
            }
        }

        /// <summary>
        /// Встановлє знижку або надбавку
        /// </summary>
        /// <param name="dm">Масив з значеннями знижки або надбавки для процентних значень</param>
        /// <param name="cdm">Масив з значеннями знижки або надбавки для грошових значень</param>
        public void SetDiscount(ref double[] dm, ref double[] cdm)
        {
            int idx = type ? 0 : 1;
            //if (type)
            //{

            textBox1.Enabled = ConfigManager.Instance.CommonConfiguration.APP_UsePercentTypeDisc;
            radioButton1.Enabled = ConfigManager.Instance.CommonConfiguration.APP_UsePercentTypeDisc;
            textBox2.Enabled = ConfigManager.Instance.CommonConfiguration.APP_UseAbsoluteTypeDisc && !Program.AppPlugins.IsActive(PluginType.ILegalPrinterDriver);
            radioButton2.Enabled = ConfigManager.Instance.CommonConfiguration.APP_UseAbsoluteTypeDisc && !Program.AppPlugins.IsActive(PluginType.ILegalPrinterDriver);

            textBox1.Text = Math.Round(Math.Abs(dm[idx]), ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, MidpointRounding.AwayFromZero).ToString();
            textBox2.Text = Math.Round(Math.Abs(cdm[idx]), ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, MidpointRounding.AwayFromZero).ToString();

            if (textBox1.Enabled && textBox2.Enabled)
                if (ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc == 0)//if percent variant is dafault
                    SetVariant((dm[idx] != 0.0 || (dm[idx] == 0.0 && cdm[idx] == 0.0)) ? 1 : 2);
                else
                    SetVariant((cdm[idx] != 0.0 || (dm[idx] == 0.0 && cdm[idx] == 0.0)) ? 2 : 1);
            else
            {
                if (textBox1.Enabled)
                    SetVariant(1);
                if (textBox2.Enabled)
                    SetVariant(2);
            }

            //if (!textBox2.Enabled || (dm[idx] != 0.0 && cdm[idx] == 0.0))
            //}
            //else
            //{
            //    textBox1.Text = Math.Round(-dm[1], mdcore.AppConfig.APP_MoneyDecimals, MidpointRounding.AwayFromZero).ToString();
            //    //textBox2.Text = Math.Round(suma * (-dm[1] / 100), mdcore.AppConfig.APP_MoneyDecimals, MidpointRounding.AwayFromZero).ToString();
            //    textBox2.Text = Math.Round(-cdm[1], mdcore.AppConfig.APP_MoneyDecimals, MidpointRounding.AwayFromZero).ToString();
            //    if (dm[1] == 0.0)
            //        this.radioButton2.Checked = true;
            //    else
            //        this.radioButton1.Checked = true;
            //}

            //if (this.radioButton1.Checked)
            //{
            //    this.textBox1.Select();
            //    this.textBox1.Focus();
            //}
            //else
            //{
            //    this.textBox2.Select();
            //    this.textBox2.Focus();
            //    this.textBox2.SelectAll();
            //}
            this.ShowDialog();

            if (this.DialogResult != DialogResult.OK)
                return;

            if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
            {
                dm[0] = dm[1] = 0.0;
                cdm[0] = cdm[1] = 0.0;
            }

            if (type)
            {
                // znugka
                dm[0] = dsc;
                cdm[0] = cdisc;
            }
            else
            {
                // nadbavka
                dm[1] = -dsc;
                cdm[1] = -cdisc;
            }
        }
    }
}