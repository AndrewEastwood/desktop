using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IKC_E260T.Components.UI.DriverUI
{
    public partial class FpInfo : Form
    {
        public FpInfo()
        {
            InitializeComponent();
        }

        public FpInfo(object[] info)
            :this()
        {
            for (int i = 0; i < info.Length; i++)
                if (info[i] != null)
                    richTextBox1.Text += info[i] + "\r\n";
        }

        public FpInfo(object[] info, string caption)
            :this(info)
        {
            this.Text = caption;
        }

        public FpInfo(object[] info, string caption, string desc)
            : this(info, caption)
        {
            this.descLabel.Text = desc;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FP_Info_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                Close();
        }

    }
}