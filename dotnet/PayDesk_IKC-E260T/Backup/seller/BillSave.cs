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
        //������� �������
        private DataTable dTable;
        //����� �������
        private uint nom;
        //���� true �� ������� � ����� ������ ������� ��� ��� ����������
        private bool isNewBill;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="dTable">������� �������</param>
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
        /// ������ ���������� �������
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
        /// �������� ���������
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