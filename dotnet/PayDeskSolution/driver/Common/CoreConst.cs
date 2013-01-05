using System;
using System.Collections.Generic;
using System.Text;

namespace driver.Common
{
    public class CoreConst
    {
        // order structure
        public const string CONTAINER_ORDER = "CN_ORDER";
        public const string ORDER_STORE_NO = "STORE_NO";
        public const string ORDER_CLIENT_ID = "CLIENT_ID";
        public const string ORDER_IS_RET = "IS_RET";
        public const string ORDER_IS_LEGAL = "IS_LEGAL";
        public const string ORDER_NO = "ORDER_NO";
        public const string ORDER_SUMA = "ORDER_SUMA";
        public const string ORDER_REAL_SUMA = "ORDER_REAL_SUMA";
        public const string ORDER_TAX_SUMA = "TAX_SUMA";
        public const string ORDER_TAX_BILL = "TAX_BILL";
        public const string ORDER_DISCOUNT = "DISCOUNT";
        public const string ORDER_PAYMENT = "PAYMENT";
        public const string ORDER_BILL = "BILL";

        // bill structure
        public const string CONTAINER_BILL = "CN_BILL";
        public const string BILL_OID = "OID";
        public const string BILL_NO = "BILL_NO";
        public const string BILL_OWNER_NO = "OWNER_NO";
        public const string BILL_DATETIME = "DATETIME";
        public const string BILL_DATETIMEEDIT = "DATETIMEEDIT";
        public const string BILL_COMMENT = "COMMENT";
        public const string BILL_IS_LOCKED = "IS_LOCKED";
        public const string BILL_PATH = "PATH";
        public const string BILL_DELETED_ROWS = "DELETED_ROWS";
        public const string BILL_DATETIME_LOCK = "DATETIME_LOCK";

        // discount structure
        public const string CONTAINER_DISC = "CN_DISC";
        public const string DISC_APPLIED = "DISC_APPLIED";
        public const string DISC_ALL_ITEMS = "DISC_ALL_ITEMS";
        public const string DISC_ARRAY_PERCENT_SUB = "DISC_ARRAY_PERCENT_SUB";
        public const string DISC_ARRAY_PERCENT_ADD = "DISC_ARRAY_PERCENT_ADD";
        public const string DISC_ARRAY_CASH_SUB = "DISC_ARRAY_CASH_SUB";
        public const string DISC_ARRAY_CASH_ADD = "DISC_ARRAY_CASH_ADD";
        public const string DISC_CONST_PERCENT = "DISC_CONST_PERCENT";
        public const string DISC_ONLY_PERCENT = "DISC_ONLY_PERCENT";
        public const string DISC_ONLY_CASH = "DISC_ONLY_CASH";
        public const string DISC_FINAL_PERCENT = "DISC_FINAL_PERCENT";
        public const string DISC_FINAL_CASH = "DISC_FINAL_CASH";

        // calculations
        public const string CONTAINER_CALC = "CN_CALC";
        public const string CASH_CHEQUE_SUMA = "CASH_CHEQUE_SUMA";
        public const string CASH_REAL_SUMA = "CASH_REAL_SUMA";
        public const string CASH_TAX_SUMA = "CASH_TAX_SUMA";

        // statements
        public const string CONTAINER_STATE = "CN_STATE";
        public const string STATE_DATA_UPDATED = "STATE_DATA_UPDATED";
        public const string STATE_DATA_UPDATE_PENDING = "STATE_DATA_UPDATE_PENDING";
        public const string STATE_DATA_UPDATE_ONLY = "STATE_DATA_UPDATE_ONLY";
        public const string STATE_ADMIN = "STATE_ADMIN";
        public const string STATE_MENU_IS_ACTIVE = "STATE_MENU_IS_ACTIVE";
        public const string STATE_APP_SUBUNIT_CHANGED = "STATE_APP_SUBUNIT_CHANGED";
        public const string STATE_CALC_USE_TOTAL_DISC = "STATE_CALC_USE_TOTAL_DISC";
        public const string STATE_APP_OK = "STATE_APP_OK";
        public const string STATE_APP_SOURCE_MODE_CHANGED = "STATE_APP_SOURCE_MODE_CHANGED";
        public const string STATE_LAN_ERROR = "LAN_ERROR";

        // source destination
        public const string SOURCE_REMOTE = "SRC_LAN";
        public const string SOURCE_LOCAL = "SRC_LOCAL";
        public const string SOURCE_TEMP = "SRC_TEMP";

        /**************************************************************/

        // data
        public const string DATA_CONTAINER_PRODUCT = "DC_ART";
        public const string DATA_CONTAINER_ALTERNATIVE = "DC_ALT";
        public const string DATA_CONTAINER_CLIENT = "DC_CLIENTS";
        public const string DATA_CONTAINER_CHEQUE = "DC_CHQ";

        // unused
        public const string DATA_CONTAINER_PROP_ID = "0";
        public const string DATA_CONTAINER_PROP_MODE = "DC_MODE";

        // common
        public const string KEY_DEFAULT_PROFILE_ID = "0";
        public const string KEY_DEFAULT = "DEFAULT";
        public const string CM_DELIM_DOWN = "_";

    }
}