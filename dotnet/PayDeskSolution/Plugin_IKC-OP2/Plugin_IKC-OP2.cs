/* 
 * IKC E260T v1.00 driver with UI (User Interface)
 * Author: Endy Woutson
 * Date: 19/01/11
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
using IKC_OP2.DriverUI;
using IKC_OP2.Config;
using IKC_OP2.DriverUI.Customs;
using IKC_OP2.Components.UI.DriverUI;
using components.Shared.Attributes;
using components.Shared.Defaults;
using components.Components.SerialPort;
using components.Lib;
using components.Shared.Interfaces;

namespace IKC_OP2
{
    [PluginSimpleAttribute(PluginType.LegalPrinterDriver)]
    public class Plugin_IKC_OP2 : ILegalPrinterDriver
    {
        // UI Components
        private UserControl _driverui;
        private UserControl _portui;
        private UserControl _compatibilityui;

        // Data
        private byte[] _inputData;
        private byte[] _outputData;
        private byte[] _dataForSend;
        private uint _readedBytes;

        // Communication port
        private Com_SerialPort _port;

        // Common functions
        private CoreLib _func;
        //private XmlParserObject xmlpo;

        // Configuarion
        private Params _param;

        // Service symbols
        private byte NOM = 0x00;
        private byte COD = 0x00;
        private byte DLE = 0x10;
        private byte STX = 0x02;
        private byte ETX = 0x03;
        private byte ACK = 0x06;
        private byte NAK = 0x15;
        private byte SYN = 0x16;
        private byte ENQ = 0x05;
        private byte CS = 0x00;

        // Constructor
        public Plugin_IKC_OP2()
        {
            // perform configuration and initialization driver plugin
            InitialiseDriverData();
            InitializeDriverComponents();
        }

        // Destructor
        ~Plugin_IKC_OP2()
        {
            if (_port.IsOpen)
                _port.Close();
            _port.Dispose(true);

            _driverui.Dispose();
            _portui.Dispose();
            _compatibilityui.Dispose();
            _param.Save();

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
            _param = new Params();
            _param.Load();
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
            _port = new Com_SerialPort();
            _func = new CoreLib();

            // perforom configation com-port
            _port.LoadPortConfig();
            //port.Open();

            // perform configuration user's interfaces
            _portui = new UI.AppUI.Port(ref _port);
            _driverui = new UI.AppUI.Tree();
            _compatibilityui = new UI.AppUI.Compatibility();
            ((TreeView)_driverui.Controls["functionsTree"]).NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(this.NodeMouseDoubleClick);
        }

        public bool Activate()
        {
            return _port.Open();
        }

        public bool Deactivate()
        {
            return _port.Close();
        }

        // Main Access Point
        public object CallFunction(string name, params object[] param)
        {
            // local variables
            string description = ((UI.AppUI.Tree)_driverui).GetDescription(name);
            object value = new object();

            // reset transfering data
            _outputData = new byte[0];
            _readedBytes = 0;

            // execute method
            switch (name)
            {
                #region Functions of registration
                case "SendStatus":
                    {
                        object[] FPinfo = SendStatus();

                        if (FPinfo != null && FPinfo.Length != 0)
                        {
                            FpInfo fi = new FpInfo(FPinfo, Name);
                            fi.ShowDialog();
                            fi.Dispose();
                        }

                        break;
                    }
                case "GetDate":
                    {
                        System.Windows.Forms.MessageBox.Show(GetDate().ToLongDateString(), Name);
                        break;
                    }
                case "SetDate":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SetDate sd = new SetDate(Name, description);
                            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SetDate(sd.date);
                            sd.Dispose();
                        }
                        else
                            SetDate((DateTime)param[0]);

                        break;
                    }
                case "GetTime":
                    {
                        System.Windows.Forms.MessageBox.Show(GetTime().ToLongTimeString(),
                            Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                case "SetTime":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SetTime st = new SetTime(Name, description);
                            if (st.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SetTime(st.time);
                            st.Dispose();
                        }
                        else
                            SetTime((DateTime)param[0]);

                        break;
                    }
                case "SetCod":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SetCod sc = new SetCod(Name, description);
                            if (sc.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SetCod(sc.oldPass, sc.nom, sc.newPass);
                            sc.Dispose();
                        }
                        else
                            SetCod((uint)param[0], (byte)param[1], (uint)param[2]);

                        break;
                    }
                case "SetCashier":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SetCashier sc = new SetCashier(Name, description);
                            if (sc.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SetCashier(sc.nom, sc.pass, sc.id);
                            sc.Dispose();
                        }
                        else
                            SetCashier(Convert.ToByte(param[0]), (uint)param[1], param[2].ToString());

                        break;
                    }
                case "PayMoney":
                    {
                        if (param == null || param.Length == 0)
                        {
                            PayMoney s = new PayMoney(Name, description);
                            uint articleNewID = 0;
                            if (s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                object[] article = null;
                                for (byte i = 0; i < s.articles.Length; i++)
                                {
                                    article = (object[])s.articles[i];
                                    articleNewID++;
                                    PayMoney((double)article[0], (byte)article[1], s.dontPrintOne,
                                        (double)article[2], article[3].ToString()[0],
                                        article[4].ToString(), articleNewID, (byte)article[6]);
                                }
                            }
                            s.Dispose();
                        }
                        else
                        {
                            FP_PayMoney(param);
                        }
                        //SaveArtID(articleNewID);
                        break;
                    }
                case "Comment":
                    {
                        if (param == null || param.Length == 0)
                        {
                            Comment cmm = new Comment(Name, description);
                            if (cmm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                Comment(cmm.commentLine, cmm.retCheque);
                            cmm.Dispose();
                        }
                        else
                            Comment(param[0].ToString(), (bool)param[1]);

                        break;
                    }
                case "CplPrint":
                    {
                        CplPrint();
                        byte[] mem = GetMemory("2A", 0, 1);
                        //BIT 6 - OnLine state
                        string _status = string.Format("{0} {1}", "Друк у нефіскальному режимі", (mem[0] & 0x08) != 0 ? "дозволений" : "не дозволений");
                        System.Windows.Forms.MessageBox.Show(_status, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                case "LineFeed":
                    {
                        LineFeed();
                        break;
                    }
                case "ResetOrder":
                    {
                        ResetOrder();
                        break;
                    }
                case "Avans":
                    {
                        if (param == null || param.Length == 0)
                        {
                            Avans a = new Avans(Name, description);
                            if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                Avans(a.copecks);
                            a.Dispose();
                        }
                        else
                            Avans(Convert.ToUInt32(param[0]));

                        break;
                    }
                case "Sale":
                    {
                        if (param == null || param.Length == 0)
                        {
                            uint articleNewID = 0;
                            Sale s = new Sale(Name, description);
                            if (s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                object[] article = null;
                                for (byte i = 0; i < s.articles.Length; i++)
                                {
                                    article = (object[])s.articles[i];
                                    articleNewID++;
                                    Sale((double)article[0], (byte)article[1], s.dontPrintOne,
                                        (double)article[2], article[3].ToString()[0],
                                        article[4].ToString(), articleNewID, (byte)article[6]);
                                }
                            }
                            s.Dispose();
                        }
                        else
                        {
                            FP_Sale(param);
                        }

                        //SaveArtID(articleNewID);
                        break;
                    }
                case "Payment":
                    {

                        if (param == null || param.Length == 0)
                        {
                            Payment p = new Payment(Name, description);
                            if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                object[] ans = Payment((byte)p.payInfo[0], (bool)p.payInfo[1],
                                    (double)p.payInfo[2], (bool)p.payInfo[3], p.payInfo[4].ToString());
                                try
                                {
                                    string info = string.Empty;
                                    if (ans[0].ToString() == "1")
                                        info += "Здача : ";
                                    else
                                        info += "Залишок : ";

                                    info += string.Format("{0:F2}", ans[1]);
                                    System.Windows.Forms.MessageBox.Show(info, Name,
                                        System.Windows.Forms.MessageBoxButtons.OK,
                                        System.Windows.Forms.MessageBoxIcon.Information);
                                }
                                catch { }
                            }
                            p.Dispose();
                        }
                        else
                            FP_Payment(param);

                        break;
                    }
                case "SetString":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SetString ss = new SetString(Name, description);
                            if (ss.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SetString(ss.lines);
                            ss.Dispose();
                        }
                        else
                            SetString((string[])param);

                        break;
                    }
                case "Give":
                    {
                        if (param == null || param.Length == 0)
                        {
                            Give g = new Give(Name, description);
                            if (g.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                Give(g.copecks);
                            g.Dispose();
                        }
                        else
                            Give((uint)param[0]);

                        break;
                    }
                case "SendCustomer":
                    {
                        if (param == null || param.Length == 0)
                        {
                            SendCustomer sc = new SendCustomer(Name, description);
                            if (sc.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SendCustomer(sc.lines, sc.show);
                            sc.Dispose();
                        }
                        else
                            SendCustomer((string[])param[0], (bool[])param[1]);

                        break;
                    }
                case "GetMemory":
                    {
                        if (param == null || param.Length == 0)
                        {
                            GetMemory gm = new GetMemory(Name, description);
                            if (gm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                byte[] mem = GetMemory(gm.block, gm.page, gm.size);
                                string memInfoLine = "DEC :";
                                for (int i = 0; i < mem.Length; i++)
                                {
                                    if (i % 10 == 0)
                                        memInfoLine += "\r\n";
                                    memInfoLine += mem[i].ToString() + " ";
                                }
                                memInfoLine += "\r\n\r\nHEX :";
                                for (int i = 0; i < mem.Length; i++)
                                {
                                    if (i % 10 == 0)
                                        memInfoLine += "\r\n";
                                    memInfoLine += String.Format("{0:X2}", mem[i]) + " ";
                                }
                                System.Windows.Forms.MessageBox.Show(memInfoLine, Name,
                                    System.Windows.Forms.MessageBoxButtons.OK,
                                    System.Windows.Forms.MessageBoxIcon.Information);
                            }
                            gm.Dispose();
                        }
                        else
                        {
                            byte[] mem = GetMemory(param[0].ToString(), Convert.ToByte(param[1]), Convert.ToByte(param[2]));
                            value = new object[] { _func.GetNumberFromBCD(mem) };
                        }

                        break;
                    }
                case "OpenBox":
                    {
                        OpenBox();
                        break;
                    }
                case "PrintCopy":
                    {
                        PrintCopy();
                        break;
                    }
                case "PrintVer":
                    {
                        PrintVer();
                        break;
                    }
                case "GetBox":
                    {
                        uint copecks = GetBox();
                        string _status = string.Format("В сейфі : {0}", (double)(copecks / 100) + (double)(copecks % 100) / 100);
                        System.Windows.Forms.MessageBox.Show(_status, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                case "Discount":
                    {
                        if (param == null || param.Length == 0)
                        {
                            Discount d = new Discount(Name, description);
                            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                Discount((byte)d.discInfo[0], (double)d.discInfo[1], Convert.ToByte(d.discInfo[2]), d.discInfo[3].ToString());
                            d.Dispose();
                        }
                        else
                            Discount((byte)param[0], (double)param[1], Convert.ToByte(param[2]), param[3].ToString());

                        break;
                    }
                case "CplOnline":
                    {
                        CplOnline();
                        byte[] mem = GetMemory("2A", 0, 1);
                        //BIT 6 - OnLine state
                        string _status = string.Format("{0} {1}", "Режим OnLine", (mem[0] & 0x40) != 0 ? "увімкнено" : "вимкнено");
                        System.Windows.Forms.MessageBox.Show(_status, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                case "CplInd":
                    {
                        CplInd();
                        byte[] mem = GetMemory("29", 0, 1);
                        //BIT 7 - Indicator state
                        string _status = string.Format("{0} {1}", "Видача суми на індикатор", (mem[0] & 0x80) != 0 ? "не активна" : "активна");
                        System.Windows.Forms.MessageBox.Show(_status, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                case "ChangeRate":
                    {
                        if (param == null || param.Length == 0)
                        {
                            ChangeRate cr = new ChangeRate(Name, description);
                            if (cr.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                ChangeRate(cr.rate);
                            cr.Dispose();
                        }
                        else
                            ChangeRate((byte)param[0]);

                        break;
                    }
                case "LineSP":
                    {
                        if (param == null || param.Length == 0)
                        {
                            LineSP lsp = new LineSP(Name, description);
                            if (lsp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                LineSP(lsp.lsp);
                            lsp.Dispose();
                        }
                        else
                            LineSP((byte)param[0]);

                        break;
                    }
                case "TransPrint":
                    {
                        if (param == null || param.Length == 0)
                        {
                            TransPrint tp = new TransPrint(Name, description);
                            if (tp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                TransPrint(tp.text, tp.endPrint);
                            tp.Dispose();
                        }
                        else
                            TransPrint(param[0].ToString(), (bool)param[0]);

                        break;
                    }
                case "GetArticle":
                    {
                        object[] artInfo = new object[0];
                        string info = string.Empty;
                        if (param == null)
                        {
                            GetArticle ga = new GetArticle(Name, description);
                            if (ga.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                artInfo = GetArticle(ga.articleID);
                            ga.Dispose();
                        }
                        else
                            artInfo = GetArticle((uint)param[0]);

                        if (artInfo.Length != 0)
                        {
                            info += "Назва товару" + " : ";
                            info += artInfo[0].ToString() + "\r\n";
                            info += "Кількість" + " : ";
                            info += artInfo[1].ToString() + "\r\n";
                            info += "Ціна" + " : ";
                            info += artInfo[2].ToString() + "\r\n";
                            info += "Податкова група" + " : ";
                            info += artInfo[3].ToString() + "\r\n";
                            info += "Сума обігу" + " : ";
                            info += artInfo[4].ToString() + "\r\n";
                            info += "\r\n\r\n";
                            info += "Зворотня операція" + "\r\n";
                            info += "Кількість" + " : ";
                            info += artInfo[5].ToString() + "\r\n";
                            info += "Сума обігу" + " : ";
                            info += artInfo[6].ToString() + "\r\n";
                        }
                        else
                            info = "Інформація про то товар відсутня";

                        System.Windows.Forms.MessageBox.Show(info, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);

                        break;
                    }
                case "GetDayReport":
                    {
                        object[] local_value = new object[2];
                        value = local_value = GetDayReport();
                        if (param == null && value != null && local_value.Length != 0)
                        {
                            string dayRepInfoLine = string.Empty;
                            int i = 0;
                            string[] payTypes = new string[] { "Картка", "Кредит", "Чек", "Готівка" };

                            double[] sales_group = (double[])((object[])local_value[1])[0];
                            double[] sales_forms = (double[])((object[])local_value[1])[1];

                            double[] pays_group = (double[])((object[])local_value[6])[0];
                            double[] pays_forms = (double[])((object[])local_value[6])[1];

                            dayRepInfoLine += "Лічильник чеків продаж : " + local_value[0];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Лічильник продаж по податковим групам і формам оплати";
                            dayRepInfoLine += "\r\n";
                            for (i = 0; i < sales_group.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", (char)(((int)'А') + i), sales_group[i]);
                            dayRepInfoLine += "--------------\r\n";
                            for (i = 0; i < sales_forms.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", payTypes[i], sales_forms[i]);
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна націнка по продажам : " + local_value[2];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна знижка по продажам : " + local_value[3];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна сума службового внесення : " + local_value[4];
                            dayRepInfoLine += "\r\n\r\n";
                            dayRepInfoLine += "Лічильник чеків виплат : " + local_value[5];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Лічильник виплат по податковим групам і формам оплати";
                            dayRepInfoLine += "\r\n";
                            for (i = 0; i < pays_group.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", (char)(((int)'А') + i), pays_group[i]);
                            dayRepInfoLine += "--------------\r\n";
                            for (i = 0; i < pays_forms.Length; i++)
                                dayRepInfoLine += string.Format("{0} : {1:F2}\r\n", payTypes[i], pays_forms[i]);
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна націнка по виплатам : " + local_value[7];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна знижка по виплатам : " + local_value[8];
                            dayRepInfoLine += "\r\n";
                            dayRepInfoLine += "Денна сума службової видачі : " + local_value[9];

                            System.Windows.Forms.MessageBox.Show(dayRepInfoLine, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        break;
                    }
                case "GetCheckSums":
                    {
                        object[] local_value = new object[2];
                        value = local_value = GetCheckSums();
                        if (value != null && local_value.Length != 0)
                        {
                            string checkSumsInfoLine = string.Empty;
                            int i = 0;
                            checkSumsInfoLine += "Лічильник обігів по податковим групам";
                            checkSumsInfoLine += "\r\n";
                            double[] tax = (double[])local_value[0];
                            for (i = 0; i < tax.Length; i++)
                                checkSumsInfoLine += string.Format("{0} : {1:F2}\r\n", (char)(((int)'А') + i), tax[i]);
                            checkSumsInfoLine += "\r\n";
                            double[] sum = (double[])local_value[1];
                            checkSumsInfoLine += "Лічильник сум сплат по формам оплат";
                            checkSumsInfoLine += "\r\n";
                            string[] payTypes = new string[] { "Картка", "Кредит", "Чек", "Готівка" };
                            for (i = 0; i < sum.Length; i++)
                                checkSumsInfoLine += string.Format("{0} : {1:F2}\r\n", payTypes[i], sum[i]);

                            System.Windows.Forms.MessageBox.Show(checkSumsInfoLine, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        break;
                    }
                case "GetTaxRates":
                    {
                        object[] taxData = GetTaxRates();
                        if (taxData != null && taxData.Length != 0)
                        {
                            string taxInfoLine = string.Empty;
                            taxInfoLine += "К-сть податкових ставок : " + taxData[0];
                            taxInfoLine += "\r\n";
                            taxInfoLine += "Дата встановлення : " + ((DateTime)taxData[1]).ToLongDateString();
                            taxInfoLine += "\r\n\r\n";
                            taxInfoLine += "Ставки ПДВ";
                            taxInfoLine += "\r\n";
                            double[] tax = (double[])taxData[2];
                            for (int i = 0; i < tax.Length; i++)
                                taxInfoLine += string.Format("{0} : {1:F2}%\r\n", (char)(((int)'А') + i), tax[i]);
                            taxInfoLine += "\r\n";
                            taxInfoLine += "К-сть десяткових розрядів грошових сум : " + taxData[3];
                            taxInfoLine += "\r\n";
                            taxInfoLine += "Тип ПДВ : " + ((taxData[4].ToString() == "0") ? "Включний" : "Додатній");
                            if ((bool)taxData[5])
                            {
                                taxInfoLine += "\r\n\r\n";
                                taxInfoLine += "Ставки зборів";
                                taxInfoLine += "\r\n";
                                tax = (double[])taxData[6];
                                for (int i = 0; i < tax.Length; i++)
                                    taxInfoLine += string.Format("{0} : {1:F2}%\r\n", (char)(((int)'А') + i), tax[i]);
                            }
                            System.Windows.Forms.MessageBox.Show(taxInfoLine, Name,
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                        break;
                    }
                case "CplCutter":
                    {
                        CplCutter();
                        byte[] mem = GetMemory("301A", 16, 1);
                        //BIT 3 - Cutter state
                        string _status = string.Format("{0} {1}", "Обріжчик", (mem[0] & 0x08) == 0 ? "активний" : "не активний");
                        System.Windows.Forms.MessageBox.Show(_status, Name,
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                        break;
                    }
                #endregion
                #region Function of programming
                case "Fiscalization":
                    {
                        Fiscalazation f = new Fiscalazation(Name, description);
                        if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            Fiscalization( f.pass, f.fn);
                        f.Dispose();

                        break;
                    }
                case "SetHeadLine":
                    {
                        SetHeadLine shl = new SetHeadLine(Name, description);
                        if (shl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetHeadLine(shl.pass, shl.line1, shl.line2, shl.line3, shl.line4);
                        shl.Dispose();

                        break;
                    }
                case "SetTaxRate":
                    {
                        SetTaxRate str = new SetTaxRate(Name, description);
                        if (str.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            SetTaxRate(str.pass, str.taxCount, str.tax, str.status, str.taxGCount, str.gtax);
                        str.Dispose();

                        break;
                    }
                case "ProgArt":
                    {
                        if (param == null || param.Length == 0)
                        {
                            ProgArt pa = new ProgArt(Name, description);
                            if (pa.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                object[] article = null;
                                for (byte i = 0; i < pa.articles.Length; i++)
                                {
                                    article = (object[])pa.articles[i];

                                    ProgArt(pa.pass, (byte)article[0],
                                        (double)article[1], article[2].ToString()[0],
                                        article[3].ToString(), (uint)article[4]);
                                }
                            }
                            pa.Dispose();
                        }
                        else
                        {
                            System.Data.DataTable dTable = (System.Data.DataTable)param[0];
                            object[] article = new object[5];
                            byte ppt = Convert.ToByte(param[1]);
                            uint articleNewID = (uint)Params.DriverData["LastArtNo"]; //LoadArtID(port);

                            System.IO.StreamWriter sWr = null;

                            for (int i = 0; i < dTable.Rows.Count; i++)
                            {
                                article[0] = ppt;
                                article[1] = _func.GetDouble(dTable.Rows[i]["PRICE"]);
                                article[2] = dTable.Rows[i]["VG"];
                                article[3] = dTable.Rows[i]["NAME"].ToString().Replace('і', 'i').Replace('І', 'I');
                                articleNewID++;

                                ProgArt((ushort)0, (byte)article[0], (double)article[1],
                                    article[3].ToString()[0], article[4].ToString(), articleNewID);
                            }
                            Params.DriverData["LastArtNo"] = articleNewID;
                            this._param.Save();
                        }

                        break;
                    }
                case "LoadBMP":
                    {
                        LoadBMP lbmp = new LoadBMP(Name, description);
                        if (lbmp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            LoadBMP((ushort)lbmp.imageInfo[0], true, lbmp.imageInfo[1].ToString());
                        lbmp.Dispose();

                        break;
                    }
                #endregion
                #region Function of reports
                case "ArtReport":
                    {
                        byte[] mem = GetMemory("3003", 16, 2 * 10);
                        uint pass = (uint)_func.GetNumber(new byte[] { mem[18], mem[19] });
                        ArtReport(pass);
                        break;
                    }
                case "ArtXReport":
                    {
                        byte[] mem = GetMemory("3003", 16, 2 * 10);
                        uint pass = (uint)_func.GetNumber(new byte[] { mem[18], mem[19] });
                        ArtXReport(pass);
                        break;
                    }
                case "DayReport":
                    {
                        byte[] mem = GetMemory("3003", 16, 2 * 10);
                        uint pass = (uint)_func.GetNumber(new byte[] { mem[18], mem[19] });
                        DayReport(pass);
                        break;
                    }
                case "DayClrReport":
                    {
                        byte[] mem = GetMemory("3003", 16, 2 * 10);
                        uint pass = (uint)_func.GetNumber(new byte[] { mem[18], mem[19] });
                        DayClrReport(pass);
                        Params.DriverData["LastArtNo"] = 1;
                        this._param.Save();
                        System.IO.StreamWriter sWr = System.IO.File.AppendText("a.txt");
                        sWr.WriteLine("---------\r\n## Z-rep ##\r\n---------");
                        sWr.Close();
                        sWr.Dispose();

                        break;
                    }
                case "PeriodicReport":
                    {
                        byte[] mem = GetMemory("3003", 16, 2 * 10);
                        uint pass = (uint)_func.GetNumber(new byte[] { mem[18], mem[19] });
                        PeriodicReport pr = new PeriodicReport(Name, description);
                        if (pr.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PeriodicReport(pass, pr.startDate, pr.endDate);
                        pr.Dispose();

                        break;
                    }
                case "PeriodicReportShort":
                    {
                        byte[] mem = GetMemory("3003", 16, 2 * 10);
                        uint pass = (uint)_func.GetNumber(new byte[] { mem[18], mem[19] });
                        PeriodicReportShort prs = new PeriodicReportShort(Name, description);
                        if (prs.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PeriodicReportShort(pass, prs.startDate, prs.endDate);
                        prs.Dispose();

                        break;
                    }
                case "PeriodicReport2":
                    {
                        byte[] mem = GetMemory("3003", 16, 2 * 10);
                        uint pass = (uint)_func.GetNumber(new byte[] { mem[18], mem[19] });
                        PeriodicReport2 pr2 = new PeriodicReport2(Name, description);
                        if (pr2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            PeriodicReport2(pass, pr2.startNo, pr2.endNo);
                        pr2.Dispose();

                        break;
                    }
                #endregion
                /*
                 * There are special methods for accessing from main app and
                 * custom methods without custom their implementation. (using built-in parameters)
                 */
                #region Program access func
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
                
                #endregion
            }
            return value;
        }

        // Driver implementation
        // Warning! Don't change this code region
        #region Driver implementation
        #region Functions of registration
        private object[] SendStatus()
        {
            Params.DriverData["LastFunc"] = "SendStatus";
            NOM = 0;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            object[] ekkrInfo = null;
            if (_outputData != null && _outputData.Length != 0)
            {
                Array.Resize<object>(ref ekkrInfo, 16 + 1 + 1 + 1 + 1 + 1 + 1);

                ekkrInfo[0] = "";
                ekkrInfo[1] = string.Format("{0} : {1}", "Режим реєстрації оплат в чеці", (_outputData[0] & 0x02) != 0 ? "ТАК" : "НІ");
                ekkrInfo[2] = string.Format("{0} : {1}", "Відкрита грошова скринька", (_outputData[0] & 0x04) != 0 ? "ТАК" : "НІ");
                ekkrInfo[3] = string.Format("{0} : {1}", "Чек", (_outputData[0] & 0x08) == 0 ? "ПРОДАЖУ" : "ВИПЛАТИ");
                ekkrInfo[4] = string.Format("{0} : {1}", "ПДВ", (_outputData[0] & 0x10) == 0 ? "ВКЛЮЧНИЙ" : "ДОДАТНІЙ");
                ekkrInfo[5] = string.Format("{0} : {1}", "Були продажі\\виплати (зміна відкрита)", (_outputData[0] & 0x20) != 0 ? "ТАК" : "НІ");
                ekkrInfo[6] = string.Format("{0} : {1}", "Відкрито чек", (_outputData[0] & 0x40) != 0 ? "ТАК" : "НІ");
                ekkrInfo[7] = string.Format("{0} : {1}", "Заборона видачі суми на індикатор", (_outputData[0] & 0x80) != 0 ? "ТАК" : "НІ");

                ekkrInfo[8] = "";
                ekkrInfo[9] = string.Format("{0} : {1}", "Були введені нові податки", (_outputData[1] & 0x02) != 0 ? "ТАК" : "НІ");
                ekkrInfo[10] = "";
                ekkrInfo[11] = string.Format("{0} : {1}", "Заборона друку", (_outputData[1] & 0x08) != 0 ? "ТАК" : "НІ");
                ekkrInfo[12] = string.Format("{0} : {1}", "Принтер фіскалізовано", (_outputData[1] & 0x10) != 0 ? "ТАК" : "НІ");
                ekkrInfo[13] = string.Format("{0} : {1}", "Аварійне завершення останньої команди", (_outputData[1] & 0x20) != 0 ? "ТАК" : "НІ");
                ekkrInfo[14] = string.Format("{0} : {1}", "Режим OnLine реєстрацій", (_outputData[1] & 0x40) != 0 ? "ТАК" : "НІ");
                ekkrInfo[15] = "";

                string sno = string.Empty;
                for (int i = 2; i < 21; i++)
                    sno += (char)_outputData[i];

                ekkrInfo[16] = string.Format("{0} : {1}", "Серійний номер і дата виробництва", sno);

                byte[] data = new byte[3];
                int[] mas = new int[3];
                int offset = 36;

                if ((_outputData[1] & 0x10) != 0)
                {
                    data[0] = _outputData[21];
                    data[1] = _outputData[22];
                    data[2] = _outputData[23];

                    try
                    {
                        mas = _func.GetArrayFromBCD(data);
                    }
                    catch { }

                    ekkrInfo[17] = string.Format("{0} : {1:D2}-{2:D2}-{3:D2}", "Дата реєстрації (ДД-ММ-РР)", mas[0], mas[1], mas[2]);

                    data = new byte[2];
                    mas = new int[2];

                    data[0] = _outputData[24];
                    data[1] = _outputData[25];

                    try
                    {
                        mas = _func.GetArrayFromBCD(data);
                    }
                    catch { }

                    ekkrInfo[18] = string.Format("{0} : {1:D2}:{2:D2}", "Час реєстрації (ЧЧ:ММ)", mas[0], mas[1]);

                    data = new byte[10];
                    for (int i = 26; i < 36; i++)
                        data[i - 26] = _outputData[i];

                    sno = Encoding.GetEncoding(866).GetString(data);

                    ekkrInfo[19] = string.Format("{0} : {1}", "Фіскальний номер", sno);

                    ekkrInfo[20] = string.Format("{0} :\r\n{1}", "Атрибути платника податків", "%1% %2% %3% %4%");

                    for (int o = 1; o < 5; o++)
                    {
                        sno = string.Empty;
                        for (int i = offset; i < offset + _outputData[offset]; i++)
                            sno += (char)_outputData[i];
                        if (sno.Length != 0)
                            sno += "\r\n";
                        else
                            sno = " ";

                        ekkrInfo[20] = ekkrInfo[20].ToString().Replace("%" + o + "%", sno);

                        offset += sno.Length;
                    }
                }
                else
                    offset += 4;

                //EP-01, EP-02, EP-51
                data = new byte[5];
                data[0] = _outputData[offset];
                data[1] = _outputData[offset + 1];
                data[2] = _outputData[offset + 2];
                data[3] = _outputData[offset + 3];
                data[4] = _outputData[offset + 4];

                sno = Encoding.GetEncoding(866).GetString(data);
                ekkrInfo[21] = string.Format("{0} : {1}", "Версія ПЗ ЕККР", sno);
            }

            return ekkrInfo;
        }//make returned info
        private DateTime GetDate()
        {
            Params.DriverData["LastFunc"] = "GetDate";
            NOM = 1;

            _dataForSend = new byte[0];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            DateTime dt = new DateTime();
            try
            {
                int[] ans = _func.GetArrayFromBCD(_outputData);
                dt = new DateTime(ans[2] + 2000, ans[1], ans[0]);
            }
            catch { }

            return dt;
        }//ok
        private void SetDate(DateTime date)
        {
            Params.DriverData["LastFunc"] = "SetDate";
            NOM = 2;

            //Creating data
            _dataForSend = _func.GetBCDFromArray(new int[] { date.Day, date.Month, int.Parse(date.Year.ToString().Substring(2, 2)) });
            //BCD dd = new BCD(date.Day);
            //BCD mm = new BCD(date.Month);
            //BCD yy = new BCD(int.Parse(date.Year.ToString().Substring(2, 2)));

            //Data = new byte[3];
            //Data[0] = dd.CompressedBCD[0];
            //Data[1] = mm.CompressedBCD[0];
            //Data[2] = yy.CompressedBCD[0];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private DateTime GetTime()
        {
            Params.DriverData["LastFunc"] = "GetTime";
            NOM = 3;

            _dataForSend = new byte[0];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            DateTime dt = new DateTime();

            try
            {
                int[] ans = _func.GetArrayFromBCD(_outputData);
                dt = new DateTime(1990, 10, 10, ans[0], ans[1], ans[2]);
            }
            catch { }

            return dt;
        }//ok
        private void SetTime(DateTime time)
        {
            Params.DriverData["LastFunc"] = "SetTime";
            NOM = 4;

            //Creating data
            _dataForSend = _func.GetBCDFromArray(new int[] { time.Hour, time.Minute, time.Second });
            //BCD hh = new BCD(time.Hour);
            //BCD mm = new BCD(time.Minute);
            //BCD ss = new BCD(time.Second);

            //Data = new byte[3];
            //Data[0] = hh.CompressedBCD[0];
            //Data[1] = mm.CompressedBCD[0];
            //Data[2] = ss.CompressedBCD[0];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void SetCod(uint oldPass, byte no, uint newPass)
        {
            Params.DriverData["LastFunc"] = "SetCod";
            NOM = 5;

            //Creating data
            _dataForSend = new byte[2 + 1 + 2];
            byte[] p = _func.GetByteArray(oldPass, 2);
            _dataForSend[0] = p[0];
            _dataForSend[1] = p[1];
            _dataForSend[2] = no;//0..7-cashier,8-program mode,9-report mode
            p = _func.GetByteArray(newPass, 2);
            _dataForSend[3] = p[0];
            _dataForSend[4] = p[1];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void SetCashier(byte n, uint pass, string name)
        {
            Params.DriverData["LastFunc"] = "SetCashier";
            NOM = 6;

            //Creating data
            _dataForSend = new byte[1 + 2 + 1 + name.Length];
            byte[] p = _func.GetByteArray(pass, 2);
            _dataForSend[0] = p[0];
            _dataForSend[1] = p[1];
            _dataForSend[2] = n;// 0..7
            _dataForSend[3] = (byte)name.Length;//0..15
            byte[] BinLine = new byte[name.Length];
            BinLine = Encoding.GetEncoding(866).GetBytes(name.Replace('і', 'i').Replace('І', 'I'));
            for (byte i = 0; i < BinLine.Length; i++)
                _dataForSend[i + 4] = BinLine[i];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private object[] PayMoney(double tot, byte doseDecimal, bool notPrintOne, double price, char pdv, string name, uint id, byte moneyDecimal)
        {
            Params.DriverData["LastFunc"] = "PayMoney";
            NOM = 8;

            //local variables
            byte[] binArr = null;
            byte oneByte = 0;

            byte[] unusedChars = {
                (byte)0xF8,
                (byte)0xF9,
                (byte)0xFA,
                (byte)0xFB,
                (byte)0xFC
            };

            string clnName = string.Empty;
            byte[] tmpNameBytes = Encoding.GetEncoding(866).GetBytes(name);
            List<byte> clnNameBytes = new List<byte>();
            foreach (byte nc in tmpNameBytes)
            {
                if (Array.IndexOf(unusedChars, nc) >= 0)
                    continue;
                if ((0x20 <= nc && nc <= 0xAF) || (0xE0 <= nc && nc <= 0xF7))
                    clnNameBytes.Add(nc);
            }
            clnName = Encoding.GetEncoding(866).GetString(clnNameBytes.ToArray());

            // !!!!!!!!!!! CHAR TABLE TEST !!!!!!!!!!!!!!
            /*
            string testName = string.Empty;
            int page = 3;
            int maxTestNameLength = 50;
            List<byte> testNameByte = new List<byte>();
            for (byte testn = 0x20; testn < 0xF8; testn++)
                if ((0x20 <= testn && testn <= 0xAF) || (0xE0 <= testn && testn <= 0xF7))
                    testNameByte.Add(testn);
            byte[] testGenName = new byte[maxTestNameLength];
            byte[] testGenNameFull = testNameByte.ToArray();
            //List<byte> testGenName = new List<byte>();
            Array.Copy(testNameByte.ToArray(), (page * maxTestNameLength), testGenName, 0, maxTestNameLength);
            /*
            for (int nTmp = 0, iTmp = (page * maxTestNameLength); nTmp < maxTestNameLength; iTmp++, nTmp++)
            {
                testGenName[nTmp] = (byte)testGenNameFull[iTmp];
            }
            *
            testName = Encoding.GetEncoding(866).GetString(testGenName);
            */
            // set name of testName for test charsets
            name = clnName;
            name = name.Trim();

            //##Creating data
            _dataForSend = new byte[3 + 1 + 4 + 1 + 1 + name.Length + 6];
            //Total
            tot *= Math.Pow(10.0, (double)doseDecimal);
            binArr = _func.GetByteArray((int)Math.Round(tot), 3);
            _dataForSend[0] = binArr[0];
            _dataForSend[1] = binArr[1];
            _dataForSend[2] = binArr[2];
            //Status
            oneByte = doseDecimal;
            if (notPrintOne)
                oneByte |= 0x80;
            _dataForSend[3] = oneByte;
            //Price
            price *= Math.Pow(10.0, (double)moneyDecimal);
            binArr = _func.GetByteArray((int)Math.Round(Math.Abs(price)), 4);
            _dataForSend[4] = binArr[0];
            _dataForSend[5] = binArr[1];
            _dataForSend[6] = binArr[2];
            if (price < 0)
                binArr[3] |= 0x80;
            _dataForSend[7] = binArr[3];
            //Tax
            oneByte = 0x00;
            switch (pdv)
            {
                case 'А': { oneByte = 0x80; break; }
                case 'Б': { oneByte = 0x81; break; }
                case 'В': { oneByte = 0x82; break; }
                case 'Г': { oneByte = 0x83; break; }
                case 'Д': { oneByte = 0x84; break; }
                case 'Е': { oneByte = 0x85; break; }
                //TAX E (no tax)
                default: { oneByte = 0x85; break; }
            }
            _dataForSend[8] = oneByte;
            //Art Name length
            /*
            name = name.Trim();
            if (name.LastIndexOf('№') == name.Length - 1)
                name = name.Substring(0, name.Length - 1);
            name = name.Trim();
            */
            _dataForSend[9] = (byte)name.Length;
            //Art Name
            binArr = Encoding.GetEncoding(866).GetBytes(name);
            binArr.CopyTo(_dataForSend, 10);
            //Art id
            binArr = _func.GetByteArray(id, 6);
            _dataForSend[_dataForSend.Length - 6] = binArr[0];
            _dataForSend[_dataForSend.Length - 5] = binArr[1];
            _dataForSend[_dataForSend.Length - 4] = binArr[2];
            _dataForSend[_dataForSend.Length - 3] = binArr[3];
            _dataForSend[_dataForSend.Length - 2] = binArr[4];
            _dataForSend[_dataForSend.Length - 1] = binArr[5];
            //##End Creating data

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            object[] ans = new object[2];
            /* ans[0] = func.ConvertFromByte(new byte[4] { outputData[0], outputData[1], outputData[2], outputData[3] });
             ans[1] = func.ConvertFromByte(new byte[4] { outputData[4], outputData[5], outputData[6], outputData[7] });
             ans[0] = Convert.ToDouble(ans[0]) / 100;
             ans[1] = Convert.ToDouble(ans[1]) / 100;*/

            return ans;
        }//ok
        private void Comment(string text, bool retCheque)
        {
            Params.DriverData["LastFunc"] = "Comment";
            NOM = 11;

            //Creating data
            _dataForSend = new byte[1 + text.Length];
            _dataForSend[0] = (byte)text.Length;
            if (retCheque)
                _dataForSend[0] |= 0x80;

            byte[] BinLine = new byte[0];
            BinLine = Encoding.GetEncoding(866).GetBytes(text.Replace('і', 'i').Replace('І', 'I'));
            for (byte i = 0; i < BinLine.Length; i++)
                _dataForSend[i + 1] = BinLine[i];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void CplPrint()
        {
            Params.DriverData["LastFunc"] = "CplPrint";
            NOM = 12;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void LineFeed()
        {
            Params.DriverData["LastFunc"] = "LineFeed";
            NOM = 14;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void ResetOrder()
        {
            Params.DriverData["LastFunc"] = "ResetOrder";
            NOM = 15;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void Avans(uint suma)
        {
            Params.DriverData["LastFunc"] = "Avans";
            NOM = 16;

            //Creating data
            _dataForSend = new byte[4];
            _dataForSend = _func.GetByteArray(suma, _dataForSend.Length);

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private object[] Sale(double tot, byte doseDecimal, bool notPrintOne, double price, char pdv, string name, uint id, byte moneyDecimal)
        {
            Params.DriverData["LastFunc"] = "Sale";
            NOM = 18;

            //local variables
            byte[] binArr = null;
            byte oneByte = 0;

            byte[] unusedChars = {
                (byte)0xF8,
                (byte)0xF9,
                (byte)0xFA,
                (byte)0xFB,
                (byte)0xFC
            };

            string clnName = string.Empty;
            byte[] tmpNameBytes = Encoding.GetEncoding(866).GetBytes(name);
            List<byte> clnNameBytes = new List<byte>();
            foreach (byte nc in tmpNameBytes)
            {
                if (Array.IndexOf(unusedChars, nc) >= 0)
                    continue;
                if ((0x20 <= nc && nc <= 0xAF) || (0xE0 <= nc && nc <= 0xF7))
                    clnNameBytes.Add(nc);
            }
            clnName = Encoding.GetEncoding(866).GetString(clnNameBytes.ToArray());

            // !!!!!!!!!!! CHAR TABLE TEST !!!!!!!!!!!!!!
            /*
            string testName = string.Empty;
            int page = 3;
            int maxTestNameLength = 50;
            List<byte> testNameByte = new List<byte>();
            for (byte testn = 0x20; testn < 0xF8; testn++)
                if ((0x20 <= testn && testn <= 0xAF) || (0xE0 <= testn && testn <= 0xF7))
                    testNameByte.Add(testn);
            byte[] testGenName = new byte[maxTestNameLength];
            byte[] testGenNameFull = testNameByte.ToArray();
            //List<byte> testGenName = new List<byte>();
            Array.Copy(testNameByte.ToArray(), (page * maxTestNameLength), testGenName, 0, maxTestNameLength);
            /*
            for (int nTmp = 0, iTmp = (page * maxTestNameLength); nTmp < maxTestNameLength; iTmp++, nTmp++)
            {
                testGenName[nTmp] = (byte)testGenNameFull[iTmp];
            }
            *
            testName = Encoding.GetEncoding(866).GetString(testGenName);
            */
            // set name of testName for test charsets
            name = clnName;
            name = name.Trim();
            //##Creating data
            _dataForSend = new byte[3 + 1 + 4 + 1 + 1 + name.Length + 6];
            //Total
            tot *= Math.Pow(10.0, (double)doseDecimal);
            binArr = _func.GetByteArray((int)Math.Round(tot), 3);
            _dataForSend[0] = binArr[0];
            _dataForSend[1] = binArr[1];
            _dataForSend[2] = binArr[2];
            //Status
            oneByte = doseDecimal;
            if (notPrintOne)
                oneByte |= 0x80;
            _dataForSend[3] = oneByte;
            //Price
            price *= Math.Pow(10.0, (double)moneyDecimal);
            binArr = _func.GetByteArray((int)Math.Round(Math.Abs(price)), 4);
            _dataForSend[4] = binArr[0];
            _dataForSend[5] = binArr[1];
            _dataForSend[6] = binArr[2];
            if (price < 0)
                binArr[3] |= 0x80;
            _dataForSend[7] = binArr[3];
            //Tax
            oneByte = 0x00;
            switch (pdv)
            {
                case 'А': { oneByte = 0x80; break; }
                case 'Б': { oneByte = 0x81; break; }
                case 'В': { oneByte = 0x82; break; }
                case 'Г': { oneByte = 0x83; break; }
                case 'Д': { oneByte = 0x84; break; }
                case 'Е': { oneByte = 0x85; break; }
                //TAX E (no tax)
                default: { oneByte = 0x85; break; }
            }
            _dataForSend[8] = oneByte;
            //Art Name length
            /*
            name = name.Trim();
            if (name.LastIndexOf('№') == name.Length - 1)
                name = name.Substring(0, name.Length - 1);
            name = name.Trim();
            */
            _dataForSend[9] = (byte)name.Length;
            //Art Name
            binArr = Encoding.GetEncoding(866).GetBytes(name);
            binArr.CopyTo(_dataForSend, 10);
            //Art id
            binArr = _func.GetByteArray(id, 6);
            _dataForSend[_dataForSend.Length - 6] = binArr[0];
            _dataForSend[_dataForSend.Length - 5] = binArr[1];
            _dataForSend[_dataForSend.Length - 4] = binArr[2];
            _dataForSend[_dataForSend.Length - 3] = binArr[3];
            _dataForSend[_dataForSend.Length - 2] = binArr[4];
            _dataForSend[_dataForSend.Length - 1] = binArr[5];
            //##End Creating data

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);


            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            object[] ans = new object[2];
            /* ans[0] = func.ConvertFromByte(new byte[4] { outputData[0], outputData[1], outputData[2], outputData[3] });
             ans[1] = func.ConvertFromByte(new byte[4] { outputData[4], outputData[5], outputData[6], outputData[7] });
             ans[0] = Convert.ToDouble(ans[0]) / 100;
             ans[1] = Convert.ToDouble(ans[1]) / 100;*/

            return ans;
        }//ok
        private object[] Payment(byte type, bool useAddRow, double cash, bool autoclose, string addRow)
        {
            NOM = 20;

            //Creating data
            _dataForSend = new byte[1 + 4 + 1 + addRow.Length];
            _dataForSend[0] = type;
            if (useAddRow)
                _dataForSend[0] |= 0x80;

            if ((type != 3) || (autoclose))
            {
                _dataForSend[1] = 0;
                _dataForSend[2] = 0;
                _dataForSend[3] = 0;
                _dataForSend[4] = 0x80;
            }
            else
            {
                byte[] c = _func.GetByteArray((int)Math.Round(cash * 100), 4);
                _dataForSend[1] = c[0];
                _dataForSend[2] = c[1];
                _dataForSend[3] = c[2];
                _dataForSend[4] = c[3];
            }

            _dataForSend[5] = (byte)addRow.Length;

            byte[] BinLine = Encoding.GetEncoding(866).GetBytes(addRow);
            for (byte i = 0; i < BinLine.Length; i++)
                _dataForSend[i + 6] = BinLine[i];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            object[] ans = new object[2];
            if (_outputData != null && _outputData.Length != 0)
            {
                Array.Reverse(_outputData);
                if ((_outputData[0] & 0x80) == 0)
                    ans[0] = 0;
                else
                    ans[0] = 1;
                _outputData[0] &= 0x7F;
                Array.Reverse(_outputData);
                ans[1] = _func.GetNumber(_outputData);
                ans[1] = Convert.ToDouble(ans[1]) / 100;
            }

            return ans;
        }//ok
        private void SetString(string[] lines)
        {
            Params.DriverData["LastFunc"] = "SetString";
            NOM = 23;

            byte[] BinLine = new byte[0];
            byte j = 0;

            for (byte i = 0; i < lines.Length; i++)
            {
                //Creating data
                _dataForSend = new byte[1 + 1 + lines[i].Length];
                _dataForSend[0] = i;
                _dataForSend[1] = (byte)lines[i].Length;
                BinLine = Encoding.GetEncoding(866).GetBytes(lines[i]);
                for (j = 0; j < BinLine.Length; j++)
                    _dataForSend[j + 2] = BinLine[j];

                //Making data
                _inputData = CreateInputData(NOM, _dataForSend);

                //sending and getting data
                SendGetData(20, true);

                //Next code for command
                GetNextCmdCode();
            }
        }//ok
        private void Give(uint suma)
        {
            Params.DriverData["LastFunc"] = "Give";
            NOM = 24;

            //Creating data
            _dataForSend = _func.GetByteArray(suma, 4);

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void SendCustomer(string[] lines, bool[] show)
        {
            Params.DriverData["LastFunc"] = "SendCustomer";
            NOM = 27;

            byte[] BinLine = new byte[0];
            byte j = 0;

            for (byte i = 0; i < lines.Length; i++)
            {
                if (!show[i])
                    continue;

                lines[i] = lines[i].Replace('і', 'i').Replace('І', 'I');
                BinLine = Encoding.GetEncoding(866).GetBytes(lines[i]);

                //Creating data
                _dataForSend = new byte[2 + BinLine.Length];
                _dataForSend[0] = i;
                _dataForSend[1] = (byte)BinLine.Length;
                for (j = 0; j < BinLine.Length; j++)
                    _dataForSend[j + 2] = BinLine[j];

                //Making data
                _inputData = CreateInputData(NOM, _dataForSend);

                //sending and getting data
                SendGetData(20, true);

                //Next code for command
                GetNextCmdCode();

                //System.Threading.Thread.Sleep(100);
            }
        }//ok   
        private byte[] GetMemory(string adrBlock, byte pageNo, byte blockSize)
        {
            Params.DriverData["LastFunc"] = "GetMemory";
            NOM = 28;

            //Creating data
            _dataForSend = new byte[2 + 1 + 1];
            uint block = Convert.ToUInt32(adrBlock, 16);
            byte[] blok = _func.GetByteArray(block, 2);
            _dataForSend[0] = blok[0];
            _dataForSend[1] = blok[1];
            _dataForSend[2] = pageNo;
            _dataForSend[3] = blockSize;

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            return _outputData;
        }//ok
        private void OpenBox()
        {
            Params.DriverData["LastFunc"] = "OpenBox";
            NOM = 29;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PrintCopy()
        {
            Params.DriverData["LastFunc"] = "PrintCopy";
            NOM = 30;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PrintVer()
        {
            Params.DriverData["LastFunc"] = "PrintVer";
            NOM = 32;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private uint GetBox()
        {
            Params.DriverData["LastFunc"] = "GetBox";
            NOM = 33;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            return (uint)_func.GetNumber(_outputData);
        }//ok
        private void Discount(byte type, double value, byte digitsAfterPoint, string helpLine)
        {
            NOM = 35;

            //Creating data
            _dataForSend = new byte[1 + 4 + 1 + helpLine.Length];
            _dataForSend[0] = type;

            bool isDiscount = true;
            if (value < 0)
                isDiscount = false;

            byte[] disc = new byte[4];
            // v 21:59 19.11.2088 vguvaemo REVO {YURIK, VEDMED, MA4OK}
            if (type == 0 || type == 2)
            {
                disc = _func.GetByteArray((int)Math.Abs(value * 100), 4);
                disc[3] &= (byte)0;
                disc[3] |= (byte)4;
            }
            else
            {
                if (digitsAfterPoint > 127)
                    digitsAfterPoint = 127;
                if (digitsAfterPoint > 0)
                    value *= Math.Pow(10.0, (double)digitsAfterPoint);
                disc = _func.GetByteArray((int)Math.Abs(value), 4);
                disc[3] &= (byte)0;
                disc[3] |= (byte)digitsAfterPoint;
            }

            if (isDiscount)
                disc[3] |= (byte)0x80;//znugka
            else
                disc[3] &= (byte)0x7F;//nadbavka

            _dataForSend[1] = disc[0];
            _dataForSend[2] = disc[1];
            _dataForSend[3] = disc[2];
            _dataForSend[4] = disc[3];

            _dataForSend[5] = (byte)helpLine.Length;
            byte[] BinLine = Encoding.GetEncoding(866).GetBytes(helpLine);
            for (byte j = 0; j < BinLine.Length; j++)
                _dataForSend[j + 6] = BinLine[j];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//!!!!
        private void CplOnline()
        {
            Params.DriverData["LastFunc"] = "CplOnline";
            NOM = 36;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void CplInd()
        {
            Params.DriverData["LastFunc"] = "CplInd";
            NOM = 37;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void ChangeRate(byte rateType)
        {
            Params.DriverData["LastFunc"] = "ChangeRate";
            NOM = 38;

            //Creationg data
            _dataForSend = new byte[1];
            _dataForSend[0] = rateType;

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void LineSP(byte lsp)
        {
            Params.DriverData["LastFunc"] = "LineSP";
            NOM = 39;

            //Creating data
            _dataForSend = new byte[1];
            _dataForSend[0] = lsp;

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void TransPrint(string line, bool close)
        {
            Params.DriverData["LastFunc"] = "TransPrint";
            NOM = 40;

            //Creating data
            _dataForSend = new byte[1 + line.Length];
            _dataForSend[0] = (byte)line.Length;
            byte[] BinLine = Encoding.GetEncoding(866).GetBytes(line.Replace('і', 'i').Replace('І', 'I'));
            for (byte i = 0; i < BinLine.Length; i++)
                _dataForSend[i + 1] = BinLine[i];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            //System.Threading.Thread.Sleep(200);
            if (close)
            {
                //Creating data
                _dataForSend = new byte[1];
                _dataForSend[0] = 0xFF;

                //Making data
                _inputData = CreateInputData(NOM, _dataForSend);

                //sending and getting data
                if (_port.IsOpen)
                    if (_port.Write(_inputData))
                        if (_port.Read(ref _outputData, out _readedBytes))
                            DecodeAnswer();

                //Next code for command
                GetNextCmdCode();
            }
        }//ok
        private object[] GetArticle(uint id)
        {
            Params.DriverData["LastFunc"] = "GetArticle";
            NOM = 41;

            //Making data
            byte[] aid = _func.GetByteArray(id, 6);
            _inputData = CreateInputData(NOM, aid);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            object[] article = new object[0];
            if (_outputData.Length != 0)
            {
                byte nameLength = (byte)_outputData[0];
                string artName = string.Empty;
                byte doseDecimal = 0;
                double dose = 0;
                double price = 0;
                char tax = ' ';
                double rotate = 0;
                double retriveDose = 0;
                byte retriveDoseDecimal = 0;
                double retriveRotate = 0;

                artName = Encoding.GetEncoding(866).GetString(_outputData, 1, nameLength);
                //dose
                byte[] mas = new byte[3];
                mas[0] = _outputData[1 + nameLength];
                mas[1] = _outputData[1 + nameLength + 1];
                mas[2] = _outputData[1 + nameLength + 2];
                dose = _func.GetNumber(mas) + 0.0;

                doseDecimal = _outputData[1 + nameLength + 3];

                mas = new byte[4];
                mas[0] = _outputData[1 + nameLength + 3 + 1];
                mas[1] = _outputData[1 + nameLength + 3 + 1 + 1];
                mas[2] = _outputData[1 + nameLength + 3 + 1 + 2];
                mas[3] = _outputData[1 + nameLength + 3 + 1 + 3];
                price = (double)_func.GetNumber(mas) / 100;

                tax = (char)_outputData[1 + nameLength + 3 + 1 + 4];
                switch (System.Convert.ToString(tax, 16))
                {
                    case "80": tax = 'А'; break;
                    case "81": tax = 'Б'; break;
                    case "82": tax = 'В'; break;
                    case "83": tax = 'Г'; break;
                    case "84": tax = 'Д'; break;
                    case "85": tax = 'Е'; break;
                }

                mas = new byte[5];
                mas[0] = _outputData[1 + nameLength + 3 + 1 + 4 + 1];
                mas[1] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 1];
                mas[2] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 2];
                mas[3] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 3];
                mas[4] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 4];
                rotate = (double)_func.GetNumber(mas) / 100;

                mas = new byte[3];
                mas[0] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 5];
                mas[1] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 1];
                mas[2] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 2];
                retriveDose = _func.GetNumber(mas) + 0.0;

                retriveDoseDecimal = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3];

                mas = new byte[5];
                mas[0] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3 + 1];
                mas[1] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3 + 1 + 1];
                mas[2] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3 + 1 + 2];
                mas[3] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3 + 1 + 3];
                mas[4] = _outputData[1 + nameLength + 3 + 1 + 4 + 1 + 5 + 3 + 1 + 4];
                retriveRotate = (double)_func.GetNumber(mas) / 100;

                dose /= System.Math.Pow(10.0, (double)doseDecimal);
                retriveDose /= System.Math.Pow(10.0, (double)retriveDoseDecimal);

                article = new object[7];
                article[0] = artName;
                article[1] = dose;
                article[2] = price;
                article[3] = tax;
                article[4] = rotate;
                article[5] = retriveDose;
                article[6] = retriveRotate;
            }

            return article;
        }//ok
        private object[] GetDayReport()
        {
            Params.DriverData["LastFunc"] = "GetDayReport";
            NOM = 42;

            //Creating data
            _dataForSend = new byte[0];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();


            object[] rezDayInfo = new object[0];
            if (_outputData.Length == 134)
            {
                rezDayInfo = new object[10];
                byte[] arr_two = new byte[2];
                byte[] arr_five = new byte[5];
                long[] int_mas = new long[0];

                double[] sales_tax = new double[6];
                double[] sales_types = new double[4];
                double[] payment_tax = new double[6];
                double[] payment_types = new double[4];

                arr_two[0] = _outputData[0];
                arr_two[1] = _outputData[1];
                rezDayInfo[0] = _func.GetNumberFromBCD(arr_two);

                for (int i = 0, j = 0, k = 0; i < sales_tax.Length; i++)
                {
                    for (j = 0; j < arr_five.Length; j++, k++)
                        arr_five[j] = _outputData[2 + k];
                    sales_tax[i] = ((double)_func.GetNumber(arr_five) / 100);
                }

                for (int i = 0, j = 0, k = 0; i < sales_types.Length; i++)
                {
                    for (j = 0; j < arr_five.Length; j++, k++)
                        arr_five[j] = _outputData[2 + (6 * 5) + k];
                    sales_types[i] = ((double)_func.GetNumber(arr_five) / 100);
                }

                rezDayInfo[1] = new object[] { sales_tax, sales_types };

                arr_five[0] = _outputData[52];
                arr_five[1] = _outputData[53];
                arr_five[2] = _outputData[54];
                arr_five[3] = _outputData[55];
                arr_five[4] = _outputData[56];
                rezDayInfo[2] = ((double)_func.GetNumber(arr_five) / 100);
                arr_five[0] = _outputData[57];
                arr_five[1] = _outputData[58];
                arr_five[2] = _outputData[59];
                arr_five[3] = _outputData[60];
                arr_five[4] = _outputData[61];
                rezDayInfo[3] = ((double)_func.GetNumber(arr_five) / 100);
                arr_five[0] = _outputData[62];
                arr_five[1] = _outputData[63];
                arr_five[2] = _outputData[64];
                arr_five[3] = _outputData[65];
                arr_five[4] = _outputData[66];
                rezDayInfo[4] = ((double)_func.GetNumber(arr_five) / 100);
                arr_two[0] = _outputData[67];
                arr_two[1] = _outputData[68];
                rezDayInfo[5] = _func.GetNumberFromBCD(arr_two);

                for (int i = 0, j = 0, k = 0; i < payment_tax.Length; i++)
                {
                    for (j = 0; j < arr_five.Length; j++, k++)
                        arr_five[j] = _outputData[2 + 5 * (6 + 4) + 5 + 5 + 5 + 2 + k];
                    payment_tax[i] = ((double)_func.GetNumber(arr_five) / 100);
                }

                for (int i = 0, j = 0, k = 0; i < payment_types.Length; i++)
                {
                    for (j = 0; j < arr_five.Length; j++, k++)
                        arr_five[j] = _outputData[2 + 5 * (6 + 4) + 5 + 5 + 5 + 2 + 6 * 5 + k];
                    payment_types[i] = ((double)_func.GetNumber(arr_five) / 100);
                }

                rezDayInfo[6] = new object[] { payment_tax, payment_types };

                arr_five[0] = _outputData[119];
                arr_five[1] = _outputData[120];
                arr_five[2] = _outputData[121];
                arr_five[3] = _outputData[122];
                arr_five[4] = _outputData[123];
                rezDayInfo[7] = ((double)_func.GetNumber(arr_five) / 100);
                arr_five[0] = _outputData[124];
                arr_five[1] = _outputData[125];
                arr_five[2] = _outputData[126];
                arr_five[3] = _outputData[127];
                arr_five[4] = _outputData[128];
                rezDayInfo[8] = ((double)_func.GetNumber(arr_five) / 100);
                arr_five[0] = _outputData[129];
                arr_five[1] = _outputData[130];
                arr_five[2] = _outputData[131];
                arr_five[3] = _outputData[132];
                arr_five[4] = _outputData[133];
                rezDayInfo[9] = ((double)_func.GetNumber(arr_five) / 100);
            }

            return rezDayInfo;
        }//ok
        private object[] GetCheckSums()
        {
            Params.DriverData["LastFunc"] = "GetCheckSums";
            NOM = 43;
            //Creating data
            _dataForSend = new byte[0];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            object[] checkSumsInfo = new object[0];

            if (_outputData.Length == 40)
            {
                double[] tax_group = new double[6];
                double[] sum_group = new double[4];
                byte[] currTaxRotate = new byte[4];
                byte[] currSumRotate = new byte[4];

                for (int i = 0, j = 0, k = 0; i < tax_group.Length; i++)
                {
                    for (j = 0; j < currTaxRotate.Length; j++, k++)
                        currTaxRotate[j] = _outputData[k];

                    tax_group[i] = ((double)_func.GetNumber(currTaxRotate) / 100);
                }

                for (int i = 0, j = 0, k = 0; i < sum_group.Length; i++)
                {
                    for (j = 0; j < currTaxRotate.Length; j++, k++)
                        currSumRotate[j] = _outputData[4 * 6 + k];

                    sum_group[i] = ((double)_func.GetNumber(currSumRotate) / 100);
                }

                checkSumsInfo = new object[2];
                checkSumsInfo[0] = tax_group;
                checkSumsInfo[1] = sum_group;
            }

            return checkSumsInfo;
        }//ok
        private object[] GetTaxRates()
        {
            Params.DriverData["LastFunc"] = "GetTaxRates";
            NOM = 44;

            //Making data
            _dataForSend = new byte[0];
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();

            object[] taxInfoRezult = new object[0];

            if (_outputData.Length >= (1 + 3 + 0 + 1 + 0))
            {
                byte totTax = (byte)_outputData[0];

                int[] ans = _func.GetArrayFromBCD(new byte[] { _outputData[1], _outputData[2], _outputData[3] });
                DateTime dt = new DateTime();
                try
                {
                    dt = new DateTime(ans[2] + 2000, ans[1], ans[0]);
                }
                catch { }

                byte[] tax = new byte[2 * totTax];
                Array.Copy(_outputData, 4, tax, 0, tax.Length);
                double[] taxRates = new double[totTax];
                byte[] currRate = new byte[2];
                for (int i = 0; i < tax.Length; i += 2)
                {
                    currRate[0] = tax[i];
                    currRate[1] = tax[i + 1];
                    taxRates[i / 2] = _func.GetNumber(currRate) / 100;
                }

                byte status = (byte)_outputData[4 + 2 * totTax];
                byte decimals = (byte)(status & 0x0F);
                byte taxType = (byte)(status & 0x10);
                byte useGetTax = (byte)(status & 0x20);

                if (useGetTax == 0)
                {
                    taxInfoRezult = new object[6];
                    taxInfoRezult[0] = totTax;
                    taxInfoRezult[1] = dt;
                    taxInfoRezult[2] = taxRates;
                    taxInfoRezult[3] = decimals;
                    taxInfoRezult[4] = taxType;
                    taxInfoRezult[5] = false;

                }
                else
                {
                    Array.Copy(_outputData, 5 + 2 * totTax, tax, 0, tax.Length);
                    double[] gtaxRates = new double[totTax];
                    for (int i = 0; i < tax.Length; i += 2)
                    {
                        currRate[0] = tax[i];
                        currRate[1] = tax[i + 1];
                        gtaxRates[i / 2] = _func.GetNumber(currRate) / 100;
                    }

                    taxInfoRezult = new object[7];
                    taxInfoRezult[0] = totTax;
                    taxInfoRezult[1] = dt;
                    taxInfoRezult[2] = taxRates;
                    taxInfoRezult[3] = decimals;
                    taxInfoRezult[4] = taxType;
                    taxInfoRezult[5] = true;
                    taxInfoRezult[6] = gtaxRates;
                }

            }
            return taxInfoRezult;
        }//ok
        private void CplCutter()
        {
            Params.DriverData["LastFunc"] = "CplCutter";
            NOM = 46;

            //Making data
            _inputData = CreateInputData(NOM, new byte[0]);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        #endregion
        #region Function of Fiscalization
        private void Fiscalization(ushort progPass, string fn)
        {
            Params.DriverData["LastFunc"] = "Fiscalization";
            NOM = 21;

            //Creating data
            byte[] pass = _func.GetByteArray(progPass, 2);
            _dataForSend = new byte[2 + 10];
            _dataForSend[0] = pass[0];
            _dataForSend[1] = pass[1];
            Encoding.GetEncoding(866).GetBytes(fn).CopyTo(_dataForSend, 2);

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void SetHeadLine(ushort progPass, string line1, string line2, string line3, string line4)
        {
            Params.DriverData["LastFunc"] = "SetHeadLine";
            NOM = 25;

            //Creating data
            _dataForSend = new byte[2 + 1 + line1.Length + 1 + line2.Length + 1 + line3.Length + 1 + line4.Length];

            byte[] pass = _func.GetByteArray(progPass, 2);

            _dataForSend[0] = pass[0];
            _dataForSend[1] = pass[1];
            _dataForSend[2] = (byte)line1.Length;
            Encoding.GetEncoding(866).GetBytes(line1).CopyTo(_dataForSend, 3);
            _dataForSend[2 + 1 + line1.Length] = (byte)line2.Length;
            Encoding.GetEncoding(866).GetBytes(line2).CopyTo(_dataForSend, 2 + 1 + line1.Length + 1);
            _dataForSend[2 + 1 + line1.Length + 1 + line2.Length] = (byte)line3.Length;
            Encoding.GetEncoding(866).GetBytes(line3).CopyTo(_dataForSend, 2 + 1 + line1.Length + 1 + line2.Length + 1);
            _dataForSend[2 + 1 + line1.Length + 1 + line2.Length + 1 + line3.Length] = (byte)line4.Length;
            Encoding.GetEncoding(866).GetBytes(line4).CopyTo(_dataForSend, 2 + 1 + line1.Length + 1 + line2.Length + 1 + line3.Length + 1);

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void SetTaxRate(ushort progPass, byte totTax, uint[] tax, byte status, byte totGTax, uint[] gtax)
        {
            Params.DriverData["LastFunc"] = "SetTaxRate";
            NOM = 25;

            //Creating data
            _dataForSend = new byte[2 + 1 + 2 * totTax + 1 + 2 * totGTax];

            byte[] pass = _func.GetByteArray(progPass, 2);
            _dataForSend[0] = pass[0];
            _dataForSend[1] = pass[1];
            _dataForSend[2] = totTax;

            byte[] currTRate = new byte[2];
            for (int i = 0; i < 2 * totTax; i += 2)
            {
                currTRate = _func.GetByteArray(tax[i / 2], 2);
                _dataForSend[3 + i] = currTRate[0];
                _dataForSend[3 + (i + 1)] = currTRate[1];
            }

            _dataForSend[3 + 2 * totTax] = status;

            for (int i = 0; i < 2 * totGTax; i += 2)
            {
                currTRate = _func.GetByteArray(gtax[i / 2], 2);
                _dataForSend[3 + 2 * totTax + 1 + i] = currTRate[0];
                _dataForSend[3 + 2 * totTax + 1 + (i + 1)] = currTRate[1];
            }

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void ProgArt(ushort progPass, byte doseDecimal, double price, char pdv, string name, uint id)
        {
            Params.DriverData["LastFunc"] = "ProgArt";
            NOM = 34;

            //Creating data
            byte[] binArr = null;
            byte oneByte = 0x00;

            _dataForSend = new byte[2 + 1 + 4 + 1 + 1 + name.Length + 2];

            binArr = _func.GetByteArray(progPass, 2);
            _dataForSend[0] = binArr[0];
            _dataForSend[1] = binArr[1];
            _dataForSend[2] = doseDecimal;
            //Price
            binArr = _func.GetByteArray((int)Math.Round(Math.Abs(price) * 100), 4);
            _dataForSend[3] = binArr[0];
            _dataForSend[4] = binArr[1];
            _dataForSend[5] = binArr[2];
            if (price < 0)
                binArr[3] |= 0x80;
            _dataForSend[6] = binArr[3];
            //Tax
            oneByte = 0x00;
            switch (pdv)
            {
                case 'А': { oneByte = 0x80; break; }
                case 'Б': { oneByte = 0x81; break; }
                case 'В': { oneByte = 0x82; break; }
                case 'Г': { oneByte = 0x83; break; }
                case 'Д': { oneByte = 0x84; break; }
                case 'Е': { oneByte = 0x85; break; }
                //TAX E (no tax)
                default: { oneByte = 0x85; break; }
            }
            _dataForSend[7] = oneByte;
            //Art Name length
            _dataForSend[8] = (byte)name.Length;
            //Art Name
            binArr = Encoding.GetEncoding(866).GetBytes(name);
            binArr.CopyTo(_dataForSend, 9);
            //Art id
            binArr = _func.GetByteArray(id, 2);
            _dataForSend[_dataForSend.Length - 2] = binArr[0];
            _dataForSend[_dataForSend.Length - 1] = binArr[1];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }
        private void LoadBMP(ushort progPass, bool allow, string fpath)
        {
            Params.DriverData["LastFunc"] = "LoadBMP";
            NOM = 45;

            byte[] binArr = null;
            byte[] pass = _func.GetByteArray((uint)progPass, 2);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(fpath);
            byte[] img = new byte[bmp.Width / 8 * bmp.Height];
            byte oneByte = 0x00;
            int k = 0;
            int wo = 0;

            for (int h = 0, w = 0; h < bmp.Height; h++)
                for (w = 0; w < (bmp.Width / 8 * 8); w += 8)
                {
                    for (wo = 0; wo < 8; wo++)
                    {
                        oneByte <<= 1;
                        if (bmp.GetPixel(w + wo, h).Name == "ff000000")
                            oneByte |= 1;
                    }

                    img[k] = oneByte;
                    k++;
                }
            k = img.Length / 64;
            if (img.Length % 64 != 0)
                k++;
            object[] imgBlocks = new object[k];
            long idx = 0;
            byte cs = 0;
            for (k = 0; k < imgBlocks.Length; k++)
            {
                if (idx + 64 < img.Length)
                    binArr = new byte[64];
                else
                    binArr = new byte[img.Length - idx];

                Array.Copy(img, idx, binArr, 0, binArr.Length);
                cs = (byte)(0 - _func.SumMas(binArr));
                Array.Resize<byte>(ref binArr, binArr.Length + 1);
                binArr[binArr.Length - 1] = cs;
                imgBlocks[k] = binArr;
                idx += 64;
            }
            //Creating data
            _dataForSend = new byte[2 + 1 + 2 + 2];
            _dataForSend[0] = pass[0];
            _dataForSend[1] = pass[1];

            if (allow)
                _dataForSend[2] = (byte)1;
            else
                _dataForSend[2] = (byte)0;

            binArr = _func.GetByteArray(bmp.Width / 8 * 8, 2);
            _dataForSend[3] = binArr[0];
            _dataForSend[4] = binArr[1];

            binArr = _func.GetByteArray(bmp.Height, 2);
            _dataForSend[5] = binArr[0];
            _dataForSend[6] = binArr[1];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data

            if (!_port.IsOpen)
                _port.Open();

            byte[] buffer = new byte[1];
            uint rb = 20;
            int totRd = 20;
            bool enqDetected = false;
            _port.Write(_inputData);
            for (k = 0; k < imgBlocks.Length; k++)
            {
                totRd = 20;
                enqDetected = false;
                buffer = new byte[1];
                while (true)
                {
                    if (totRd < 0)
                        break;
                    _port.Read(ref buffer, out rb);
                    if (buffer[0] == ENQ)
                    {
                        _port.PortClear();
                        enqDetected = true;
                        break;
                    }
                    if (buffer[0] == SYN)
                        continue;
                    totRd--;
                }

                if (!enqDetected)
                    break;

                _port.Write((byte[])imgBlocks[k]);
            }
            _port.Close();

            //Next code for command
            GetNextCmdCode();
        }
        #endregion
        #region Function of reports
        private void ArtReport(uint reportPass)
        {
            Params.DriverData["LastFunc"] = "ArtReport";
            NOM = 7;

            //Creating data
            _dataForSend = _func.GetByteArray(reportPass, 2);

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void ArtXReport(uint reportPass)
        {
            Params.DriverData["LastFunc"] = "ArtXReport";
            NOM = 10;

            //Creating data
            _dataForSend = _func.GetByteArray(reportPass, 2);

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void DayReport(uint reportPass)
        {
            Params.DriverData["LastFunc"] = "DayReport";
            NOM = 9;

            //Creating data
            _dataForSend = _func.GetByteArray(reportPass, 2);

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void DayClrReport(uint reportPass)
        {
            Params.DriverData["LastFunc"] = "DayClrReport";
            NOM = 13;

            //Creating data
            _dataForSend = _func.GetByteArray(reportPass, 2);

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //SaveArtID(0);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PeriodicReport(uint reportPass, DateTime startDate, DateTime endDate)
        {
            Params.DriverData["LastFunc"] = "PeriodicReport";
            NOM = 17;

            //Creating data
            _dataForSend = new byte[2 + 3 + 3];
            byte[] p = _func.GetByteArray(reportPass, 2);
            _dataForSend[0] = p[0];
            _dataForSend[1] = p[1];
            byte[] date = _func.GetBCDFromArray(new int[] { startDate.Day, startDate.Month, int.Parse(startDate.Year.ToString().Substring(2, 2)) });
            //BCD sdd = new BCD(startDate.Day);
            //BCD sdm = new BCD(startDate.Month);
            //BCD sdy = new BCD(int.Parse(startDate.Year.ToString().Substring(2, 2)));
            //Data[2] = sdd.CompressedBCD[0];
            //Data[3] = sdm.CompressedBCD[0];
            //Data[4] = sdy.CompressedBCD[0];
            _dataForSend[2] = date[0];
            _dataForSend[3] = date[1];
            _dataForSend[4] = date[2];
            //BCD edd = new BCD(endDate.Day);
            //BCD edm = new BCD(endDate.Month);
            //BCD edy = new BCD(int.Parse(endDate.Year.ToString().Substring(2, 2)));
            date = _func.GetBCDFromArray(new int[] { endDate.Day, endDate.Month, int.Parse(endDate.Year.ToString().Substring(2, 2)) });
            _dataForSend[5] = date[0];
            _dataForSend[6] = date[1];
            _dataForSend[7] = date[2];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PeriodicReportShort(uint reportPass, DateTime startDate, DateTime endDate)
        {
            Params.DriverData["LastFunc"] = "PeriodicReportShort";
            NOM = 26;

            //Creating data
            _dataForSend = new byte[2 + 3 + 3];
            byte[] p = _func.GetByteArray(reportPass, 2);
            _dataForSend[0] = p[0];
            _dataForSend[1] = p[1];
            byte[] date = _func.GetBCDFromArray(new int[] { startDate.Day, startDate.Month, int.Parse(startDate.Year.ToString().Substring(2, 2)) });
            //BCD sdd = new BCD(startDate.Day);
            //BCD sdm = new BCD(startDate.Month);
            //BCD sdy = new BCD(int.Parse(startDate.Year.ToString().Substring(2, 2)));
            //Data[2] = sdd.CompressedBCD[0];
            //Data[3] = sdm.CompressedBCD[0];
            //Data[4] = sdy.CompressedBCD[0];
            _dataForSend[2] = date[0];
            _dataForSend[3] = date[1];
            _dataForSend[4] = date[2];
            //BCD edd = new BCD(endDate.Day);
            //BCD edm = new BCD(endDate.Month);
            //BCD edy = new BCD(int.Parse(endDate.Year.ToString().Substring(2, 2)));
            date = _func.GetBCDFromArray(new int[] { endDate.Day, endDate.Month, int.Parse(endDate.Year.ToString().Substring(2, 2)) });
            _dataForSend[5] = date[0];
            _dataForSend[6] = date[1];
            _dataForSend[7] = date[2];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        private void PeriodicReport2(uint reportPass, uint startID, uint endID)
        {
            Params.DriverData["LastFunc"] = "PeriodicReport2";
            NOM = 31;

            //Creating data
            _dataForSend = new byte[2 + 2 + 2];
            byte[] ps = _func.GetByteArray(reportPass, 2);
            _dataForSend[0] = ps[0];
            _dataForSend[1] = ps[1];
            byte[] sid = _func.GetByteArray(startID, 2);
            _dataForSend[2] = sid[0];
            _dataForSend[3] = sid[1];
            byte[] eid = _func.GetByteArray(endID, 2);
            _dataForSend[4] = eid[0];
            _dataForSend[5] = eid[1];

            //Making data
            _inputData = CreateInputData(NOM, _dataForSend);

            //sending and getting data
            SendGetData(20, true);

            //Next code for command
            GetNextCmdCode();
        }//ok
        #endregion
        #endregion

        // Driver's Application Interface (Access Methods)
        // Need for access to FP from main program
        protected virtual void FP_Sale(object[] param)
        {
            // Return if parameters are empty
            if (_func.IsEmpty(param))
                return;

            // Check if function has errors
            if ((bool)Params.ErrorFlags["FP_Payment"])
            {
                ResetOrder();
                Params.ErrorFlags["FP_Payment"] = false;
                this._param.Save();
            }
            
            if ((bool)Params.ErrorFlags["FP_Sale"])
            {
                ResetOrder();
                Params.ErrorFlags["FP_Sale"] = false;
                this._param.Save();
            }

            Exception ex = null;

            try
            {
                // prev: uint nextArticleNo = uint.Parse(Params.DriverData["LastArtNo"].ToString());
                //uint articleNewID = 0;// LoadArtID(port);
                uint nextArticleNo = (uint)_func.GetNumber(GetMemory("0065", (byte)16, (byte)2));

                if (nextArticleNo + 50 == (uint)Params.DriverData["ArtMemorySize"])
                {
                    System.Windows.Forms.MessageBox.Show("Немає вільної пам'яті для здійснення продажу\r\nНеохідно зробити Z-звіт для наступного продажу",
                        Name, System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }

                System.Data.DataTable dTable = (System.Data.DataTable)param[0];
                object[] article = new object[5];
                byte dose_ppt = Convert.ToByte(param[1]);
                byte money_ppt = Convert.ToByte(param[3]);
                bool useTotDisc = (bool)param[2];

                System.IO.StreamWriter sWr = null;

                for (int i = 0; i < dTable.Rows.Count; i++)
                {
                    article[0] = _func.GetDouble(dTable.Rows[i]["TOT"]);
                    article[2] = _func.GetDouble(dTable.Rows[i]["PRICE"]);
                    article[3] = dTable.Rows[i]["VG"];
                    article[4] = dTable.Rows[i]["NAME"].ToString().Replace('і', 'i').Replace('І', 'I');
                    nextArticleNo++;

                    sWr = System.IO.File.AppendText(string.Format("reports\\report_sale_{0}.txt", DateTime.Now.ToShortDateString()));
                    sWr.WriteLine(nextArticleNo);
                    sWr.Close();
                    sWr.Dispose();
                    Sale((double)article[0], dose_ppt, false, (double)article[2],
                        article[3].ToString()[0], article[4].ToString(), nextArticleNo, money_ppt);
                    if (!useTotDisc && (bool)dTable.Rows[i]["USEDDISC"] && (double)dTable.Rows[i]["DISC"] != 0)
                        FP_Discount(new object[] { (byte)0, (double)dTable.Rows[i]["DISC"], money_ppt, string.Empty });
                    //(byte)0, (double)dTable.Rows[i]["DISC"], money_ppt, string.Empty
                }
                sWr = System.IO.File.AppendText(string.Format("reports\\report_sale_{0}.txt", DateTime.Now.ToShortDateString()));
                sWr.WriteLine("-----");
                sWr.Close();
                sWr.Dispose();

                Params.DriverData["LastArtNo"] = nextArticleNo;
                Params.ErrorFlags["FP_Sale"] = false;
                this._param.Save();
                return;
            }
            catch (Exception _ex)
            {
                Params.ErrorFlags["FP_Sale"] = true;
                ex = _ex;
            }
            this._param.Save();
            throw new Exception(ex.Message, ex);
        }
        protected virtual void FP_PayMoney(object[] param)
        {
            // Return if parameters are empty
            if (_func.IsEmpty(param))
                return;

            // Check last executed function for error
            if ((bool)Params.ErrorFlags["FP_Payment"])
                return;

            if ((bool)Params.ErrorFlags["FP_PayMoney"])
            {
                ResetOrder();
                Params.ErrorFlags["FP_PayMoney"] = false;
                this._param.Save();
            }

            Exception ex = null;

            try
            {
                // prev: uint nextArticleNo = (uint)Params.DriverData["LastArtNo"];
                //uint articleNewID = 0;// LoadArtID(port);
                uint nextArticleNo = (uint)_func.GetNumber(GetMemory("0065", (byte)16, (byte)2));

                if (nextArticleNo + 50 == (uint)Params.DriverData["ArtMemorySize"])
                {
                    System.Windows.Forms.MessageBox.Show("Немає вільної пам'яті для здійснення продажу\r\nНеохідно зробити Z-звіт для наступного продажу",
                        Name, System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }

                System.Data.DataTable dTable = (System.Data.DataTable)param[0];
                object[] article = new object[5];
                byte dose_ppt = Convert.ToByte(param[1]);
                byte money_ppt = Convert.ToByte(param[3]);
                bool useTotDisc = (bool)param[2];

                System.IO.StreamWriter sWr = null;

                for (int i = 0; i < dTable.Rows.Count; i++)
                {
                    article[0] = _func.GetDouble(dTable.Rows[i]["TOT"]);
                    article[2] = _func.GetDouble(dTable.Rows[i]["PRICE"]);
                    article[3] = dTable.Rows[i]["VG"];
                    article[4] = dTable.Rows[i]["NAME"].ToString().Replace('і', 'i').Replace('І', 'I');
                    nextArticleNo++;

                    sWr = System.IO.File.AppendText(string.Format("reports\\report_sale_{0}.txt", DateTime.Now.ToShortDateString()));
                    sWr.WriteLine(nextArticleNo);
                    sWr.Close();
                    sWr.Dispose();
                    PayMoney((double)article[0], dose_ppt, false, (double)article[2],
                        article[3].ToString()[0], article[4].ToString(), nextArticleNo, money_ppt);
                    if (!useTotDisc && (bool)dTable.Rows[i]["USEDDISC"] && (double)dTable.Rows[i]["DISC"] != 0)
                        FP_Discount(new object[] { (byte)0, (double)dTable.Rows[i]["DISC"], money_ppt, string.Empty });
                    //(byte)0, (double)dTable.Rows[i]["DISC"], money_ppt, string.Empty
                }
                sWr = System.IO.File.AppendText(string.Format("reports\\report_sale_{0}.txt", DateTime.Now.ToShortDateString()));
                sWr.WriteLine("-----");
                sWr.Close();
                sWr.Dispose();

                Params.DriverData["LastArtNo"] = nextArticleNo;
                Params.ErrorFlags["FP_Sale"] = false;
                this._param.Save();
                return;
            }
            catch (Exception _ex)
            {
                Params.ErrorFlags["FP_Sale"] = true;
                ex = _ex;
            }
            this._param.Save();
            throw new Exception(ex.Message, ex);

        }
        protected virtual void FP_Payment(object[] param)
        {
            // Return if parameters are empty
            if (_func.IsEmpty(param))
                return;

            // Check last executed function for error
            if ((bool)Params.ErrorFlags["FP_Sale"] ||
                (bool)Params.ErrorFlags["FP_PayMoney"])
            {
                ResetOrder();
                Params.ErrorFlags["FP_Sale"] = false;
                Params.ErrorFlags["FP_PayMoney"] = false;
                this._param.Save();
                return;
            }

            // Try to perform commands
            Exception ex = null;
            try
            {
                // Indicate payment type
                Payment((byte)param[0], false, (double)param[1], (bool)param[2], string.Empty);
                // Set current order number
                Params.ErrorFlags["FP_Payment"] = false;
                this._param.Save();
                return;
            }
            catch (Exception _ex)
            {
                Params.ErrorFlags["FP_Payment"] = true;
                ex = _ex;
            }

            this._param.Save();
            throw new Exception(ex.Message, ex);
        }
        protected virtual void FP_Discount(object[] param)
        {
            // Return if parameters are empty
            if (_func.IsEmpty(param))
                return;

            // Check if function has errors
            if ((bool)Params.ErrorFlags["FP_Discount"])
            {
                ResetOrder();
                Params.ErrorFlags["FP_Discount"] = false;
				this._param.Save();
                return;
            }

            // Try to perform commands
            Exception ex = null;
            try
            {
                byte _discType = (byte)param[0];
                double _discValue = (double)param[1];
                byte digitsAfterPoint = byte.Parse(param[2].ToString());
                string helpLine = param[3].ToString();
                Discount(_discType, _discValue, digitsAfterPoint, helpLine);
                //double[] _d = SubTotal(true, false, _discValue, _discType);
                Params.ErrorFlags["FP_Discount"] = false;
                this._param.Save();
                return;
            }
            catch (Exception _ex)
            {
                Params.ErrorFlags["FP_Discount"] = true;
                ex = _ex;
            }

            this._param.Save();
            throw new Exception(ex.Message, ex);
        }
        protected virtual uint FP_LastChqNo(object[] param)
        {
            uint cfnom = 0;
            byte[] mem = null;
            try
            {
                bool isRetrive = (bool)param[0];
                if (isRetrive)
                {
                    string memoryAddr = "30AB";
                    if (Params.Compatibility.ContainsKey("OP6")) memoryAddr = "3077";
                    mem = GetMemory(memoryAddr, (byte)16, (byte)2);
                    Params.DriverData["LastFOrderNo"] = cfnom = uint.Parse(mem[1].ToString("X") + mem[0].ToString("X"));
                }
                else
                {
                    mem = GetMemory("301B", (byte)16, (byte)2);
                    Params.DriverData["LastROrderNo"] = cfnom = uint.Parse(mem[1].ToString("X") + mem[0].ToString("X"));
                }

                using (System.IO.StreamWriter sWr = System.IO.File.AppendText(string.Format("reports\\report_sale_{0}.txt", DateTime.Now.ToShortDateString())))
                {
                    sWr.WriteLine(":] closed cheque: " + mem[0] + "|" + mem[1]);
                    sWr.WriteLine("----->>>");
                    sWr.Close();
                }
            }
            catch { }


            return cfnom;
            //return (uint)Params.DriverData["LastFOrderNo"]; and LastROrderNo

        }
        protected virtual uint FP_LastZRepNo(object[] param)
        {
            byte[] _mem = GetMemory("0037", (byte)16, (byte)2);
            uint _zn = (uint)_func.GetNumberFromBCD(_mem, 10);
            return _zn;
        }
        protected virtual void FP_OpenBox()
        {
            this.OpenBox();
        }
        protected virtual bool FP_SetCashier(object[] param)
        {
            bool rez = false;
            // Return if parameters are empty
            if (_func.IsEmpty(param))
                return rez;

            // Try to perform commands
            try
            {
                byte _pdId = (byte)param[0];
                string _userNo = param[1].ToString();
                string _userPwd = param[2].ToString();
                string _userId = param[3].ToString();

                /*
                if (_userPwd.Length < 4)
                    _userPwd = _userPwd.PadLeft(4, '0');*/

                Params.DriverData["DeskNo"] = _pdId;
                Params.DriverData["UserNo"] = _userNo;
                Params.DriverData["UserPwd"] = _userPwd;

                SetCashier(Convert.ToByte(_userNo), uint.Parse(_userPwd), _userId);

                rez = true;
            }
            catch { rez = false; }

            return rez;
        }
        protected virtual void FP_SendCustomer(object[] param)
        {
            // Return if parameters are empty
            if (_func.IsEmpty(param))
                return;

            // Try to perform commands
            try
            {
                string[] _lines = (string[])param[0];
                bool[] _show = (bool[])param[1];
                this.SendCustomer(_lines, _show);
                //this.DisplBotLine(_lines[1], _show[1]);
            }
            catch { }
        }

        // Base Methods
        // Implementation of them can be different for other drivers
        private void GetNextCmdCode()
        {
            COD++;
        }
        private byte[] CreateInputData(byte _nom, byte[] param)
        {
            byte[] mas = new byte[4 + param.Length + 3];
            byte i = 0;

            mas[0] = DLE;
            mas[1] = STX;
            mas[2] = COD;
            mas[3] = _nom;

            CS = (byte)(0 - (COD + _func.SumMas(param) + _nom));

            for (i = 0; i < param.Length; i++)
                mas[i + 4] = param[i];

            mas[4 + param.Length] = CS;
            mas[4 + param.Length + 1] = DLE;
            mas[4 + param.Length + 2] = ETX;

            byte[] validData = new byte[0];

            for (i = 0; i < mas.Length; i++)
            {
                if (mas[i] == DLE && i != 0 && i != mas.Length - 2)
                {
                    Array.Resize<byte>(ref validData, validData.Length + 1);
                    validData[validData.Length - 1] = DLE;
                }

                Array.Resize<byte>(ref validData, validData.Length + 1);
                validData[validData.Length - 1] = mas[i];
            }

            return validData;
        }
        protected virtual bool SendGetData(int totRead, bool close)
        {
            components.Components.WinApi.Com_WinApi.OutputDebugString("SendGetData");

            string exceptionMsg = "";

            if (!_port.IsOpen)
                try
                {
                    _port.Open();
                }
                catch (Exception ex)
                {
                    components.Components.WinApi.Com_WinApi.OutputDebugString("Port Open Exception: " + ex.Message);
                }

            bool portIsOpen = _port.IsOpen;

            if (portIsOpen)
                components.Components.WinApi.Com_WinApi.OutputDebugString("Port Is OPEN");

            if (_port.Write(_inputData))
            {
                components.Components.WinApi.Com_WinApi.OutputDebugString("Port Write InputData OK");

                // TEST
                // Thread.Sleep(1000);
                // components.Components.WinApi.Com_WinApi.OutputDebugString("WRITE");

                int attemptsGeneral = 20;
                int attemptsToResend = 6;
                int attemptsOnBusy = 40;
                int attemptsToRead = 5;

                byte[] buffer = new byte[512];
                _outputData = new byte[0];
                do
                {
                    Array.Clear(buffer, 0, buffer.Length);

                    components.Components.WinApi.Com_WinApi.OutputDebugString("READ");

                    if (_port.Read(ref buffer, out _readedBytes))
                    {
                        Array.Resize<byte>(ref _outputData, (int)(_outputData.Length + _readedBytes));
                        Array.Copy(buffer, 0, _outputData, _outputData.Length - _readedBytes, _readedBytes);
                        try
                        {
                            components.Components.WinApi.Com_WinApi.OutputDebugString("DECODE");
                            switch (DecodeAnswer())
                            {
                                case "busy":
                                    {
                                        components.Components.WinApi.Com_WinApi.OutputDebugString("BUSY");
                                        if (attemptsOnBusy < 0)
                                            throw new Exception("Таймаут очікування від ЕККР: ф-ція \"" + Params.DriverData["LastFunc"] + "\"");
                                        attemptsOnBusy--;
                                        this.Hook_SendGetData_OnBusy();
                                        break;
                                    }
                                case "true":
                                    {
                                        components.Components.WinApi.Com_WinApi.OutputDebugString("TRUE");
                                        UpdateCriticalMethod(Params.DriverData["LastFunc"].ToString(), false);
                                        this.Hook_SendGetData_OnReceiveTrue();
                                        return true;
                                    }
                                case "false":
                                    {
                                        components.Components.WinApi.Com_WinApi.OutputDebugString("FALSE");
                                        this.Hook_SendGetData_OnReceiveFalse();
                                        attemptsGeneral--;
                                        break;
                                    }
                                case "resend":
                                    {
                                        components.Components.WinApi.Com_WinApi.OutputDebugString("RESEND");
                                        if (attemptsToResend < 0)
                                            throw new Exception("Перевищено к-сть запитів до ЕККР: ф-ція \"" + Params.DriverData["LastFunc"] + "\"");
                                        attemptsToResend--;
                                        this.Hook_SendGetData_OnResend();
                                        break;
                                    }
                                case "failed":
                                    {
                                        components.Components.WinApi.Com_WinApi.OutputDebugString("FAILED");
                                        this.Hook_SendGetData_OnReceiveFailed();
                                        throw new Exception("Помилка виконання команди \"" + Params.DriverData["LastFunc"] + "\"");
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            exceptionMsg = "\r\n" + ex.Message;

                            if (attemptsToResend < 0)
                                attemptsGeneral = -1;

                            break;
                        }
                    }
                    else
                    {
                        attemptsToRead--;
                        if (attemptsToRead > 0)
                            System.Threading.Thread.Sleep(250);
                        else
                            break;
                    }

                } while (attemptsGeneral > 0);
                components.Components.WinApi.Com_WinApi.OutputDebugString("EXCEPTION");
            }
            else
                components.Components.WinApi.Com_WinApi.OutputDebugString("Port Write InputData ERROR");

            if (close)
                _port.Close();

            UpdateCriticalMethod(Params.DriverData["LastFunc"].ToString(), true);
            throw new Exception("Помилка читання з фіскального принтера" + " " + Name + exceptionMsg);

        }
        private string DecodeAnswer()
        {
            CS = 0x00;

            if (_outputData.Length == 0)
                return "false";

            if (_outputData[0] == ACK)
            {
                byte i = 0;
                byte symbol = 0x00;

                byte[] normalizedAnswer = new byte[0];

                bool dleDetected = false;
                bool synDetected = false;

                //remove all SYN symbols
                for (i = 0; i < _outputData.Length; i++)
                {
                    if (_outputData[i] == DLE && !dleDetected) dleDetected = true;
                    if (_outputData[i] == SYN && !synDetected) synDetected = true;

                    if (!dleDetected && synDetected)
                    {
                        symbol++;
                        continue;
                    }

                    Array.Resize<byte>(ref normalizedAnswer, normalizedAnswer.Length + 1);
                    normalizedAnswer[normalizedAnswer.Length - 1] = _outputData[i];
                }

                if (synDetected && !dleDetected)
                    return "busy";

                _outputData = (byte[])normalizedAnswer.Clone();
                normalizedAnswer = new byte[0];

                //remove all dolbyDLE symbols
                for (i = 0; i < (_outputData.Length - 1); i++)
                {
                    Array.Resize<byte>(ref normalizedAnswer, normalizedAnswer.Length + 1);
                    normalizedAnswer[normalizedAnswer.Length - 1] = _outputData[i];
                    if (_outputData[i] == DLE && _outputData[i + 1] == DLE) i++;
                }
                Array.Resize<byte>(ref normalizedAnswer, normalizedAnswer.Length + 1);
                normalizedAnswer[normalizedAnswer.Length - 1] = _outputData[i];

                // detect CS and get its position
                if (normalizedAnswer[normalizedAnswer.Length - 1] == ETX && normalizedAnswer[normalizedAnswer.Length - 2] == DLE)
                {
                    i = (byte)(normalizedAnswer.Length - 3);
                    CS = normalizedAnswer[i];
                }

                i--;

                //normalizedAnswer = (byte[])answer.Clone();
                _outputData = (byte[])normalizedAnswer.Clone();
                normalizedAnswer = new byte[0];

                symbol = 0x00;
                while (true)
                {
                    try
                    {
                        Array.Resize<byte>(ref normalizedAnswer, normalizedAnswer.Length + 1);
                        normalizedAnswer[normalizedAnswer.Length - 1] = _outputData[i];
                        symbol = _func.SumMas(normalizedAnswer);
                        i--;
                        if ((byte)(symbol + CS) == 0 && _outputData[i] == STX && _outputData[i - 1] == DLE)
                            break;
                    }
                    catch { return "false"; }
                }

                Array.Reverse(normalizedAnswer);

                if (normalizedAnswer.Length == 0)
                    return "false";

                try
                {
                    if (normalizedAnswer[0] == COD && normalizedAnswer[1] == NOM)
                    {
                        byte state = normalizedAnswer[2];
                        byte rezult = normalizedAnswer[3];
                        byte reserv = normalizedAnswer[4];

                        string[] allSatusMsg = (string[])Params.DriverData["Status"];

                        //read state
                        // 47 - start index of status messages
                        i = 0;
                        string oper_info = string.Empty;
                        for (byte mask = 1; i < 8; mask *= 2, i++)
                            if ((state & mask) == 1)
                                oper_info += allSatusMsg[i + 47] + "\r\n";

                        //read rezult
                        oper_info += allSatusMsg[rezult];

                        if (oper_info.Length != 0)
                            throw new Exception(oper_info);


                        //read reserv state
                        /*
                        if (reserv < reservMsg.Length && reservMsg[reserv] != "")
                            System.Windows.Forms.MessageBox.Show(reservMsg[reserv], Protocol_Name, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        */
                        Array.Reverse(normalizedAnswer);
                        Array.Resize<byte>(ref normalizedAnswer, normalizedAnswer.Length - 5);
                        Array.Reverse(normalizedAnswer);

                        _outputData = normalizedAnswer;
                        return "true";
                    }
                }
                catch(Exception ex)
                {
                    components.Components.WinApi.Com_WinApi.OutputDebugString(ex.Message);
                }
            }//if ACK

            if (Params.Compatibility.ContainsKey("OP6") && OutputData[0] == SYN)
            {
                return "resend";
            }

            return "false";
        }
        private void UpdateCriticalMethod(string fname, bool state)
        {
            if (Params.ErrorFlags.ContainsKey(fname))
                Params.ErrorFlags[fname] = state;
        }

        // Data Transfer Hooks
        protected virtual void Hook_SendGetData_OnBusy()
        {
            Thread.Sleep(210);
        }
        protected virtual void Hook_SendGetData_OnResend()
        {
            Thread.Sleep(300);
            if (Params.Compatibility.ContainsKey("OP6"))
            {
                ComPort.Write(InputData);
                OutputData = new byte[0];
            }
        }
        protected virtual void Hook_SendGetData_OnReceiveFalse()
        {
            Thread.Sleep(100);
        }
        protected virtual void Hook_SendGetData_OnReceiveTrue() { }
        protected virtual void Hook_SendGetData_OnReceiveFailed() { }

        // Access Properties
        protected byte[] InputData { get { return this._inputData; } set { this._inputData = value; } }
        protected byte[] OutputData { get { return this._outputData; } set { this._outputData = value; } }
        protected byte[] DataForSend { get { return this._dataForSend; } set { this._dataForSend = value; } }
        protected uint ReadedBytes { get { return this._readedBytes; } set { this._readedBytes = value; } }
        protected Com_SerialPort ComPort { get { return this._port; } set { this._port = value; } }
        protected CoreLib Tools { get { return this._func; } set { this._func = value; } }
        protected Params Configuarion { get { return this._param; } set { this._param = value; } }

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
        public UserControl DriverUI
        {
            get
            {
                if (_driverui == null || _driverui.IsDisposed)
                    _driverui = new UI.AppUI.Tree();
                return _driverui;
            }
        }
        public UserControl PortUI
        {
            get
            {
                if (_portui == null || _portui.IsDisposed)
                    _portui = new UI.AppUI.Port(ref _port);
                return _portui;
            }
        }
        public UserControl CompatibilityUI
        {
            get
            {
                if (_compatibilityui == null || _compatibilityui.IsDisposed)
                    _compatibilityui = new UI.AppUI.Compatibility();
                return _compatibilityui;
            }
        }
        #endregion
    }
}
