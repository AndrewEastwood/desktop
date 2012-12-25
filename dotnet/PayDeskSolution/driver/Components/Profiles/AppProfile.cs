using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace driver.Components.Profiles
{
    public class AppProfile
    {
        private string id;
        private DataTable saleItems;
        private DataTable products;
        private DataTable alternateBarcodes;
        private DataTable customerProgramCards;

        // Order Data
        private Hashtable cashInfo;
        private Hashtable discountInfo;
        private Hashtable props;

        // triggers
        private bool trExchangeAccessError;

        public void setup()
        {
        }
        public void resetCash() { }
        public void resetDisc() { }
        public void resetAll() { }

        public DataTable getOrderDocument () {
            return null;
        }


    }
}
