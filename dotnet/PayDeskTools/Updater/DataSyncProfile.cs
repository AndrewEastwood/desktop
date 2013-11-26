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
        public string ProfileName { get; set; }
        public string ProfileDisplayName { get { return this.sync_profileDisplayText.Text; } set { this.sync_profileDisplayText.Text = value; } }
        public bool IsDefaultProfile { get; set; } 
        //private bool _stopEventProfileNameChange;

        public delegate void ProfileNameChanged(string newProfileName, DataSyncProfile sender, EventArgs e);
        public event ProfileNameChanged OnProfileNameChange;

        public DataSyncProfile(string profileName, bool isDefault)
        {
            InitializeComponent();
            ProfileName = profileName;
            IsDefaultProfile = isDefault;
        }
        public DataSyncProfile()
            : this("profile-default", true)
        {
            InitializeComponent();
            //CollectAndSetProfileName
        }

        private void sync_profileDisplayText_TextChanged(object sender, EventArgs e)
        {
            if (this.sync_profileDisplayText.Text.Length == 0)
                this.sync_profileDisplayText.Text = "Новий профіль";
            if (OnProfileNameChange != null)
                OnProfileNameChange(this.sync_profileDisplayText.Text, this, e);
        }

        private void label2_Click(object sender, EventArgs e)
        {


        }

        //private void sync_profileName_TextChanged(object sender, EventArgs e)
        //{
        //    if (_stopEventProfileNameChange)
        //        return;

        //    _stopEventProfileNameChange = true;

        //    if (this.sync_profileDisplayText.Text.Length == 0)
        //        this.sync_profileDisplayText.ResetText();

        //    if (OnProfileNameChange != null)
        //        OnProfileNameChange(PropfileName, this, e);

        //    _stopEventProfileNameChange = false;
        //}
    }
}
