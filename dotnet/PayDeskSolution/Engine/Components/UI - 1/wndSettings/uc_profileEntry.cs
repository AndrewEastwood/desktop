using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PayDesk.Components.UI.wndSettings
{
    public partial class uc_profileEntry : UserControl
    {
        private string key;
        private string title;
        private actionHandler a;

        public uc_profileEntry()
        {
            InitializeComponent();
            this.title = "< Нова Назва Профілю >";
            this.key = "p_0";
            this.a = this.ProfileAction;
        }

        private delegate int actionHandler(object sender);

        private void button_Click(object sender, EventArgs e)
        {
            this.a.Invoke(sender);
        }

        public int ProfileAction(object sender)
        {
            return int.Parse(((Button)sender).Tag.ToString());
        }
    }
}
