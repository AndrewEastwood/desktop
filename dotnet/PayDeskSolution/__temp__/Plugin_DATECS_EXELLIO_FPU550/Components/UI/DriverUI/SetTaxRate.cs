using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO_FPU550.DriverUI
{
    public partial class SetTaxRate : Form
    {
        private string _pass;
        private byte _dpoint;
        private bool[] _userates;
        private double[] _rates;
        // Constants
        private const byte TOT_RATES = 4;
        private const double FIRST_RATE = 20.0;


        // Constructors
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

        // Events
        private void SetTaxRate_Load(object sender, EventArgs e)
        {
            DataGridViewCheckBoxColumn cc = new DataGridViewCheckBoxColumn();
            cc.HeaderText = "Дозволити";
            dataGridView1.Columns.Add(cc);
            dataGridView1.Columns.Add("tax", "Ставка");
            dataGridView1.Columns.Add("rate", "Значення");

            _userates = new bool[TOT_RATES];
            _rates = new double[TOT_RATES];

            double[] value = new double[TOT_RATES];

            if (value.Length == 0)
                return;

            value[0] = FIRST_RATE;

            for (int i = 0; i < TOT_RATES; i++)
                dataGridView1.Rows.Add(new object[] { true, (char)('А' + i), value[i] });
        }
        private void SetTaxRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Length < 4)
                    return;

                _pass = textBox1.Text;
                _dpoint = (byte)numericUpDown3.Value;

                for (int i = 0; i < TOT_RATES; i++)
                {
                    _userates[i] = (bool)((DataGridViewCheckBoxCell)dataGridView1[0, i]).Value;
                    _rates[i] = double.Parse(dataGridView1[2, i].Value.ToString());
                }
            }
            catch
            {
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        // Propperties
        public string Password { get { return _pass; } }
        public byte DecimalPoint { get { return _dpoint; } }
        public bool[] UseRates { get { return _userates; } }
        public double[] Rates { get { return _rates; } }
    }
}