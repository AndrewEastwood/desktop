using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mdcore;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PayDesk
{
    public partial class BillSave : Form
    {
        //таблиця рахунку
        private DataTable dTable;
        //номер рахунку
        private uint nom;
        //Якщо true то рахунок є новим інакше рахунок вже був збережений
        private bool isNewBill;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dTable">Таблиця рахунку</param>
        public BillSave(DataTable dTable)
        {
            InitializeComponent();

            isNewBill = !dTable.ExtendedProperties.Contains("BILL");

            if (isNewBill)
                nom = AppFunc.GetNextBillID();
            else
            {
                nom = uint.Parse(dTable.ExtendedProperties["NOM"].ToString());
                richTextBox1.Text = dTable.ExtendedProperties["CMT"].ToString();
            }

            this.dTable = dTable;
            Text += " " + nom.ToString();
        }

        /// <summary>
        /// Виконує збереження рахунку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (AppFunc.SaveBill(isNewBill, nom, richTextBox1.Text, dTable))
            {
                dTable.ExtendedProperties.Clear();
                DialogResult = DialogResult.OK;
            }

            Close();
        }

        /// <summary>
        /// Обробник клавіатури
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BillRequets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                Close();
            }
            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
            {
                button1.PerformClick();
            }
        }
    }
}