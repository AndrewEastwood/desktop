using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MyTestAPP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = ADD(int.Parse(textBox1.Text), int.Parse(textBox2.Text));
        }





 
       private string ADD(int a, int b)
        {
            int c = a + b;
            return c.ToString();
        }

    }
}
