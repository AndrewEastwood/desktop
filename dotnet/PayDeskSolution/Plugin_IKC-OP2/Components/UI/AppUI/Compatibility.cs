using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using IKC_OP2.Config;

namespace IKC_OP2.UI.AppUI
{
    public partial class Compatibility : UserControl
    {
        public Compatibility()
        {
            InitializeComponent();
            RestoreSettings();
        }

        ~Compatibility()
        {
            ;
        }

        private void RestoreSettings()
        {
            this.checkBox_runAsOP6.Checked = Params.Compatibility.ContainsKey("OP6");
        }

        private void checkBox_runAsOP6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_runAsOP6.Checked)
                Params.Compatibility["OP6"] = true;
            else if (Params.Compatibility.ContainsKey("OP6"))
                Params.Compatibility.Remove("OP6");
        }
    }
}
