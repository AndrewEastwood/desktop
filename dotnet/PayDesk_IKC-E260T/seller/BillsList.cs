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
    public partial class BillsList : Form
    {
        private FileStream stream;
        private BinaryFormatter binF = new BinaryFormatter();
        private DataTable dTBill = new DataTable();
        private DataTable dTList = new DataTable();
        private string[] bills;

        public BillsList()
        {
            InitializeComponent();
        }

        //Load all bills
        private void BillsList_Load(object sender, EventArgs e)
        {
            bills = Directory.GetFiles(AppConfig.Path_Bills, string.Format("{0:X2}_N*_????????.bill", AppConfig.APP_SubUnit));
            Array.Sort(bills);

            string item = string.Empty;

            for (int i = 0; i < bills.Length; i++, item = string.Empty)
            {
                stream = new FileStream(bills[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                dTBill = (DataTable)binF.Deserialize(stream);
                stream.Close();
                stream.Dispose();

                //Adding item
                listGrid.Rows.Add(new object[] {dTBill.ExtendedProperties["NOM"],
                    dTBill.ExtendedProperties["DT"],
                    dTBill.ExtendedProperties["CMT"]});
            }

            listGrid.SelectionChanged += listGrid_SelectionChanged;
            listGrid.Select();
            if (listGrid.RowCount != 0)
                listGrid_SelectionChanged(listGrid, EventArgs.Empty);
        }

        //Key watcher
        private void BillsList_KeyDown(object sender, KeyEventArgs e)
        {
            // Find
            if (e.KeyValue == new KeyEventArgs(Keys.F).KeyValue && e.Alt)
            {
                textBox1.Select();
                textBox1.SelectAll();
                return;
            }
            // Print
            if (e.KeyValue == new KeyEventArgs(Keys.P).KeyValue && e.Alt)
            {
                button2.PerformClick();
                return;
            }
            // Delete
            if (e.KeyValue == new KeyEventArgs(Keys.D).KeyValue && e.Alt)
            {
                button3.PerformClick();
                return;
            }
            // Load selected bill
            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
            {
                button1.PerformClick();
                return;
            }
            // Exit
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                if (textBox1.Focused)
                {
                    for (int i = 0; i < listGrid.RowCount; i++)
                        listGrid.Rows[i].Visible = true;
                    listGrid.Select();
                }
                else
                    Close();
                return;
            }
        }

        //Show selected bill
        private void listGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (DialogResult == DialogResult.OK)
                return;

            if (listGrid.SelectedRows.Count != 0 && listGrid.SelectedRows[0] != null)
            {
                try
                {
                    dTBill = LoadTableByIndex(listGrid.SelectedRows[0].Index);
                    billGrid.DataSource = dTBill;
                    for (int i = 0; i < billGrid.ColumnCount; i++)
                        switch (billGrid.Columns[i].Name)
                        {
                            case "NAME":
                                billGrid.Columns[i].HeaderText = "Назва";
                                billGrid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                break;
                            case "TOT":
                                billGrid.Columns[i].HeaderText = "К-сть";
                                billGrid.Columns[i].Width = 40;
                                break;
                            case "PRICE":
                                billGrid.Columns[i].HeaderText = "Ціна";
                                billGrid.Columns[i].Width = 50;
                                break;
                            case "SUM":
                                billGrid.Columns[i].HeaderText = "Сума";
                                billGrid.Columns[i].Width = 70;
                                break;
                            default:
                                billGrid.Columns[i].Visible = false;
                                break;
                        }

                    label2.Text = string.Format("{0} {1}", "Перегляд рахунку №", listGrid.SelectedRows[0].Cells["NOM"].Value);
                    button2.Enabled = button3.Enabled = true;

                }
                catch { }
            }
            else
            {
                label2.Text = string.Empty;
                button2.Enabled = button3.Enabled = false;
                billGrid.DataSource = null;
            }
        }

        //Styles
        private void listGrid_Leave(object sender, EventArgs e)
        {
            listGrid.DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.GrayText);
        }
        private void listGrid_Enter(object sender, EventArgs e)
        {
            listGrid.DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.Highlight);
        }

        //Print
        private void button2_Click(object sender, EventArgs e)
        {
            AppFunc.Print(new object[] { dTBill }, "kitchen", 1);

            for (int i = 0; i < dTBill.Rows.Count; i++)
                dTBill.Rows[i]["PRINTCOUNT"] = Convert.ToDouble(dTBill.Rows[i]["TOT"]);

            AppFunc.SaveBill(false, 0, "", dTBill);
        }
        //delete
        private void button3_Click(object sender, EventArgs e)
        {
            if (DeleteBill(dTBill))
            {
                for (int i = 0, j = 0; j < bills.Length; i++, j++)
                {
                    if (i == listGrid.CurrentRow.Index)
                        j++;
                    if (j < bills.Length)
                        bills[i] = bills[j];
                }
                Array.Resize<string>(ref bills, bills.Length - 1);
                listGrid.Rows.Remove(listGrid.CurrentRow);
            }
            
        }
        /// <summary>
        /// Завантаження рахунку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (listGrid.Focused || button1.Focused)
            {
                if (listGrid.RowCount != 0)
                {
                    dTBill = LoadTableByIndex(listGrid.CurrentRow.Index);
                    DialogResult = DialogResult.OK;
                }
                else
                    DialogResult = DialogResult.None;
                listGrid.SelectionChanged -= listGrid_SelectionChanged;
                Close();
            }
            else
                for (int i = 0; i < listGrid.RowCount; i++)
                    if (textBox1.Text == listGrid["NOM", i].Value.ToString())
                    {
                        listGrid.CurrentCell = listGrid["NOM", i];
                        listGrid.Rows[i].Selected = true;
                    }
                    else
                        listGrid.Rows[i].Visible = false;
        }

        /// <summary>
        /// Завантажений рахунок
        /// </summary>
        public DataTable LoadedBill { get { return dTBill; } }
        //GetBillByIndex
        private DataTable LoadTableByIndex(int index)
        {
            stream = new FileStream(bills[index], FileMode.Open, FileAccess.Read, FileShare.Read);
            DataTable dTable = (DataTable)binF.Deserialize(stream);
            stream.Close();
            stream.Dispose();
            return dTable;
        }
        /// <summary>
        /// Видалення рахунку
        /// </summary>
        /// <param name="bill">Рахунок</param>
        /// <returns></returns>
        static public bool DeleteBill(DataTable bill)
        {
            if (MMessageBox.Show("Видалити рахунок № " + bill.ExtendedProperties["NOM"],
                  Application.ProductName,
                  MessageBoxButtons.YesNo,
                  MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    File.Delete(bill.ExtendedProperties["PATH"].ToString());
                    bill.Clear();
                    bill.ExtendedProperties.Clear();
                    return true;
                }
                catch { }
            }
            return false;
        }
    }
}