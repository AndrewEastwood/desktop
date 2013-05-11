using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using components.Public;

namespace components.UI.Windows.AppSettingsWindow
{
    public partial class AppSettingsWindow : Form
    {

        public bool SaveSettingsOnClose { get; set; }
        /* simple settings instance reference */
        public ApplicationConfiguration ApplicationSettingsContext { get; set; }

        public AppSettingsWindow()
        {
            InitializeComponent();
            ApplicationSettingsContext = ApplicationConfiguration.Instance;
        }

        private void AppSettingsWindow_Load(object sender, EventArgs e)
        {
            LoadControlSettings();
        }

        private void AppSettingsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SaveSettingsOnClose)
                SaveControlSettings();
        }

        public void SaveControlSettings()
        {
            ApplicationSettingsContext.CollectValuesFromAllControls(this.Controls.Owner);
            ApplicationSettingsContext.SaveConfigurationData();
        }
        public void LoadControlSettings()
        {
            ApplicationSettingsContext.BindValuesToAllControls(this.Controls.Owner);
        }
    }
}
