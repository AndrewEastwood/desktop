using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using driver.Config;

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
        private Dictionary<string, AppProfile> profiles;

        // configuration
        private ConfigManager Configuration { get { return ConfigManager.Instance; } }
        /* commons */
        private string[] dataKeys = { "ORDER", "PRODUCT", "ALTBC", "DCARD" };
        /* data */
        private Dictionary<string, DataTable> data;
        private DataTable commonSaleItems;
        private DataTable commonProducts;
        private DataTable commonAlternateBarcodes;
        private DataTable commonCustomerProgramCards;
        /* cash values */
        private Hashtable commonCashInfo;
        /* runtime values */
        private string rtvClientID;
        private int rtvClientPriceNo;
        private int rtvCurrentSubUnit;
        private int rtvCurrentSearchType;
        private int rtvLastPayment;
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
        public bool triggerTaxDocRequired { get { return _fl_taxDocRequired; } set { _fl_taxDocRequired = value; } }
        public bool triggerSingleMode { get { return _fl_singleMode; } set { _fl_singleMode = value; } }
        public bool triggerReturnCheque { get { return _fl_isReturnCheque; } set { _fl_isReturnCheque = value; } }
        public bool triggerInventCheque { get { return _fl_isInvenCheque; } set { _fl_isInvenCheque = value; } }
        public bool triggerUseTotDisc { get { return _fl_useTotDisc; } set { _fl_useTotDisc = value; } }

        /* data access */

        public DataTable dataOrder { get { return dataGetOrder(); } }
        public DataTable dataProducts { get { return dataGetProducts(); } }

        public ProfilesContainer()
        {
            data = new Dictionary<string, DataTable>();
            for (int i = 0; i < dataKeys.Length; i++)
                data.Add(dataKeys[i], new DataTable(dataKeys[i]));

            profiles = new Dictionary<string, AppProfile>();
            if (Configuration.CommonConfiguration.PROFILES_UseProfiles)
                foreach (DictionaryEntry de in Configuration.CommonConfiguration.PROFILES_Items)
                    ;
            else
                profileAdd(driver.Common.CoreConst.KEY_DEFAULT_PROFILE_ID, "Default");
        }

        /* general methods */

        

        /* = profiles */
        public AppProfile profileGet(string id)
        {
            if (profileExists(id))
                return profiles[id];
            return null;
        }

        public void profileAdd(string name, string id)
        {
            profiles.Add(id, new AppProfile(id, name));
        }

        public bool profileRemove(string id)
        {
            return profiles.Remove(id);
        }

        public bool profileExists(string id)
        {
            return profiles.ContainsKey(id);
        }

        public void profileReset(string id)
        {
            profiles[id].resetAll();
        }

        /* = data management */
        public void updateCash()
        {
            foreach (KeyValuePair<string, AppProfile> pr in profiles)
                pr.Value.calcCash();
            if (!triggerInventCheque)
                OnCashChanged(EventArgs.Empty);
        }

        /* in progress */

        public DataTable dataGetOrder()
        {
            return getData(DataType.ORDER);
        }

        public DataTable dataGetProducts()
        {
            return getData(DataType.PRODUCT);
        }

        public DataTable getData(DataType dType)
        {
            //string n = Enum.GetName(typeof(DataType), dType); 
            return this.data[dType.ToString()];
        }

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
                    DataRow[] dRows = this.data[prData[0].ToUpper()].Select("F = " + key);
                    foreach (DataRow dr in dRows)
                        dr.Delete();
                } else
                    this.data[prData[0].ToUpper()].Clear();
                _local_fl_updated = true;
                this.data[prData[0].ToUpper()].Merge(dt);
            }

            if (_local_fl_updated)
            {
                /* Removing unused rows */
                string cleanupQuery = string.Empty;
                foreach (string existedProfiles in allProfiles)
                {
                    cleanupQuery += " F <> " + existedProfiles + " AND ";
                }
                cleanupQuery = cleanupQuery.Trim(new char[] { ' ', 'A', 'N', 'D' });
                for (int i = 0; i < dataKeys.Length; i++)
                {
                    DataRow[] unusedRows = this.data[dataKeys[i]].Select(cleanupQuery);
                    foreach (DataRow dr in unusedRows)
                        dr.Delete();
                }

                OnDataUpdated(EventArgs.Empty);
            }
        }

        public void orderProductAdd()
        {

        }

        public void orderProductUpdate()
        {
        }

        public void orderProductRemove()
        {
        }



        /* events */
        public event CashChangedEventHandler onCashChanged;
        public event DataUpdatedEventHandler onDataUpdated;

        // Invoke the Changed event; called whenever cash value changes
        protected virtual void OnCashChanged(EventArgs e)
        {
            if (onCashChanged != null)
                onCashChanged(this, e);
        }

        protected virtual void OnDataUpdated(EventArgs e)
        {
            if (onDataUpdated != null)
                onDataUpdated(this, e);
        }


    }

    public delegate void CashChangedEventHandler(object sender, EventArgs e);
    public delegate void DataUpdatedEventHandler(object sender, EventArgs e);

    public enum DataType : int
    {
        ORDER = 0,
        PRODUCT = 1,
        ALTERNATEBC = 2,
        DCARDS = 3
    }
}
