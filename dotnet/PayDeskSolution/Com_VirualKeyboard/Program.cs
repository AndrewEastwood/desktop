﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace vk
{
    static class Program
    {
        public static int SleepTime { get; set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string appType = string.Empty;
            int wndIdx = -1;
            Program.SleepTime = 100;
            if (args.Length != 0)
            {
                if (args[0] != null)
                    appType = args[0];

                try
                {
                    if (args.Length > 1)
                        wndIdx = int.Parse(args.GetValue(1).ToString());
                    if (args.Length > 2)
                        Program.SleepTime = int.Parse(args.GetValue(2).ToString());
                }
                catch { }
            }
            // one copy only
            System.Threading.Mutex myMutex = null;
            try
            {
                myMutex = System.Threading.Mutex.OpenExisting("InTechPayDeskVirtualKeyboard" + appType);
            }
            catch { }

            if (myMutex != null)
            {
                Environment.Exit(0);
            }
            else
                myMutex = new System.Threading.Mutex(true, "InTechPayDeskVirtualKeyboard" + appType);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            switch (appType)
            {
                case "mini":
                    {
                        Application.Run(new Components.UI.Other.Com_VirtualKeyboard_Mini(wndIdx));
                        break;
                    }
                default:
                    {
                        Application.Run(new Components.UI.Other.Com_VirtualKeyboard(wndIdx));
                        break;
                    }
            }
        }
    }
}
