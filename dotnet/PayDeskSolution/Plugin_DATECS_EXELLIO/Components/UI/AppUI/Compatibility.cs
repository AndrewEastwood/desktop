using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DATECS_EXELLIO.Config;

namespace DATECS_EXELLIO.UI.AppUI
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
            if (Params.Compatibility.ContainsKey("msg_comm_attemptsToRead"))
                this.msg_comm_attemptsToRead.Value = decimal.Parse(Params.Compatibility["msg_comm_attemptsToRead"].ToString());

            if (Params.Compatibility.ContainsKey("msg_comm_timeoutOnFail"))
                this.msg_comm_timeoutOnFail.Value = decimal.Parse(Params.Compatibility["msg_comm_timeoutOnFail"].ToString());

            if (Params.Compatibility.ContainsKey("msg_comm_attemptsToWait"))
                this.msg_comm_attemptsToWait.Value = decimal.Parse(Params.Compatibility["msg_comm_attemptsToWait"].ToString());

            if (Params.Compatibility.ContainsKey("msg_comm_timeoutOnBusy"))
                this.msg_comm_timeoutOnBusy.Value = decimal.Parse(Params.Compatibility["msg_comm_timeoutOnBusy"].ToString());
        }

        private void checkBox_runAsOP6_CheckedChanged(object sender, EventArgs e)
        {
            Params.Compatibility["msg_comm_attemptsToRead"] = this.msg_comm_attemptsToRead.Value;
            Params.Compatibility["msg_comm_timeoutOnFail"] = this.msg_comm_timeoutOnFail.Value;
            Params.Compatibility["msg_comm_attemptsToWait"] = this.msg_comm_attemptsToWait.Value;
            Params.Compatibility["msg_comm_timeoutOnBusy"] = this.msg_comm_timeoutOnBusy.Value;
        }
    }
}
