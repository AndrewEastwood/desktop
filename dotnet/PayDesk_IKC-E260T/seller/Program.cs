using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using mdcore;
//using mdcore.API;
using mdcore.Lib;
using mdcore.Components.UI;
using mdcore.Config;
using PayDesk.Components.UI;

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
    static class Program
    {
        //Libs
        static private FPService.FPService service;
        static public FPService.FPService Service { get { return service; } }

        [STAThread]
        static void Main(string[] args)
        {

            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();


            //MMessageBox.Show("holder", Application.ProductName);


            service = new FPService.FPService();

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
            AppConfig.LoadData();
            winapi.WApi.OutputDebugString("after AppConfig.LoadData()");

            CoreLib.VerifyAllFolders();
            winapi.WApi.OutputDebugString("after CoreLib.VerifyAllFolders()");

            CoreLib.WriteLog(new Exception("It is not an error. This message used for tracking of app's startup only."), "AppStartup (Main)");

            //one copy
            System.Threading.Mutex myMutex = null;
            try
            {
                myMutex = System.Threading.Mutex.OpenExisting("PayDeskMutex");
                winapi.WApi.OutputDebugString("mutex received");
            }
            catch { }

            try
            {
                if (myMutex != null && AppConfig.APP_AllowOneCopy)
                {
                    MMessageBox.Show("Вже запущена одна копія програми", Application.ProductName);
                    Environment.Exit(0);
                }
                else
                    myMutex = new System.Threading.Mutex(true, "PayDeskMutex");
            }
            catch (Exception ex)
            {
                winapi.WApi.OutputDebugString(ex.Message);
            }

            bool active = true;
            do
            {
                active = true;

                uiWndStartupDialog entry = new uiWndStartupDialog();
                active = entry.ShowDialog() == DialogResult.OK;
                entry.Dispose();

                if (!active)
                    break;

                winapi.WApi.OutputDebugString("before main action switcher");
                switch (CoreLib.Authorize(entry.Password, entry.Login))
                {
                    case "service":
                        {
                            winapi.WApi.OutputDebugString("before service windoow");
                            uiWndService serv = new uiWndService();
                            serv.ShowDialog();
                            serv.Dispose();
                            break;
                        }
                    case "main":
                        {
                            winapi.WApi.OutputDebugString("before main windoow");
                            uiWndMain main = null;
                            try
                            {
                                main = new uiWndMain();
                                if (main.ShowDialog() != DialogResult.Retry)
                                    active = false;
                            }
                            catch(Exception ex)
                            {
                                CoreLib.WriteLog(ex, "AppStartup inside switcher (Main)");

                            }
                            finally
                            {
                                if (main != null)
                                    main.Dispose();
                            }
                            /*
                            if (Program.MainArgs.ContainsKey("-c") && Program.MainArgs["-c"] != null)
                                AppConfig.SaveData(Program.MainArgs["-c"].ToString());
                            else*/
                            AppConfig.SaveData();
                            winapi.WApi.OutputDebugString("after saving configuration");
                            break;
                        }
                    default: { break; }
                }

            } while (active);

            CoreLib.WriteLog(new Exception("It is not an error. This message used for tracking of app's exit only."), "AppStartup (Main)");

        }

    }
}