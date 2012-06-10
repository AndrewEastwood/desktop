using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI.Customs
{
    public partial class ReportZ : Form
    {
        private string _pwd;
        private byte _rtype;
        private bool _restUserSumm;
        private bool _restArtsSumm;

        // Constructors
        public ReportZ()
        {
            InitializeComponent();
            _restUserSumm = false;
            _restArtsSumm = false;
            _rtype = 0;
        }
        public ReportZ(string caption)
            : this()
        {
            Text = caption;
        }
        public ReportZ(string caption, string desc)
            : this()
        {
            Text = caption;
            descLabel.Text = desc;
        }

        // Events
        private void ReportXZ_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
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