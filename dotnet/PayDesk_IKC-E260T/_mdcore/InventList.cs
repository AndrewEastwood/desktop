using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mdcore
{
    public partial class InventList : Form
    {
        public InventList()
        {
            InitializeComponent();

            string[] files = System.IO.Directory.GetFiles(AppConfig.Path_Cheques, string.Format("_IS{0:X2}{1:X2}*", AppConfig.APP_SubUnit, AppConfig.APP_PayDesk));

            if (files.Length != 0)
            {
                bool hasDocToday = false;
                DateTime doc = DateTime.Now;
                Array.Reverse(files);
                for (int i = 0; i < files.Length; i++)
                {
                    doc = Microsoft.VisualBasic.FileIO.FileSystem.GetFileInfo(files[i]).CreationTime;
                    listBox1.Items.Add(doc.ToString());
                    if (!hasDocToday && doc.Day == DateTime.Now.Day &&
                        doc.Month == DateTime.Now.Month &&
                        doc.Year == DateTime.Now.Year)
                        hasDocToday = true;
                }

                if (!hasDocToday)
                    listBox1.Items.Insert(0, "Новий документ");

                listBox1.SelectedIndex = 0;
            }
        }

        private void InventList_KeyDown(object sender, KeyEventArgs e)
        {
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

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;

            DialogResult = DialogResult.OK;
            Close();
        }

        public DateTime? OpenInvent()
        {
            if (listBox1.Items.Count == 0)
                return DateTime.Now;

            if (listBox1.Items.Count == 1)
            {
                DateTime dt = DateTime.Parse(listBox1.SelectedItem.ToString());
                if (dt.Day == DateTime.Now.Day &&
                    dt.Month == DateTime.Now.Month &&
                    dt.Year == DateTime.Now.Year)
                    return DateTime.Now;
            }
            if (this.ShowDialog() != DialogResult.OK)
                return null;

            if (listBox1.SelectedIndex == 0)
                return DateTime.Now;

            return DateTime.Parse(listBox1.SelectedItem.ToString());
        }
    }
}