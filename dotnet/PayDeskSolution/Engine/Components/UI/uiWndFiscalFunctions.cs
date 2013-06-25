using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace PayDesk.Components.UI
{
    public partial class uiWndFiscalFunctions : Form
    {
        private Hashtable _allowedMethods;
        private byte idx;

        public uiWndFiscalFunctions(object device, Hashtable pubFunc)
        {
            InitializeComponent();

            listBox1.Items.Clear();

            if (pubFunc.Count == 0)
            {
                listBox1.Enabled = false;
                button1.Enabled = false;
                return;
            }
            foreach (object obj in pubFunc)
                listBox1.Items.Add(((DictionaryEntry)obj).Value);
            //listBox1.DataSource = pubFunc.Values;
            listBox1.SelectedIndex = 0;
            //this.desc = pubFunc[0];
            //this.names = pubFunc[1];
            _allowedMethods = pubFunc;
            this.idx = 0;

            this.Text += " : " + device;
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                idx = (byte)listBox1.SelectedIndex;
            }
            catch { return; }

            DialogResult dlgRez = MessageBox.Show("Виконати ф-цію ЕККР [" + Descriprion + "] ? ", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dlgRez == System.Windows.Forms.DialogResult.Yes)
            {
                DialogResult = dlgRez;
                Close();
            }
        }

        private void FiscalFunctions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
            {
                Close();
                return;
            }

            if (e.KeyValue == new KeyEventArgs(Keys.Enter).KeyValue)
            {
                button1.PerformClick();
                return;
            }

        }

        public string Function
        {
            get
            {
                foreach (object obj in _allowedMethods)
                {
                    if (((DictionaryEntry)obj).Value == listBox1.SelectedItem)
                        return ((DictionaryEntry)obj).Key.ToString();
                }

                return string.Empty;// names[idx];
            }
        }
        public string Descriprion
        {
            get
            {
                return listBox1.SelectedItem.ToString();//desc[idx];
            }
        }
    }
}