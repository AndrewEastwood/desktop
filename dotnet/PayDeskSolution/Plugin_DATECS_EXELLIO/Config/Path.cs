using System;
using System.Collections.Generic;
using System.Text;

namespace DATECS_EXELLIO.Config
{
    /// <summary>
    /// Configuration class
    /// Contains shared fileds with secificated values 
    /// </summary>
    public class Path
    {
        #region Main constats
        /// <summary>
        /// The full name of plugin module
        /// </summary>
        public const string MODULE_NAME = Config.Assembly.NAME + ".dll";
        /// <summary>
        /// Configuration file of commmunication port
        /// </summary>
        public const string CFG_PORT = Config.Assembly.NAME + ".xml";
        /// <summary>
        /// Configuration file of plugin module
        /// </summary>
        public const string CFG_PLUG = Config.Assembly.NAME + ".smf";
        /// <summary>
        /// Configuration file of plugin module
        /// </summary>
        public const string CFG_PARAM = Config.Assembly.NAME + "P.xml";
        /// <summary>
        /// Directory separator character
        /// </summary>
        public const char DS = '/';
        #endregion

        #region Complicated properties
        private static string _startupDir = "";
        /// <summary>
        /// Return full path of startup folder
        /// whitch contained this module
        /// </summary>
        public static string STARTUP_DIR
        {
            get
            {
                if (System.IO.File.Exists(_startupDir + DS + MODULE_NAME))
                    return _startupDir;

                _startupDir = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                _startupDir = _startupDir.Substring(0, _startupDir.LastIndexOf(DS));
                _startupDir = _startupDir.Substring(_startupDir.IndexOf("///") + 3);

                return _startupDir;
            }
        }
        /// <summary>
        /// Return full path of port configuration
        /// </summary>
        public static string FULL_CFG_PORT_PATH { get { return STARTUP_DIR + DS + CFG_PORT; } }
        /// <summary>
        /// Return full path of plugin configuration
        /// </summary>
        public static string FULL_CFG_PLUG_PATH { get { return STARTUP_DIR + DS + CFG_PLUG; } }
        /// <summary>
        /// Return full path of plugin configuration
        /// </summary>
        public static string FULL_CFG_PARAM_PATH { get { return STARTUP_DIR + DS + CFG_PARAM; } }
        #endregion
    }
}
