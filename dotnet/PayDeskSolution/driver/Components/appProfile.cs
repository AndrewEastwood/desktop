using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace driver.Components
{
    public class AppProfile
    {
        private string id;
        private DataTable Cheque;
        private DataTable Products;
        private DataTable AltBC;
        private DataTable Cards;

        // Order Data
        private Hashtable cashInfo;
        private Hashtable discountInfo;

        // triggers
        private bool trExchangeAccessError;

        private void setup()
        {
        }
        private void resetCash() { }
        private void resetDisc() { }
        private void resetAll() { }


        private Hashtable props;
    }
}
