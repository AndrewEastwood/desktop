using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DATECS_EXELLIO_FPU550.DriverUI
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
                    MessageBox.Show("Розмірик артинки повинні бути не більше 432х92px", Text,
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }

                _pwd = textBox1.Text;

                Color _pxColor = new Color();
                _logo = new byte[_imglogo.Height][];
                for (int i = 0, j = 0; i < _imglogo.Height; i++)
                {
                    _logo[i] = new byte[_imglogo.Width];
                    for (j = 0; j < _imglogo.Width; j++)
                    {
                        _pxColor = _imglogo.GetPixel(j, i);
                        if (_pxColor.R > 0x77 &&
                            _pxColor.G > 0x77 &&
                            _pxColor.B > 0x77)
                            _logo[i][j] = 0;
                        else
                            _logo[i][j] = 1;
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