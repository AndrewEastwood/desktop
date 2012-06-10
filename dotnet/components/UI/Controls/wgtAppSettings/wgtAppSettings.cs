using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using components.Public;

namespace components.UI.Controls.wgtAppSettings
{
    public partial class wgtAppSettings : UserControl
    {
        public wgtAppSettings()
        {
            InitializeComponent();
            SaveSettingsOnClose = true;
        }

        /* PROPERTIES */

        public bool SaveSettingsOnClose { get; set; }

        public static ApplicationConfiguration ApplicationSettingsContext
        {
            get
            {
                return ApplicationConfiguration.Instance;
            }
        }

        private void wgtAppSettings_Load(object sender, EventArgs e)
        {
            LoadControlSettings();
        }

        private void wgtAppSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SaveSettingsOnClose)
                SaveControlSettings();
        }

        /* METHODS */

        public void SaveControlSettings()
        {
            ApplicationSettingsContext.CollectValuesFromAllControls(this);
            ApplicationSettingsContext.SaveConfigurationData();
        }
        public void LoadControlSettings()
        {
            ApplicationSettingsContext.BindValuesToAllControls(this);
        }
    }
}
