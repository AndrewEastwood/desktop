using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Updater
{
    public partial class DataSyncProfile : UserControl
    {
        public string PropfileName { get; set; }
        public bool IsDefaultProfile { get; set; }
        private bool _stopEventProfileNameChange;

        public delegate void ProfileNameChanged(string newProfileName, DataSyncProfile sender, EventArgs e);
        public event ProfileNameChanged OnProfileNameChange;

        public DataSyncProfile(string profileName, bool isDefault)
        {
            InitializeComponent();
            PropfileName = profileName;
            this.sync_profileName.Text = PropfileName;
            IsDefaultProfile = isDefault;
            this.sync_profileName.Enabled = !IsDefaultProfile;
        }
        public DataSyncProfile()
            : this("profile-default", true)
        {
            InitializeComponent();
            //CollectAndSetProfileName
        }

        private void sync_profileName_TextChanged(object sender, EventArgs e)
        {
            if (_stopEventProfileNameChange)
                return;

            _stopEventProfileNameChange = true;

            if (this.sync_profileName.Text.Length == 0)
                this.sync_profileName.Text = "profile-empty";

            // remove all unnecessary characters
            PropfileName = this.sync_profileName.Text.Replace("_", "").Replace(" ", "").Trim();
            this.sync_profileName.Text = PropfileName;

            if (OnProfileNameChange != null)
                OnProfileNameChange(PropfileName, this, e);

            _stopEventProfileNameChange = false;
        }
    }
}
