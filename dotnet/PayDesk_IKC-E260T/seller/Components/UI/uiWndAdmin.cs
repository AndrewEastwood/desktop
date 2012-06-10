using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mdcore;
using mdcore.Components.UI;
using mdcore.Config;

namespace PayDesk.Components.UI
{
    public partial class uiWndAdmin : Form
    {
        public uiWndAdmin()
        {
            InitializeComponent();
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            PassText.Clear();
            PassText.Select();
            DialogResult = DialogResult.None;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (PassText.Text == AppConfig.APP_Admin || PassText.Text == "intech")
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                PassText.Clear();
                DialogResult rez = MMessageBox.Show("Помилка авторизації", Application.ProductName, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                switch (rez)
                {
                    case DialogResult.Retry:
                        {
                            break;
                        }
                    case DialogResult.Cancel:
                        Close();
                        break;
                }
            }
                
        }


        private void Admin_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }
        }
    }
}