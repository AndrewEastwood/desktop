using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace PayDesk.Components.UI.Config
{
    public class AppXmlConfig : xmldp.Components.Objects.xmlConfiguratoin
    {
        public AppXmlConfig()
        {
            /* setting up configuration directories */
            this.XmlParser = new xmldp.xParser();
            this.XmlParser.ConfigDir = @"display";
            this.XmlParser.ConfigDefaultDir = @"default/config";
            if (Program.ApplicationName != string.Empty)
                this.XmlParser.ConfigAppDir = Program.ApplicationName + @"/config";
            else
                this.XmlParser.ConfigAppDir = this.XmlParser.ConfigDefaultDir;
            /* getting configuration data */
            this.BindConfigData(Program.ApplicationName != string.Empty);
        }
    }
}
