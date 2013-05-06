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
        //private Hashtable props = new Hashtable();
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
        private bool _fl_runUpdateOnly = true;

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
        //public bool triggerRunUpdateOnly { get { return _fl_runUpdateOnly; } set { _fl_runUpdateOnly = value; } }

        /* data access */
        public Dictionary<string, AppProfile> Profiles { get { return profiles; } set { profiles = value; } }
        public AppProfile Default { get { return this[CoreConst.KEY_DEFAULT_PROFILE_ID]; } set { this[CoreConst.KEY_DEFAULT_PROFILE_ID] = value; } }
        public AppProfile this[object profileID] { get { return getProfile(profileID.ToString()); } set { profiles[profileID.ToString()] = value; } }


        public ProfilesContainer()
        {
            // init startup values
            triggerSingleMode = Configuration.CommonConfiguration.PROFILES_UseProfiles;
            valueOfUpdateMode = UpdateMode.ALLSOURCES;
            valueOfCurrentSubUnit = -1; // will cause force update at startup // Configuration.CommonConfiguration.APP_SubUnit;
            
            // init profiles
            initContainerProfiles(true);
        }

        /* general methods */


        public T getDefaultProfileValue<T>(string key)
        {
            return Default.getPropertyValue<T>(key);
        }
        // 
        // at startup app has subunit equals -1 to load data
        // 
        public void refresh(bool includeProfiles)
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
                valueOfUpdateMode = UpdateMode.ALLSOURCES;
                _local_needUpdate = true;
                // trigger event
                OnSubUnitChanged(EventArgs.Empty);
            }

            //if (includeProfiles)
                foreach (KeyValuePair<string, AppProfile> ap in profiles)
                    ap.Value.refresh();

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
            profiles.Add(profileID, new AppProfile(profileID, name, this));
            profiles[profileID].onPropertiesUpdated += new PropertiesUpdatedEventHandler(ProfilesContainer_onPropertiesUpdated);
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
            //profiles[profileID].resetAll();
        }

        /* = data management */

        public DataSet getDataAllProfiles(DataType dType)
        {
            DataSet ds = new DataSet();
            foreach (KeyValuePair<string, AppProfile> ap in profiles)
                ds.Tables.Add(ap.Value.CommonData[dType]);
            return ds;
        }

        public List<string> getProfileList()
        {
            List<string> _allProfiles = new List<string>();
            bool hasLegalProfile = Configuration.CommonConfiguration.PROFILES_Items.ContainsKey(Configuration.CommonConfiguration.PROFILES_LegalProgileID);
            foreach (DictionaryEntry de in Configuration.CommonConfiguration.PROFILES_Items)
                _allProfiles.Add(de.Key.ToString());
            if (hasLegalProfile)
            {
                _allProfiles.Remove(Configuration.CommonConfiguration.PROFILES_LegalProgileID.ToString());
                _allProfiles.Insert(0, Configuration.CommonConfiguration.PROFILES_LegalProgileID.ToString());
            }

            return _allProfiles;
        }

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
                    DataRow[] dRows = this.Default.CommonData[getValueByName<DataType>(prData[0])].Select("F = " + key);
                    foreach (DataRow dr in dRows)
                        dr.Delete();
                }
                else
                    this.Default.CommonData[getValueByName<DataType>(prData[0])].Clear();
                _local_fl_updated = true;
                this.Default.CommonData[getValueByName<DataType>(prData[0])].Merge(dt.Copy());
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
                    DataRow[] unusedRows = this.Default.CommonData[getValueByName<DataType>(dKeys[i])].Select(String.Join(" AND ", cleanupQuery.ToArray()));
                    foreach (DataRow dr in unusedRows)
                        dr.Delete();
                }

                OnDataUpdated(EventArgs.Empty);
            }
            else OnDataUnChanged(EventArgs.Empty);

            // this.triggerRunUpdateOnly = true;
            // 
            this.valueOfUpdateMode = UpdateMode.SERVERDATAONLY;
        }


        /* events */
        // * public event CashChangedEventHandler onCashChanged;
        public event DataUpdatedEventHandler onDataUpdated;
        public event DataUnchangedEventHandler onDataUnchanged;
        public event SuibUnitChangedEventHandler onSubUnitChanged;
        public event UpdateRequiredEventHandler onUpdateRequired;
        public event ProfileCommandReceivedEventHandler onProfileCommandReceived;

        // Invoke the Changed event; called whenever cash value changes
        /*protected virtual void OnPropertiesChanged(EventArgs e)
        {
            if (onCashChanged != null)
                onCashChanged(this, e);
        }*/

        protected virtual void OnDataUpdated(EventArgs e)
        {
            if (onDataUpdated != null)
                onDataUpdated(this, e);
        }

        protected virtual void OnDataUnChanged(EventArgs e)
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

        protected virtual void OnProfileCommandReceived(AppProfile sender, Hashtable props, string command, EventArgs e)
        {
            if (onProfileCommandReceived != null)
                onProfileCommandReceived(sender, props, command, e);
        }

        // profile event handlers
        void ProfilesContainer_onPropertiesUpdated(AppProfile sender, Hashtable props, string actionKey, EventArgs e)
        {
            OnProfileCommandReceived(sender, props, "pu_" + actionKey, e);
        }

    }

    // * public delegate void CashChangedEventHandler(object sender, EventArgs e);
    public delegate void DataUpdatedEventHandler(object sender, EventArgs e);
    public delegate void DataUnchangedEventHandler(object sender, EventArgs e);
    public delegate void SuibUnitChangedEventHandler(object sender, EventArgs e);
    public delegate void UpdateRequiredEventHandler(object sender, EventArgs e);
    public delegate void ProfileCommandReceivedEventHandler(AppProfile sender, Hashtable props, string command, EventArgs e);

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
