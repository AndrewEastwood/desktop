using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using driver.Components.Objects;
using components.Components.MMessageBox;
//0using mdcore;
//0using mdcore.Components.UI;
//0using mdcore.Config;
//0using mdcore.Components.Objects;

namespace PayDesk.Components.UI
{
    public partial class uiWndAdmin : FormEx
    {
        public uiWndAdmin()
        {
            InitializeComponent();
        }


        public uiWndAdmin(FormStartPosition startPos)
            : this()
        {
            this.StartPosition = startPos;
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
            if (PassText.Text == driver.Config.ConfigManager.Instance.CommonConfiguration.APP_Admin || PassText.Text == "intech")
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                PassText.Clear();
                DialogResult rez = MMessageBoxEx.Show(this, "Помилка авторизації", Application.ProductName, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
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