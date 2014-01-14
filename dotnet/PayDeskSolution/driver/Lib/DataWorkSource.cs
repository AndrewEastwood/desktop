using System;
using System.Collections.Generic;
using System.Text;
using driver.Config;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data;
using driver.Lib;
using Microsoft.VisualBasic.FileIO;
using System.Collections;
using components.Components.SecureRuntime;
using components.Components.WinApi;
using components.Lib;
using driver.Common;
using components.Components.HashObject;
using components.Components.DataContainer;

namespace driver.Lib
{
    public static class DataWorkSource
    {
        // last date-time source update
        private static Dictionary<string, long> _sourcesLastDate;

        public static Dictionary<string, FileInfo> GetSourceFiles()
        {
            // source folder
            string sourceDir = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles;
            // get profile subunit
            int subUnit = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit;
            Dictionary<string, FileInfo> dSources = new Dictionary<string, FileInfo>();
            dSources[CoreConst.DATA_CONTAINER_PRODUCT] = new FileInfo(string.Format("{0}\\Products.xml", sourceDir, subUnit));
            dSources[CoreConst.DATA_CONTAINER_ALTERNATIVE] = new FileInfo(string.Format("{0}\\Alternatives.xml", sourceDir, subUnit));
            dSources[CoreConst.DATA_CONTAINER_CLIENT] = new FileInfo(string.Format("{0}\\ClientCards.xml", sourceDir, subUnit));

            return dSources;
        }

        public static Dictionary<string, DataTable> GetSourceTables()
        {
            Dictionary<string, DataTable> dTables = new Dictionary<string, DataTable>();

            // init data data
            dTables[CoreConst.DATA_CONTAINER_PRODUCT] = CreateDataTableForProduct();
            dTables[CoreConst.DATA_CONTAINER_ALTERNATIVE] = CreateDataTableForAlternative();
            dTables[CoreConst.DATA_CONTAINER_CLIENT] = CreateDataTableForCard();
            dTables[CoreConst.DATA_CONTAINER_CHEQUE] = CreateDataTableForOrder();

            return dTables;
        }

        public static bool NewSourcesAvailable()
        {
            Dictionary<string, FileInfo> dSources = GetSourceFiles();

            // init dates
            if (_sourcesLastDate == null)
            {
                _sourcesLastDate = new Dictionary<string, long>();
                // init empty collection
                _sourcesLastDate[CoreConst.DATA_CONTAINER_PRODUCT] = dSources[CoreConst.DATA_CONTAINER_PRODUCT].LastWriteTimeUtc.Ticks;
                _sourcesLastDate[CoreConst.DATA_CONTAINER_ALTERNATIVE] = dSources[CoreConst.DATA_CONTAINER_ALTERNATIVE].LastWriteTimeUtc.Ticks;
                _sourcesLastDate[CoreConst.DATA_CONTAINER_CLIENT] = dSources[CoreConst.DATA_CONTAINER_CLIENT].LastWriteTimeUtc.Ticks;
                return true;
            }

            bool hasAny = false;

            // compare with last write access info ticks
            if (_sourcesLastDate[CoreConst.DATA_CONTAINER_PRODUCT] != dSources[CoreConst.DATA_CONTAINER_PRODUCT].LastWriteTimeUtc.Ticks)
                hasAny = true;
            if (_sourcesLastDate[CoreConst.DATA_CONTAINER_ALTERNATIVE] != dSources[CoreConst.DATA_CONTAINER_ALTERNATIVE].LastWriteTimeUtc.Ticks)
                hasAny = true;
            if (_sourcesLastDate[CoreConst.DATA_CONTAINER_CLIENT] != dSources[CoreConst.DATA_CONTAINER_CLIENT].LastWriteTimeUtc.Ticks)
                hasAny = true;

            // set current source dates
            _sourcesLastDate[CoreConst.DATA_CONTAINER_PRODUCT] = dSources[CoreConst.DATA_CONTAINER_PRODUCT].LastWriteTimeUtc.Ticks;
            _sourcesLastDate[CoreConst.DATA_CONTAINER_ALTERNATIVE] = dSources[CoreConst.DATA_CONTAINER_ALTERNATIVE].LastWriteTimeUtc.Ticks;
            _sourcesLastDate[CoreConst.DATA_CONTAINER_CLIENT] = dSources[CoreConst.DATA_CONTAINER_CLIENT].LastWriteTimeUtc.Ticks;

            return hasAny;
        }

        public static Dictionary<string, DataTable> DownloadSource()
        {
            Dictionary<string, DataTable> dTables = GetSourceTables();
            Dictionary<string, FileInfo> dSources = GetSourceFiles();

            // set names
            dTables[CoreConst.DATA_CONTAINER_PRODUCT].TableName = Path.GetFileNameWithoutExtension(dSources[CoreConst.DATA_CONTAINER_PRODUCT].Name);
            dTables[CoreConst.DATA_CONTAINER_ALTERNATIVE].TableName = Path.GetFileNameWithoutExtension(dSources[CoreConst.DATA_CONTAINER_ALTERNATIVE].Name);
            dTables[CoreConst.DATA_CONTAINER_CLIENT].TableName = Path.GetFileNameWithoutExtension(dSources[CoreConst.DATA_CONTAINER_CLIENT].Name);

            try
            {
                // load data
                if (dSources[CoreConst.DATA_CONTAINER_PRODUCT].Exists)
                    dTables[CoreConst.DATA_CONTAINER_PRODUCT].ReadXml(dSources[CoreConst.DATA_CONTAINER_PRODUCT].FullName);

                if (dSources[CoreConst.DATA_CONTAINER_ALTERNATIVE].Exists)
                    dTables[CoreConst.DATA_CONTAINER_ALTERNATIVE].ReadXml(dSources[CoreConst.DATA_CONTAINER_ALTERNATIVE].FullName);

                if (dSources[CoreConst.DATA_CONTAINER_CLIENT].Exists)
                    dTables[CoreConst.DATA_CONTAINER_CLIENT].ReadXml(dSources[CoreConst.DATA_CONTAINER_CLIENT].FullName);
            }
            catch { }
            finally
            {
            }

            return dTables;
        }

        public static void CreateTables(ref DataTable chq, ref DataTable art, ref DataTable alt, ref DataTable cli, ref DataSet chqs)
        {
            CreateTables(ref chq, ref art, ref alt, ref cli);
            /* loop by all available profiles */

            chqs.Tables.Clear();
            foreach (DictionaryEntry de in driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
            {
                DataTable dT = CreateDataTableForOrder();
                dT.TableName = de.Key.ToString();
          
                chqs.Tables.Add(dT);
            }
        }

        public static void CreateTables(ref DataTable chq, ref DataTable art, ref DataTable alt, ref DataTable cli)
        {
            chq = CreateDataTableForOrder();
            art = CreateDataTableForProduct();
            alt = CreateDataTableForAlternative();
            cli = CreateDataTableForCard();
        }//ok
        
<<<<<<< HEAD
=======
        /* multiprofile */
        public static Hashtable CheckForUpdate_profile()
        {
            /* main variables */
            string[] exFiles = new string[3];
            string[] localFiles = new string[3];
            DateTime dTime = new DateTime();
            Hashtable load = new Hashtable();
            string[] load_profile = new string[3] { "", "", "" };

            if (driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime == null)
                driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime = new Hashtable();
            bool hasUpdates = false;
            /* loop by all available profiles */
            string sourceDir = string.Empty;
            foreach (DictionaryEntry de in driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
            {
                Hashtable profile = (Hashtable)de.Value;
                hasUpdates = false;
                /* checking for updates */
                //Com_WinApi.OutputDebugString("CheckUpdate_begin");

                sourceDir = profile.ContainsKey("SOURCE") ? profile["SOURCE"].ToString() : "";
                if (sourceDir == string.Empty)
                    sourceDir = ConfigManager.Instance.CommonConfiguration.Path_Exchnage;

                int subUnit = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit;
                // override subunit
                if (profile.ContainsKey("SUBUNIT"))
                    try
                    {
                        subUnit = int.Parse(profile["SUBUNIT"].ToString());
                    }
                    catch { }

                exFiles = new string[3];
                exFiles[0] = sourceDir + "\\" + string.Format("Art_{1:D2}{0:D2}", subUnit, int.Parse(de.Key.ToString())) + ".SDF";
                exFiles[1] = sourceDir + "\\" + string.Format("Alt_{1:D2}{0:D2}", subUnit, int.Parse(de.Key.ToString())) + ".SDF";
                exFiles[2] = sourceDir + "\\" + string.Format("Cli_{0:D2}", int.Parse(de.Key.ToString())) + ".SDF";

                localFiles = new string[3];
                localFiles[0] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Art_{1:D2}{0:D2}", subUnit, int.Parse(de.Key.ToString())) + ".xml";
                localFiles[1] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Alt_{1:D2}{0:D2}", subUnit, int.Parse(de.Key.ToString())) + ".xml";
                localFiles[2] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Cli_{0:D2}", int.Parse(de.Key.ToString())) + ".xml";

                /* ToDo
                 * 
                 * loop all server's files and detect for newer date
                 * also detect for deleted files 
                 * 
                 * if someone of them is newer than local's files
                 * we'll load all them directly.
                 * 
                 */

                load_profile = new string[3] { "", "", "" };
                for (int i = 0; i < exFiles.Length; i++)
                {
                    try
                    {
                        //Com_WinApi.OutputDebugString("check for index " + i);
                        if (AsyncFunc.FileExists(exFiles[i], driver.Config.ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout))
                        {
                            dTime = Microsoft.VisualBasic.FileSystem.FileDateTime(exFiles[i]);

                            if (!AsyncFunc.FileExists(localFiles[i], driver.Config.ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout))
                            {
                                load_profile[i] = exFiles[i].Clone().ToString();
                                ((DateTime[])driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime[de.Key.ToString()])[i] = dTime;
                            }
                            else
                            {
                                if (!driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime.ContainsKey(de.Key.ToString()))
                                    driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime.Add(de.Key.ToString(), new DateTime[3]);

                                if (hasUpdates || dTime > ((DateTime[])driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime[de.Key.ToString()])[i])
                                {
                                    hasUpdates = true;
                                    load_profile[i] = exFiles[i].Clone().ToString();
                                    ((DateTime[])driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime[de.Key.ToString()])[i] = dTime;
                                }
                            }
                        }
                        else
                            if (i == 0)
                            {
                                //Com_WinApi.OutputDebugString("Art file is unvaliable _ CheckUpdate_end");
                                if (!AsyncFunc.FileExists(profile["SOURCE"].ToString(), driver.Config.ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout))
                                    load_profile[i] = CoreConst.STATE_LAN_ERROR;
                            }
                    }
                    catch { ; }
                }

                load.Add(de.Key, (string[])load_profile.Clone());

                //Com_WinApi.OutputDebugString("CheckUpdate_end");
            }

            return load;
        }//ok
        /* singleprofile */
        /// <summary>
        /// Перевірка на оновлення бази з товарами
        /// </summary>
        /// <returns>Масив з шляхами до нових файлів</returns>
        public static string[] CheckForUpdate_single()
        {
            // EXCHANGE FILES
            string[] exFiles = new string[3];
            exFiles[0] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Exchnage + "\\" + string.Format("Art_{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit) + ".SDF";
            exFiles[1] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Exchnage + "\\" + string.Format("Alt_{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit) + ".SDF";
            exFiles[2] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Exchnage + "\\" + "Cli_BC.SDF";

            // LOCAL FILES
            string[] localFiles = new string[3];
            localFiles[0] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Art_{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit) + ".xml";
            localFiles[1] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Alt_{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit) + ".xml";
            localFiles[2] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + "Cli.xml";

            // TAKES DATE/TIME OF EACH EXCHANGE FILE
            DateTime dTime = new DateTime();
    
            // LIST OF NEW FILES TO DOWNLOAD
            string[] load = new string[3] { "", "", "" };

            //Com_WinApi.OutputDebugString("CheckUpdate_begin");

            //Microsoft.VisualBasic.Devices.Network nwt = new Microsoft.VisualBasic.Devices.Network();
            //try
            //{
            //    nwt.DownloadFile(AppConfig.Path_Exchnage + "\\" + "a.xxx", AppConfig.Path_Temp + "\\a.xxx", "", "", false,
            //        AppConfig.APP_RefreshTimeout, true);
            //}
            //catch (System.Net.WebException we)
            //{
            //    if (we.Message == "The operation has timed out")
            //        _access = false;
            //}

            //if (!nwt.IsAvailable || !_access)
            //{
            //    winapi.Funcs.OutputDebugString("Network is unavailable");
            //    return load;
            //}
            
            for (int i = 0; i < exFiles.Length; i++)
                try
                {
                    //Com_WinApi.OutputDebugString("check for index " + i);
                    // CHECK IF EXCHANGE FILE AVAILABLE
                    if (AsyncFunc.FileExists(exFiles[i], driver.Config.ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout))
                    {
                        if (!AsyncFunc.FileExists(localFiles[i], driver.Config.ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout))
                        {
                            load[i] = exFiles[i];
                            driver.Config.ConfigManager.Instance.CommonConfiguration.ADD_updateDateTime[i] = Microsoft.VisualBasic.FileSystem.FileDateTime(exFiles[i]);
                        }
                        else
                        {
                            dTime = Microsoft.VisualBasic.FileSystem.FileDateTime(exFiles[i]);

                            //hExFile = winapi.Funcs.CreateFile(exFiles[i], winapi.Enums.dwDesiredAccess.NONE,
                            //    winapi.Enums.dwShareMode.FILE_SHARE_READ, 0, winapi.Enums.dwCreationDisposion.OPEN_EXISTING, 0, IntPtr.Zero);
                            //if (hExFile == (IntPtr)winapi.Consts.INVALID_HANDLE_VALUE)
                            //    continue;
                            //fOK = winapi.Funcs.GetFileTime(hExFile, out fTime, out fTime, out fLastWiriteTime);
                            //winapi.Funcs.CloseHandle(hExFile);

                            if (dTime > driver.Config.ConfigManager.Instance.CommonConfiguration.ADD_updateDateTime[i])
                            {
                                load[i] = exFiles[i];
                                driver.Config.ConfigManager.Instance.CommonConfiguration.ADD_updateDateTime[i] = dTime;
                            }
                        }
                    }
                    else
                        if (i == 0)
                        {
                            // CHECK IF EXCHANGE FOLDER AVAILABLE
                            //Com_WinApi.OutputDebugString("Art file is unvaliable _ CheckUpdate_end");
                            if (!AsyncFunc.FileExists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Exchnage, driver.Config.ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout))
                                load[0] = CoreConst.STATE_LAN_ERROR;
                            return load;
                        }
                }
                catch(Exception ex) {
                    CoreLib.WriteLog(ex, "driver.Lib.CheckForUpdate_single");
                }

            //Com_WinApi.OutputDebugString("CheckUpdate_end");
            return load;
        }//ok

        /* PayDesk Source Structure v2.0 */


        public delegate void UpdatesAvailable();

        // get available data
        public static Com_HashObject CheckGetDataSource(bool updateOnly)
        {
            
            /* main variables */
            //SortedList remoteFiles = new SortedList();
            //SortedList localFiles = new SortedList();
            //string[] remoteFiles = new string[3];
            //string[] localFiles = new string[3];

            // source file container
            Com_HashObject loadedData = new Com_HashObject();

            // local data container
            Com_HashObject fileData = new Com_HashObject();

            // data comparer
            DateTime exchangeFileDateTime = new DateTime();

            // source file name patterns
            Com_HashObject sourcePattern = new Com_HashObject();

            // file name pattern arguments
            //object[] patternArguments = new object[4];
            Com_HashObject patternArguments2 = new Com_HashObject();

            List<components.Shared.Enums.Enu_SourceEnums.pdDataItemType> allAvailableSources = new List<components.Shared.Enums.Enu_SourceEnums.pdDataItemType>()
            {
                components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Product,
                components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative,
                components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Client
            };

            /*
            Dictionary<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, string> remoteFiles = new Dictionary<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, string>();
            Dictionary<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, string> localFiles = new Dictionary<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, string>();
            Dictionary<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, string> tempFiles = new Dictionary<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, string>();
            */

            // patternt for remote files
            sourcePattern[CoreConst.SOURCE_REMOTE].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Product, "{0}\\Art_{2:D2}{1:D2}.SDF");
            sourcePattern[CoreConst.SOURCE_REMOTE].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative, "{0}\\Alt_{2:D2}{1:D2}.SDF");
            sourcePattern[CoreConst.SOURCE_REMOTE].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Client, "{0}\\Cli_{2:D2}.SDF");

            // patternt for temporary files
            sourcePattern[CoreConst.SOURCE_TEMP].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Product, "{0}\\Art_{2:D2}{1:D2}.tmp");
            sourcePattern[CoreConst.SOURCE_TEMP].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative, "{0}\\Alt_{2:D2}{1:D2}.tmp");
            sourcePattern[CoreConst.SOURCE_TEMP].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Client, "{0}\\Cli_{2:D2}.tmp");

            // patternt for local files
            sourcePattern[CoreConst.SOURCE_LOCAL].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Product, "{0}\\Products_{2:D2}{1:D2}.xml");
            sourcePattern[CoreConst.SOURCE_LOCAL].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative, "{0}\\Alternative_{2:D2}{1:D2}.xml");
            sourcePattern[CoreConst.SOURCE_LOCAL].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Client, "{0}\\Clients_{2:D2}.xml");

            // patternt arguments
            patternArguments2.SetValue("PATH", ConfigManager.Instance.CommonConfiguration.Path_Exchnage);
            patternArguments2.SetValue("SUBUNIT", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit);
            patternArguments2.SetValue("PROFILEID", string.Empty);


            //Hashtable load = new Hashtable();
            //string[] load_profile = new string[3] { "", "", "" };

            if (driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime == null)
                driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime = new Hashtable();
            //bool hasUpdates = false;

            /* loop by all available profiles */
            // [foreach 1]
            int dateTimeIndex = 0;
            Hashtable profile = new Hashtable();
            foreach (DictionaryEntry de in driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
            {

                // removing all previous files
                fileData.Clear();

                try
                {
                    profile = (Hashtable)de.Value;
                }
                catch { }

                /* checking for updates */
                Com_WinApi.OutputDebugString("CheckUpdate_begin");

                // profile index (ID)
                patternArguments2.SetValue("PROFILEID", int.Parse(de.Key.ToString()));

                // = remote files

                // set value at 0 index with path to remote directory (exchange folder)
                // if profile doesn't have that path we'll set it with default path
                if (profile == null || !profile.ContainsKey("SOURCE") || profile["SOURCE"] == null)
                    patternArguments2.SetValue("PATH", ConfigManager.Instance.CommonConfiguration.Path_Exchnage);
                else
                    patternArguments2.SetValue("PATH", profile["SOURCE"].ToString());


                // set all remote files
                fileData[CoreConst.SOURCE_REMOTE].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Product, string.Format(sourcePattern[CoreConst.SOURCE_REMOTE].GetValue<string>(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Product), patternArguments2.ValuesToArray()));
                //                   sourcePattern[CoreConst.SOURCE_REMOTE].GetValue()    profileSourceDir + "\\" + string.Format("Art_{1:D2}{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(de.Key.ToString())) + ".SDF";
                fileData[CoreConst.SOURCE_REMOTE].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative, string.Format(sourcePattern[CoreConst.SOURCE_REMOTE].GetValue<string>(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative), patternArguments2.ValuesToArray()));
                // profileSourceDir + "\\" + string.Format("Alt_{1:D2}{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(de.Key.ToString())) + ".SDF";
                fileData[CoreConst.SOURCE_REMOTE].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Client, string.Format(sourcePattern[CoreConst.SOURCE_REMOTE].GetValue<string>(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Client), patternArguments2.ValuesToArray()));
                //profileSourceDir + "\\" + string.Format("Cli_{0:D2}", int.Parse(de.Key.ToString())) + ".SDF";

                // = local files
                // set value at 0 index with path to local directory (articles folder)
                patternArguments2.SetValue("PATH", ConfigManager.Instance.CommonConfiguration.Path_Articles);
                // removing all previous files
                //fileData.Clear();
                // set all local files
                fileData[CoreConst.SOURCE_LOCAL].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Product, string.Format(sourcePattern[CoreConst.SOURCE_LOCAL].GetValue<string>(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative), patternArguments2.ValuesToArray()));
                //driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Art_{1:D2}{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(de.Key.ToString())) + ".xml";
                fileData[CoreConst.SOURCE_LOCAL].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative, string.Format(sourcePattern[CoreConst.SOURCE_LOCAL].GetValue<string>(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative), patternArguments2.ValuesToArray()));
                //driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Alt_{1:D2}{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(de.Key.ToString())) + ".xml";
                fileData[CoreConst.SOURCE_LOCAL].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Client, string.Format(sourcePattern[CoreConst.SOURCE_LOCAL].GetValue<string>(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative), patternArguments2.ValuesToArray()));
                //driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Cli_{0:D2}", int.Parse(de.Key.ToString())) + ".xml";

                // = temporary files
                // set value at 0 index with path to temporary directory (temp folder)
                // uncomment this lisne if it's neccessary to store remote file into temp dir. (Products dir default) patternArguments2.SetValue("PATH", ConfigManager.Instance.CommonConfiguration.Path_Temp);
                // removing all previous files
                //fileData.Clear();
                // set all temp files
                fileData[CoreConst.SOURCE_TEMP].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Product, string.Format(sourcePattern[CoreConst.SOURCE_TEMP].GetValue<string>(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative), patternArguments2.ValuesToArray()));
                //driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Art_{1:D2}{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(de.Key.ToString())) + ".tmp";
                fileData[CoreConst.SOURCE_TEMP].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative, string.Format(sourcePattern[CoreConst.SOURCE_TEMP].GetValue<string>(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative), patternArguments2.ValuesToArray()));
                //driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Alt_{1:D2}{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(de.Key.ToString())) + ".tmp";
                fileData[CoreConst.SOURCE_TEMP].SetValue(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Client, string.Format(sourcePattern[CoreConst.SOURCE_TEMP].GetValue<string>(components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative), patternArguments2.ValuesToArray()));
                //driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Cli_{0:D2}", int.Parse(de.Key.ToString())) + ".tmp";

                if (!driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime.ContainsKey(de.Key))
                    driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime.Add(de.Key, new DateTime[3]);


                /* ToDo
                 * 
                 * loop all server's files and detect for newer date
                 * also detect for deleted files 
                 * 
                 * if someone of them is newer than local's files
                 * we'll load all them directly.
                 * 
                 */

                // [foreach 11] check all files related to running profile
                // remoteFiles contains same keys as localFiles.
                // so we'll use remoteFiles keys
                dateTimeIndex = 0;
                foreach (components.Shared.Enums.Enu_SourceEnums.pdDataItemType sourceKey in allAvailableSources)
                {
                    // [try 1]
                    try
                    {
                        Com_WinApi.OutputDebugString("checking file " + fileData[CoreConst.SOURCE_REMOTE].GetValue(sourceKey));

                        loadedData[de.Key][CoreConst.SOURCE_REMOTE].SetValue(sourceKey, string.Empty);
                        loadedData[de.Key][CoreConst.SOURCE_LOCAL].SetValue(sourceKey, fileData[CoreConst.SOURCE_LOCAL].GetValue(sourceKey));
                        loadedData[de.Key][CoreConst.SOURCE_TEMP].SetValue(sourceKey, fileData[CoreConst.SOURCE_TEMP].GetValue(sourceKey));

                        // check if remote folder has source file
                        if (updateOnly && AsyncFunc.FileExists(fileData[CoreConst.SOURCE_REMOTE].GetValue<string>(sourceKey), driver.Config.ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout, CoreConst.SOURCE_REMOTE))
                        {
                            // get date and time of remote file
                            exchangeFileDateTime = Microsoft.VisualBasic.FileSystem.FileDateTime(fileData[CoreConst.SOURCE_REMOTE].GetValue<string>(sourceKey));
                            // if remote file datetime is larger than current local file
                            // add this file to loadedData collection for further loading into application
                            // (update is available)
                            if (exchangeFileDateTime > ((DateTime[])driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime[de.Key])[dateTimeIndex])
                            {
                                loadedData[de.Key][CoreConst.SOURCE_REMOTE].SetValue(sourceKey, fileData[CoreConst.SOURCE_REMOTE].GetValue(sourceKey));
                                ((DateTime[])driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_updateDateTime[de.Key.ToString()])[dateTimeIndex] = exchangeFileDateTime;
                            }
                        }

                        // if local file is removed but remote exists
                        // add it for loading
                        if (!AsyncFunc.FileExists(fileData[CoreConst.SOURCE_LOCAL].GetValue<string>(sourceKey), driver.Config.ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout, CoreConst.SOURCE_LOCAL) &&
                            AsyncFunc.FileExists(fileData[CoreConst.SOURCE_REMOTE].GetValue<string>(sourceKey), driver.Config.ConfigManager.Instance.CommonConfiguration.APP_RefreshTimeout, CoreConst.SOURCE_REMOTE))
                        {
                            loadedData[de.Key][CoreConst.SOURCE_REMOTE].SetValue(sourceKey, fileData[CoreConst.SOURCE_REMOTE].GetValue(sourceKey));
                        }

                    }// end of try 1
                    catch (Exception ex)
                    {
                        if (ex.Message == "Timeout" + CoreConst.SOURCE_REMOTE)
                            loadedData[de.Key][CoreConst.SOURCE_REMOTE][CoreConst.CONTAINER_STATE].SetValue(sourceKey, CoreConst.STATE_LAN_ERROR);
                        if (ex.Message == "Timeout" + CoreConst.SOURCE_LOCAL)
                            loadedData[de.Key][CoreConst.SOURCE_LOCAL][CoreConst.CONTAINER_STATE].SetValue(sourceKey, CoreConst.STATE_LAN_ERROR);
                    }
                    dateTimeIndex++;
                } // end of foreach 11

                Com_WinApi.OutputDebugString("CheckUpdate_end");
            } // end of foreach 1


            return loadedData;
        }
        public static void UpdateSource(Com_HashObject source, ref components.Components.DataContainer.DataContainer dc)
        {
            Dictionary<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, DataTableCreateMethod> dTCreator = new Dictionary<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, DataTableCreateMethod>();
            Dictionary<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, DataSourceReader> dSReader = new Dictionary<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, DataSourceReader>();

            // data table creators
            dTCreator[components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Product] = CreateDataTableForProduct;
            dTCreator[components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative] = CreateDataTableForAlternative;
            dTCreator[components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Client] = CreateDataTableForCard;
            dTCreator[components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Order] = CreateDataTableForOrder;

            // source readers
            dSReader[components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Product] = ReadProduct;
            dSReader[components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Alternative] = ReadAlternative;
            dSReader[components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Client] = ReadCard;
            // order reader is not implemented because of ugc.


            // steps to load data
            // 1. load new data or load existed
            // 2. create data sources
            // 3. fill data sources with loaded data

            DataSourceItem dSrcItem = new DataSourceItem();

            DataTable __tempDataTable = new DataTable();
            bool fOK = false;

            string __tempFilePath = string.Empty;

            // loop by profiles
            // [foreach 1 - profile]
            foreach (DictionaryEntry profileEntry in source)
            {
                foreach (KeyValuePair<components.Shared.Enums.Enu_SourceEnums.pdDataItemType, DataSourceReader> currentSourceReader in dSReader)
                {
                    // create data table
                    __tempDataTable = dTCreator[currentSourceReader.Key].Invoke();
                    __tempDataTable.TableName = string.Format("{0}_{1:D2}", currentSourceReader.Key, int.Parse(profileEntry.Key.ToString()));

                    //dSrcItem = new DataSourceItem(dTCreator[currentSourceReader.Key].Invoke(), currentSourceReader.Key.ToString());


                    // if there is remote file
                    // we'll copy and load it
                    if (!source[profileEntry.Key][CoreConst.SOURCE_REMOTE].GetValue(currentSourceReader.Key).ToString().Equals(string.Empty))
                    {
                        Com_WinApi.OutputDebugString("copy start");
                        __tempFilePath = source[profileEntry.Key][CoreConst.SOURCE_TEMP].GetValue(currentSourceReader.Key).ToString();
                        fOK = Com_WinApi.CopyFile(source[profileEntry.Key][CoreConst.SOURCE_REMOTE].GetValue(currentSourceReader.Key).ToString(), __tempFilePath, false);
                        Com_WinApi.OutputDebugString("copy end");
                        if (fOK && Com_WinApi.PathFileExists(__tempFilePath))
                        {
                            dSReader[currentSourceReader.Key].Invoke(__tempFilePath, ref __tempDataTable, int.Parse(profileEntry.Key.ToString()));
                            __tempDataTable.WriteXml(source[profileEntry.Key][CoreConst.SOURCE_LOCAL].GetValue(currentSourceReader.Key).ToString());
                        }
                        try
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(__tempFilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin); 
                        }
                        catch { }
                    }
                    else // check if there is local file we'll load it
                    {
                        __tempFilePath = source[profileEntry.Key][CoreConst.SOURCE_LOCAL].GetValue(currentSourceReader.Key).ToString();
                        if (Com_WinApi.PathFileExists(__tempFilePath)) try
                            {
                                dSrcItem.Source.ReadXml(__tempFilePath);
                            }
                            catch { }
                    }

                    dSrcItem = new DataSourceItem(__tempDataTable);
                    dc.Storages.Add(dSrcItem);
                }

                // ugc


                __tempDataTable = dTCreator[components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Order].Invoke();
                __tempDataTable.TableName = string.Format("{0}_{1:D2}", components.Shared.Enums.Enu_SourceEnums.pdDataItemType.Order, int.Parse(profileEntry.Key.ToString()));
                dSrcItem = new DataSourceItem(__tempDataTable);
                dSrcItem.Properties = DataWorkShared.GetStandartOrderInfoStructure2();
                dc.Storages.Add(dSrcItem);

            }// end of [foreach 1 - profile]



        }


>>>>>>> stable
        /* common */
        #region PrivateFunctions
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


            dTable.Columns.Add("C", typeof(long));

            dTable.Columns.Add("F", typeof(string));

            for (byte i = 0; i < driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ARTColumnName.Length; i++)
                dTable.Columns.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ARTColumnName[i], cTypes[i]);

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

            dTable.Columns.Add("C", typeof(long));

            dTable.Columns.Add("F", typeof(string));

            for (byte i = 0; i < driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ALTColumnName.Length; i++)
                dTable.Columns.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ALTColumnName[i], cTypes[i]);

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

            if (driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_CARDColumnName.Length != cTypes.Length)
                driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_CARDColumnName = new string[] { "CBC", "CID", "CDISC", "CPRICENO" };

            dTable.Columns.Add("C", typeof(long));
            for (byte i = 0; i < driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_CARDColumnName.Length; i++)
                dTable.Columns.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_CARDColumnName[i], cTypes[i]);

            dTable.Columns.Add("F", typeof(string));

            //dTable.TableName = "DCards";

            dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
            dTable.Columns["C"].AutoIncrement = true;
            dTable.Columns["C"].Unique = true;

            return dTable;
        }//ok
        private static DataTable CreateDataTableForOrder()
        {
            DataTable dTable = CreateDataTableForProduct();

            DataColumn dCol = new DataColumn("TOT");
            dCol.DataType = typeof(string);
            dCol.DefaultValue = "0";
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("TAX_VAL");
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("USEDDISC");
            dCol.DataType = typeof(bool);
            dCol.DefaultValue = (bool)true;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("DISC");
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("SUM");
            dCol.AllowDBNull = false;
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("ASUM");
            dCol.AllowDBNull = false;
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("TAX_MONEY");
            dCol.DataType = typeof(double);
            //dCol.Expression = "(ASUM*TAX_VAL)/(TAX_VAL+100)";
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("TMPTOT");
            dCol.DataType = typeof(string);
            dCol.DefaultValue = "0";
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("ORIGPRICE");
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dCol = new DataColumn("PRINTCOUNT");
            dCol.DataType = typeof(double);
            dCol.DefaultValue = (double)0.0;
            dTable.Columns.Add(dCol);

            dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
            dTable.Columns["C"].AutoIncrement = true;
            dTable.Columns["C"].Unique = true;

            return dTable;
        }//ok
        #endregion

    }
}
