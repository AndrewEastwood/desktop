using System;
using System.Collections.Generic;
using System.Text;
using driver.Config;
using System.IO;
using System.Data;
using driver.Lib;
using driver.Common;
using components.Components.MMessageBox;
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;
using System.Diagnostics;


namespace driver.Lib
{
    /// <summary>
    /// Present Object Working With Output Devices
    /// </summary>
    public static class DataWorkOutput
    {
        // many printers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="printerData"></param>
        public static void Print(Enums.PrinterType type, DataTable order)
        {
            // sort printers by name
            List<string> prnNames = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, string>> printer in driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Printers)
                prnNames.Add(printer.Key.ToString());
            string[] sortedNames = prnNames.ToArray();
            Array.Sort<string>(sortedNames);
            Dictionary<string, Dictionary<string, string>> sortedPrintersByName = new Dictionary<string, Dictionary<string, string>>();
            for (int i = 0; i < sortedNames.Length; i++)
                sortedPrintersByName.Add(sortedNames[i], driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Printers[sortedNames[i]]);

            foreach (KeyValuePair<string, Dictionary<string, string>> printer in sortedPrintersByName)
            {
                components.Components.WinApi.Com_WinApi.OutputDebugString("printer = " + printer.Value["TYPE"].ToString());
                components.Components.WinApi.Com_WinApi.OutputDebugString("active = " + (bool.Parse(printer.Value["ACTIVE"]) ? '1' : '0'));
                if (printer.Value["TYPE"].ToString() == ((int)type).ToString() && bool.Parse(printer.Value["ACTIVE"]))
                {
                    try
                    {
                        components.Components.WinApi.Com_WinApi.OutputDebugString("printing");
                        PrintThrowPrinter(printer, order);
                    }
                    catch (Exception e) { CoreLib.WriteLog(e, "driver.Lib.DataWorkOutput.Print(PrinterType type, DataTable order);"); }
                }
            }

        }

        // only one
        public static void Print(string name, DataTable order)
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> printer in driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Printers)
            {
                if (printer.Value["NAME"].ToString() == name)
                {
                    try
                    {
                        PrintThrowPrinter(printer, order);
                    }
                    catch (Exception ex) { CoreLib.WriteLog(ex, "driver.Lib.DataWorkOutput.Print(string name, DataTable order);"); }
                }
            }
        }

        // perform configuring printer
        // migrate to new method using dictionary of data instead
        private static bool PrintThrowPrinter(object printerConfig, DataTable order)
        {
            bool fRez = false;

            try
            {
                KeyValuePair<string, Dictionary<string, string>> printer = (KeyValuePair<string, Dictionary<string, string>>)printerConfig;
                string fPath = FeetchTemplate(printer.Value["TPL"], order);
                components.Components.WinApi.Com_WinApi.OutputDebugString("document = " + fPath + ";   by template = " + printer.Value["TPL"]);
                bool isBusy = false;
                int timeout = 1000;
                bool timeoutIs = false;
                int steps = ConfigManager.Instance.CommonConfiguration.Content_Common_PrinterDelaySec;
                if (fPath != string.Empty)
                {
                    // send data to system (default) printer
                    if (printer.Value["PRN"] == string.Empty)
                    {
                        try
                        {
                            StreamReader streamToPrint = new StreamReader(fPath, Encoding.Default);
                            try
                            {
                                Font printFont = new Font("Lucida Console", 8);
                                PrintDocument pd = new PrintDocument();
                                pd.PrintPage += new PrintPageEventHandler(delegate(object sender, PrintPageEventArgs ev)
                                {
                                    float linesPerPage = 0;
                                    float yPos = 0;
                                    int count = 0;
                                    float leftMargin = 0;
                                    float topMargin = 0;
                                    string line = null;

                                    // Calculate the number of lines per page.
                                    linesPerPage = ev.MarginBounds.Height /
                                       printFont.GetHeight(ev.Graphics);

                                    // Print each line of the file.
                                    while (count < linesPerPage &&
                                       ((line = streamToPrint.ReadLine()) != null))
                                    {
                                        yPos = topMargin + (count *
                                           printFont.GetHeight(ev.Graphics));
                                        ev.Graphics.DrawString(line, printFont, Brushes.Black,
                                           leftMargin, yPos, new StringFormat());
                                        count++;
                                    }

                                    // If more lines exist, print another page.
                                    if (line != null)
                                        ev.HasMorePages = true;
                                    else
                                        ev.HasMorePages = false;
                                });
                                pd.Print();
                            }
                            finally
                            {
                                streamToPrint.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            CoreLib.WriteLog(ex, "driver.Lib.DataWorkOutput.PrintThrowPrinter: system printing");
                        }
                    }
                    else
                    {
                        string appName = System.IO.Path.GetFileNameWithoutExtension(printer.Value["PRN"].ToString());

                        for (int i = 0; i < steps; i++)
                        {
                            isBusy = false;
                            System.Diagnostics.Process[] prc = System.Diagnostics.Process.GetProcesses();
                            foreach (System.Diagnostics.Process p in prc)
                            {
                                if (p.ProcessName == appName || p.ProcessName == appName + ".exe" ||
                                    p.ProcessName.ToLower() == appName.ToLower() || p.ProcessName.ToLower() == appName.ToLower() + ".exe")
                                {
                                    isBusy = true;
                                    break;
                                }
                            }
                            if (isBusy)
                            {
                                System.Threading.Thread.Sleep(timeout);
                                if (i + 1 == steps)
                                    timeoutIs = true;
                            }
                            else
                                break;

                        }
                        if (timeoutIs)
                        {
                            //mdcore.Components.UI.MMessageBox.Show("Закрийте попереднє вікно друку і потім натисніть кнопку ОК для продовження наступного роздруку.", System.Windows.Forms.Application.ProductName);

                            System.Diagnostics.Process[] pp = System.Diagnostics.Process.GetProcessesByName(appName);

                            if (pp.Length > 0 && pp[0] != null)
                            //foreach (System.Diagnostics.Process sp in pp)
                            {

                                //Dictionary<int, string> windowsV = winapi.WApi.GetTaskWindows();
                                //int iHandle = winapi.WApi.FindWindow(null, windowsV[1]);

                                components.Components.WinApi.Com_WinApi.SetForegroundWindow((int)(pp[0].MainWindowHandle));
                                System.Drawing.Rectangle msgRect = new System.Drawing.Rectangle();
                                components.Components.WinApi.Com_WinApi.GetWindowRect(pp[0].MainWindowHandle, ref msgRect);

                                components.Components.MMessageBox.MMessageBox.StartPoint = new System.Drawing.Point(msgRect.Location.X - 50, msgRect.Location.Y - 150);
                                components.Components.MMessageBox.MMessageBox.Show("Закрийте попереднє вікно друку, якщо таке є.\r\nПотім закрийте це повідомлення для нового друку.", System.Windows.Forms.Application.ProductName);

                            }


                        }
                        /*
                        System.Diagnostics.Process[] prc = System.Diagnostics.Process.GetProcesses();
                        foreach (System.Diagnostics.Process p in prc)
                        {
                            if (p.ProcessName == appName || p.ProcessName == appName + ".exe" ||
                                p.ProcessName.ToLower() == appName.ToLower() || p.ProcessName.ToLower() == appName.ToLower() + ".exe")
                            {
                                isBusy = true;
                                break;
                            }
                        }

                        if (isBusy)
                            driver.Components.UI.MMessageBox.Show("Закрийте попереднє вікно програми друку для наступного друкування документу", "Додатковий друк - InTech PayDesk");
                        */
                        System.Diagnostics.Process.Start(printer.Value["PRN"], fPath);
                        fRez = true;
                    }
                }
            }
            catch (Exception ex) { CoreLib.WriteLog(ex, "driver.Lib.DataWorkOutput.PrintThrowPrinter(object printerConfig, DataTable order);"); }

            return fRez;
        }

        public static string FeetchTemplate(string tplFile, DataTable order)
        {   //input data
            //0 - chqTable (DataTable)
            //1 - chqNumber (string)
            //2 - retriveCheque (bool)
            //3 - fix (bool)
            //4 - chequeSuma (double)
            //5 - realSuma (double)
            //6 - paymentTypes (List)
            //7 - buyersCash (double)
            //8 - buyersItemCash (List)
            //9 - buyersRest (double)
            //10 - useTotDiscount (bool)
            //11 - discountPercent (double[])
            //12 - discountCash (double[])
            //13 - discConstPercent (double)
            //14 - null
            //15 - discOnlyPercent (double)
            //16 - discOnlyCash (double)
            //17 - discCommonPercent (double)
            //18 - discCommonCash (double)
            //19 - billNumber (string)
            //20 - billComment (string)
            object[] simpleData = new object[25];
            PropertyCollection data = order.ExtendedProperties;
            
            try
            {
                // = Basic Information
                //0 - chqTable (DataTable)
                //1 - chqNumber (string)
                //2 - retriveCheque (bool)
                //3 - fix (bool)
                //4 - chequeSuma (double)
                //5 - realSuma (double)
                simpleData[0] = order;
                if (data.ContainsKey("ORDER_NO"))
                    simpleData[1] = data["ORDER_NO"];
                if (data.ContainsKey("IS_RET"))
                    simpleData[2] = data["IS_RET"];
                if (data.ContainsKey("IS_LEGAL"))
                    simpleData[3] = data["IS_LEGAL"];
                if (data.ContainsKey("ORDER_SUMA"))
                    simpleData[4] = data["ORDER_SUMA"];
                if (data.ContainsKey("ORDER_REAL_SUMA"))
                    simpleData[5] = data["ORDER_REAL_SUMA"];
            }
            catch (Exception e) { CoreLib.WriteLog(e, "driver.Lib.DataWorkOutput.FeetchTemplate(string tplFile, Dictionary<string, object> data) Attempt to initialize: Basic Information"); }

            if (data.ContainsKey("PAYMENT") && data["PAYMENT"] != null && data["PAYMENT"] != string.Empty)
            {
                // = Payment Information
                //6 - paymentTypes (List)
                //7 - buyersCash (double)
                //8 - buyersItemCash (List)
                //9 - buyersRest (double)
                try
                {
                    Dictionary<string, object> paymentInfo = (Dictionary<string, object>)data["PAYMENT"];
                    if (paymentInfo.ContainsKey("TYPE"))
                        simpleData[6] = paymentInfo["TYPE"];
                    if (paymentInfo.ContainsKey("SUMA"))
                        simpleData[7] = paymentInfo["SUMA"];
                    if (paymentInfo.ContainsKey("CASHLIST"))
                        simpleData[8] = paymentInfo["CASHLIST"];
                    if (paymentInfo.ContainsKey("REST"))
                        simpleData[9] = paymentInfo["REST"];
                }
                catch (Exception e) { CoreLib.WriteLog(e, "driver.Lib.DataWorkOutput.FeetchTemplate(string tplFile, Dictionary<string, object> data) Attempt to initialize: Payment Information"); }
            }

            if (data.ContainsKey("DISCOUNT") && data["DISCOUNT"] != null && data["DISCOUNT"] != string.Empty)
            {
                // = Discount Information
                //10 - useTotDiscount (bool)
                //11 - discountPercent (double[])
                //12 - discountCash (double[])
                //13 - discConstPercent (double)
                //14 - null
                //15 - discOnlyPercent (double)
                //16 - discOnlyCash (double)
                //17 - discCommonPercent (double)
                //18 - discCommonCash (double)
                try
                {
                    Hashtable discountInfo = (Hashtable)data["DISCOUNT"];
                    if (discountInfo.ContainsKey("DISC_ALL_ITEMS"))
                        simpleData[10] = discountInfo["DISC_ALL_ITEMS"];
                    if (discountInfo.ContainsKey("DISC_ARRAY_PERCENT"))
                        simpleData[11] = discountInfo["DISC_ARRAY_PERCENT"];
                    if (discountInfo.ContainsKey("DISC_ARRAY_CASH"))
                        simpleData[12] = discountInfo["DISC_ARRAY_CASH"];
                    if (discountInfo.ContainsKey("DISC_CONST_PERCENT"))
                        simpleData[13] = discountInfo["DISC_CONST_PERCENT"];
                    if (discountInfo.ContainsKey("DISC_ONLY_PERCENT"))
                        simpleData[15] = discountInfo["DISC_ONLY_PERCENT"];
                    if (discountInfo.ContainsKey("DISC_ONLY_CASH"))
                        simpleData[16] = discountInfo["DISC_ONLY_CASH"];
                    if (discountInfo.ContainsKey("DISC_FINAL_PERCENT"))
                        simpleData[17] = discountInfo["DISC_FINAL_PERCENT"];
                    if (discountInfo.ContainsKey("DISC_FINAL_CASH"))
                        simpleData[18] = discountInfo["DISC_FINAL_CASH"];
                    if (discountInfo.ContainsKey("DISC_APPLIED"))
                        simpleData[22] = discountInfo["DISC_APPLIED"];
                }
                catch (Exception e) { CoreLib.WriteLog(e, "driver.Lib.DataWorkOutput.FeetchTemplate(string tplFile, Dictionary<string, object> data) Attempt to initialize: Discount Information"); }
            }

            if (data.ContainsKey("BILL") && data["BILL"] != null)
            {
                try
                {
                    Dictionary<string, object> billInfo = (Dictionary<string, object>)data["BILL"];
                    if (billInfo.ContainsKey("BILL_NO"))
                        simpleData[19] = billInfo["BILL_NO"];
                    if (billInfo.ContainsKey("COMMENT"))
                        simpleData[20] = billInfo["COMMENT"];
                    if (billInfo.ContainsKey("DATETIME"))
                        simpleData[21] = billInfo["DATETIME"];
                    if (billInfo.ContainsKey(driver.Common.CoreConst.DATETIME_LOCK))
                        simpleData[23] = billInfo[driver.Common.CoreConst.DATETIME_LOCK];
                }
                catch (Exception e) { CoreLib.WriteLog(e, "driver.Lib.DataWorkOutput.FeetchTemplate(string tplFile, Dictionary<string, object> data) Attempt to initialize: Bill Information"); }
            
            }

            try
            {
                components.Components.WinApi.Com_WinApi.OutputDebugString("Fetching data");
                return FeetchTemplate(tplFile, simpleData);
            }
            catch (Exception e) { CoreLib.WriteLog(e, "driver.Lib.DataWorkOutput.FeetchTemplate(string tplFile, Dictionary<string, object> data) Attempt to perform Base Function: driver.Lib.DataWorkOutput.FeetchTemplate(string tplFile, object[] data)"); }

            return string.Empty;
        }

        public static string FeetchTemplate(string tplFile, object[] data)
        {
            if (!File.Exists(tplFile))
                return "";

            if (!Directory.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp))
                Directory.CreateDirectory(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp);

            string dCor = string.Empty;
            for (byte i = 0; i < driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals; i++)
                dCor += '0';

            //Set directives
            byte LoAN = 15;
            byte LoAD = 20;
            byte TSpL = 0;
            string PrMk = string.Empty;
            string PrBcMk = string.Empty;
            //Load messages
            Dictionary<string, string> _MSG = new Dictionary<string, string>();
            using (System.Xml.XmlTextReader xmlRd = new System.Xml.XmlTextReader(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Templates + "\\" + "Messages.xml"))
            {
                string varName = string.Empty;
                xmlRd.WhitespaceHandling = System.Xml.WhitespaceHandling.None;
                while (xmlRd.Read())
                    if (xmlRd.NodeType == System.Xml.XmlNodeType.Element && xmlRd.Name != null
                        && xmlRd.Name != string.Empty && xmlRd.Name.ToUpper() != "MESSAGES")
                    {
                        varName = xmlRd.Name;
                        xmlRd.Read();
                        _MSG.Add(varName.ToUpper(), xmlRd.Value.TrimStart('\r', '\n').TrimEnd('\r', '\n'));
                    }
                xmlRd.Close();
            }

            // Aligment blocks
            string[][] _alignBlocks = new string[2][];
            string[] _separators = new string[] { "[hex:", "[dec:" };
            _alignBlocks[0] = new string[] { "[right]", "[center]" };
            _alignBlocks[1] = new string[] { "[\\right]", "[\\center]" };
            // Separators

            //Making info values
            object[] infoV = null;
            #region Info values
            //input data
            //0 - chqTable (DataTable)
            //1 - chqNumber (string)
            //2 - retriveCheque (bool)
            //3 - fix (bool)
            //4 - chequeSuma (double)
            //5 - realSuma (double)
            //6 - paymentTypes (List)
            //7 - buyersCash (double)
            //8 - buyersItemCash (List)
            //9 - buyersRest (double)
            //10 - useTotDiscount (bool)
            //11 - discountPercent (double[])
            //12 - discountCash (double[])
            //13 - discConstPercent (double)
            //14 - null
            //15 - discOnlyPercent (double)
            //16 - discOnlyCash (double)
            //17 - discCommonPercent (double)
            //18 - discCommonCash (double)
            //19 - billNumber (string)
            //20 - billComment (string)
            //21 - billDateTimeOpen (DateTime)
            //22 - discount is applied
            //23 - billDateTimeLock (DateTime)

            string SiMask = "{0:0." + dCor + ";0." + dCor + ";0." + dCor + "}";
            string DiMask = "{0:0." + dCor + ";0." + dCor + ";!ZeRo!}";
            List<byte> paymentTypes = (List<byte>)data[6];
            List<double> buyersMoney = (List<double>)data[8];

            //Info values (is as type).
            object[] sValues = new object[40];
            //00 - subUnit (byte)
            sValues[00] = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit;//0
            //01 - subUnitName (string)
            sValues[01] = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnitName;//1
            //02 - payDesk (byte)
            sValues[02] = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_PayDesk;//2
            //03 - cashierName (string)
            sValues[03] = UserConfig.UserID;//3
            //04 - currentDateTime (DateTime)
            sValues[04] = DateTime.Now;//4
            //05 - chequeNumber (string)
            sValues[05] = data[1] != null ? data[1] : string.Empty;//5
            //06 - chequeType (bool) fix || !fix
            sValues[06] = data[3] != null && (bool)data[3] ? 1 : 0;//6
            //07 - retirveCheque (bool)
            sValues[07] = data[2] != null && (bool)data[2] ? 1 : 0;//7
            //08 - chequeSuma (double)
            sValues[08] = data[4] != null ? data[4] : 0.0;//8
            //09 - realSuma (double)
            sValues[09] = data[5] != null ? data[5] : 0.0;//9
            //10 - --null-- payment Type [cash] (string)
            sValues[10] = null;//_MSG["PAYMENT_CASH"],//10
            //11 - --null-- payment Type [card] (string)
            sValues[11] = null;//_MSG["PAYMENT_CARD"],//11
            //12 - --null-- payment Type [credit] (string)
            sValues[12] = null;//_MSG["PAYMENT_CREDIT"],//12
            //13 - --null-- payment Type [cheque] (string)
            sValues[13] = null;//_MSG["PAYMENT_CHEQUE"],//13
            //14 - buyers money [cash] (double)
            sValues[14] = (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)3) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)3)] : 0;//14
            //15 - buyers money [card] (double)
            sValues[15] = (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)0) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)0)] : 0;//15
            //16 - buyers money [credit] (double)
            sValues[16] = (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)1) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)1)] : 0;//16
            //17 - buyers money [cheque] (double)
            sValues[17] = (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)2) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)2)] : 0;//17
            //18 - billNumber (string)
            sValues[18] = data[19] != null ? data[19] : string.Empty;//18
            //19 - billComment (string)
            sValues[19] = data[20] != null ? data[20] : string.Empty;//19
            //20 - buyersCash (double)
            sValues[20] = data[7] != null ? data[7] : 0.0;//20
            //21 - buyersRest (double)
            sValues[21] = (paymentTypes != null && buyersMoney != null && buyersMoney.Count == 1 && paymentTypes[0] == 3) ? data[9] : 0;//21
            //22 - useTotDiscount (bool)
            sValues[22] = data[10] != null && (bool)data[10] ? 1 : 0;//22
            //23 - discountPercent (double)
            sValues[23] = data[11] != null && ((double[])data[11])[0] != null ? ((double[])data[11])[0] : 0.0;//23
            //24 - [-discountPercent] (double)
            sValues[24] = data[11] != null && ((double[])data[11])[1] != null ? ((double[])data[11])[1] : 0.0; ;//24
            //25 - discountCash (double)
            sValues[25] = data[12] != null && ((double[])data[12])[0] != null ? ((double[])data[12])[0] : 0.0; ;//25
            //26 - [-discountCash] (double)
            sValues[26] = data[12] != null && ((double[])data[12])[1] != null ? ((double[])data[12])[1] : 0.0; ;//26
            //27 - discountConstPercent (double)
            sValues[27] = data[13] != null ? data[13] : 0.0;//27
            //28 - bill date time open
            sValues[28] = data[21];//28
            //29 - E_discountPercent[] (double)
            sValues[29] = data[15] != null ? data[15] : 0.0;//29
            //30 - E_discountCash[] (double)
            sValues[30] = data[16] != null ? data[16] : 0.0;//30
            //31 - discountCommonPercent (double)
            sValues[31] = data[17] != null ? data[17] : 0.0;//31
            //32 - discountCommonCash (double)
            sValues[32] = data[18] != null ? data[18] : 0.0;//32
            //33 - discount applied
            sValues[33] = ((bool)data[22] ? 1 : -1);//33
            //34 - -
            sValues[34] = null;//34
            //35 - true or false to detect bill type
            sValues[35] = (data[19] != null || data[20] != null) ? 1 : 0;//35
            //36 - bill date time lock
            sValues[36] = data[23];//36
            //37 - -
            sValues[37] = null;//37
            //38 - -
            sValues[38] = null;//38
            //39 - -
            sValues[39] = null;//39

            infoV = new object[sValues.Length];
            Array.Copy(sValues, infoV, sValues.Length);

            //Info values (type parsed to string (used SiMaks)).
            object[] sStrValues = new object[40];
            //40 - -
            sStrValues[00] = null;//40
            //41 - -
            sStrValues[01] = null;//41
            //42 - -
            sStrValues[02] = null;//42
            //43 - -
            sStrValues[03] = null;//43
            //44 - -
            sStrValues[04] = null;//44
            //45 - -
            sStrValues[05] = null;//45
            //46 - -
            sStrValues[06] = null;//46
            //47 - -
            sStrValues[07] = null;//47
            //48 - chequeSuma (double)
            sStrValues[08] = string.Format(SiMask, sValues[8]);//48 - chequeSuma
            //49 - realSuma (double)
            sStrValues[09] = string.Format(SiMask, sValues[9]);//49 - realSuma
            //50 - -
            sStrValues[10] = null;//_MSG["PAYMENT_CASH"];//50
            //51 - -
            sStrValues[11] = null;//_MSG["PAYMENT_CARD"];//51
            //52 - -
            sStrValues[12] = null;//_MSG["PAYMENT_CREDIT"];//52
            //53 - -
            sStrValues[13] = null;//_MSG["PAYMENT_CHEQUE"];//53
            //54 - buyers money [cash] (double)
            sStrValues[14] = string.Format(SiMask, (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)3) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)3)] : 0);//54
            //55 - buyers money [card] (double)
            sStrValues[15] = string.Format(SiMask, (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)0) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)0)] : 0);//55
            //56 - buyers money [credit] (double)
            sStrValues[16] = string.Format(SiMask, (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)1) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)1)] : 0);//56
            //57 - buyers money [cheque] (double)
            sStrValues[17] = string.Format(SiMask, (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)2) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)2)] : 0);//57
            //58 - -
            sStrValues[18] = null;//58
            //59 - -
            sStrValues[19] = null;//59
            //60 - buyersCash (double)
            sStrValues[20] = string.Format(SiMask, sValues[20]);//60 - 
            //61 - buyersRest (double)
            sStrValues[21] = string.Format(SiMask, (paymentTypes != null && buyersMoney != null && buyersMoney.Count == 1 && paymentTypes[0] == 3) ? data[9] : 0);//61 - 
            //62 - -
            sStrValues[22] = null;//62
            //63 - discountPercent (double)
            sStrValues[23] = string.Format(SiMask, sValues[23]);//63 - 
            //64 - [-discountPercent] (double)
            sStrValues[24] = string.Format(SiMask, sValues[24]);//64 - 
            //65 - discountCash (double)
            sStrValues[25] = string.Format(SiMask, sValues[25]);//65 - 
            //66 - [-discountCash] (double)
            sStrValues[26] = string.Format(SiMask, sValues[26]);//66 - 
            //67 - discountConstPercent (double)
            sStrValues[27] = string.Format(SiMask, data[13]);//67 - 
            //68 - -
            sStrValues[28] = null;//68
            //69 - E_discountPercent[] (double)
            sStrValues[29] = string.Format(SiMask, data[15]);//69 - 
            //70 - E_discountCash[] (double) 
            sStrValues[30] = string.Format(SiMask, data[16]);//70 - 
            //71 - discountCommonPercent (double)
            sStrValues[31] = string.Format(SiMask, data[17]);//71 - 
            //72 - discountCommonCash (double)
            sStrValues[32] = string.Format(SiMask, data[18]);//72 - 
            //73 - -
            sStrValues[33] = null;//73 - 
            //74 - -
            sStrValues[34] = null;//74 - 
            //75 - -
            sStrValues[35] = null;//75 - 
            //76 - -
            sStrValues[36] = null;//76 - 
            //77 - -
            sStrValues[37] = null;//77 - 
            //78 - -
            sStrValues[38] = null;//78 - 
            //79 - -
            sStrValues[39] = null;//79 - 

            Array.Resize<object>(ref infoV, infoV.Length + sStrValues.Length);
            Array.Copy(sStrValues, 0, infoV, infoV.Length - sStrValues.Length, sStrValues.Length);

            //Info values (type parsed to string (used DiMaks)).
            object[] sDynStrValues = new object[40];
            //80 - -
            sDynStrValues[00] = null;//80
            //81 - -
            sDynStrValues[01] = null;//81
            //82 - -
            sDynStrValues[02] = null;//82
            //83 - -
            sDynStrValues[03] = null;//83
            //84 - -
            sDynStrValues[04] = null;//84
            //85 - -
            sDynStrValues[05] = null;//85
            //86 - -
            sDynStrValues[06] = null;//86
            //87 - -
            sDynStrValues[07] = null;//87
            //88 - chequeSuma (double)
            sDynStrValues[08] = null;//88 - chequeSuma (double)
            //89 - realSuma (double)
            sDynStrValues[09] = null;//89 - realSuma (double)
            //90 - -
            sDynStrValues[10] = null;//_MSG["PAYMENT_CASH"];//90
            //91 - -
            sDynStrValues[11] = null;//_MSG["PAYMENT_CARD"];//91
            //92 - -
            sDynStrValues[12] = null;//_MSG["PAYMENT_CREDIT"];//92
            //93 - -
            sDynStrValues[13] = null;//_MSG["PAYMENT_CHEQUE"];//93
            //94 - buyers money [cash] (double)
            sDynStrValues[14] = string.Format(DiMask, (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)3) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)3)] : 0);//94
            //95 - buyers money [card] (double)
            sDynStrValues[15] = string.Format(DiMask, (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)0) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)0)] : 0);//95
            //96 - buyers money [credit] (double)
            sDynStrValues[16] = string.Format(DiMask, (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)1) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)1)] : 0);//96
            //97 - buyers money [cheque] (double)
            sDynStrValues[17] = string.Format(DiMask, (paymentTypes != null && buyersMoney != null && paymentTypes.IndexOf((byte)2) >= 0) ? buyersMoney[paymentTypes.IndexOf((byte)2)] : 0);//97
            //98 - billNumber (string)
            sDynStrValues[18] = data[19] == null || data[19].ToString() == string.Empty ? "!ZeRo!" : data[19];//98
            //99 - billComment (string)
            sDynStrValues[19] = data[20] == null || data[20].ToString() == string.Empty ? "!ZeRo!" : data[20].ToString().Replace("%20", " ");//99
            //100 - buyersCash (double)
            sDynStrValues[20] = null;//100 - 
            //101 - buyersRest (double)
            sDynStrValues[21] = string.Format(DiMask, (paymentTypes != null && buyersMoney != null && buyersMoney.Count == 1 && paymentTypes[0] == 3) ? data[9] : 0);//101 - 
            //102 - -
            sDynStrValues[22] = null;//102
            //103 - discountPercent (double)
            sDynStrValues[23] = string.Format(DiMask, sValues[23]);//103 - 
            //104 - [-discountPercent] (double)
            sDynStrValues[24] = string.Format(DiMask, sValues[24]);//104 - 
            //105 - discountCash (double)
            sDynStrValues[25] = string.Format(DiMask, sValues[25]);//105 - 
            //106 - [-discountCash] (double)
            sDynStrValues[26] = string.Format(DiMask, sValues[26]);//106 - 
            //107 - discountConstPercent (double)
            sDynStrValues[27] = string.Format(DiMask, data[13]);//107 - 
            //108 - -
            sDynStrValues[28] = null;//108
            //109 - E_discountPercent[] (double)
            sDynStrValues[29] = string.Format(DiMask, data[15]);//109 - 
            //110 - E_discountCash[] (double) 
            sDynStrValues[30] = string.Format(DiMask, data[16]);//110 - 
            //111 - discountCommonPercent (double)
            sDynStrValues[31] = string.Format(DiMask, data[17]);//111 - 
            //112 - discountCommonCash (double)
            sDynStrValues[32] = string.Format(DiMask, data[18]);//112 - 
            //113 - -
            sDynStrValues[33] = null;//113 - 
            //114 - -
            sDynStrValues[34] = null;//114 - 
            //115 - -
            sDynStrValues[35] = null;//115 - 
            //116 - -
            sDynStrValues[36] = null;//116 - 
            //117 - -
            sDynStrValues[37] = null;//117 - 
            //118 - -
            sDynStrValues[38] = null;//118 - 
            //119 - -
            sDynStrValues[39] = null;//119 - 

            Array.Resize<object>(ref infoV, infoV.Length + sDynStrValues.Length);
            Array.Copy(sDynStrValues, 0, infoV, infoV.Length - sDynStrValues.Length, sDynStrValues.Length);

            #endregion

            // additional data
            bool productPrintState = false;

            //Creating output file
            string chqTxtName = string.Format("{0:X2}{1:X2}_{2:yyMMdd}_{2:HHmmss.fff}.txt", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, driver.Config.ConfigManager.Instance.CommonConfiguration.APP_PayDesk, DateTime.Now);
            StreamWriter streamWr = new StreamWriter(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp + "\\" + chqTxtName, false, Encoding.Default);
            chqTxtName = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp + "\\" + chqTxtName;

            //Load template
            StreamReader streamRd = File.OpenText(tplFile);

            //Fill data by loaded template
            string template = string.Empty;
            while ((template = streamRd.ReadLine()) != null)
            {
                if (template == string.Empty)
                    template = " ";

                template = template.Replace("\\r", "\r");
                template = template.Replace("\\n", "\n");
                template = template.Replace("\\t", "\t");
                template = template.Replace("\\v", "\v");
                template = template.Replace("\\b", "\b");

                switch (template[0])
                {
                    #region Info values [S]
                    case 'S':
                        {
                            template = template.Remove(0, 1);
                            template = string.Format(template, infoV);

                            if (!template.Contains("!ZeRo!"))
                            {
                                template = GetHEX(template, _separators);
                                streamWr.WriteLine(template);
                            }
                            break;
                        }
                    #endregion
                    #region Articles [W]
                    case 'W':
                        {
                            //Output Format [W]
                            //00 - articleName (string)
                            //01 - articleDecription (string)
                            //02 - articleUnit (string)
                            //03 - articleTaxChar (char)
                            //04 - articlePrice (double)
                            //05 - articleDose (double)
                            //06 - articleTaxValue (double)
                            //07 - articleDiscount (double)
                            //08 - articleSuma (doulble)
                            //09 - articleActualSum (double)
                            //10 - articleTaxMoney (double)
                            //11 - articleCashDiscount (double)
                            //12 - articleDoseDiff (double)

                            //static values parsed as string [Program decimal correction]
                            //20 - articlePrice (double)
                            //21 - articleDose (double)
                            //22 - articleTaxValue (double)
                            //23 - articleDiscount (double)
                            //24 - articleSuma (double)
                            //25 - articleActualSum (double)
                            //26 - articleTaxMoney (double)
                            //27 - articleCashDiscount (double)
                            //28 - articleDoseDiff (double)

                            string articleTemplate = template.Remove(0, 1);
                            string art_name = "";
                            string art_desc = "";
                            double quantityDiff = 0.0;
                            bool printThisRecord = true;
                            DataRow dRow = ((DataTable)data[0]).NewRow();
                            for (int i = 0; i < ((DataTable)data[0]).Rows.Count; i++)
                            {
                                dRow.ItemArray = ((DataTable)data[0]).Rows[i].ItemArray;

                                // checking PrMk and PrBcMk
                                // Product Id Mask and Product Barcode Mask
                                // if some one is empty it will be ignored.
                                if (PrMk.Length != 0 || PrBcMk.Length != 0)
                                {
                                    printThisRecord = false;

                                    if (PrMk.Length != 0 &&
                                        PrBcMk.Length != 0 &&
                                        dRow["ID"].ToString().StartsWith(PrMk) &&
                                        dRow["BC"].ToString().StartsWith(PrBcMk))
                                        printThisRecord = true;

                                    if (PrMk.Length != 0 && dRow["ID"].ToString().StartsWith(PrMk))
                                        printThisRecord = true;

                                    if (PrBcMk.Length != 0 && dRow["BC"].ToString().StartsWith(PrBcMk))
                                        printThisRecord = true;
                                }

                                if (!printThisRecord)
                                    continue;

                                art_name = ((DataTable)data[0]).Rows[i]["NAME"].ToString();
                                art_desc = ((DataTable)data[0]).Rows[i]["DESC"].ToString();

                                if (art_name.Length > LoAN)
                                    art_name = art_name.Substring(0, LoAN);

                                if (art_desc.Length > LoAD)
                                    art_desc = art_desc.Substring(0, LoAD);

                                if (art_name.Length == 0)
                                    art_name = art_desc;

                                if (art_desc.Length == 0)
                                    art_desc = art_name;

                                quantityDiff = MathLib.GetRoundedDose(MathLib.GetDouble(((System.Data.DataTable)data[0]).Rows[i]["TOT"]) - (double)((System.Data.DataTable)data[0]).Rows[i]["PRINTCOUNT"]);

                                try
                                {
                                    template = string.Format(articleTemplate,
                                        //0
                                        art_name,
                                        art_desc,
                                        ((DataTable)data[0]).Rows[i]["UNIT"],
                                        ((DataTable)data[0]).Rows[i]["VG"],
                                        ((DataTable)data[0]).Rows[i]["PRICE"],
                                        ((DataTable)data[0]).Rows[i]["TOT"],
                                        ((DataTable)data[0]).Rows[i]["TAX_VAL"],
                                        data[10] != null && (bool)data[10] ? 0 : (double)((DataTable)data[0]).Rows[i]["DISC"] / 100,
                                        ((DataTable)data[0]).Rows[i]["SUM"],
                                        ((DataTable)data[0]).Rows[i]["ASUM"],
                                        //10
                                        ((DataTable)data[0]).Rows[i]["TAX_MONEY"],
                                        MathLib.GetRoundedMoney((double)((System.Data.DataTable)data[0]).Rows[i]["SUM"] - (double)((System.Data.DataTable)data[0]).Rows[i]["ASUM"]),//11
                                        quantityDiff,//12
                                        (quantityDiff > 0) ? 1 : (quantityDiff == 0 ? 0 : -1),//13
                                        null,//14
                                        null,//15
                                        null,//16
                                        null,//17
                                        null,//18
                                        null,//19
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["PRICE"]),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals + "}", ((DataTable)data[0]).Rows[i]["TOT"]),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["TAX_VAL"]),
                                        data[10] != null && (bool)data[10] ? "" : string.Format("{0:-0." + dCor + "%;+0." + dCor + "%;0." + dCor + "%}", (double)((DataTable)data[0]).Rows[i]["DISC"] / 100),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["SUM"]),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["ASUM"]),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["TAX_MONEY"]),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", MathLib.GetRoundedMoney((double)((System.Data.DataTable)data[0]).Rows[i]["SUM"] - (double)((System.Data.DataTable)data[0]).Rows[i]["ASUM"])),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals + "}", MathLib.GetRoundedDose(MathLib.GetDouble(((System.Data.DataTable)data[0]).Rows[i]["TOT"]) - (double)((System.Data.DataTable)data[0]).Rows[i]["PRINTCOUNT"]))
                                    );
                                }
                                catch (Exception ex) { CoreLib.WriteLog(ex, "FeetchTemplate(); at Generating row template for article."); }

                                if (!template.Contains("!ZeRo!"))
                                {
                                    if (i == 0)
                                        template = template.TrimStart(new char[] { '\n', '\r' });
                                    if (i + 1 == ((DataTable)data[0]).Rows.Count)
                                        template = template.TrimEnd(new char[] { '\n', '\r' });

                                    template = GetHEX(template, _separators);
                                    streamWr.WriteLine(template);

                                    // indicates that this product is printed
                                    productPrintState = true;
                                }
                            }

                            break;
                        }
                    #endregion
                    #region Deleted Rows [D]
                    case 'D':
                        {
                            string articleTemplate = template.Remove(0, 1);
                            try
                            {
                                Dictionary<string, object[]> deletedRows = (Dictionary<string, object[]>)DataWorkShared.ExtractBillProperty(((DataTable)data[0]), driver.Common.CoreConst.DELETED_ROWS);
                                int i = 0;
                                string art_name = string.Empty;
                                string art_desc = string.Empty;
                                DataRow dRow = ((DataTable)data[0]).NewRow();
                                bool printThisRecord = true;
                                foreach (KeyValuePair<string, object[]> deletedRecord in deletedRows)
                                {
                                    printThisRecord = true;

                                    try
                                    {
                                        dRow.ItemArray = deletedRecord.Value;

                                        // checking PrMk and PrBcMk
                                        // Product Id Mask and Product Barcode Mask
                                        // if some one is empty it will be ignored.
                                        if (PrMk.Length != 0 || PrBcMk.Length != 0)
                                        {
                                            printThisRecord = false;

                                            if (PrMk.Length != 0 &&
                                                PrBcMk.Length != 0 &&
                                                dRow["ID"].ToString().StartsWith(PrMk) &&
                                                dRow["BC"].ToString().StartsWith(PrBcMk))
                                                printThisRecord = true;

                                            if (PrMk.Length != 0 && dRow["ID"].ToString().StartsWith(PrMk))
                                                printThisRecord = true;

                                            if (PrBcMk.Length != 0 && dRow["BC"].ToString().StartsWith(PrBcMk))
                                                printThisRecord = true;
                                        }

                                        if (!printThisRecord)
                                            continue;

                                        art_name = dRow["NAME"].ToString();
                                        art_desc = dRow["DESC"].ToString();

                                        if (art_name.Length > LoAN)
                                            dRow["NAME"] = art_name.Substring(0, LoAN);

                                        if (art_desc.Length > LoAD)
                                            dRow["DESC"] = art_desc.Substring(0, LoAD);


                                        template = string.Format(articleTemplate, dRow.ItemArray);


                                        if (!template.Contains("!ZeRo!"))
                                        {
                                            if (i == 0)
                                                template = template.TrimStart(new char[] { '\n', '\r' });
                                            if (i + 1 == ((DataTable)data[0]).Rows.Count)
                                                template = template.TrimEnd(new char[] { '\n', '\r' });

                                            template = GetHEX(template, _separators);
                                            streamWr.WriteLine(template);

                                            // indicates that this product is printed
                                            productPrintState = true;
                                        }
                                        i++;
                                    }
                                    catch (Exception ex) { }
                                }// foreach
                            }
                            catch (Exception ex) { }
                            break;
                        }
                    #endregion
                    #region Directives [#]
                    case '#':
                        {
                            //Template variables
                            //LoAN - Limit of Article Name (byte)
                            //LoAD - Limit of Article Description (byte)
                            //TSpL - Total Symbols per Line (byte)

                            template = template.Remove(0, 1);
                            object[] var = template.Split('=');

                            try
                            {
                                switch (var[0].ToString())
                                {
                                    case "LoAN": LoAN = byte.Parse(var[1].ToString()); break;
                                    case "LoAD": LoAD = byte.Parse(var[1].ToString()); break;
                                    case "TSpL": TSpL = byte.Parse(var[1].ToString()); break;
                                    case "PrMk": PrMk = var[1].ToString(); break;
                                    case "PrBcMk": PrBcMk = var[1].ToString(); break;
                                }
                            }
                            catch { }

                            break;
                        }
                    #endregion
                    #region Messages [M]
                    case 'M':
                        {
                            template = template.Remove(0, 1).ToUpper();
                            string[] keys = new string[_MSG.Keys.Count];
                            string[] blocks = new string[0];
                            string newLine = string.Empty;
                            string cuttedLine = string.Empty;
                            int startIndex = 0;
                            int length = 0;
                            int i = 0;
                            int j = 0;

                            _MSG.Keys.CopyTo(keys, 0);

                            try
                            {
                                for (i = 0; i < _MSG.Keys.Count; i++)
                                    template = template.Replace(keys[i], _MSG[keys[i]]);
                            }
                            catch { }

                            //Aligment
                            for (i = 0; i < _alignBlocks[0].Length; i++)
                            {
                                while (template.Contains(_alignBlocks[0][i]) && template.Contains(_alignBlocks[1][i]))
                                {
                                    startIndex = template.IndexOf(_alignBlocks[0][i]) + _alignBlocks[0][i].Length;
                                    length = template.IndexOf(_alignBlocks[1][i]) - startIndex;

                                    newLine = cuttedLine = template.Substring(startIndex, length);
                                    blocks = newLine.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                                    newLine = string.Empty;
                                    for (j = 0; j < blocks.Length; j++)
                                    {
                                        blocks[j] = blocks[j].Trim();
                                        switch (_alignBlocks[0][i])
                                        {
                                            case "[center]":
                                                if (blocks[j].Length % 2 == 0)
                                                    length = (TSpL - blocks[j].Length) / 2 + blocks[j].Length;
                                                else
                                                    length = (TSpL - (blocks[j].Length - 1)) / 2 + blocks[j].Length;
                                                break;
                                            case "[right]":
                                                length = TSpL;
                                                break;
                                        }
                                        blocks[j] = blocks[j].PadLeft(length);
                                        newLine += blocks[j];
                                        if (j + 1 < blocks.Length)
                                            newLine += "\r\n";
                                    }
                                    cuttedLine = _alignBlocks[0][i] + cuttedLine + _alignBlocks[1][i];
                                    template = template.Replace(cuttedLine, newLine);
                                }

                                //remove first single aligment's block
                                if (template.Contains(_alignBlocks[0][i]) && !template.Contains(_alignBlocks[1][i]))
                                    template = template.Replace(_alignBlocks[0][i], "");

                                //remove end single aligment's block
                                if (!template.Contains(_alignBlocks[0][i]) && template.Contains(_alignBlocks[1][i]))
                                    template = template.Replace(_alignBlocks[1][i], "");
                            }

                            template = template.Replace("\\r", "\r");
                            template = template.Replace("\\n", "\n");
                            template = GetHEX(template, _separators);
                            streamWr.WriteLine(template);

                            break;
                        }
                    #endregion
                    #region Comment [;]
                    case ';':
                        {
                            break;
                        }
                    #endregion
                    #region [Q]
                    case 'Q':
                        {
                            // categorized products by product ID

                            //Output Format is the same as format for [W]

                            string articleTemplate = template.Remove(0, 1);
                            string art_name = string.Empty;
                            string art_desc = string.Empty;
                            double quantityDiff = 0.0;
                            bool printThisRecord = true;
                            DataRow dRow = ((DataTable)data[0]).NewRow();

                            // getting parameters

                            string categoryParam = string.Empty;
                            int categoryID = 0;
                            try
                            {
                                categoryParam = articleTemplate.Substring(articleTemplate.IndexOf('['), articleTemplate.IndexOf(']') + 1);
                                articleTemplate = articleTemplate.Remove(articleTemplate.IndexOf('['), articleTemplate.IndexOf(']') + 1);
                            }
                            catch { }

                            // getting category id
                            categoryID = new components.Lib.CoreLib().GetOnlyNumericValue(categoryParam);


                            if (categoryParam.Contains("?"))
                            {
                                int categoryProdcutsCount = ((DataTable)data[0]).Select(string.Format("ID like '{0}%'", categoryID)).Length;

                                if (categoryProdcutsCount > 0)
                                {
                                    template = GetHEX(articleTemplate, _separators);
                                    streamWr.WriteLine(template);
                                }
                            }
                            else
                                for (int i = 0; i < ((DataTable)data[0]).Rows.Count; i++)
                                {
                                    dRow.ItemArray = ((DataTable)data[0]).Rows[i].ItemArray;

                                    printThisRecord = false;

                                    if (dRow["ID"].ToString().StartsWith(categoryID.ToString()))
                                        printThisRecord = true;

                                    if (!printThisRecord)
                                        continue;

                                    art_name = ((DataTable)data[0]).Rows[i]["NAME"].ToString();
                                    art_desc = ((DataTable)data[0]).Rows[i]["DESC"].ToString();

                                    if (art_name.Length > LoAN)
                                        art_name = art_name.Substring(0, LoAN);

                                    if (art_desc.Length > LoAD)
                                        art_desc = art_desc.Substring(0, LoAD);

                                    quantityDiff = MathLib.GetRoundedDose(MathLib.GetDouble(((System.Data.DataTable)data[0]).Rows[i]["TOT"]) - (double)((System.Data.DataTable)data[0]).Rows[i]["PRINTCOUNT"]);

                                    try
                                    {
                                        template = string.Format(articleTemplate,
                                            //0
                                            art_name,
                                            art_desc,
                                            ((DataTable)data[0]).Rows[i]["UNIT"],
                                            ((DataTable)data[0]).Rows[i]["VG"],
                                            ((DataTable)data[0]).Rows[i]["PRICE"],
                                            ((DataTable)data[0]).Rows[i]["TOT"],
                                            ((DataTable)data[0]).Rows[i]["TAX_VAL"],
                                            data[10] != null && (bool)data[10] ? 0 : (double)((DataTable)data[0]).Rows[i]["DISC"] / 100,
                                            ((DataTable)data[0]).Rows[i]["SUM"],
                                            ((DataTable)data[0]).Rows[i]["ASUM"],
                                            //10
                                            ((DataTable)data[0]).Rows[i]["TAX_MONEY"],
                                            MathLib.GetRoundedMoney((double)((System.Data.DataTable)data[0]).Rows[i]["SUM"] - (double)((System.Data.DataTable)data[0]).Rows[i]["ASUM"]),//11
                                            quantityDiff,//12
                                            (quantityDiff > 0) ? 1 : (quantityDiff == 0 ? 0 : -1),//13
                                            null,//14
                                            null,//15
                                            null,//16
                                            null,//17
                                            null,//18
                                            null,//19
                                            string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["PRICE"]),
                                            string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals + "}", ((DataTable)data[0]).Rows[i]["TOT"]),
                                            string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["TAX_VAL"]),
                                            data[10] != null && (bool)data[10] ? "" : string.Format("{0:-0." + dCor + "%;+0." + dCor + "%;0." + dCor + "%}", (double)((DataTable)data[0]).Rows[i]["DISC"] / 100),
                                            string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["SUM"]),
                                            string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["ASUM"]),
                                            string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", ((DataTable)data[0]).Rows[i]["TAX_MONEY"]),
                                            string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", MathLib.GetRoundedMoney((double)((System.Data.DataTable)data[0]).Rows[i]["SUM"] - (double)((System.Data.DataTable)data[0]).Rows[i]["ASUM"])),
                                            string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals + "}", MathLib.GetRoundedDose(MathLib.GetDouble(((System.Data.DataTable)data[0]).Rows[i]["TOT"]) - (double)((System.Data.DataTable)data[0]).Rows[i]["PRINTCOUNT"]))
                                        );
                                    }
                                    catch (Exception ex) { CoreLib.WriteLog(ex, "FeetchTemplate(); at Generating row template for article."); }

                                    if (!template.Contains("!ZeRo!"))
                                    {
                                        if (i == 0)
                                            template = template.TrimStart(new char[] { '\n', '\r' });
                                        if (i + 1 == ((DataTable)data[0]).Rows.Count)
                                            template = template.TrimEnd(new char[] { '\n', '\r' });

                                        template = GetHEX(template, _separators);
                                        streamWr.WriteLine(template);

                                        // indicates that this product is printed
                                        productPrintState = true;
                                    }
                                }

                            break;
                        }
                    #endregion
                    default:
                        {
                            template = GetHEX(template, _separators);
                            streamWr.WriteLine(template);
                            break;
                        }
                }
            }

            //End fill data
            streamWr.Close();
            streamWr.Dispose();
            streamRd.Close();
            streamRd.Dispose();

            if (!productPrintState)
                chqTxtName = string.Empty;

            //Path for txt file.
            return chqTxtName;
        }//ok
        public static string FormTxtBill(string tplFile, DataTable dTable)
        {
            if (!File.Exists(tplFile))
                return "";

            if (!Directory.Exists(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp))
                Directory.CreateDirectory(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp);

            string dCor = string.Empty;
            for (byte i = 0; i < driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals; i++)
                dCor += '0';

            //Set directives
            byte LoAN = 15;
            byte LoAD = 20;
            byte TSpL = 0;

            //Load messages
            Dictionary<string, string> _MSG = new Dictionary<string, string>();
            using (System.Xml.XmlTextReader xmlRd = new System.Xml.XmlTextReader(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Templates + "\\" + "Messages.xml"))
            {
                string varName = string.Empty;
                xmlRd.WhitespaceHandling = System.Xml.WhitespaceHandling.None;
                while (xmlRd.Read())
                    if (xmlRd.NodeType == System.Xml.XmlNodeType.Element && xmlRd.Name != null
                        && xmlRd.Name != string.Empty && xmlRd.Name.ToUpper() != "MESSAGES")
                    {
                        varName = xmlRd.Name;
                        xmlRd.Read();
                        _MSG.Add(varName.ToUpper(), xmlRd.Value.TrimStart('\r', '\n').TrimEnd('\r', '\n'));
                    }
                xmlRd.Close();
            }

            // Aligment blocks
            string[][] _alignBlocks = new string[2][];
            string[] _separators = new string[] { "[hex:", "[dec:" };
            _alignBlocks[0] = new string[] { "[right]", "[center]" };
            _alignBlocks[1] = new string[] { "[\\right]", "[\\center]" };
            // Separators

            //Making info values
            #region Info values
            object[] infoV = new object[7];
            //00 - subUnit (byte)
            infoV[00] = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit;//0
            //01 - subUnitName (string)
            infoV[01] = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnitName;//1
            //02 - payDesk (byte)
            infoV[02] = driver.Config.ConfigManager.Instance.CommonConfiguration.APP_PayDesk;//2
            //03 - cashierName (string)
            infoV[03] = UserConfig.UserID;//3
            //04 - currentDateTime (DateTime)
            infoV[04] = DateTime.Now;//4
            //05 - billNumber (string)
            infoV[05] = dTable.ExtendedProperties["NOM"].ToString();//05
            //19 - billComment (string)
            infoV[06] = dTable.ExtendedProperties["CMT"].ToString();//06

            #endregion

            //Creating output file
            string chqTxtName = string.Format("{0:X2}{1:X2}_{2:yyMMdd}_{2:HHmmss}.txt", driver.Config.ConfigManager.Instance.CommonConfiguration.APP_SubUnit, driver.Config.ConfigManager.Instance.CommonConfiguration.APP_PayDesk, DateTime.Now);
            StreamWriter streamWr = new StreamWriter(driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp + "\\" + chqTxtName, false, Encoding.Default);
            chqTxtName = driver.Config.ConfigManager.Instance.CommonConfiguration.Path_Temp + "\\" + chqTxtName;

            //Load template
            StreamReader streamRd = File.OpenText(tplFile);

            //Fill data by loaded template
            string template = string.Empty;
            while ((template = streamRd.ReadLine()) != null)
            {
                if (template == string.Empty)
                    template = " ";

                template = template.Replace("\\r", "\r");
                template = template.Replace("\\n", "\n");
                template = template.Replace("\\t", "\t");
                template = template.Replace("\\v", "\v");
                template = template.Replace("\\b", "\b");

                switch (template[0])
                {
                    #region Info values [S]
                    case 'S':
                        {
                            template = template.Remove(0, 1);
                            template = string.Format(template, infoV);

                            if (!template.Contains("!ZeRo!"))
                            {
                                template = GetHEX(template, _separators);
                                streamWr.WriteLine(template);
                            }
                            break;
                        }
                    #endregion
                    #region Articles [W]
                    case 'W':
                        {
                            //Output Format [W]
                            //00 - articleName (string)
                            //01 - articleDecription (string)
                            //02 - articleUnit (string)
                            //03 - articleTaxChar (char)
                            //04 - articlePrice (double)
                            //05 - articleDose (double)
                            //06 - articleTaxValue (double)
                            //07 - articleDiscount (double)
                            //08 - articleSuma (doulble)
                            //09 - articleActualSum (double)
                            //10 - articleTaxMoney (double)
                            //11 - articleCashDiscount (double)

                            //static values parsed as string [Program decimal correction]
                            //20 - articlePrice (double)
                            //21 - articleDose (double)
                            //22 - articleTaxValue (double)
                            //23 - articleDiscount (double)
                            //24 - articleSuma (double)
                            //25 - articleActualSum (double)
                            //26 - articleTaxMoney (double)
                            //27 - articleCashDiscount (double)

                            string articleTemplate = template.Remove(0, 1);
                            string art_name = "";
                            string art_desc = "";
                            double _pTotal = 0.0;

                            for (int i = 0; i < dTable.Rows.Count; i++)
                            {
                                art_name = dTable.Rows[i]["NAME"].ToString();
                                art_desc = dTable.Rows[i]["DESC"].ToString();

                                if (art_name.Length > LoAN)
                                    art_name = art_name.Substring(0, LoAN);

                                if (art_desc.Length > LoAD)
                                    art_desc = art_desc.Substring(0, LoAD);

                                _pTotal = Convert.ToDouble(dTable.Rows[i]["TOT"]) - (double)dTable.Rows[i]["PRINTCOUNT"];
                                _pTotal = MathLib.GetRoundedDose(_pTotal);

                                if (_pTotal > 0.0)
                                {
                                    template = string.Format(articleTemplate,
                                        //0
                                        art_name,
                                        art_desc,
                                        dTable.Rows[i]["UNIT"],
                                        dTable.Rows[i]["VG"],
                                        dTable.Rows[i]["PRICE"],
                                        _pTotal,
                                        dTable.Rows[i]["TAX_VAL"],
                                        "!ZeRo!",
                                        dTable.Rows[i]["SUM"],
                                        dTable.Rows[i]["ASUM"],
                                        //10
                                        dTable.Rows[i]["TAX_MONEY"],
                                        MathLib.GetRoundedMoney((double)dTable.Rows[i]["SUM"] - (double)dTable.Rows[i]["ASUM"]),//11
                                        null,//12
                                        null,//13
                                        null,//14
                                        null,//15
                                        null,//16
                                        null,//17
                                        null,//18
                                        null,//19
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", dTable.Rows[i]["PRICE"]),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals + "}", _pTotal),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", dTable.Rows[i]["TAX_VAL"]),
                                        "!ZeRo!",
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", dTable.Rows[i]["SUM"]),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", dTable.Rows[i]["ASUM"]),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", dTable.Rows[i]["TAX_MONEY"]),
                                        string.Format("{0:F" + driver.Config.ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", MathLib.GetRoundedMoney((double)dTable.Rows[i]["SUM"] - (double)dTable.Rows[i]["ASUM"]))
                                    );

                                    if (!template.Contains("!ZeRo!"))
                                    {
                                        if (i == 0)
                                            template = template.TrimStart(new char[] { '\n', '\r' });
                                        if (i + 1 == dTable.Rows.Count)
                                            template = template.TrimEnd(new char[] { '\n', '\r' });

                                        template = GetHEX(template, _separators);
                                        streamWr.WriteLine(template);
                                    }
                                }
                            }

                            break;
                        }
                    #endregion
                    #region Directives [#]
                    case '#':
                        {
                            //Template variables
                            //LoAN - Limit of Article Name (byte)
                            //LoAD - Limit of Article Description (byte)
                            //TSpL - Total Symbols per Line (byte)

                            template = template.Remove(0, 1);
                            object[] var = template.Split('=');

                            try
                            {
                                switch (var[0].ToString())
                                {
                                    case "LoAN": LoAN = byte.Parse(var[1].ToString()); break;
                                    case "LoAD": LoAD = byte.Parse(var[1].ToString()); break;
                                    case "TSpL": TSpL = byte.Parse(var[1].ToString()); break;
                                }
                            }
                            catch { }

                            break;
                        }
                    #endregion
                    #region Messages [M]
                    case 'M':
                        {
                            template = template.Remove(0, 1).ToUpper();
                            string[] keys = new string[_MSG.Keys.Count];
                            string[] blocks = new string[0];
                            string newLine = string.Empty;
                            string cuttedLine = string.Empty;
                            int startIndex = 0;
                            int length = 0;
                            int i = 0;
                            int j = 0;

                            _MSG.Keys.CopyTo(keys, 0);

                            try
                            {
                                for (i = 0; i < _MSG.Keys.Count; i++)
                                    template = template.Replace(keys[i], _MSG[keys[i]]);
                            }
                            catch { }

                            //Aligment
                            for (i = 0; i < _alignBlocks[0].Length; i++)
                            {
                                while (template.Contains(_alignBlocks[0][i]) && template.Contains(_alignBlocks[1][i]))
                                {
                                    startIndex = template.IndexOf(_alignBlocks[0][i]) + _alignBlocks[0][i].Length;
                                    length = template.IndexOf(_alignBlocks[1][i]) - startIndex;

                                    newLine = cuttedLine = template.Substring(startIndex, length);
                                    blocks = newLine.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                                    newLine = string.Empty;
                                    for (j = 0; j < blocks.Length; j++)
                                    {
                                        switch (_alignBlocks[0][i])
                                        {
                                            case "[center]":
                                                if (blocks[j].Length % 2 == 0)
                                                    length = (TSpL - blocks[j].Length) / 2 + blocks[j].Length;
                                                else
                                                    length = (TSpL - (blocks[j].Length - 1)) / 2 + blocks[j].Length;
                                                break;
                                            case "[right]":
                                                length = TSpL;
                                                break;
                                        }
                                        blocks[j] = blocks[j].PadLeft(length);
                                        newLine += blocks[j];
                                        if (j + 1 < blocks.Length)
                                            newLine += "\r\n";
                                    }
                                    cuttedLine = _alignBlocks[0][i] + cuttedLine + _alignBlocks[1][i];
                                    template = template.Replace(cuttedLine, newLine);
                                }

                                //remove first single aligment's block
                                if (template.Contains(_alignBlocks[0][i]) && !template.Contains(_alignBlocks[1][i]))
                                    template = template.Replace(_alignBlocks[0][i], "");

                                //remove end single aligment's block
                                if (!template.Contains(_alignBlocks[0][i]) && template.Contains(_alignBlocks[1][i]))
                                    template = template.Replace(_alignBlocks[1][i], "");
                            }

                            template = GetHEX(template, _separators);
                            streamWr.WriteLine(template);

                            break;
                        }
                    #endregion
                    #region Comment [;]
                    case ';':
                        {
                            break;
                        }
                    #endregion
                    default:
                        {
                            template = GetHEX(template, _separators);
                            streamWr.WriteLine(template);
                            break;
                        }
                }
            }

            //End fill data
            streamWr.Close();
            streamWr.Dispose();
            streamRd.Close();
            streamRd.Dispose();

            //Path for txt file.
            return chqTxtName;
        }
        #region PrivateFunctions
        private static string GetHEX(string data, string[] separator)
        {
            int pos = 0;
            string val = null;
            string oData = data;
            string newData = string.Empty;
            for (int i = 0; i < separator.Length; i++)
            {
                while ((pos = data.ToLower().IndexOf(separator[i])) >= 0 && pos + 5 < data.Length)
                    try
                    {
                        //val = data.Substring(pos + 5, 2);
                        int closeScobe = data.IndexOf(']', pos + 5 + 1);
                        val = data.Substring(pos + 5, closeScobe - (pos + 5));

                        newData = string.Empty;
                        switch (separator[i])
                        {
                            case "[hex:":
                                newData += (char)Convert.ToByte(val, 16);
                                data = data.Replace("[hex:" + val + "]", newData);
                                break;
                            case "[dec:":
                                newData += Convert.ToChar(byte.Parse(val));
                                data = data.Replace("[dec:" + val + "]", newData);
                                break;
                        }
                    }
                    catch
                    {
                        break;
                    }
            }

            return data;
        }
        #endregion

        public static string WriteToReport()
        {
            string p = string.Empty;

            return p;
        }


    }
}
