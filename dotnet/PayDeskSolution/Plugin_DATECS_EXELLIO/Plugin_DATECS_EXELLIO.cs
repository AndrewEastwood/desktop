/* 
 * DATECS FP-3530T v1.10 driver with UI (User Interface)
 * Author: Andriy Ivaskevych
 * Date: 20/11/09
 * Notes:
 *  This driver used with PayDesk application only.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
/* PLUGIN REFERENCE */
//using PluginModule;
//using PluginModule.Components.Objects;
//using PluginModule.Lib;
/* INTERNAL REFERENCE */
using DATECS_EXELLIO.DriverUI;
using DATECS_EXELLIO.Config;
using DATECS_EXELLIO.DriverUI.Customs;
using DATECS_EXELLIO.Components.UI.DriverUI;
using components.Shared.Attributes;
using components.Shared.Interfaces;
using components.Components.SerialPort;
using components.Lib;

namespace DATECS_EXELLIO
{
    [PluginSimpleAttribute(PluginType.LegalPrinterDriver)]
    public class DATECS_EXELLIO : ILegalPrinterDriver
    {
        // UI Components
        private UserControl driverui;
        private UserControl portui;

        // Data
        private byte[] InputData;
        private byte[] OutputData;
        private byte[] DataForSend;
        private uint ReadedBytes;

        // Number Format Provider
        //private NumberFormatInfo NumberFormat;

        // Error Flags
        //private Hashtable DriverData;
        //private Hashtable ErrorFlags;
        //private Hashtable AllowedMethods;

        // Communication port
        private Com_SerialPort port;

        // Common functions
        private CoreLib func;
        //private XmlParserObject xmlpo;

        // Configuarion
        private Params param;

        // Service symbols
        private byte SEQ = 0x00;
        private byte CMD = 0x00;
        private byte PRE = 0x01;
        private byte TER = 0x03;
        private byte SEP = 0x04;
        private byte POS = 0x05;
        private byte LEN = 0x00;
        private byte NAK = 0x15;
        private byte SYN = 0x16;

        // Constructor
        public DATECS_EXELLIO()
        {
            // perform configuration and initialization driver plugin
            InitialiseDriverData();
            InitializeDriverComponents();
        }

        // Destructor
        ~DATECS_EXELLIO()
        {
            if (port.IsOpen)
                port.Close();
            port.Dispose(true);

            driverui.Dispose();
            portui.Dispose();

            param.Save();

            // perform saving configuration data
            //DocumentElement config = new DocumentElement(Name);
            //config.Add(Params.DriverData, "DriverData", xDataContainers.block);
            //config.Add(Params.ErrorFlags, "ErrorFlags", xDataContainers.block);
            //config.Add(Params.AllowedMethods, "AllowedMethods", xDataContainers.block);
            //config.Add(Params.AppAccess, "AppAccess", xDataContainers.block);
            //config.Add(Params.MiscData, "MiscData", xDataContainers.block);
        }

        // Driver Data
        private void InitialiseDriverData()
        {
            param = new Params();
            param.Load();
            //xmlpo = new XmlParserObject(Config.Path.FULL_CFG_PARAM_PATH);
            
            // initialize variables
            /*
            NumberFormat = new NumberFormatInfo();
            DriverData = new Hashtable();
            ErrorFlags = new Hashtable();
            string[] state = new string[48] { };

            // perform configuration number format
            NumberFormat.CurrencyDecimalSeparator = ".";
            NumberFormat.NumberDecimalSeparator = ".";

            // perform configuration driver data
            DriverData.Add("Status", state.Clone());
            DriverData.Add("ArtMemorySize", (uint)11800);
            DriverData.Add("UserNo", (byte)1);
            DriverData.Add("UserPwd", "0000");
            DriverData.Add("DeskNo", (byte)1);
            DriverData.Add("LastFunc", "");
            DriverData.Add("LastArtNo", (uint)1);
            DriverData.Add("LastFOrderNo", (uint)0);
            DriverData.Add("LastNOrderNo", (uint)0);
            DriverData.Add("LastROrderNo", (uint)0);

            // perform configuration error flags
            ErrorFlags.Add("FP_Sale", false);
            ErrorFlags.Add("FP_PayMoney", false);
            ErrorFlags.Add("FP_Payment", false);
            ErrorFlags.Add("FP_Discount", false);*/

            // trying to restore last saved configuration
            /*try
            {
                Hashtable _ldata = new Hashtable();
                bool _lstat = false;

                // perform restoring data
                _lstat = xmlpo.QuickParse(xParseType.XmlToData, ref _ldata);

                // check if restore was valid
                if (!_lstat)
                    throw new Exception("Configuration wasn't restored from file. Plugin will used default configuration");

                // if restoring was successfull then begin applying restored data to varibles
                param = new Params(_ldata);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }*/
        }

        // Driver Components
        private void InitializeDriverComponents()
        {
            // initialize objects
            port = new Com_SerialPort();
            func = new CoreLib();

            // perforom configation com-port
            port.LoadPortConfig();
            //port.Open();

            // perform configuration user's interfaces
            portui = new UI.AppUI.Port(ref port);
            driverui = new UI.AppUI.Tree();
            ((TreeView)driverui.Controls["functionsTree"]).NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(this.NodeMouseDoubleClick);
        }

        public bool Activate()
        {
            return port.Open();
        }

        public bool Deactivate()
        {
            return port.Close();
        }

        // Main Access Point
        public object CallFunction(string name, params object[] param)
        {
            // local variables
            string description = ((UI.AppUI.Tree)driverui).GetDescription(name);
            object value = new object();

            // reset transfering data
            OutputData = new byte[0];
            ReadedBytes = 0;

            // execute method
            switch (name)
            {
                /*
                 * General methods
                 * 
                 */
                #region Initialization
                case "SetPrintParams":
                    {
                        SetPrintParams sdt = new SetPrintParams(Name, description);
                        if (sdt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            Hashtable item = new Hashtable();
                            foreach (DictionaryEntry pItem in sdt.PrinterFormat)
                            {
                                item = (Hashtable)pItem.Value;
                                foreach (DictionaryEntry headLine in item)
                                    SetPrintParams(headLine.Key.ToString(), headLine.Value.ToString());
                            }
                        }
                        sdt.Dispose();
                        break;
                    }
                case "SetDateTime":
                    {
                        SetDateTime sdt = new SetDateTime(Name, description);
                        if (sdt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetDateTime(sdt.NewDateTime);
                        sdt.Dispose();
                        break;
                    }
                case "Fiscalize":
                    {
                        string[] _errMsgs = new string[9] {
                                "Таблиця податкових номерів вичерпана",
                                "Не вказаний фіскальний номер",
                                "Неправильний заводський номер або інші параметри",
                                "Відкритий чек",
                                "Не обнулені суми за день (зробіть Z-звіт)",
                                "Не задані податкові ставки",
                                "Податковий номер складається з нулів",
                                "Нема чекової (контрольної) стрічки",
                                "Годиннк не встановлений"};
                        break;
                    }
                case "SetTaxRate":
                    {
                        SetTaxRate str = new SetTaxRate(Name, description);
                        if (str.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] taxData = SetTaxRate(str.Password, str.DecimalPoint, str.UseRates, str.Rates);
                            string _infotext = string.Empty;

                            if (func.IsEmpty(taxData))
                            {
                                _infotext = string.Format("{0}\r\n\r\n{1} - {2}\r\n{3}: {4:0.00} [{5}]\r\n{6}: {7:0.00} [{8}]\r\n{9}: {10:0.00} [{11}]\r\n{12}: {13:0.00} [{14}]",
                                    "Податкові ставки",
                                    "Кількість знаків після коми", (byte)taxData[0],
                                    "Група А", (double)taxData[5], (bool)taxData[1] ? "дозволено" : "не дозволено",
                                    "Група Б", (double)taxData[6], (bool)taxData[2] ? "дозволено" : "не дозволено",
                                    "Група В", (double)taxData[7], (bool)taxData[3] ? "дозволено" : "не дозволено",
                                    "Група Г", (double)taxData[8], (bool)taxData[4] ? "дозволено" : "не дозволено");

                                value = taxData;
                            }
                            else
                                _infotext = "Нема даних";

                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        str.Dispose();
                        break;
                    }
                case "SetSaleMode":
                    {
                        SetSaleMode ssm = new SetSaleMode(Name, description);
                        if (ssm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetSaleMode(ssm.SaleMode);
                        ssm.Dispose();
                        break;
                    }
                case "SetSerialNum":
                    {
                        SetSerialNum ssm = new SetSerialNum(Name, description);
                        if (ssm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            char r = SetSerialNum(ssm.CountryCode, ssm.SerialNumber);
                            string _infotext = string.Empty;

                            if (r == 'P')
                                _infotext = "Команда виконана успішно";
                            if (r == 'F')
                                _infotext = "Невідформатована фіскальна память\r\nЗаводський номер вже встановлено\r\nНе встановлено дату";
                            if (r == '\0')
                                _infotext = "Нема відповіді від принтера";

                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        ssm.Dispose();
                        break;
                    }
                case "SetFixNum":
                    {
                        SetFixNum sfn = new SetFixNum(Name, description);
                        if (sfn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            char r = SetFixNum(sfn.FiscalNumber);
                            string _infotext = string.Empty;

                            if (r == 'P')
                                _infotext = "Команда виконана успішно";
                            if (r == 'F')
                                _infotext = "Заводський номер не встановлено\r\nНе встановлено дату\r\nВідкритий чек або необхідно виконати Z-звіт";
                            if (r == '\0')
                                _infotext = "Нема відповіді від принтера";

                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        sfn.Dispose();
                        break;
                    }
                case "SetTaxNum":
                    {
                        SetTaxNum stn = new SetTaxNum(Name, description);
                        if (stn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            char r = SetTaxNum(stn.TaxNumber, stn.TaxType);
                            string _infotext = string.Empty;

                            if (r == 'P')
                                _infotext = "Команда виконана успішно";
                            if (r == 'F')
                                _infotext = "Помилка виконання команди";
                            if (r == '\0')
                                _infotext = "Нема відповіді від принтера";

                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        stn.Dispose();
                        break;
                    }
                case "SetUserPass":
                    {
                        SetUserPass sup = new SetUserPass(Name, description);
                        if (sup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetUserPass(sup.OldPassword, sup.NewPassword, sup.UserNo);
                        sup.Dispose();
                        break;
                    }
                case "SetUserName":
                    {
                        SetUserName sun = new SetUserName(Name, description);
                        if (sun.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetUserName(sun.Password, sun.UserNo, sun.UserName);
                        sun.Dispose();
                        break;
                    }
                case "ResetUserData":
                    {
                        ResetUserData sun = new ResetUserData(Name, description);
                        if (sun.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ResetUserData(sun.Password, sun.UserNo);
                        sun.Dispose();
                        break;
                    }
                case "SetGetArticle":
                    {
                        SetGetArticle sga = new SetGetArticle(Name, description);
                        if (sga.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] artInfo = SetGetArticle(sga.Option, sga.Param);

                            string[] _errorMsgs = new string[2]{
                                "Команда виконана успішно",
                                "Помилка виконання команди"};
                            string _infotext = string.Empty;
                            string _infoFormat = string.Empty;
                            if (!func.IsEmpty(artInfo) && artInfo[0].ToString() != "F")
                                switch (sga.Option)
                                {
                                    case 'I':
                                        {
                                            _infotext = string.Format("{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}",
                                                "Загальна інформація",
                                                "Максимальний розмір назви товару", artInfo[0],
                                                "Всього товарів", artInfo[1],
                                                "Запроограмовано", artInfo[2]);
                                            break;
                                        }
                                    case 'D':
                                    case 'C':
                                    case 'P':
                                        {
                                            _infotext = _errorMsgs[0];
                                            break;
                                        }
                                    case 'F':
                                    case 'N':
                                    case 'R':
                                        {
                                            _infoFormat = "{0}\r\n\r\n{1}: {2}\r\n{3} - {4}\r\n{5}: {6}\r\n{7}: {8:0.00}\r\n";
                                            _infoFormat += "{9}: {10}\r\n{11}: {12:0.00}\r\n{13}: {14}\r\n{15}: {16:0.00}\r\n{17}: {18}";
                                            _infotext = string.Format(_infoFormat,
                                                "Інформація про товар",
                                                "Номер товару", artInfo[1],
                                                "Податкова група", artInfo[2],
                                                "Група", artInfo[3],
                                                "Ціна", double.Parse(artInfo[4].ToString(), Params.NumberFormat),
                                                "Кількість продажів", double.Parse(artInfo[5].ToString(), Params.NumberFormat),
                                                "Сума продажів", double.Parse(artInfo[6].ToString(), Params.NumberFormat),
                                                "Кількість в межах чеку", double.Parse(artInfo[7].ToString(), Params.NumberFormat),
                                                "Сума в межах чеку", double.Parse(artInfo[8].ToString(), Params.NumberFormat),
                                                "Назва", artInfo[9]);
                                            break;
                                        }
                                }
                            else
                                _infotext = _errorMsgs[1];

                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        sga.Dispose();
                        break;
                    }
                case "LoadLogo":
                    {
                        //LoadLogo("0000", new byte[96][]);
                        LoadLogo ll = new LoadLogo(Name, description);
                        if (ll.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            LoadLogo(ll.Password, ll.Logo);
                        ll.Dispose();
                        break;
                    }
                case "SetAdminPass":
                    {
                        SetAdminPass sap = new SetAdminPass(Name, description);
                        if (sap.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetAdminPass(sap.OldPassword, sap.NewPassword);
                        sap.Dispose();
                        break;
                    }
                case "ResetUserPass":
                    {
                        ResetUserPass sup = new ResetUserPass(Name, description);
                        if (sup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ResetUserPass(sup.Password, sup.UserNo);
                        sup.Dispose();
                        break;
                    }
                #endregion
                #region Other
                case "SetGetMoney":
                    {
                        SetGetMoney sgm = new SetGetMoney(Name, description);
                        if (sgm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            double[] money = SetGetMoney(sgm.Money);
                            string _infotext = string.Format("{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}",
                                "Фінансова звітнсть",
                                "Загальна сума внеску за день", money[2],
                                "Загальна сума вилучення за день", money[3],
                                "Сума в касі", money[1]);
                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = money.Clone();
                        }
                        sgm.Dispose();
                        break;
                    }
                case "PrintDiagInfo":
                    {
                        PrintDiagInfo();
                        break;
                    }
                case "Beep":
                    {
                        Beep();
                        break;
                    }
                case "OpenBox":
                    {
                        OpenBox ob = new OpenBox(Name, description);
                        if (ob.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            OpenBox(ob.Impulse);

                        value = new object[1] { ob.Impulse };
                        break;
                    }
                #endregion
                #region Display
                case "DisplText":
                    {
                        DisplText dt = new DisplText(Name, description);
                        if (dt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            DisplText(dt.TextLine);
                        dt.Dispose();
                        break;
                    }
                case "DisplBotLine":
                    {
                        DisplBotLine dbl = new DisplBotLine(Name, description);
                        if (dbl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            DisplBotLine(dbl.BottomLine);
                        dbl.Dispose();
                        break;
                    }
                case "DisplTopLine":
                    {
                        DisplTopLine dtl = new DisplTopLine(Name, description);
                        if (dtl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            DisplTopLine(dtl.TopLine);
                        dtl.Dispose();
                        break;
                    }
                case "ClrDispl":
                    {
                        ClrDispl();
                        break;
                    }
                case "DisplayDateTime":
                    {
                        DisplayDateTime();
                        break;
                    }
                #endregion
                #region DayReport
                case "ReportXZ":
                    {
                        ReportXZ rxz = new ReportXZ(Name, description);
                        if (rxz.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] xzInfo = ReportXZ(rxz.Password, rxz.ReportType, new bool[] { rxz.ClearUserSumm, rxz.ClearArtsSumm });
                            string _infotext = string.Empty;
                            if (!func.IsEmpty(xzInfo))
                            {
                                string _infoFormat = "{0}\r\n\r\n{1} - {2}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                                _infoFormat += "{7}: {8}\r\n{9}: {10}\r\n{11}: {12}";
                                _infotext = string.Format(_infoFormat,
                                    "Інформація поточного " + (rxz.ReportType == 0 ? "Z" : "X") + "-звіту",
                                    "Номер звіту", (uint)xzInfo[0],
                                    "Сума по групі А", (double)xzInfo[1],
                                    "Сума по групі Б", (double)xzInfo[2],
                                    "Сума по групі В", (double)xzInfo[3],
                                    "Сума по групі Г", (double)xzInfo[4],
                                    "Сума по групі Д", (double)xzInfo[5]);
                            }

                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = xzInfo.Clone();
                        }
                        rxz.Dispose();
                        break;
                    }
                #endregion
                #region Reports
                case "ReportByTax":
                    {
                        ReportByTax rbt = new ReportByTax(Name, description);
                        if (rbt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByTax(rbt.Password, rbt.StartDate, rbt.EndDate);
                        rbt.Dispose();
                        break;
                    }
                case "ReportByNoFull":
                    {
                        ReportByNoFull rbnf = new ReportByNoFull(Name, description);
                        if (rbnf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByNoFull(rbnf.Password, rbnf.StartNo, rbnf.EndNo);
                        rbnf.Dispose();
                        break;
                    }
                case "ReportByDateFull":
                    {
                        ReportByDateFull rbdf = new ReportByDateFull(Name, description);
                        if (rbdf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByDateFull(rbdf.Password, rbdf.StartDate, rbdf.EndDate);
                        rbdf.Dispose();
                        break;
                    }
                case "ReportByDateShort":
                    {
                        ReportByDateShort rbds = new ReportByDateShort(Name, description);
                        if (rbds.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByDateShort(rbds.Password, rbds.StartDate, rbds.EndDate);
                        rbds.Dispose();
                        break;
                    }
                case "ReportByNoShort":
                    {
                        ReportByNoShort rbns = new ReportByNoShort(Name, description);
                        if (rbns.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByNoShort(rbns.Password, rbns.StartNo, rbns.EndNo);
                        rbns.Dispose();
                        break;
                    }
                case "ReportByUsers":
                    {
                        ReportByUsers rbu = new ReportByUsers(Name, description);
                        if (rbu.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByUsers(rbu.Password);
                        rbu.Dispose();
                        break;
                    }
                case "ReportByArts":
                    {
                        ReportByArts rba = new ReportByArts(Name, description);
                        if (rba.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            ReportByArts(rba.Password, rba.ReportMode);
                        rba.Dispose();
                        break;
                    }
                #endregion
                #region Selling
                case "OpenNOrder":
                    {
                        ushort[] oinfo = OpenNOrder();
                        if (oinfo[3] != 0)
                        {
                            string[] _errMsgs = new string[4]{
                                "Не відформатована фіскальна пам'ять",
                                "Відкритий фіскальний чек",
                                "Вже відкритий нефіскальний чек",
                                "Не встановлена дата і час"};
                            System.Windows.Forms.DialogResult _res = System.Windows.Forms.MessageBox.Show(_errMsgs[oinfo[3] - 1], Name,
                                System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning);
                        }

                        value = oinfo.Clone();
                        break;
                    }
                case "CloseNOrder":
                    {
                        ushort[] oinfo = CloseNOrder();
                        value = oinfo.Clone();
                        break;
                    }
                case "PrintNText":
                    {
                        PrintNText pnt = new PrintNText(Name, description);
                        if (pnt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PrintNText(pnt.NonFixText);
                        pnt.Dispose();
                        break;
                    }
                case "OpenFOrder":
                    {
                        //OpenFOrder(port);
                        break;
                    }
                case "SubTotal": { break; }
                case "SaleArtIndicate": { break; }
                case "Total": { break; }
                case "PrintFText":
                    {
                        PrintFText pft = new PrintFText(Name, description);
                        if (pft.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PrintFText(pft.FixText);
                        pft.Dispose();
                        break;
                    }
                case "TotalAndClose": { break; }
                case "CloseFOrder":
                    {
                        CloseFOrder();
                        break;
                    }
                case "ResetOrder":
                    {
                        string _infotext = string.Format("{0}", "Анулювати поточне замовлення");
                        System.Windows.Forms.DialogResult _res = System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                        if (_res == System.Windows.Forms.DialogResult.Yes)
                            ResetOrder();
                        break;
                    }
                case "SaleArt": { break; }
                case "GroupDiscount": { break; }
                case "OpenROrder": { break; }
                case "PrintCopy":
                    {
                        PrintCopy pc = new PrintCopy(Name, description);
                        if (pc.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PrintCopy(pc.Copies);
                        pc.Dispose();
                        break;
                    }
                #endregion
                #region Printer commands
                case "CutChq":
                    {
                        CutChq();
                        break;
                    }
                case "LineFeed":
                    {
                        LineFeed lf = new LineFeed(Name, description);
                        if (lf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            LineFeed(lf.Lines);
                        lf.Dispose();
                        break;
                    }
                #endregion
                #region Get Info
                case "GetDateTime":
                    {
                        DateTime dtime = GetDateTime();
                        string _infotext = string.Format("{0}: {1}", "Поточна дата принтера", dtime.ToString("dd-MM-yyyy HH:mm:ss"));
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = dtime;
                        break;
                    }
                case "GetLastZReport":
                    {
                        GetLastZReport glzr = new GetLastZReport(Name, description);
                        if (glzr.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] zrep = GetLastZReport(glzr.ReportMode);
                            string _infotext = string.Empty;
                            if (!func.IsEmpty(zrep))
                            {
                                string _infoFormat = "{0}\r\n\r\n{1} - {2}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                                _infoFormat += "{7}: {8}\r\n{9}: {10}\r\n{11}: {12}";
                                _infotext = string.Format(_infoFormat,
                                    "Інформація останнього Z-звіту",
                                    "Номер звіту", (uint)zrep[0],
                                    "Сума по групі А", (double)zrep[1],
                                    "Сума по групі Б", (double)zrep[2],
                                    "Сума по групі В", (double)zrep[3],
                                    "Сума по групі Г", (double)zrep[4],
                                    "Сума по групі Д", (double)zrep[5]);
                            }
                            else
                                _infotext = "Немає даних";

                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = zrep.Clone();
                        }
                        glzr.Dispose();
                        break;
                    }
                case "GetSummsByDay":
                    {
                        GetSummsByDay gsbd = new GetSummsByDay(Name, description);
                        if (gsbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            double[] summ = GetSummsByDay(gsbd.ReportMode);
                            string _infoFormat = "{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                            _infoFormat += "{7}: {8:0.00}\r\n{9}: {10:0.00}";
                            string _infotext = string.Format(_infoFormat,
                                "Інформація сум за день",
                                "Сума по групі А", (double)summ[0],
                                "Сума по групі Б", (double)summ[1],
                                "Сума по групі В", (double)summ[2],
                                "Сума по групі Г", (double)summ[3],
                                "Сума по групі Д", (double)summ[4]);
                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = summ.Clone();
                        }
                        gsbd.Dispose();
                        break;
                    }
                case "GetSumCorByDay":
                    {
                        object[] scorr = GetSumCorByDay();
                        string _infoFormat = "{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                        _infoFormat += "{7}: {8}\r\n{9}: {10}\r\n{11}: {12}";
                        string _infotext = string.Format(_infoFormat,
                            "Суми корекцій за день",
                            "Сума всіх продажів", (double)scorr[0],
                            "Сума по чекових корекціях", (double)scorr[1],
                            "Сума виплат по кредиту", (double)scorr[2],
                            "Нефіскальні чеки", (ushort)scorr[3],
                            "Фіскальні чеки", (ushort)scorr[4],
                            "Чеки повернення", (ushort)scorr[5]);
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = scorr.Clone();
                        break;
                    }
                case "GetFreeMem":
                    {
                        uint fmem = GetFreeMem();
                        string _infotext = string.Format("{0} {1} {2}", "Розмір вільної пам'яті:", fmem, "записів");
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = fmem;
                        break;
                    }
                case "GetState":
                    {
                        byte[] _state = GetState();
                        string _infotext = string.Empty;
                        for (int i = 0; i < _state.Length; i++)
                        {
                            if (((string[])Params.DriverData["Status"])[_state[i]].Length == 0)
                                continue;
                            _infotext += ((string[])Params.DriverData["Status"])[_state[i]] + "\r\n";
                            if (i != 0 && _state[i] == 0)
                                break;
                        }
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = _infotext.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Clone();
                        break;
                    }
                case "GetFixTransState":
                    {
                        GetFixTransState gfts = new GetFixTransState(Name, description);
                        if (gfts.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] transinfo = GetFixTransState(gfts.TransParam);
                            string _infoFormat = "{0}\r\n\r\n{1} {2}\r\n{3}: {4}\r\n{5}: {6:0.00}{7}{8}";
                            string _infotext = string.Format(_infoFormat,
                                "Стан фіскальної транзакції",
                                "Чек", ((byte)transinfo[0] == 1) ? "відкритий" : "не відкритий",
                                "Лічильник фіскальних чеків", transinfo[1],
                                "Сума по чеку", (double)transinfo[2],
                                (transinfo[3] == null) ? "" : ("\r\nОплата по чеку: " + string.Format("{0:0.00}", (double)transinfo[3])),
                                (transinfo[4] == null) ? "" : ("\r\nОстання оплата виконана " + ((byte)transinfo[4] == 1 ? "успішно" : "не успішно")));
                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = transinfo.Clone();
                        }

                        break;
                    }
                case "GetDiagInfo":
                    {
                        string[] dinfo = GetDiagInfo();
                        string _infoFormat = "{0}\r\n\r\n{1} - {2}\r\n{3}: {4}\r\n{5}: {6}\r\n{7}: {8}";
                        _infoFormat += "\r\n{9} - {10}\r\n{11}: {12}\r\n{13}: {14}";
                        string _infotext = string.Format(_infoFormat,
                            "Діагностична інформація",
                            "Версія ПЗ", dinfo[0],
                            "Дата час", DateTime.Parse(dinfo[1] + " " + dinfo[2].Insert(2, ":")).ToString("dd/MM/yyyy HH:mm"),
                            "Контрольна сума", dinfo[3],
                            "Перемикачі Sw", dinfo[4],
                            "Код країни", dinfo[5],
                            "Серійний номер", dinfo[6],
                            "Фіскальний номер", dinfo[7]);
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = dinfo.Clone();
                        break;
                    }
                case "GetTaxRates":
                    {
                        double[] taxs = GetTaxRates();
                        string _infotext = string.Format("{0}\r\n\r\n{1}: {2:0.00}%\r\n{3}: {4:0.00}%\r\n{5}: {6:0.00}%\r\n{7}: {8:0.00}%",
                            "Податкові ставки",
                            "А", taxs[0], "Б", taxs[1], "В", taxs[2], "Г", taxs[3]);
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = taxs.Clone();
                        break;
                    }
                case "GetTaxID":
                    {
                        string[] taxid = GetTaxID();
                        string _infotext = string.Format("{0}\r\n\r\n{1} {2}",
                            "Податковий ідентифікаційний номер", taxid[0], (taxid[1] == "0") ? "ПН" : "ІД");
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = taxid.Clone();
                        break;
                    }
                case "GetChqInfo":
                    {
                        object[] cinfo = GetChqInfo();
                        string _infoFormat = "{0}\r\n\r\n{1} - {2}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                        _infoFormat += "{7}: {8:0.00}\r\n{9}: {10:0.00}\r\n{11}: {12:0.00}\r\n{13} - {14}";
                        string _infotext = string.Format(_infoFormat,
                            "Інформація поточного фіскального чеку",
                            "Здійснене повернення", ((byte)cinfo[0] == 1) ? "Ні" : "Так",
                            "Сума по групі А", (double)cinfo[1],
                            "Сума по групі Б", (double)cinfo[2],
                            "Сума по групі В", (double)cinfo[3],
                            "Сума по групі Г", (double)cinfo[4],
                            "Сума по групі *", (double)cinfo[5],
                            "Розширений чек", ((byte)cinfo[6] == 1) ? "Так" : "Ні");
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = cinfo.Clone();
                        break;
                    }
                case "GetPaymentInfo":
                    {
                        object[] pinfo = GetPaymentInfo();
                        string _infoFormat = "{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}\r\n";
                        _infoFormat += "{7}: {8:0.00}\r\n{9} - {10}\r\n{11} - {12}\r\n{13} - {14}";
                        string _infotext = string.Format(_infoFormat,
                            "Інформація по типах розрахунків",
                            "Готівка", (double)pinfo[0],
                            "Кредит", (double)pinfo[1],
                            "Дебетна картка", (double)pinfo[2],
                            "Чек", (double)pinfo[3],
                            "Номер останньго Z-звіту", (uint)pinfo[4],
                            "Номер наступного Z-звіту", (uint)pinfo[5],
                            "Глобальний лічильник чеків", (uint)pinfo[6]);
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = pinfo.Clone();
                        break;
                    }
                case "GetUserInfo":
                    {
                        GetUserInfo gui = new GetUserInfo(Name, description);
                        if (gui.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] uinfo = GetUserInfo(gui.UserNo);
                            string _infoFormat = "{0}\r\n\r\n{1} - {2}\r\n{3}: [{4}]; {5:0.00}\r\n{6}: [{7}]; {8:0.00}\r\n";
                            _infoFormat += "{9}: [{10}]; {11:0.00}\r\n{12}: [{13}]; {14:0.00}\r\n{15}: [{16}]; {17:0.00}\r\n{18}: {19}";
                            string _infotext = string.Format(_infoFormat,
                                "Інформація про касира",
                                "Реалізовано чеків", (uint)uinfo[0],
                                "Кількість і сума продажів", (uint)uinfo[1], (double)uinfo[2],
                                "Кількість і сума скидок", (uint)uinfo[3], (double)uinfo[4],
                                "Кількість і сума надбаваок", (uint)uinfo[5], (double)uinfo[6],
                                "Кількість і сума чеків поверення", (uint)uinfo[7], (double)uinfo[8],
                                "Інше", (uint)uinfo[9], (double)uinfo[10],
                                "Ідентифікатор", uinfo[11].ToString());
                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = uinfo.Clone();
                        }
                        gui.Dispose();

                        break;
                    }
                case "GetLastFChqNo":
                    {
                        uint cno = GetLastFChqNo();
                        string _infotext = string.Format("{0} {1}", "Номер останнього фіскального чеку:", cno);
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = cno;
                        break;
                    }
                case "GetFixMem":
                    {
                        GetFixMem gfm = new GetFixMem(Name, description);
                        if (gfm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] fmem = GetFixMem(gfm.FixNo, gfm.ReturnType, gfm.SecondaryParam);
                            string _infoFormat = "{0}\r\n\r\n{1} {2}\r\n{3}: {4}\r\n{5}: {6}\r\n{7}: {8}\r\n{9} {10}";
                            string _infotext = string.Format(_infoFormat,
                                "Інформація фіскальної пам'яті",
                                "Чек", fmem[0],
                                "Лічильник фіскальних чеків", fmem[1],
                                "Сума по чеку", fmem[3],
                                "Оплата по чеку", fmem[4],
                                "Остання оплата виконана", fmem[5]);
                            System.Windows.Forms.MessageBox.Show(_infotext, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);

                            value = fmem.Clone();
                        }
                        break;
                    }
                case "GetRemTime":
                    {
                        object[] remtime = GetRemTime();
                        string _infotext = string.Format("{0}", "Інтервал часу до завершення зміни");
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        value = remtime.Clone();
                        break;
                    }
                #endregion

                /*
                 * Custom Methods
                 * 
                 */
                #region Custom Methods

                case "CustomGetBoxMoney":
                    {
                        double[] money = SetGetMoney(0.0);
                        string _infotext = string.Format("{0}\r\n\r\n{1}: {2:0.00}\r\n{3}: {4:0.00}\r\n{5}: {6:0.00}",
                            "Фінансова звітнсть",
                            "Загальна сума внеску за день", money[2],
                            "Загальна сума вилучення за день", money[3],
                            "Сума в касі", money[1]);
                        System.Windows.Forms.MessageBox.Show(_infotext, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                #endregion

                /*
                 * There are special methods for accessing from main app and
                 * custom methods without custom their implementation. (using built-in parameters)
                 */
                #region Program access methods
                case "FP_Sale":
                    {
                        FP_Sale(param);
                        break;
                    }
                case "FP_PayMoney":
                    {
                        FP_PayMoney(param);
                        break;
                    }
                case "FP_Payment":
                    {
                        FP_Payment(param);
                        break;
                    }
                case "FP_Discount":
                    {
                        FP_Discount(param);
                        break;
                    }
                case "FP_LastChqNo":
                    {
                        value = FP_LastChqNo(param);
                        break;
                    }
                case "FP_LastZRepNo":
                    {
                        value = FP_LastZRepNo(param);
                        break;
                    }
                case "FP_OpenBox":
                    {
                        FP_OpenBox();
                        break;
                    }
                case "FP_SetCashier":
                    {
                        value = FP_SetCashier(param);
                        break;
                    }
                case "FP_SendCustomer":
                    {
                        FP_SendCustomer(param);
                        break;
                    }
                #endregion
                #region Custom methods
                case "Custom_ReportX":
                    {
                        ReportX rxz = new ReportX(Name, description);
                        if (rxz.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] xzInfo = ReportXZ(rxz.Password, rxz.ReportType, new bool[] { rxz.ClearUserSumm, rxz.ClearArtsSumm });
                            value = xzInfo.Clone();
                        }
                        rxz.Dispose();
                        break;
                    }
                case "Custom_ReportZ":
                    {
                        ReportZ rxz = new ReportZ(Name, description);
                        if (rxz.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            object[] xzInfo = ReportXZ(rxz.Password, rxz.ReportType, new bool[] { rxz.ClearUserSumm, rxz.ClearArtsSumm });
                            value = xzInfo.Clone();
                            SetGetArticle('D', "A,0000");
                            Params.DriverData["LastArtNo"] = (uint)1;
                            this.param.Save();
                        }
                        rxz.Dispose();
                        break;
                    }
                #endregion
            }
            return value;
        }

        // Driver implementation
        // Warning! Don't change this code region
        #region Driver implementation
        #region DayReport
        private object[] ReportXZ(string pwd, byte type, bool[] clearModes)
        {
            Params.DriverData["LastFunc"] = "ReportXZ";
            CMD = 69;

            // Creating data
            string _data = pwd + ',' + type;
            if (clearModes[0])
                _data += 'N';
            if (clearModes[1])
                _data += 'A';

            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _xzinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            object[] _xzData = new object[6];
            if (_xzinfo.Length != 0)
            {
                string[] _zxItems = _xzinfo.Split(new char[1] { ',' });
                _xzData[0] = uint.Parse(_zxItems[0]);
                _xzData[1] = double.Parse(_zxItems[1], Params.NumberFormat) / 100;
                _xzData[2] = double.Parse(_zxItems[2], Params.NumberFormat) / 100;
                _xzData[3] = double.Parse(_zxItems[3], Params.NumberFormat) / 100;
                _xzData[4] = double.Parse(_zxItems[4], Params.NumberFormat) / 100;
                _xzData[5] = double.Parse(_zxItems[5], Params.NumberFormat) / 100;
            }

            return _xzData;
        }
        #endregion
        #region Reports
        private void ReportByTax(string pwd, DateTime startDate, DateTime endDate)
        {
            Params.DriverData["LastFunc"] = "ReportByTax";
            CMD = 50;

            // Creating data
            string _data = pwd + ',';
            _data += startDate.ToString("ddMMyy") + ',';
            _data += endDate.ToString("ddMMyy");
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void ReportByNoFull(string pwd, uint startZNo, uint endZNo)
        {
            Params.DriverData["LastFunc"] = "ReportByNoFull";
            CMD = 73;

            // Creating data
            string _data = pwd + ',';
            _data += string.Format("{0:0000}", startZNo) + ',';
            _data += string.Format("{0:0000}", endZNo);
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void ReportByDateFull(string pwd, DateTime startDate, DateTime endDate)
        {
            Params.DriverData["LastFunc"] = "ReportByDateFull";
            CMD = 94;

            // Creating data
            string _data = pwd + ',';
            _data += startDate.ToString("ddMMyy") + ',';
            _data += endDate.ToString("ddMMyy");
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void ReportByDateShort(string pwd, DateTime startDate, DateTime endDate)
        {
            Params.DriverData["LastFunc"] = "ReportByDateShort";
            CMD = 79;

            // Creating data
            string _data = pwd + ',';
            _data += startDate.ToString("ddMMyy") + ',';
            _data += endDate.ToString("ddMMyy");
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void ReportByNoShort(string pwd, uint startZNo, uint endZNo)
        {
            Params.DriverData["LastFunc"] = "ReportByNoShort";
            CMD = 95;

            // Creating data
            string _data = pwd + ',';
            _data += string.Format("{0:0000}", startZNo) + ',';
            _data += string.Format("{0:0000}", endZNo);
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void ReportByUsers(string pwd)
        {
            Params.DriverData["LastFunc"] = "ReportUsers";
            CMD = 105;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(pwd);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void ReportByArts(string pwd, string mode)
        {
            Params.DriverData["LastFunc"] = "ReportArts";
            CMD = 111;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(pwd + ',' + mode);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Selling
        private ushort[] OpenNOrder()
        {
            Params.DriverData["LastFunc"] = "OpenNOrder";
            CMD = 38;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _noinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            ushort[] _noinf = new ushort[4];
            if (_noinfo.Length != 0)
            {
                string[] _noItems = _noinfo.Split(new char[1] { ',' });
                _noinf[0] = ushort.Parse(_noItems[0]);
                _noinf[1] = ushort.Parse(_noItems[1]);
                _noinf[2] = ushort.Parse(_noItems[2]);
                if (_noItems.Length == 4)
                    _noinf[3] = ushort.Parse(_noItems[3]);
            }
            return _noinf;
        }
        private ushort[] CloseNOrder()
        {
            Params.DriverData["LastFunc"] = "CloseNOrder";
            CMD = 39;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _noinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            ushort[] _noinf = new ushort[4];
            if (_noinfo.Length != 0)
            {
                string[] _noItems = _noinfo.Split(new char[1] { ',' });
                _noinf[0] = ushort.Parse(_noItems[0]);
                _noinf[1] = ushort.Parse(_noItems[1]);
                _noinf[2] = ushort.Parse(_noItems[2]);
                if (_noItems.Length == 4)
                    _noinf[3] = ushort.Parse(_noItems[3]);
            }

            return _noinf;
        }
        private void PrintNText(string ntext)
        {
            Params.DriverData["LastFunc"] = "PrintNText";
            CMD = 42;

            // Creating data
            ntext = ntext.Replace('і', 'i').Replace('І', 'I');
            DataForSend = Encoding.GetEncoding(1251).GetBytes(ntext);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private ushort[] OpenFOrder(byte uid, string pwd, byte cdno, bool extended)
        {
            Params.DriverData["LastFunc"] = "OpenFOrder";
            CMD = 48;

            // Creating data
            string _order = uid.ToString() + ',' + pwd + ',' + cdno + (extended ? ",I" : "");
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_order);
         
            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _foinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            ushort[] _foinf = null;
            if (_foinfo.Length != 0)
            {
                string[] _foItems = _foinfo.Split(new char[1] { ',' });
                _foinf = new ushort[3];
                _foinf[0] = ushort.Parse(_foItems[0]);
                _foinf[1] = ushort.Parse(_foItems[1]);
                _foinf[2] = ushort.Parse(_foItems[2]);
                if (_foItems.Length == 4)
                    _foinf[3] = ushort.Parse(_foItems[3]);
            }
           
            return _foinf;
        }
        private double[] SubTotal(bool print, bool indicate, double disc, bool discmode)
        {
            Params.DriverData["LastFunc"] = "SubTotal";
            CMD = 51;

            // Creating data
            string ddata = (print ? "1" : "0") + (indicate ? "1" : "0");
            if (disc != 0.0)
            {
                ddata += discmode ? ',' : ';';
                ddata += disc.ToString("+0.00;-0.00;0.00", Params.NumberFormat);
            }

            DataForSend = Encoding.GetEncoding(1251).GetBytes(ddata);

            // Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            return new double[0];
        }
        private void SaleArtIndicate(char sign, uint artno, double tot, double nprice, double disc, bool discmode)
        {
            Params.DriverData["LastFunc"] = "SaleArtIndicate";
            CMD = 52;

            // Creating data
            string sdata = artno.ToString();
            sdata += string.Format(Params.NumberFormat, "*{0:0.000}", tot);
            if (disc != 0.0)
            {
                sdata += discmode ? ',' : ';';
                sdata += ',' + string.Format("{0;+00.00;-00.00;0.0}", disc);
            }

            DataForSend = Encoding.GetEncoding(1251).GetBytes(sdata);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private object[] Total(string text, char pmode, double money)
        {
            Params.DriverData["LastFunc"] = "Total";
            CMD = 53;

            // Creating data
            string _total = string.Empty;
            if (text != string.Empty)
            {
                if (text.Length > 25)
                    text = text.Insert(25, (char)0x0A + "");
                _total = text;
            }
            _total += (char)0x09 + pmode.ToString() + string.Format(Params.NumberFormat, "{0:0.00}", money);

            DataForSend = Encoding.GetEncoding(1251).GetBytes(_total);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _trez = Encoding.GetEncoding(1251).GetString(OutputData);
            object[] _tinfo = new object[2];
            if (_trez.Length != 0)
            {
                _tinfo[0] = _trez[0];
                if (_trez.Length > 2)
                    _tinfo[1] = double.Parse(_trez.Substring(1), Params.NumberFormat) / 100;
            }

            return _tinfo;
        }
        private void PrintFText(string ftext)
        {
            Params.DriverData["LastFunc"] = "PrintFText";
            CMD = 54;

            //Creating data
            ftext = ftext.Replace('і', 'i').Replace('І', 'I');
            DataForSend = Encoding.GetEncoding(1251).GetBytes(ftext);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private object[] TotalAndClose(string text, char pmode, double money)
        {
            // Supported in version 1.31
            return new object[0];
        }
        private ushort[] CloseFOrder()
        {
            Params.DriverData["LastFunc"] = "CloseFOrder";
            CMD = 56;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

             // Perform processing recived data
            string _foinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            ushort[] _foinf = new ushort[4];
            if (_foinfo.Length != 0)
            {
                string[] _foItems = _foinfo.Split(new char[1] { ',' });
                _foinf[0] = ushort.Parse(_foItems[0]);
                _foinf[1] = ushort.Parse(_foItems[1]);
                _foinf[2] = ushort.Parse(_foItems[2]);
                if (_foItems.Length == 4)
                    _foinf[3] = ushort.Parse(_foItems[3]);
            }

            return _foinf;
        }
        private void ResetOrder()
        {
            Params.DriverData["LastFunc"] = "ResetOrder";
            CMD = 57;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void SaleArt(char sign, uint artno, double tot, double nprice, double disc, bool discmode)
        {
            Params.DriverData["LastFunc"] = "SaleArt";
            CMD = 58;

            //Creating data
            string sdata = artno.ToString();
            sdata += string.Format(Params.NumberFormat, "*{0:0.000}", tot);
            if (disc != 0.0)
            {
                sdata += discmode ? ',' : ';';
                sdata += ',' + string.Format("{0;+00.00;-00.00;0.0}", disc);
            }

            DataForSend = Encoding.GetEncoding(1251).GetBytes(sdata);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void GroupDiscount()
        {
            // Supported in version 1.31
        }
        private ushort[] OpenROrder(byte uid, string pwd, byte cdno, bool extended)
        {
            Params.DriverData["LastFunc"] = "OpenROrder";
            CMD = 85;

            // Creating data
            string _order = uid.ToString() + ',' + pwd + ',' + cdno + (extended ? ",I" : "");
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_order);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _foinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            ushort[] _foinf = new ushort[3];
            if (_foinfo.Length != 0)
            {
                string[] _foItems = _foinfo.Split(new char[1] { ',' });
                _foinf[0] = ushort.Parse(_foItems[0]);
                _foinf[1] = ushort.Parse(_foItems[1]);
                _foinf[2] = ushort.Parse(_foItems[2]);
                if (_foItems.Length == 4)
                    _foinf[3] = ushort.Parse(_foItems[3]);
            }

            return _foinf;
        }
        private void PrintCopy(byte copies)
        {
            Params.DriverData["LastFunc"] = "PrintCopy";
            CMD = 109;

            // Creating data
            if (copies > 1)
            {
                DataForSend = new byte[1];
                DataForSend[0] = (byte)(copies + 0x30);
            }
            else
                DataForSend = new byte[0];

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Initialization
        private void SetPrintParams(string value, string text)
        {
            Params.DriverData["LastFunc"] = "SetPrintParams";
            CMD = 43;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(value + text);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void SetDateTime(DateTime dt)
        {
            Params.DriverData["LastFunc"] = "SetDateTime";
            CMD = 61;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(dt.ToString("dd-MM-yy HH:mm:ss"));

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }

        private object[] SetTaxRate(string pwd, byte dpoint, bool[] userates, double[] rates)
        {
            Params.DriverData["LastFunc"] = "SetTaxRate";
            CMD = 83;

            // Creating data
            string _data = pwd + ',' + dpoint.ToString() + ',';
            foreach (bool a in userates)
                _data += a ? '1' : '0';
            foreach (double r in rates)
                _data += ',' + r.ToString(Params.NumberFormat);

            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _taxdata = Encoding.GetEncoding(1251).GetString(OutputData);
            object[] _taxInfo = new object[9];
            if (_taxdata.Length != 0)
            {
                string[] _trItems = _taxdata.Split(new char[] { ',' });
                _taxInfo[0] = byte.Parse(_trItems[0]);
                _taxInfo[1] = _trItems[1][0] == '1' ? true : false;
                _taxInfo[2] = _trItems[1][1] == '1' ? true : false; ;
                _taxInfo[3] = _trItems[1][2] == '1' ? true : false; ;
                _taxInfo[4] = _trItems[1][3] == '1' ? true : false; ;

                _taxInfo[5] = double.Parse(_trItems[2], Params.NumberFormat);
                _taxInfo[6] = double.Parse(_trItems[3], Params.NumberFormat);
                _taxInfo[7] = double.Parse(_trItems[4], Params.NumberFormat);
                _taxInfo[8] = double.Parse(_trItems[5], Params.NumberFormat);
            }

            return _taxInfo;
        }
        private void SetSaleMode(byte mode)
        {
            Params.DriverData["LastFunc"] = "SetSaleMode";
            CMD = 84;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(mode.ToString());

            // Preparing data
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private char SetSerialNum(byte ccode, string snum)
        {
            Params.DriverData["LastFunc"] = "SetSerialNum";
            CMD = 91;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(ccode + ',' + snum);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            char _fok = '\0';
            string _fdata = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fdata.Length != 0)
                _fok = _fdata[0];

            return _fok;
        }
        private char SetFixNum(string fnum)
        {
            Params.DriverData["LastFunc"] = "SetFixNum";
            CMD = 92;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(fnum);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            char _fok = '\0';
            string _fdata = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fdata.Length != 0)
                _fok = _fdata[0];

            return _fok;
        }
        private char SetTaxNum(string tnum, char ttype)
        {
            Params.DriverData["LastFunc"] = "SetTaxNum";
            CMD = 98;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(tnum + ',' + ttype);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            char _fok = '\0';
            string _fdata = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fdata.Length != 0)
                _fok = _fdata[0];

            return _fok;
        }
        private void SetUserPass(string opwd, string npwd, byte uid)
        {
            Params.DriverData["LastFunc"] = "SetUserPwd";
            CMD = 101;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(uid + "," + opwd + "," + npwd);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void SetUserName(string pwd, byte uid, string name)
        {
            Params.DriverData["LastFunc"] = "SetUserName";
            CMD = 102;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(uid + "," + pwd + "," + name);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void ResetUserData(string pwd, byte uid)
        {
            Params.DriverData["LastFunc"] = "ResetUserData";
            CMD = 104;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(uid + "," + pwd);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private string[] SetGetArticle(char option, params object[] param)
        {
            Params.DriverData["LastFunc"] = "SetGetArticle";
            CMD = 107;

            // Creating data
            string _data = option.ToString();
            if (!func.IsEmpty(param))
                for (int i = 0; i < param.Length; i++)
                    _data += param[i].ToString();

            DataForSend = Encoding.GetEncoding(1251).GetBytes(_data);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            //Perform processing recived data
            string _art = Encoding.GetEncoding(1251).GetString(OutputData);
            string[] artInfo = new string[0];

            if (_art.Length != 0)
            {
                _art = _art.Insert(1, ",");
                artInfo = _art.Split(new char[1] { ',' });
            }

            return artInfo;
        }
        private void LoadLogo(string pwd, byte[][] logo)
        {
            Params.DriverData["LastFunc"] = "LoadLogo";
            CMD = 115;

            byte[] _predata = new byte[0];

            // converting image data
            byte[][] newLogo = new byte[96][];
            byte[] db = new byte[2];
            for (int bc = 0; bc < logo.Length; bc++)
            {
                newLogo[bc] = new byte[108];
                for (int j = 0, bccc = 0; j < logo[bc].Length; j++)
                {
                    db = Encoding.Default.GetBytes(logo[bc][j].ToString("x2").ToUpper());
                    newLogo[bc][bccc] = db[0];
                    bccc++;
                    newLogo[bc][bccc] = db[1];
                    bccc++;
                }
            }

            for (int _r = 0; _r < newLogo.Length; _r++)
            {
                // Creating data
                _predata = Encoding.GetEncoding(1251).GetBytes(pwd + ',' + _r + ',');

                DataForSend = new byte[_predata.Length + newLogo[_r].Length];
                Array.Copy(_predata, 0, DataForSend, 0, _predata.Length);
                Array.Copy(newLogo[_r], 0, DataForSend, _predata.Length, newLogo[_r].Length);

                // Converting data to specific format
                InputData = CreateInputData(CMD, DataForSend);

                // Sending and reciving data
                SendGetData(20, true);

                // Next code for command
                GetNextCmdCode();
            }

        }
        private void SetAdminPass(string opwd, string npwd)
        {
            Params.DriverData["LastFunc"] = "SetAdminPass";
            CMD = 118;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(opwd + "," + npwd);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void ResetUserPass(string pwd, byte uid)
        {
            Params.DriverData["LastFunc"] = "ResetUserPass";
            CMD = 119;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(uid + "," + pwd);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Get info
        private DateTime GetDateTime()
        {
            Params.DriverData["LastFunc"] = "GetDateTime";
            CMD = 62;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _dtime = Encoding.GetEncoding(1251).GetString(OutputData);
            return DateTime.Parse(_dtime);
        }
        private object[] GetLastZReport(byte type)
        {
            Params.DriverData["LastFunc"] = "GetLastZReport";
            CMD = 64;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(type.ToString());

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            object[] _zrep = new object[6];
            string _zrepInfo = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_zrepInfo.Length != 0)
            {
                string[] _zrepItems = _zrepInfo.Split(new char[] { ',' });
                _zrep[0] = uint.Parse(_zrepItems[0]);
                _zrep[1] = double.Parse(_zrepItems[1], Params.NumberFormat);
                _zrep[2] = double.Parse(_zrepItems[2], Params.NumberFormat);
                _zrep[3] = double.Parse(_zrepItems[3], Params.NumberFormat);
                _zrep[4] = double.Parse(_zrepItems[4], Params.NumberFormat);
                _zrep[5] = double.Parse(_zrepItems[5], Params.NumberFormat);
            }

            return _zrep;
        }
        private double[] GetSummsByDay(byte type)
        {
            Params.DriverData["LastFunc"] = "GetSummsByDay";
            CMD = 65;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(type.ToString());

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            double[] _summ = new double[6];
            string _summInfo = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_summInfo.Length != 0)
            {
                string[] _summItems = _summInfo.Split(new char[] { ',' });
                _summ[0] = double.Parse(_summItems[0], Params.NumberFormat) / 100;
                _summ[1] = double.Parse(_summItems[1], Params.NumberFormat) / 100;
                _summ[2] = double.Parse(_summItems[2], Params.NumberFormat) / 100;
                _summ[3] = double.Parse(_summItems[3], Params.NumberFormat) / 100;
                _summ[4] = double.Parse(_summItems[4], Params.NumberFormat) / 100;
            }

            return _summ;
        }
        private object[] GetSumCorByDay()
        {
            Params.DriverData["LastFunc"] = "GetSumCorByDay";
            CMD = 67;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            object[] _scorr = new object[6];
            string _corrdata = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_corrdata.Length != 0)
            {
                string[] _corrItems = _corrdata.Split(new char[1] { ',' });
                _scorr[0] = double.Parse(_corrItems[0]) / 100;
                _scorr[1] = double.Parse(_corrItems[1]) / 100;
                _scorr[2] = double.Parse(_corrItems[2]) / 100;
                _scorr[3] = ushort.Parse(_corrItems[3]);
                _scorr[4] = ushort.Parse(_corrItems[4]);
                _scorr[5] = ushort.Parse(_corrItems[5]);
            }

            return _scorr;
        }
        private uint GetFreeMem()
        {
            Params.DriverData["LastFunc"] = "GetFreeMem";
            CMD = 68;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _fmem = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fmem.Length != 0)
                _fmem = _fmem.Split(new char[1] { ',' })[0];

            return uint.Parse(_fmem);
        }
        private byte[] GetState()
        {
            Params.DriverData["LastFunc"] = "GetState";
            CMD = 74;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            byte[] _stat = new byte[48];
            byte _sIdx = 0;
            byte _lIdx = 0;
            byte _mask = 0x80;
            for (int _i = 0, _j = 0; _i < OutputData.Length; _i++)
            {
                for (_j = 0; _j < 8; _j++)
                {
                    if ((OutputData[_i] & _mask) != 0)
                    {
                        _stat[_lIdx] = _sIdx;
                        _lIdx++;
                    }
                    _mask >>= 1;
                    _sIdx++;
                }
                _mask = 0x80;
            }

            return _stat;
        }
        private object[] GetFixTransState(char param)
        {
            Params.DriverData["LastFunc"] = "GetFixTransState";
            CMD = 76;

            // Creating data
            if (param != 0)
            {
                DataForSend = new byte[1];
                DataForSend[0] = (byte)param;
            }
            else
                DataForSend = new byte[0];

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _trans = Encoding.GetEncoding(1251).GetString(OutputData);
            object[] _tInfo = new object[5];
            if (_trans.Length != 0)
            {
                string[] _tItems = _trans.Split(new char[1] { ',' });
                _tInfo[0] = byte.Parse(_tItems[0]);
                _tInfo[1] = ushort.Parse(_tItems[1]);
                _tInfo[2] = uint.Parse(_tItems[2]) / (double)100;
                if (_tItems.Length == 4)
                    _tInfo[3] = uint.Parse(_tItems[3]) / (double)100;
                if (_tItems.Length == 5)
                    _tInfo[4] = byte.Parse(_tItems[4]);
            }

            return _tInfo;
        }
        private string[] GetDiagInfo()
        {
            Params.DriverData["LastFunc"] = "GetDiagInfo";
            CMD = 90;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _dinfo = Encoding.GetEncoding(1251).GetString(OutputData);

            return _dinfo.Split(new char[2] { ',', ' ' });
        }
        private double[] GetTaxRates()
        {
            Params.DriverData["LastFunc"] = "GetTaxRates";
            CMD = 97;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            double[] _taxrates = new double[4];
            string _taxs = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_taxs.Length != 0)
            {
                string[] _taxItem = _taxs.Split(new char[1] { ',' });
                _taxrates[0] = double.Parse(_taxItem[0], Params.NumberFormat); //A
                _taxrates[1] = double.Parse(_taxItem[1], Params.NumberFormat); //B
                _taxrates[2] = double.Parse(_taxItem[2], Params.NumberFormat); //V
                _taxrates[3] = double.Parse(_taxItem[3], Params.NumberFormat); //G
            }

            return _taxrates;
        }
        private string[] GetTaxID()
        {
            Params.DriverData["LastFunc"] = "GetTaxID";
            CMD = 99;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _taxid = Encoding.GetEncoding(1251).GetString(OutputData);
            return _taxid.Split(new char[1] { ',' });
        }
        private object[] GetChqInfo()
        {
            Params.DriverData["LastFunc"] = "GetChqInfo";
            CMD = 103;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            object[] _chqinfo = new object[7];
            string _cinfo = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_cinfo.Length != 0)
            {
                string[] _infoItem = _cinfo.Split(new char[1] { ',' });
                _chqinfo[0] = byte.Parse(_infoItem[0]);
                for (int _i = 1; _i < _infoItem.Length - 1; _i++)
                    _chqinfo[_i] = double.Parse(_infoItem[_i], Params.NumberFormat) / 100;
                _chqinfo[6] = byte.Parse(_infoItem[6]);
            }

            return _chqinfo;
        }
        private object[] GetPaymentInfo()
        {
            Params.DriverData["LastFunc"] = "GetPaymentInfo";
            CMD = 110;

            // Creating data
            DataForSend = new byte[0];

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            object[] _payment = new object[7];
            string _paymentInfo = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_paymentInfo.Length != 0)
            {
                string[] _paymentItems = _paymentInfo.Split(new char[1] { ',' });
                _payment[0] = double.Parse(_paymentItems[0], Params.NumberFormat) / 100;
                _payment[1] = double.Parse(_paymentItems[1], Params.NumberFormat) / 100;
                _payment[2] = double.Parse(_paymentItems[2], Params.NumberFormat) / 100;
                _payment[3] = double.Parse(_paymentItems[3], Params.NumberFormat) / 100;
                _payment[4] = uint.Parse(_paymentItems[_paymentItems.Length - 3]);
                _payment[5] = uint.Parse(_paymentItems[_paymentItems.Length - 2]);
                _payment[6] = uint.Parse(_paymentItems[_paymentItems.Length - 1]);
            }

            return _payment;
        }
        private object[] GetUserInfo(byte uid)
        {
            Params.DriverData["LastFunc"] = "GetUserInfo";
            CMD = 112;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(uid.ToString());

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            object[] _user = new object[12];
            string _userInfo = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_userInfo.Length != 0)
            {
                string[] _userItems = _userInfo.Split(new char[2] { ',', ';' });
                _user[0] = uint.Parse(_userItems[0]);
                _user[1] = uint.Parse(_userItems[1]);
                _user[2] = double.Parse(_userItems[2], Params.NumberFormat);
                _user[3] = uint.Parse(_userItems[3]);
                _user[4] = double.Parse(_userItems[4], Params.NumberFormat);
                _user[5] = uint.Parse(_userItems[5]);
                _user[6] = double.Parse(_userItems[6], Params.NumberFormat);
                _user[7] = uint.Parse(_userItems[7]);
                _user[8] = double.Parse(_userItems[8], Params.NumberFormat);
                _user[9] = uint.Parse(_userItems[9]);
                _user[10] = double.Parse(_userItems[10], Params.NumberFormat);
                _user[11] = _userItems[11];
            }

            return _user;
        }
        private uint GetLastFChqNo()
        {
            Params.DriverData["LastFunc"] = "GetLastChqNo";
            CMD = 113;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _chqno = Encoding.GetEncoding(1251).GetString(OutputData);
            return uint.Parse(_chqno);
        }
        private object[] GetFixMem(string fno, byte type, string sparam)
        {
            Params.DriverData["LastFunc"] = "GetFixMem";
            CMD = 114;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(fno + ',' + type + ((sparam.Length != 0) ? (',' + sparam) : ""));

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _fmem = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fmem.Length != 0)
                _fmem = _fmem.Split(new char[1] { ',' })[0];

            return new object[0];
        }
        private object[] GetRemTime()
        {
            Params.DriverData["LastFunc"] = "GetRemTime";
            CMD = 46;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _fmem = Encoding.GetEncoding(1251).GetString(OutputData);
            if (_fmem.Length != 0)
                _fmem = _fmem.Split(new char[1] { ',' })[0];

            return new object[0];
        }
        #endregion
        #region Printer commads
        private void CutChq()
        {
            Params.DriverData["LastFunc"] = "CutChq";
            CMD = 45;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void LineFeed(byte lines)
        {
            Params.DriverData["LastFunc"] = "LineFeed";
            CMD = 44;

            // Creating data
            DataForSend = Encoding.GetEncoding(1251).GetBytes(lines.ToString());

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Display
        private void ClrDispl()
        {
            Params.DriverData["LastFunc"] = "ClrDispl";
            CMD = 33;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void DisplBotLine(string line)
        {
            Params.DriverData["LastFunc"] = "DisplBotLine";
            CMD = 35;

            // Creating data
            line = line.Replace('і', 'i').Replace('І', 'I');
            DataForSend = Encoding.GetEncoding(1251).GetBytes(line);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void DisplTopLine(string line)
        {
            Params.DriverData["LastFunc"] = "DisplTopLine";
            CMD = 47;

            // Creating data
            line = line.Replace('і', 'i').Replace('І', 'I');
            DataForSend = Encoding.GetEncoding(1251).GetBytes(line);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void DisplayDateTime()
        {
            Params.DriverData["LastFunc"] = "DisplayDateTime";
            CMD = 63;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void DisplText(string text)
        {
            Params.DriverData["LastFunc"] = "DisplText";
            CMD = 100;

            // Creating data
            text = text.Replace('і', 'i').Replace('І', 'I');
            DataForSend = Encoding.GetEncoding(1251).GetBytes(text);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Other
        private double[] SetGetMoney(double amount)
        {
            Params.DriverData["LastFunc"] = "SetGetMoney";
            CMD = 70;

            // Creating data
            string _amount = amount.ToString(Params.NumberFormat);
            if (amount > 0)
                _amount = "+" + _amount;
            DataForSend = Encoding.GetEncoding(1251).GetBytes(_amount);

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();

            // Perform processing recived data
            string _rdata = Encoding.GetEncoding(1251).GetString(OutputData);
            double[] cashInfo = new double[4];
            if (_rdata.Length != 0)
            {
                string[] _explStr = _rdata.Split(new char[] { ',' });
                if (_explStr[0] != "F")
                {
                    cashInfo[1] = double.Parse(_explStr[1]) / 100;
                    cashInfo[2] = double.Parse(_explStr[2]) / 100;
                    cashInfo[3] = double.Parse(_explStr[3]) / 100;
                }
            }

            return cashInfo;
        }
        private void PrintDiagInfo()
        {
            Params.DriverData["LastFunc"] = "PrintDiagInfo";
            CMD = 71;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void OpenBox(byte impulse)
        {
            Params.DriverData["LastFunc"] = "OpenBox";
            CMD = 106;

            // Creating data
            if (impulse != 0)
                DataForSend = Encoding.GetEncoding(1251).GetBytes(impulse.ToString());
            else
                DataForSend = new byte[0];

            // Converting data to specific format
            InputData = CreateInputData(CMD, DataForSend);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        private void Beep()
        {
            Params.DriverData["LastFunc"] = "Beep";
            CMD = 80;

            // Converting data to specific format
            InputData = CreateInputData(CMD, new byte[0]);

            // Sending and reciving data
            SendGetData(20, true);

            // Next code for command
            GetNextCmdCode();
        }
        #endregion
        #endregion

        #region Custom Methods
        #endregion

        // Driver's Application Interface (Access Methods)
        // Need for access to FP from main program
        private void FP_Sale(object[] param)
        {
            // Return if parameters are empty
            if (func.IsEmpty(param))
                return;

            ClrDispl();

            // Check if function has errors
            if ((bool)Params.ErrorFlags["FP_Payment"])
                return;

            if ((bool)Params.ErrorFlags["FP_Sale"])
            {
                ResetOrder();
                Params.ErrorFlags["FP_Sale"] = false;
                this.param.Save();
            }

            Exception ex = null;
            // Try to perform commands
            try
            {
                // Local varibles
                System.Data.DataTable dTable = (System.Data.DataTable)param[0];
                byte ppt = Convert.ToByte(param[1]);
                bool useTotDisc = (bool)param[2];
                byte ppm = Convert.ToByte(param[3]);
                object[] article = new object[4];
                uint nextArticleNo = (uint)Params.DriverData["LastArtNo"];
                string[] _propArtStatus = new string[0];

                // Get last article id
                string[] _ainfo = SetGetArticle('L');
                if (!func.IsEmpty(_ainfo) && _ainfo[0] != "F")
                {
                    nextArticleNo = uint.Parse(_ainfo[1]);
                    nextArticleNo++;
                }
                else
                    for (; nextArticleNo < (uint)Params.DriverData["ArtMemorySize"]; nextArticleNo++)
                    {
                        _ainfo = SetGetArticle('R', nextArticleNo);
                        if (func.IsEmpty(_ainfo) || _ainfo[0] == "F")
                            break;
                    }

                // Check free memory for current sale
                if (nextArticleNo + dTable.Rows.Count >= (uint)Params.DriverData["ArtMemorySize"])
                {
                    System.Windows.Forms.MessageBox.Show("Немає вільної пам'яті для здійснення продажу\r\nНеохідно зробити Z-звіт для наступного продажу",
                        Name, System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }

                // Open fiscal order
                ushort[] _ofoRez = OpenFOrder((byte)Params.DriverData["UserNo"], Params.DriverData["UserPwd"].ToString(), (byte)Params.DriverData["DeskNo"], true);
                if (_ofoRez == null)
                    throw new Exception("Помилка відкриття чеку");
                
                // Program and sale each articles
                for (int i = 0; i < dTable.Rows.Count; i++)
                {
                    article[0] = dTable.Rows[i]["VG"];
                    article[1] = func.GetDouble(dTable.Rows[i]["PRICE"]).ToString(Params.NumberFormat);
                    article[2] = dTable.Rows[i]["DESC"].ToString();
                    article[3] = func.GetDouble(dTable.Rows[i]["TOT"]);

                    if (article[2].ToString().Length == 0)
                        article[2] = dTable.Rows[i]["NAME"].ToString();
                    if (article[2].ToString().Length > 24)
                        article[2] = article[2].ToString().Substring(0, 24);

                    _propArtStatus = SetGetArticle('P', article[0], nextArticleNo, ',', 1, ',', article[1], ',', "0000", ',', article[2]);

                    if (_propArtStatus[0] == "F")
                        throw new Exception("Помилка програмування товару");

                    if (useTotDisc)
                        SaleArt('\0', nextArticleNo, (double)article[3], double.Parse(article[1].ToString(), Params.NumberFormat), 0.0, false);
                    else
                        SaleArt('\0', nextArticleNo, (double)article[3], double.Parse(article[1].ToString(), Params.NumberFormat), (double)dTable.Rows[i]["DISC"], false);

                    nextArticleNo++;
                    //if (!useTotDisc && (bool)dTable.Rows[i]["USEDDISC"] && (double)dTable.Rows[i]["DISC"] != 0)
                    //FP_Discount((byte)0, (double)dTable.Rows[i]["DISC"], ppt, "");
                }

                Params.DriverData["LastArtNo"] = nextArticleNo;
                Params.ErrorFlags["FP_Sale"] = false;
                this.param.Save();
                return;
            }
            catch(Exception _ex)
            {
                Params.ErrorFlags["FP_Sale"] = true;
                ex = _ex;
            }

            this.param.Save();
            throw new Exception(ex.Message , ex);
        }
        private void FP_PayMoney(object[] param)
        {
            // Return if parameters are empty
            if (func.IsEmpty(param))
                return;

            // Check last executed function for error
            if ((bool)Params.ErrorFlags["FP_Payment"])
                return;

            if ((bool)Params.ErrorFlags["FP_PayMoney"])
            {
                ResetOrder();
                Params.ErrorFlags["FP_PayMoney"] = false;
                this.param.Save();
            }
            bool storeErrorState = false;
            Exception ex = null;
            // Try to perform commands
            try
            {
                System.Data.DataTable dTable = (System.Data.DataTable)param[0];
                double suma = 0;
                for (int i = 0; i < dTable.Rows.Count; i++)
                {
                    suma += double.Parse(dTable.Rows[i]["ASUM"].ToString());
                }

                double[] cashBoxInfo = SetGetMoney(0.0);

                if (cashBoxInfo == null)
                    throw new Exception("Неможливо отримати суму в грошовій скринці");
                else
                {
                    if (cashBoxInfo.Length >= 3 && cashBoxInfo[1] < suma)
                        throw new Exception("Недостатньо коштів для закриття видаткового чеку");
                }
                storeErrorState = true;
                // Local varibles
                byte ppt = Convert.ToByte(param[1]);
                bool useTotDisc = (bool)param[2];
                byte ppm = Convert.ToByte(param[3]);
                object[] article = new object[4];
                uint nexArticleNo = (uint)Params.DriverData["LastArtNo"];
                string[] _propArtStatus = new string[0];

                // Get last article id
                string[] _ainfo = SetGetArticle('L');
                if (!func.IsEmpty(_ainfo) && _ainfo[0] != "F")
                {
                    nexArticleNo = uint.Parse(_ainfo[1]);
                    nexArticleNo++;
                }
                else
                    for (; nexArticleNo < (uint)Params.DriverData["ArtMemorySize"]; nexArticleNo++)
                    {
                        _ainfo = SetGetArticle('R', nexArticleNo);
                        if (func.IsEmpty(_ainfo) || _ainfo[0] == "F")
                            break;
                    }

                // Check free memory for current sale
                if (nexArticleNo + dTable.Rows.Count >= (uint)Params.DriverData["ArtMemorySize"])
                {
                    System.Windows.Forms.MessageBox.Show("Немає вільної пам'яті для здійснення продажу\r\nНеохідно зробити Z-звіт для наступного продажу",
                        Name, System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }

                // Open fiscal order
                ushort[] _ofoRez = OpenROrder((byte)Params.DriverData["UserNo"], Params.DriverData["UserPwd"].ToString(), (byte)Params.DriverData["DeskNo"], true);
                if (_ofoRez == null)
                    throw new Exception("Помилка відкриття чеку");

                // Program and sale each articles
                for (int i = 0; i < dTable.Rows.Count; i++)
                {
                    article[0] = dTable.Rows[i]["VG"];
                    article[1] = func.GetDouble(dTable.Rows[i]["PRICE"]).ToString(Params.NumberFormat);
                    article[2] = dTable.Rows[i]["DESC"].ToString();
                    article[3] = func.GetDouble(dTable.Rows[i]["TOT"]);

                    if (article[2].ToString().Length == 0)
                        article[2] = dTable.Rows[i]["NAME"].ToString();
                    if (article[2].ToString().Length > 24)
                        article[2] = article[2].ToString().Substring(0, 24);

                    _propArtStatus = SetGetArticle('P', article[0], nexArticleNo, ',', 1, ',', article[1], ',', "0000", ',', article[2]);

                    if (_propArtStatus[0] == "F")
                        throw new Exception("Помилка програмування товару");

                    if (useTotDisc)
                        SaleArt('\0', nexArticleNo, (double)article[3], double.Parse(article[1].ToString(), Params.NumberFormat), 0.0, false);
                    else
                        SaleArt('\0', nexArticleNo, (double)article[3], double.Parse(article[1].ToString(), Params.NumberFormat), (double)dTable.Rows[i]["DISC"], false);

                    nexArticleNo++;
                    //if (!useTotDisc && (bool)dTable.Rows[i]["USEDDISC"] && (double)dTable.Rows[i]["DISC"] != 0)
                    //FP_Discount((byte)0, (double)dTable.Rows[i]["DISC"], ppt, "");
                }

                Params.ErrorFlags["FP_PayMoney"] = false;
                Params.DriverData["LastArtNo"] = nexArticleNo;
                this.param.Save();
                return;
            }
            catch (Exception _ex)
            {
                if (storeErrorState)
                    Params.ErrorFlags["FP_PayMoney"] = true;
                ex = _ex;
            }

            this.param.Save();

            throw new Exception(ex.Message, ex);
        }
        private void FP_Payment(object[] param)
        {
            // Return if parameters are empty
            if (func.IsEmpty(param))
                return;

            // Check last executed function for error
            if ((bool)Params.ErrorFlags["FP_Sale"] ||
                (bool)Params.ErrorFlags["FP_PayMoney"])
            {
                ResetOrder();
                Params.ErrorFlags["FP_Sale"] = false;
                Params.ErrorFlags["FP_PayMoney"] = false;
                this.param.Save();
                return;
            }

            // Try to perform commands
            Exception ex = null;
            try
            {
                // check if we have money to return
                //if ((bool)param[3])
                //{
                //}

                // Indicate payment type
                char pmode = 'P';
                switch ((byte)param[0])
                {
                    case 0:
                        {
                            pmode = 'D';
                            break;
                        }
                    case 1:
                        {
                            pmode = 'N';
                            break;
                        }
                    case 2:
                        {
                            pmode = 'C';
                            break;
                        }
                    case 3:
                        {
                            pmode = 'P';
                            break;
                        }
                }

                // 
                bool _canClose = false;

                // Detect if current order was paid
                object[] _tState = GetFixTransState('T');
                if (!func.IsEmpty(_tState) && _tState[2] != null && _tState[3] != null && (double)_tState[2] == (double)_tState[3])
                    _canClose = true;

                // Perform payment of that order
                if (!_canClose)
                {
                    object[] _pdata = Total("", pmode, (double)param[1]);
                        if (!func.IsEmpty(_pdata))
                        {
                            switch (_pdata[0].ToString())
                            {
                                case "F":
                                    {
                                        throw new Exception("Помилка під час закриття чеку [F]");
                                    }
                                case "E":
                                    {
                                        throw new Exception("Обрахована сума меньше 0.00. Закриття чеку не можливе. [E]");
                                    }
                                case "R":
                                    {
                                        _canClose = true;
                                        break;
                                    }
                                case "D":
                                    {
                                        if (_pdata[1] != null && (double)_pdata[1] == 0.0)
                                            _canClose = true;
                                        else
                                            throw new Exception("Сума чеку більша за внесені кошти. [D]");
                                        break;
                                    }
                                default:
                                    throw new Exception("Невизначена помилка під час закриття чеку.");
                            }
                        }
                        else
                            throw new Exception("Принтер не відповідає.");
                        //if (_pdata[0].ToString() == "R" ||
                        //    (_pdata[0].ToString() == "D" && (double)_pdata[1] == 0.0))
                        //        _canClose = true;
                }
                
                if (_canClose)
                {
                    // Perform closing order
                    ushort[] order = CloseFOrder();
                    // Get current order number
                    Params.DriverData["LastFOrderNo"] = GetLastFChqNo();
                }

                // Set current order number
                Params.ErrorFlags["FP_Payment"] = false;
                this.param.Save();
                return;
            }
            catch (Exception _ex)
            {
                Params.ErrorFlags["FP_Payment"] = true;
                ex = _ex;
            }

            this.param.Save();
            throw new Exception(ex.Message, ex);
        }
        private void FP_Discount(object[] param)
        {
            // Return if parameters are empty
            if (func.IsEmpty(param))
                return;

            // Check if function has errors
            if ((bool)Params.ErrorFlags["FP_Discount"])
            {
                ResetOrder();
                Params.ErrorFlags["FP_Discount"] = false;
				this.param.Save();
                return;
            }

            // Try to perform commands
            Exception ex = null;
            try
            {
                bool _discType = true;
                double _discValue = -(double)param[1];
                
                if ((byte)param[0] != 2)
                    _discType = false;

                double[] _d = SubTotal(true, false, _discValue, _discType);

                Params.ErrorFlags["FP_Discount"] = false;
                this.param.Save();
                return;
            }
            catch (Exception _ex)
            {
                Params.ErrorFlags["FP_Discount"] = true;
                ex = _ex;
            }

            this.param.Save();
            throw new Exception(ex.Message, ex);
        }
        private uint FP_LastChqNo(object[] param)
        {
            return (uint)Params.DriverData["LastFOrderNo"];
        }
        private uint FP_LastZRepNo(object[] param)
        {
            // Return if parameters are empty
            if (func.IsEmpty(param))
                return 0;

            object[] zRepInfo = GetPaymentInfo();

            if (func.IsEmpty(zRepInfo))
                zRepInfo[4] = (uint)0;

            return (uint)zRepInfo[4];
        }
        private void FP_OpenBox()
        {
            this.OpenBox(0);
        }
        private bool FP_SetCashier(object[] param)
        {
            bool rez = false;
            // Return if parameters are empty
            if (func.IsEmpty(param))
                return rez;

            // Try to perform commands
            try
            {
                byte _pdId = byte.Parse(param[0].ToString());
                byte _userNo = byte.Parse(param[1].ToString());
                string _userPwd = param[2].ToString();
                string _userId = param[3].ToString();

                if (_userPwd.Length < 4)
                    _userPwd = _userPwd.PadLeft(4, '0');

                Params.DriverData["DeskNo"] = _pdId;
                Params.DriverData["UserNo"] = _userNo;
                Params.DriverData["UserPwd"] = _userPwd;
                SetUserName(_userPwd, _pdId, _userId);
                rez = true;
            }
            catch { rez = false; }

            return rez;
        }
        private void FP_SendCustomer(object[] param)
        {
            // Return if parameters are empty
            if (func.IsEmpty(param))
                return;

            // Try to perform commands
            try
            {
                string[] _lines = (string[])param[0];
                bool[] _show = (bool[])param[1];

                if (_show[0])
                    this.DisplTopLine(_lines[0]);
                if (_show[1])
                    this.DisplBotLine(_lines[1]);
            }
            catch { }
        }

        // Base Methods
        // Implementation of them can be different for other drivers
        private void GetNextCmdCode()
        {
            SEQ++;
            if (SEQ >= 0x80)
                SEQ = 0x20;
        }
        private byte[] CreateInputData(byte _nom, byte[] param)
        {
            byte[] mas = new byte[4 + param.Length + 6];
            byte i = 0;
            byte[] bccData = new byte[] { 0x30, 0x30, 0x30, 0x30 };
            ushort bccItem = 0x0000;
            ushort bccItemMask = 0x000F;

            mas[0] = PRE;
            mas[1] = Convert.ToByte(0x20 + param.Length + 4);
            mas[2] = (byte)(SEQ + 0x20);
            mas[3] = CMD;

            for (i = 0; i < param.Length; i++)
                mas[i + 4] = param[i];

            mas[4 + param.Length] = POS;

            bccItem = (ushort)(mas[1] + mas[2] + _nom + func.UIntSumMas(param) + POS);
            for (i = 0; i < bccData.Length; i++)
                bccData[i] += (byte)((bccItem >> (i * 4)) & bccItemMask);

            mas[4 + param.Length + 1] = bccData[3];
            mas[4 + param.Length + 2] = bccData[2];
            mas[4 + param.Length + 3] = bccData[1];
            mas[4 + param.Length + 4] = bccData[0];
            mas[4 + param.Length + 4 + 1] = TER;

            return mas;
        }
        private bool SendGetData(int totRead, bool close)
        {
            string exceptionMsg = "";

            if (!port.IsOpen)
                try
                {
                    port.Open();
                }
                catch { }

            if (port.IsOpen && port.Write(InputData))
            {
                //WinAPI.OutputDebugString("W");

                int t = totRead;
                int b = totRead * 2;
                byte[] buffer = new byte[512];
                OutputData = new byte[0];
                do
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    //WinAPI.OutputDebugString("R");
                    if (port.Read(ref buffer, out ReadedBytes))
                    {
                        Array.Resize<byte>(ref OutputData, (int)(OutputData.Length + ReadedBytes));
                        Array.Copy(buffer, 0, OutputData, OutputData.Length - ReadedBytes, ReadedBytes);
                        try
                        {
                            //WinAPI.OutputDebugString("D");
                            switch (DecodeAnswer())
                            {
                                case "busy":
                                    {
                                        //WinAPI.OutputDebugString("S");
                                        if (b < 0)
                                            throw new Exception("Помилка виконання команди \"" + Params.DriverData["LastFunc"] + "\"");
                                        b--;
                                        Thread.Sleep(200);
                                        break;
                                    }
                                case "true":
                                    {
                                        //WinAPI.OutputDebugString("T");
                                        UpdateCriticalMethod(Params.DriverData["LastFunc"].ToString(), false);
                                        return true;
                                    }
                                case "false":
                                    {
                                        //WinAPI.OutputDebugString("F");
                                        t--;
                                        Thread.Sleep(50);
                                        break;
                                    }
                                case "failed":
                                    {
                                        throw new Exception("Помилка виконання команди \"" + Params.DriverData["LastFunc"] + "\"");
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            exceptionMsg = "\r\n" + ex.Message;
                            break;
                        }
                    }
                    else
                        break;

                } while (t > 0);
                //WinAPI.OutputDebugString("E");
            }

            if (close)
                port.Close();

            UpdateCriticalMethod(Params.DriverData["LastFunc"].ToString(), true);
            throw new Exception("Помилка читання з фіскального принтера" + " " + Name + exceptionMsg);
        }
        private string DecodeAnswer()
        {
            // if no recived data then set false state
            if (OutputData.Length == 0)
                return "false";
            // if message wsn't accepted then set failed state            
            if (OutputData[0] == NAK)
                return "failed";
            // remove all SYN symbols
            byte symbol = 0x00;
            byte[] normalizedAnswer = (byte[])(OutputData.Clone());
            bool preDetected = false;
            bool synDetected = false;
            for (byte i = 0; i < normalizedAnswer.Length; i++)
            {
                if (OutputData[i] == PRE && !preDetected) preDetected = true;
                if (OutputData[i] == SYN && !synDetected) synDetected = true;
                // if detected only SYN symbol
                if (!preDetected && synDetected)
                {
                    symbol++;
                    continue;
                }
                break;
            }
            // if not detected first char of message thet set busy state
            if (synDetected && !preDetected)
                return "busy";
            // remove all SYN symbols
            if (symbol != 0)
            {
                OutputData = new byte[normalizedAnswer.Length - symbol];
                Array.Copy(normalizedAnswer, symbol, OutputData, 0, OutputData.Length);
            }
            // if first char is message's char then translate this message
            if (OutputData[0] == PRE)
            {
                LEN = (byte)(OutputData[1] - 0x20);

                if (LEN + 6 > OutputData.Length)
                    return "busy";

                if (SEQ == (OutputData[2] - 0x20) && CMD == OutputData[3] && SEP == OutputData[1 + LEN - 1 - 6 - 1] && POS == OutputData[LEN])
                {
                    byte[] _data = new byte[LEN - 3 - 8];
                    byte[] _status = new byte[6];
                    byte _statusMask = 0x80;
                    byte _statusItem = 0x00;
                    ushort _resBcc = 0;
                    ushort _bcc = 0x0000;
                    byte midx = 0;
                    byte i = 0;
                    byte j = 0;

                    // get data
                    Array.Copy(OutputData, 4, _data, 0, _data.Length);
                    // get status bytes
                    Array.Copy(OutputData, LEN - 6, _status, 0, _status.Length);
                    // get recived checksum
                    for (i = 0, j = 3; i < 4; i++, j--)
                        _resBcc += (ushort)((OutputData[LEN + 1 + i] - 0x30) << j * 4);
                    // calculate checksum by recived message
                    _bcc = (ushort)(LEN + 0x20 + SEQ + 0x20 + CMD + func.UIntSumMas(_data) + SEP + func.UIntSumMas(_status) + POS);
                    // compare calculated and recived checksums
                    if (_bcc != _resBcc)
                        throw new Exception("Неправильна конрольна сума отриманого повідомлення");
                    // get status messages
                    Exception _msgCriticalStatus = new Exception();
                    string _criticalMsgs = string.Empty;
                    string _currMsgs = string.Empty;
                    try
                    {
                        string oper_info = string.Empty;
                        for (i = 0, j = 0, midx = 0; i < _status.Length; i++)
                        {
                            _statusItem = _status[i];
                            _statusMask = 0x80;
                            for (j = 0; j < 8; j++)
                            {
                                _currMsgs = ((string[])Params.DriverData["Status"])[midx];
                                if ((_statusItem & _statusMask) != 0 && _currMsgs.Length != 0)
                                {
                                    oper_info += _currMsgs + "\r\n";
                                    if (Array.IndexOf((int[])Params.DriverData["ErrorStatus"], (int)midx) >= 0)
                                        _criticalMsgs += _currMsgs + "\r\n";
                                }
                                _statusMask >>= 1;
                                midx++;
                            }
                            j = 0;
                        }
                    }
                    catch (Exception _ex) { _msgCriticalStatus = _ex; }

                    if (_criticalMsgs.Length != 0)
                        throw new Exception(_criticalMsgs.Trim());

                    OutputData = _data;
                    return "true";
                }
            }//if PRE

            return "false";
        }
        private void UpdateCriticalMethod(string fname, bool state)
        {
            if (Params.ErrorFlags.ContainsKey(fname))
                Params.ErrorFlags[fname] = state;
        }

        // Event Handlers for UI
        private void NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                CallFunction(e.Node.Name, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region ILegalPrinterDriver Members
        /// <summary>
        /// Show plugin's name
        /// </summary>
        public string Name { get { return Config.Assembly.NAME; } }
        /// <summary>
        /// Show plugin's version
        /// </summary>
        public string Version { get { return Config.Assembly.VERSION; } }
        /// <summary>
        /// Show plugin's author
        /// </summary>
        public string Author { get { return Config.Assembly.AUTHOR; } }
        public Hashtable AllowedMethods { get { return Params.AllowedMethods; } }
        public UserControl DriverUI { get { return driverui; } }
        public UserControl PortUI { get { return portui; } }
        public UserControl CompatibilityUI { get { return null; } }
        #endregion
    }
}
