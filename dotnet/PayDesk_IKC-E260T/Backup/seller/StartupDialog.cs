using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PayDesk
{
    public partial class StartupDialog : Form
    {
        private bool hold = false;
        private Point oldPos = new Point();
        private string login;
        private string password;

        public StartupDialog()
        {
            InitializeComponent();
            this.Text = Application.ProductName;
        }

        private void StartupDialog_Load(object sender, EventArgs e)
        {
            textBox1.Select();
            this.BringToFront();
            this.Select();
            this.Activate();
            this.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }//exit

        private void button2_Click(object sender, EventArgs e)
        {
            if (maskedTextBox1.Focused && textBox1.Text == "")
            {
                textBox1.Focus();
                return;
            }

            if (textBox1.Focused && maskedTextBox1.Text == "")
            {
                maskedTextBox1.Focus();
                return;
            }

            password = maskedTextBox1.Text;
            login = textBox1.Text;
            DialogResult = DialogResult.OK;

        }//ok

        private void maskedTextBox1_Enter(object sender, EventArgs e)
        {
            maskedTextBox1.SelectAll();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                hold = true;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            hold = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (hold)
            {
                Point nPos = Control.MousePosition;
                int x = nPos.X - oldPos.X;
                int y = nPos.Y - oldPos.Y;
                Location = new Point(Location.X + x, Location.Y + y);
                Update();
                oldPos = nPos;
            }
            else
                oldPos = Control.MousePosition;
        }

        private void StartupDialog_Activated(object sender, EventArgs e)
        {
            this.BackgroundImage = Properties.Resources.startupActive;
        }

        private void StartupDialog_Deactivate(object sender, EventArgs e)
        {
            this.BackgroundImage = Properties.Resources.startupDeactive;
        }

        private void start_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                button3.PerformClick();
                return;
            }
        }


        //Properties
        public string Password
        {
            set
            {
                password = value;
            }
            get
            {
                return password;
            }
        }
        public string Login
        {
            set
            {
                login = value;
            }
            get
            {
                return login;
            }
        }
    }
}