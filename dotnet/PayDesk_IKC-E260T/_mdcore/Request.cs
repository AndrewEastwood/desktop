using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mdcore
{
    /// <summary>
    /// Request form for setting or updating product quantity or price
    /// </summary>
    public partial class Request : Form
    {
        private double newTotal;
        private double newPrice;
        private DataRow dRow;
        private double currTotal;
        private double addTotal;
        private bool locked;
        private double articlePackage;

        /// <summary>
        /// Main Request constructor
        /// </summary>
        /// <param name="dRow"></param>
        /// <param name="tot"></param>
        public Request(DataRow dRow, double tot)
        {
            InitializeComponent();
            double.TryParse(dRow["PACK"].ToString(), out this.articlePackage);
            if (this.articlePackage == 0)
                this.articlePackage = 1;
            this.currTotal = tot;

            // window description
            Text = dRow["DESC"].ToString();

            // general quantity
            this.textBox1.Text = this.currTotal.ToString();

            // additional quantity
            if (AppConfig.APP_AddTotal == "none")
                this.textBox2.Visible = this.label4.Visible = false;

            // price list
            comboBox1.Items.Add(dRow["PRICE"]);
            if (UserStruct.Properties[1])
                comboBox1.Items.AddRange(new object[]{
                    dRow["PR1"],
                    dRow["PR2"],
                    dRow["PR3"]});
            if (UserStruct.Properties[2])
                comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
            else
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndex = 0;

            // helper
            textBox3.Text = "1 " + dRow["UNIT"].ToString() + " = " + this.articlePackage.ToString();

            // current article data row
            this.dRow = dRow;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                if (UserStruct.Properties[2])
                    comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void AddProductBtn_Click(object sender, EventArgs e)
        {
            try
            {
                newTotal = AppFunc.GetDouble(textBox1.Text);
                newPrice = AppFunc.GetDouble(comboBox1.Text);

                newTotal = AppFunc.GetRoundedDose(newTotal);
                newPrice = AppFunc.GetRoundedMoney(newPrice);

                if (newTotal <= 0 || newTotal > 100000 )
                {
                    textBox1.Focus();
                    textBox1.SelectAll();
                    MMessageBox.Show("Помилкове значення кількості");
                    return;
                }

                if (newPrice <= 0)
                {
                    MMessageBox.Show("Ціна не може бути відємною");
                    return;
                }

                if (UserStruct.Properties[8] && !UserStruct.Properties[1] && !UserStruct.Properties[2])
                    newPrice = AppFunc.AutomaticPrice(newTotal, dRow);

            }
            catch { return; }

            DialogResult = DialogResult.OK;
            Close();
        }


        public DataRow dataRow { get { return this.dRow; } }

        public bool UpdateRowSource()
        {
            if (this.ShowDialog() != DialogResult.OK)
                return false;
           /* int pack = 1;
            int.TryParse(dRow["PACK"].ToString(), out pack);
            if (pack == 0) pack = 1;*/
            dRow["PRICE"] = newPrice;
            /*
            if (AppConfig.APP_AddTotal == "type1")
                newTotal = AppFunc.GetRoundedDose(newTotal / pack);
            if (AppConfig.APP_AddTotal == "type2")
                newTotal = AppFunc.GetRoundedDose(newTotal * pack);
            */
            dRow["TOT"] = newTotal;
            dRow["SUM"] = dRow["ASUM"] = AppFunc.GetRoundedMoney(newTotal * newPrice);
            return true;
        }

        private void Request_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                Close();
                return;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (locked) { locked = false; return; }


            switch (((TextBox)sender).Tag.ToString())
            {
                case "main":
                    {
                        this.currTotal = AppFunc.GetDouble(textBox1.Text);
                        if (AppConfig.APP_AddTotal == "type1")
                        {
                            this.addTotal = AppFunc.GetRoundedDose(this.currTotal * this.articlePackage);
                        }
                        if (AppConfig.APP_AddTotal == "type2")
                        {
                            this.addTotal = AppFunc.GetRoundedDose(this.currTotal / this.articlePackage);
                        }
                        locked = true;
                        textBox2.Text = this.addTotal.ToString();
                        break;

                    }
                case "add":
                    {
                        this.addTotal = AppFunc.GetDouble(textBox2.Text);
                        if (AppConfig.APP_AddTotal == "type1")
                        {
                            this.currTotal = AppFunc.GetRoundedDose(this.addTotal / this.articlePackage);
                        }
                        if (AppConfig.APP_AddTotal == "type2")
                        {
                            this.currTotal = AppFunc.GetRoundedDose(this.addTotal * this.articlePackage);
                        }
                        locked = true;
                        textBox1.Text = this.currTotal.ToString();
                        break;
                    }
            }
        }
    }
}