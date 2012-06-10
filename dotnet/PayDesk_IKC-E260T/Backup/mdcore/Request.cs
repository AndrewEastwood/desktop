using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mdcore
{
    public partial class Request : Form
    {
        private double newTotal;
        private double newPrice;
        private DataRow dRow;

        public Request(DataRow dRow, double tot)
        {
            InitializeComponent();

            Text = dRow["DESC"].ToString();

            textBox1.Text = tot.ToString();

            /*

            startTotal = -1;
            if (tot < 0)
                textBox1.Text = dRow["TOT"].ToString();
            else
            {
                textBox1.Text = tot.ToString();
                startTotal = AppFunc.GetDouble(dRow["TOT"]);
            }*/

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

            dRow["PRICE"] = newPrice;
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
    }
}