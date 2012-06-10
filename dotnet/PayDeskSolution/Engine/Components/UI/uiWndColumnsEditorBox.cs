using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PayDesk.Components.UI
{
    public partial class uiWndColumnsEditorBox : Form
    {
        private int i = 0;
        private DataGridView dataGridView1 = null;
        private bool[] autoSizeColumns = new bool[0];
        private string[] cNames = new string[0];

        public uiWndColumnsEditorBox(ref DataGridView dataGridView1, int type)
        {
            InitializeComponent();
            label1.Text += type == 1 ? "чеку" : "товарів";

            autoSizeColumns = new bool[dataGridView1.ColumnCount];
            cNames = new string[dataGridView1.ColumnCount];

            for (i = 0; i < dataGridView1.ColumnCount; i++)
            {
                checkedListBox1.Items.Add(dataGridView1.Columns[i].Name + " (" + dataGridView1.Columns[i].HeaderText + ")", dataGridView1.Columns[i].Visible);
                cNames[i] = dataGridView1.Columns[i].Name;
                if (dataGridView1.Columns[i].AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                    autoSizeColumns[i] = true;
                else
                    autoSizeColumns[i] = false;
            }

            if (checkedListBox1.Items.Count != 0)
                checkedListBox1.SelectedIndex = 0;

            this.dataGridView1 = dataGridView1;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedIndex != -1)
                AutoSizeChBox.Checked = autoSizeColumns[checkedListBox1.SelectedIndex];
        }

        private void AutoSizeChBox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedIndex != -1)
                autoSizeColumns[checkedListBox1.SelectedIndex] = AutoSizeChBox.Checked;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                for (i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    dataGridView1.Columns[cNames[i]].Visible = checkedListBox1.GetItemChecked(i);
                    if (autoSizeColumns[i])
                        dataGridView1.Columns[cNames[i]].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    else
                        dataGridView1.Columns[cNames[i]].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                }
            }
            catch
            {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void columnsEditorBox_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                Close();
                return;
            }
        }
    }
}