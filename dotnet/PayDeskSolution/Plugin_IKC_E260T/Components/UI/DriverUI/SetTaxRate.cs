using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IKC_E260T.Components.UI.DriverUI
{
    public partial class SetTaxRate : Form
    {
        public ushort pass;
        public byte taxCount;
        public uint[] tax;
        public byte status;
        public byte taxGCount;
        public uint[] gtax;

        public SetTaxRate()
        {
            InitializeComponent();
        }
        public SetTaxRate(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public SetTaxRate(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        private void SetTaxRate_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("tax", "Ставка");
            dataGridView1.Columns.Add("rate", "Значення");

            dataGridView2.Columns.Add("tax", "Ставка");
            dataGridView2.Columns.Add("rate", "Значення");

            double value = 20.0;
            for (int i = 0; i < 5; i++, value = 0)
            {

                dataGridView1.Rows.Add(new object[] { (char)('А' + i), value });
                dataGridView2.Rows.Add(new object[] { (char)('А' + i), 0 });
            }
            comboBox2.SelectedIndex = 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView2.Enabled = checkBox1.Checked;
            label7.Enabled = numericUpDown2.Enabled = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                pass = ushort.Parse(textBox1.Text);
                taxCount = (byte)numericUpDown1.Value;
                taxGCount = (byte)0;
                tax = new uint[taxCount];
                gtax = new uint[taxGCount];

                double val = 0;
                int i = 0;
                for (i = 0; i < taxCount; i++)
                {
                    val = Convert.ToDouble(dataGridView1["rate", i].Value);
                    tax[i] = (uint)(val * 100);
                }

                status |= (byte)numericUpDown3.Value;
                byte taxType = Convert.ToByte(comboBox2.SelectedIndex);
                status |= (byte)(taxType << 4);
                if (checkBox1.Checked)
                {
                    status |= 0x20;

                    taxGCount = (byte)numericUpDown2.Value;
                    gtax = new uint[taxGCount];
                    for (i = 0; i < taxGCount; i++)
                    {
                        val = Convert.ToDouble(dataGridView2["rate", i].Value);
                        gtax[i] = (uint)(val * 100);
                    }
                }

            }
            catch
            {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void SetTaxRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
    }
}