using System;
using System.Collections.Generic;
using System.Text;
using components.UI.Windows.AppSettingsWindow;
using components.Components.XmlDocumentParser;

namespace components.Public
{
    /* user mode interface */
    public class ApplicationConfiguration : components.Shared.BaseObjects.Obj_xmlConfiguratoin
    {
        private static ApplicationConfiguration _instance;
        private static bool _useCustomConfigurationMethod;

        static ApplicationConfiguration()
        {
            //Instance = AppSettingsWindowContext.Instance;
            //Settings = AppSettingsWindowContext.Settings;
            _useCustomConfigurationMethod = true;
        }

        private ApplicationConfiguration() { }

        public delegate void CustomConfigurationDelegate(Com_XmlDocumentParser_Configuration Settings);
        public static CustomConfigurationDelegate CustomConfigurationMethod { get; set; }

        /* customzied instance */
        public static ApplicationConfiguration InstanceCustomSettings
        {
            get
            {
                if (_instance == null)
                    _instance = new ApplicationConfiguration();

                /* this custom method always will be invoked */
                if (CustomConfigurationMethod != null)
                    CustomConfigurationMethod.Invoke(Settings);
                return _instance;
            }
        }

        /* simple references */
        public static ApplicationConfiguration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ApplicationConfiguration();

                /* use this method only once */
                if (_useCustomConfigurationMethod && CustomConfigurationMethod != null)
                {
                    _useCustomConfigurationMethod = false;
                    CustomConfigurationMethod.Invoke(Settings);
                }
                return _instance;
            }
        }
        public static Com_XmlDocumentParser_Configuration Settings
        {
            get
            {
                return _instance.XmlParser.Settings;
            }
            set
            {
                _instance.XmlParser.Settings = value;
            }
        }
        
    }
}
