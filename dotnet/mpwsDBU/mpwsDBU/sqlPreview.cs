using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mpwsDBU
{
    public partial class sqlPreview : Form
    {
        public sqlPreview()
        {
            InitializeComponent();
        }

        public void AddSQLLine(string sqlline)
        {
            richTextBox1.Text += sqlline;
            AddSQLLine();
        }

        public void AddSQLLine()
        {
            richTextBox1.Text += Environment.NewLine;
        }

        public void ClearPreview()
        {
            richTextBox1.Text = string.Empty;
        }
    }
}
