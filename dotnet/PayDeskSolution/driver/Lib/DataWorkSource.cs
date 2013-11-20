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
            /* main variables */
            FileInfo[] sourceFiles = new FileInfo[3];
            FileInfo[] destinationFiles = new FileInfo[3];
            Hashtable load = new Hashtable();
            string[] load_profile = new string[3] { "", "", "" };
            bool hasUpdates = false;
            /* loop by all available profiles */
            string sourceDir = string.Empty;
            string destDir = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles;
            foreach (DictionaryEntry de in driver.Config.ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
            {
                Hashtable profile = (Hashtable)de.Value;
                hasUpdates = false;
                /* checking for updates */
                //Com_WinApi.OutputDebugString("CheckUpdate_begin");

                sourceDir = profile["SOURCE"].ToString();
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

                int profileID = int.Parse(de.Key.ToString());

                //sourceFiles = new FileInfo[] {
                //    new FileInfo(sourceDir + "\\" + string.Format("Art_{1:D2}{0:D2}", subUnit, profileID) + "_raw.xml"),
                //    new FileInfo(sourceDir + "\\" + string.Format("Alt_{1:D2}{0:D2}", subUnit, profileID) + "_raw.xml"),
                //    new FileInfo(sourceDir + "\\" + string.Format("Cli_{0:D2}", profileID) + "_raw.xml")
                //};
                //destinationFiles = new FileInfo[] {
                //    new FileInfo(destDir + "\\" + string.Format("Art_{1:D2}{0:D2}", subUnit, profileID) + ".xml"),
                //    new FileInfo(destDir + "\\" + string.Format("Alt_{1:D2}{0:D2}", subUnit, profileID) + ".xml"),
                //    new FileInfo(destDir + "\\" + string.Format("Cli_{0:D2}", profileID) + ".xml")
                //};
                sourceFiles = new FileInfo[] {
                    new FileInfo(sourceDir + "\\" + string.Format("Products_{0:D2}.xml", subUnit)),
                    new FileInfo(sourceDir + "\\" + string.Format("Alternatives_{0:D2}.xml", subUnit)),
                    new FileInfo(sourceDir + "\\" + string.Format("ClientCards_{0:D2}.xml", subUnit))
                };
                destinationFiles = new FileInfo[] {
                    new FileInfo(destDir + "\\" + string.Format("Products_{0:D2}.xml", subUnit)),
                    new FileInfo(destDir + "\\" + string.Format("Alternatives_{0:D2}.xml", subUnit)),
                    new FileInfo(destDir + "\\" + string.Format("ClientCards_{0:D2}.xml", subUnit))
                };
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
                for (int i = 0; i < sourceFiles.Length; i++)
                {
                    try
                    {
                        // when we have file in the source folder but not in dest folder
                        if (sourceFiles[i].Exists)
                        {
                            if (!destinationFiles[i].Exists)
                                load_profile[i] = sourceFiles[i].FullName;
                            else
                            {
                                if (hasUpdates || sourceFiles[i].LastWriteTimeUtc.Ticks > destinationFiles[i].LastWriteTimeUtc.Ticks)
                                {
                                    hasUpdates = true;
                                    load_profile[i] = sourceFiles[i].FullName;
                                }
                            }
                        }
                        else
                        {
                            if (i == 0 && !System.IO.Directory.Exists(sourceDir))
                                load_profile[i] = CoreConst.STATE_LAN_ERROR;
                        }
                    }
                    catch { ; }
                }

                load.Add(de.Key, (string[])load_profile.Clone());

                //Com_WinApi.OutputDebugString("CheckUpdate_end");
            }

            return load;
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
            int profileID = int.Parse(profile.ToString());
            IntPtr hFile = IntPtr.Zero;
            int subUnit = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit;
            // override subunit
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_Items.ContainsKey(profile))
            {
                Hashtable profileObj = (Hashtable)ConfigManager.Instance.CommonConfiguration.PROFILES_Items[profile];
                if (profileObj.ContainsKey("SUBUNIT"))
                    try
                    {
                        subUnit = int.Parse(profileObj["SUBUNIT"].ToString());
                    }
                    catch { }
            }
            DataTableCreateMethod[] crFunc = new DataTableCreateMethod[] {
                CreateDataTableForProduct,
                CreateDataTableForAlternative,
                CreateDataTableForCard
            };
            FileInfo[] artFiles = new FileInfo[] {
                //new FileInfo(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Art_{1:D2}{0:D2}.xml", subUnit, profileID)),
                //new FileInfo(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Alt_{1:D2}{0:D2}.xml", subUnit, profileID)),
                //new FileInfo(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Cli_{0:D2}.xml", profileID))
                new FileInfo(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Products_{0:D2}.xml", subUnit)),
                new FileInfo(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("Alternatives_{0:D2}.xml", subUnit)),
                new FileInfo(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles + "\\" + string.Format("ClientCards_{0:D2}.xml", subUnit))
            };

            if (!Directory.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles))
                Directory.CreateDirectory(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Articles);

            for (int i = 0; i < artFiles.Length; i++)
            {

                if (localLoadedTempFiles != null && localLoadedTempFiles[i] != string.Empty)
                {
                    //Com_WinApi.OutputDebugString("EXCHAGE LoadingFromNewSource_begin");
                    //dTables[i] = crFunc[i].Invoke();
                    //rdFunc[i].Invoke(localLoadedTempFiles[i], ref dTables[i], startupIndex);
                    //fs = new FileStream(artFiles[i].FullName, FileMode.Create);
                    //binF.Serialize(fs, dTables[i]);
                    //dTables[i].TableName = Path.GetFileNameWithoutExtension(localLoadedTempFiles[i]);
                    //dTables[i].WriteXml(fs);
                    //fs.Close();
                    //fs.Dispose();

                    dTables[i] = crFunc[i].Invoke();

                    dTables[i].TableName = Path.GetFileNameWithoutExtension(artFiles[i].Name);

                    // read from source dir
                    dTables[i].ReadXml(localLoadedTempFiles[i]);

                    // apply startup index
                    //int AutoIncrementSeed = startupIndex + 1000000 * profileID;
                    //dTables[i].Columns["C"].AutoIncrementSeed = AutoIncrementSeed;
                    //for (int rowIdx = 0, rowCount = dTables[i].Rows.Count; rowIdx < rowCount; rowIdx++)
                    //{
                    //    dTables[i].Rows[rowIdx]["C"] = AutoIncrementSeed + rowIdx;
                    //}

                    // and save to our storgae
                    dTables[i].WriteXml(artFiles[i].FullName, true);

                    exchangeFldUsed = true;
                }
                else
                {
                    // this will work when we load existed sources as well
                    if (!onlyUpdate && artFiles[i].Exists)
                    {
                        dTables[i] = crFunc[i].Invoke();
                        //Com_WinApi.OutputDebugString("LOCAL LoadingFromLocalCopy_begin");
                        //fs = new FileStream(artFiles[i].FullName, FileMode.Open, FileAccess.Read);
                        //dTables[i].ReadXml(fs);
                        //dTables[i] =  DataTable (DataTable)binF.Deserialize(fs);
                        //fs.Close();
                        //fs.Dispose();
                        dTables[i].TableName = Path.GetFileNameWithoutExtension(artFiles[i].Name);
                        dTables[i].ReadXml(artFiles[i].FullName);
                        //Com_WinApi.OutputDebugString("LOCAL LoadingFromLocalCopy_end");
                    }
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
        #endregion

    }
}
