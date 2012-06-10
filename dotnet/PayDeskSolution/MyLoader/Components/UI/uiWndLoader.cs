using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Components.UI
{
    public partial class uiWndLoader : Form
    {
        public uiWndLoader()
        {
            InitializeComponent();
        }

        // it's on my side (i'll generate response and say to client)
        private string giveRegisrationNumber(string appPublicSerial)
        {// mask 
            // we should generate private key and crypt it;

            // making private key (same as in getPrivateNumber)
            string h = this.stringFiltering(appPublicSerial);
            string code = string.Empty;
            int cc = 0;
            for (int i = 0, j = 1; i < h.Length; i++, j++)
            {
                if (j >= h.Length)
                    j = 0;

                cc = byte.Parse(h[i].ToString()) ^ byte.Parse(h[j].ToString());
                code += this.GetCleanDMD5Hash(cc.ToString())[i];
            }

            // crypting it
            string cryptedCode = string.Empty;
            int[] cpk = new int[code.Length];
            int[] coded = new int[cpk.Length];
            for (int j = 0; j < code.Length; j += 4)
            {
                cpk[j] = int.Parse(string.Format("{0}{1}{2}{3}", code[j], code[j + 1], code[j + 2], code[j + 3]));
                coded[j] = cpk[j] ^ 0xd9a;
                cryptedCode += coded[j].ToString("00000");
            }
            return cryptedCode;
        }
        private string GetCleanDMD5Hash(string input)
        {
            return this.stringFiltering(this.GetMD5Hash(input) + this.GetMD5Hash(input, 2));
        }
        private string stringFiltering(string input)
        {
            string output = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsNumber(input[i]))
                    output += input[i];
            }
            return output;
        }
        private string GetMD5Hash(string input, byte cycle)
        {
            string sh = input;
            for (byte i = 0; i < cycle; i++)
                sh = this.GetMD5Hash(sh);
            return sh;
        }
        private string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();



            return password;
        }

        /* EVENTS */

        private void button_WndLoader_MakeCode_Click(object sender, EventArgs e)
        {
            if (this.maskedTextBox1.Text == string.Empty ||
                this.maskedTextBox3.Text == string.Empty)
            {
                MessageBox.Show(this, "Fill all fileds", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.maskedTextBox1.Text.Length != 0)
            {
                maskedTextBox2.Text = this.giveRegisrationNumber(this.maskedTextBox1.Text);
                //if (!File.Exists("customers.txt"))
                //File.CreateText("customers.txt").Dispose();
                // saving customer
                if (!Directory.Exists("customers"))
                    Directory.CreateDirectory("customers");
                string customerFileName = string.Format("customers\\c_{0}-{1:MM-dd-yyyy}.txt", maskedTextBox3.Text, DateTime.Now);
                if (File.Exists(customerFileName))
                {
                    MessageBox.Show("Customer already registered.\r\nEnter new customer name.");
                    return;
                }
                StreamWriter swr = File.CreateText(customerFileName);
                swr.WriteLine(string.Format("/*= DATE: {4}\r\ncustomer: {0}\r\nPayDeskSn: {1}\r\nActivation No. {2}\r\n{3}", maskedTextBox3.Text, maskedTextBox1.Text, maskedTextBox2.Text, string.Empty.PadRight(20, '-'), DateTime.Now.ToString()));
                swr.Close();
                swr.Dispose();
            }
        }
    }
}
