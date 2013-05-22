using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Windows.Forms;

/*
 * There are a plugin collections
 * which are used in application.
 */

namespace components.Shared.Interfaces
{
    /// <summary>
    /// Main plugin interface.
    /// Base interface for other interfaces.
    /// </summary>
    public interface IPlugin
    {
        // Common Information
        string Name { get; }
        string Version { get; }
        string Author { get; }
    }

    /// <summary>
    /// Defulat plugin interface
    /// </summary>
    public interface IDefault : IPlugin
    {
        // Common Information
        string Name { get; }
        string Version { get; }
        string Author { get; }

        object Execute(string param);

        // ... under developing
    }

    /// <summary>
    /// Fixal Driver plugin.
    /// That plugin extend functionality of application
    /// and made possible to use fiscal printer device.
    /// </summary>
    public interface IFPDriver : IPlugin
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

    /// <summary>
    /// Service plugin interface
    /// </summary>
    public interface IService : IPlugin
    {
        // Common Information
        string Name { get; }
        string Version { get; }
        string Author { get; }

        // ... under developing
    }

    /// <summary>
    /// UI plugin interface
    /// </summary>
    public interface IAppUI : IPlugin
    {
        // Common Information
        string Name { get; }
        string Version { get; }
        string Author { get; }

        object Execute(params Control[] param);

        // ... under developing
    }

}
