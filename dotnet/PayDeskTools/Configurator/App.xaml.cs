using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using components.Components.XmlDocumentParser;

namespace Configurator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void ConfigurationSettingsContext(Com_XmlDocumentParser_Configuration Settings)
        {
            /* general app configuration */

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            Settings.DocumentVersion = assembly.GetName().Version;
            Settings.ConfigDirectoryPathGeneral = AppDomain.CurrentDomain.BaseDirectory + "\\config";
            Settings.ConfigDirectoryNameDefault = string.Empty;//.Format("v.{0}.{1}", _cfg.ApplicationVersion.Major, _cfg.ApplicationVersion.Minor);
            Settings.ConfigDirectoryNameApplication = "configurator";// string.Empty;//.Format("v.{0}.{1}", _cfg.ApplicationVersion.Major, _cfg.ApplicationVersion.Minor);
            Settings.UseVersionForConfiguration = false;
            Settings.TrackVersionConfiguration = false;
            Settings.MergeData = false;

            /* profiles */
            // if (components.Components.ArgumentParser.Com_ArgumentParser.Arguments.ContainsKey("-p"))
            //    Settings.ConfigDirectoryNameApplication = components.Components.ArgumentParser.Com_ArgumentParser.Arguments["-p"].ToString();
        }
    }

           
}
