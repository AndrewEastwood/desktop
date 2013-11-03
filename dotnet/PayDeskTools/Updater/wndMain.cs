﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using components.Public;
using System.IO;
using System.Collections;
using components.Components.WinApi;
using components.Lib;

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

            // add .lock file into local dir


            // notifyIcon1.ShowBalloonTip(500, Application.ProductName, "Data Sync Started", ToolTipIcon.Info);

            // loop through remote files
            string[] files = ApplicationConfiguration.Instance["sync.monitorFiles"].ToString().Split('\n');
            string remotePathBase = ApplicationConfiguration.Instance["sync.remotePath"].ToString();
            string localPathBase = ApplicationConfiguration.Instance["sync.localPath"].ToString();
            FileInfo localRemoteFileInfo = new FileInfo(localPathBase + @"\info.txt");
            FileInfo localLockFile = new FileInfo(localPathBase + @"\.lock");
            FileInfo localStorageInfo = new FileInfo(localPathBase);
            Dictionary<string, DateTime> _filenNameToDate = new Dictionary<string, DateTime>();
            Dictionary<string, DateTime> _filenNameToDatNew = new Dictionary<string, DateTime>();
            bool copyAllFiles = false;

            
            // create loca dir when it does not exsist
            if (!System.IO.Directory.Exists(localStorageInfo.FullName))
                System.IO.Directory.CreateDirectory(localStorageInfo.FullName);

            if (!System.IO.Directory.Exists(remotePathBase))
            {
                notifyIcon1.ShowBalloonTip(1, "Помилка синхронізації", "Немає звязку з сервером", ToolTipIcon.Error);
                timer1.Start();
                return;
            }

            if (files.Length == 0)
            {
                timer1.Start();
                return;
            }

            // add lock file
            File.CreateText(localLockFile.FullName).Close();

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
                if (files[i].Length == 0)
                    continue;

                FileInfo remoteFileInfo = new FileInfo(remotePathBase + "\\" + files[i]);
                FileInfo localFileInfo = new FileInfo(localPathBase + "\\" + files[i]);

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
                    // do file data tarnsformations
                    dataReaderAndTransformation(localFileInfo);
                    downloadedFiles++;
                    continue;
                }

                // we have remote file but we do not have it on local 
                if (!localFileInfo.Exists)
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[files[i]] = remoteFileInfo.LastWriteTimeUtc;
                    // do file data tarnsformations
                    dataReaderAndTransformation(localFileInfo);
                    downloadedFiles++;
                    continue;
                }

                if (!_filenNameToDate.ContainsKey(files[i]))
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[files[i]] = remoteFileInfo.LastWriteTimeUtc;
                    // do file data tarnsformations
                    dataReaderAndTransformation(localFileInfo);
                    downloadedFiles++;
                    continue;
                }

                // do compare dates and compy when the remote one is newer than it was before
                if (remoteFileInfo.LastWriteTimeUtc.Ticks > _filenNameToDate[files[i]].Ticks)
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[files[i]] = remoteFileInfo.LastWriteTimeUtc;
                    // do file data tarnsformations
                    dataReaderAndTransformation(localFileInfo);
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
                // save remote file info
                _info.Add(remoteOnLocalInfoEntry.Key + "#" + remoteOnLocalInfoEntry.Value.Ticks.ToString());
            }
            System.IO.File.WriteAllLines(localRemoteFileInfo.FullName,  _info.ToArray());


            // System.Threading.Thread.Sleep(3000);

            // remove lock file
            File.Delete(localLockFile.FullName);

            timer1.Start();

            string _baloonInfo = string.Format("Нових: {0}\nВидалено: {1}\nБез змін: {2}", downloadedFiles, removedFiles, unchangedFiles);
            notifyIcon1.ShowBalloonTip(1, "Синхронізація завершена", _baloonInfo, ToolTipIcon.Info);
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

        /* data readers  */
        private void dataReaderAndTransformation(FileInfo file)
        {
            Hashtable mapToReaders = ApplicationConfiguration.Instance.GetValueByPath<Hashtable>("sync.dataReaders");

            foreach (DictionaryEntry de in mapToReaders)
            {
                Hashtable entry = (Hashtable)de.Value;

                if (entry["Source"].ToString().Equals(file.Name))
                {
                    if (entry["Reader"] == null)
                        continue;

                    DataTable data = null;

                    // do data transformatios
                    switch (entry["Reader"].ToString())
                    {
                        case "Products":
                            data = ReadProduct(file.FullName);
                            data.TableName = Path.GetFileNameWithoutExtension(file.Name);
                            data.WriteXml(file.FullName.Replace(file.Extension, "_raw.xml"), true);
                            break;
                        case "AlternativeBarCodes":
                            data = ReadAlternative(file.FullName);
                            data.TableName = Path.GetFileNameWithoutExtension(file.Name);
                            data.WriteXml(file.FullName.Replace(file.Extension, "_raw.xml"), true);
                            break;
                        case "ClientCards":
                            data = ReadCard(file.FullName);
                            data.TableName = Path.GetFileNameWithoutExtension(file.Name);
                            data.WriteXml(file.FullName.Replace(file.Extension, "_raw.xml"), true);
                            break;
                    }
                }
            }
        }

        private static DataTable CreateDataTableForProduct()
        {
            DataTable dTable = new DataTable();
            Type[] cTypes = {
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double)};

            string[] STYLE_ARTColumnName = new string[] { "ID", "BC", "NAME", "DESC", "UNIT", "VG", "TID", "TQ", "PACK", "WEIGHT", "PRICE", "PR1", "PR2", "PR3", "Q2", "Q3" };

            dTable.Columns.Add("C", typeof(long));

            dTable.Columns.Add("F", typeof(string));

            for (byte i = 0; i < STYLE_ARTColumnName.Length; i++)
                dTable.Columns.Add(STYLE_ARTColumnName[i], cTypes[i]);

            dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
            dTable.Columns["C"].AutoIncrement = true;
            dTable.Columns["C"].Unique = true;

            return dTable;
        }//ok
        private static DataTable CreateDataTableForAlternative()
        {
            DataTable dTable = new DataTable();
            Type[] cTypes = {
                typeof(string),
                typeof(string)};

            string[] STYLE_ALTColumnName = new string[] { "ABC", "AID" };

            dTable.Columns.Add("C", typeof(long));

            dTable.Columns.Add("F", typeof(string));

            for (byte i = 0; i < STYLE_ALTColumnName.Length; i++)
                dTable.Columns.Add(STYLE_ALTColumnName[i], cTypes[i]);

            dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
            dTable.Columns["C"].AutoIncrement = true;
            dTable.Columns["C"].Unique = true;

            return dTable;
        }//ok
        private static DataTable CreateDataTableForCard()
        {
            DataTable dTable = new DataTable();
            Type[] cTypes = {
                typeof(string),
                typeof(string),
                typeof(double),
                typeof(int)};

            string[] CARDColumnName = new string[] { "CBC", "CID", "CDISC", "CPRICENO" };

            dTable.Columns.Add("C", typeof(long));
            for (byte i = 0; i < CARDColumnName.Length; i++)
                dTable.Columns.Add(CARDColumnName[i], cTypes[i]);

            dTable.Columns.Add("F", typeof(string));

            //dTable.TableName = "DCards";

            dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
            dTable.Columns["C"].AutoIncrement = true;
            dTable.Columns["C"].Unique = true;

            return dTable;
        }//ok
        private static DataTable ReadProduct(string path)
        {
            DataTable dTable = CreateDataTableForProduct();
            CoreLib coreLib = new CoreLib();
            byte err_cnt = 0;
            int startupIndex = 0;

            if (File.Exists(path))
            {
                //StreamReader sr = null;
                Stream sr = null;
                do
                {
                    try
                    {
                        //sr = new StreamReader(path, Encoding.Default);
                        sr = File.OpenRead(path);
                        break;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(100);
                        err_cnt++;
                    }

                } while (err_cnt < 10);

                if (sr == null)
                    return dTable;

                // StringReader strRd = new StringReader(reader.ReadToEnd());
                // sr.Close();
                // sr.Dispose();
                string line = string.Empty;
                DataRow dRow = dTable.NewRow();
                dTable.Rows.Clear();

                string firmId = "";
                string[] fName = Path.GetFileNameWithoutExtension(path).Split(new char[] { '_' });
                try
                {
                    firmId = int.Parse(fName[1].Substring(0, 2)).ToString();
                }
                catch { }

                dTable.Columns["C"].AutoIncrementSeed = startupIndex + 1000000 * int.Parse(firmId);
                if (dTable.Columns["C"].AutoIncrementSeed == 0)
                    dTable.Columns["C"].AutoIncrementSeed = 1;

                Com_WinApi.OutputDebugString("ReadArtSDF_begin");
                using (StreamReader reader = new StreamReader(new BufferedStream(sr, 131072), Encoding.Default))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line == string.Empty)
                            continue;

                        /*
                        if (isfull && (sc + 1) % 100 == 0)
                            isfull = new sgmode.ClassMode().FullLoader();
                        */
                        //dRow["C"] = index++;
                        line = line.Replace("\\\\", "\\");
                        dRow = dTable.NewRow();

                        dRow["F"] = firmId;
                        dRow["ID"] = line.Substring(0, 10).Trim();//id
                        dRow["BC"] = line.Substring(10, 14).Trim();//skod
                        dRow["NAME"] = line.Substring(24, 35).Trim().Replace('i', 'і').Replace('I', 'І');//name
                        dRow["DESC"] = line.Substring(59, 60).Trim().Replace('i', 'і').Replace('I', 'І');//desc
                        dRow["UNIT"] = line.Substring(119, 15).Trim();//unit
                        dRow["VG"] = line.Substring(134, 1).Trim();//vg
                        if (dRow["VG"].ToString() == "")
                            dRow["VG"] = " ";
                        dRow["TID"] = line.Substring(135, 11).Trim();//tid

                        dRow["TQ"] = coreLib.GetDouble(line.Substring(146, 12).Trim());//tq
                        dRow["PACK"] = coreLib.GetDouble(line.Substring(158, 18).Trim());//pack
                        dRow["WEIGHT"] = coreLib.GetDouble(line.Substring(176, 18).Trim());//weight
                        dRow["PRICE"] = coreLib.GetDouble(line.Substring(194, 12).Trim());//price
                        dRow["PR1"] = coreLib.GetDouble(line.Substring(206, 12).Trim());//pr1
                        dRow["PR2"] = coreLib.GetDouble(line.Substring(218, 12).Trim());//pr2
                        dRow["PR3"] = coreLib.GetDouble(line.Substring(230, 12).Trim());//pr3
                        dRow["Q2"] = coreLib.GetDouble(line.Substring(242, 12).Trim());//q2
                        dRow["Q3"] = coreLib.GetDouble(line.Substring(254, 10).Trim());//q3


                        //dRow["C"] = dRow["ID"];
                        dTable.Rows.Add(dRow);
                        //dRow = dTable.NewRow();

                    }
                    Com_WinApi.OutputDebugString("ReadArtSDF_end");
                }
            }

            return dTable;
        }//ok
        private static DataTable ReadAlternative(string path)
        {
            DataTable dTable = CreateDataTableForAlternative();
            int startupIndex = 0;
            CoreLib coreLib = new CoreLib();
            byte err_cnt = 0;

            if (File.Exists(path))
            {
                Stream sr = null;
                do
                {
                    try
                    {
                        sr = new FileStream(path, FileMode.Open);
                        break;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(100);
                        err_cnt++;
                    }

                } while (err_cnt < 10);

                if (sr == null)
                    return dTable;

                BufferedStream buff = new BufferedStream(sr, 131072);
                StreamReader reader = new StreamReader(buff, Encoding.Default);
                StringReader strRd = new StringReader(reader.ReadToEnd());
                sr.Close();
                sr.Dispose();
                string line = string.Empty;
                DataRow dRow = dTable.NewRow();
                dTable.Rows.Clear();

                string firmId = "";
                string[] fName = Path.GetFileNameWithoutExtension(path).Split(new char[] { '_' });
                try
                {
                    firmId = int.Parse(fName[1].Substring(0, 2)).ToString();
                }
                catch { }

                dTable.Columns["C"].AutoIncrementSeed = startupIndex + 1000000 * int.Parse(firmId);
                if (dTable.Columns["C"].AutoIncrementSeed == 0)
                    dTable.Columns["C"].AutoIncrementSeed = 1;

                Com_WinApi.OutputDebugString("ReadAltSDF_begin");
                while ((line = strRd.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    /*
                    if (!test1 && testdsc >= 10)
                        break;

                    if (test1 && (testdsc + 1) % 100 == 0)
                        test1 = new sgmode.ClassMode().FullLoader();
                    */
                    //dRow["C"] = index++;
                    dRow = dTable.NewRow();
                    dRow["F"] = firmId;
                    dRow["ABC"] = line.Substring(0, 20).Trim();//abc
                    dRow["AID"] = line.Substring(20, 10).Trim();//aid

                    //dRow["C"] = dRow["AID"];
                    dTable.Rows.Add(dRow);
                    //dRow = dTable.NewRow();

                    //testdsc++;
                }
                Com_WinApi.OutputDebugString("ReadAltSDF_end");
                sr.Close();
                sr.Dispose();
            }
            return dTable;
        }//ok
        private static DataTable ReadCard(string path)
        {
            DataTable dTable = CreateDataTableForCard();
            int startupIndex = 0;
            CoreLib coreLib = new CoreLib();
            byte err_cnt = 0;

            if (File.Exists(path))
            {
                StreamReader sr = null;
                do
                {
                    try
                    {
                        sr = new StreamReader(path, Encoding.Default);
                        break;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(100);
                        err_cnt++;
                    }

                } while (err_cnt < 10);

                if (sr == null)
                    return dTable;

                string line = string.Empty;
                DataRow dRow = dTable.NewRow();
                //long index = 0;
                int myrd = 0;

                string firmId = "";
                string[] fName = Path.GetFileNameWithoutExtension(path).Split(new char[] { '_' });
                try
                {
                    firmId = int.Parse(fName[1].Substring(0, 2)).ToString();
                }
                catch { }

                dTable.Columns["C"].AutoIncrementSeed = startupIndex + 1000000 * int.Parse(firmId);
                if (dTable.Columns["C"].AutoIncrementSeed == 0)
                    dTable.Columns["C"].AutoIncrementSeed = 1;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;

                    //dRow["C"] = index++;

                    dRow["F"] = firmId;
                    dRow["CBC"] = line.Substring(0, 20).Trim();//abc
                    dRow["CID"] = line.Substring(20, 10).Trim();//aid
                    dRow["CDISC"] = coreLib.GetDouble(line.Substring(30, 6).Trim());//cdisc
                    try
                    {
                        dRow["CPRICENO"] = int.Parse(line.Substring(36).Trim());//cdisc
                    }
                    catch { dRow["CPRICENO"] = (int)0; }
                    dTable.Rows.Add(dRow);
                    dRow = dTable.NewRow();

                    myrd++;
                }
                sr.Close();
                sr.Dispose();
            }
            return dTable;
        }//ok 

    }
}
