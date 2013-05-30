using System;
using System.Collections.Generic;
using System.Text;

namespace components.Shared.Attributes
{
    /// <summary>
    /// That enum used for indication of specific plugin structure and it usage in application
    /// </summary>
    public enum PluginType
    {
        /// <summary>
        /// Inticate driver type for fiscal printer device.
        /// </summary>
        LegalPrinterDriver = 0,
        /// <summary>
        /// Indicate plugin for service mode of application.
        /// </summary>
        Service = 1,
        /// <summary>
        /// User interface plugin
        /// </summary>
        AppUI = 2,
        /// <summary>
        /// Other plugin
        /// </summary>
        Default = 3
    };

    /// <summary>
    /// That enum used for indication how plugins must be execution in application
    /// </summary>
    public enum PluginMode
    {
        /// <summary>
        /// Indicate single plugin mode
        /// Plugins of that mode can't be performing together
        /// The singleloaders are these types as:
        ///     FPDriver
        /// </summary>
        SingleLoader,
        /// <summary>
        /// Indicate multiloaders plugin mode
        /// Plugins of that mode can be performing together
        /// The multiloaders are these types as:
        ///     Service,
        ///     AppUI,
        ///     Default
        /// </summary>
        MultiLoader
    }
    
    /// <summary>
    /// Identificate type of plgin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PluginSimpleAttribute : Attribute
    {        
        private PluginType _Type;

        //
        public PluginSimpleAttribute(PluginType T) { _Type = T; }

        //
        public PluginType Type { get { return _Type; } }
    };

    /// <summary>
    /// Identificate type of plgin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PluginExtendedAttribute : Attribute
    {
        private PluginType _Type;
        private PluginMode _Mode;

        //
        public PluginExtendedAttribute(PluginType T, PluginMode M)
        {
            if (T == PluginType.LegalPrinterDriver && M == PluginMode.MultiLoader)
                throw new Exception("Type FPDriver can't be used in multiloader plugin");
            _Type = T;
            _Mode = M;
        }

        //
        public PluginType Type { get { return _Type; } }
        public PluginMode Mode { get { return _Mode; } }
    };

}
