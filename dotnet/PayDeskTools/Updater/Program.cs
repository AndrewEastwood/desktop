using System;
using System.Collections.Generic;
using System.Text;
using components.Components.XmlDocumentParser;
using components.Public;
using System.Windows.Forms;

namespace Updater
{
    class Program
    {
        private static string ConfigurationProfileName { get; set; }

        [STAThread]
        static void Main(string[] args)
        {

            System.Threading.Mutex myMutex = null;
            try
            {
                myMutex = System.Threading.Mutex.OpenExisting("PayDeskUpdater");
            }
            catch { }
            if (myMutex != null)
            {
                MessageBox.Show("Вже запущена одна копія програми", Application.ProductName);
                Environment.Exit(0);
            }
            else
                myMutex = new System.Threading.Mutex(true, "PayDeskUpdater");

            ApplicationConfiguration.CustomConfigurationMethod = CustomConfigurationSettingsContext;
            ApplicationConfiguration.Instance.LoadConfigurationData();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new wndMain());
        }

        private static void CustomConfigurationSettingsContext(Com_XmlDocumentParser_Configuration Settings)
        {

            /* general app configuration */
            Settings.DocumentVersion = new Version(System.Windows.Forms.Application.ProductVersion);
            Settings.ConfigDirectoryPathGeneral = System.Windows.Forms.Application.StartupPath + "\\display";
            // Settings.ConfigDirectoryNameDefault = "default";//.Format("v.{0}.{1}", _cfg.ApplicationVersion.Major, _cfg.ApplicationVersion.Minor);
            Settings.ConfigDirectoryNameApplication = "\\default\\updater";// string.Empty;//.Format("v.{0}.{1}", _cfg.ApplicationVersion.Major, _cfg.ApplicationVersion.Minor);
            Settings.UseVersionForConfiguration = false;
            Settings.TrackVersionConfiguration = false;
            Settings.MergeData = false;

            /* profiles */
            // if (components.Components.ArgumentParser.Com_ArgumentParser.Arguments.ContainsKey("-p"))
            //    Settings.ConfigDirectoryNameApplication = components.Components.ArgumentParser.Com_ArgumentParser.Arguments["-p"].ToString();
        }
    }
}
