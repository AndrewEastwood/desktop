using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using components.Public;
using System.IO;

namespace Updater
{
    public partial class wndMain : Form
    {
        public wndMain()
        {
            InitializeComponent();
            try
            {
                this.timer1.Interval = ApplicationConfiguration.Instance.GetValueByPath<int>("sync.fetchTimer");
            }
            catch { }
            this.timer1.Start();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 wndAbout = new AboutBox1();
            wndAbout.StartPosition = FormStartPosition.CenterScreen;
            wndAbout.ShowDialog();
        }

        private void forceSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1_Tick(timer1, EventArgs.Empty);
        }

        private void reloadConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplicationConfiguration.Instance.ReloadConfigurationData();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            // notifyIcon1.ShowBalloonTip(500, Application.ProductName, "Data Sync Started", ToolTipIcon.Info);

            // loop through remote files
            string[] files = ApplicationConfiguration.Instance["sync.monitorFiles"].ToString().Split('\n');
            string remotePathBase = ApplicationConfiguration.Instance["sync.remotePath"].ToString();
            string localPathBase = ApplicationConfiguration.Instance["sync.localPath"].ToString();
            FileInfo localRemoteFileInfo = new FileInfo(localPathBase + @"\info.txt");
            Dictionary<string, DateTime> _filenNameToDate = new Dictionary<string, DateTime>();
            Dictionary<string, DateTime> _filenNameToDatNew = new Dictionary<string, DateTime>();
            bool copyAllFiles = false;

            if (files.Length == 0)
            {
                timer1.Start();
                return;
            }

            if (localRemoteFileInfo.Exists)
            {
                string[] remoteFlesInfo = System.IO.File.ReadAllLines(localRemoteFileInfo.FullName);
                for (int i = 0, len = remoteFlesInfo.Length; i < len; i++)
                {
                    string[] fileNameAndDateTime = remoteFlesInfo[i].Split('#');
                    if (fileNameAndDateTime.Length == 2)
                        _filenNameToDate.Add(fileNameAndDateTime[0], new DateTime(long.Parse(fileNameAndDateTime[1])));
                }
            }
            else
            {
                copyAllFiles = true;
            }

            int downloadedFiles = 0;
            int removedFiles = 0;
            int unchangedFiles = 0;

            for (int i = 0, len = files.Length; i < len; i++)
            {
                FileInfo localStorageInfo = new FileInfo(localPathBase);
                FileInfo remoteFileInfo = new FileInfo(remotePathBase + "\\" + files[i]);
                FileInfo localFileInfo = new FileInfo(localPathBase + "\\" + files[i]);

                // create loca dir when it does not exsist
                if (!System.IO.Directory.Exists(localStorageInfo.FullName))
                    System.IO.Directory.CreateDirectory(localStorageInfo.FullName);

                // skip unexisted remote file
                if (!remoteFileInfo.Exists)
                {
                    removedFiles++;
                    continue;
                }

                // we do this when we do not have local info file about remote files
                if (copyAllFiles)
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[files[i]] = remoteFileInfo.LastWriteTimeUtc;
                    downloadedFiles++;
                    continue;
                }

                // we have remote file but we do not have it on local 
                if (!localFileInfo.Exists)
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[files[i]] = remoteFileInfo.LastWriteTimeUtc;
                    downloadedFiles++;
                    continue;
                }

                if (!_filenNameToDate.ContainsKey(files[i]))
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[files[i]] = remoteFileInfo.LastWriteTimeUtc;
                    downloadedFiles++;
                    continue;
                }

                // do compare dates and compy when the remote one is newer than it was before
                if (remoteFileInfo.LastWriteTimeUtc.Ticks > _filenNameToDate[files[i]].Ticks)
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[files[i]] = remoteFileInfo.LastWriteTimeUtc;
                    downloadedFiles++;
                    continue;
                }
                else
                {
                    _filenNameToDatNew[files[i]] = remoteFileInfo.LastWriteTimeUtc;
                    unchangedFiles++;
                }

            }

            // save remote file list
            List<string> _info = new List<string> ();
            foreach (KeyValuePair<string, DateTime> remoteOnLocalInfoEntry in _filenNameToDatNew)
            {
                _info.Add(remoteOnLocalInfoEntry.Key + "#" + remoteOnLocalInfoEntry.Value.Ticks.ToString());
            }
            System.IO.File.WriteAllLines(localRemoteFileInfo.FullName,  _info.ToArray());

            // System.Threading.Thread.Sleep(3000);

            timer1.Start();

            string _baloonInfo = string.Format("Downloaded files {0}\nRemoved {1}\nUnchanged {2}", downloadedFiles, removedFiles, unchangedFiles);
            notifyIcon1.ShowBalloonTip(500, "Data Sync Completed", _baloonInfo, ToolTipIcon.Info);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.timer1.Stop();
            new wndSettings().ShowDialog();
            ApplicationConfiguration.Instance.ReloadConfigurationData();
            try
            {
                this.timer1.Interval = ApplicationConfiguration.Instance.GetValueByPath<int>("sync.fetchTimer");
            }
            catch { }
            this.timer1.Start();
        }
    }
}
