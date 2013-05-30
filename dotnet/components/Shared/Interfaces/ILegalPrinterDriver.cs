using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace components.Shared.Interfaces
{
    public interface ILegalPrinterDriver: IPlugin
    {
        // Common Information
        /// <summary>
        /// Show plugin's name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Show plugin's version
        /// </summary>
        string Version { get; }
        /// <summary>
        /// Show author's version
        /// </summary>
        string Author { get; }

        // Main Access Method
        object CallFunction(string name, params object[] param);

        // Additional Properties
        Hashtable AllowedMethods { get; }

        // UI
        System.Windows.Forms.UserControl DriverUI { get; }
        System.Windows.Forms.UserControl PortUI { get; }
        System.Windows.Forms.UserControl CompatibilityUI { get; }

        //
        bool Activate();
        bool Deactivate();
    }
}
