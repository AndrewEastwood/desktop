using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace components.Components.pdLogger
{
    public class pdLogger
    {
        private string realm;
        private string repDirectory;
        private Hashtable formatting;
        private static pdLogger instance;

        public pdLogger()
        {
            this.repDirectory = "reports";
            this.realm = "app";
        }

        public pdLogger(string realm)
            : this()
        {
            if (realm != null && realm.Length > 0)
                this.realm = realm;
        }


        public void Log(Exception ex)
        {
            Log(ex, string.Empty, this.ReportsDirectory, this.realm);
        }

        public void Log(string message)
        {
            this.Log(null, message, this.ReportsDirectory, this.realm);
        }

        public void Log(Exception ex, string message)
        {
            this.Log(ex, message, this.ReportsDirectory, this.realm);
        }

        public void Log(Exception ex, string message, string path, string realm)
        {
            string reportName = string.Format("{0}\\pd_report_{1}_{2}.log", path, realm, DateTime.Now.ToShortDateString());

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            FileStream fs = new FileStream(reportName, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            if (ex != null)
            {
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
                sw.WriteLine(string.Empty.PadLeft(40, '-'));
            }

            if (message != null && message.Length > 0)
                sw.WriteLine(message);

            sw.WriteLine(string.Empty.PadLeft(40, '='));

            sw.Close();
            sw.Dispose();
        }

        public static void Logme(Exception ex, string message, string realm)
        {
            if (instance == null)
            {
                instance = new pdLogger();
            }
            instance.Log(ex, message, instance.ReportsDirectory, realm);
        }

        public static void Logme(Exception ex)
        {
            if (instance == null)
            {
                instance = new pdLogger();
            }
            new pdLogger().Log(ex, string.Empty);
        }

        public static void Logme(Exception ex, string realm)
        {
            if (instance == null)
            {
                instance = new pdLogger();
            }
            new pdLogger().Log(ex, string.Empty, instance.ReportsDirectory, realm);
        }

        public static void Logme(string message)
        {
            if (instance == null)
            {
                instance = new pdLogger();
            }
            new pdLogger().Log(message);
        }

        public static void Logme(string message, string realm)
        {
            if (instance == null)
            {
                instance = new pdLogger(realm);
            }
            new pdLogger().Log(null, message, instance.ReportsDirectory, realm);
        }

        public string ReportsDirectory { get { return repDirectory; } set { repDirectory = value; } }
        public Hashtable ReportsFormat
        {
            get { return formatting; }
            set { formatting = value; }
        }
    }
}
