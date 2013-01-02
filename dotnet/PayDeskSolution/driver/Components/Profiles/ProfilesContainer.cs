using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using driver.Config;
using driver.Common;
using driver.Lib;

namespace driver.Components.Profiles
{
    // it is common container with one or more profiles
    // one profile is always used by default even when all are removed
    // singlemode uses one general profile with ID = 0
    // 
    public class ProfilesContainer
    {
        // do not have own configuration settings
        // will be added dynamically at runtime
        private Dictionary<string, AppProfile> profiles = new Dictionary<string,AppProfile>();

        // configuration
        public ConfigManager Configuration { get { return ConfigManager.Instance; } }
        /* data */
        // * private Dictionary<DataType, DataTable> data = new Dictionary<DataType,DataTable>();
        private Hashtable props = new Hashtable();
        /* cash values */
        // * private Hashtable commondataPropsInfo;
        /* runtime values */
        private string rtvClientID = "";
        private int rtvClientPriceNo = 0;
        private int rtvCurrentSubUnit = 0;
        private int rtvCurrentSearchType = 0;
        private int rtvLastPayment = 0;
        private UpdateMode rtvUpdateMode = UpdateMode.ALLSOURCES;
        /* triggers */
        private bool _fl_taxDocRequired = false;
        private bool _fl_singleMode = false;
        private bool _fl_isReturnCheque = false;
        private bool _fl_isInvenCheque = false;
        private bool _fl_subUnitChanged = false;
        private bool _fl_useTotDisc = true;

        /* properties */
        public string valueOfClientID { get { return rtvClientID; } set { rtvClientID = value; } }
        public int valueOfCurrentSubUnit { get { return rtvCurrentSubUnit; } set { rtvCurrentSubUnit = value; } }
        public int valueOfCurrentSearchType { get { return rtvCurrentSearchType; } set { rtvCurrentSearchType = value; } }
        public int valueOfLastPayment { get { return rtvLastPayment; } set { rtvLastPayment = value; } }
        public int valueOfClientPriceNo { get { return rtvClientPriceNo; } set { rtvClientPriceNo = value; } }
        public UpdateMode valueOfUpdateMode { get { return rtvUpdateMode; } set { rtvUpdateMode = value; } }
        public bool triggerTaxDocRequired { get { return _fl_taxDocRequired; } set { _fl_taxDocRequired = value; } }
        public bool triggerSingleMode { get { return _fl_singleMode; } set { _fl_singleMode = value; } }
        public bool triggerReturnCheque { get { return _fl_isReturnCheque; } set { _fl_isReturnCheque = value; } }
        public bool triggerInventCheque { get { return _fl_isInvenCheque; } set { _fl_isInvenCheque = value; } }
        public bool triggerUseTotDisc { get { return _fl_useTotDisc; } set { _fl_useTotDisc = value; } }

        /* data access */

        public AppProfile Default { get { return this[CoreConst.KEY_DEFAULT_PROFILE_ID]; } set { this[CoreConst.KEY_DEFAULT_PROFILE_ID] = value; } }
        public AppProfile this[string profileID] { get { return getProfile(CoreConst.KEY_DEFAULT_PROFILE_ID); } set { profiles[CoreConst.KEY_DEFAULT_PROFILE_ID] = value; } }


        public ProfilesContainer()
        {
            // init data containers
            string[] dataKeys = Enum.GetNames(typeof(DataType));
            for (int i = 0; i < dataKeys.Length; i++)
            {
                data.Add((DataType)i, setupEmptyDataTable((DataType)i));
                data[(DataType)i].TableName = dataKeys[i];
            }
            // init startup values
            triggerSingleMode = Configuration.CommonConfiguration.PROFILES_UseProfiles;
            valueOfCurrentSubUnit = Configuration.CommonConfiguration.APP_SubUnit;
            
            // init profiles
            initContainerProfiles(true);
        }

        /* general methods */

        public void refresh()
        {
            bool _local_needUpdate = false;
            if (triggerSingleMode != Configuration.CommonConfiguration.PROFILES_UseProfiles)
            {
                // save current mode to compare in future
                triggerSingleMode = Configuration.CommonConfiguration.PROFILES_UseProfiles;
                valueOfUpdateMode = UpdateMode.ALLSOURCES;
                _local_needUpdate = true;
                initContainerProfiles(false);
            }

            if (valueOfCurrentSubUnit != Configuration.CommonConfiguration.APP_SubUnit)
            {
                // save current subunit to compare in future
                valueOfCurrentSubUnit = Configuration.CommonConfiguration.APP_SubUnit;
                // trigger event
                OnSubUnitChanged(EventArgs.Empty);
                valueOfUpdateMode = UpdateMode.ALLSOURCES;
                _local_needUpdate = true;
            }

            if (_local_needUpdate)
                OnUpdateRequired(EventArgs.Empty);
        }

        public void initContainerProfiles(bool clearAll)
        {
            if (clearAll && profiles.Count > 0)
                profiles.Clear();

            // main profile
            // all general operations will be running within that.
            profileAdd(driver.Common.CoreConst.KEY_DEFAULT_PROFILE_ID, "Default");


            // add additional profiles
            if (Configuration.CommonConfiguration.PROFILES_UseProfiles)
            {
                // remove unused
                // add new
                // -- skip existed
                foreach (DictionaryEntry de in Configuration.CommonConfiguration.PROFILES_Items)
                    if (!profiles.ContainsKey(de.Key.ToString()))
                        profileAdd(de.Key.ToString(), ((Hashtable)de.Value)["NAME"].ToString());
            }
        }

        public T getValueByName<T>(string name)
        {
            Array items = Enum.GetValues(typeof(T));
            for (int i = 0; i < items.Length; i++)
                if (items.GetValue(i).ToString().ToUpper().Equals(name.ToUpper()))
                    return (T)items.GetValue(i);
            return default(T);
        }

        public string[] getDataKeys()
        {
            return Enum.GetNames(typeof(DataType));
        }

        /* = profiles */
        public AppProfile getProfile(string profileID)
        {
            if (profileExists(profileID))
                return profiles[profileID];
            return null;
        }

        public void profileAdd(string profileID, string name)
        {
            profiles.Add(profileID, new AppProfile(profileID, name));
        }

        public bool profileRemove(string profileID)
        {
            return profiles.Remove(profileID);
        }

        public bool profileExists(string profileID)
        {
            return profiles.ContainsKey(profileID);
        }

        public void profileReset(string profileID)
        {
            profiles[profileID].resetAll();
        }

        /* = data management */


        /* in progress */

        public void setupData(DataSet data)
        {
            List<string> allProfiles = new List<string>();
            bool _local_fl_updated = false;

            // add all data
            foreach (DataTable dt in data.Tables)
            {
                // get profile name and data
                // 0 - data name
                // 1 - profile name
                string[] prData = dt.TableName.Split(new[] { '=' });
                string key = prData[1];

                // add unique profile index
                if (!allProfiles.Contains(key))
                    allProfiles.Add(key);

                if (Configuration.CommonConfiguration.PROFILES_UseProfiles)
                {
                    DataRow[] dRows = this.Default.Data[getValueByName<DataType>(prData[0])].Select("F = " + key);
                    foreach (DataRow dr in dRows)
                        dr.Delete();
                }
                else
                    this.Default.Data[getValueByName<DataType>(prData[0])].Clear();
                _local_fl_updated = true;
                this.Default.Data[getValueByName<DataType>(prData[0])].Merge(dt.Copy());
            }

            if (_local_fl_updated)
            {
                /* Removing unused rows */
                List<string> cleanupQuery = new List<string>();
                foreach (string existedProfiles in allProfiles)
                    cleanupQuery.Add(" F <> " + existedProfiles);
                string[] dKeys = getDataKeys();
                for (int i = 0; i < getDataKeys().Length; i++)
                {
                    DataRow[] unusedRows = this.Default.Data[getValueByName<DataType>(dKeys[i])].Select(String.Join(" AND ", cleanupQuery.ToArray()));
                    foreach (DataRow dr in unusedRows)
                        dr.Delete();
                }

                OnDataUpdated(EventArgs.Empty);
            }
            else OnDataUchanged(EventArgs.Empty);
        }


        /* events */
        public event CashChangedEventHandler onCashChanged;
        public event DataUpdatedEventHandler onDataUpdated;
        public event DataUnchangedEventHandler onDataUnchanged;
        public event SuibUnitChangedEventHandler onSubUnitChanged;
        public event UpdateRequiredEventHandler onUpdateRequired;

        // Invoke the Changed event; called whenever cash value changes
        protected virtual void OnPropertiesChanged(EventArgs e)
        {
            if (onCashChanged != null)
                onCashChanged(this, e);
        }

        protected virtual void OnDataUpdated(EventArgs e)
        {
            if (onDataUpdated != null)
                onDataUpdated(this, e);
        }

        protected virtual void OnDataUchanged(EventArgs e)
        {
            if (onDataUnchanged != null)
                onDataUnchanged(this, e);
        }

        protected virtual void OnSubUnitChanged(EventArgs e)
        {
            if (onSubUnitChanged != null)
                onSubUnitChanged(this, e);
        }

        protected virtual void OnUpdateRequired(EventArgs e)
        {
            if (onUpdateRequired != null)
                onUpdateRequired(this, e);
        }

        public DataTable setupEmptyDataTable(DataType dType)
        {
            DataTable dTable = new DataTable();
            switch (dType)
            {
                case DataType.ALTERNATEBC:
                    {
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
                        break;
                    }
                case DataType.PRODUCT:
                    {
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
                        break;
                    }
                case DataType.DCARDS:
                    {
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
                        break;
                    }
                case DataType.ORDER:
                    {
                        dTable = setupEmptyDataTable(DataType.PRODUCT);

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
                        break;
                    }
            }

            return dTable;
        }

    }

    public delegate void CashChangedEventHandler(object sender, EventArgs e);
    public delegate void DataUpdatedEventHandler(object sender, EventArgs e);
    public delegate void DataUnchangedEventHandler(object sender, EventArgs e);
    public delegate void SuibUnitChangedEventHandler(object sender, EventArgs e);
    public delegate void UpdateRequiredEventHandler(object sender, EventArgs e);

    public enum DataType : int
    {
        ORDER = 0,
        PRODUCT = 1,
        ALTERNATEBC = 2,
        DCARDS = 3
    }

    public enum UpdateMode : int
    {
        ALLSOURCES = 0,
        SERVERDATAONLY = 1
    }
}
