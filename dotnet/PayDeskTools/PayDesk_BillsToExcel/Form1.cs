using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using components.Components.ExcelDataWorker;

namespace PayDesk_BillsToExcel
{
    public partial class Form1 : Form
    {
        private List<string> billFiles = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void uploadControl1_OnFilePathChanged(string path)
        {
            // read bill files here

            billFiles.AddRange(Directory.GetFiles(path, "*.bill"));
            // billFiles
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // open save dialog here
            if (saveFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                new ExcelDataWorker().FileWrite(saveFileDialog1.FileName, billsToExcel());
                MessageBox.Show("Completed! Bills are saved");
            }
        }

        private DataTable billsToExcel()
        {
            DataTable dTable = new DataTable();



            return dTable;
        }
    }

}
