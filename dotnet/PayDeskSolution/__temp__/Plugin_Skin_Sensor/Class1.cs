using System;
using System.Collections.Generic;
using System.Text;

using PluginModule.Components;
using PluginModule.Components.Objects;
using PluginModule;
using System.Windows.Forms;

namespace pgui_sensor
{
    [PluginSimpleAttribute(PluginType.AppUI)]
    public class GUI_Sensor : IAppUI
    {
        #region IAppUI Members

        public string Name
        {
            get { return "GUI_Sensor"; }
        }

        public string Version
        {
            get { return "1.0.0.1"; }
        }

        public string Author
        {
            get { return "J.Bo"; }
        }

        public object Execute(params Control[] obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
