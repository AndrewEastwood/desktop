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
    public class AppProfile : ICloneable
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

        public AppProfile(string id, string name, ProfilesContainer parent)
            : this(id, name, "%", parent)
        {
        }
        public AppProfile(string id, string name, string filter, ProfilesContainer parent)
        {
            this.id = id;
            this.name = name;
            this.productFilter = filter;
            this.parent = parent;

            if (this.isDefaultProfile())
            {
                this.props = getEmptyProperties(DataSection.All);
                // init data containers
                string[] dataKeys = Enum.GetNames(typeof(DataType));
                for (int i = 0; i < dataKeys.Length; i++)
                {
                    data.Add((DataType)i, setupEmptyDataTable((DataType)i));
                    data[(DataType)i].TableName = string.Format("{0}_{1}", dataKeys[i], id);
                }

                // init order event handlers
                DataOrder.RowDeleted += new DataRowChangeEventHandler(Order_RowDeleted);
                DataOrder.RowChanged += new DataRowChangeEventHandler(Order_RowChanged);
            }
        }

        /* setup */
        public Hashtable getEmptyProperties(DataSection ds)
        {
            if (!this.isDefaultProfile())
                return Container.Default.getEmptyProperties(ds);
            
            Hashtable props = new Hashtable();

            // Order Block
            // -----------
            if (((int)ds & (int)DataSection.Order) != 0)
            {
                props.Add(CoreConst.ORDER_STORE_NO, Container.Configuration.CommonConfiguration.APP_SubUnit);
                // Default Client ID
                props.Add(CoreConst.ORDER_CLIENT_ID, string.Empty);
                // Detect if cheque is retiermant
                props.Add(CoreConst.ORDER_IS_RET, false);
                // Determinate that cheque is legal
                props.Add(CoreConst.ORDER_IS_LEGAL, false);
                // Cheque Number
                props.Add(CoreConst.ORDER_NO, null);
                // Determinate if this cheque need tax bill
                props.Add(CoreConst.ORDER_TAX_BILL, null);
            }

            // Bill block
            // ------------
            if (((int)ds & (int)DataSection.Bill) != 0)
            {
                props.Add(CoreConst.BILL_OID, string.Empty);
                // DataTable source
                props.Add(CoreConst.BILL_NO, string.Empty);
                // String - owner number (for clonned bill)
                props.Add(CoreConst.BILL_OWNER_NO, string.Empty);
                // Storu Number
                props.Add(CoreConst.BILL_DATETIME, DateTime.Now);
                // Storu Number
                props.Add(CoreConst.BILL_DATETIMEEDIT, DateTime.Now);
                // Default Client ID
                props.Add(CoreConst.BILL_COMMENT, string.Empty);
                // Detect if cheque is retiermant
                props.Add(CoreConst.BILL_IS_LOCKED, false);
                // Determinate that cheque is legal
                props.Add(CoreConst.BILL_PATH, string.Empty);
                // Determinate that cheque is legal
                props.Add(CoreConst.BILL_DELETED_ROWS, new Dictionary<string, object[]>());
            }

            // Cash Block
            // ------------
            if (((int)ds & (int)DataSection.Cash) != 0)
            {
                props.Add(CoreConst.CASH_CHEQUE_SUMA, 0.0);
                // 
                props.Add(CoreConst.CASH_REAL_SUMA, 0.0);
                // 
                props.Add(CoreConst.CASH_TAX_SUMA, 0.0);
            }

            // Discount block
            // ------------

            if (((int)ds & (int)DataSection.Discount) != 0)
            {
                //Якщо true то знижка чи надбавка діє на всі позиції(товари) чеку
                props.Add(CoreConst.DISCOUNT_ALL_ITEMS, false);
                //Масив з значеннями знижки та надбавки в процентних значеннях
                props.Add(CoreConst.DISCOUNT_MANUAL_PERCENT_SUB, 0.0);
                props.Add(CoreConst.DISCOUNT_MANUAL_PERCENT_ADD, 0.0);
                //Масив з значеннями знижки та надбавки в грошових значеннях
                props.Add(CoreConst.DISCOUNT_MANUAL_CASH_SUB, 0.0);
                props.Add(CoreConst.DISCOUNT_MANUAL_CASH_ADD, 0.0);
                //Значення постійної знижки в процентному значенні
                props.Add(CoreConst.DISCOUNT_CONST_PERCENT, 0.0);
                //Сума знижки і надбавки з процентними значеннями
                props.Add(CoreConst.DISCOUNT_ONLY_PERCENT, 0.0);
                //Сума знижки і надбавки з грошовими значеннями
                props.Add(CoreConst.DISCOUNT_ONLY_CASH, 0.0);
                //Загальний коефіціент знижки в процентному значенні
                props.Add(CoreConst.DISCOUNT_FINAL_PERCENT, 0.0);
                //Загальний коефіціент знижки в грошовому значенні
                props.Add(CoreConst.DISCOUNT_FINAL_CASH, 0.0);
                //Загальний коефіціент знижки в грошовому значенні
                props.Add(CoreConst.DISCOUNT_APPLIED, false);
            }

            return props;
        }

        /* properties */

        // profile specific data
        public ProfilesContainer Container { set { parent = value; } get { return parent; } }
        public string Name { get { return this.name; } }
        public string ID { get { return this.id; } }
        public string ProductFilter { get { return this.productFilter; } set { productFilter = value; } }
        
        // profile dynamic data
        public DataTable DataOrder { get { return getOrder(); } }
        public DataTable DataProducts { get { return getProducts(); } }
        public DataTable DataAlternative { get { return getAlternative(); } }
        public DataTable DataDiscountCards { get { return getDiscountCards(); } }
        public Hashtable Properties
        {
            get { return props; }
            set
            {
                props = value;
            }
        }
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

        public Hashtable refreshProperties()
        {
            // Hashtable Properties = new Hashtable();
            DataTable _local_order = this.DataOrder;

            //*bool useConstDisc = discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 &&
            //*    discArrdataProps[0] == 0.0 && discArrdataProps[1] == 0.0;
            double[] _discArrP = { customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB], customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD] };
            double[] _discArrC = { customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_SUB], customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_ADD] };
            bool useConstDisc = _discArrP[0] == 0 && _discArrP[1] == 0 && _discArrC[0] == 0 && _discArrC[1] == 0;

            // Get discount value
            if (useConstDisc)
            {
                // * discConstPercent = 0.0;
                Properties[CoreConst.DISCOUNT_CONST_PERCENT] = 0.0;

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

                Properties[CoreConst.DISCOUNT_ONLY_PERCENT] = MathLib.GetRoundedMoney(_discArrP[0] + _discArrP[1]);
                Properties[CoreConst.DISCOUNT_ONLY_CASH] = MathLib.GetRoundedMoney(_discArrC[0] + _discArrC[1]);
            }

            //if (Cheque.Rows.Count == 0)
            if (_local_order.Rows.Count == 0)
            {
                //realSUMA = chqSUMA = taxSUMA = 0.0;
                Properties[CoreConst.CASH_REAL_SUMA] = 0.0;
                Properties[CoreConst.CASH_CHEQUE_SUMA] = 0.0;
                Properties[CoreConst.CASH_TAX_SUMA] = 0.0;


                // ???? UpdateSumDisplay(false, updateCustomer);
                // this.PD_Empty_local_order;
                // *** return Properties;
                if (!Container.triggerInventCheque)
                    OnPropertiesUpdated(this, Properties, "refresh_cash", EventArgs.Empty);
                return Properties;
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
                Properties[CoreConst.DISCOUNT_FINAL_PERCENT] = Properties[CoreConst.DISCOUNT_CONST_PERCENT];
            }
            else
            {
                //discCommonPercent = discOnlyPercent;
                Properties[CoreConst.DISCOUNT_FINAL_PERCENT] = Properties[CoreConst.DISCOUNT_ONLY_PERCENT];
            }
            if (discSUMA != 0.0)
            {
                // * discCommonPercent += (discOnlydataProps * 100) / discSUMA;
                Properties[CoreConst.DISCOUNT_FINAL_PERCENT] = getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT) + (getPropertyValue<double>(CoreConst.DISCOUNT_ONLY_CASH) * 100) / discSUMA;
            }
            // * discCommonPercent = MathLib.GetRoundedMoney(discCommonPercent);
            Properties[CoreConst.DISCOUNT_FINAL_PERCENT] = MathLib.GetRoundedMoney(getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT));

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
                            if (_discountPrices > getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT))
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
            object dSummaW = _local_order.Compute("sum(SUM)", "");
            Properties[CoreConst.CASH_CHEQUE_SUMA] = MathLib.GetRoundedMoney((double)(_local_order.Compute("sum(SUM)", "")));
            Properties[CoreConst.CASH_REAL_SUMA] = Properties[CoreConst.CASH_CHEQUE_SUMA];

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
                    Properties[CoreConst.DISCOUNT_APPLIED] = false;
                }
                else
                    Properties[CoreConst.DISCOUNT_APPLIED] = true; // * this.discApplied = true;
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
                    dValue = MathLib.GetRoundedMoney((getPropertyValue<double>(CoreConst.DISCOUNT_CONST_PERCENT) * discSUMA) / 100);
                }
                else
                {
                    // * dValue = (discOnlyPercent * discSUMA) / 100;
                    // * dValue = MathLib.GetRoundedMoney(dValue);
                    // * realSUMA -= dValue;
                    // * realSUMA -= discOnlydataProps;

                    dValue = MathLib.GetRoundedMoney((getPropertyValue<double>(CoreConst.DISCOUNT_CONST_PERCENT) * discSUMA) / 100);
                    Properties[CoreConst.CASH_REAL_SUMA] = getPropertyValue<double>(CoreConst.CASH_REAL_SUMA) - dValue - getPropertyValue<double>(CoreConst.DISCOUNT_ONLY_CASH);
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
                    dRows[i]["DISC"] = getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT);
                    dValue = (getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT) * (double)dRows[i]["SUM"]) / 100;
                    dValue = (double)dRows[i]["SUM"] - dValue;
                    dRows[i]["ASUM"] = MathLib.GetRoundedMoney(dValue);
                }
                // * realSUMA = (double)_local_order.Compute("Sum(ASUM)", "");
                Properties[CoreConst.CASH_REAL_SUMA] = (double)_local_order.Compute("Sum(ASUM)", "");
            }
            Properties[CoreConst.CASH_REAL_SUMA] = MathLib.GetRoundedMoney(getPropertyValue<double>(CoreConst.CASH_REAL_SUMA));

            //vuvedennja zagalnogo koeficientu znugku v 2oh tupah
            //groshovuj koeficient
            // * discCommondataProps = chqSUMA - realSUMA;
            // * discCommondataProps = MathLib.GetRoundedMoney(discCommondataProps);
            Properties[CoreConst.DISCOUNT_ONLY_CASH] = MathLib.GetRoundedMoney(getPropertyValue<double>(CoreConst.CASH_CHEQUE_SUMA) - getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_CASH));

            // calculating tax sum
            // * taxSUMA = 0.0;
            Properties[CoreConst.CASH_TAX_SUMA] = 0.0;

            for (i = 0; i < _local_order.Rows.Count; i++)
            {
                try
                {
                    taxValue = MathLib.GetDouble(_local_order.Rows[i]["TAX_VAL"]);
                    if (Boolean.Parse(_local_order.Rows[i]["USEDDISC"].ToString()))
                    {
                        // * artSum = (discCommonPercent * (double)_local_order.Rows[i]["SUM"]) / 100;
                        artSum = (getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT) * (double)_local_order.Rows[i]["SUM"]) / 100;
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
                Properties[CoreConst.CASH_TAX_SUMA] = getPropertyValue<double>(CoreConst.CASH_TAX_SUMA) + taxValue;
            }

            // * taxSUMA = MathLib.GetRoundedMoney(taxSUMA);
            Properties[CoreConst.CASH_TAX_SUMA] = MathLib.GetRoundedMoney(getPropertyValue<double>(CoreConst.CASH_TAX_SUMA));

            // * if (!_fl_isInvenCheque)
            // *     UpdateSumDisplay(true, updateCustomer);
            // (i) called within parent event


            //if (!Container.triggerInventCheque)
            //    Container.OnPropertiesChanged(EventArgs.Empty);


            if (!Container.triggerInventCheque)
                OnPropertiesUpdated(this, Properties, "refresh_cash", EventArgs.Empty);

            //return Properties;
            return Properties;
        }

        /* data access and management */
        public string getProductFilterQuery()
        {
            if (ProductFilter.Length == 0 || ProductFilter.Equals("%"))
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
        public DataTable getAlternative()
        {
            return getData(DataType.ALTERNATEBC);
        }
        public DataTable getDiscountCards()
        {
            return getData(DataType.DCARDS);
        }
        public DataTable getData(DataType dType)
        {

            if (this.isDefaultProfile())
                return data[dType];
            
            List<DataRow> dRows = new List<DataRow>();
            dRows.AddRange(data[dType].Select(getProductFilterQuery()));
            DataTable dT = setupEmptyDataTable(DataType.PRODUCT);
            foreach (DataRow dr in dRows)
                dT.Rows.Add(dr.ItemArray);
            return dT;
        }

        public DataTable setupEmptyDataTable(DataType dType)
        {
            DataTable dTable = new DataTable();
            switch (dType)
            {
                case DataType.ALTERNATEBC:
                    {
                        Type[] cTypes = {
                            typeof(string),
                            typeof(string)};

                        dTable.Columns.Add("C", typeof(long));

                        dTable.Columns.Add("F", typeof(string));

                        for (byte i = 0; i < driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ALTColumnName.Length; i++)
                            dTable.Columns.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ALTColumnName[i], cTypes[i]);

                        dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
                        dTable.Columns["C"].AutoIncrement = true;
                        dTable.Columns["C"].Unique = true;
                        break;
                    }
                case DataType.PRODUCT:
                    {
                        Type[] cTypes = {
                            typeof(string),
                            typeof(string),
                            typeof(string),
                            typeof(string),
                            typeof(string),
                            typeof(string),
                            typeof(string),
                            typeof(double),
                            typeof(double),
                            typeof(double),
                            typeof(double),
                            typeof(double),
                            typeof(double),
                            typeof(double),
                            typeof(double),
                            typeof(double)};

                        dTable.Columns.Add("C", typeof(long));

                        dTable.Columns.Add("F", typeof(string));

                        for (byte i = 0; i < driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ARTColumnName.Length; i++)
                            dTable.Columns.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_ARTColumnName[i], cTypes[i]);

                        dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
                        dTable.Columns["C"].AutoIncrement = true;
                        dTable.Columns["C"].Unique = true;
                        break;
                    }
                case DataType.DCARDS:
                    {
                        Type[] cTypes = {
                            typeof(string),
                            typeof(string),
                            typeof(double),
                            typeof(int)};

                        if (driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_CARDColumnName.Length != cTypes.Length)
                            driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_CARDColumnName = new string[] { "CBC", "CID", "CDISC", "CPRICENO" };

                        dTable.Columns.Add("C", typeof(long));
                        for (byte i = 0; i < driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_CARDColumnName.Length; i++)
                            dTable.Columns.Add(driver.Config.ConfigManager.Instance.CommonConfiguration.STYLE_CARDColumnName[i], cTypes[i]);

                        dTable.Columns.Add("F", typeof(string));

                        //dTable.TableName = "DCards";

                        dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
                        dTable.Columns["C"].AutoIncrement = true;
                        dTable.Columns["C"].Unique = true;
                        break;
                    }
                case DataType.ORDER:
                    {
                        dTable = setupEmptyDataTable(DataType.PRODUCT);

                        DataColumn dCol = new DataColumn("TOT");
                        dCol.DataType = typeof(string);
                        dCol.DefaultValue = "0";
                        dTable.Columns.Add(dCol);

                        dCol = new DataColumn("TAX_VAL");
                        dCol.DataType = typeof(double);
                        dCol.DefaultValue = (double)0.0;
                        dTable.Columns.Add(dCol);

                        dCol = new DataColumn("USEDDISC");
                        dCol.DataType = typeof(bool);
                        dCol.DefaultValue = (bool)true;
                        dTable.Columns.Add(dCol);

                        dCol = new DataColumn("DISC");
                        dCol.DataType = typeof(double);
                        dCol.DefaultValue = (double)0.0;
                        dTable.Columns.Add(dCol);

                        dCol = new DataColumn("SUM");
                        dCol.AllowDBNull = false;
                        dCol.DataType = typeof(double);
                        dCol.DefaultValue = (double)0.0;
                        dTable.Columns.Add(dCol);

                        dCol = new DataColumn("ASUM");
                        dCol.AllowDBNull = false;
                        dCol.DataType = typeof(double);
                        dCol.DefaultValue = (double)0.0;
                        dTable.Columns.Add(dCol);

                        dCol = new DataColumn("TAX_MONEY");
                        dCol.DataType = typeof(double);
                        //dCol.Expression = "(ASUM*TAX_VAL)/(TAX_VAL+100)";
                        dCol.DefaultValue = (double)0.0;
                        dTable.Columns.Add(dCol);

                        dCol = new DataColumn("TMPTOT");
                        dCol.DataType = typeof(string);
                        dCol.DefaultValue = "0";
                        dTable.Columns.Add(dCol);

                        dCol = new DataColumn("ORIGPRICE");
                        dCol.DataType = typeof(double);
                        dCol.DefaultValue = (double)0.0;
                        dTable.Columns.Add(dCol);

                        dCol = new DataColumn("PRINTCOUNT");
                        dCol.DataType = typeof(double);
                        dCol.DefaultValue = (double)0.0;
                        dTable.Columns.Add(dCol);

                        dTable.PrimaryKey = new DataColumn[] { dTable.Columns["C"] };
                        dTable.Columns["C"].AutoIncrement = true;
                        dTable.Columns["C"].Unique = true;
                        break;
                    }
            }

            return dTable;
        }


        public void Merge(AppProfile profile)
        {
            Properties.Clear();
            Data.Clear();

            Properties = (Hashtable)profile.Properties.Clone();
            Data = profile.Data;
        }

        public Dictionary<string, object> getPropertyBlock(DataSection ds)
        {
            Dictionary<string, object> block = new Dictionary<string, object>();
            switch (ds)
            {
                case DataSection.All:
                    foreach (KeyValuePair<object, object> kv in Properties)
                        block[kv.Key.ToString()] = kv.Value;
                    break;
                default:
                    foreach (KeyValuePair<object, object> kv in Properties)
                        if (kv.Key.ToString().ToUpper().StartsWith(ds.ToString().ToUpper()))
                            block[kv.Key.ToString()] = kv.Value;
                    break;
            }

            return block;
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

        //
        public void refresh()
        {
            refreshProperties();
        }

        public void reset()
        {
            refreshProperties();
        }

        public void resetOrder()
        {
            DataOrder.Rows.Clear();
        }

        // triggers
        public bool isDefaultProfile()
        {
            return this.ID.Equals(CoreConst.KEY_DEFAULT_PROFILE_ID);
        }

        // = custom propperties
        public bool customCashDiscountManualSavingsEnabled
        {
            get
            {
                return (getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_SUB) != 0.0 ||
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_SUB) != 0.0);
            }
        }

        public bool customCashDiscountManualExtraEnabled
        {
            get
            {
                return (getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_ADD) != 0.0 ||
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_ADD) != 0.0);
            }
        }

        public bool customCashDiscountManualSavingsOnlyEnabled
        {
            get
            {
                return ((getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_SUB) != 0.0 &&
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_ADD) == 0.0) ||
                    (getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_SUB) != 0.0 &&
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_ADD) == 0.0));
            }
        }

        public bool customCashDiscountManualExtraOnlyEnabled
        {
            get
            {
                return ((getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_SUB) == 0.0 &&
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_ADD) != 0.0) ||
                    (getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_SUB) == 0.0 &&
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_ADD) != 0.0));
            }
        }

        public bool customCashDiscountManualOnlyEnabled
        {
            get
            {
                return (getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_SUB) != 0.0 ||
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_ADD) != 0.0 ||
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_SUB) != 0.0 ||
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_ADD) != 0.0) &&
                    // propgram discount must be empty
                    getPropertyValue<double>(CoreConst.DISCOUNT_CONST_PERCENT) == 0.0;
            }
        }

        public bool customCashDiscountPropgramOnlyEnable
        {
            get
            {
                return !customCashDiscountManualOnlyEnabled && getPropertyValue<double>(CoreConst.DISCOUNT_CONST_PERCENT) != 0.0;
            }
        }

        public bool customCashDiscountSomeEnabled
        {
            get
            {
                return customCashDiscountManualOnlyEnabled || customCashDiscountPropgramOnlyEnable;
            }
        }

        public bool customCashDiscountAllEnabled
        {
            get
            {
                return customCashDiscountManualOnlyEnabled && customCashDiscountPropgramOnlyEnable;
            }
        }

        public bool customCashDiscountManualIsEmpty
        {
            get
            {
                return (getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_SUB) == 0.0 &&
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_ADD) == 0.0 &&
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_SUB) == 0.0 &&
                    getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_ADD) == 0.0);
            }
        }

        public Dictionary<string, double> customCashDiscountItems
        {
            get {
                Dictionary<string, double> _discount = new Dictionary<string, double>();
                _discount[CoreConst.DISCOUNT_MANUAL_CASH_ADD] = Container.Default.getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_ADD);
                _discount[CoreConst.DISCOUNT_MANUAL_CASH_SUB] = Container.Default.getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_SUB);
                _discount[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD] = Container.Default.getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_ADD);
                _discount[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] = Container.Default.getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_SUB);
                _discount[CoreConst.DISCOUNT_CONST_PERCENT] = Container.Default.getPropertyValue<double>(CoreConst.DISCOUNT_CONST_PERCENT);
                _discount[CoreConst.DISCOUNT_FINAL_CASH] = Container.Default.getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_CASH);
                _discount[CoreConst.DISCOUNT_FINAL_PERCENT] = Container.Default.getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT);
                _discount[CoreConst.DISCOUNT_ONLY_CASH] = Container.Default.getPropertyValue<double>(CoreConst.DISCOUNT_ONLY_CASH);
                _discount[CoreConst.DISCOUNT_ONLY_PERCENT] = Container.Default.getPropertyValue<double>(CoreConst.DISCOUNT_ONLY_PERCENT);
                return _discount;
            }
        }

        // = custom methods
        public void customResetBlockDiscountManual()
        {
            customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_ADD] = 0.0;
            customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_SUB] = 0.0;
            customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD] = 0.0;
            customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] = 0.0;

            // recalculate cash and trigger event
            refreshProperties();
        }
        public void customResetBlockDiscountAll()
        {
            // reset all values
            foreach (KeyValuePair<string, double> de in customCashDiscountItems)
                customCashDiscountItems[de.Key.ToString()] = 0.0;

            // setup default values
            Properties[CoreConst.DISCOUNT_APPLIED] = false;
            Properties[CoreConst.DISCOUNT_ALL_ITEMS] = false;

            // recalculate cash and trigger event
            refreshProperties();
        }

        // = events

        public event PropertiesUpdatedEventHandler onPropertiesUpdated;

        protected virtual void OnPropertiesUpdated(AppProfile sender, Hashtable props, string actionKey, EventArgs e)
        {
            if (onPropertiesUpdated != null)
                onPropertiesUpdated(sender, props, actionKey, e);
        }


        protected void Order_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            refresh();
            OnPropertiesUpdated(this, Properties, "order_item_changed", EventArgs.Empty);
        }

        protected void Order_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            refresh();
            if (DataOrder.Rows.Count > 0)
                OnPropertiesUpdated(this, Properties, "order_item_removed", EventArgs.Empty);
            else
                OnPropertiesUpdated(this, Properties, "order_cleared", EventArgs.Empty);
        }

        public object Clone()
        {
            AppProfile p = new AppProfile(ID, Name, ProductFilter, Container);
            p.Properties = this.Properties;
            p.Data = Data;

            return p;
        }
    }

    public delegate void PropertiesUpdatedEventHandler(AppProfile sender, Hashtable props, string actionKey, EventArgs e);

    public enum DataSection : int
    {
        All = 0x00,
        Order = 0x01,
        Bill = 0x02,
        Cash = 0x04,
        Discount = 0x08
    }

}