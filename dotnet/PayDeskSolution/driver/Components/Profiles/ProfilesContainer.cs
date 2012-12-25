using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace driver.Components.Profiles
{
    public class ProfilesContainer
    {
        private Dictionary<string, AppProfile> profiles;

        private DataTable commonSaleItems;
        private DataTable commonProducts;
        private DataTable commonAlternateBarcodes;
        private DataTable commonCustomerProgramCards;


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
