using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PayDesk
{
    public partial class FiscalFunctions : Form
    {
        private string[] names;
        private string[] desc;
        private byte idx;

        public FiscalFunctions(object device, string[][] pubFunc)
        {
            InitializeComponent();

            listBox1.Items.Clear();

            if (pubFunc[0].Length == 0)
            {
                listBox1.Enabled = false;
                button1.Enabled = false;
                return;
            }

            listBox1.Items.AddRange(pubFunc[0]);
            listBox1.SelectedIndex = 0;
            this.desc = pubFunc[0];
            this.names = pubFunc[1];
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
            DialogResult = DialogResult.OK;
            Close();
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
                return names[idx];
            }
        }
        public string Descriprion
        {
            get
            {
                return desc[idx];
            }
        }
    }
}