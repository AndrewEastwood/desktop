using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using mdcore;

namespace PayDesk
{
    static class Program
    {
        //Libs
        static private FPService.FPService service;
        static public FPService.FPService Service { get { return service; } }

        [STAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
            service = new FPService.FPService();

            if (!Active.SetGetState(string.Empty))
            {
                Registration reg = new Registration();
                reg.ShowDialog();
                reg.Dispose();
                Environment.Exit(Environment.ExitCode);
            }

            AppConfig.LoadData();
            AppFunc.VerifyAllFolders();

            //one copy
            System.Threading.Mutex myMutex = null;
            try
            {
                myMutex = System.Threading.Mutex.OpenExisting("PayDeskMutex");
            }
            catch { }
            if (myMutex != null && AppConfig.APP_AllowOneCopy)
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

                StartupDialog entry = new StartupDialog();
                active = entry.ShowDialog() == DialogResult.OK;
                entry.Dispose();

                if (!active)
                    break;

                switch (AppFunc.Authorize(entry.Password, entry.Login))
                {
                    case "service":
                        {
                            Service serv = new Service();
                            serv.ShowDialog();
                            serv.Dispose();
                            break;
                        }
                    case "main":
                        {
                            Main main = new Main();
                            if (main.ShowDialog() != DialogResult.Retry)
                                active = false;
                            main.Dispose();
                            AppConfig.SaveData();
                            break;
                        }
                    default: { break; }
                }

            } while (active);

            try
            {
                service.ComPort.Close();
            }
            catch { }
        }

    }
}