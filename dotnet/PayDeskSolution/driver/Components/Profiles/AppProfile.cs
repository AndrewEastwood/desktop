using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using driver.Common;
using driver.Lib;
using driver.Config;

namespace driver.Components.Profiles
{
    public class AppProfile
    {
        // container link
        private ProfilesContainer parent;

        // profile data
        private string id;
        private string name;
        private DataTable saleItems;
        private DataTable products;
        private DataTable alternateBarcodes;
        private DataTable customerProgramCards;

        // Order Data
        private Hashtable cashInfo;
        private Hashtable props;

        // triggers
        private bool trExchangeAccessError;

        public AppProfile(string id, string name)
        {
            this.id = id;
            this.name = name;

            saleItems = new DataTable();
            products = new DataTable();
            alternateBarcodes = new DataTable();
            customerProgramCards = new DataTable();

            cashInfo = new Hashtable();
            props = new Hashtable();

            this.setup();
        }

        /* setup */

        private void setup()
        {
            initCashStructure(Cash);
            initOrderStructure(Order);
        }

        /* properties */

        public string Name { get { return this.name; } }
        public string ID { get { return this.id; } }
        public ProfilesContainer Parent { set { parent = value; } get { return parent; } }
        public DataTable Order { get { return getOrder(); } }
        public DataTable Products { get { return getProducts(); } }
        public Hashtable Cash { get { return this.cashInfo; } set { this.cashInfo = value; } }

        /*  */

        public void calcCash()
        {
            //*bool useConstDisc = discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 &&
            //*    discArrCash[0] == 0.0 && discArrCash[1] == 0.0;
            double[] _discArrP = getCashValue<double[]>(CoreConst.DISC_ARRAY_PERCENT);
            double[] _discArrC = getCashValue<double[]>(CoreConst.DISC_ARRAY_CASH);
            bool useConstDisc = _discArrP[0] == 0 && _discArrP[1] == 0 && _discArrC[0] == 0 && _discArrC[1] == 0;


            // Get discount value
            if (useConstDisc)
            {
                // * discConstPercent = 0.0;
                Cash[CoreConst.DISC_CONST_PERCENT] = 0.0;

                //form "sum" by static discount
                // **** if (ConfigManager.Instance.CommonConfiguration.APP_UseStaticDiscount)
                // ****    discConstPercent = ConfigManager.Instance.CommonConfiguration.APP_StaticDiscountValue;
                //form "sum" by dynamic discount
                // **** if (ConfigManager.Instance.CommonConfiguration.APP_UseStaticRules)
                // ****     discConstPercent = CoreLib.UpdateSumbyRules(Cheque);
            }
            else
            {
                // * discOnlyPercent = discArrPercent[0] + discArrPercent[1];
                // * discOnlyCash = discArrCash[0] + discArrCash[1];
                // * discOnlyPercent = MathLib.GetRoundedMoney(discOnlyPercent);
                // * discOnlyCash = MathLib.GetRoundedMoney(discOnlyCash);

                Cash[CoreConst.DISC_ONLY_PERCENT] = MathLib.GetRoundedMoney(_discArrP[0] + _discArrP[1]);
                Cash[CoreConst.DISC_ONLY_CASH] = MathLib.GetRoundedMoney(_discArrC[0] + _discArrC[1]);
            }

            //if (Cheque.Rows.Count == 0)
            if (saleItems.Rows.Count == 0)
            {
                //realSUMA = chqSUMA = taxSUMA = 0.0;
                Cash[CoreConst.CALC_REAL_SUMA] = 0.0;
                Cash[CoreConst.CALC_CHEQUE_SUMA] = 0.0;
                Cash[CoreConst.CALC_TAX_SUMA] = 0.0;

                 
                // ???? UpdateSumDisplay(false, updateCustomer);
                // this.PD_EmptyOrder;
                return;
            }

            int i = 0;
            double dValue = 0.0;
            double taxValue = 0.0;
            double artSum = 0.0;
            //int index = 0;
            DataRow[] dRows = null;
            double discSUMA = 0.0;


            //procentnuj zagalnuj koeficient
            if (useConstDisc)
            {
                // * discCommonPercent = discConstPercent;
                Cash[CoreConst.DISC_FINAL_PERCENT] = Cash[CoreConst.DISC_CONST_PERCENT];
            }
            else
            {
                //discCommonPercent = discOnlyPercent;
                Cash[CoreConst.DISC_FINAL_PERCENT] = Cash[CoreConst.DISC_ONLY_PERCENT];
            }
            if (discSUMA != 0.0)
            {
                // * discCommonPercent += (discOnlyCash * 100) / discSUMA;
                Cash[CoreConst.DISC_FINAL_PERCENT] = getCashValue<double>(CoreConst.DISC_FINAL_PERCENT) + (getCashValue<double>(CoreConst.DISC_ONLY_CASH) * 100) / discSUMA;
            }
            // * discCommonPercent = MathLib.GetRoundedMoney(discCommonPercent);
            Cash[CoreConst.DISC_FINAL_PERCENT] = MathLib.GetRoundedMoney(getCashValue<double>(CoreConst.DISC_FINAL_PERCENT));

            // restore native cheque sum
            // and set price acording to client's discount card
            //DataRow[] artRecord = null;
            double newPrice = 0.0;
            double newTmpPrice = 0.0; //bool isSet = false;
            Hashtable profileDefinedTaxGrid = new Hashtable();
            Hashtable profileCompatibleTaxGrid = new Hashtable();
            for (i = 0; i < saleItems.Rows.Count; i++)
            {
                newPrice = MathLib.GetDouble(saleItems.Rows[i]["ORIGPRICE"]);

                //isSet = false;
                if (parent.valueOfClientPriceNo != 0)
                {
                    newTmpPrice = MathLib.GetDouble(saleItems.Rows[i]["PR" + this.parent.valueOfClientPriceNo].ToString());
                    if (newTmpPrice != 0.0) newPrice = newTmpPrice;
                }
                else if (UserConfig.Properties[8])
                {
                    //DataRow dRow = Cheque.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                    //price = AppFunc.AutomaticPrice(thisTot, dRow);
                    double _newPrice = CoreLib.AutomaticPrice(MathLib.GetDouble(saleItems.Rows[i]["TOT"].ToString()), saleItems.Rows[i]);
                    try
                    {
                        profileDefinedTaxGrid = (Hashtable)driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates[saleItems.Rows[i]["F"]];
                    }
                    catch { }
                    // new tax mode
                    bool _thisRowCanUseDiscount = true;
                    try
                    {
                        // get application tax char with compatible tax grid
                        string[] definedTaxData = profileDefinedTaxGrid[saleItems.Rows[i]["VG"].ToString()].ToString().Split(';');
                        _thisRowCanUseDiscount = Boolean.Parse(definedTaxData[1]);
                    }
                    catch { }

                    if (_thisRowCanUseDiscount)
                    {

                        double _discountPrices = 0.0;
                        //for (int ii = 0; ii < Cheque.Rows.Count; ii++)
                        //{
                        if (_newPrice != (double)saleItems.Rows[i]["ORIGPRICE"])
                        {
                            _discountPrices = 100 - _newPrice * 100 / (double)saleItems.Rows[i]["ORIGPRICE"];
                            // * if (_discountPrices > discCommonPercent)
                            if (_discountPrices > getCashValue<double>(CoreConst.DISC_FINAL_PERCENT))
                            {
                                saleItems.Rows[i]["USEDDISC"] = Boolean.FalseString;
                                newPrice = _newPrice;
                            }
                            else
                            {
                                saleItems.Rows[i]["USEDDISC"] = Boolean.TrueString;
                                newPrice = MathLib.GetDouble(saleItems.Rows[i]["ORIGPRICE"]);
                            }
                        }
                        else newPrice = _newPrice;
                        //}
                    }
                    else newPrice = _newPrice;
                }
                else if (UserConfig.Properties[1] || UserConfig.Properties[2])
                {
                    newPrice = MathLib.GetDouble(saleItems.Rows[i]["PRICE"]);
                }
                saleItems.Rows[i]["PRICE"] = newPrice;
                saleItems.Rows[i]["ASUM"] = saleItems.Rows[i]["SUM"] = MathLib.GetRoundedMoney(MathLib.GetDouble(saleItems.Rows[i]["TOT"].ToString()) * newPrice);
                saleItems.Rows[i]["DISC"] = 0.0;
            }
            // * chqSUMA = (double)saleItems.Compute("sum(SUM)", "");
            // * chqSUMA = MathLib.GetRoundedMoney(chqSUMA);
            // * realSUMA = chqSUMA;
            Cash[CoreConst.CALC_CHEQUE_SUMA] = MathLib.GetRoundedMoney((double)saleItems.Compute("sum(SUM)", ""));
            Cash[CoreConst.CALC_REAL_SUMA] = Cash[CoreConst.CALC_CHEQUE_SUMA];

            //select rows with discount mode
            try
            {
                dRows = saleItems.Select("USEDDISC = " + Boolean.TrueString);
                // * _fl_useTotDisc = (dRows.Length == saleItems.Rows.Count);
                parent.triggerUseTotDisc = (dRows.Length == saleItems.Rows.Count);
                
                //discSUMA = (double)Cheque.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                object d = saleItems.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                if (d != DBNull.Value)
                    double.TryParse(d.ToString(), out discSUMA);
                if (dRows.Length == 0)
                {
                    // * this.discApplied = false;
                    Cash[CoreConst.DISC_APPLIED] = false;
                }
                else
                    Cash[CoreConst.DISC_APPLIED] = true; // * this.discApplied = true;
            }
            catch { };


            DataRow[] prRows = null;
            // * if (this.clientPriceNo != 0)
            if (parent.valueOfClientPriceNo!= 0)
                prRows = saleItems.Select("PR" + parent.valueOfClientPriceNo + " <> 0");


            // * if (_fl_useTotDisc && prRows == null)
            if (Parent.triggerUseTotDisc && prRows == null)
            {
                //obrahunok realnoi sumu cheku zi znugkojy
                if (useConstDisc)
                {
                    // * dValue = (discConstPercent * discSUMA) / 100;
                    // * dValue = MathLib.GetRoundedMoney(dValue);
                    // * realSUMA -= dValue;
                    dValue = MathLib.GetRoundedMoney((getCashValue<double>(CoreConst.DISC_CONST_PERCENT) * discSUMA) / 100);
                }
                else
                {
                    // * dValue = (discOnlyPercent * discSUMA) / 100;
                    // * dValue = MathLib.GetRoundedMoney(dValue);
                    // * realSUMA -= dValue;
                    // * realSUMA -= discOnlyCash;

                    dValue = MathLib.GetRoundedMoney((getCashValue<double>(CoreConst.DISC_CONST_PERCENT) * discSUMA) / 100);
                    Cash[CoreConst.CALC_REAL_SUMA] = getCashValue<double>(CoreConst.CALC_REAL_SUMA) - dValue - getCashValue<double>(CoreConst.DISC_ONLY_CASH);
                }
            }
            else
            {
                // * _fl_useTotDisc = false;
                Parent.triggerUseTotDisc = false;
                for (i = 0; i < dRows.Length; i++)
                {
                    // don't use discount when client want to has another price for current article
                    // * if (this.clientPriceNo != 0 && MathLib.GetDouble(dRows[i]["PR" + this.clientPriceNo].ToString()) > 0.0)
                    if (Parent.valueOfClientPriceNo != 0 && MathLib.GetDouble(dRows[i]["PR" + Parent.valueOfClientPriceNo].ToString()) > 0.0)
                    {
                        dRows[i]["DISC"] = 0.0;
                        continue;
                    }
                    dRows[i]["DISC"] = getCashValue<double>(CoreConst.DISC_FINAL_PERCENT);
                    dValue = (getCashValue<double>(CoreConst.DISC_FINAL_PERCENT) * (double)dRows[i]["SUM"]) / 100;
                    dValue = (double)dRows[i]["SUM"] - dValue;
                    dRows[i]["ASUM"] = MathLib.GetRoundedMoney(dValue);
                }
                // * realSUMA = (double)saleItems.Compute("Sum(ASUM)", "");
                Cash[CoreConst.CALC_REAL_SUMA] = (double)saleItems.Compute("Sum(ASUM)", "");
            }
            Cash[CoreConst.CALC_REAL_SUMA] = MathLib.GetRoundedMoney(getCashValue<double>(CoreConst.CALC_REAL_SUMA));

            //vuvedennja zagalnogo koeficientu znugku v 2oh tupah
            //groshovuj koeficient
            // * discCommonCash = chqSUMA - realSUMA;
            // * discCommonCash = MathLib.GetRoundedMoney(discCommonCash);
            Cash[CoreConst.DISC_ONLY_CASH] = MathLib.GetRoundedMoney(getCashValue<double>(CoreConst.CALC_CHEQUE_SUMA) - getCashValue<double>(CoreConst.DISC_FINAL_CASH));

            // calculating tax sum
            // * taxSUMA = 0.0;
            Cash[CoreConst.CALC_TAX_SUMA] = 0.0;

            for (i = 0; i < saleItems.Rows.Count; i++)
            {
                try
                {
                    taxValue = MathLib.GetDouble(saleItems.Rows[i]["TAX_VAL"]);
                    if (Boolean.Parse(saleItems.Rows[i]["USEDDISC"].ToString()))
                    {
                        // * artSum = (discCommonPercent * (double)saleItems.Rows[i]["SUM"]) / 100;
                        artSum = (getCashValue<double>(CoreConst.DISC_FINAL_PERCENT) * (double)saleItems.Rows[i]["SUM"]) / 100;
                        artSum = (double)saleItems.Rows[i]["SUM"] - artSum;
                        artSum = MathLib.GetRoundedMoney(artSum);
                        taxValue = (artSum * taxValue) / (taxValue + 100);
                    }
                    else
                        taxValue = (((double)saleItems.Rows[i]["ASUM"]) * taxValue) / (taxValue + 100);
                }
                catch
                {
                    taxValue = 0;
                }

                saleItems.Rows[i]["TAX_MONEY"] = taxValue;
                // * taxSUMA += taxValue;
                Cash[CoreConst.CALC_TAX_SUMA] = getCashValue<double>(CoreConst.CALC_TAX_SUMA) + taxValue;
            }

            // * taxSUMA = MathLib.GetRoundedMoney(taxSUMA);
            Cash[CoreConst.CALC_TAX_SUMA] = MathLib.GetRoundedMoney(getCashValue<double>(CoreConst.CALC_TAX_SUMA));

            // * if (!_fl_isInvenCheque)
            // *     UpdateSumDisplay(true, updateCustomer);
            // (i) called within parent event

        }

        /* data access and management */
        public DataTable getProducts()
        {
            return new DataTable();
        }
        public DataTable getOrder()
        {
            return new DataTable();
        }
        public DataTable getOrder(bool reinit)
        {
            // * Hashtable _suma = (Hashtable)this.Summa[profileKey];
            // * Hashtable _discount = (Hashtable)this.Discount[profileKey];
            // * DataTable _cheque = this.Cheques.Tables[profileKey.ToString()];

            if (reinit)
                initOrderStructure(saleItems);

            /* initializing discount values * /
            bool _discApplied = CoreLib.GetValue<bool>(_discount, CoreConst.DISC_APPLIED);
            double[] _discArrPercent = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_PERCENT);
            double[] _discArrCash = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_CASH);
            double _discConstPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_CONST_PERCENT);
            double _discOnlyPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_PERCENT);
            double _discOnlyCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_CASH);
            double _discCommonPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_PERCENT);
            double _discCommonCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_CASH);
            / * calculation items * /
            double _realSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_REAL_SUMA);
            double _chqSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_CHEQUE_SUMA);
            double _taxSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_TAX_SUMA);
            */
            /*discInfo["DISC_ALL_ITEMS"] = this._fl_useTotDisc;
            //Масив з значеннями знижки та надбавки в процентних значеннях
            discInfo["DISC_ARRAY_PERCENT"] = new double[2] { _discArrPercent[0], _discArrPercent[1] };
            //Масив з значеннями знижки та надбавки в грошових значеннях
            discInfo["DISC_ARRAY_CASH"] = new double[2] { _discArrCash[0], _discArrCash[1] };
            //Значення постійної знижки в процентному значенні
            discInfo["DISC_CONST_PERCENT"] = _discConstPercent;
            //Сума знижки і надбавки з процентними значеннями
            discInfo["DISC_ONLY_PERCENT"] = _discOnlyPercent;
            //Сума знижки і надбавки з грошовими значеннями
            discInfo["DISC_ONLY_CASH"] = _discOnlyCash;
            //Загальний коефіціент знижки в процентному значенні
            discInfo["DISC_FINAL_PERCENT"] = _discCommonPercent;
            //Загальний коефіціент знижки в грошовому значенні
            discInfo["DISC_FINAL_CASH"] = _discCommonCash;
            discInfo["DISC_APPLIED"] = _discApplied;

            chqInfo["STORE_NO"] = this.currentSubUnit;
            chqInfo["CLIENT_ID"] = this.clientID;
            chqInfo["IS_RET"] = this._fl_isReturnCheque;
            chqInfo["IS_LEGAL"] = false;
            chqInfo["ORDER_SUMA"] = _chqSUMA;
            chqInfo["ORDER_REAL_SUMA"] = _realSUMA;
            chqInfo["TAX_SUMA"] = _realSUMA;
            chqInfo["TAX_BILL"] = this._fl_taxDocRequired;
            chqInfo["DISCOUNT"] = discInfo;
            */

            // * DataWorkShared.UpdateExtendedProperties(Order, chqInfo);

            saleItems.ExtendedProperties[CoreConst.STORE_NO] = Parent.valueOfCurrentSubUnit;
            saleItems.ExtendedProperties[CoreConst.CLIENT_ID] = Parent.valueOfClientID;
            saleItems.ExtendedProperties[CoreConst.IS_RET] = Parent.triggerReturnCheque;
            saleItems.ExtendedProperties[CoreConst.IS_LEGAL] = false;
            saleItems.ExtendedProperties[CoreConst.ORDER_SUMA] = Parent;
            saleItems.ExtendedProperties[CoreConst.ORDER_REAL_SUMA] = Parent;
            saleItems.ExtendedProperties[CoreConst.TAX_SUMA] = Parent;
            saleItems.ExtendedProperties[CoreConst.TAX_BILL] = Parent.triggerTaxDocRequired;
            saleItems.ExtendedProperties[CoreConst.DISCOUNT] = (Hashtable)Cash.Clone();

            return saleItems;
        }

        public void resetCash() { }
        public void resetDisc() { }
        public void resetAll() { }

        public T getCashValue<T>(string key)
        {
            if (!Cash.ContainsKey(key))
                return default(T);

            object val = Cash[key];

            try
            {
                return components.Lib.TypeConv.ConvertTo<T>(val);
            }
            catch (Exception ex) { CoreLib.WriteLog(ex, "driver.Components.Profiles.AppProfile@getCashValue"); }

            return default(T);
        }
        // used
        // * public void UpdateDiscountValues(DataTable order)
        public void initProfileByOrder(DataTable order)
        {/*
            this.currentSubUnit = (byte)order.ExtendedProperties[CoreConst.STORE_NO];
            try
            {
                this.clientID = order.ExtendedProperties[CoreConst.CLIENT_ID].ToString();
            }
            catch { }
            this._fl_isReturnCheque = (bool)order.ExtendedProperties[CoreConst.IS_RET];

            if (order.ExtendedProperties.ContainsKey(CoreConst.DISCOUNT))
            {

                Hashtable discount = (Hashtable)order.ExtendedProperties[CoreConst.DISCOUNT];

                try
                {
                    this._fl_useTotDisc = (bool)discount[CoreConst.DISC_ALL_ITEMS];
                    this.discArrPercent = (double[])discount[CoreConst.DISC_ARRAY_PERCENT];
                    this.discArrCash = (double[])discount[CoreConst.DISC_ARRAY_CASH];
                    this.discConstPercent = (double)discount[CoreConst.DISC_CONST_PERCENT];
                    this.discOnlyCash = (double)discount[CoreConst.DISC_ONLY_CASH];
                    this.discOnlyPercent = (double)discount[CoreConst.DISC_ONLY_PERCENT];
                    this.discCommonPercent = (double)discount[CoreConst.DISC_FINAL_PERCENT];
                    this.discCommonCash = (double)discount[CoreConst.DISC_FINAL_CASH];
                    this.discApplied = (bool)discount[CoreConst.DISC_APPLIED];
                }
                catch { }
                /*
                chqInfo["DISC_ALL_ITEMS"] = this._fl_useTotDisc;
                //Масив з значеннями знижки та надбавки в процентних значеннях
                chqInfo["DISC_ARRAY_PERCENT"] = new double[2] { discArrPercent[0], discArrPercent[1] };
                //Масив з значеннями знижки та надбавки в грошових значеннях
                chqInfo["DISC_ARRAY_CASH"] = new double[2] { discArrCash[0], discArrCash[1] };
                //Значення постійної знижки в процентному значенні
                chqInfo["DISC_CONST_PERCENT"] = this.discConstPercent;
                //Сума знижки і надбавки з процентними значеннями
                chqInfo["DISC_ONLY_PERCENT"] = this.discOnlyPercent;
                //Сума знижки і надбавки з грошовими значеннями
                chqInfo["DISC_ONLY_CASH"] = this.discOnlyCash;
                //Загальний коефіціент знижки в процентному значенні
                chqInfo["DISC_FINAL_PERCENT"] = this.discCommonPercent;
                //Загальний коефіціент знижки в грошовому значенні
                chqInfo["DISC_FINAL_CASH"] = this.discCommonCash;
                 * /
            }*/
        }
        // * private void CreateOrderStructure(DataTable dtOrder)
        public void initOrderStructure(DataTable dtOrder)
        {
            // * Dictionary<string, object> chqInfo = DataWorkShared.GetStandartOrderInfoStructure();
            // * DataWorkShared.AppendExtendedProperties(dtOrder, chqInfo, true);

            dtOrder.ExtendedProperties.Clear();

            dtOrder.ExtendedProperties.Add(CoreConst.STORE_NO, null);
            // Default Client ID
            dtOrder.ExtendedProperties.Add(CoreConst.CLIENT_ID, null);
            // Detect if cheque is retiermant
            dtOrder.ExtendedProperties.Add(CoreConst.IS_RET, null);
            // Determinate that cheque is legal
            dtOrder.ExtendedProperties.Add(CoreConst.IS_LEGAL, null);
            // Cheque Number
            dtOrder.ExtendedProperties.Add(CoreConst.ORDER_NO, null);
            // Cheque Suma with all discounts
            dtOrder.ExtendedProperties.Add(CoreConst.ORDER_SUMA, null);
            // Cheque real suma (without discount)
            dtOrder.ExtendedProperties.Add(CoreConst.ORDER_REAL_SUMA, null);
            // Cheque's tax suma
            dtOrder.ExtendedProperties.Add(CoreConst.TAX_SUMA, null);
            // Determinate if this cheque need tax bill
            dtOrder.ExtendedProperties.Add(CoreConst.TAX_BILL, null);
            // Discount Structure
            dtOrder.ExtendedProperties.Add(CoreConst.DISCOUNT, null);
            // Payment Structure
            dtOrder.ExtendedProperties.Add(CoreConst.PAYMENT, null);
            // Payment Structure
            dtOrder.ExtendedProperties.Add(CoreConst.BILL, null);

        }
        public void initCashStructure(Hashtable cashStructure)
        {
            cashStructure.Add(CoreConst.CALC_CHEQUE_SUMA, 0.0);
            cashStructure.Add(CoreConst.CALC_REAL_SUMA, 0.0);
            cashStructure.Add(CoreConst.CALC_TAX_SUMA, 0.0);
            //Якщо true то знижка чи надбавка діє на всі позиції(товари) чеку
            cashStructure.Add(CoreConst.DISC_ALL_ITEMS, false);
            //Масив з значеннями знижки та надбавки в процентних значеннях
            cashStructure.Add(CoreConst.DISC_ARRAY_PERCENT, new double[2]);
            //Масив з значеннями знижки та надбавки в грошових значеннях
            cashStructure.Add(CoreConst.DISC_ARRAY_CASH, new double[2]);
            //Значення постійної знижки в процентному значенні
            cashStructure.Add(CoreConst.DISC_CONST_PERCENT, 0.0);
            //Сума знижки і надбавки з процентними значеннями
            cashStructure.Add(CoreConst.DISC_ONLY_PERCENT, 0.0);
            //Сума знижки і надбавки з грошовими значеннями
            cashStructure.Add(CoreConst.DISC_ONLY_CASH, 0.0);
            //Загальний коефіціент знижки в процентному значенні
            cashStructure.Add(CoreConst.DISC_FINAL_PERCENT, 0.0);
            //Загальний коефіціент знижки в грошовому значенні
            cashStructure.Add(CoreConst.DISC_FINAL_CASH, 0.0);
            //Загальний коефіціент знижки в грошовому значенні
            cashStructure.Add(CoreConst.DISC_APPLIED, false);
        }
    }
}