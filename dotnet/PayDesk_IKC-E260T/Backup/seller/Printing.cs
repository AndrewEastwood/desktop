using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mdcore;
using System.IO;

namespace PayDesk
{
    public partial class Printing : Form
    {
        public Printing()
        {
            InitializeComponent();
        }

        private void Printing_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = AppConfig.APP_UseCommonPrinting;

            // Printers
            if (AppConfig.APP_PrintersLinks != null && AppConfig.APP_PrintersLinks[0] != null)
                for (int i = 0; i < AppConfig.APP_PrintersLinks[0].Length; i++)
                    printersGrid.Rows.Add(new object[] { AppConfig.APP_PrintersLinks[0][i], AppConfig.APP_PrintersLinks[1][i], "Видалити" });
            
            //Templates
            textBox1.Text = AppConfig.Path_Tpl_1;
            textBox2.Text = AppConfig.Path_Tpl_2;
            textBox3.Text = AppConfig.Path_Tpl_3;
        }

        private void PathSelection_Click(object sender, EventArgs e)
        {
            byte idx = byte.Parse(((Button)sender).Tag.ToString());
            openFileDialog1.Title = tabControl1.TabPages[1].Controls["label" + idx].Text;
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                tabControl1.TabPages[1].Controls["textBox" + idx].Text = openFileDialog1.FileName;
        }

        private void addPrinterBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog(this) == DialogResult.OK)
                printersGrid.Rows.Add(new object[] { openFileDialog2.FileName, "Принтер " + (printersGrid.Rows.Count + 1), "Видалити" });

        }

        private void printersGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                string pName = printersGrid[1, e.RowIndex].Value.ToString();
                DialogResult delRez = MMessageBox.Show(this, "Видалити принтер \"" + pName + "\"", Application.ProductName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (delRez == DialogResult.Yes)
                    printersGrid.Rows.RemoveAt(e.RowIndex);
            }
        }
       
        private void PrintSet_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                Close();
                return;
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            try
            {
                AppConfig.APP_UseCommonPrinting = checkBox1.Checked;

                if (AppConfig.APP_PrintersLinks == null)
                    AppConfig.APP_PrintersLinks = new string[2][];
                AppConfig.APP_PrintersLinks[0] = new string[printersGrid.RowCount];
                AppConfig.APP_PrintersLinks[1] = new string[printersGrid.RowCount];

                for (int i = 0; i < printersGrid.RowCount; i++)
                {
                    AppConfig.APP_PrintersLinks[0][i] = printersGrid[0, i].Value.ToString();
                    AppConfig.APP_PrintersLinks[1][i] = printersGrid[1, i].Value.ToString();

                }

                AppConfig.Path_Tpl_1 = textBox1.Text;
                AppConfig.Path_Tpl_2 = textBox2.Text;
                AppConfig.Path_Tpl_3 = textBox3.Text;

                AppConfig.SaveData();
            }
            catch { return; }
            DialogResult = DialogResult.OK;
        }

    }
}