using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO.DriverUI
{
    public partial class LoadLogo : Form
    {
        private string _pwd;
        private byte[][] _logo;

        //Constructors
        public LoadLogo()
        {
            InitializeComponent();
        }
        public LoadLogo(string caption)
        {
            InitializeComponent();
            Text = caption;
        }
        public LoadLogo(string caption, string desc)
        {
            InitializeComponent();
            Text = caption;
            descLabel.Text = desc;
        }

        //Events
        private void LoadLogo_Load(object sender, EventArgs e)
        {
            if (descLabel.Text == "[DESC]")
                descLabel.Text = "";
        }
        private void LoadLogo_KeyDown(object sender, KeyEventArgs e)
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

                if (!System.IO.File.Exists(textBox2.Text))
                    return;

                Bitmap _imglogo = new Bitmap(textBox2.Text);
                if (_imglogo.Width > 432 || _imglogo.Height > 96)
                {
                    MessageBox.Show("Розмірик артинки повинні бути не більше 432х96px", Text,
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }

                _pwd = textBox1.Text;

                Color _pxColor = new Color();
                _logo = new byte[96][];
                byte mask = 0x00;
                int bitCorrector = 7;
                for (int i = 0, j = 0, cbuyte = 0; i < _imglogo.Height; i++)
                {
                    _logo[i] = new byte[54];
                    cbuyte = 0;
                    for (j = 0; j < _imglogo.Width; j++)
                    {
                        _pxColor = _imglogo.GetPixel(j, i);
                        if (_pxColor.R > 0x77 &&
                            _pxColor.G > 0x77 &&
                            _pxColor.B > 0x77)
                        {
                            ;
                            // we're askipping it because it is white color;
                            //_logo[i][cbuyte] = 0;
                        }
                        else
                        {
                            _logo[i][cbuyte] = (byte)(_logo[i][cbuyte] | (byte)Math.Pow(2, bitCorrector));
                        }



                        bitCorrector--;
                        if (bitCorrector < 0)
                        {
                            cbuyte++;
                            bitCorrector = 7;
                        }
                    }
                }
                
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { return; }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            textBox2.Text = openFileDialog1.FileName;
        }

        //Properties
        public string Password { get { return _pwd; } }
        public byte[][] Logo { get { return _logo; } }
    }
}