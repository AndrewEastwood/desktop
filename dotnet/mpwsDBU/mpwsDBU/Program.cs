using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using components.Public;
using components.Components.XmlDocumentParser;

namespace mpwsDBU
{
    static class Program
    {
        private static string ConfigurationProfileName { get; set; }
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            components.Components.ArgumentParser.Com_ArgumentParser.TransformArguments(args);

            ApplicationConfiguration.CustomConfigurationMethod = CustomConfigurationSettingsContext;
            ApplicationConfiguration.Instance.LoadConfigurationData();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new com.SplashScreen());
            Application.Run(new Form1());
        }

        private static void CustomConfigurationSettingsContext(Com_XmlDocumentParser_Configuration Settings)
        {
            /* general app configuration */
            Settings.DocumentVersion = new Version(System.Windows.Forms.Application.ProductVersion);
            Settings.ConfigDirectoryPathGeneral = System.Windows.Forms.Application.StartupPath + "\\config";
            Settings.ConfigDirectoryNameDefault = string.Empty;//.Format("v.{0}.{1}", _cfg.ApplicationVersion.Major, _cfg.ApplicationVersion.Minor);
            Settings.ConfigDirectoryNameApplication = "myapp_1";// string.Empty;//.Format("v.{0}.{1}", _cfg.ApplicationVersion.Major, _cfg.ApplicationVersion.Minor);
            Settings.UseVersionForConfiguration = true;
            Settings.TrackVersionConfiguration = true;
            Settings.MergeData = false;

            /* profiles */
            if (components.Components.ArgumentParser.Com_ArgumentParser.Arguments.ContainsKey("-p"))
                Settings.ConfigDirectoryNameApplication = components.Components.ArgumentParser.Com_ArgumentParser.Arguments["-p"].ToString();
        }
    }
}
