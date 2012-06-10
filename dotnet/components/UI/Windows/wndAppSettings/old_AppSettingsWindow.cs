using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace components.Controls.AppSettingsWindow
{
    public partial class AppSettingsWindow : Form
    {
        public AppSettigsContext ApplicationSettingsContext { get; set; }
        public bool SaveSettings { get; set; }

        public AppSettingsWindow()
        {
            InitializeComponent();

            ApplicationSettingsContext = AppSettigsContext.Instance;
        }

        private void AppSettingsWindow_Load(object sender, EventArgs e)
        {
            ApplicationSettingsContext.BindConfigurationValues(this.Controls.Owner);
        }

        private void AppSettingsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.SaveSettings)
                SaveAllSettings();
        }

        public void SaveAllSettings()
        {
            ApplicationSettingsContext.CollectConfigurationValues(this.Controls.Owner);
            ApplicationSettingsContext.StoreConfigData();
        }
    }
}
