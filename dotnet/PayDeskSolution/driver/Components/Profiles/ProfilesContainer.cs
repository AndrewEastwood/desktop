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

        /* data */
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

        /* methods */

        public void initProfiles(Hashtable profiles)
        {

        }

        public void updateCash()
        {
            foreach (KeyValuePair<string, AppProfile> pr in profiles)
                pr.Value.calcCash();
            if (!triggerInventCheque)
                OnCashChanged(EventArgs.Empty);
        }

        public AppProfile getProfileById(string id)
        {
            return profiles[id];
        }

        public void resetProfileById(string id)
        {
            profiles[id].resetAll();
        }

        public void resetProfileById()
        {

        }

        /* events */
        public event CashChangedEventHandler eventCashChanged;

        // Invoke the Changed event; called whenever cash value changes
        protected virtual void OnCashChanged(EventArgs e)
        {
            if (eventCashChanged != null)
                eventCashChanged(this, e);
        }


    }

    public delegate void CashChangedEventHandler(object sender, EventArgs e);
}
