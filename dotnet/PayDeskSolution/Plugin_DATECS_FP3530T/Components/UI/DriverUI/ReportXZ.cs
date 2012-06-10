using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_FP3530T.DriverUI
{
    public partial class ReportXZ : Form
    {
        private string _pwd;
        private byte _rtype;
        private bool _restUserSumm;
        private bool _restArtsSumm;

        // Constructors
        public ReportXZ()
        {
            InitializeComponent();
        }
        public ReportXZ(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public ReportXZ(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void ReportXZ_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
            comboBox1.SelectedIndex = 0;
        }
        private void ReportXZ_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == new KeyEventArgs(Keys.Escape).KeyValue)
                return;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Length == 0)
                    return;

                _pwd = textBox1.Text;
                if (comboBox1.SelectedIndex == 1)
                    _rtype = 0;
                else
                    _rtype = 2;
                _restUserSumm = checkBox1.Checked;
                _restArtsSumm = checkBox2.Checked;
            }
            catch { return; }

            DialogResult = DialogResult.OK;
            Close();
        }

        // Properties
        public string Password { get { return this._pwd; } }
        public byte ReportType { get { return this._rtype; } }
        public bool ClearUserSumm { get { return this._restUserSumm; } }
        public bool ClearArtsSumm { get { return this._restArtsSumm; } }
    }
}