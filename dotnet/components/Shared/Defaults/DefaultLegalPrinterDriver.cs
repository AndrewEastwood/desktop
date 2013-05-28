using System;
using System.Collections.Generic;
using System.Text;
using components.Shared.Attributes;
using components.Shared.Interfaces;
using System.Windows.Forms;
using System.Collections;

namespace components.Shared.Defaults
{
    [PluginSimpleAttribute(PluginType.ILegalPrinterDriver)]
    public class DefaultLegalPrinterDriver : DefaultPlugin
    {
        


        // Hooks

        protected virtual UserControl _hook_control_driverUI()
        {
            throw new Exception("Implement DriverUI component");
        }

        protected virtual UserControl _hook_control_portUI()
        {
            throw new Exception("Implement PortUI component");
        }

        protected virtual UserControl _hook_control_settingsUI()
        {
            throw new Exception("Implement SettingsUI component");
        }

        // = PROPERTIES

        // Service Controls
        public UserControl DriverUI
        {
            get
            {
                return _hook_control_driverUI();
            }
        }
        public UserControl PortUI
        {
            get
            {
                return _hook_control_portUI();
            }
        }
        public UserControl CompatibilityUI
        {
            get
            {
                return _hook_control_settingsUI();
            }
        }


        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string Version
        {
            get { throw new NotImplementedException(); }
        }

        public string Author
        {
            get { throw new NotImplementedException(); }
        }

        public object CallFunction(string name, params object[] param)
        {
            throw new NotImplementedException();
        }

        public System.Collections.Hashtable AllowedMethods
        {
            get { throw new NotImplementedException(); }
        }

        public bool Activate()
        {
            throw new NotImplementedException();
        }

        public bool Deactivate()
        {
            throw new NotImplementedException();
        }
    }


}
