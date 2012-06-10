using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using components.Public;

namespace mpwsDBU.lib
{
    public class Configuration : AppXmlConfig
    {
        private static Configuration _cfg;

        private Configuration(bool forceLoad)
            : base(forceLoad)
        {
        }

        public static Configuration Instance
        {
            get
            {
                if (_cfg == null)
                {
                    _cfg = new Configuration(false);

                    _cfg.XmlParser.Settings.DocumentVersion = new Version(System.Windows.Forms.Application.ProductVersion);

                    _cfg.XmlParser.Settings.GeneralConfigDirectory = System.Windows.Forms.Application.StartupPath + "\\config";
                    _cfg.XmlParser.Settings.DefaultConfigDirectory = "default";//.Format("v.{0}.{1}", _cfg.ApplicationVersion.Major, _cfg.ApplicationVersion.Minor);
                    _cfg.XmlParser.Settings.ApplicationConfigDirectory = "mpws";// string.Empty;//.Format("v.{0}.{1}", _cfg.ApplicationVersion.Major, _cfg.ApplicationVersion.Minor);
                    _cfg.XmlParser.Settings.UseVersionForConfiguration = true;
                    _cfg.ReloadConfiguration(true);
                }
                return _cfg;
            }
        }
    }
}
