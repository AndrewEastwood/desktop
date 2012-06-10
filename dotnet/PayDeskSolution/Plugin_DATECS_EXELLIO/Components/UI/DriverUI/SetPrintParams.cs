using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace DATECS_EXELLIO.Components.UI.DriverUI
{
    public partial class SetPrintParams : Form
    {
        private Hashtable prnFormat;

        //Constructors
        public SetPrintParams()
        {
            InitializeComponent();
            prnFormat = new Hashtable();
        }
        public SetPrintParams(string caption)
            : this()
        {
            Text = caption;
        }
        public SetPrintParams(string caption, string desc)
            : this(caption)
        {
            descLabel.Text = desc;
        }

        // Events
        private void SetPrintParams_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void SetPrintParams_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // seting header text
                Hashtable TEXTS = new Hashtable();
                string[] hlines = this.richTextBox1.Text.Split(new string[] { "\n" }, StringSplitOptions.None);
                for (int idx = 0; idx < 7; idx++)
                {
                    if (hlines.Length > idx && hlines[idx] != null)
                        TEXTS.Add(idx, hlines[idx]);
                    else
                        TEXTS.Add(idx, string.Empty);
                }
                this.prnFormat["TEXTS"] = TEXTS;


                Hashtable AUTOCUT = new Hashtable();
                AUTOCUT.Add("C", checkBox1.Checked ? "1" : "0");
                this.prnFormat["AUTOCUT"] = AUTOCUT;

                Hashtable ALLOWLOGO = new Hashtable();
                ALLOWLOGO.Add("L", checkBox2.Checked ? "1" : "0");
                this.prnFormat["ALLOWLOGO"] = ALLOWLOGO;


                Hashtable CHARLIGHT = new Hashtable();
                CHARLIGHT.Add("D", trackBar1.Value.ToString());
                this.prnFormat["CHARLIGHT"] = CHARLIGHT;


                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        //Properties
        public Hashtable PrinterFormat { get { return this.prnFormat; } }
    }
}
