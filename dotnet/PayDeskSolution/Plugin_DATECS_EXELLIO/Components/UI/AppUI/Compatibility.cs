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
        private bool _is_init;

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
            bool _useDefaults = false;

            _is_init = true;
            if (Params.Compatibility.ContainsKey("msg_comm_attemptsToRead"))
                this.msg_comm_attemptsToRead.Value = decimal.Parse(Params.Compatibility["msg_comm_attemptsToRead"].ToString());
            else
                _useDefaults = true;

            if (Params.Compatibility.ContainsKey("msg_comm_timeoutOnFail"))
                this.msg_comm_timeoutOnFail.Value = decimal.Parse(Params.Compatibility["msg_comm_timeoutOnFail"].ToString());
            else
                _useDefaults = true;

            if (Params.Compatibility.ContainsKey("msg_comm_attemptsToWait"))
                this.msg_comm_attemptsToWait.Value = decimal.Parse(Params.Compatibility["msg_comm_attemptsToWait"].ToString());
            else
                _useDefaults = true;

            if (Params.Compatibility.ContainsKey("msg_comm_timeoutOnBusy"))
                this.msg_comm_timeoutOnBusy.Value = decimal.Parse(Params.Compatibility["msg_comm_timeoutOnBusy"].ToString());
            else
                _useDefaults = true;

            _is_init = false;

            if (_useDefaults)
                SetupDefaults();
        }

        private void SetupDefaults()
        {
            this.msg_comm_attemptsToRead.Value = 20;
            this.msg_comm_timeoutOnFail.Value = 200;
            this.msg_comm_attemptsToWait.Value = 40;
            this.msg_comm_timeoutOnBusy.Value = 200;
        }

        private void msg_comm_ValueChanged(object sender, EventArgs e)
        {
            if (_is_init)
                return;
            Params.Compatibility["msg_comm_attemptsToRead"] = this.msg_comm_attemptsToRead.Value;
            Params.Compatibility["msg_comm_timeoutOnFail"] = this.msg_comm_timeoutOnFail.Value;
            Params.Compatibility["msg_comm_attemptsToWait"] = this.msg_comm_attemptsToWait.Value;
            Params.Compatibility["msg_comm_timeoutOnBusy"] = this.msg_comm_timeoutOnBusy.Value;
        }

        private void msg_comm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_is_init)
                return;
            Params.Compatibility["msg_comm_attemptsToRead"] = this.msg_comm_attemptsToRead.Value;
            Params.Compatibility["msg_comm_timeoutOnFail"] = this.msg_comm_timeoutOnFail.Value;
            Params.Compatibility["msg_comm_attemptsToWait"] = this.msg_comm_attemptsToWait.Value;
            Params.Compatibility["msg_comm_timeoutOnBusy"] = this.msg_comm_timeoutOnBusy.Value;
        }
    }
}
