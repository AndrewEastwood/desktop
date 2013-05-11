using System;
using System.Collections.Generic;
using System.Text;

namespace driver.Components.API
{
    public class PdApi
    {
        // in future all config files will be stored in different display folders
        const string API_PARAM_CONFIGFILE = "-c";

        // name of active display folder
        const string API_PARAM_DISPLAY = "-d";

        // load app in pointed mode:
        //   -load user
        //    app will load only user screen. Others logins will be ignored.
        //   -load service
        //    app will load only service screen.
        //   -load default
        //    standart login mode.
        //   -load config
        //    load app settings window througth main window. (Need admin password)
        const string API_PARAM_LOAD = "-load";
    }
}
