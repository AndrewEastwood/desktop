using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
//0using mdcore;
//using mdcore.API;
//0using mdcore.Lib;
//0using mdcore.Components.UI;
//0using mdcore.Config;
//0using PluginModule;
using PayDesk.Components.UI;
using components.Components.PluginManager;
using components.Components.XmlDocumentParser;
using driver;
using driver.Lib;
using driver.Components.UI;
using components.Public;
using components.Components.MMessageBox;
using System.Collections;
//using PayDesk.Components.UI.Config;
//0using xmldp.Public;

/*
 * PayDesk program.
 *  Used in shops and same places.
 *  
 * Features:
 *  Compatible with Market 2.7
 *  Support random ussers access of different possibilities
 *  Possibility full configuring and performing order's rules
 *  Support article's filtering
 *  Support "offline cheques" (used in restourants)
 *  Compatible with sacanners of barcode
 *  Compatible with termoprinters
 *  Compatible with fiscal printer devices
 *  Support distributed printing
 *  Support plugin modules
 *  
 */

namespace PayDesk
{
    /// <summary>
    /// The main class of PayDesk.
    /// </summary>
    static class Program
    {
        // Plugin library
        static private Com_PluginManager plugs;
        static public Com_PluginManager AppPlugins { get { return plugs; } }
        static public System.Collections.Hashtable MainArgs;
        //static public ApiManager appApi;
        //static public ApiManager ApiManager { get { return appApi; } }
        static public string ApplicationName;
        //static public ApplicationConfiguration axCfg;
        //static public mdcore.Config.ConfigManager stdCfg;

        [STAThread]
        static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            // get arguments
            Hashtable hargs = new Hashtable();
            components.Components.ArgumentParser.Com_ArgumentParser.TransformArguments(hargs, args);

            // load xml settings
            ApplicationConfiguration.CustomConfigurationMethod = CustomConfigurationSettingsContext;
            ApplicationConfiguration.Instance.LoadConfigurationData();

            // load bin settings
            driver.Config.ConfigManager.getInstance();
            if (!driver.Config.ConfigManager.LoadConfiguration())
                return;

            // mode only
            try
            {
                CoreLib.WriteLog(new Exception("Starting service mode for " + hargs["mode"]), "AppStartup (Main)");

                if (hargs.ContainsKey("mode"))
                {
                    // admin verification


                    DialogResult rez = DialogResult.None;
                    PayDesk.Components.UI.uiWndAdmin admin = new uiWndAdmin(FormStartPosition.CenterScreen);
                    rez = admin.ShowDialog();

                    // mode selector with admin
                    if (rez == DialogResult.OK)
                        switch (hargs["mode"].ToString())
                        {
                            case "settings":
                                {
                                    PayDesk.Components.UI.uiWndSettings wnd = new PayDesk.Components.UI.uiWndSettings();
                                    wnd.ShowDialog();
                                    break;
                                }
                            case "printer":
                                {
                                    PayDesk.Components.UI.uiWndPrinting wnd = new PayDesk.Components.UI.uiWndPrinting();
                                    wnd.ShowDialog();
                                    break;
                                }
                            case "billmanager":
                                {
                                    PayDesk.Components.UI.wndBills.uiWndBillManagercs wnd = new PayDesk.Components.UI.wndBills.uiWndBillManagercs();
                                    wnd.ShowDialog();
                                    break;
                                }
                        }

                    admin.Dispose();

                    return;
                }

            }
            catch { }

            ApplicationName = string.Empty;
            //if (args.Length != 0 && args[0] != null && args[0] != string.Empty)
            try
            {
                if (hargs.ContainsKey("app") && hargs["app"].ToString().Length != 0)
                    ApplicationName = args[0];
            }
            catch { }
            
            //axCfg = ApplicationConfiguration. ("display", @"default/config", (ApplicationName == "" ? "" : ApplicationName + @"/config"), (Program.ApplicationName != string.Empty));

            //MessageBox.Show("wait");


            //appApi = new ApiManager(args);
            /*
            if (!SecureLib.SetGetState(string.Empty))
            {
                Registration reg = new Registration();
                reg.ShowDialog();
                reg.Dispose();
                Environment.Exit(Environment.ExitCode);
            }
            */
            // in futrue move this to mdcore lib
            /* test
            if (MainArgs.ContainsKey("-c") && MainArgs["-c"] != null)
                AppConfig.LoadData(MainArgs["-c"].ToString());
            else
                AppConfig.LoadData();
            test */
            // [a] // AppConfig.LoadData();

            FileMgrLib.VerifyAllFolders();
            CoreLib.WriteLog(new Exception("It is not an error. This message used for tracking of app's startup only."), "AppStartup (Main)");

            // [a] // plugs = new PluginModule.Plugins(AppConfig.Path_Plugins);
            plugs = new Com_PluginManager(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Plugins);

            //one copy
            System.Threading.Mutex myMutex = null;
            try
            {
                myMutex = System.Threading.Mutex.OpenExisting("PayDeskMutex");
            }
            catch { }
            if (myMutex != null && driver.Config.ConfigManager.Instance.CommonConfiguration.APP_AllowOneCopy)
            {
                MMessageBox.Show("Вже запущена одна копія програми", Application.ProductName);
                Environment.Exit(0);
            }
            else
                myMutex = new System.Threading.Mutex(true, "PayDeskMutex");

            bool active = true;
            do
            {
                active = true;

                uiWndStartupDialog entry = new uiWndStartupDialog();
                active = entry.ShowDialog() == DialogResult.OK;
                entry.Dispose();

                if (!active)
                    break;

                switch (SecureLib.Authorize(entry.Password, entry.Login))
                {
                    case "service":
                        {
                            uiWndService serv = new uiWndService();
                            serv.ShowDialog();
                            serv.Dispose();
                            break;
                        }
                    case "main":
                        {
                            uiWndMain main = new uiWndMain();
                            if (main.ShowDialog() != DialogResult.Retry)
                                active = false;
                            main.Dispose();/*
                            if (Program.MainArgs.ContainsKey("-c") && Program.MainArgs["-c"] != null)
                                AppConfig.SaveData(Program.MainArgs["-c"].ToString());
                            else*/
                            driver.Config.ConfigManager.SaveConfiguration();
                            break;
                        }
                    default: { break; }
                }

            } while (active);

            CoreLib.WriteLog(new Exception("It is not an error. This message used for tracking of app's exit only."), "AppStartup (Main)");

            System.Diagnostics.Process[] prc = System.Diagnostics.Process.GetProcessesByName("VirtualKeyboard");
            foreach (System.Diagnostics.Process p in prc)
                p.Kill();

            System.Diagnostics.Process[] prcUpdater = System.Diagnostics.Process.GetProcessesByName("Updater");
            foreach (System.Diagnostics.Process p in prcUpdater)
                p.Kill();
        }

        private static void CustomConfigurationSettingsContext(Com_XmlDocumentParser_Configuration Settings)
        {
            // from prev
            //  axCfg = ApplicationConfiguration. ("display", @"default/config", (ApplicationName == "" ? "" : ApplicationName + @"/config"), (Program.ApplicationName != string.Empty));


            /* general app configuration */
            Settings.DocumentVersion = new Version(System.Windows.Forms.Application.ProductVersion);
            Settings.ConfigDirectoryPathGeneral = "display";
            Settings.ConfigDirectoryNameDefault = @"default/config";//.Format("v.{0}.{1}", _cfg.ApplicationVersion.Major, _cfg.ApplicationVersion.Minor);
            Settings.ConfigDirectoryNameApplication = string.Empty;// string.Empty;//.Format("v.{0}.{1}", _cfg.ApplicationVersion.Major, _cfg.ApplicationVersion.Minor);
            Settings.UseVersionForConfiguration = false;
            Settings.TrackVersionConfiguration = false;
            Settings.MergeData = false;

            /* profiles */
            if (components.Components.ArgumentParser.Com_ArgumentParser.Arguments.ContainsKey("-p"))
                Settings.ConfigDirectoryNameApplication = components.Components.ArgumentParser.Com_ArgumentParser.Arguments["-p"].ToString();

            if (Settings.ConfigDirectoryNameApplication.Length != 0)
                Settings.ConfigDirectoryNameApplication += @"/config"; 

        }

    }
}