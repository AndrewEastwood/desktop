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

        /* data */
        // data must be used in the default profile only
        private Dictionary<DataType, DataTable> data = new Dictionary<DataType, DataTable>();
        private Hashtable props = new Hashtable();

        // profile data
        private string id;
        private string name;
        private string productFilter;

        // triggers
        private bool trExchangeAccessError;

        public AppProfile(string id, string name)
            : this(id, name, "%")
        {
        }
        public AppProfile(string id, string name, string filter)
        {
            this.id = id;
            this.name = name;
            this.productFilter = filter;
            this.props = getEmptyProperties();
        }

        /* setup */
        public Hashtable getEmptyProperties()
        {
            if (!this.isDefaultProfile())
                return Container.Default.getEmptyProperties();
            
            Hashtable props = new Hashtable();

            // Append Order General Info Block
            // -----------
            props.Add(CoreConst.STORE_NO, Container.Configuration.CommonConfiguration.APP_SubUnit);
            // Default Client ID
            props.Add(CoreConst.CLIENT_ID, string.Empty);
            // Detect if cheque is retiermant
            props.Add(CoreConst.IS_RET, false);
            // Determinate that cheque is legal
            props.Add(CoreConst.IS_LEGAL, false);
            // Discount Structure
            // ***** props.Add(CoreConst.DISCOUNT, null);
            // Payment Structure
            // ****** props.Add(CoreConst.PAYMENT, null);
            // Payment Structure
            // ****** props.Add(CoreConst.BILL, null);

            // Cash Block
            // ------------
            // Cheque Number
            props.Add(CoreConst.ORDER_NO, null);
            // Cheque Suma with all discounts
            // * props.Add(CoreConst.ORDER_SUMA, null);
            // Cheque real suma (without discount)
            // * props.Add(CoreConst.ORDER_REAL_SUMA, null);
            // Cheque's tax suma
            // * props.Add(CoreConst.TAX_SUMA, null);
            // Determinate if this cheque need tax bill
            props.Add(CoreConst.TAX_BILL, null);


            // Discount block
            // ------------
            props.Add(CoreConst.CALC_CHEQUE_SUMA, 0.0);
            // 
            props.Add(CoreConst.CALC_REAL_SUMA, 0.0);
            // 
            props.Add(CoreConst.CALC_TAX_SUMA, 0.0);

            // 
            //Якщо true то знижка чи надбавка діє на всі позиції(товари) чеку
            props.Add(CoreConst.DISC_ALL_ITEMS, false);
            //Масив з значеннями знижки та надбавки в процентних значеннях
            props.Add(CoreConst.DISC_ARRAY_PERCENT, new double[2]);
            //Масив з значеннями знижки та надбавки в грошових значеннях
            props.Add(CoreConst.DISC_ARRAY_CASH, new double[2]);
            //Значення постійної знижки в процентному значенні
            props.Add(CoreConst.DISC_CONST_PERCENT, 0.0);
            //Сума знижки і надбавки з процентними значеннями
            props.Add(CoreConst.DISC_ONLY_PERCENT, 0.0);
            //Сума знижки і надбавки з грошовими значеннями
            props.Add(CoreConst.DISC_ONLY_CASH, 0.0);
            //Загальний коефіціент знижки в процентному значенні
            props.Add(CoreConst.DISC_FINAL_PERCENT, 0.0);
            //Загальний коефіціент знижки в грошовому значенні
            props.Add(CoreConst.DISC_FINAL_CASH, 0.0);
            //Загальний коефіціент знижки в грошовому значенні
            props.Add(CoreConst.DISC_APPLIED, false);

            return props;
        }

        /* properties */

        // profile specific data
        public ProfilesContainer Container { set { parent = value; } get { return parent; } }
        public string Name { get { return this.name; } }
        public string ID { get { return this.id; } }
        public string ProductFilter { get { return this.productFilter; } set { productFilter = value; } }
        
        // profile dynamic data
        public DataTable Order { get { return getOrder(); } }
        public DataTable Products { get { return getProducts(); } }
        public Hashtable Properties { get { return getProperties(); } }
        public Dictionary<DataType, DataTable> Data
        {
            get
            {
                if (!this.isDefaultProfile())
                    return Container.Default.Data;
                return data;
            }
            set
            {
                if (this.isDefaultProfile())
                    data = value;
                else
                    Container.Default.Data = value;
            }
        }

        /* in progress */

        public T getPropertyValue<T>(string key)
        {
            if (this.isDefaultProfile())
            {
                if (!Properties.ContainsKey(key))
                    return default(T);

                object val = Properties[key];

                try
                {
                    return components.Lib.TypeConv.ConvertTo<T>(val);
                }
                catch (Exception ex) { CoreLib.WriteLog(ex, "driver.Components.Profiles.AppProfile@getdataPropsValue"); }

                return default(T);
            }
            
            return Container.Default.getPropertyValue<T>(key);
        }

        public void setProperties(string key, object value)
        {
            if (this.isDefaultProfile())
            {
                props[key] = value;
            }
            else
                Container.Default.setProperties(key, value);
        }

        public Hashtable getProperties()
        {
            // Hashtable props = new Hashtable();
            DataTable _local_order = this.Order;

            //*bool useConstDisc = discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 &&
            //*    discArrdataProps[0] == 0.0 && discArrdataProps[1] == 0.0;
            double[] _discArrP = getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT);
            double[] _discArrC = getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH);
            bool useConstDisc = _discArrP[0] == 0 && _discArrP[1] == 0 && _discArrC[0] == 0 && _discArrC[1] == 0;

            // Get discount value
            if (useConstDisc)
            {
                // * discConstPercent = 0.0;
                props[CoreConst.DISC_CONST_PERCENT] = 0.0;

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
                // * discOnlydataProps = discArrdataProps[0] + discArrdataProps[1];
                // * discOnlyPercent = MathLib.GetRoundedMoney(discOnlyPercent);
                // * discOnlydataProps = MathLib.GetRoundedMoney(discOnlydataProps);

                props[CoreConst.DISC_ONLY_PERCENT] = MathLib.GetRoundedMoney(_discArrP[0] + _discArrP[1]);
                props[CoreConst.DISC_ONLY_CASH] = MathLib.GetRoundedMoney(_discArrC[0] + _discArrC[1]);
            }

            //if (Cheque.Rows.Count == 0)
            if (_local_order.Rows.Count == 0)
            {
                //realSUMA = chqSUMA = taxSUMA = 0.0;
                props[CoreConst.CALC_REAL_SUMA] = 0.0;
                props[CoreConst.CALC_CHEQUE_SUMA] = 0.0;
                props[CoreConst.CALC_TAX_SUMA] = 0.0;


                // ???? UpdateSumDisplay(false, updateCustomer);
                // this.PD_Empty_local_order;
                // *** return props;
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
                props[CoreConst.DISC_FINAL_PERCENT] = props[CoreConst.DISC_CONST_PERCENT];
            }
            else
            {
                //discCommonPercent = discOnlyPercent;
                props[CoreConst.DISC_FINAL_PERCENT] = props[CoreConst.DISC_ONLY_PERCENT];
            }
            if (discSUMA != 0.0)
            {
                // * discCommonPercent += (discOnlydataProps * 100) / discSUMA;
                props[CoreConst.DISC_FINAL_PERCENT] = getPropertyValue<double>(CoreConst.DISC_FINAL_PERCENT) + (getPropertyValue<double>(CoreConst.DISC_ONLY_CASH) * 100) / discSUMA;
            }
            // * discCommonPercent = MathLib.GetRoundedMoney(discCommonPercent);
            props[CoreConst.DISC_FINAL_PERCENT] = MathLib.GetRoundedMoney(getPropertyValue<double>(CoreConst.DISC_FINAL_PERCENT));

            // restore native cheque sum
            // and set price acording to client's discount card
            //DataRow[] artRecord = null;
            double newPrice = 0.0;
            double newTmpPrice = 0.0; //bool isSet = false;
            Hashtable profileDefinedTaxGrid = new Hashtable();
            Hashtable profileCompatibleTaxGrid = new Hashtable();
            for (i = 0; i < _local_order.Rows.Count; i++)
            {
                newPrice = MathLib.GetDouble(_local_order.Rows[i]["ORIGPRICE"]);

                //isSet = false;
                if (Container.valueOfClientPriceNo != 0)
                {
                    newTmpPrice = MathLib.GetDouble(_local_order.Rows[i]["PR" + Container.valueOfClientPriceNo].ToString());
                    if (newTmpPrice != 0.0) newPrice = newTmpPrice;
                }
                else if (UserConfig.Properties[8])
                {
                    //DataRow dRow = Cheque.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                    //price = AppFunc.AutomaticPrice(thisTot, dRow);
                    double _newPrice = CoreLib.AutomaticPrice(MathLib.GetDouble(_local_order.Rows[i]["TOT"].ToString()), _local_order.Rows[i]);
                    try
                    {
                        profileDefinedTaxGrid = (Hashtable)driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates[_local_order.Rows[i]["F"]];
                    }
                    catch { }
                    // new tax mode
                    bool _thisRowCanUseDiscount = true;
                    try
                    {
                        // get application tax char with compatible tax grid
                        string[] definedTaxData = profileDefinedTaxGrid[_local_order.Rows[i]["VG"].ToString()].ToString().Split(';');
                        _thisRowCanUseDiscount = Boolean.Parse(definedTaxData[1]);
                    }
                    catch { }

                    if (_thisRowCanUseDiscount)
                    {

                        double _discountPrices = 0.0;
                        //for (int ii = 0; ii < Cheque.Rows.Count; ii++)
                        //{
                        if (_newPrice != (double)_local_order.Rows[i]["ORIGPRICE"])
                        {
                            _discountPrices = 100 - _newPrice * 100 / (double)_local_order.Rows[i]["ORIGPRICE"];
                            // * if (_discountPrices > discCommonPercent)
                            if (_discountPrices > getPropertyValue<double>(CoreConst.DISC_FINAL_PERCENT))
                            {
                                _local_order.Rows[i]["USEDDISC"] = Boolean.FalseString;
                                newPrice = _newPrice;
                            }
                            else
                            {
                                _local_order.Rows[i]["USEDDISC"] = Boolean.TrueString;
                                newPrice = MathLib.GetDouble(_local_order.Rows[i]["ORIGPRICE"]);
                            }
                        }
                        else newPrice = _newPrice;
                        //}
                    }
                    else newPrice = _newPrice;
                }
                else if (UserConfig.Properties[1] || UserConfig.Properties[2])
                {
                    newPrice = MathLib.GetDouble(_local_order.Rows[i]["PRICE"]);
                }
                _local_order.Rows[i]["PRICE"] = newPrice;
                _local_order.Rows[i]["ASUM"] = _local_order.Rows[i]["SUM"] = MathLib.GetRoundedMoney(MathLib.GetDouble(_local_order.Rows[i]["TOT"].ToString()) * newPrice);
                _local_order.Rows[i]["DISC"] = 0.0;
            }
            // * chqSUMA = (double)_local_order.Compute("sum(SUM)", "");
            // * chqSUMA = MathLib.GetRoundedMoney(chqSUMA);
            // * realSUMA = chqSUMA;
            props[CoreConst.CALC_CHEQUE_SUMA] = MathLib.GetRoundedMoney((double)_local_order.Compute("sum(SUM)", ""));
            props[CoreConst.CALC_REAL_SUMA] = props[CoreConst.CALC_CHEQUE_SUMA];

            //select rows with discount mode
            try
            {
                dRows = _local_order.Select("USEDDISC = " + Boolean.TrueString);
                // * _fl_useTotDisc = (dRows.Length == _local_order.Rows.Count);
                Container.triggerUseTotDisc = (dRows.Length == _local_order.Rows.Count);

                //discSUMA = (double)Cheque.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                object d = _local_order.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                if (d != DBNull.Value)
                    double.TryParse(d.ToString(), out discSUMA);
                if (dRows.Length == 0)
                {
                    // * this.discApplied = false;
                    props[CoreConst.DISC_APPLIED] = false;
                }
                else
                    props[CoreConst.DISC_APPLIED] = true; // * this.discApplied = true;
            }
            catch { };


            DataRow[] prRows = null;
            // * if (this.clientPriceNo != 0)
            if (Container.valueOfClientPriceNo != 0)
                prRows = _local_order.Select("PR" + Container.valueOfClientPriceNo + " <> 0");


            // * if (_fl_useTotDisc && prRows == null)
            if (Container.triggerUseTotDisc && prRows == null)
            {
                //obrahunok realnoi sumu cheku zi znugkojy
                if (useConstDisc)
                {
                    // * dValue = (discConstPercent * discSUMA) / 100;
                    // * dValue = MathLib.GetRoundedMoney(dValue);
                    // * realSUMA -= dValue;
                    dValue = MathLib.GetRoundedMoney((getPropertyValue<double>(CoreConst.DISC_CONST_PERCENT) * discSUMA) / 100);
                }
                else
                {
                    // * dValue = (discOnlyPercent * discSUMA) / 100;
                    // * dValue = MathLib.GetRoundedMoney(dValue);
                    // * realSUMA -= dValue;
                    // * realSUMA -= discOnlydataProps;

                    dValue = MathLib.GetRoundedMoney((getPropertyValue<double>(CoreConst.DISC_CONST_PERCENT) * discSUMA) / 100);
                    props[CoreConst.CALC_REAL_SUMA] = getPropertyValue<double>(CoreConst.CALC_REAL_SUMA) - dValue - getPropertyValue<double>(CoreConst.DISC_ONLY_CASH);
                }
            }
            else
            {
                // * _fl_useTotDisc = false;
                Container.triggerUseTotDisc = false;
                for (i = 0; i < dRows.Length; i++)
                {
                    // don't use discount when client want to has another price for current article
                    // * if (this.clientPriceNo != 0 && MathLib.GetDouble(dRows[i]["PR" + this.clientPriceNo].ToString()) > 0.0)
                    if (Container.valueOfClientPriceNo != 0 && MathLib.GetDouble(dRows[i]["PR" + Container.valueOfClientPriceNo].ToString()) > 0.0)
                    {
                        dRows[i]["DISC"] = 0.0;
                        continue;
                    }
                    dRows[i]["DISC"] = getPropertyValue<double>(CoreConst.DISC_FINAL_PERCENT);
                    dValue = (getPropertyValue<double>(CoreConst.DISC_FINAL_PERCENT) * (double)dRows[i]["SUM"]) / 100;
                    dValue = (double)dRows[i]["SUM"] - dValue;
                    dRows[i]["ASUM"] = MathLib.GetRoundedMoney(dValue);
                }
                // * realSUMA = (double)_local_order.Compute("Sum(ASUM)", "");
                props[CoreConst.CALC_REAL_SUMA] = (double)_local_order.Compute("Sum(ASUM)", "");
            }
            props[CoreConst.CALC_REAL_SUMA] = MathLib.GetRoundedMoney(getPropertyValue<double>(CoreConst.CALC_REAL_SUMA));

            //vuvedennja zagalnogo koeficientu znugku v 2oh tupah
            //groshovuj koeficient
            // * discCommondataProps = chqSUMA - realSUMA;
            // * discCommondataProps = MathLib.GetRoundedMoney(discCommondataProps);
            props[CoreConst.DISC_ONLY_CASH] = MathLib.GetRoundedMoney(getPropertyValue<double>(CoreConst.CALC_CHEQUE_SUMA) - getPropertyValue<double>(CoreConst.DISC_FINAL_CASH));

            // calculating tax sum
            // * taxSUMA = 0.0;
            props[CoreConst.CALC_TAX_SUMA] = 0.0;

            for (i = 0; i < _local_order.Rows.Count; i++)
            {
                try
                {
                    taxValue = MathLib.GetDouble(_local_order.Rows[i]["TAX_VAL"]);
                    if (Boolean.Parse(_local_order.Rows[i]["USEDDISC"].ToString()))
                    {
                        // * artSum = (discCommonPercent * (double)_local_order.Rows[i]["SUM"]) / 100;
                        artSum = (getPropertyValue<double>(CoreConst.DISC_FINAL_PERCENT) * (double)_local_order.Rows[i]["SUM"]) / 100;
                        artSum = (double)_local_order.Rows[i]["SUM"] - artSum;
                        artSum = MathLib.GetRoundedMoney(artSum);
                        taxValue = (artSum * taxValue) / (taxValue + 100);
                    }
                    else
                        taxValue = (((double)_local_order.Rows[i]["ASUM"]) * taxValue) / (taxValue + 100);
                }
                catch
                {
                    taxValue = 0;
                }

                _local_order.Rows[i]["TAX_MONEY"] = taxValue;
                // * taxSUMA += taxValue;
                props[CoreConst.CALC_TAX_SUMA] = getPropertyValue<double>(CoreConst.CALC_TAX_SUMA) + taxValue;
            }

            // * taxSUMA = MathLib.GetRoundedMoney(taxSUMA);
            props[CoreConst.CALC_TAX_SUMA] = MathLib.GetRoundedMoney(getPropertyValue<double>(CoreConst.CALC_TAX_SUMA));

            // * if (!_fl_isInvenCheque)
            // *     UpdateSumDisplay(true, updateCustomer);
            // (i) called within parent event


            //if (!Container.triggerInventCheque)
            //    Container.OnPropertiesChanged(EventArgs.Empty);

            //return props;
            return props;
        }

        /* data access and management */
        public string getProductFilterQuery()
        {
            if (ProductFilter.Length == 0)
                return string.Format("F='{0}'", ID);

            List<string> profileFilteredProducts = new List<string>();
            string[] filterProductIDs = ProductFilter.Split(' ', ',', ';');
            foreach (string lss in filterProductIDs)
                if (lss != string.Empty)
                    profileFilteredProducts.Add(string.Format("ID LIKE '{0}%'", lss));

            if (profileFilteredProducts.Capacity > 0)
                return String.Join(" OR ", profileFilteredProducts.ToArray());

            return string.Format("F='{0}'", ID);
        }
        public DataTable getProducts()
        {
            return getData(DataType.PRODUCT);
        }
        public DataTable getOrder()
        {
            return getData(DataType.ORDER);
        }
        public DataTable getData(DataType dType)
        {
            List<DataRow> dRows = new List<DataRow>();
            dRows.AddRange(data[dType].Select(getProductFilterQuery()));
            DataTable dT = Container.setupEmptyDataTable(DataType.PRODUCT);
            foreach (DataRow dr in dRows)
                dT.Rows.Add(dr.ItemArray);
            return dT;
        }

        // these methods must use default profile 
        public void orderProductAdd(object[] arrayData)
        {
            if (isDefaultProfile())
            {
                this.data[DataType.ORDER].Rows.Add(arrayData);
            }
            else
                Container.Default.orderProductAdd(arrayData);
        }
        public void orderProductUpdate(string recordID, object[] arrayData)
        {
        }
        public void orderProductRemove(string recordID)
        {
        }

        // triggers
        public bool isDefaultProfile()
        {
            return this.ID.Equals(CoreConst.KEY_DEFAULT_PROFILE_ID);
        }

    }
}