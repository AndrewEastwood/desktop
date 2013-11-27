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
                this.timer1.Interval = ApplicationConfiguration.Instance.GetValueByPath<int>("general.main.fetchTimeout");
            }
            catch { }
            // trigger timer event at startup
            this.timer1_Tick(timer1, EventArgs.Empty);
        }

        ~wndMain()
        {
            Hashtable dataSyncProfiles = (Hashtable)ApplicationConfiguration.Instance.Configuration["datasync"];
            if (dataSyncProfiles != null)
                foreach (DictionaryEntry de in dataSyncProfiles)
                {
                    Hashtable _syncConfig = (Hashtable)de.Value;
                    if (!_syncConfig.ContainsKey("sync"))
                        continue;
                }
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

            notifyIcon1.Icon = AppStateIcons.isync_busy;

            string _statusMessage = "";
            Dictionary<string, int> _generalUpdateStatus = new Dictionary<string, int>();
            Hashtable dataSyncProfiles = (Hashtable)ApplicationConfiguration.Instance.Configuration["datasync"];
            if (dataSyncProfiles != null)
                foreach (DictionaryEntry de in dataSyncProfiles)
                {
                    Hashtable _syncConfig = (Hashtable)de.Value;
                    if (!_syncConfig.ContainsKey("sync"))
                        continue;
                    try
                    {
                        // get sync profile config
                        _syncConfig = (Hashtable)_syncConfig["sync"];
                        // do sync
                        Dictionary<string, int> _profileUpdateStatus = perfromDataSync(de.Key.ToString(), _syncConfig);
                        // update sync status
                        if (_profileUpdateStatus != null)
                        {
                            string title = _syncConfig["profileDisplayText"].ToString();

                            // update general status data
                            foreach (KeyValuePair<string, int> _propfileStatusEntry in _profileUpdateStatus)
                                if (_generalUpdateStatus.ContainsKey(_propfileStatusEntry.Key))
                                    _generalUpdateStatus[_propfileStatusEntry.Key] += _propfileStatusEntry.Value;
                                else
                                    _generalUpdateStatus[_propfileStatusEntry.Key] = _propfileStatusEntry.Value;

                            _statusMessage += string.Format("{0}\n    Нових: {1}\n    Видалено: {2}\n    Без змін: {3}", title, _profileUpdateStatus["new"], _profileUpdateStatus["removed"], _profileUpdateStatus["skipped"]);
                            _statusMessage += "\n".PadRight(15, '=') + "\n";
                        }
                        //_statusMessage += "\r\n" + title + ":\r\n" + _statusMessageProfile + "\r\n" + "".PadRight(15, '=');
                    }
                    catch { }
                }

            // System.Threading.Thread.Sleep(3000);
            // do data transformations
            try
            {
                if (this.dataTransformation(_generalUpdateStatus))
                    switchTempSourceToNormal();
            }
            catch { }

            timer1.Start();

            if (_statusMessage.Length > 0)
                notifyIcon1.ShowBalloonTip(1, "Синхронізація завершена", _statusMessage, ToolTipIcon.Info);

            notifyIcon1.Icon = AppStateIcons.isync;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.timer1.Stop();
            new wndSettings().ShowDialog();
            ApplicationConfiguration.Instance.ReloadConfigurationData();
            try
            {
                this.timer1.Interval = ApplicationConfiguration.Instance.GetValueByPath<int>("general.fetchTimeout");
            }
            catch { }
            this.timer1.Start();
        }

        /* files and files collections */
        private Dictionary<string, string> getFilesByProfileByName(string profileKey)
        {
            Dictionary<string, string> _filesToMerge = new Dictionary<string, string>();
            Hashtable dataSyncProfiles = (Hashtable)ApplicationConfiguration.Instance.Configuration["datasync"];
            if (dataSyncProfiles != null)
            {
                Hashtable _syncConfig = (Hashtable)dataSyncProfiles[profileKey];
                if (_syncConfig.ContainsKey("sync"))
                    try
                    {
                        _syncConfig = (Hashtable)_syncConfig["sync"];
                        _filesToMerge["ART"] = string.Format("Art_{0:X2}{1:X2}.SDF", int.Parse(_syncConfig["srcSchema"].ToString()), int.Parse(_syncConfig["srcSubunit"].ToString()));
                        _filesToMerge["ALT"] = string.Format("Alt_{0:X2}{1:X2}.SDF", int.Parse(_syncConfig["srcSchema"].ToString()), int.Parse(_syncConfig["srcSubunit"].ToString()));
                        _filesToMerge["CLI"] = string.Format("Cli_{0:X2}{1:X2}.SDF", int.Parse(_syncConfig["srcSchema"].ToString()), int.Parse(_syncConfig["srcSubunit"].ToString()));
                    }
                    catch { }
            }

            return _filesToMerge;
        }

        private Dictionary<string, List<string>> getFilesAllProfilesByType()
        {
            Dictionary<string, List<string>> _filesToMerge = new Dictionary<string, List<string>>();
            Hashtable dataSyncProfiles = (Hashtable)ApplicationConfiguration.Instance.Configuration["datasync"];
            if (dataSyncProfiles != null)
                foreach (DictionaryEntry de in dataSyncProfiles)
                {
                    Hashtable _syncConfig = (Hashtable)de.Value;
                    if (!_syncConfig.ContainsKey("sync"))
                        continue;
                    try
                    {
                        _syncConfig = (Hashtable)_syncConfig["sync"];

                        if (!_filesToMerge.ContainsKey("ART"))
                            _filesToMerge["ART"] = new List<string>();

                        if (!_filesToMerge.ContainsKey("ALT"))
                            _filesToMerge["ALT"] = new List<string>();

                        if (!_filesToMerge.ContainsKey("CLI"))
                            _filesToMerge["CLI"] = new List<string>();

                        _filesToMerge["ART"].Add(string.Format("Art_{0:X2}{1:X2}.xml", int.Parse(_syncConfig["srcSchema"].ToString()), int.Parse(_syncConfig["srcSubunit"].ToString())));
                        _filesToMerge["ALT"].Add(string.Format("Alt_{0:X2}{1:X2}.xml", int.Parse(_syncConfig["srcSchema"].ToString()), int.Parse(_syncConfig["srcSubunit"].ToString())));
                        _filesToMerge["CLI"].Add(string.Format("Cli_{0:X2}{1:X2}.xml", int.Parse(_syncConfig["srcSchema"].ToString()), int.Parse(_syncConfig["srcSubunit"].ToString())));
                    }
                    catch { }
                }

            return _filesToMerge;
        }

        private Dictionary<string, string> getFilesDestinationNames(bool isTemporary)
        {
            return new Dictionary<string, string>()
            {
                {"ART", (isTemporary ? "__" : "") + "Products.xml"},
                {"ALT", (isTemporary ? "__" : "") + "Alternatives.xml"},
                {"CLI", (isTemporary ? "__" : "") + "ClientCards.xml"}
            };
        }

        private void switchTempSourceToNormal()
        {
            string localPathBase = ApplicationConfiguration.Instance.GetValueByPath<string>("general.main.localPath");
            // FileInfo localStorageInfo = new FileInfo(localPathBase);

            string[] _tmpSrc = getFilesDestinationNames(true).Values.ToArray<string>();
            string[] _normSrc = getFilesDestinationNames(false).Values.ToArray<string>();

            for (int i = 0, len = _tmpSrc.Length; i < len; i++)
            {
                try
                {
                    if (File.Exists(localPathBase + Path.DirectorySeparatorChar + _normSrc[i]))
                        File.Delete(localPathBase + Path.DirectorySeparatorChar + _normSrc[i]);

                    File.Move(localPathBase + Path.DirectorySeparatorChar + _tmpSrc[i], localPathBase + Path.DirectorySeparatorChar + _normSrc[i]);

                    if (File.Exists(localPathBase + Path.DirectorySeparatorChar + _tmpSrc[i]))
                        File.Delete(localPathBase + Path.DirectorySeparatorChar + _tmpSrc[i]);
                }
                catch { }
            }

        }

        private FileInfo getFileDataConvertionBySource(FileInfo sourceFile)
        {
            return new FileInfo(sourceFile.FullName.Replace(sourceFile.Extension, ".xml"));
        }

        /* datasync profile */
        private Dictionary<string, int> perfromDataSync(string profileName, Hashtable config)
        {
            // notifyIcon1.ShowBalloonTip(500, Application.ProductName, "Data Sync Started", ToolTipIcon.Info);
            Dictionary<string, string> _src = getFilesByProfileByName(profileName);
            string remotePathBase = config["remotePath"].ToString();
            string localPathBase = ApplicationConfiguration.Instance.GetValueByPath<string>("general.main.localPath");
            FileInfo localRemoteFileInfo = new FileInfo(localPathBase + @"\info_" + profileName + ".txt");
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
                // notifyIcon1.ShowBalloonTip(1, "Помилка синхронізації", "Немає звязку з сервером", ToolTipIcon.Error);
                notifyIcon1.Icon = AppStateIcons.isync_err;
                return null;
            }

            notifyIcon1.Icon = AppStateIcons.isync_ok;

            // add lock file
            // File.CreateText(localLockFile.FullName).Close();

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
            int totalFiles = 0;

            foreach (KeyValuePair<string, string> _fileEntry in _src)
            {
                if (_fileEntry.Value.Length == 0)
                    continue;

                totalFiles++;

                FileInfo remoteFileInfo = new FileInfo(remotePathBase + "\\" + _fileEntry.Value);
                FileInfo localFileInfo = new FileInfo(localPathBase + "\\" + _fileEntry.Value);

                // List<FileInfo> _listOfRemovedFile = new List<FileInfo>();

                // skip unexisted remote file
                if (!remoteFileInfo.Exists)
                {
                    if (localFileInfo.Exists)
                    {
                        File.Delete(localFileInfo.FullName);
                        FileInfo dataFile = this.getFileDataConvertionBySource(localFileInfo);
                        if (dataFile.Exists)
                            File.Delete(dataFile.FullName);
                        removedFiles++;
                    }
                    else
                        totalFiles--;
                    // _listOfRemovedFile.Add(localFileInfo);
                    continue;
                }

                // we do this when we do not have local info file about remote files
                if (copyAllFiles)
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[_fileEntry.Value] = remoteFileInfo.LastWriteTimeUtc;
                    // do file data tarnsformations
                    dataConverterManager(_fileEntry.Key, localFileInfo);
                    downloadedFiles++;
                    continue;
                }

                // we have remote file but we do not have it on local 
                if (!localFileInfo.Exists)
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[_fileEntry.Value] = remoteFileInfo.LastWriteTimeUtc;
                    // do file data tarnsformations
                    dataConverterManager(_fileEntry.Key, localFileInfo);
                    downloadedFiles++;
                    continue;
                }

                if (!_filenNameToDate.ContainsKey(_fileEntry.Value))
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[_fileEntry.Value] = remoteFileInfo.LastWriteTimeUtc;
                    // do file data tarnsformations
                    dataConverterManager(_fileEntry.Key, localFileInfo);
                    downloadedFiles++;
                    continue;
                }

                // do compare dates and compy when the remote one is newer than it was before
                if (remoteFileInfo.LastWriteTimeUtc.Ticks > _filenNameToDate[_fileEntry.Value].Ticks)
                {
                    System.IO.File.Copy(remoteFileInfo.FullName, localFileInfo.FullName, true);
                    _filenNameToDatNew[_fileEntry.Value] = remoteFileInfo.LastWriteTimeUtc;
                    // do file data tarnsformations
                    dataConverterManager(_fileEntry.Key, localFileInfo);
                    downloadedFiles++;
                    continue;
                }
                else
                {
                    _filenNameToDatNew[_fileEntry.Value] = remoteFileInfo.LastWriteTimeUtc;
                    unchangedFiles++;
                }

            }

            // update status
            Dictionary<string, int> _statusUpdate = new Dictionary<string, int>();
            _statusUpdate["new"] = downloadedFiles;
            _statusUpdate["removed"] = removedFiles;
            _statusUpdate["skipped"] = unchangedFiles;
            _statusUpdate["files"] = totalFiles;

            // save remote file list
            List<string> _info = new List<string>();
            foreach (KeyValuePair<string, DateTime> remoteOnLocalInfoEntry in _filenNameToDatNew)
            {
                // save remote file info
                _info.Add(remoteOnLocalInfoEntry.Key + "#" + remoteOnLocalInfoEntry.Value.Ticks.ToString());
            }
            System.IO.File.WriteAllLines(localRemoteFileInfo.FullName, _info.ToArray());

            return _statusUpdate;
        }

        /* data converters and transformations  */
        private void dataConverterManager(string fileType, FileInfo file)
        {
            //Hashtable mapToReaders = (Hashtable)config["dataReaders"];
            FileInfo dataSourceFile = this.getFileDataConvertionBySource(file);

            //foreach (DictionaryEntry de in mapToReaders)
            //{
                //Hashtable entry = (Hashtable)de.Value;

                //if (entry["Source"].ToString().Equals(file.Name))
                //{
                //    if (entry["Reader"] == null)
                //        continue;

                    DataTable data = null;

                    // do data transformatios
                    //switch (entry["Reader"].ToString())
                    switch (fileType)
                    {
                        case "ART":
                            data = ReadProduct(file.FullName);
                            data.TableName = Path.GetFileNameWithoutExtension(file.Name);
                            data.ExtendedProperties["NAME"] = data.TableName;
                            data.WriteXml(dataSourceFile.FullName, true);
                            break;
                        case "ALT":
                            data = ReadAlternative(file.FullName);
                            data.TableName = Path.GetFileNameWithoutExtension(file.Name);
                            data.ExtendedProperties["NAME"] = data.TableName;
                            data.WriteXml(dataSourceFile.FullName, true);
                            break;
                        case "CLI":
                            data = ReadCard(file.FullName);
                            data.TableName = Path.GetFileNameWithoutExtension(file.Name);
                            data.ExtendedProperties["NAME"] = data.TableName;
                            data.WriteXml(dataSourceFile.FullName, true);
                            break;
                    }
                //}
            //}

        }

        private bool dataTransformation(Dictionary<string, int> updateStatus)
        {
            string localPathBase = ApplicationConfiguration.Instance.GetValueByPath<string>("general.main.localPath");
            bool _allNormalFilesExist = true;

            // do check whether we have all normal files
            Dictionary<string, string> _normSrc = getFilesDestinationNames(false);
            foreach (KeyValuePair<string, string> _fileNormalNameEntryToCheck in _normSrc)
                if (!File.Exists(localPathBase + Path.DirectorySeparatorChar + _fileNormalNameEntryToCheck.Value))
                {
                    _allNormalFilesExist = false;
                    break;
                }
            
            // do not transform data when nothing has changed
            if (_allNormalFilesExist && (updateStatus == null || updateStatus["files"] == updateStatus["skipped"]))
                return false;

            Dictionary<string, string> _tmpSrc = getFilesDestinationNames(true);
            Dictionary<string, List<string>> _filesToMergeAllProfiles = getFilesAllProfilesByType();

            bool hasFail = false;

            // do data transformation
            foreach (KeyValuePair<string, List<string>> _filesToMergeSameType in _filesToMergeAllProfiles)
            {
                string _mergedSourceName = _tmpSrc[_filesToMergeSameType.Key];
                string _mergedTableName = Path.GetFileNameWithoutExtension(_normSrc[_filesToMergeSameType.Key]);

                switch (_filesToMergeSameType.Key)
                {
                    case "ART":
                        {
                            // source list
                            //string _mergedSourceName = "__Products.xml";
                            DataTable _mergedSources = CreateDataTableForProduct(_mergedTableName);
                            DataTable _runningSource = null;
                            try
                            {
                                foreach (string _sourceName in _filesToMergeSameType.Value)
                                {
                                    if (!File.Exists(localPathBase + Path.DirectorySeparatorChar + _sourceName))
                                        continue;
                                    // read source
                                    _runningSource = CreateDataTableForProduct(Path.GetFileNameWithoutExtension(_sourceName));
                                    _runningSource.ReadXml(localPathBase + Path.DirectorySeparatorChar + _sourceName);
                                    // do merge
                                    _mergedSources.Merge(_runningSource);
                                }

                                // save merged data
                                //if (_mergedSources.Rows.Count > 0)
                                _mergedSources.WriteXml(localPathBase + Path.DirectorySeparatorChar + _mergedSourceName);
                            }
                            catch { hasFail = true; }
                            break;
                        }
                    case "ALT":
                        {
                            // source list
                            //string _mergedSourceName = "__Alternatives.xml";
                            DataTable _mergedSources = CreateDataTableForAlternative(_mergedTableName);
                            DataTable _runningSource = null;
                            try
                            {
                                foreach (string _sourceName in _filesToMergeSameType.Value)
                                {
                                    if (!File.Exists(localPathBase + Path.DirectorySeparatorChar + _sourceName))
                                        continue;
                                    // read source
                                    _runningSource = CreateDataTableForAlternative(Path.GetFileNameWithoutExtension(_sourceName));
                                    _runningSource.ReadXml(localPathBase + Path.DirectorySeparatorChar + _sourceName);
                                    // do merge
                                    _mergedSources.Merge(_runningSource);
                                }

                                // save merged data
                                //if (_mergedSources.Rows.Count > 0)
                                _mergedSources.WriteXml(localPathBase + Path.DirectorySeparatorChar + _mergedSourceName);
                            }
                            catch { hasFail = true; }
                            break;
                        }
                    case "CLI":
                        {
                            // source list
                            //string _mergedSourceName = "__ClientCards.xml";
                            DataTable _mergedSources = CreateDataTableForCard(_mergedTableName);
                            DataTable _runningSource = null;
                            try
                            {
                                foreach (string _sourceName in _filesToMergeSameType.Value)
                                {
                                    if (!File.Exists(localPathBase + Path.DirectorySeparatorChar + _sourceName))
                                        continue;
                                    // read source
                                    _runningSource = CreateDataTableForCard(Path.GetFileNameWithoutExtension(_sourceName));
                                    _runningSource.ReadXml(localPathBase + Path.DirectorySeparatorChar + _sourceName);
                                    // do merge
                                    _mergedSources.Merge(_runningSource);
                                }

                                // save merged data
                                //if (_mergedSources.Rows.Count > 0)
                                _mergedSources.WriteXml(localPathBase + Path.DirectorySeparatorChar + _mergedSourceName);
                            }
                            catch { hasFail = true; }
                            break;
                        }
                }
            }

            if (hasFail)
                return false;

            return true;
        }

        /* data init and raw parsers */
        private static DataTable CreateDataTableForProduct(string name)
        {
            DataTable dTable = new DataTable(name);
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
        private static DataTable CreateDataTableForAlternative(string name)
        {
            DataTable dTable = new DataTable(name);
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
        private static DataTable CreateDataTableForCard(string name)
        {
            DataTable dTable = new DataTable(name);
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
            DataTable dTable = CreateDataTableForProduct(Path.GetFileNameWithoutExtension(path));
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
            DataTable dTable = CreateDataTableForAlternative(Path.GetFileNameWithoutExtension(path));
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
            DataTable dTable = CreateDataTableForCard(Path.GetFileNameWithoutExtension(path));
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
