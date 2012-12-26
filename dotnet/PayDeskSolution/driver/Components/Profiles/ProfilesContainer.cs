using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

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

        /* data */
        private DataTable commonSaleItems;
        private DataTable commonProducts;
        private DataTable commonAlternateBarcodes;
        private DataTable commonCustomerProgramCards;
        /* cash values */
        private Hashtable commonCashInfo;
        /* runtime values */
        private string rtvClientID;
        private byte rtvCurrentSubUnit;
        private int rtvCurrentSearchType;
        private int rtvLastPayment;
        /* triggers */
        private bool _fl_taxDocRequired = false;
        private bool _fl_singleMode = false;
        private bool _fl_isReturnCheque = false;
        private bool _fl_isInvenCheque = false;
        private bool _fl_subUnitChanged = false;
        private bool _fl_useTotDisc = true;

        public void initProfiles(Hashtable profiles)
        {

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
    }
}
