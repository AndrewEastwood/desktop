using System;
using System.Collections.Generic;
using System.Text;

namespace components.Controls.AppSettingsWindow
{
    public class AppSettigsContext : components.Public.AppXmlConfig
    {
        private static AppSettigsContext _inst;

        private AppSettigsContext()
        {
            this.XmlParser.Settings.ApplicationConfigDirectory = string.Empty;
            this.XmlParser.Settings.DefaultConfigDirectory = string.Empty;
            this.XmlParser.Settings.GeneralConfigDirectory = System.Windows.Forms.Application.StartupPath + "\\config";
            this.XmlParser.Settings.CheckForDocumentVersion = false;
            this.XmlParser.Settings.UseVersionForConfiguration = false;
            this.ReloadConfiguration(false);
        }

        public static AppSettigsContext Instance
        {
            get
            {
                if (_inst == null)
                    _inst = new AppSettigsContext();
                return _inst;
            }
        }

    }
}
