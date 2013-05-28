using System;
using System.Collections.Generic;
using System.Text;
using components.Shared.Interfaces;
using System.Collections;

namespace components.Shared.Defaults
{
    public class DefaultPlugin : IPlugin
    {
        private InnerDefaultLegalPrinterDriverAssembly assemblyInfo;

        public DefaultPlugin(Hashtable pluginAssemblyInfo)
        {
            assemblyInfo = new InnerDefaultLegalPrinterDriverAssembly(pluginAssemblyInfo);
        }
        public DefaultPlugin()
            : this(new Hashtable())
        {
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
    }

    /// <summary>
    /// Represent information about plugin module
    /// </summary>
    class InnerDefaultLegalPrinterDriverAssembly
    {
        private Hashtable assemblyInfo;

        public InnerDefaultLegalPrinterDriverAssembly(Hashtable assemblyInfo)
        {
            this.assemblyInfo = assemblyInfo;
        }

        /// <summary>
        /// The name of plugin
        /// </summary>
        public string NAME
        {
            get
            {
                if (this.assemblyInfo.ContainsKey("NAME"))
                    return this.assemblyInfo["NAME"].ToString();
                return string.Empty;
            }
        }
        /// <summary>
        /// Title of plugin
        /// </summary>
        public string TITLE
        {
            get
            {
                if (this.assemblyInfo.ContainsKey("TITLE"))
                    return this.assemblyInfo["TITLE"].ToString();
                return string.Empty;
            }
        }
        /// <summary>
        /// Version of realized plugin functionlaity
        /// </summary>
        public string VERSION
        {
            get
            {
                if (this.assemblyInfo.ContainsKey("VERSION"))
                    return this.assemblyInfo["VERSION"].ToString();
                return string.Empty;
            }
        }
        /// <summary>
        /// Version of plugin module
        /// </summary>
        public string PLUGIN_VERSION
        {
            get
            {
                if (this.assemblyInfo.ContainsKey("PLUGIN_VERSION"))
                    return this.assemblyInfo["PLUGIN_VERSION"].ToString();
                return string.Empty;
            }
        }
        /// <summary>
        /// Author's name
        /// </summary>
        public string AUTHOR
        {
            get
            {
                if (this.assemblyInfo.ContainsKey("AUTHOR"))
                    return this.assemblyInfo["AUTHOR"].ToString();
                return string.Empty;
            }
        }
    }
}
