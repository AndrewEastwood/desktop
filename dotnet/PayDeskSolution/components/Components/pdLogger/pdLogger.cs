using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace components.Components.pdLogger
{
    public class pdLogger
    {
        private string repDirectory;
        private Hashtable formatting;

        public pdLogger()
        {
            this.repDirectory = "reports";
        }


        public void Log(Exception ex)
        {
            Log(ex, string.Empty, repDirectory);
        }

        public void Log(Exception ex, string message)
        {
            this.Log(ex, message, this.repDirectory);
        }

        public void Log(Exception ex, string message, string path)
        {
            string reportName = string.Format("{0}\\pd_report_{1}.log", path, DateTime.Now.ToShortDateString());

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileStream fs = new FileStream(reportName, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            sw.WriteLine("at Method: " + ex.Data);
            sw.WriteLine(ex.Message);
            sw.WriteLine(ex.StackTrace);
            sw.WriteLine(ex.InnerException);
            sw.WriteLine(ex.Source);
            sw.WriteLine(ex.HelpLink);
            sw.WriteLine(string.Empty.PadLeft(40, '*'));
            sw.WriteLine("Environment:");
            sw.WriteLine(Environment.OSVersion);
            sw.WriteLine(Environment.MachineName);
            sw.WriteLine(Environment.UserName);
            sw.WriteLine(Environment.Version);
            sw.WriteLine(string.Empty.PadLeft(40, '='));

            sw.Close();
            sw.Dispose();
        }

        public static void Logme(Exception ex, string message)
        {
            new pdLogger().Log(ex, message);
        }

        public static void Logme(Exception ex)
        {
            new pdLogger().Log(ex, string.Empty);
        }

        public string ReportsDirectory { get { return repDirectory; } set { repDirectory = value; } }
        public Hashtable ReportsFormat
        {
            get { return formatting; }
            set { formatting = value; }
        }
    }
}
