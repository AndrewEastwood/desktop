using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace PayDesk.Components.UI.wndSettings
{
    public partial class uc_firmEntry : UserControl
    {
        public uc_firmEntry()
        {
            InitializeComponent();
        }

        public uc_firmEntry(object id, Hashtable ht) :
            this()
        {
            if (ht.ContainsKey("NAME"))
                this.textBox1.Text = ht["NAME"].ToString();
            this.textBox2.Text = id.ToString();
            //if (ht.ContainsKey("SOURCE"))
            //    this.textBox3.Text = ht["SOURCE"].ToString();
            if (ht.ContainsKey("OUTPUT"))
                this.textBox4.Text = ht["OUTPUT"].ToString();
            if (ht.ContainsKey("FILTER"))
                this.textBox5.Text = ht["FILTER"].ToString();
            if (ht.ContainsKey("SUBUNIT"))
                this.textBox6.Text = ht["SUBUNIT"].ToString();
        }

        private void uc_firmEntry_Load(object sender, EventArgs e)
        {
            if (this.Parent != null)
                this.Parent.Text = textBox1.Text;
        }

        private void event_buttonClick(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                //    if (this.tableLayoutPanel1.Controls.ContainsKey(((Control)sender).Tag.ToString()))
                //        this.tableLayoutPanel1.Controls[((Control)sender).Tag.ToString()].Text = folderBrowserDialog1.SelectedPath;
                //    else
                        this.tableLayoutPanel2.Controls[((Control)sender).Tag.ToString()].Text = folderBrowserDialog1.SelectedPath;
                }
                catch { }
            }
        }

        /* data */
        public string Profile_Name { get { return this.textBox1.Text; } set { this.textBox1.Text = value; } }
        public string Profile_ID { get { return this.textBox2.Text; } set { this.textBox2.Text = value; } }
        // public string Profile_Source { get { return this.textBox3.Text; } set { this.textBox3.Text = value; } }
        public string Profile_Output { get { return this.textBox4.Text; } set { this.textBox4.Text = value; } }
        public string Profile_Filter { get { return this.textBox5.Text; } set { this.textBox5.Text = value; } }
        public string Profile_SubUnit { get { return this.textBox6.Text; } set { this.textBox6.Text = value; } }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.Parent != null)
                this.Parent.Text = textBox1.Text;
        }

    }
}
