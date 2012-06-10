using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
// project libs
using mdcore.Common;

namespace mdcore.Lib
{
    public static class DataWorkShared
    {/*
        private object[] CreatePrinterData(Dictionary<string, object> additional)
        {
            object[] printerData = new object[21];

            //Копія таблиці типу DataTable з записами товарів
            printerData[0] = Cheque.Copy();
            //Номер Чеку
            printerData[1] = 0;
            //Якщо true то цей чек є чеком поверення
            printerData[2] = retriveChq;
            //Якщо true то чек є фіскальний
            printerData[3] = type;
            //Сума товарів без знижки чи надбавки
            printerData[4] = chqSUMA;
            //Сума товарів з знижкою або надбавкою
            printerData[5] = realSUMA;
            //Тип закриття чеку (Готівка, чек, кредит, картка)
            printerData[6] = null;
            if (paymentInfo.Count != 0 && paymentInfo["TYPE"] != null)
                printerData[6] = (List<byte>)paymentInfo["TYPE"];
            //Загальна сума грошей покупця
            printerData[7] = null;
            if (paymentInfo.Count != 0 && paymentInfo["SUMA"] != null)
                printerData[7] = (double)paymentInfo["SUMA"];
            //Сума грошей з кожного типу оплати
            printerData[8] = null;
            if (paymentInfo.Count != 0 && paymentInfo["CASHLIST"] != null)
                printerData[7] = (List<double>)paymentInfo["CASHLIST"];
            //Здача
            printerData[9] = null;
            if (paymentInfo.Count != 0 && paymentInfo["REST"] != null)
                printerData[7] = (double)paymentInfo["REST"];
            //Якщо true то знижка чи надбавка діє на всі позиції(товари) чеку
            printerData[10] = _fl_useTotDisc;
            //Масив з значеннями знижки та надбавки в процентних значеннях
            printerData[11] = new double[2] { discArrPercent[0], discArrPercent[1] };
            //Масив з значеннями знижки та надбавки в грошових значеннях
            printerData[12] = new double[2] { discArrCash[0], discArrCash[1] };
            //Значення постійної знижки в процентному значенні
            printerData[13] = discConstPercent;
            //
            printerData[14] = null;
            //Сума знижки і надбавки з процентними значеннями
            printerData[15] = discOnlyPercent;
            //Сума знижки і надбавки з грошовими значеннями
            printerData[16] = discOnlyCash;
            //Загальний коефіціент знижки в процентному значенні
            printerData[17] = discCommonPercent;
            //Загальний коефіціент знижки в грошовому значенні
            printerData[18] = discCommonCash;
            //Якщо чек є закритий з рахунку
            if (Cheque.ExtendedProperties.Contains(CoreConst.BILL))
            {
                //Номер рахунку
                printerData[19] = Cheque.ExtendedProperties["NOM"];
                //Коментр рахунку
                printerData[20] = Cheque.ExtendedProperties["CMT"];
            }
            else
            {
                //Номер рахунку
                printerData[19] = string.Empty;
                //Коментр рахунку
                printerData[20] = string.Empty;
            }

            return printerData;
        }*/
        /*
        public static Dictionary<string, object> GetBillStructure(DataTable dtBill)
        {
            Dictionary<string, object> billInfoStructure = GetStandartChequeInfoStructure();
            billInfoStructure["DATA"] = dtBill;
            billInfoStructure["BILL_NO"] = dtBill.ExtendedProperties["NOM"];
            billInfoStructure["BILL_COMMENT"] = dtBill.ExtendedProperties["CMT"];
            return billInfoStructure;
        }
        */
        /* Property and Structure Actions */
        public static Dictionary<string, object> GetBillInfo(DataTable dtOrder)
        {
            Dictionary<string, object> billInfo = GetStandartBillInfoStructure();
            try
            {
                if (dtOrder.ExtendedProperties.ContainsKey(CoreConst.BILL))
                    billInfo = ((Dictionary<string, object>)dtOrder.ExtendedProperties[CoreConst.BILL]);
            }
            catch (Exception ex) { CoreLib.WriteLog(ex, "GetBillInfo(DataTable dtOrder); Unable to extract bill info."); }

            return billInfo;
        }
        public static Dictionary<string, object> GetOrderInfo(DataTable dtOrder)
        {
            return GetStandartOrderInfoStructure(dtOrder);
        }
        public static object ExtractBillProperty(DataTable dtBill, string propertyKey)
        {
            return ExtractBillProperty(dtBill, propertyKey, new object());
        }
        public static object ExtractBillProperty(DataTable dtBill, string propertyKey, object defaultValue)
        {
            return ExtractBillProperty(dtBill, propertyKey, defaultValue, false);
        }
        public static object ExtractBillProperty(DataTable dtBill, string propertyKey, object defaultValue, bool allowNull)
        {
            object propData = defaultValue;
            try
            {
                if (dtBill.ExtendedProperties.ContainsKey(CoreConst.BILL) && dtBill.ExtendedProperties[CoreConst.BILL] != null)
                {
                    Dictionary<string, object> bill = ((Dictionary<string, object>)dtBill.ExtendedProperties[CoreConst.BILL]);
                    if (bill.ContainsKey(propertyKey))
                    {
                        if ((bill[propertyKey] == null && allowNull) || bill[propertyKey] != null)
                            propData = bill[propertyKey];
                    }
                }
            }
            catch (Exception ex) { CoreLib.WriteLog(ex, "ExtractBillProperty(DataTable dtBill, string propertyKey, object defaultValue, bool allowNull); Unable to extract bill property."); }

            return propData;
        }
        public static object ExtractOrderProperty(DataTable dtBill, string propertyKey)
        {
            return ExtractOrderProperty(dtBill, propertyKey, new object());
        }
        public static object ExtractOrderProperty(DataTable dtBill, string propertyKey, object defaultValue)
        {
            return ExtractOrderProperty(dtBill, propertyKey, defaultValue, false);
        }
        public static object ExtractOrderProperty(DataTable dtBill, string propertyKey, object defaultValue, bool allowNull)
        {
            object propData = defaultValue;
            try
            {
                if (dtBill.ExtendedProperties.ContainsKey(propertyKey))
                    propData = dtBill.ExtendedProperties[propertyKey];
                if (propData == null && !allowNull)
                    propData = defaultValue;
            }
            catch (Exception ex) { CoreLib.WriteLog(ex, "ExtractOrderProperty(DataTable dtBill, string propertyKey, object defaultValue, bool allowNull); Unable to extract order property."); }

            return propData;
        }
        public static bool SetBillProperty(DataTable dtBill, string propertyKey, object value)
        {
            bool fnRez = false;
            try
            {
                if (dtBill.ExtendedProperties.ContainsKey(CoreConst.BILL) && dtBill.ExtendedProperties[CoreConst.BILL] != null)
                {
                    Dictionary<string, object> bill = ((Dictionary<string, object>)dtBill.ExtendedProperties[CoreConst.BILL]);
                    if (bill.ContainsKey(propertyKey))
                    {
                        fnRez = true;
                        bill[propertyKey] = value;
                    }
                }
            }
            catch (Exception ex) { CoreLib.WriteLog(ex, "SetBillProperty(DataTable dtBill, string propertyKey, object value); Unable to set bill property."); }

            return fnRez;
        }
        public static bool SetOrderProperty(DataTable dtOrder, string propertyKey, object value)
        {
            bool fnRez = false;
            try
            {
                if (dtOrder.ExtendedProperties.ContainsKey(propertyKey))
                {
                    fnRez = true;
                    dtOrder.ExtendedProperties[propertyKey] = value;
                }
            }
            catch (Exception ex) { CoreLib.WriteLog(ex, "SetOrderProperty(DataTable dtBill, string propertyKey, object value); Unable to set order property."); }

            return fnRez;
        }
        public static bool ResetBillProperty(DataTable dtBill, string propertyKey)
        {
            bool fnRez = false;
            Dictionary<string, object> billInfo = GetStandartBillInfoStructure();
            try
            {
                if (billInfo.ContainsKey(propertyKey))
                    fnRez = SetBillProperty(dtBill, propertyKey, billInfo[propertyKey]);
            }
            catch (Exception e) { CoreLib.WriteLog(e, "ResetBillProperty(); Unable to reset bill property [" + propertyKey + "]."); }

            return fnRez;
        }
        public static bool ResetOrderProperty(DataTable dtOrder, string propertyKey)
        {
            bool fnRez = false;
            Dictionary<string, object> orderInfo = GetStandartOrderInfoStructure();
            try
            {
                if (orderInfo.ContainsKey(propertyKey))
                    fnRez = SetOrderProperty(dtOrder, propertyKey, orderInfo[propertyKey]);
            }
            catch (Exception e) { CoreLib.WriteLog(e, "ResetOrderProperty(); Unable to extract bill property [" + propertyKey + "]."); }

            return fnRez;
        }

        /* Object Structure */
        public static Dictionary<string, object> GetStandartBillInfoStructure()
        {/*
            dTable.ExtendedProperties.Add("NOM", nom);
            dTable.ExtendedProperties.Add("DT", DateTime.Now.ToShortDateString());
            dTable.ExtendedProperties.Add("CMT", comment);
            dTable.ExtendedProperties.Add("LOCK", false);
            dTable.ExtendedProperties.Add("FXNO", string.Empty);
            //dTable.ExtendedProperties.Add("DISC", new object[2][]);
            dTable.ExtendedProperties.Add("PATH", path);
            dTable.ExtendedProperties.Add(CoreConst.BILL, true);
            dTable.ExtendedProperties.Add("CHEQUE", extendedProps);*/
            Dictionary<string, object> billInfoStructure = new Dictionary<string, object>();
            // DataTable source
            billInfoStructure.Add(CoreConst.OID, null);
            // DataTable source
            billInfoStructure.Add(CoreConst.BILL_NO, null);
            // String - owner number (for clonned bill)
            billInfoStructure.Add(CoreConst.OWNER_NO, null);
            // Storu Number
            billInfoStructure.Add(CoreConst.DATETIME, null);
            // Storu Number
            billInfoStructure.Add(CoreConst.DATETIMEEDIT, null);
            // Default Client ID
            billInfoStructure.Add(CoreConst.COMMENT, null);
            // Detect if cheque is retiermant
            billInfoStructure.Add(CoreConst.IS_LOCKED, null);
            // Determinate that cheque is legal
            billInfoStructure.Add(CoreConst.PATH, null);
            // Determinate that cheque is legal
            billInfoStructure.Add(CoreConst.DELETED_ROWS, new Dictionary<string, object[]>());
            // Cheque Number
            //chequeInfoStructure.Add("CHEQUE_NO", null);
            // Cheque Suma with all discounts
            //chequeInfoStructure.Add("CHEQUE_SUMA", null);
            // Cheque real suma (without discount)
            //chequeInfoStructure.Add("CHEQUE_REAL_SUMA", null);
            // Cheque's tax suma
            //chequeInfoStructure.Add("TAX_SUMA", null);
            // Determinate if this cheque need tax bill
            //chequeInfoStructure.Add("TAX_BILL", null);
            // Discount Structure
            //chequeInfoStructure.Add("DISCOUNT", null);
            // Payment Structure
            //GetStandartBillInfoStructurchequeInfoStructure.Add("PAYMENT", null);
            // Bil number. If cheque is a bill.
            //chequeInfoStructure.Add("BILL_NO", null);
            // ad comment for this bill
            //chequeInfoStructure.Add("BILL_COMMENT", null);
            //Dictionary<string, Dictionary<string, object>> billInfoStructureWrapper = new Dictionary<string, Dictionary<string, object>>();
            //billInfoStructureWrapper.Add(CoreConst.BILL, billInfoStructure);
            //return billInfoStructureWrapper;
            return billInfoStructure;
        }
        public static Dictionary<string, object> GetStandartOrderInfoStructure()
        {
            Dictionary<string, object> chequeInfoStructure = new Dictionary<string, object>();
            // DataTable source
            //chequeInfoStructure.Add("DATA", null);
            // Storu Number
            chequeInfoStructure.Add(CoreConst.STORE_NO, null);
            // Default Client ID
            chequeInfoStructure.Add(CoreConst.CLIENT_ID, null);
            // Detect if cheque is retiermant
            chequeInfoStructure.Add(CoreConst.IS_RET, null);
            // Determinate that cheque is legal
            chequeInfoStructure.Add(CoreConst.IS_LEGAL, null);
            // Cheque Number
            chequeInfoStructure.Add(CoreConst.ORDER_NO, null);
            // Cheque Suma with all discounts
            chequeInfoStructure.Add(CoreConst.ORDER_SUMA, null);
            // Cheque real suma (without discount)
            chequeInfoStructure.Add(CoreConst.ORDER_REAL_SUMA, null);
            // Cheque's tax suma
            chequeInfoStructure.Add(CoreConst.TAX_SUMA, null);
            // Determinate if this cheque need tax bill
            chequeInfoStructure.Add(CoreConst.TAX_BILL, null);
            // Discount Structure
            chequeInfoStructure.Add(CoreConst.DISCOUNT, null);
            // Payment Structure
            chequeInfoStructure.Add(CoreConst.PAYMENT, null);
            // Payment Structure
            chequeInfoStructure.Add(CoreConst.BILL, null);
            // Bil number. If cheque is a bill.
            //chequeInfoStructure.Add("BILL_NO", null);
            // ad comment for this bill
            //chequeInfoStructure.Add("BILL_COMMENT", null);
            //Dictionary<string, Dictionary<string, object>> chequeInfoStructureWrapper = new Dictionary<string, Dictionary<string, object>>();
            //chequeInfoStructureWrapper.Add("ORDER", chequeInfoStructure);
            //return chequeInfoStructureWrapper;
            return chequeInfoStructure;
        }
        public static Dictionary<string, object> GetStandartOrderInfoStructure(DataTable dtOrder)
        {
            Dictionary<string, object> structure = GetStandartOrderInfoStructure();
            Dictionary<string, object> retStruct = GetStandartOrderInfoStructure();
            foreach (KeyValuePair<string, object> prop in structure)
            {
                if (dtOrder.ExtendedProperties.ContainsKey(prop.Key))
                    retStruct[prop.Key] = dtOrder.ExtendedProperties[prop.Key];
            }
            return retStruct;
        }
        public static Dictionary<string, object> GetStandartDiscountInfoStructure()
        {
            Dictionary<string, object> discountInfoStructure = new Dictionary<string, object>();
            //Якщо true то знижка чи надбавка діє на всі позиції(товари) чеку
            discountInfoStructure.Add(CoreConst.DISC_ALL_ITEMS, null);
            //Масив з значеннями знижки та надбавки в процентних значеннях
            discountInfoStructure.Add(CoreConst.DISC_ARRAY_PERCENT, null);
            //Масив з значеннями знижки та надбавки в грошових значеннях
            discountInfoStructure.Add(CoreConst.DISC_ARRAY_CASH, null);
            //Значення постійної знижки в процентному значенні
            discountInfoStructure.Add(CoreConst.DISC_CONST_PERCENT, null);
            //Сума знижки і надбавки з процентними значеннями
            discountInfoStructure.Add(CoreConst.DISC_ONLY_PERCENT, null);
            //Сума знижки і надбавки з грошовими значеннями
            discountInfoStructure.Add(CoreConst.DISC_ONLY_CASH, null);
            //Загальний коефіціент знижки в процентному значенні
            discountInfoStructure.Add(CoreConst.DISC_FINAL_PERCENT, null);
            //Загальний коефіціент знижки в грошовому значенні
            discountInfoStructure.Add(CoreConst.DISC_FINAL_CASH, null);
            return discountInfoStructure;
        }

        /* Property Manager */
        public static void UpdateExtendedProperties(DataTable dtObj, Dictionary<string, object> props)
        {
            UpdateExtendedProperties(dtObj, props, true);
        }
        public static void UpdateExtendedProperties(DataTable dtObj, Dictionary<string, object> props, bool addNew)
        {
            foreach (KeyValuePair<string, object> item in props)
            {
                if (dtObj.ExtendedProperties.ContainsKey(item.Key))
                    dtObj.ExtendedProperties[item.Key] = item.Value;
                else if (addNew)
                    dtObj.ExtendedProperties.Add(item.Key, item.Value);
            }
        }
        public static void UpdateExtendedProperties(DataTable dtObj, PropertyCollection props)
        {
            UpdateExtendedProperties(dtObj, props, false);
        }
        public static void UpdateExtendedProperties(DataTable dtObj, PropertyCollection props, bool addNew)
        {
            foreach (DictionaryEntry item in props)
            {
                if (dtObj.ExtendedProperties.ContainsKey(item.Key))
                    dtObj.ExtendedProperties[item.Key] = item.Value;
                else if (addNew)
                    dtObj.ExtendedProperties.Add(item.Key, item.Value);
            }
        }
        public static void AppendExtendedProperties(DataTable dtObj, Dictionary<string, object> props)
        {
            AppendExtendedProperties(dtObj, props, false);
        }
        public static void AppendExtendedProperties(DataTable dtObj, Dictionary<string, object> props, bool withClear)
        {
            if (withClear)
                dtObj.ExtendedProperties.Clear();
            foreach (KeyValuePair<string, object> item in props)
            {
                dtObj.ExtendedProperties.Add(item.Key, item.Value);
            }
        }
        public static void AppendExtendedProperties(DataTable dtObj, PropertyCollection props)
        {
            AppendExtendedProperties(dtObj, props, false);
        }
        public static void AppendExtendedProperties(DataTable dtObj, PropertyCollection props, bool withClear)
        {
            if (withClear)
                dtObj.ExtendedProperties.Clear();
            try
            {
                foreach (DictionaryEntry item in props)
                {
                    dtObj.ExtendedProperties.Add(item.Key, item.Value);
                }
            }
            catch { }
        }
        public static void MergeDataTableProperties(ref DataTable dtObjOlder, DataTable dtObjNewer)
        {
            foreach (DictionaryEntry prop in dtObjNewer.ExtendedProperties)
            {
                if (!dtObjOlder.ExtendedProperties.ContainsKey(prop.Key))
                    dtObjOlder.ExtendedProperties.Add(prop.Key, prop.Value);
                else
                {
                    dtObjOlder.ExtendedProperties[prop.Key] = prop.Value;
                }

            }
        }

        /* Other */
        public static string DumpDataTableRow(DataRow dRow)
        {
            string rowInfo = string.Empty;

            for (int i = 0; i < dRow.ItemArray.Length; i++)
                rowInfo += string.Format("cell index [{0}] = {1}", i, dRow[i]);

            return rowInfo;
        }

        /* Data Storages */
        public static object[] GetDataObject(DataTable dtObj)
        {
            return new object[2] { dtObj, dtObj.ExtendedProperties };
        }
        public static DataTable CombineDataObject(object[] dataSource)
        {
            DataTable dtObj = new DataTable();
            try
            {
                dtObj = (DataTable)dataSource[0];
                PropertyCollection props = (PropertyCollection)dataSource[1];
                AppendExtendedProperties(dtObj, props, true);
            }
            catch(Exception ex) { }


            return dtObj;
        }

    }
}
