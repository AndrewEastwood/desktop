using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FPService.Protocol
{
    public partial class FP_Info : Form
    {
        public FP_Info()
        {
            InitializeComponent();
        }

        public FP_Info(object[] info,string caption)
        {
            InitializeComponent();
            this.Text = caption;

            for (int i = 0; i < info.Length; i++)
                if (info[i] != null)
                    richTextBox1.Text += info[i] + "\r\n";
        }

        private void FP_Info_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                Close();
        }
    }
}