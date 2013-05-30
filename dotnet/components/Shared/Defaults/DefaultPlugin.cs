using System;
using System.Collections.Generic;
using System.Text;
using components.Shared.Interfaces;
using System.Collections;

namespace components.Shared.Defaults
{
    public class DefaultPlugin// : IPlugin
    {
        private DefaultPluginAssembly assemblyInfo;

        public DefaultPlugin(Hashtable pluginAssemblyInfo)
        {
            assemblyInfo = new DefaultPluginAssembly(pluginAssemblyInfo);
        }
        public DefaultPlugin()
        {
            assemblyInfo = new DefaultPluginAssembly();
        }

        public DefaultPluginAssembly Assembly
        {
            get { return assemblyInfo; }
        }
    }
}
