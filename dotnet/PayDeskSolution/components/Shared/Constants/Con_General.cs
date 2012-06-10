using System;
using System.Collections.Generic;
using System.Text;

namespace components.Shared.Constants
{
    public class Con_General
    {
        // common
        public const string KEY_DEFAULT = "DEFAULT";
        public const string CM_DELIM_DOWN = "_";

        // order structure
        public const string STORE_NO = "STORE_NO";
        public const string CLIENT_ID = "CLIENT_ID";
        public const string IS_RET = "IS_RET";
        public const string IS_LEGAL = "IS_LEGAL";
        public const string ORDER_NO = "ORDER_NO";
        public const string ORDER_SUMA = "ORDER_SUMA";
        public const string ORDER_REAL_SUMA = "ORDER_REAL_SUMA";
        public const string TAX_SUMA = "TAX_SUMA";
        public const string TAX_BILL = "TAX_BILL";
        public const string DISCOUNT = "DISCOUNT";
        public const string PAYMENT = "PAYMENT";
        public const string BILL = "BILL";

        // bill structure
        public const string OID = "OID";
        public const string BILL_NO = "BILL_NO";
        public const string OWNER_NO = "OWNER_NO";
        public const string DATETIME = "DATETIME";
        public const string DATETIMEEDIT = "DATETIMEEDIT";
        public const string COMMENT = "COMMENT";
        public const string IS_LOCKED = "IS_LOCKED";
        public const string PATH = "PATH";
        public const string DELETED_ROWS = "DELETED_ROWS";

        // discount structure
        public const string DISC_ALL_ITEMS = "DISC_ALL_ITEMS";
        public const string DISC_ARRAY_PERCENT = "DISC_ARRAY_PERCENT";
        public const string DISC_ARRAY_CASH = "DISC_ARRAY_CASH";
        public const string DISC_CONST_PERCENT = "DISC_CONST_PERCENT";
        public const string DISC_ONLY_PERCENT = "DISC_ONLY_PERCENT";
        public const string DISC_ONLY_CASH = "DISC_ONLY_CASH";
        public const string DISC_FINAL_PERCENT = "DISC_FINAL_PERCENT";
        public const string DISC_FINAL_CASH = "DISC_FINAL_CASH";

        // calculations
        public const string CALC_CHEQUE_SUMA = "CALC_CHEQUE_SUMA";
        public const string CALC_REAL_SUMA = "CALC_REAL_SUMA";
        public const string CALC_TAX_SUMA = "CALC_TAX_SUMA";

        // statements
        public const string STATE_DATA_UPDATED = "STATE_DATA_UPDATED";
        public const string STATE_DATA_UPDATE_PENDING = "STATE_DATA_UPDATE_PENDING";
        public const string STATE_DATA_UPDATE_ONLY = "STATE_DATA_UPDATE_ONLY";
        public const string STATE_ADMIN = "STATE_ADMIN";
        public const string STATE_MENU_IS_ACTIVE = "STATE_MENU_IS_ACTIVE";
        public const string STATE_APP_SUBUNIT_CHANGED = "STATE_APP_SUBUNIT_CHANGED";
        public const string STATE_CALC_USE_TOTAL_DISC = "STATE_CALC_USE_TOTAL_DISC";
        public const string STATE_APP_OK = "STATE_APP_OK";
        public const string STATE_APP_SOURCE_MODE_CHANGED = "STATE_APP_SOURCE_MODE_CHANGED";

        // data
        public const string DATA_CONTAINER_PRODUCT = "DC_ART";
        public const string DATA_CONTAINER_ALTERNATIVE = "DC_ALT";
        public const string DATA_CONTAINER_CLIENT = "DC_CLIENTS";
        public const string DATA_CONTAINER_CHEQUE = "DC_CHQ";

        public const string DATA_CONTAINER_PROP_ID = "0";
        public const string DATA_CONTAINER_PROP_MODE = "DC_MODE";
    }
}
