﻿using System;
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
            dSources[CoreConst.DATA_CONTAINER_PRODUCT] = new FileInfo(string.Format("{0}\\Products_{1:D2}.xml", sourceDir, subUnit));
            dSources[CoreConst.DATA_CONTAINER_ALTERNATIVE] = new FileInfo(string.Format("{0}\\Alternatives_{1:D2}.xml", sourceDir, subUnit));
            dSources[CoreConst.DATA_CONTAINER_CLIENT] = new FileInfo(string.Format("{0}\\ClientCards_{1:D2}.xml", sourceDir, subUnit));

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
