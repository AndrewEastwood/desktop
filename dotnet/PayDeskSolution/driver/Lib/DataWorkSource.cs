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
        private static BinaryFormatter binF = new BinaryFormatter();

        /* general */
        public static Hashtable CheckForUpdate()
        {
            Hashtable data = new Hashtable();

            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                data = CheckForUpdate_profile();
            else
                data.Add(0, CheckForUpdate_single());

            return data;
        }
        public static string[] LoadFilesOnLocalTempFolder(string[] exchangeFiles, object profile)
        {
            string[] tmpFiles = new string[3];
            string[] fileNamePatterns = new string[3];

            if (driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
            {
                fileNamePatterns[0] = "Art_{1:D2}{0:D2}.SDF";
                fileNamePatterns[1] = "Alt_{1:D2}{0:D2}.SDF";
                fileNamePatterns[2] = "Cli_{1:D2}.SDF";
            }
            else
            {
                fileNamePatterns[0] = "Art_{0:D2}.SDF";
                fileNamePatterns[1] = "Alt_{0:D2}.SDF";
                fileNamePatterns[2] = "Cli_BC.SDF";
            }

            tmpFiles[0] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format(fileNamePatterns[0], driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(profile.ToString()));
            tmpFiles[1] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format(fileNamePatterns[1], driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(profile.ToString()));
            tmpFiles[2] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format(fileNamePatterns[2], driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(profile.ToString()));
            
            bool fOK = false;
            string[] loadedFilesOnLoacal = new string[3] { "", "", "" };

            for (int i = 0; i < tmpFiles.Length; i++)
                if (!string.IsNullOrEmpty(exchangeFiles[i]))
                {
                    try
                    {
                        //Com_WinApi.OutputDebugString("copy start");
                        fOK = Com_WinApi.CopyFile(exchangeFiles[i], tmpFiles[i], false);
                        //Com_WinApi.OutputDebugString("copy end");
                        if (fOK && Com_WinApi.PathFileExists(tmpFiles[i]))
                            loadedFilesOnLoacal[i] = tmpFiles[i];
                    }
                    catch { }
                }

            return loadedFilesOnLoacal;
        }
        /// <summary>
        /// Завантаження інформації товарів
        /// </summary>
        /// <param name="localLoadedTempFiles">Шляхи до файлів</param>
        /// <param name="onlyUpdate">Якщо ture то завантажувати інформацію лише з нових файлів в іншому випадку завантажувати з всіх</param>
        /// <returns>Завантажена інформація</returns>
        public static object[] LoadData(string[] localLoadedTempFiles, bool onlyUpdate, object profile, int startupIndex)
        {
            //Com_WinApi.OutputDebugString("--- LoadData_begin");

            FileStream fs = null;
            DataTable[] dTables = new DataTable[3];
            bool exchangeFldUsed = false;
            IntPtr hFile = IntPtr.Zero;
            DataSourceReader[] rdFunc = new DataSourceReader[3];
            rdFunc[0] = ReadProduct;
            rdFunc[1] = ReadAlternative;
            rdFunc[2] = ReadCard;
            DataTableCreateMethod[] crFunc = new DataTableCreateMethod[3];
            crFunc[0] = CreateDataTableForProduct;
            crFunc[1] = CreateDataTableForAlternative;
            crFunc[2] = CreateDataTableForCard;

            if (!Directory.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles))
                Directory.CreateDirectory(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles);

            string[] artFiles = new string[3];
            if (driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
            {
                artFiles[0] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Art_{1:D2}{0:D2}.xml", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(profile.ToString()));
                artFiles[1] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Alt_{1:D2}{0:D2}.xml", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(profile.ToString()));
                artFiles[2] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Cli_{0:D2}.xml", int.Parse(profile.ToString()));
            }
            else
            {
                artFiles[0] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Art_{0:D2}.xml", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit);
                artFiles[1] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Alt_{0:D2}.xml", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit);
                artFiles[2] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Cli.xml");
            }

            for (int i = 0; i < artFiles.Length; i++)
                if (localLoadedTempFiles != null && localLoadedTempFiles[i] != string.Empty)
                {
                    //Com_WinApi.OutputDebugString("EXCHAGE LoadingFromNewSource_begin");
                    dTables[i] = crFunc[i].Invoke();
                    rdFunc[i].Invoke(localLoadedTempFiles[i], ref dTables[i], startupIndex);
                    fs = new FileStream(artFiles[i], FileMode.Create);
                    binF.Serialize(fs, dTables[i]);
                    //dTables[i].TableName = Path.GetFileNameWithoutExtension(localLoadedTempFiles[i]);
                    //dTables[i].WriteXml(fs);
                    fs.Close();
                    fs.Dispose();
                    exchangeFldUsed = true;
                    if (!driver.Config.ConfigManager.Instance.CommonConfiguration.Content_Articles_KeepDataAfterImport)
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(localLoadedTempFiles[i], UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    //Com_WinApi.OutputDebugString("EXCHAGE LoadingFromNewSource_end");
                }
                else
                {
                    if (!onlyUpdate && Com_WinApi.PathFileExists(artFiles[i]))
                    {
                        //Com_WinApi.OutputDebugString("LOCAL LoadingFromLocalCopy_begin");
                        fs = new FileStream(artFiles[i], FileMode.Open, FileAccess.Read);
                        //dTables[i].ReadXml(fs);
                        dTables[i] = (DataTable)binF.Deserialize(fs);
                        if (!new Com_SecureRuntime().FullLoader())
                        {
                            DataTable tDt = dTables[i].Clone();
                            for (int jp = 0; jp < 10; jp++)
                            {
                                if (dTables[i].Rows.Count <= jp) break;
                                tDt.Rows.Add(dTables[i].Rows[jp].ItemArray);
                            }
                            dTables[i] = tDt.Copy();
                        }
                        fs.Close();
                        fs.Dispose();
                        //Com_WinApi.OutputDebugString("LOCAL LoadingFromLocalCopy_end");
                    }
                }

            if (fs != null)
                fs.Dispose();

            object[] rez = new object[] { dTables, exchangeFldUsed };


            //Com_WinApi.OutputDebugString("--- LoadData_end");
            return rez;
        }//ok


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

                sourceDir = profile["SOURCE"].ToString();
                if (sourceDir == string.Empty)
                    sourceDir = ConfigManager.Instance.CommonConfiguration.Path_Exchnage;

                exFiles = new string[3];
                exFiles[0] = sourceDir + "\\" + string.Format("Art_{1:D2}{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(de.Key.ToString())) + ".SDF";
                exFiles[1] = sourceDir + "\\" + string.Format("Alt_{1:D2}{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(de.Key.ToString())) + ".SDF";
                exFiles[2] = sourceDir + "\\" + string.Format("Cli_{0:D2}", int.Parse(de.Key.ToString())) + ".SDF";

                localFiles = new string[3];
                localFiles[0] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Art_{1:D2}{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(de.Key.ToString())) + ".xml";
                localFiles[1] = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Alt_{1:D2}{0:D2}", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, int.Parse(de.Key.ToString())) + ".xml";
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


        /* common */
        #region PrivateFunctions
        private delegate DataTable DataTableCreateMethod();
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

        private delegate void DataSourceReader(string path, ref DataTable dTable, int startupIndex);
        private static void ReadProduct(string path, ref DataTable dTable, int startupIndex)
        {
            byte err_cnt = 0;

            if (File.Exists(path))
            {
                //StreamReader sr = null;
                Stream sr = null;
                do
                {
                    try
                    {
                        //sr = new StreamReader(path, Encoding.Default);
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
                    return;

                BufferedStream buff = new BufferedStream(sr, 131072);
                StreamReader reader = new StreamReader(buff, Encoding.Default);
                StringReader strRd = new StringReader(reader.ReadToEnd());
                sr.Close();
                sr.Dispose();
                string line = string.Empty;
                DataRow dRow = dTable.NewRow();
                int sc = 0;
                bool isfull = new Com_SecureRuntime().FullLoader();
                dTable.Rows.Clear();

                string firmId = CoreConst.KEY_DEFAULT_PROFILE_ID;
                if (driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                {
                    string[] fName = Path.GetFileNameWithoutExtension(path).Split(new char[] { '_' });
                    try
                    {
                        firmId = int.Parse(fName[1].Substring(0, 2)).ToString();
                    }
                    catch { }
                }

                dTable.Columns["C"].AutoIncrementSeed = startupIndex + 10 * int.Parse(firmId);
                if (dTable.Columns["C"].AutoIncrementSeed == 0)
                    dTable.Columns["C"].AutoIncrementSeed = 1;


                Com_WinApi.OutputDebugString("ReadArtSDF_begin");
                while ((line = strRd.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    
                    if (!isfull && sc >= 10)
                        break;
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

                    dRow["TQ"] = MathLib.GetDouble(line.Substring(146, 12).Trim());//tq
                    dRow["PACK"] = MathLib.GetDouble(line.Substring(158, 18).Trim());//pack
                    dRow["WEIGHT"] = MathLib.GetDouble(line.Substring(176, 18).Trim());//weight
                    dRow["PRICE"] = MathLib.GetDouble(line.Substring(194, 12).Trim());//price
                    dRow["PR1"] = MathLib.GetDouble(line.Substring(206, 12).Trim());//pr1
                    dRow["PR2"] = MathLib.GetDouble(line.Substring(218, 12).Trim());//pr2
                    dRow["PR3"] = MathLib.GetDouble(line.Substring(230, 12).Trim());//pr3
                    dRow["Q2"] = MathLib.GetDouble(line.Substring(242, 12).Trim());//q2
                    dRow["Q3"] = MathLib.GetDouble(line.Substring(254, 10).Trim());//q3


                    //dRow["C"] = dRow["ID"];
                    dTable.Rows.Add(dRow);
                    //dRow = dTable.NewRow();

                    sc++;
                }
                Com_WinApi.OutputDebugString("ReadArtSDF_end");


            }
        }//ok
        private static void ReadAlternative(string path, ref DataTable dTable, int startupIndex)
        {
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
                    return;

                BufferedStream buff = new BufferedStream(sr, 131072);
                StreamReader reader = new StreamReader(buff, Encoding.Default);
                StringReader strRd = new StringReader(reader.ReadToEnd());
                sr.Close();
                sr.Dispose();
                string line = string.Empty;
                DataRow dRow = dTable.NewRow();
                int testdsc = 0;
                bool test1 = new Com_SecureRuntime().FullLoader();
                dTable.Rows.Clear();

                string firmId = CoreConst.KEY_DEFAULT_PROFILE_ID;
                if (driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                {
                    string[] fName = Path.GetFileNameWithoutExtension(path).Split(new char[] { '_' });
                    try
                    {
                        firmId = int.Parse(fName[1].Substring(0, 2)).ToString();
                    }
                    catch { }
                }

                dTable.Columns["C"].AutoIncrementSeed = startupIndex + 10 * int.Parse(firmId) + 1;
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
        }//ok
        private static void ReadCard(string path, ref DataTable dTable, int startupIndex)
        {
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
                    return;

                string line = string.Empty;
                DataRow dRow = dTable.NewRow();
                //long index = 0;
                int myrd = 0;
                bool reader = new Com_SecureRuntime().FullLoader();

                string firmId = CoreConst.KEY_DEFAULT_PROFILE_ID;
                if (driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                {
                    string[] fName = Path.GetFileNameWithoutExtension(path).Split(new char[] { '_' });
                    try
                    {
                        firmId = int.Parse(fName[1].Substring(0, 2)).ToString();
                    }
                    catch { }
                }

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;

                    if (!reader && myrd >= 10)
                        break;

                    if (reader && (myrd + 1) % 100 == 0)
                        reader = new Com_SecureRuntime().FullLoader();

                    //dRow["C"] = index++;

                    dRow["F"] = firmId;
                    dRow["CBC"] = line.Substring(0, 20).Trim();//abc
                    dRow["CID"] = line.Substring(20, 10).Trim();//aid
                    dRow["CDISC"] = MathLib.GetDouble(line.Substring(30, 6).Trim());//cdisc
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
        }//ok 

        #endregion

    }
}
