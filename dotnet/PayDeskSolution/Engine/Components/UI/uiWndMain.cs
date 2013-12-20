using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PayDesk.Components.UI.wndBills;
using System.Collections;
using driver.Config;
using driver.Common;
using driver.Lib;
using components.Components.SecureRuntime;
using components.Components.WinApi;
using components.Shared.Attributes;
using components.Shared.Interfaces;
using driver.Components;
using driver.Components.UI;
using components.Components.XmlDocumentParser;
using driver.Components.Objects;
using components.Components.MMessageBox;
using components.Components.HashObject;
using components.Public;
using components.UI.Controls.SensorUgcPanel;
using components.UI.Controls.SensorDataPanel;

/*
 * Notes:
 *  Application for perform selling products
 *  in the shops, supermarkets, fast-foods, restourants
 *  and similar places.
 * 
 *  Application can't used as single program.
 *
 *  Have nice day:)
 */
namespace PayDesk.Components.UI
{
    /// <summary>
    /// Main application class
    /// </summary>
    public partial class uiWndMain : Form
    {
        
        // Administrator access defender
        private uiWndAdmin admin;
        // Main Data
        private DataTable Cheque;
        private DataTable Articles;
        private DataTable AltBC;
        private DataTable Cards;
        // Scanner Data
        private string chararray;
        private DateTime lastInputChar;
        // Order Data
        private double chqSUMA;
        private double realSUMA;
        private double taxSUMA;

        private string clientID;
        private byte currentSubUnit;
        private bool nakladna;
        private int currSrchType;
        private int lastPayment;
        private bool currentModeIsSingle;
        // Cheque Types
        private bool retriveChq;
        private bool inventChq;
        // Discount Data
        private double discConstPercent;
        private int clientPriceNo;
        private double[] discArrPercent = new double[2] { 0.0, 0.0 };
        private double[] discArrCash = new double[2] { 0.0, 0.0 };
        private double discOnlyPercent = 0.0;
        private double discOnlyCash = 0.0;
        private double discCommonPercent = 0.0;
        private double discCommonCash = 0.0;
        private bool discApplied = false;
        /* new data */
        components.Components.DataContainer.DataContainer dataContainer2;

        private DataSet Data = new DataSet();
        private Hashtable Discount = new Hashtable();
        private Hashtable Summa = new Hashtable();
        private DataSet Cheques = new DataSet();
        private Dictionary<string, bool> State = new Dictionary<string, bool>();

        // Statements
        private bool _fl_artUpdated;
        private bool _fl_canUpdate = false;
        private bool _fl_onlyUpdate;
        private bool ADMIN_STATE = false;
        private bool _fl_menuIsActive = false;
        private bool _fl_SubUnitChanged = false;
        private bool _fl_useTotDisc = true;
        private bool _fl_isOk = false;
        //private bool _fl_modeChanged = false;

        private Hashtable GetAppStatements()
        {
            Hashtable currentStates = new Hashtable();
            currentStates[CoreConst.STATE_DATA_UPDATED] = false;

            return currentStates;
        }
        
        /// <summary>
        /// Application's Constructor
        /// </summary>
        public uiWndMain()
        {
            InitializeComponent();

            dataContainer2 = DataWorkShared.GetDataContainer();

            // initialize statements
            dataContainer2.Structures[CoreConst.CONTAINER_STATE, CoreConst.STATE_CALC_USE_TOTAL_DISC] = true;


            dataContainer2.Structures[CoreConst.CONTAINER_STATE].UpdateMethod = this.GetAppStatements;
            //dataContainer2.Structures[CoreConst.CONTAINER_STATE] = this.GetAppStatements;
            dataContainer2.Structures[CoreConst.CONTAINER_STATE].Update();
            

            //Program.AppPlugins.GetActive<IAppUI>().Execute(this);
                    
           // Settings1.Default["test"] = "gfdgdgdf";
            //Settings1.Default.Save();
            /*
            Properties.Settings.Default.Context.Add("gdfgdfg", "gdfgdfg");
            Properties.Settings.Default.Save();

            System.Configuration.ConfigXmlDocument f = new System.Configuration.ConfigXmlDocument();
            f.LoadXml("PayDesk.exe.config");
            System.Configuration.SettingElement g = new System.Configuration.SettingElement("gfdgf", System.Configuration.SettingsSerializeAs.Xml);
            System.Configuration.SettingsContext sc = new System.Configuration.SettingsContext();
            System.Configuration.LocalFileSettingsProvider lfs = new System.Configuration.LocalFileSettingsProvider();
            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();
            nvc.Add("test2", "tttt"); lfs.Initialize("PayDesk.xml", nvc);
            lfs.Reset(sc);


            */

            

            /*
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new BinaryFormatter();

            XmlPO.XmlParserObject xobj = new XmlPO.XmlParserObject();
            */
            /*
            System.Collections.Hashtable _driverData = new System.Collections.Hashtable();
            _driverData.Add("Status", 1);
            _driverData.Add("ErrorStatus", 1);
            _driverData.Add("ArtMemorySize", (uint)11800);
            _driverData.Add("UserNo", (byte)1);
            _driverData.Add("UserPwd", "0000");
            _driverData.Add("DeskNo", (byte)1);
            
            System.Collections.Hashtable _driverData2 = new System.Collections.Hashtable();
            _driverData2.Add("LastFunc", "");
            _driverData2.Add("LastArtNo", (uint)1);
            _driverData2.Add("LastFOrderNo", (uint)0);
            _driverData2.Add("LastNOrderNo", (uint)0);
            _driverData2.Add("LastROrderNo", (uint)0);

            System.Collections.Hashtable _xData = new System.Collections.Hashtable();
            _xData.Add("testBlock", _driverData);
            _xData.Add("testBlock2", _driverData2);
            _xData.Add("listtest", new int[] { 1, 2, 3, 5, 7, 0 });
            
            xobj.ParseDataToXml("myxmlfile.xml", _xData);
            */
            /*
            System.Collections.Hashtable ldata = xobj.ParseXmlToData("AppConfig.xml");
            ;

            XmlPO.Components.Objects.SectionElement section = new XmlPO.Components.Objects.SectionElement("simple");
            section.Comment = "demo test";

            XmlPO.Components.Objects.BlockElement b = new XmlPO.Components.Objects.BlockElement("b1");
            XmlPO.Components.Objects.PropertyElement p = new XmlPO.Components.Objects.PropertyElement("p1");
            p.Value = 12345;
            b.Add(p);
            section.Add(b);
            XmlPO.Components.Objects.DocumentElement doc = new XmlPO.Components.Objects.DocumentElement();
            doc.AddContainer(section);



            xobj.ParseDocumentToXml("t.xml", doc);
            */
            //System.Collections.Hashtable ldata = xobj.ParseXmlToData("myxmlfile.xml");


            //ComPort _cport = new ComPort();
            //PayDesk.Config.AppSettings sett = new Config.AppSettings();

           /* ConfigManager cm = new ConfigManager();
            sett.Tax_AppTaxChar = new char[] { 'a' };
            sett.Tax_AppTaxDisc = new bool[] { true };
            sett.PRN_Templates = new Dictionary<string, object>();
            sett.PRN_Templates.Add("test", new Dictionary<string, object>(2));
            string rez = cm.SerializeObject(sett, sett.GetType());
            cm.Save(rez);*/
        }

        ~uiWndMain()
        {
            global::components.Components.SerialPort.Com_SerialPort.CloseAllPorts(true);
        }

        /// <summary>
        /// Перевизначений метод для виконання операцій відновлення
        /// інтерфейсу та інших параметрів програми під час її завантаження
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            /* initialise data values */


            
            /* loop by all available profiles */
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                {
                    this.Discount.Add(de.Key, DataWorkShared.GetStandartDiscountInfoStructure2());
                    this.Summa.Add(de.Key, DataWorkShared.GetStandartCalculationInfoStructure2());
                }
            else
            {
                this.Discount.Add(CoreConst.KEY_DEFAULT_PROFILE_ID, this.PD_DiscountInfo);
                this.Summa.Add(CoreConst.KEY_DEFAULT_PROFILE_ID, DataWorkShared.GetStandartCalculationInfoStructure2());
            }

            admin = new uiWndAdmin();
            admin.OwnerControlEx = this.chequeDGV;

            //winapi.Funcs.OutputDebugString("load_begin");
            this._fl_isOk = new Com_SecureRuntime().FullLoader();


            //Restore position
            this.Text = string.Format("{2}{1}{0}", Application.ProductName, " - ", ConfigManager.Instance.CommonConfiguration.APP_SubUnitName);
            this.WindowState = ConfigManager.Instance.CommonConfiguration.STYLE_MainWndState;
            this.Location = new Point(ConfigManager.Instance.CommonConfiguration.STYLE_MainWndPosition.X, ConfigManager.Instance.CommonConfiguration.STYLE_MainWndPosition.Y);
            this.Size = new Size(ConfigManager.Instance.CommonConfiguration.STYLE_MainWndSize.Width, ConfigManager.Instance.CommonConfiguration.STYLE_MainWndSize.Height);
            this.splitContainer1.Panel2Collapsed = ConfigManager.Instance.CommonConfiguration.STYLE_ArtSideCollapsed;
            
            try
            {
                this.splitContainer1.Orientation = ConfigManager.Instance.CommonConfiguration.STYLE_SplitOrient;
                this.splitContainer1.SplitterDistance = ConfigManager.Instance.CommonConfiguration.STYLE_SplitterDistance;
            }
            catch { }
            //updateThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CheckForUpdate));

            ADMIN_STATE = UserConfig.AdminState;

            //create tables


            DataWorkSource.CreateTables(ref Cheque, ref Articles, ref AltBC, ref Cards, ref Cheques);
            this.CreateOrderStructure(this.Cheque);
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                    this.CreateOrderStructure(this.Cheques.Tables[de.Key.ToString()]);

            // new feature: this.bdo

            chequeDGV.DataSource = Cheque;
            articleDGV.DataSource = Articles;
            DataGridView[] grids = new DataGridView[] { chequeDGV, articleDGV };
            ViewLib.LoadGridsView(ref grids, splitContainer1.Orientation);

            //if (ConfigManager.Instance.CommonConfiguration.Path_Exchnage == string.Empty)
            //{
            //    MMessageBoxEx.Show("Вкажіть шлях до папки обміну", Application.ProductName,
            //        MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            //    folderBrowserDialog1.ShowDialog();
            //    ConfigManager.Instance.CommonConfiguration.Path_Exchnage = folderBrowserDialog1.SelectedPath;
            //    /*
            //    if (Program.MainArgs.ContainsKey("-c") && Program.MainArgs["-c"] != null)
            //        ConfigManager.Instance.CommonConfiguration.SaveData(Program.MainArgs["-c"].ToString());
            //    else*/
            //    ConfigManager.SaveConfiguration();
            //}

            UpdateMyControls();


            // temporary refresh skins
            Com_WinApi.OutputDebugString("RefershStyles_Start");
            if (ConfigManager.Instance.CommonConfiguration.skin_sensor_active)
            {
                this.сенсорToolStripMenuItem.PerformClick();
            }

            Com_WinApi.OutputDebugString("RefershStyles_End");

            //Set default type of search
            SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
            UpdateSumInfo(true);


            this.Activate();
            this.BringToFront();
            this.UpdateZOrder();

            if (ConfigManager.Instance.CommonConfiguration.APP_BuyerBarCodeSource != 0)
            {
                // port config

                //Hashtable xmlPortCfg = ComPort.GetXmlPortConfiguration("MagstripeCardReaderPort.xml");
                Hashtable xmlPortCfg = (Hashtable)ApplicationConfiguration.Instance["serialPortConnect.additionalDevices.magstripe"];
                bool error = false;
                try
                {
                    this.serialPort1.PortName = xmlPortCfg["PORT"].ToString();
                    this.serialPort1.BaudRate = int.Parse(xmlPortCfg["RATE"].ToString());
                    this.serialPort1.DataBits = int.Parse(xmlPortCfg["DBITS"].ToString());
                    this.serialPort1.StopBits = (System.IO.Ports.StopBits)(int.Parse(xmlPortCfg["SBITS"].ToString()));
                    this.serialPort1.Parity = (System.IO.Ports.Parity)(int.Parse(xmlPortCfg["PARITY"].ToString()));
                }
                catch (Exception ex)
                {
                    error = true;
                    driver.Lib.CoreLib.WriteLog(ex, "Неможливо налаштувати сом-порт зчитувача карток");
                    MMessageBox.Show("Неможливо налаштувати сом-порт зчитувача карток.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                if (!error)
                    try
                    {
                        this.serialPort1.Open();
                        this.timer_buyer_ready.Start();
                    }
                    catch (Exception ex)
                    {
                        driver.Lib.CoreLib.WriteLog(ex, "Неможливо відкрити сом-порт зчитувача карток");
                        MMessageBox.Show("Неможливо відкрити сом-порт зчитувача карток.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            }
            else
            {
                this.timer_buyer_ready.Stop();
            }

            if (Program.AppPlugins.IsActive(PluginType.LegalPrinterDriver))
            {
                try
                {
                    bool status = (bool)Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_SetCashier", ConfigManager.Instance.CommonConfiguration.APP_PayDesk, UserConfig.UserFpLogin, UserConfig.UserFpPassword, UserConfig.UserID);
                    if (status)
                        DDM_FPStatus.Image = Properties.Resources.ok;
                    else
                    {
                        MMessageBoxEx.Show(this.chequeDGV, "Немає зв'язку з фіскальним пристроєм.\r\nНеможливо зареєструвати касира в ЕККР",
                            Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        DDM_FPStatus.Image = Properties.Resources.FpNotOk;
                    }
                }
                catch (Exception ex)
                {
                    DDM_FPStatus.Image = Properties.Resources.FpNotOk;
                    driver.Lib.CoreLib.WriteLog(ex, "Немає зв'язку з фіскальним пристроєм.\r\nНеможливо зареєструвати касира в ЕККР");
                    MMessageBoxEx.Show(this.chequeDGV, ex.Message + "\r\nНемає зв'язку з фіскальним пристроєм.\r\nНеможливо зареєструвати касира в ЕККР",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else
                DDM_FPStatus.Image = Properties.Resources.FpNotOk;

            this.label_uiWndmain_DemoShowArt.Visible = this.label_uiWndmain_DemoShowChq.Visible = !this._fl_isOk;

            // set additional devices
            //components.Components.SerialPort.

            //global::components.Components.SerialPort.Com_SerialPort.GetAndConfigurePort("scales", (Hashtable)ApplicationConfiguration.Instance["serialPortConnect.additionalDevices.scale"]);
            //WinAPI.OutputDebugString("load_end");
        }
        /// <summary>
        /// Event checking for window resize
        /// </summary>
        /// <param name="e">Event argument</param>
        protected override void OnSizeChanged(EventArgs e)
        {/*
            if (WindowState == FormWindowState.Normal)
                ConfigManager.Instance.CommonConfiguration.STYLE_MainWndSize = this.Size;*/
            base.OnSizeChanged(e);
        }
        /// <summary>
        /// Event check changes of window position
        /// </summary>
        /// <param name="e">Event argument</param>
        protected override void OnLocationChanged(EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                ConfigManager.Instance.CommonConfiguration.STYLE_MainWndPosition = this.Location;
            if (WindowState == FormWindowState.Minimized)
                this.OnDeactivate(EventArgs.Empty);
            else
                this.OnActivated(EventArgs.Empty);
            base.OnLocationChanged(e);
        }
        /// <summary>
        /// Event perform when window closing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (inventChq)
            {
                MMessageBoxEx.Show(this.chequeDGV, "Відкритий чек інвентаризації!", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            DialogResult rez = DialogResult.No;

            if (chequeDGV.RowCount != 0)
            {
                MMessageBoxEx.Show(this.chequeDGV, "Закрийте чек перед тим, як вийти", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
                return;
            }

            if (DialogResult == DialogResult.Retry)
                rez = MMessageBoxEx.Show(this.chequeDGV, "Змінити касира ?", Application.ProductName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            else
                rez = MMessageBoxEx.Show(this.chequeDGV, "Вийти з програми ?", Application.ProductName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            switch (rez)
            {
                case DialogResult.Yes:
                    {
                        // save data
                        //AppConfig.SaveData();

                        if (ConfigManager.Instance.CommonConfiguration.APP_ClearTEMPonExit)
                            FileMgrLib.ClearFolder(ConfigManager.Instance.CommonConfiguration.Path_Temp);
                        if (Program.AppPlugins.IsActive(PluginType.LegalPrinterDriver))
                        {
                            try
                            {
                                Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_SetCashier", UserConfig.UserPassword, UserConfig.UserLogin, 0, string.Empty);
                            }
                            catch (Exception ex)
                            {
                                MMessageBoxEx.Show(this.chequeDGV, ex.Message + "\r\nНеможливо розреєструвати касира в ЕККР",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        break;
                    }
                default:
                    {
                        DialogResult = DialogResult.None;
                        e.Cancel = true;
                        break;
                    }
            }

            base.OnClosing(e);
        }
        /// <summary>
        /// Event catch every pressed key
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            //winapi.WinAPI.OutputDebugString(string.Concat((char)e.KeyValue));

            // sensor: redirecting hot key to wndproc
            /*
            if (e.KeyCode == Keys.Enter)
                Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_Enter), new IntPtr(0));
            */
            /*
            if (!UserConfig.Properties[22])
            {
                if (lastInputChar.AddMilliseconds(130) < DateTime.Now)
                    chararray = string.Empty;

                if (!e.Alt && !e.Control && !e.Shift)
                {
                    lastInputChar = DateTime.Now;
                    chararray += (char)e.KeyValue;
                    //winapi.WinAPI.OutputDebugString(chararray);
                }
            }

            if (e.Control && !e.SuppressKeyPress && chequeDGV.CurrentCell != null && chequeDGV.CurrentCell.IsInEditMode)
                chequeDGV.EndEdit();
            */

            //base.OnKeyDown(e);

            //winapi.WinAPI.OutputDebugString(string.Concat((char)e.KeyValue));

            //winapi.WApi.OutputDebugString("code=" + e.KeyCode + ": data=" + e.KeyData + ": value=" + e.KeyValue);
            //if (!UserConfig.Properties[22] && !this.SrchTbox.Focused)
            //{
            if (ConfigManager.Instance.CommonConfiguration.APP_ScannerUseProgamMode)
            {
                if (lastInputChar.AddMilliseconds(ConfigManager.Instance.CommonConfiguration.APP_ScannerCharReadFrequency) < DateTime.Now)
                    chararray = string.Empty;

                if (!e.Alt && !e.Control)
                {
                    lastInputChar = DateTime.Now;
                    chararray += (char)e.KeyValue;

                    global::components.Components.WinApi.Com_WinApi.OutputDebugString("E: value = " + e.KeyValue + "  code = " + e.KeyCode.ToString() + "  data = " + e.KeyData.ToString());
                    global::components.Components.WinApi.Com_WinApi.OutputDebugString("received char = [" + chararray + "]");
                    //winapi.WApi.OutputDebugString("E: value = " + e.KeyValue + "  code = " + e.KeyCode.ToString() + "  data = " + e.KeyData.ToString());
                    //winapi.WApi.OutputDebugString("received char = [" + chararray + "]");
                }
                // custom end char
                if (this.currSrchType != 0 && chararray.Length != 0 && chararray[chararray.Length - 1] == ' ')
                {
                    chararray = chararray.Substring(0, chararray.Length - 1);
                    //winapi.WApi.OutputDebugString("CUSTOM BARCODE END SYMBOL");
                    Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_Enter), new IntPtr(0));
                }

            }
            else
                chararray = string.Empty;
            //}


            if (e.Control && !e.SuppressKeyPress && chequeDGV.CurrentCell != null && chequeDGV.CurrentCell.IsInEditMode)
            {
                Com_WinApi.OutputDebugString("EndEdit perform");
                chequeDGV.EndEdit();
            }

        }
        /// <summary>
        /// Event of window activation
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            //winapi.API.OutputDebugString("regHotKeys");

            base.OnActivated(e);

            Keys myHotKey = Keys.Delete | Keys.Control;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_CtrlDel);

            myHotKey = Keys.Delete | Keys.Control | Keys.Shift;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_CtrlShiftDel);

            myHotKey = Keys.PageDown | Keys.Control;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_CtrlPgDn);

            myHotKey = Keys.PageUp | Keys.Control;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_CtrlPgUp);

            myHotKey = Keys.Delete | Keys.Shift;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_ShiftDel);

            myHotKey = Keys.Enter;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_Enter);

            myHotKey = Keys.Enter | Keys.Control;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_CtrlEnter);

            myHotKey = Keys.Enter | Keys.Control | Keys.Shift;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_CtrlShiftEnter);

            myHotKey = Keys.F5;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_F5);

            myHotKey = Keys.F6;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_F6);

            myHotKey = Keys.F7;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_F7);

            myHotKey = Keys.F8;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_F8);

            myHotKey = Keys.F9;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_F9);

            myHotKey = Keys.Escape;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_Esc);

            myHotKey = Keys.Q | Keys.Control;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_CtrlQ);

            myHotKey = Keys.Control;
            CoreLib.RegisterHotKey(this, myHotKey, CoreLib.MyHotKeys.HK_Ctrl);

            Com_WinApi.OutputDebugString("regHotKeys");
        }
        /// <summary>
        /// Event of window deactivation
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDeactivate(EventArgs e)
        {
            //winapi.API.OutputDebugString("un_RegHotKeys");
            base.OnDeactivate(e);
            for (int i = 0x10; i < 0x20; i++)
                CoreLib.UnregisterHotKey(this, i);
            Com_WinApi.OutputDebugString("un_RegHotKeys");
        }
        /// <summary>
        /// Перевизначений метод обробки повідомлень.
        /// </summary>
        /// <param name="m">Повідомлення</param>
        protected override void WndProc(ref Message m)
        {
                            
            base.WndProc(ref m);

            //winapi.WinAPI.OutputDebugString(m.ToString());

            if (m.Msg == (int)CoreLib.MyMsgs.WM_HOTKEY)
            {
                //winapi.Funcs.OutputDebugString("Q");
                #region hot key control
                switch (m.WParam.ToInt32())
                {
                    case 0x10:
                        #region CONTROL + DELETE
                        {
                            //if (this.Cheque.ExtendedProperties.ContainsKey("BILL") && this.Cheque.ExtendedProperties["BILL"] != null && bool.Parse(this.Cheque.ExtendedProperties["LOCK"].ToString()))
                            if ((bool)DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }
                            
                            if (Cheque.Rows.Count == 0)
                                break;//r

                            if (!getAdminAccess(24))
                                break;

                            //if (!(ADMIN_STATE || UserConfig.Properties[24]) && (DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.OWNER_NO, string.Empty).ToString() == string.Empty))
                            //    if (admin.ShowDialog() != DialogResult.OK)
                            //        break;//r

                            try
                            {
                                int index = chequeDGV.CurrentRow.Index;

                                if (DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.OID, string.Empty).ToString() != string.Empty)
                                {
                                    try
                                    {
                                        Dictionary<string, object[]> deletedRows = new Dictionary<string, object[]>();
                                        deletedRows = (Dictionary<string, object[]>)DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.DELETED_ROWS, deletedRows);
                                        //DataRow[] dRow = new DataRow[1] { };
                                        //this.Cheque.Rows.CopyTo(dRow, index);
                                        if (!deletedRows.ContainsKey(this.Cheque.Rows[index]["C"].ToString()))
                                        {
                                            deletedRows.Add(this.Cheque.Rows[index]["C"].ToString(), this.Cheque.Rows[index].ItemArray);
                                            DataWorkShared.SetBillProperty(this.Cheque, CoreConst.DELETED_ROWS, deletedRows);
                                        }
                                    }
                                    catch { }
                                }


                                object profileKey = this.Cheque.Rows[index]["F"];
                                object productId = this.Cheque.Rows[index]["ID"];
                                object productOID = this.Cheque.Rows[index]["C"];

                                Cheque.Rows.RemoveAt(index);

                                try
                                {
                                    DataRow dProfileRow = this.Cheques.Tables[profileKey.ToString()].Rows.Find(productOID);
                                    int idxPR = this.Cheques.Tables[profileKey.ToString()].Rows.IndexOf(dProfileRow);
                                    this.Cheques.Tables[profileKey.ToString()].Rows.RemoveAt(idxPR);
                                }
                                catch { }

                                //AppFunc.OutputDebugString("k");
                                if (m.LParam.ToInt32() == 0x100)
                                    RowsRemoved_MyEvent(true, false);
                                else
                                    RowsRemoved_MyEvent(true);
                                index--;
                                if (index < 0)
                                    if (Cheque.Rows.Count != 0)
                                        index = 0;
                                    else
                                        break;//r
                                chequeDGV.CurrentCell = chequeDGV.Rows[index].Cells[chequeDGV.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Name];
                                chequeDGV.Rows[index].Selected = true;
                            }
                            catch { }
                            break;
                        } 
                        #endregion 
                    case 0x11:
                        #region CONTROL + SHIFT + DELETE
                        {
                            //if (this.Cheque.ExtendedProperties.ContainsKey("BILL") && this.Cheque.ExtendedProperties["BILL"] != null && bool.Parse(this.Cheque.ExtendedProperties["LOCK"].ToString()))
                            if ((bool)DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }
                            
                            if (Cheque.Rows.Count == 0)
                                break;//r

                            if (!getAdminAccess(24))
                                return;

                            if (DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.OID, string.Empty).ToString() != string.Empty)
                            {
                                try
                                {
                                    Dictionary<string, object[]> deletedRows = new Dictionary<string, object[]>();
                                    deletedRows = (Dictionary<string, object[]>)DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.DELETED_ROWS, deletedRows);
                                    for (int index = 0; index < this.Cheque.Rows.Count ; index++)
                                    {
                                        if (!deletedRows.ContainsKey(this.Cheque.Rows[index]["C"].ToString()))
                                            deletedRows.Add(this.Cheque.Rows[index]["C"].ToString(), this.Cheque.Rows[index].ItemArray);
                                    }
                                    DataWorkShared.SetBillProperty(this.Cheque, CoreConst.DELETED_ROWS, deletedRows);
                                
                                }
                                catch { }
                            }


                            Cheque.Rows.Clear();
                            /*
                            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                                RowsRemoved_MyEvent_profile(de.Key);
                            */
                            if (m.LParam.ToInt32() == 0x100)
                                RowsRemoved_MyEvent(true, false, false);
                            else
                                RowsRemoved_MyEvent(true, true, true);
                            break;
                        } 
                        #endregion
                    case 0x12:
                        #region CONTROL + PageDown
                        {
                            if ((bool)DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }
                            
                            if (inventChq)
                                return;

                            if (!getAdminAccess(3))
                                return;

                            double discSUMA = 0.0;
                            try
                            {
                                discSUMA = (double)Cheque.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                            }
                            catch { }
                            uiWndDiscountRequest d = new uiWndDiscountRequest(discSUMA, true);
                            d.SetDiscount(ref discArrPercent, ref discArrCash);
                            d.Dispose();

                            if (discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 && discArrCash[0] == 0.0 && discArrCash[1] == 0.0)
                                ResetDiscount();
                            else
                            {
                                відмінитиЗнижкунадбавкуToolStripMenuItem.Enabled = true;
                                if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                                    відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку";
                                else
                                {
                                    if ((discArrPercent[0] != 0.0 && discArrPercent[1] != 0.0) || (discArrCash[0] != 0.0 && discArrCash[1] != 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку і націнку";
                                    if ((discArrPercent[0] == 0.0 && discArrPercent[1] != 0.0) || (discArrCash[0] == 0.0 && discArrCash[1] != 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати націнку";
                                    if ((discArrPercent[0] != 0.0 && discArrPercent[1] == 0.0) || (discArrCash[0] != 0.0 && discArrCash[1] == 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку";
                                }
                            }

                            UpdateSumInfo(true);
                            break;
                        } 
                        #endregion
                    case 0x13:
                        #region CONTROL + PageUp
                        {
                            if ((bool)DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }
                            
                            if (inventChq)
                                return;

                            if (!getAdminAccess(3))
                                return;

                            double discSUMA = 0;
                            try
                            {
                                discSUMA = (double)Cheque.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                            }
                            catch { }
                            uiWndDiscountRequest d = new uiWndDiscountRequest(discSUMA, false);
                            d.SetDiscount(ref discArrPercent, ref discArrCash);
                            d.Dispose();

                            if (discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 && discArrCash[0] == 0.0 && discArrCash[1] == 0.0)
                                ResetDiscount();
                            else
                            {
                                відмінитиЗнижкунадбавкуToolStripMenuItem.Enabled = true;
                                if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                                    відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати націнку";
                                else
                                {
                                    if ((discArrPercent[0] != 0.0 && discArrPercent[1] != 0.0) || (discArrCash[0] != 0.0 && discArrCash[1] != 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку і націнку";
                                    if ((discArrPercent[0] == 0.0 && discArrPercent[1] != 0.0) || (discArrCash[0] == 0.0 && discArrCash[1] != 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати націнку";
                                    if ((discArrPercent[0] != 0.0 && discArrPercent[1] == 0.0) || (discArrCash[0] != 0.0 && discArrCash[1] == 0.0))
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку";
                                }
                            }

                            UpdateSumInfo(true);
                            break;
                        } 
                        #endregion
                    case 0x14:
                        #region SHIFT + DELETE
                        {
                            if ((bool)DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }
                            
                            if (inventChq)
                                return;

                            ResetDiscount();
                            UpdateSumInfo(true);
                            addChequeInfo.Text = string.Empty;
                            break;
                        } 
                        #endregion
                    case 0x15:
                        #region ENTER
                        {

                            if (global::components.Components.WinApi.Com_WinApi.InSendMessage())
                            {
                                if (ConfigManager.Instance.CommonConfiguration.APP_ScannerUseProgamMode)
                                    System.Threading.Thread.Sleep(ConfigManager.Instance.CommonConfiguration.APP_ScannerCharReadFrequency + 1);
                                global::components.Components.WinApi.Com_WinApi.ReplyMessage(new IntPtr(0x15));
                            }
                            
                            //if (   this.Cheque.ExtendedProperties.ContainsKey("BILL") && this.Cheque.ExtendedProperties["BILL"] != null && bool.Parse(((Dictionary<string, object>)this.Cheque.ExtendedProperties["BILL"])["IS_LOCKED"].ToString()))
                            if ((bool)DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }
                            //for (int i = 0x10; i < 0x20; i++)
                            //AppFunc.UnregisterHotKey(this, i);

                            int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.PD_Order);
                            switch (changeState)
                            {
                                case 1:
                                    {
                                        MMessageBoxEx.Show(this.chequeDGV, "Були внесені змінити в поточний рахунок\r\nПерезавантажте рахунок за допомогою меню або натисніть ALT+R", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        break;
                                    }
                                case 2:
                                    {
                                        MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок вже закритий\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        _fl_menuIsActive = false;
                                        this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                        break;
                                    }
                                case -1:
                                    {
                                        MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок видалений з бази\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        _fl_menuIsActive = false;
                                        this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                        break;
                                    }
                            }
                            if (changeState != 0)
                                break;

                            bool editWasClosed = false;

                            //winapi.WinAPI.OutputDebugString("Enter");

                            if (!UserConfig.Properties[22])
                                if (ConfigManager.Instance.CommonConfiguration.APP_ScannerUseProgamMode && lastInputChar.AddMilliseconds(ConfigManager.Instance.CommonConfiguration.APP_ScannerCharReadFrequency) > DateTime.Now &&
                                    chararray != null && chararray.Length != 0)
                                {
                                    //if (chequeDGV.CurrentCell != null && chequeDGV.CurrentCell.IsInEditMode)
                                    //{
                                    //    if (chequeDGV.CurrentCell.EditedFormattedValue.ToString().Contains(chararray))
                                    //    {
                                    //        string val = chequeDGV.CurrentCell.EditedFormattedValue.ToString();
                                    //        int bcidx = val.IndexOf(chararray);
                                    //        val = val.Substring(0, bcidx);
                                    //        if (val == string.Empty)
                                    //            val = "0";
                                    //        chequeDGV.CurrentCell.Value = Convert.ToDouble(val);
                                    //    }
                                    //    chequeDGV.EndEdit();
                                    //    editWasClosed = true;
                                    //}
                                    bool okToProceed = true;
                                    if (chararray.Length == 1 && !char.IsDigit(chararray[0]))
                                        okToProceed = false;

                                    if (okToProceed)
                                    {
                                        Com_WinApi.OutputDebugString("SEARCH: ok to proceed by program");
                                        //winapi.WinAPI.OutputDebugString("srch: " + chararray);
                                        SearchFilter(false, 2, true);
                                        SrchTbox.Text = chararray;
                                    }
                                    else
                                    {
                                        Com_WinApi.OutputDebugString("SEARCH: no ok. Using textbox.");
                                        string _sameText = SrchTbox.Text;
                                        SearchFilter(false, 2, true);
                                        SrchTbox.Text = _sameText;
                                    }
                                    SrchTbox.Select();
                                    chararray = string.Empty;
                                }

                            //close edit
                            if (chequeDGV.CurrentCell != null && chequeDGV.CurrentCell.IsInEditMode)
                            {
                                chequeDGV.EndEdit();
                                editWasClosed = true;
                            }

                            //lastInputChar = DateTime.Now;
                            //launch article property
                            if (chequeDGV.Focused && chequeDGV.RowCount != 0)
                            {
                                if (!getAdminAccess(24))
                                    return;

                                DataRow dRow = Cheque.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                                Request req = new Request(dRow, MathLib.GetDouble(dRow["TOT"]));
                                req.UpdateRowSource(this.chequeDGV, this);
                                req.Dispose();
                                UpdateSumInfo(true);
                                break;//r
                            }

                            //Adding article to Cheque
                            if (articleDGV.Focused && articleDGV.RowCount != 0)
                            {
                                DataRow article = Articles.Rows.Find(articleDGV.CurrentRow.Cells["C"].Value);

                                if (!this._fl_isOk && this.Cheque.Rows.Count >= 3 && this.Cheque.Rows.Find(article["C"].ToString()) == null)
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Ви не можете продавати більше позицій в демо-режимі", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    break;
                                }

                                if (article != null)
                                {
                                    CoreLib.AddArticleToCheque(chequeDGV, articleDGV, article, ConfigManager.Instance.CommonConfiguration.APP_StartTotal, Articles);
                                    SearchFilter(true, this.currSrchType, false);
                                    // hide product panel when it was displayed automatically
                                    /*if (splitContainer1.Panel2.Tag != null)
                                    {
                                        вікноТоварівToolStripMenuItem.PerformClick();
                                        splitContainer1.Panel2.Tag = null;
                                        // skin sensor integration (show panel when some arts are hidden)
                                        if (this.сенсорToolStripMenuItem.Checked)
                                            this.chequeContainer.Panel2Collapsed = false;
                                    }*/
                                }
                                break;//r
                            }
                            //winapi.WinAPI.OutputDebugString("srch: " + SrchTbox.Text);

                            //Searching
                            if (!editWasClosed && SrchTbox.Focused && SrchTbox.Text != string.Empty)
                            {
                                DataTable sTable = Articles.Clone();
                                bool allowToShow = false;
                                int i = 0;

                                //Debug.Write("BeginAdd");
                                #region search box
                                if (SrchTbox.Text != "")
                                {
                                    switch (currSrchType)
                                    {
                                        case 0:
                                            {
                                                #region by name
                                                string[] words = SrchTbox.Text.Trim().Split(' ');
                                                DataRow[] dr1 = new DataRow[0];
                                                DataRow[] dr2 = new DataRow[0];
                                                DataTable dTable = (DataTable)articleDGV.DataSource;

                                                //string srchString = string.Empty;
                                                SrchTbox.Text = string.Empty;
                                                for (int l = 0; l < words.Length; l++)
                                                {
                                                    try
                                                    {
                                                        dr1 = dTable.Select("NAME Like '%" + words[l] + "%'");
                                                        dr2 = dTable.Select("DESC Like '%" + words[l] + "%'");
                                                    }
                                                    catch { }

                                                    sTable.Clear();
                                                    sTable.BeginLoadData();

                                                    if (dr1.Length > dr2.Length)
                                                    {
                                                        for (i = 0; i < dr1.Length; i++)
                                                            sTable.Rows.Add(dr1[i].ItemArray);
                                                    }
                                                    else
                                                    {
                                                        for (i = 0; i < dr2.Length; i++)
                                                            sTable.Rows.Add(dr2[i].ItemArray);
                                                    }

                                                    sTable.EndLoadData();

                                                    dTable = sTable.Copy();

                                                    if (dTable.Rows.Count > 0)
                                                    {
                                                        articleDGV.DataSource = dTable;
                                                        articleDGV.Select();
                                                        allowToShow = true;
                                                        SrchTbox.Text += words[l] + " ";
                                                        //SrchTbox.Select(0, srchString.Length);
                                                    }
                                                }

                                                if (SrchTbox.Text == string.Empty)
                                                //if (SrchTbox.SelectedText == string.Empty)
                                                {
                                                    MMessageBoxEx.Show(this.chequeDGV, "Нажаль нічого не вдалось знайти", "Результат пошуку",
                                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                                                }

                                                #endregion
                                                break;
                                            }
                                        case 1:
                                            {
                                                #region by id
                                                try
                                                {
                                                    DataRow[] dr = Articles.Select("ID Like \'" + SrchTbox.Text + "%\'");

                                                    if (dr.Length == 0)
                                                    {
                                                        MMessageBoxEx.Show(this.chequeDGV, "Нажаль нічого не вдалось знайти", "Результат пошуку", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                        SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                                                        break;
                                                    }
                                                    if (dr.Length == 1)
                                                    {
                                                        SearchFilter(false, currSrchType, true);
                                                        CoreLib.AddArticleToCheque(chequeDGV, articleDGV, dr[0], ConfigManager.Instance.CommonConfiguration.APP_StartTotal, Articles);
                                                        allowToShow = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        sTable.Clear();
                                                        sTable.BeginLoadData();
                                                        for (i = 0; i < dr.Length; i++)
                                                            sTable.Rows.Add(dr[i].ItemArray);
                                                        sTable.EndLoadData();

                                                        articleDGV.DataSource = sTable;
                                                        articleDGV.Select();
                                                        allowToShow = true;
                                                    }

                                                }
                                                catch
                                                {
                                                    SrchTbox.Focus();
                                                    SrchTbox.SelectAll();
                                                }
                                                #endregion
                                                break;
                                            }
                                        case 2:
                                            {
                                                #region by bc
                                                try
                                                {
                                                    allowToShow = BCSearcher(SrchTbox.Text, true);
                                                }
                                                catch (FormatException)
                                                {
                                                    SrchTbox.Focus();
                                                    SrchTbox.SelectAll();
                                                }
                                                #endregion
                                                break;
                                            }
                                    }
                                }
                                #endregion

                                // show article panel if there are filtered productss
                                if (splitContainer1.Panel2Collapsed && allowToShow)
                                {
                                    вікноТоварівToolStripMenuItem.PerformClick();
                                    splitContainer1.Panel2.Tag = new object();
                                    articleDGV.Select();
                                    // skin sensor integration (hide panel when some arts are showing)
                                    if (this.сенсорToolStripMenuItem.Checked)
                                        this.chequeContainer.Panel2Collapsed = true;
                                }

                                articleDGV.Update();
                                //Debug.Write("EndAdd");
                            }

                            break;
                        } 
                        #endregion
                    case 0x16:
                        #region CONTROL + ENTER
                        {
                            if (inventChq || Cheque.Rows.Count == 0)
                                break;//r

                            if (!getAdminAccess(23, false))
                            {
                                //if (admin.ShowDialog() != DialogResult.OK)
                                MMessageBoxEx.Show(this.chequeDGV, "Закриття чеку заблоковано", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;//r
                            }

                            // if we use legal printer
                            if (Program.AppPlugins.IsActive(PluginType.LegalPrinterDriver))
                            {
                                // we close legal cheque
                                CloseCheque(true);
                            }
                            else
                            {
                                // if we don't use legal printer and
                                // if we allow to close normal cheque or admin mode is active
                                if (getAdminAccess(6, false))
                                {
                                    // we close normal cheque
                                    CloseCheque(false);
                                }
                                else
                                    MMessageBoxEx.Show(this.chequeDGV, "Не встановлено фіскальний принтер.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            break;
                        } 
                        #endregion
                    case 0x17:
                        #region CONTROL + SHIFT + ENTER
                        {
                            if (inventChq)
                                break;//r

                            if (Cheque.Rows.Count == 0 && getAdminAccess(12, false)) //  UserConfig.Properties[12]
                            {
                                string nextChqNom = string.Empty;
                                object[] localData = new object[0];
                                if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                                {
                                    string info = "";
                                    foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                                    {
                                        localData = DataWorkCheque.NonFxChqsInfo(0, ref nextChqNom, int.Parse(de.Key.ToString()));
                                        info += "| "+ string.Format("{3}: за {1} продано {0} чек(ів) на суму {2:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "} ", localData[0], localData[1], MathLib.GetDouble(localData[2].ToString()), ((Hashtable)de.Value)["NAME"]);
                                    }
                                    DDM_Status.Text = info;
                                }
                                else
                                {
                                    localData = DataWorkCheque.NonFxChqsInfo(0, ref nextChqNom);
                                    DDM_Status.Text = string.Format("За {1} продано {0} чек(ів) на суму {2:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", localData[0], localData[1], MathLib.GetDouble(localData[2].ToString()));
                                }
                                break;//r
                            }

                            if (Cheque.Rows.Count == 0)// || !Program.Service.UseEKKR)
                                break;//r

                            if (!getAdminAccess(6, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Закриття нефіксованого чеку заблоковано", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }

                            if (!getAdminAccess(23, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Функція закриття чеку заблоковано", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }

                            //if (!(ADMIN_STATE || (UserConfig.Properties[23] && UserConfig.Properties[6]) ))
                            //{
                            //    MMessageBoxEx.Show(this.chequeDGV, "Закриття чеку заблоковано", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //    break;
                            //}

                            if (ConfigManager.Instance.CommonConfiguration.Content_Common_PromptMsgOnIllegal &&
                                DialogResult.Yes != MMessageBoxEx.Show(this.chequeDGV, "Закрити чек без фіксації оплати",
                                Application.ProductName,
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1))
                                break;//r
                            /*
                            if (!(ADMIN_STATE || (UserConfig.Properties[23] && UserConfig.Properties[6])))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    break;//r
                            */
                            CloseCheque(false);
                            break;
                        }
                        #endregion
                    case 0x18:
                        #region F5
                        {
                            if (!ConfigManager.Instance.CommonConfiguration.APP_SrchTypesAccess[0])
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Пошук по назві не дозволений", Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }

                            if (currSrchType != 0)
                                SearchFilter(false, 0, true);
                            else
                            {
                                SrchTbox.Focus();
                                SrchTbox.Select(SrchTbox.Text.Length, 0);
                            }
                            break;
                        } 
                        #endregion
                    case 0x19:
                        #region F6
                        {
                            if (!ConfigManager.Instance.CommonConfiguration.APP_SrchTypesAccess[1])
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Пошук по коду не дозволений", Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }

                            if (currSrchType != 1)
                                SearchFilter(false, 1, true);
                            else
                            {
                                SrchTbox.Focus();
                                SrchTbox.Select(SrchTbox.Text.Length, 0);
                            }
                            break;
                        } 
                        #endregion
                    case 0x1A:
                        #region F7
                        {
                            //winapi.WinAPI.OutputDebugString("F7");
                            if (!ConfigManager.Instance.CommonConfiguration.APP_SrchTypesAccess[2])
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Пошук по штрих-коду не дозволений", Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }

                            if (currSrchType != 2)
                                SearchFilter(false, 2, true);
                            else
                            {
                                SrchTbox.Focus();
                                SrchTbox.Select(0, SrchTbox.Text.Length);
                            }
                            break;
                        } 
                        #endregion
                    case 0x1B:
                        #region F8
                        {
                            if (Cheque.ExtendedProperties.Contains("BILL"))
                                MMessageBoxEx.Show(this.chequeDGV, "Відкритий рахунок №" + " " + Cheque.ExtendedProperties["NOM"], Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        } 
                        #endregion
                    case 0x1C:
                        #region F9
                        {
                            string infoText = string.Empty;
                            UserSchema us = new UserSchema();
                            infoText += UserConfig.UserID;
                            infoText += "\r\n\r\n--------------------------------------------------\r\n\r\n";
                            for (int i = 0; i < UserSchema.ITEMS_COUNT; i++)
                                infoText += us.SchemaItems[i] + " : " + (UserConfig.Properties[i] ? "Так" : "Ні") + "\r\n";
                            infoText += "\r\n\r\n--------------------------------------------------\r\n\r\n";
                            infoText +=  "К-сть товарів: " + Articles.Rows.Count.ToString() + "\r\n";
                            MMessageBoxEx.Show(infoText, UserConfig.UserID);
                            break;
                        } 
                        #endregion
                    case 0x1D:
                        #region ESCAPE
                        {
                            if (this.currSrchType != ConfigManager.Instance.CommonConfiguration.APP_SearchType ||
                                this.SrchTbox.Text.Length != 0 ||
                                this.Articles.Rows.Count != this.articleDGV.RowCount)
                            {
                                SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                                //this.sensorDataPanel1.Navigator.DisplayedCategoryFilter = "";
                                //this.Navigator_OnFilterChanged("", EventArgs.Empty);
                            }
                            else
                                this.Close();
                            break;
                        } 
                        #endregion
                    case 0x1E:
                        #region CONTROL + Q
                        {
                            if (inventChq)
                                break;//r

                            nakladna = !nakladna;

                            if (nakladna)
                                CashLbl.Image = Properties.Resources.naklad;
                            else
                                CashLbl.Image = null;
                            break;
                        } 
                        #endregion
                    case 0x1F:
                        #region CONTROL
                        {
                            if (chequeDGV.CurrentCell != null && chequeDGV.CurrentCell.IsInEditMode)
                                chequeDGV.EndEdit();
                            break;
                        } 
                        #endregion
                }
                #endregion
                //winapi.Funcs.OutputDebugString("W");
            }

            if (m.Msg == (int)CoreLib.MyMsgs.WM_UPDATE)
            {
                if (_fl_canUpdate)
                    this.timer1_Tick(this.timer1, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Custom method. Used for updating data of elements.
        /// </summary>
        private void UpdateMyControls()
        {
            //winapi.Funcs.OutputDebugString("UpdateMyControls_begin");
            RefreshAppInformer();
            RefreshChequeInformer(true);
            RefershStyles();
            RefershMenus();
            RefreshWindowMenu();

            timer1.Interval = ConfigManager.Instance.CommonConfiguration.APP_RefreshRate;

            bool needUpdate = false;
            if (currentModeIsSingle != ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
            {
                currentModeIsSingle = ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles;
                _fl_onlyUpdate = false;
                needUpdate = true;
            }

            if (currentSubUnit != ConfigManager.Instance.CommonConfiguration.APP_SubUnit)
            {
                _fl_onlyUpdate = false;
                _fl_SubUnitChanged = true;
                needUpdate = true;
                currentSubUnit = ConfigManager.Instance.CommonConfiguration.APP_SubUnit;
            }

            if (needUpdate)
                timer1_Tick(timer1, EventArgs.Empty);
            //winapi.Funcs.OutputDebugString("UpdateMyControls_end");
        }
        #region InitCtrl SubMethods
        private void RefreshAppInformer()
        {
            appInfoLabel.Text = string.Format("{0}: {1}     {2}: \"{3}\"     {4}: {5}     {6}: \"{7}\"",
                "Підрозділ №",
                ConfigManager.Instance.CommonConfiguration.APP_SubUnit,
                "Назва підрозділу",
                ConfigManager.Instance.CommonConfiguration.APP_SubUnitName == string.Empty ? "без назви" : ConfigManager.Instance.CommonConfiguration.APP_SubUnitName,
                "Каса №",
                ConfigManager.Instance.CommonConfiguration.APP_PayDesk,
                "Касир",
                UserConfig.UserID);
        }//ok//label
        private void RefreshChequeInformer(bool resetDigitalPanel)
        {
            if (inventChq)
            {
                CashLbl.Text = string.Format("{0}", "ІНВЕНТАРИЗАЦІЯ"); ;
                chequeInfoLabel.Text = string.Format("{0}", Cheque.ExtendedProperties["Date"]);
            }
            else
            {
                string ctrlWord = "чеку";
                if (Cheque.ExtendedProperties["BILL"] != null)
                    ctrlWord = "рахунку";
                string totalWord = "позиці";
                int numValue = Cheque.Rows.Count;

                while (numValue > 20)
                    numValue %= 10;

                switch (numValue)
                {
                    case 1: totalWord += 'я'; break;
                    case 2: totalWord += 'ї'; break;
                    case 3: totalWord += 'ї'; break;
                    case 4: totalWord += 'ї'; break;
                    default: totalWord += 'й'; break;
                }
                
                if (retriveChq)
                    chequeInfoLabel.Text = string.Format("{0} {1} {2} {3} {4}", "В", ctrlWord, Cheque.Rows.Count, totalWord, "повертається на суму");
                else
                    chequeInfoLabel.Text = string.Format("{0} {1} {2} {3} {4}", "В", ctrlWord, Cheque.Rows.Count, totalWord, "продається на суму");
                CashLbl.Text = string.Format("{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", realSUMA);
                if (DataWorkShared.ExtractOrderProperty(this.Cheque, CoreConst.BILL, null, true) == null)
                    this.addBillInfo.Text = string.Empty;
            }

            if(resetDigitalPanel)
            {
                CashLbl.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_SumFontColor;
                CashLbl.Font = ConfigManager.Instance.CommonConfiguration.STYLE_SumFont;

                CashLbl.Image = null;
                digitalPanel.BackgroundImage = null;
                nakladna = false;
            }
        }
        private void RefershStyles()
        {
            //Colors
            infoPanel.BackColor = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundInfPan;
            addChequeInfo.BackColor = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundAddPan;
            digitalPanel.BackColor = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundSumRest;
            chequeDGV.BackgroundColor = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundNAChqTbl;
            chequeDGV.DefaultCellStyle.BackColor = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundNAChqTbl;
            articleDGV.BackgroundColor = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundArtTbl;
            articleDGV.DefaultCellStyle.BackColor = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundArtTbl;
            statusStrip1.BackColor = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundStatPan;

            //Fonts
            CashLbl.Font = ConfigManager.Instance.CommonConfiguration.STYLE_SumFont;
            CashLbl.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_SumFontColor;
            articleDGV.Font = ConfigManager.Instance.CommonConfiguration.STYLE_ArticlesFont;
            articleDGV.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_ArticlesFontColor;
            chequeDGV.Font = ConfigManager.Instance.CommonConfiguration.STYLE_ChequeFont;
            chequeDGV.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_ChequeFontColor;
            statusStrip1.Font = ConfigManager.Instance.CommonConfiguration.STYLE_StatusFont;
            statusStrip1.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_StatusFontColor;
            addChequeInfo.Font = ConfigManager.Instance.CommonConfiguration.STYLE_AddInformerFont;
            addChequeInfo.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_AddInformerFontColor;
            chequeInfoLabel.Font = ConfigManager.Instance.CommonConfiguration.STYLE_ChqInformerFont;
            chequeInfoLabel.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_ChqInformerFontColor;
            appInfoLabel.Font = ConfigManager.Instance.CommonConfiguration.STYLE_AppInformerFont;
            appInfoLabel.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_AppInformerFontColor;

            // misc
            this.articleDGV.RowTemplate.Height = ConfigManager.Instance.CommonConfiguration.STYLE_Misc_ArticleRowHeight;
            this.articleDGV.Invalidate();//
            this.articleDGV.Refresh();
            this.articleDGV.Update();
            //this.articleDGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            
            this.chequeDGV.RowTemplate.Height = ConfigManager.Instance.CommonConfiguration.STYLE_Misc_ChequeRowHeight;
            this.chequeDGV.Invalidate();
            this.chequeDGV.Refresh();
            this.chequeDGV.Update();
            //this.chequeDGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

        }//ok
        private void RefershMenus()
        {
            if (Program.AppPlugins.IsActive(PluginType.LegalPrinterDriver))
                try
                {
                    fxFunc_toolStripMenuItem.Enabled = Program.AppPlugins.GetActive<ILegalPrinterDriver>().AllowedMethods.Count != 0;
                }
                catch { }
            else
                fxFunc_toolStripMenuItem.Enabled = false;
            адміністраторToolStripMenuItem.Checked = ADMIN_STATE;
            фільтрОдиницьToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0;
            формуванняЧекуToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0 && ADMIN_STATE;
            інвентаризаціяToolStripMenuItem.Enabled = (inventChq || Cheque.Rows.Count == 0) && ADMIN_STATE;
            чекПоверненняToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0;
            налаштуванняToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0 && ADMIN_STATE;
            параметриДрукуToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0 && ADMIN_STATE;
            змінитиКористувачаToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0;
            вихідToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count == 0;

            //друкуватиРахунокToolStripMenuItem.Enabled = Cheque.ExtendedProperties.Contains("BILL");
            bool isLocked = (bool)DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.IS_LOCKED, false);
            bool isBill = DataWorkShared.ExtractOrderProperty(this.Cheque, CoreConst.BILL, null, true) != null;
            анулюватиРахунокToolStripMenuItem.Enabled = isBill && !isLocked;
            зберегтиРахунокToolStripMenuItem.Enabled = Cheque.Rows.Count != 0 && !inventChq && !isLocked;
            всіРахункиToolStripMenuItem.Enabled = !inventChq;
            зберегтиІЗакритиToolStripMenuItem.Enabled = Cheque.Rows.Count != 0 && !inventChq;
            зберегтиІДрукуватиToolStripMenuItem.Enabled = Cheque.Rows.Count != 0 && !inventChq;// && !isLocked;
            ToolStripMenu_Bills_SavePrintAndClose.Enabled = Cheque.Rows.Count != 0 && !inventChq;// && !isLocked;
            закритиБезЗмінToolStripMenuItem.Enabled = isBill;
            перезавантажитиРахунокToolStripMenuItem.Enabled = isBill;
            змінитиКоментарToolStripMenuItem.Enabled = isBill;
            
            змінитиКстьТоваруToolStripMenuItem.Enabled = Cheque.Rows.Count != 0;
            видалитиВибранийТоварToolStripMenuItem.Enabled = Cheque.Rows.Count != 0;
            видалитиВсіТовариToolStripMenuItem.Enabled = Cheque.Rows.Count != 0;
            здійснитиОплатуToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count != 0;
            задатиЗнижкаToolStripMenuItem.Enabled = !inventChq && Cheque.Rows.Count != 0;
            задатиНадбавкуToolStripMenuItem1.Enabled = !inventChq && Cheque.Rows.Count != 0;

        }//ok
        private void RefreshWindowMenu()
        {
            вертикальноToolStripMenuItem.Checked = (splitContainer1.Orientation == Orientation.Vertical);
            вікноТоварівToolStripMenuItem.Checked = !splitContainer1.Panel2Collapsed;
        }
        private void RefreshComponents(bool force)
        {
            if (this.сенсорToolStripMenuItem.Checked || force)
            {
                Com_WinApi.OutputDebugString("RefershStyles_Sensor_Activated");

                this.sensorPanel1.SensorType = ConfigManager.Instance.CommonConfiguration.skin_sensor_com_size_cheque;


                // cheque
                переміщенняПоЧекуToolStripMenuItem1.Checked = this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Scrolling, ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqnav);
                операціїЧекуToolStripMenuItem1.Checked = this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Operations, ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqopr);
                режимиПошукуToolStripMenuItem1.Checked = this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Search, ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqsrch);
                рахункиToolStripMenuItem1.Checked = this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Additional, ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqbills);
                
                // art
                навігаціяТоварівToolStripMenuItem.Checked = ConfigManager.Instance.CommonConfiguration.skin_sensor_com_artnav;
                переміщенняПоТоварахToolStripMenuItem.Checked = this.sensorDataPanel1.Scroller.Visible = ConfigManager.Instance.CommonConfiguration.skin_sensor_com_artscroll;

                // splitters
                this.sensorDataPanel1.Container.Panel1Collapsed = !ConfigManager.Instance.CommonConfiguration.skin_sensor_com_artnav;
                this.sensorDataPanel1.Container.SplitterDistance = ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_artnav;
                this.sensorDataPanel1.NavigatorFont = ConfigManager.Instance.CommonConfiguration.skin_sensor_fontsize;
                this.chequeContainer.Orientation = (Orientation)ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_orient;
                //this.splitContainer_chequeControlContainer.SplitterDistance = ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chqcontrols;
                try
                {

                    switch (this.sensorPanel1.SensorType)
                    {
                        case 50:
                            {

                                this.chequeContainer.SplitterDistance = ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_50;
                                this.sensorPanel1.SetSplitterDistance("h_50", ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_h_50);
                                this.sensorPanel1.SetSplitterDistance("v_50", ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_v_50);
                                break;
                            }
                        default:
                        case 100:
                            {
                                this.chequeContainer.SplitterDistance = ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_100;
                                this.sensorPanel1.SetSplitterDistance("h_100", ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_h_100);
                                this.sensorPanel1.SetSplitterDistance("v_100", ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_v_100);
                                break;
                            }
                    }

                    ;//this.splitContainer_chequeControls.SplitterDistance = ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chqmain;
                }
                catch { }
            }


        }
        #endregion

        /// <summary>
        /// Обробляє виконання операцій відповідно до того, який пункт меню був вибраний
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (_fl_menuIsActive || e.ClickedItem.Tag == null) 
                return;

            _fl_menuIsActive = true;
            switch (e.ClickedItem.Tag.ToString())
            {
                #region Main
                case "fiscalFunctions":
                    {
                        uiWndFiscalFunctions ff = new uiWndFiscalFunctions(Program.AppPlugins.GetActive<ILegalPrinterDriver>().Name, Program.AppPlugins.GetActive<ILegalPrinterDriver>().AllowedMethods);
                        try
                        {
                            if (ff.ShowDialog(this) == DialogResult.Yes)
                                Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction(ff.Function);
                        }
                        catch (Exception ex)
                        {
                            MMessageBoxEx.Show(this.chequeDGV, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        ff.Dispose();
                        break;
                    }
                case "LastDBChanges":
                    {
                        this.timer1_Tick(this.timer1, EventArgs.Empty);
                        /*uiWndBaseChanges DBChanges = new uiWndBaseChanges();
                        if (DBChanges.ShowDialog() == DialogResult.OK)
                            timer1_Tick(timer1, EventArgs.Empty);
                        DBChanges.Dispose();
                        */
                        break;
                    }
                case "Administrator":
                    {
                        DialogResult rez = DialogResult.None;
                        if (ADMIN_STATE)
                            rez = MMessageBoxEx.Show(this.chequeDGV, "Вийти з режиму адміністратора", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        else
                            rez = admin.ShowDialog();

                        switch (rez)
                        {
                            case DialogResult.OK:
                                ADMIN_STATE = true;
                                break;
                            case DialogResult.Yes:
                                ADMIN_STATE = false;
                                break;
                        }

                        RefershMenus();
                        break;
                    }
                case "UnitFilter":
                    {
                        if (!getAdminAccess(9))
                            break;

                        uiWndUnitFilter fl = new uiWndUnitFilter(Articles);
                        fl.ShowDialog();
                        fl.Dispose();
                        break;
                    }
                case "ChequeFormat":
                    {
                        uiWndDiscountSettings billRul = new uiWndDiscountSettings();
                        billRul.ShowDialog();
                        billRul.Dispose();
                        ResetDiscount();
                        UpdateSumInfo(true);
                        break;
                    }
                case "Invent":
                    {
                        if (Cheque.Rows.Count != 0 && !inventChq)
                            return;
                        inventChq = !inventChq;
                        if (inventChq)
                        {
                            DataTable dTable=new DataTable();
                            DataSet dSet = new DataSet();
                            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                            {
                                dSet = DataWorkCheque.OpenInvent(this.Cheques);
                                this.Cheques.Tables.Clear();
                                foreach (DataTable dt in dSet.Tables)
                                {
                                    this.Cheques.Tables.Add(dt.Copy());
                                    dTable.Merge(dt.Copy());
                                }
                            }
                            else
                                dTable = DataWorkCheque.OpenInvent();


                            if (dTable != null)
                            {
                                dTable.ExtendedProperties.Add("loading", true);
                                Cheque.Merge(dTable);
                                Cheque.ExtendedProperties.Remove("loading");
                                dTable.ExtendedProperties.Remove("loading");
                            }
                            else
                                inventChq = false;
                        }
                        else
                        {
                            DataWorkCheque.SaveInvent(Cheque, false, this.Cheques);
                            //Cheque.Rows.Clear();
                            //UpdateSumDisplay(true, true);
                            RowsRemoved_MyEvent(true, true, true);
                        }

                        RefershMenus();
                        RefreshChequeInformer(true);
                        інвентаризаціяToolStripMenuItem.Checked = inventChq;
                        break;
                    }
                case "RetriveCheque":
                    {
                        if (!getAdminAccess(5))
                            break;

                        retriveChq = !retriveChq;
                        чекПоверненняToolStripMenuItem.Checked = retriveChq;
                        RefreshChequeInformer(true);
                        break;
                    }
                case "Settings":
                    {
                        uiWndSettings set = new uiWndSettings();
                        if (set.ShowDialog() == DialogResult.OK)
                        {
                            RefreshComponents(false);
                            UpdateMyControls();
                            SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                            this.Text = string.Format("{2}{1}{0}", Application.ProductName, " - ", ConfigManager.Instance.CommonConfiguration.APP_SubUnitName);
                        }
                        set.Dispose();
                        break;
                    }
                case "PrintingSettings":
                    {
                        uiWndPrinting pSet = new uiWndPrinting();
                        pSet.ShowDialog();
                        pSet.Dispose();
                        break;
                    }
                case "Registration":
                    {
                        uiWndRegistration rf = new uiWndRegistration();
                        if (rf.ShowDialog() == DialogResult.OK)
                            MMessageBoxEx.Show(this.chequeDGV, "Перезавантажте програму для продовження реєстрації", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        rf.Dispose();
                        break;
                    }
                case "AboutApp":
                    {
                        uiWndAboutBox abox = new uiWndAboutBox();
                        abox.ShowDialog();
                        abox.Dispose();
                        break;
                    }
                case "ChangeCashier":
                    {
                        DialogResult = DialogResult.Retry;
                        Close();
                        break;
                    }
                case "Exit":
                    {
                        DialogResult = DialogResult.Cancel;
                        Close();
                        break;
                    }
                #endregion
                #region TablesView
                case "Vertical":
                    {
                        if (!((ToolStripMenuItem)e.ClickedItem).Checked)
                        {
                            splitContainer1.Orientation = Orientation.Vertical;
                            ConfigManager.Instance.CommonConfiguration.STYLE_SplitOrient = Orientation.Vertical;
                            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
                        }
                        else
                        {
                            splitContainer1.Orientation = Orientation.Horizontal;
                            ConfigManager.Instance.CommonConfiguration.STYLE_SplitOrient = Orientation.Horizontal;
                            splitContainer1.SplitterDistance = splitContainer1.Height / 2;
                        }
                        RefreshWindowMenu();
                        break;
                    }
                case "ArticleWindow":
                    {
                        splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
                        ConfigManager.Instance.CommonConfiguration.STYLE_ArtSideCollapsed = !ConfigManager.Instance.CommonConfiguration.STYLE_ArtSideCollapsed;
                        RefreshWindowMenu();
                        break;
                    }
                case "SensorType":
                    {
                        if (!((ToolStripMenuItem)e.ClickedItem).Checked)
                        {/*
                            if (splitContainer1.Orientation != Orientation.Vertical)
                            {
                                splitContainer1.Tag = "R";
                            }*/
                            //splitContainer1.Orientation = Orientation.Vertical;
                            //AppConfig.STYLE_SplitOrient = Orientation.Vertical;
                            //RefreshWindowMenu();
                            //splitContainer1.SplitterDistance = splitContainer1.Width / 2;

                            this.sensorDataPanel1.Navigator.SetAndShowNavigator(ApplicationConfiguration.Instance.GetValueByKey<Hashtable>("productFiltering"));


                            this.sensorDataPanel1.Visible = true;
                            this.chequeContainer.Panel2Collapsed = false;

                            /*
                            if (this.sensorPanel1.SensorType == 50 && ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_50 < this.chequeContainer.Height && this.chequeContainer.Height * 2 / 3 > ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_50)
                                this.chequeContainer.SplitterDistance = ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_50;
                            else if (this.sensorPanel1.SensorType == 100 && ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_100 < this.chequeContainer.Height && this.chequeContainer.Height * 2 / 3 > ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_100)
                                this.chequeContainer.SplitterDistance = ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_100;
                            else
                                this.chequeContainer.SplitterDistance = this.chequeContainer.Height * 2 / 3;

                            */
                            this.articleDGV.Parent = this.sensorDataPanel1.Placeholder;
                            //this.articleDGV.BringToFront();

                            RefreshComponents(true);

                            //-Sensor_EventHandler(null);

                            //this.TopMost = true;
                            //System.Diagnostics.Process.Start("vk.exe");

                            //enabling dependence menu
                            управліннToolStripMenuItem.Enabled = true; 
                        }
                        else
                        {/*
                            if (splitContainer1.Tag != null && splitContainer1.Tag.ToString() == "R")
                            {
                                splitContainer1.Orientation = Orientation.Horizontal;
                                ConfigManager.Instance.CommonConfiguration.STYLE_SplitOrient = Orientation.Horizontal;
                                RefreshWindowMenu();
                                splitContainer1.SplitterDistance = splitContainer1.Height / 2;
                            }*/

                            this.sensorDataPanel1.Visible = false;
                            this.chequeContainer.Panel2Collapsed = true;
                            
                            this.articleDGV.Parent = this.splitContainer1.Panel2;

                            //this.TopMost = false;
                            управліннToolStripMenuItem.Enabled = false; 
                        }
                        break;
                    }
                case "SensorType_Components_ChqNav":
                    {
                        this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Scrolling, !((ToolStripMenuItem)e.ClickedItem).Checked);
                        break;
                    }
                case "SensorType_Components_ChqOpr":
                    {
                        this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Operations, !((ToolStripMenuItem)e.ClickedItem).Checked);
                        break;
                    }
                case "SensorType_Components_ChqSrch":
                    {
                        this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Search, !((ToolStripMenuItem)e.ClickedItem).Checked);
                        break;
                    }
                case "SensorType_Components_ChqBills":
                    {
                        this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Additional, !((ToolStripMenuItem)e.ClickedItem).Checked);
                        break;
                    }
                case "SensorType_VK":
                    {
                        if (сенсорToolStripMenuItem.Checked)
                        {
                            ProcessStartInfo _vkProc = new ProcessStartInfo();
                            _vkProc.FileName = Application.StartupPath + "//tools//VirtualKeyboard//VirtualKeyboard.bat";
                            _vkProc.WorkingDirectory = Application.StartupPath + "//tools//VirtualKeyboard//";
                            System.Diagnostics.Process.Start(_vkProc);
                        }
                        break;
                    }
                case "SensorType_Components_ChqIsVertical":
                    {
                        if (!((ToolStripMenuItem)e.ClickedItem).Checked)
                            this.chequeContainer.Orientation = Orientation.Vertical;
                        else
                            this.chequeContainer.Orientation = Orientation.Horizontal;
                        break;
                    }
                case "SensorType_Components_ArtNav":
                    {
                        this.sensorDataPanel1.Container.Panel1Collapsed = ((ToolStripMenuItem)e.ClickedItem).Checked;
                        //tableLayoutPanel1.Visible = !((ToolStripMenuItem)e.ClickedItem).Checked;
                        //sensor_breadcrumb_container.Visible = !((ToolStripMenuItem)e.ClickedItem).Checked;
                        break;
                    }
                case "SensorType_Components_ArtScroll":
                    {
                        this.sensorDataPanel1.Scroller.Visible = !((ToolStripMenuItem)e.ClickedItem).Checked;
                        break;
                    }
                #endregion
                #region Bills
                case "BillManager": // lock bill forever
                    {
                        if (!getAdminAccess(-1))
                            break;

                        uiWndBillManagercs bm = new uiWndBillManagercs();
                        bm.ShowDialog(this);
                        bm.Dispose();
                        break;
                    }
                case "ResetBill": // lock bill forever
                    {
                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.PD_Order);
                        switch (changeState)
                        {
                            case 1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Були внесені змінити в поточний рахунок\r\nПерезавантажте рахунок за допомогою меню або натисніть ALT+R", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                            case 2:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок вже закритий\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                            case -1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок видалений з бази\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                        }
                        if (changeState != 0)
                            break;

                        if ((bool)DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.IS_LOCKED, false))
                        {
                            MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;//r
                        }
                        /*
                        if (new uiWndAdmin().ShowDialog(this) != DialogResult.OK)
                            break;
                        */

                        if (DialogResult.Yes != MMessageBoxEx.Show(this.chequeDGV, "Анулювати поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO),
                            Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            break;
                        //CoreLib.LockBill(Cheque, "null");
                        string billNo = DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.BILL_NO, string.Empty, false).ToString();
                       DataWorkBill.LockBill(this.PD_Order, "null");
                        RowsRemoved_MyEvent(true, true, true);
                        this.RefershMenus();
                        //this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", billNo);
                        break;
                    }
                case "SaveAndPrintAndClose": // save, print and close
                    {
                        if (inventChq)
                            break;

                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.PD_Order);
                        switch (changeState)
                        {
                            case 1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Були внесені змінити в поточний рахунок\r\nПерезавантажте рахунок за допомогою меню або натисніть ALT+R", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                            case 2:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок вже закритий\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                            case -1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок видалений з бази\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                        }
                        if (changeState != 0)
                            break;

                        uiWndBillSave bs = new uiWndBillSave(this.PD_Order);
                        bs.ShowDialog();

                        if (bs.DialogResult == DialogResult.OK)
                        {
                            //if (bs.IsNewBill)
                            //this.addBillInfo.Text = string.Format("{0} {1}", "ЗБЕРЕЖЕНИЙ РАХ.№", bs.GetNewBillNo);
                            //else
                            //    this.addBillInfo.Text = "";
                            uiWndBillPrint bPrn = new uiWndBillPrint(bs.SavedBill);
                            if (bPrn.ShowDialog(this) == DialogResult.OK)
                            {
                                this.RefershMenus();
                            }
                            bPrn.Dispose();
                            addChequeInfo.Text = string.Empty;
                            RowsRemoved_MyEvent(true, true, true);
                            //this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", bs.GetNewBillNo);
                        }
                        bs.Dispose();
                        break;
                    }
                case "SaveAndPrint": // save, print but leave opened
                    {
                        if (inventChq)
                            break;

                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.PD_Order);
                        switch (changeState)
                        {
                            case 1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Були внесені змінити в поточний рахунок\r\nПерезавантажте рахунок за допомогою меню або натисніть ALT+R", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                            case 2:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок вже закритий\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                            case -1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок видалений з бази\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                        }
                        if (changeState != 0)
                            break;

                        uiWndBillSave bs = new uiWndBillSave(this.PD_Order);
                        if (bs.ShowDialog() == DialogResult.OK)
                        {
                            this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", bs.GetNewBillNo);
                            uiWndBillPrint bPrn = new uiWndBillPrint(bs.SavedBill);
                            if (bPrn.ShowDialog(this) == DialogResult.OK)
                            {
                                this.RefershMenus();
                            }
                            bPrn.Dispose();
                            //DataWorkShared.MergeDataTableProperties(ref this.Cheque, bs.SavedBill);
                            Cheque.Merge(bs.SavedBill);
                            this.RefershMenus();
                        }
                        bs.Dispose();
                        break;
                    }
                case "SaveAndClose": // save and close
                    {
                        if (inventChq)
                            break;

                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.PD_Order);
                        switch (changeState)
                        {
                            case 1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Були внесені змінити в поточний рахунок\r\nПерезавантажте рахунок за допомогою меню або натисніть ALT+R", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                            case 2:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок вже закритий\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                            case -1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок видалений з бази\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                        }
                        if (changeState != 0)
                            break;


                        /*for (int i = 0; i < 200; i++)
                        {
                            DataTable _testDT = this.PD_Order.Copy();
                            DataWorkBill.SaveBill(true, i.ToString().PadLeft(4, '0'), "DEMO TEST", ref _testDT);
                        }*/
                        uiWndBillSave bs = new uiWndBillSave(this.PD_Order);
                        if (bs.ShowDialog() == DialogResult.OK)
                        {
                            addChequeInfo.Text = string.Empty;
                            RowsRemoved_MyEvent(true, true, true);
                            // if (bs.IsNewBill)
                            // ------ this.addBillInfo.Text = string.Format("{0} {1}", "ЗБЕРЕЖЕНИЙ РАХ.№", bs.GetNewBillNo);
                            //else
                            //this.addBillInfo.Text = "";
                        }
                        bs.Dispose();
                        break;
                    }
                case "AllBills": // show bill manager
                    {
                        string currentBillNumber = DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO).ToString();
                        /*
                            if (this.Cheque.ExtendedProperties["NOM"] != null)
                                currentBillNumber = this.Cheque.ExtendedProperties["NOM"].ToString();
                        */
                        //if ()
                        uiWndBillList bl = new uiWndBillList(currentBillNumber);
                        if (bl.ShowDialog() == DialogResult.OK)
                            if (Cheque.Rows.Count == 0)
                            {
                                Cheque.Merge(bl.LoadedBill);
                                this.UpdateDiscountValues(this.Cheque);
                                this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO));
                                UpdateSumInfo(true);
                            }
                            else MMessageBoxEx.Show(this.chequeDGV, "Неможливо відкрити рахунок\nТаблиця чеку не є порожня", Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                        bl.Dispose();
                        break;
                    }
                case "PrintBill": // removed
                    {
                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.PD_Order);
                        switch (changeState)
                        {
                            case 1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Були внесені змінити в поточний рахунок\r\nПерезавантажте рахунок за допомогою меню або натисніть ALT+R", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                            case 2:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок вже закритий\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                            case -1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок видалений з бази\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                        }
                        if (changeState != 0)
                            break;

                        uiWndBillPrint bPrn = new uiWndBillPrint(this.PD_Order);
                        bPrn.ShowDialog(this);
                        bPrn.Dispose();
                        break;
                    }
                case "SaveChangeComment":
                    {
                        if (inventChq)
                            break;

                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.PD_Order);
                        switch (changeState)
                        {
                            case 1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Були внесені змінити в поточний рахунок\r\nПерезавантажте рахунок за допомогою меню або натисніть ALT+R", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                            case 2:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок вже закритий\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                            case -1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок видалений з бази\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                        }
                        if (changeState != 0)
                            break;


                        uiWndBillSave bs = new uiWndBillSave(this.PD_Order);
                        bs.UpdateComment = true;
                        if (bs.ShowDialog() == DialogResult.OK)
                        {
                            //if (bs.IsNewBill)
                            this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", bs.GetNewBillNo);
                            //this.PD_Order = bs.SavedBill;
                            // else
                            //    this.addBillInfo.Text = "";

                            DataWorkShared.MergeDataTableProperties(ref this.Cheque, bs.SavedBill);

                            this.RefershMenus();

                        }
                        bs.Dispose();
                        break;
                    }
                case "SaveBill": // just save and leave opened
                    {
                        if (inventChq)
                            break;

                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.PD_Order);
                        switch (changeState)
                        {
                            case 1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Були внесені змінити в поточний рахунок\r\nПерезавантажте рахунок за допомогою меню або натисніть ALT+R", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                            case 2:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок вже закритий\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                            case -1:
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок видалений з бази\r\nНатисніть ОК для продовження роботи", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    _fl_menuIsActive = false;
                                    this.Menu_ItemClicked(this.billMenu, new ToolStripItemClickedEventArgs(закритиБезЗмінToolStripMenuItem));
                                    break;
                                }
                        }
                        if (changeState != 0)
                            break;


                        uiWndBillSave bs = new uiWndBillSave(this.PD_Order);
                        if (bs.ShowDialog() == DialogResult.OK)
                        {
                            //if (bs.IsNewBill)
                            this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", bs.GetNewBillNo);
                            //this.PD_Order = bs.SavedBill;
                            // else
                            //    this.addBillInfo.Text = "";

                            DataWorkShared.MergeDataTableProperties(ref this.Cheque, bs.SavedBill);

                            this.RefershMenus();

                        }
                        bs.Dispose();
                        break;
                    }
                case "ReloadBill": // ReloadBill
                    {
                        object billName = DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.PATH);
                        DataTable LoadedBill = DataWorkBill.LoadCombinedBill(ConfigManager.Instance.CommonConfiguration.Path_Bills + "\\" + billName.ToString());
                        RowsRemoved_MyEvent(true, true, true);
                        Cheque.Merge(LoadedBill);
                        this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", DataWorkShared.ExtractBillProperty(this.Cheque, CoreConst.BILL_NO));
                        UpdateSumInfo(true);
                        break;
                    }
                case "CloseBillWithoutChanges": // CloseBill
                    {
                        addChequeInfo.Text = string.Empty;
                        RowsRemoved_MyEvent(true, true, true);
                        break;
                    }
                #endregion
                #region Plugins
                case "SendComPortCommand":
                    {
                        wndAdditional.uiWndAdditionalPortCommands cm = new wndAdditional.uiWndAdditionalPortCommands();
                        cm.ShowDialog();
                        cm.Dispose();
                        //MessageBox.Show(cm.PortCommand);
                        break;
                    }
                #endregion
            }

            _fl_menuIsActive = false;
        }
        private void Context_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null)
                return;

            switch (((ContextMenuStrip)sender).Tag.ToString())
            {
                case "ChequeTableContext": chequeDGV.Select(); break;
                case "ArticleTableContext": articleDGV.Select(); break;
                case "ColumnsTableContext": break;
            }

            switch (e.ClickedItem.Tag.ToString())
            {
                #region ChequeContextMenu
                case "ChangeDose":
                    {
                        // Send ENTER Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_Enter), new IntPtr(0));
                        break;
                    }
                case "DeleteArticle":
                    {
                        // Send CTRL+ENTER Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_CtrlDel), new IntPtr(0));
                        break;
                    }
                case "DeleteAllArticles":
                    {
                        // Send CTRL+SHIFT+DEL Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_CtrlShiftDel), new IntPtr(0));
                        break;
                    }
                case "Payment":
                    {
                        // Send CTRL+ENTER Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_CtrlEnter), new IntPtr(0));
                        break;
                    }
                case "SetDiscount":
                    {
                        // Send CTRL+PgDn Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_CtrlPgDn), new IntPtr(0));
                        break;
                    }
                case "SetUpcount":
                    {
                        // Send CTRL+PgUp Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_CtrlPgUp), new IntPtr(0));
                        break;
                    }
                case "CancelDiscount":
                    {
                        // Send ENTER Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_ShiftDel), new IntPtr(0));
                        break;
                    }
                #endregion
                #region ArticleContextMenu
                case "SelectArticle":
                    {
                        // Send ENTER Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_Enter), new IntPtr(0));
                        break;
                    }
                case "SearchByName":
                    {
                        // Send F5 Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_F5), new IntPtr(0));
                        break;
                    }
                case "SearchByCode":
                    {
                        // Send F6 Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_F6), new IntPtr(0));
                        break;
                    }
                case "SearchByBarCode":
                    {
                        // Send F7 Key
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_F7), new IntPtr(0));
                        break;
                    } 
                #endregion
                #region FieldContext
                case "FieldEditor":
                    {
                        uiWndColumnsEditorBox colEd = null;

                        if (chequeDGV.NewRowIndex == -1 && chequeDGV.Focused)
                            colEd = new uiWndColumnsEditorBox(ref chequeDGV, 1);
                        if (articleDGV.NewRowIndex == -1 && articleDGV.Focused)
                            colEd = new uiWndColumnsEditorBox(ref articleDGV, 2);
                        
                        colEd.ShowDialog();
                        colEd.Dispose();

                        ViewLib.SaveGridsView(new DataGridView[] { chequeDGV, articleDGV }, splitContainer1.Orientation);
                        break;
                    }
                case "FieldLock":
                    {
                        закріпитиToolStripMenuItem.Checked = !закріпитиToolStripMenuItem.Checked;

                        if (chequeDGV.Focused)
                        {
                            chequeDGV.AllowUserToOrderColumns = !закріпитиToolStripMenuItem.Checked;
                            chequeDGV.AllowUserToResizeColumns = !закріпитиToolStripMenuItem.Checked;
                            ConfigManager.Instance.CommonConfiguration.STYLE_ChqColumnLock = закріпитиToolStripMenuItem.Checked;
                            break;
                        }
                        if (articleDGV.Focused)
                        {
                            articleDGV.AllowUserToOrderColumns = !закріпитиToolStripMenuItem.Checked;
                            articleDGV.AllowUserToResizeColumns = !закріпитиToolStripMenuItem.Checked;
                            ConfigManager.Instance.CommonConfiguration.STYLE_ArtColumnLock = закріпитиToolStripMenuItem.Checked;
                        }
                        break;
                    }
                case "SaveFieldPositions":
                    {
                        SaveGUI();
                        ViewLib.SaveGridsView(new DataGridView[] { chequeDGV, articleDGV }, splitContainer1.Orientation);
                        break;
                    } 
                #endregion
            }
        }

        #region Table Events
        //CHEQUE DataGridView
        /// <summary>
        /// Invoke when added row into order table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chequeDGV_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (Cheque.ExtendedProperties.Contains("loading"))
                return;

            if (Cheque.Rows.Count % ConfigManager.Instance.CommonConfiguration.APP_InvAutoSave == 0 && inventChq)
                DataWorkCheque.SaveInvent(Cheque, true, this.Cheques);

            if (chequeDGV.Rows.Count == 1)
                RefershMenus();

            if (!inventChq)
                RefreshChequeInformer(Cheque.Rows.Count == 1);
        }//ok
        /// <summary>
        /// Invoke when row begin editing 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chequeDGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //winapi.Funcs.OutputDebugString("1");
            chequeDGV.Rows[e.RowIndex].Selected = true;
            //winapi.Funcs.OutputDebugString("2");
        }
        /// <summary>
        /// Invoke when vslue in data row was edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chequeDGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //winapi.WinAPI.OutputDebugString("3");
            double addedTot = MathLib.GetDouble(chequeDGV["TOT", e.RowIndex].Value.ToString());
            addedTot = MathLib.GetRoundedDose(addedTot);

            if (addedTot <= 0 && !this.сенсорToolStripMenuItem.Checked)
            {
                MMessageBoxEx.Show(this.chequeDGV, "Помилкове значення кількості", Application.ProductName);
                chequeDGV.BeginEdit(true);
            }
            else
            {
                double activeTot = MathLib.GetDouble(chequeDGV["TMPTOT", e.RowIndex].Value);
                double thisTot = addedTot + activeTot;
                if (thisTot <= 0)
                {
                    if (activeTot < 1 && activeTot > 0) thisTot = activeTot;
                    else thisTot = 1.0;
                }
                thisTot = MathLib.GetRoundedDose(thisTot);
                double price = MathLib.GetDouble(chequeDGV["PRICE", e.RowIndex].Value.ToString());
                if (UserConfig.Properties[8])
                {
                    DataRow dRow = Cheque.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                    price = CoreLib.AutomaticPrice(thisTot, dRow);
                }
                double sum = MathLib.GetRoundedMoney(thisTot * price);

                chequeDGV["TOT", e.RowIndex].Value = thisTot;
                chequeDGV["TMPTOT", e.RowIndex].Value = thisTot;
                chequeDGV["PRICE", e.RowIndex].Value = price;
                chequeDGV["SUM", e.RowIndex].Value = sum;
                chequeDGV["ASUM", e.RowIndex].Value = sum;
                chequeDGV.Update();

                UpdateSumInfo(true);
                SrchTbox.Select();
                SrchTbox.SelectAll();

                try
                {
                    DataRow[] dr = Articles.Select("ID like '" + chequeDGV.CurrentRow.Cells["TID"].Value + "'");
                    if (dr != null && dr.Length != 0 && dr[0] != null)
                    {
                        thisTot = MathLib.GetDouble(chequeDGV["TQ", e.RowIndex].Value);
                        if (thisTot != 0)
                            addedTot *= MathLib.GetDouble(chequeDGV["TQ", e.RowIndex].Value);
                        CoreLib.AddArticleToCheque(chequeDGV, articleDGV, dr[0], addedTot, Articles);
                    }
                }
                catch { }
            }
            //winapi.Funcs.OutputDebugString("4");
        }//OK

        //COMMON EVENTS OF TABLES
        private void DGV_MouseClick(object sender, MouseEventArgs e)
        {
            (sender as DataGridView).Select();

            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo info = chequeDGV.HitTest(e.X, e.Y);

                if (info.ColumnIndex > 0 && info.RowIndex < 0)
                {
                    if ((sender as DataGridView).Name == "chequeDGV")
                        закріпитиToolStripMenuItem.Checked = ConfigManager.Instance.CommonConfiguration.STYLE_ChqColumnLock;
                    else
                        закріпитиToolStripMenuItem.Checked = ConfigManager.Instance.CommonConfiguration.STYLE_ArtColumnLock;
                    columnsEditor.Show(Control.MousePosition);
                    return;
                }
                if ((sender as DataGridView).Name == "chequeDGV")
                    chequeContextMenu.Show(Control.MousePosition);
                else
                    articleContextMenu.Show(Control.MousePosition);
            }
        }
        private void DGV_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            (sender as DataGridView).Select();
            /*
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex < 0)
                {
                    if ((sender as DataGridView).Name == "chequeDGV")
                        закріпитиToolStripMenuItem.Checked = ConfigManager.Instance.CommonConfiguration.STYLE_ChqColumnLock;
                    else
                        закріпитиToolStripMenuItem.Checked = ConfigManager.Instance.CommonConfiguration.STYLE_ArtColumnLock;
                    columnsEditor.Show(Control.MousePosition);
                    return;
                }
                if ((sender as DataGridView).Name == "chequeDGV")
                    chequeContextMenu.Show(Control.MousePosition);
                else
                    articleContextMenu.Show(Control.MousePosition);
            }
            */
            if (e.RowIndex < 0)
                return;

            (sender as DataGridView).CurrentCell = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
            DDM_Status.Text = CoreLib.ShowArticleInfo(chequeDGV, articleDGV);

            // seonsor command
            if (this.сенсорToolStripMenuItem.Checked && (sender as DataGridView).Name == "articleDGV")
                Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_Enter), new IntPtr(0));

        }
        private void DGV_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            (sender as DataGridView).Select();

            if (e.RowIndex < 0)
                return;

            (sender as DataGridView).CurrentCell = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
            DDM_Status.Text = CoreLib.ShowArticleInfo(chequeDGV, articleDGV);

            // Send ENTER Key
            Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_Enter), new IntPtr(0));
        }
        private void DGV_Enter(object sender, EventArgs e)
        {
            if ((sender as DataGridView).Name == "chequeDGV")
                chequeDGV.BackgroundColor = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundAChqTbl;

            (sender as DataGridView).DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.Highlight);
        }//ok
        private void DGV_Leave(object sender, EventArgs e)
        {
            if ((sender as DataGridView).Name == "chequeDGV")
                chequeDGV.BackgroundColor = ConfigManager.Instance.CommonConfiguration.STYLE_BackgroundNAChqTbl;
            (sender as DataGridView).DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.InactiveCaption);
        }//ok
        #endregion

        /// <summary>
        /// Timer's event indicate when timer is up and perform application data updating
        /// </summary>
        /// <param name="sender">Timer object</param>
        /// <param name="e">Timer event arguments</param>
        private void timer1_Tick(object sender, EventArgs e)//lbl
        {
            timer1.Stop();
            this.Update();

            // check for opened oreder or unlocked folder
            if (Cheque.Rows.Count != 0)
            {
                // so we want to come back here immediately once order is empty
                _fl_canUpdate = true;
                timer1.Start();
                return;
            }

            _fl_canUpdate = false;

            // check wheter we have new sources
            if (!DataWorkSource.NewSourcesAvailable())
            {
                timer1.Start();
                return;
            }

            // Com_WinApi.OutputDebugString("MainWnd --- AddingData Begin");

            // update source status icon
            DDM_UpdateStatus.Image = Properties.Resources.ok;
            this.Update();

            // show data loading message
            uiWndUpdateWnd _uiWndUpdateDlg = null;
            {
                _uiWndUpdateDlg = new uiWndUpdateWnd();
                _uiWndUpdateDlg.ShowUpdate(this);
                _uiWndUpdateDlg.Update();
                _uiWndUpdateDlg.Refresh();
            }

            // download new sources
            Dictionary<string, DataTable> newSources = DataWorkSource.DownloadSource();

            // hide data loading message
            if (_uiWndUpdateDlg != null)
            {
                _uiWndUpdateDlg.Close();
                _uiWndUpdateDlg.Dispose();
                _uiWndUpdateDlg = null;
            }

            // update source status icon
            DDM_UpdateStatus.Image = Properties.Resources.ExNotOk;
            this.Update();

            // show message that db is updated
            if (_fl_onlyUpdate)
                MMessageBox.Show(this.articleDGV, "Будуть внесені зміни в базу товарів", Application.ProductName);

            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles && this.Cheques.Tables.Count != ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Count)
            {
                DataWorkSource.CreateTables(ref Cheque, ref Articles, ref AltBC, ref Cards, ref Cheques);
                this.CreateOrderStructure(this.Cheque);
                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                {
                    this.CreateOrderStructure(this.Cheques.Tables[de.Key.ToString()]);
                    this.Discount[de.Key] = DataWorkShared.GetStandartDiscountInfoStructure2();
                    this.Summa[de.Key] = DataWorkShared.GetStandartCalculationInfoStructure2();
                }
            }

            if (this.Summa.Count != ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Count)
            {
                this.Discount.Clear();
                this.Summa.Clear();
                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                {
                    this.Discount[de.Key] = DataWorkShared.GetStandartDiscountInfoStructure2();
                    this.Summa[de.Key] = DataWorkShared.GetStandartCalculationInfoStructure2();
                }
            }

            // clear all prev data
            Articles.Rows.Clear();
            AltBC.Rows.Clear();
            Cards.Rows.Clear();

            // merge new data
            Articles.Merge(newSources[CoreConst.DATA_CONTAINER_PRODUCT]);
            AltBC.Merge(newSources[CoreConst.DATA_CONTAINER_ALTERNATIVE]);
            Cards.Merge(newSources[CoreConst.DATA_CONTAINER_CLIENT]);

            // check app state
            this._fl_isOk = new Com_SecureRuntime().FullLoader();
            this.label_uiWndmain_DemoShowArt.Visible = this.label_uiWndmain_DemoShowChq.Visible = !this._fl_isOk;

            // indicate that next time we will show message about new chages
            this._fl_onlyUpdate = true; // <- this is false at startup

            // resume source fetch timer
            timer1.Start();

            // activate searchbox
            SrchTbox.Select();

            // cleanup
            GC.Collect();

            /* device status */
            if (Program.AppPlugins.IsActive(PluginType.LegalPrinterDriver))
            {
                try
                {
                    bool status = (bool)Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_SetCashier", ConfigManager.Instance.CommonConfiguration.APP_PayDesk, UserConfig.UserFpLogin, UserConfig.UserFpPassword, UserConfig.UserID);
                    if (status)
                        DDM_FPStatus.Image = Properties.Resources.ok;
                    else
                        DDM_FPStatus.Image = Properties.Resources.FpNotOk;
                }
                catch { DDM_FPStatus.Image = Properties.Resources.FpNotOk; }
            }
            else
                DDM_FPStatus.Image = Properties.Resources.FpNotOk;

            return;


            ///* notification */
            ///*
            //*/
            //int currentProfileIndex = 0;
            //int startupIndex = 0;
            //bool notificationIsActive = false;
            //uiWndUpdateWnd uw = new uiWndUpdateWnd();


            ///* Data Loader v2.0 */
            //// Com_HashObject newFiles = DataWorkSource.CheckGetDataSource(dataContainer2.Structures[CoreConst.CONTAINER_STATE].GetTypedProperty<bool>(CoreConst.STATE_DATA_UPDATE_ONLY));
            //// DataWorkSource.UpdateSource(newFiles, ref this.dataContainer2);

            //if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles && this.Cheques.Tables.Count != ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Count)
            //{
            //    DataWorkSource.CreateTables(ref Cheque, ref Articles, ref AltBC, ref Cards, ref Cheques);
            //    this.CreateOrderStructure(this.Cheque);
            //    foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
            //    {
            //        this.CreateOrderStructure(this.Cheques.Tables[de.Key.ToString()]);
            //        this.Discount[de.Key] = DataWorkShared.GetStandartDiscountInfoStructure2();
            //        this.Summa[de.Key] = DataWorkShared.GetStandartCalculationInfoStructure2();
            //    }
            //}

            //if (this.Summa.Count != ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Count)
            //{
            //    this.Discount.Clear();
            //    this.Summa.Clear();
            //    foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
            //    {
            //        this.Discount[de.Key] = DataWorkShared.GetStandartDiscountInfoStructure2();
            //        this.Summa[de.Key] = DataWorkShared.GetStandartCalculationInfoStructure2();
            //    }
            //}



            ////MessageBox.Show("done 1");

            //List<string> allProfiles = new List<string>();
            ////bool wasUpdatedAtLeastOneSource = false;
            //_fl_artUpdated = false;
            //foreach (DictionaryEntry de in hfiles)
            //{

            //    string[] files = (string[])de.Value;

            //    allProfiles.Add(de.Key.ToString());

            //    /* detectiong for updates */

            //    // server status
            //    if (files[0] == CoreConst.STATE_LAN_ERROR && hfiles.Count == 1)
            //        DDM_UpdateStatus.Image = Properties.Resources.ExNotOk;
            //    else
            //        DDM_UpdateStatus.Image = Properties.Resources.ok;

            //    if ((files[0] == CoreConst.STATE_LAN_ERROR || files[0] == "") && files[1] == "" && files[2] == "" && _fl_onlyUpdate)
            //    {
            //        /* if only one profile */
            //        if (hfiles.Count == 1)
            //        //if (!_fl_artUpdated && (hfiles.Count == 1 || currentProfileIndex + 1 == hfiles.Count))
            //        {
            //            timer1.Start();
            //            GC.Collect();
            //            /* close notification */
            //            if (notificationIsActive)
            //            {
            //                uw.Close();
            //                uw.Dispose();
            //            }
            //            return;
            //        }

            //        /* next turn */
            //        currentProfileIndex++;
            //        continue;
            //    }
            //    //MessageBox.Show("done 2");

            //    if (!notificationIsActive)
            //    {
            //        uw.ShowUpdate(this);
            //        uw.Update();
            //        uw.Refresh();
            //        notificationIsActive = true;
            //    }

            //    /* loading */

            //    //MessageBox.Show("done 3");
            //    // string[] localFiles = DataWorkSource.LoadFilesOnLocalTempFolder(files, de.Key);

            //    if (currentProfileIndex == 0)
            //        startupIndex = 0;
            //    else
            //        startupIndex = Articles.Rows.Count;
            //    object[] loadResult = DataWorkSource.LoadData(files, _fl_onlyUpdate, de.Key, startupIndex);


            //    // ConfigManager.SaveConfiguration();

            //    /* adding data */


            //    //MessageBox.Show("done 4");

            //    DataTable[] tables = (DataTable[])loadResult[0];
            //    if (!_fl_artUpdated)
            //        _fl_artUpdated = (bool)loadResult[1];

            //    if (tables[0] != null)
            //    {
            //        //Articles = tables[0].Copy();
            //        // var remainRows = from myRow in Articles.AsEnumerable() where myRow["F"] != de.Key select myRow;
            //        //DataTable table = Articles.Clone();
            //        //try
            //        //{
            //        //    if (Articles.Rows.Count > 0)
            //        //        table = Articles.AsEnumerable().Where(r => r.Field<string>("F") != de.Key.ToString()).CopyToDataTable();
            //        //}
            //        //catch { }

            //        //Articles.Rows.Clear();

            //        //if (table.Rows.Count > 0)
            //        //    Articles.Merge(table);

            //        //foreach (DataRow dr in remainRows)
            //        //    Articles.ImportRow(dr);


            //        //DataRow[] dRows = Articles.Select("F = " + de.Key);
            //        //foreach (DataRow dr in dRows)
            //        //    dr.Delete();
            //        Articles.Clear();
            //        Articles.Merge(tables[0]);
            //        //wasUpdatedAtLeastOneSource = true;
            //    }
            //    if (tables[1] != null)
            //    {
            //        //AltBC = tables[1].Copy();
            //        //DataRow[] dRows = AltBC.Select("F = " + de.Key);
            //        //foreach (DataRow dr in dRows)
            //        //    dr.Delete();
            //        AltBC.Clear();
            //        AltBC.Merge(tables[1]);
            //        //wasUpdatedAtLeastOneSource = true;
            //    }
            //    if (tables[2] != null)
            //    {
            //        //Cards = tables[2].Copy();
            //        //if (currentProfileIndex == 0)
            //        Cards.Rows.Clear();
            //        Cards.Merge(tables[2]);
            //        //wasUpdatedAtLeastOneSource = true;
            //    }

            //    //MessageBox.Show("done 5");
            //    currentProfileIndex++;

            //}


            //// ConfigManager.SaveConfiguration();

            ////MessageBox.Show("done 6");
            ///* Removing unused rows */
            ////string cleanupQuery = string.Empty;
            ////foreach (string existedProfiles in allProfiles)
            ////{
            ////    cleanupQuery += " F <> " + existedProfiles + " AND ";
            ////}
            ////cleanupQuery = cleanupQuery.Trim(new char[] {' ', 'A', 'N', 'D' });
            ////DataRow[] unusedRowsArt = Articles.Select(cleanupQuery);
            ////DataRow[] unusedRowsAlt = AltBC.Select(cleanupQuery);
            ////foreach (DataRow dr in unusedRowsArt)
            ////    dr.Delete();
            ////foreach (DataRow dr in unusedRowsAlt)
            ////    dr.Delete();

            ////MessageBox.Show("done 7");
            ///* close notification */
            //if (notificationIsActive)
            //{
            //    uw.Close();
            //    uw.Dispose();
            //}

            ////MessageBox.Show("done 8");
            //Com_WinApi.OutputDebugString("MainWnd --- AddingData End");

            //if (_fl_artUpdated)
            //{
            //    if (this.WindowState == FormWindowState.Minimized)
            //        this.WindowState = FormWindowState.Normal;
            //    this.BringToFront();
            //    MMessageBoxEx.Show(this.chequeDGV, "Були внесені зміни в базу товарів", Application.ProductName,
            //        MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}

            ////MessageBox.Show("done 9");
            //_fl_onlyUpdate = true;
            //_fl_SubUnitChanged = false;

            //this._fl_isOk = new Com_SecureRuntime().FullLoader();
            //this.label_uiWndmain_DemoShowArt.Visible = this.label_uiWndmain_DemoShowChq.Visible = !this._fl_isOk;
            ////MessageBox.Show("done 10");

            //timer1.Start();
            //SrchTbox.Select();
            //GC.Collect();

            ///* device status */
            //if (Program.AppPlugins.IsActive(PluginType.LegalPrinterDriver))
            //{
            //    try
            //    {
            //        bool status = (bool)Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_SetCashier", ConfigManager.Instance.CommonConfiguration.APP_PayDesk, UserConfig.UserFpLogin, UserConfig.UserFpPassword, UserConfig.UserID);
            //        if (status)
            //            DDM_FPStatus.Image = Properties.Resources.ok;
            //        else
            //            DDM_FPStatus.Image = Properties.Resources.FpNotOk;
            //    }
            //    catch { DDM_FPStatus.Image = Properties.Resources.FpNotOk; }
            //} else
            //    DDM_FPStatus.Image = Properties.Resources.FpNotOk;
        }

        /// <summary>
        /// Обробляє чек після видалення одного або всіх товару(ів).
        /// (обраховує суму, відновлує фільтрацію таблиці товарів, оновлює повідомлення на панелях)
        /// </summary>
        /// <param name="updateCustomer">Якщо true то результати обчислення будуть ще виведені на дисплей ФП</param>
        private void RowsRemoved_MyEvent(bool updateCustomer)
        {
            this.RowsRemoved_MyEvent(updateCustomer, true, false);
        }
        private void RowsRemoved_MyEvent(bool updateCustomer, bool resetSrchFilter)
        {
            this.RowsRemoved_MyEvent(updateCustomer, resetSrchFilter, false);
        }
        private void RowsRemoved_MyEvent(bool updateCustomer, bool resetSrchFilter, bool clearData)
        {
            if (clearData)
            {
                this.Cheque.Rows.Clear();
                this.CreateOrderStructure(this.Cheque);
                //this.Cheque.ExtendedProperties.Clear();

                if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                    foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                        RowsRemoved_MyEvent_profile(de.Key);
            }

            //winapi.Funcs.OutputDebugString("t");
            if (Cheque.Rows.Count == 0)
            {
                RefershMenus();
                retriveChq = false;
                //if (retriveChq)
                //    чекПоверненняToolStripMenuItem.PerformClick();
                //winapi.Funcs.OutputDebugString("3");
                if (resetSrchFilter)
                    SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                else
                    SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, false);
                //winapi.Funcs.OutputDebugString("4");
                clientID = string.Empty;
                chqSUMA = 0.0;
                realSUMA = 0.0;
                ResetDiscount();
            }
            //winapi.Funcs.OutputDebugString("l");
            UpdateSumInfo(updateCustomer);
            RefreshChequeInformer(false);
            //winapi.Funcs.OutputDebugString("e");
            //winapi.Funcs.OutputDebugString("r");

            this.Update();
            Com_WinApi.PostMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_UPDATE, IntPtr.Zero, IntPtr.Zero);
        }
        private void RowsRemoved_MyEvent_profile(object profileKey)
        {
            this.Summa[profileKey] = DataWorkShared.GetStandartCalculationInfoStructure2();
            this.Discount[profileKey] = DataWorkShared.GetStandartDiscountInfoStructure2();
            
            // clear rows from common cheque

            foreach (DataRow dRow in this.Cheques.Tables[profileKey.ToString()].Rows)
            {
                try
                {
                    this.Cheque.Rows.Find(dRow["C"]).Delete();
                }
                catch { }
            }
            
            // clear profile cheque
            this.Cheques.Tables[profileKey.ToString()].Rows.Clear();

            

        }
        /// <summary>
        /// Виконує обрахунок суми товарів, які знаходяться в таблиці чеку
        /// а також вираховує коефіціенти знижок чи надбавок
        /// </summary>
        /// <param name="updateCustomer">Якщо true то результати обчислення будуть ще виведені на дисплей ФП</param>
        /// 

        // discount

        private void UpdateSumInfo(bool updateCustomer)
        {
            UpdateSumInfo_single(updateCustomer);
            List<string> soldItems = new List<string>();
            /* perform update for all profiles if it necessary */
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
            {
                DataRow[] profileDataRows = null;
                string[] filterProductIDs = null;
                string selectCommandForLegalProducts = string.Empty;
                string selectAdditionalCommandForLegalProducts = string.Empty;

                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                {

                    selectCommandForLegalProducts = "F=" + de.Key;
                    selectAdditionalCommandForLegalProducts = string.Empty;
                    try
                    {
                        filterProductIDs = (((Hashtable)de.Value)["FILTER"]).ToString().Split(' ', ',', ';');
                    }
                    catch
                    {
                        filterProductIDs = new string[0];
                    }
                    
                    if (filterProductIDs.Length != 0)
                    {
                        foreach (string lss in filterProductIDs)
                            if (lss != string.Empty)
                                selectAdditionalCommandForLegalProducts += string.Format("ID LIKE '{0}%' OR ", lss);
                        //selectAdditionalCommandForLegalProducts = string.Format("({0})", selectAdditionalCommandForLegalProducts);
                        selectAdditionalCommandForLegalProducts = selectAdditionalCommandForLegalProducts.TrimEnd(' ', 'O', 'R');
                    }

                    if (selectAdditionalCommandForLegalProducts != string.Empty)
                        selectCommandForLegalProducts = string.Format("{0}", selectAdditionalCommandForLegalProducts);

                    profileDataRows = this.Cheque.Select(selectCommandForLegalProducts);
                    this.Cheques.Tables[de.Key.ToString()].Rows.Clear();
                    foreach (DataRow dr in profileDataRows)
                    {
                        if (!soldItems.Contains(dr["ID"].ToString()))
                        {
                            this.Cheques.Tables[de.Key.ToString()].Rows.Add(dr.ItemArray);
                            soldItems.Add(dr["ID"].ToString());
                        }
                    }


                    /* set discounts */



                    Hashtable _discount = (Hashtable)this.Discount[de.Key.ToString()];
                    _discount[CoreConst.DISC_ARRAY_PERCENT] = this.discArrPercent;
                    _discount[CoreConst.DISC_ARRAY_CASH] = this.discArrCash;
                    //_discount[CoreConst.DISC_CONST_PERCENT] = this.discConstPercent;
                    //_discount[CoreConst.DISC_ONLY_CASH] = this.discOnlyCash;
                    //_discount[CoreConst.DISC_ONLY_PERCENT] = this.discOnlyPercent;
                    this.Discount[de.Key.ToString()] = _discount;


                    UpdateSumInfo_profile(de.Key.ToString(), updateCustomer);

                    if (this.Cheque.Rows.Count == 0)
                    {
                        realSUMA = chqSUMA = taxSUMA = 0.0;
                        UpdateSumDisplay(false, updateCustomer);
                        // this.PD_EmptyOrder;
                        return;
                    }
                }
            }
        }

        private void UpdateSumInfo_profile(object profileKey, bool updateCustomer)
        {
            //OnDeactivate(EventArgs.Empty);
            //winapi.Funcs.OutputDebugString("X");
            //if (inventChq)
            //    return;
            /* Current cheque is this.Cheques.Tables[profileKey.ToString()] */

            Hashtable _suma = (Hashtable)this.Summa[profileKey];
            Hashtable _discount = (Hashtable)this.Discount[profileKey];
            DataTable _cheque = this.Cheques.Tables[profileKey.ToString()];

            /* standart product filtering */

            /*
            DataRow[] profileDataRows = Cheque.Select("F = " + profileKey);
            this.Cheques.Tables[profileKey.ToString()].Rows.Clear();

            foreach (DataRow dr in profileDataRows)
                this.Cheques.Tables[profileKey.ToString()].Rows.Add(dr.ItemArray);
            */


            /* initializing discount values */
            bool _discApplied = CoreLib.GetValue<bool>(_discount, CoreConst.DISC_APPLIED);
            double[] _discArrPercent = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_PERCENT);
            double[] _discArrCash = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_CASH);
            double _discConstPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_CONST_PERCENT);
            double _discOnlyPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_PERCENT);
            double _discOnlyCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_CASH);
            double _discCommonPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_PERCENT);
            double _discCommonCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_CASH);
            /* calculation items */
            double _realSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_REAL_SUMA);
            double _chqSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_CHEQUE_SUMA);
            double _taxSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_TAX_SUMA);
            /* internal */
            int i = 0;
            double dValue = 0.0;
            double taxValue = 0.0;
            double artSum = 0.0;
            int index = 0;
            DataRow[] dRows = null;
            double discSUMA = 0.0;
            //DataRow[] artRecord = null;
            double newPrice = 0.0;
            double newTmpPrice = 0.0; //bool isSet = false;
            Hashtable profileDefinedTaxGrid = new Hashtable();
            Hashtable profileCompatibleTaxGrid = new Hashtable();
            
            /*  */
            bool useConstDisc = _discArrPercent[0] == 0.0 && _discArrPercent[1] == 0.0 &&
                _discArrCash[0] == 0.0 && _discArrCash[1] == 0.0;


            // Get discount value
            if (useConstDisc)
            {
                _discConstPercent = 0.0;
                //form "sum" by static discount
                if (ConfigManager.Instance.CommonConfiguration.APP_UseStaticDiscount)
                    _discConstPercent = ConfigManager.Instance.CommonConfiguration.APP_StaticDiscountValue;
                //form "sum" by dynamic discount
                if (ConfigManager.Instance.CommonConfiguration.APP_UseStaticRules)
                    _discConstPercent = CoreLib.UpdateSumbyRules(_cheque);
            }
            else
            {
                _discOnlyPercent = _discArrPercent[0] + _discArrPercent[1];
                _discOnlyCash = _discArrCash[0] + _discArrCash[1];
                _discOnlyPercent = MathLib.GetRoundedMoney(_discOnlyPercent);
                _discOnlyCash = MathLib.GetRoundedMoney(_discOnlyCash);
            }
            if (_cheque.Rows.Count == 0) return;
            /* removed             if (_cheque.Rows.Count == 0)
            {
                _realSUMA = _chqSUMA = _taxSUMA = 0.0;
                UpdateSumDisplay(false, updateCustomer);
                // this.PD_EmptyOrder;
                return;
            } */

            // restore native cheque sum
            // and set price acording to client's discount card
            for (i = 0; i < _cheque.Rows.Count; i++)
            {
                newPrice = MathLib.GetDouble(_cheque.Rows[i]["ORIGPRICE"]);

                ////isSet = false;
                //if (this.clientPriceNo != 0)
                //{
                //    newTmpPrice = MathLib.GetDouble(_cheque.Rows[i]["PR" + this.clientPriceNo].ToString());
                //    if (newTmpPrice != 0.0) newPrice = newTmpPrice;
                //}
                //else if (UserConfig.Properties[8])
                //{
                //    //DataRow dRow = Cheque.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                //    //price = AppFunc.AutomaticPrice(thisTot, dRow);
                //    newPrice = CoreLib.AutomaticPrice(MathLib.GetDouble(_cheque.Rows[i]["TOT"].ToString()), _cheque.Rows[i]);
                //}
                //isSet = false;
                if (this.clientPriceNo != 0)
                {
                    newTmpPrice = MathLib.GetDouble(_cheque.Rows[i]["PR" + this.clientPriceNo].ToString());
                    if (newTmpPrice != 0.0) newPrice = newTmpPrice;
                }
                else if (UserConfig.Properties[8])
                {
                    //DataRow dRow = Cheque.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                    //price = AppFunc.AutomaticPrice(thisTot, dRow);
                    double _newPrice = CoreLib.AutomaticPrice(MathLib.GetDouble(_cheque.Rows[i]["TOT"].ToString()), _cheque.Rows[i]);
                    try
                    {
                        profileDefinedTaxGrid = (Hashtable)driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates[_cheque.Rows[i]["F"]];
                    }
                    catch { }
                    // new tax mode
                    bool _thisRowCanUseDiscount = true;
                    try
                    {
                        // get application tax char with compatible tax grid
                        string[] definedTaxData = profileDefinedTaxGrid[_cheque.Rows[i]["VG"].ToString()].ToString().Split(';');
                        _thisRowCanUseDiscount = Boolean.Parse(definedTaxData[1]);
                    }
                    catch { }

                    if (_thisRowCanUseDiscount)
                    {

                        double _discountPrices = 0.0;
                        //for (int ii = 0; ii < Cheque.Rows.Count; ii++)
                        //{
                        if (_newPrice != (double)_cheque.Rows[i]["ORIGPRICE"])
                        {
                            _discountPrices = 100 - _newPrice * 100 / (double)_cheque.Rows[i]["ORIGPRICE"];
                            if (_discountPrices > discCommonPercent)
                            {
                                _cheque.Rows[i]["USEDDISC"] = Boolean.FalseString;
                                newPrice = _newPrice;
                            }
                            else
                            {
                                _cheque.Rows[i]["USEDDISC"] = Boolean.TrueString;
                                newPrice = MathLib.GetDouble(_cheque.Rows[i]["ORIGPRICE"]);
                            }
                        }
                        else newPrice = _newPrice;
                        //}
                    }
                    else newPrice = _newPrice;
                }
                else if (UserConfig.Properties[1] || UserConfig.Properties[2])
                {
                    newPrice = MathLib.GetDouble(_cheque.Rows[i]["PRICE"]);
                }
                _cheque.Rows[i]["PRICE"] = newPrice;
                _cheque.Rows[i]["ASUM"] = _cheque.Rows[i]["SUM"] = MathLib.GetRoundedMoney(MathLib.GetDouble(_cheque.Rows[i]["TOT"].ToString()) * newPrice);
                _cheque.Rows[i]["DISC"] = 0.0;
            }
            _chqSUMA = (double)_cheque.Compute("sum(SUM)", "");
            _chqSUMA = MathLib.GetRoundedMoney(_chqSUMA);
            _realSUMA = _chqSUMA;

            //select rows with discount mode
            try
            {
                dRows = _cheque.Select("USEDDISC = " + Boolean.TrueString);
                _fl_useTotDisc = (dRows.Length == _cheque.Rows.Count);
                object d = _cheque.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                if (d != DBNull.Value)
                    double.TryParse(d.ToString(), out discSUMA);
                if (dRows.Length == 0)
                {
                    _discApplied = false;
                    /*
                    _discArrPercent[0] = 0.0;
                    _discArrPercent[1] = 0.0;
                    _discArrCash[0] = 0.0;
                    _discArrCash[1] = 0.0;
                    */
                }
                else
                    _discApplied = true;
            }
            catch { };


            //procentnuj zagalnuj koeficient
            if (useConstDisc)
                _discCommonPercent = _discConstPercent;
            else
                _discCommonPercent = _discOnlyPercent;
            if (discSUMA != 0.0)
                _discCommonPercent += (_discOnlyCash * 100) / discSUMA;
            _discCommonPercent = MathLib.GetRoundedMoney(_discCommonPercent);

            DataRow[] prRows = null;
            if (this.clientPriceNo != 0)
                prRows = _cheque.Select("PR" + this.clientPriceNo + " <> 0");

            if (_fl_useTotDisc && prRows == null)
            {
                //obrahunok realnoi sumu cheku zi znugkojy
                if (useConstDisc)
                {
                    dValue = (_discConstPercent * discSUMA) / 100;
                    dValue = MathLib.GetRoundedMoney(dValue);
                    _realSUMA -= dValue;
                }
                else
                {
                    dValue = (_discOnlyPercent * discSUMA) / 100;
                    dValue = MathLib.GetRoundedMoney(dValue);
                    _realSUMA -= dValue;
                    _realSUMA -= discOnlyCash;
                }
            }
            else
            {
                _fl_useTotDisc = false;
                for (i = 0; i < dRows.Length; i++)
                {
                    // don't use discount when client want to has another price for current article
                    if (this.clientPriceNo != 0 && MathLib.GetDouble(dRows[i]["PR" + this.clientPriceNo].ToString()) > 0.0)
                    {
                        dRows[i]["DISC"] = 0.0;
                        continue;
                    }
                    dRows[i]["DISC"] = _discCommonPercent;
                    dValue = (_discCommonPercent * (double)dRows[i]["SUM"]) / 100;
                    //discValue = AppFunc.GetRoundedMoney(discValue);
                    dValue = (double)dRows[i]["SUM"] - dValue;
                    dRows[i]["ASUM"] = MathLib.GetRoundedMoney(dValue);
                }
                _realSUMA = (double)_cheque.Compute("Sum(ASUM)", "");
            }
            _realSUMA = MathLib.GetRoundedMoney(_realSUMA);


            //vuvedennja zagalnogo koeficientu znugku v 2oh tupah
            //groshovuj koeficient
            _discCommonCash = _chqSUMA - _realSUMA;
            _discCommonCash = MathLib.GetRoundedMoney(_discCommonCash);
            
            // calculating tax sum
            _taxSUMA = 0.0;
            for (i = 0; i < _cheque.Rows.Count; i++)
            {
                try
                {
                    taxValue = MathLib.GetDouble(_cheque.Rows[i]["TAX_VAL"]);
                    if (Boolean.Parse(_cheque.Rows[i]["USEDDISC"].ToString()))
                    {
                        artSum = (discCommonPercent * (double)_cheque.Rows[i]["SUM"]) / 100;
                        artSum = (double)_cheque.Rows[i]["SUM"] - artSum;
                        artSum = MathLib.GetRoundedMoney(artSum);
                        taxValue = (artSum * taxValue) / (taxValue + 100);
                    }
                    else
                        taxValue = (((double)_cheque.Rows[i]["ASUM"]) * taxValue) / (taxValue + 100);
                }
                catch
                {
                    taxValue = 0;
                }

                /*
                index = Array.IndexOf<char>(ConfigManager.Instance.CommonConfiguration.TAX_AppTaxChar, _cheque.Rows[i]["VG"].ToString()[0]);
                if (index >= 0)
                {
                    taxValue = ConfigManager.Instance.CommonConfiguration.TAX_AppTaxRates[index];

                    if (ConfigManager.Instance.CommonConfiguration.TAX_AppTaxDisc[index])
                    {
                        artSum = (_discCommonPercent * (double)_cheque.Rows[i]["SUM"]) / 100;
                        artSum = (double)_cheque.Rows[i]["SUM"] - artSum;
                        artSum = MathLib.GetRoundedMoney(artSum);
                        taxValue = (artSum * taxValue) / (taxValue + 100);
                    }
                    else
                        taxValue = (((double)Cheque.Rows[i]["ASUM"]) * taxValue) / (taxValue + 100);
                }
                else
                    taxValue = 0;
                */
                //if (!_fl_useTotDisc)
                //else
                //taxValue = AppFunc.GetRoundedMoney(taxValue);
                _cheque.Rows[i]["TAX_MONEY"] = taxValue;
                _taxSUMA += taxValue;
            }
            //taxSUMA = (double)Cheque.Compute("sum(TAX_MONEY)", "");
            _taxSUMA = MathLib.GetRoundedMoney(_taxSUMA);


            /* initializing discount values */
            CoreLib.SetContainerValue(ref _discount, CoreConst.DISC_APPLIED, _discApplied);
            CoreLib.SetContainerValue(ref _discount, CoreConst.DISC_ARRAY_PERCENT, _discArrPercent);
            CoreLib.SetContainerValue(ref _discount, CoreConst.DISC_ARRAY_CASH, _discArrCash);
            CoreLib.SetContainerValue(ref _discount, CoreConst.DISC_CONST_PERCENT, _discConstPercent);
            CoreLib.SetContainerValue(ref _discount, CoreConst.DISC_ONLY_PERCENT, _discOnlyPercent);
            CoreLib.SetContainerValue(ref _discount, CoreConst.DISC_ONLY_CASH, _discOnlyCash);
            CoreLib.SetContainerValue(ref _discount, CoreConst.DISC_FINAL_PERCENT, _discCommonPercent);
            CoreLib.SetContainerValue(ref _discount, CoreConst.DISC_FINAL_CASH, _discCommonCash);
            /* calculation items */
            CoreLib.SetContainerValue(ref _suma, CoreConst.CALC_REAL_SUMA, _realSUMA);
            CoreLib.SetContainerValue(ref _suma, CoreConst.CALC_CHEQUE_SUMA, _chqSUMA);
            CoreLib.SetContainerValue(ref _suma, CoreConst.CALC_TAX_SUMA, _taxSUMA);

            this.Discount[profileKey] = _discount;
            this.Summa[profileKey] = _suma;
            
            /*
            double[] _discArrPercent = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_PERCENT);
            double[] _discArrCash = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_CASH);
            double _discConstPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_CONST_PERCENT);
            double _discOnlyPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_PERCENT);
            double _discOnlyCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_CASH);
            double _discCommonPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_PERCENT);
            double _discCommonCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_CASH);*/
            /* calculation items *//*
            double _realSUMA = CoreLib.GetValue<double>(_suma, CoreConst.DISC_FINAL_CASH);
            double _chqSUMA = CoreLib.GetValue<double>(_suma, CoreConst.DISC_FINAL_CASH);
            double _taxSUMA = CoreLib.GetValue<double>(_suma, CoreConst.DISC_FINAL_CASH);*/


            if (!inventChq)
                UpdateSumDisplay(true, updateCustomer);

            return;
            //winapi.Funcs.OutputDebugString("Z");
        }//ok

        private void UpdateSumInfo_single(bool updateCustomer)
        {
            //OnDeactivate(EventArgs.Empty);
            //winapi.Funcs.OutputDebugString("X");
            //if (inventChq)
            //    return;

            bool useConstDisc = discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 &&
                discArrCash[0] == 0.0 && discArrCash[1] == 0.0;

            // Get discount value
            if (useConstDisc)
            {
                discConstPercent = 0.0;
                //form "sum" by static discount
                if (ConfigManager.Instance.CommonConfiguration.APP_UseStaticDiscount)
                    discConstPercent = ConfigManager.Instance.CommonConfiguration.APP_StaticDiscountValue;
                //form "sum" by dynamic discount
                if (ConfigManager.Instance.CommonConfiguration.APP_UseStaticRules)
                    discConstPercent = CoreLib.UpdateSumbyRules(Cheque);
            }
            else
            {
                discOnlyPercent = discArrPercent[0] + discArrPercent[1];
                discOnlyCash = discArrCash[0] + discArrCash[1];
                discOnlyPercent = MathLib.GetRoundedMoney(discOnlyPercent);
                discOnlyCash = MathLib.GetRoundedMoney(discOnlyCash);
            }

            if (Cheque.Rows.Count == 0)
            {
                realSUMA = chqSUMA = taxSUMA = 0.0;
                UpdateSumDisplay(false, updateCustomer);
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
                discCommonPercent = discConstPercent;
            else
                discCommonPercent = discOnlyPercent;
            if (discSUMA != 0.0)
                discCommonPercent += (discOnlyCash * 100) / discSUMA;
            discCommonPercent = MathLib.GetRoundedMoney(discCommonPercent);


            // restore native cheque sum
            // and set price acording to client's discount card
            //DataRow[] artRecord = null;
            double newPrice = 0.0;
            double newTmpPrice = 0.0; //bool isSet = false;
            Hashtable profileDefinedTaxGrid = new Hashtable();
            Hashtable profileCompatibleTaxGrid = new Hashtable();
            for (i = 0; i < Cheque.Rows.Count; i++)
            {
                newPrice = MathLib.GetDouble(Cheque.Rows[i]["ORIGPRICE"]);

                //isSet = false;
                if (this.clientPriceNo != 0)
                {
                    newTmpPrice = MathLib.GetDouble(Cheque.Rows[i]["PR" + this.clientPriceNo].ToString());
                    if (newTmpPrice != 0.0) newPrice = newTmpPrice;
                }
                else if (UserConfig.Properties[8])
                {
                    //DataRow dRow = Cheque.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                    //price = AppFunc.AutomaticPrice(thisTot, dRow);
                    double _newPrice = CoreLib.AutomaticPrice(MathLib.GetDouble(Cheque.Rows[i]["TOT"].ToString()), Cheque.Rows[i]);
                    try
                    {
                        profileDefinedTaxGrid = (Hashtable)driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates[Cheque.Rows[i]["F"]];
                    }
                    catch { }
                    // new tax mode
                    bool _thisRowCanUseDiscount = true;
                    try
                    {
                        // get application tax char with compatible tax grid
                        string[] definedTaxData = profileDefinedTaxGrid[Cheque.Rows[i]["VG"].ToString()].ToString().Split(';');
                        _thisRowCanUseDiscount = Boolean.Parse(definedTaxData[1]);
                    }
                    catch { }

                    if (_thisRowCanUseDiscount)
                    {

                        double _discountPrices = 0.0;
                        //for (int ii = 0; ii < Cheque.Rows.Count; ii++)
                        //{
                        if (_newPrice != (double)Cheque.Rows[i]["ORIGPRICE"])
                        {
                            _discountPrices = 100 - _newPrice * 100 / (double)Cheque.Rows[i]["ORIGPRICE"];
                            if (_discountPrices > discCommonPercent)
                            {
                                Cheque.Rows[i]["USEDDISC"] = Boolean.FalseString;
                                newPrice = _newPrice;
                            }
                            else
                            {
                                Cheque.Rows[i]["USEDDISC"] = Boolean.TrueString;
                                newPrice = MathLib.GetDouble(Cheque.Rows[i]["ORIGPRICE"]);
                            }
                        }
                        else newPrice = _newPrice;
                        //}
                    } else newPrice = _newPrice;
                }
                else if (UserConfig.Properties[1] || UserConfig.Properties[2])
                {
                    newPrice = MathLib.GetDouble(Cheque.Rows[i]["PRICE"]);
                }
                Cheque.Rows[i]["PRICE"] = newPrice;
                Cheque.Rows[i]["ASUM"] = Cheque.Rows[i]["SUM"] = MathLib.GetRoundedMoney(MathLib.GetDouble(Cheque.Rows[i]["TOT"].ToString()) * newPrice);
                Cheque.Rows[i]["DISC"] = 0.0;
            }
            chqSUMA = (double)Cheque.Compute("sum(SUM)", "");
            chqSUMA = MathLib.GetRoundedMoney(chqSUMA);
            realSUMA = chqSUMA;




            // check if we apply discount ot second or third price 
            /*if (UserConfig.Properties[8])
            {
                double _discountPrices = 0.0;
                for (i = 0; i < Cheque.Rows.Count; i++)
                {
                    if ((double)Cheque.Rows[i]["PRICE"] != (double)Cheque.Rows[i]["ORIGPRICE"])
                    {
                        _discountPrices = 100 - (double)Cheque.Rows[i]["PRICE"] * 100 / (double)Cheque.Rows[i]["ORIGPRICE"];
                        if (_discountPrices > discCommonPercent)
                        {
                            Cheque.Rows[i]["USEDDISC"] = Boolean.FalseString;
                        }
                        else
                        {
                            Cheque.Rows[i]["USEDDISC"] = Boolean.TrueString;
                            Cheque.Rows[i]["PRICE"] = Cheque.Rows[i]["ORIGPRICE"];
                        }
                    }
                }
            }*/
           
            //select rows with discount mode
            try
            {
                dRows = Cheque.Select("USEDDISC = " + Boolean.TrueString);
                _fl_useTotDisc = (dRows.Length == Cheque.Rows.Count);
                //discSUMA = (double)Cheque.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                object d = Cheque.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                if (d != DBNull.Value)
                    double.TryParse(d.ToString(), out discSUMA);
                if (dRows.Length == 0)
                {
                    this.discApplied = false;
                    /*
                    _discArrPercent[0] = 0.0;
                    _discArrPercent[1] = 0.0;
                    _discArrCash[0] = 0.0;
                    _discArrCash[1] = 0.0;
                    */
                }
                else
                    this.discApplied = true;
            }
            catch { };


            DataRow[] prRows = null;
            if (this.clientPriceNo != 0)
                prRows = Cheque.Select("PR" + this.clientPriceNo + " <> 0");


            if (_fl_useTotDisc && prRows == null)
            {
                //obrahunok realnoi sumu cheku zi znugkojy
                if (useConstDisc)
                {
                    dValue = (discConstPercent * discSUMA) / 100;
                    dValue = MathLib.GetRoundedMoney(dValue);
                    realSUMA -= dValue;
                }
                else
                {
                    dValue = (discOnlyPercent * discSUMA) / 100;
                    dValue = MathLib.GetRoundedMoney(dValue);
                    realSUMA -= dValue;
                    realSUMA -= discOnlyCash;
                }
            }
            else
            {
                _fl_useTotDisc = false;
                for (i = 0; i < dRows.Length; i++)
                {
                    // don't use discount when client want to has another price for current article
                    if (this.clientPriceNo != 0 && MathLib.GetDouble(dRows[i]["PR" + this.clientPriceNo].ToString()) > 0.0)
                    {
                        dRows[i]["DISC"] = 0.0;
                        continue;
                    }
                    dRows[i]["DISC"] = discCommonPercent;
                    dValue = (discCommonPercent * (double)dRows[i]["SUM"]) / 100;
                    //discValue = AppFunc.GetRoundedMoney(discValue);
                    dValue = (double)dRows[i]["SUM"] - dValue;
                    dRows[i]["ASUM"] = MathLib.GetRoundedMoney(dValue);
                }
                realSUMA = (double)Cheque.Compute("Sum(ASUM)", "");
            }
            realSUMA = MathLib.GetRoundedMoney(realSUMA);

            //vuvedennja zagalnogo koeficientu znugku v 2oh tupah
            //groshovuj koeficient
            discCommonCash = chqSUMA - realSUMA;
            discCommonCash = MathLib.GetRoundedMoney(discCommonCash);

            // calculating tax sum
            taxSUMA = 0.0;

            //Hashtable profileDefinedTaxGrid = new Hashtable();
            //Hashtable profileCompatibleTaxGrid = new Hashtable();
            //bool taxGridError = false;
            //string[] definedTaxData = null;
            for (i = 0; i < Cheque.Rows.Count; i++)
            {/*
                try
                {
                    profileDefinedTaxGrid = (Hashtable)driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_DefinedRates[Cheque.Rows[i]["F"]];
                    profileCompatibleTaxGrid = (Hashtable)driver.Config.ConfigManager.Instance.CommonConfiguration.TAX_Compatibility[Cheque.Rows[i]["F"]];
                }
                catch { taxGridError = false; }
                */
                // new tax operation
                //Cheque.Rows[i]["VG"].ToString()[0]
               // if ( true)
               // {
                    // defined data structure
                    // 0 - tax rate
                    // 1- use discount
                    
                    //definedTaxData = profileDefinedTaxGrid[profileCompatibleTaxGrid[Cheque.Rows[i]["VG"].ToString()[0]]].ToString().Split(';');
                //taxValue = MathLib.GetDouble(definedTaxData[0]);
                try
                {
                    taxValue = MathLib.GetDouble(Cheque.Rows[i]["TAX_VAL"]);
                    if (Boolean.Parse(Cheque.Rows[i]["USEDDISC"].ToString()))
                    {
                        artSum = (discCommonPercent * (double)Cheque.Rows[i]["SUM"]) / 100;
                        artSum = (double)Cheque.Rows[i]["SUM"] - artSum;
                        artSum = MathLib.GetRoundedMoney(artSum);
                        taxValue = (artSum * taxValue) / (taxValue + 100);
                    }
                    else
                        taxValue = (((double)Cheque.Rows[i]["ASUM"]) * taxValue) / (taxValue + 100);
                }
                catch
                {
                    taxValue = 0;
                }
               /* }
                else
                    taxValue = 0;*/

                /* // old tax operation
                index = Array.IndexOf<char>(ConfigManager.Instance.CommonConfiguration.TAX_AppTaxChar, Cheque.Rows[i]["VG"].ToString()[0]);
                if (index >= 0)
                {
                    taxValue = ConfigManager.Instance.CommonConfiguration.TAX_AppTaxRates[index];

                    if (ConfigManager.Instance.CommonConfiguration.TAX_AppTaxDisc[index])
                    {
                        artSum = (discCommonPercent * (double)Cheque.Rows[i]["SUM"]) / 100;
                        artSum = (double)Cheque.Rows[i]["SUM"] - artSum;
                        artSum = MathLib.GetRoundedMoney(artSum);
                        taxValue = (artSum * taxValue) / (taxValue + 100);
                    }
                    else
                        taxValue = (((double)Cheque.Rows[i]["ASUM"]) * taxValue) / (taxValue + 100);
                }
                else
                    taxValue = 0;
                */
                //if (!_fl_useTotDisc)
                //else
                //taxValue = AppFunc.GetRoundedMoney(taxValue);
                Cheque.Rows[i]["TAX_MONEY"] = taxValue;
                taxSUMA += taxValue;
            }
            //taxSUMA = (double)Cheque.Compute("sum(TAX_MONEY)", "");
            taxSUMA = MathLib.GetRoundedMoney(taxSUMA);

            if (!inventChq)
                UpdateSumDisplay(true, updateCustomer);

            //winapi.Funcs.OutputDebugString("Z");
        }//ok

        /// <summary>
        /// Custom method. Perform updating of information labels. Also update device displai
        /// </summary>
        /// <param name="updateAddChequeInfo"></param>
        /// <param name="updateCustomer">If true methid will update device display otherwise false</param>
        private void UpdateSumDisplay(bool updateAddChequeInfo, bool updateCustomer)
        {
            if (updateAddChequeInfo)
                addChequeInfo.Text = string.Empty;
            // if (updateAddChequeInfo && discCommonPercent != 0.0)

            if (discConstPercent != 0.0 || discArrPercent[0] != 0.0 || discArrPercent[1] != 0.0 ||
                discArrCash[0] != 0.0 || discArrCash[1] != 0.0)
            {
                object[] discInfo = new object[6];
                string valueMask = "{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}{1}";
                bool useConstDisc = discArrPercent[0] == 0.0 && discArrPercent[1] == 0.0 &&
                    discArrCash[0] == 0.0 && discArrCash[1] == 0.0;

                discInfo[0] = "";
                if (Cheque.Rows.Count != 0)
                    if (_fl_useTotDisc)
                        discInfo[0] = " загальна";
                    else 
                        discInfo[0] = " позиційна";

                if (useConstDisc)
                {
                    discInfo[0] = "постійна" + discInfo[0].ToString();
                    discInfo[1] = discConstPercent > 0 ? "знижка" : "націнка";
                    discInfo[2] = string.Format(valueMask, Math.Abs(discConstPercent), "%");
                }
                else
                    if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                    {
                        if (discArrPercent[0] != 0.0 || discArrCash[0] != 0.0)
                        {
                            discInfo[1] = "знижка";
                            if (ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc == 0)
                                if (discArrCash[0] == 0.0)
                                    discInfo[2] = string.Format(valueMask, discArrPercent[0], "%");
                                else
                                    discInfo[2] = string.Format(valueMask, discArrCash[0], "грн.");
                            else
                                if (discArrPercent[0] == 0.0)
                                    discInfo[2] = string.Format(valueMask, discArrCash[0], "грн.");
                                else
                                    discInfo[2] = string.Format(valueMask, discArrPercent[0], "%");
                        }
                        if (discArrPercent[1] != 0.0 || discArrCash[1] != 0.0)
                        {
                            discInfo[1] = "націнка";
                            if (ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc == 0)
                                if (discArrCash[1] == 0.0)
                                    discInfo[2] = string.Format(valueMask, Math.Abs(discArrPercent[1]), "%");
                                else
                                    discInfo[2] = string.Format(valueMask, Math.Abs(discArrCash[1]), "грн.");
                            else
                                if (discArrPercent[1] == 0.0)
                                    discInfo[2] = string.Format(valueMask, Math.Abs(discArrCash[1]), "грн.");
                                else
                                    discInfo[2] = string.Format(valueMask, Math.Abs(discArrPercent[1]), "%");
                        }
                    }
                    else
                    {
                        discInfo[1] = "знижка";

                        if (ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc == 0)
                            if (discArrCash[0] == 0.0)
                                discInfo[2] = string.Format(valueMask, discArrPercent[0], "%");
                            else
                                discInfo[2] = string.Format(valueMask, discArrCash[0], "грн.");
                        else
                            if (discArrPercent[0] == 0.0)
                                discInfo[2] = string.Format(valueMask, discArrCash[0], "грн.");
                            else
                                discInfo[2] = string.Format(valueMask, discArrPercent[0], "%");

                        discInfo[3] = "i";
                        discInfo[4] = "націнка";

                        if (ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc == 0)
                            if (discArrCash[1] == 0.0)
                                discInfo[5] = string.Format(valueMask, Math.Abs(discArrPercent[1]), "%");
                            else
                                discInfo[5] = string.Format(valueMask, Math.Abs(discArrCash[1]), "грн.");
                        else
                            if (discArrPercent[1] == 0.0)
                                discInfo[5] = string.Format(valueMask, Math.Abs(discArrCash[1]), "грн.");
                            else
                                discInfo[5] = string.Format(valueMask, Math.Abs(discArrPercent[1]), "%");
                    }

                addChequeInfo.Text = valueMask = string.Empty;
                for (byte i = 0; i < discInfo.Length && discInfo[i] != null; i++)
                    valueMask += (discInfo[i] + " ");
                addChequeInfo.Text = valueMask.Remove(valueMask.Length - 1, 1);
            }

            //Show cheque Suma on display
            string cashLabelFormat = string.Format("{0: ;-; } {{0:F{1}}}", retriveChq ? -1 : 1, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);
            CashLbl.Text = string.Format(cashLabelFormat, realSUMA);

            if (ConfigManager.Instance.CommonConfiguration.APP_ShowInfoOnIndicator && Program.AppPlugins.IsActive(PluginType.LegalPrinterDriver) && updateCustomer)
                try
                {
                    string _topLabel = "СУМА:" + CashLbl.Text;
                    if (discCommonPercent != 0)
                    {
                        if (discCommonPercent > 0)
                            _topLabel += " Зн:";
                        else
                            _topLabel += " Нб:";
                        _topLabel += Math.Abs(discCommonPercent) + "%";
                    }

                    string[] lines = new string[] { string.Empty, string.Empty };
                    bool[] show = new bool[] { true, true };
                    if (Cheque.Rows.Count != 0)
                        lines = new string[] { _topLabel, chequeDGV.CurrentRow.Cells["DESC"].Value.ToString() };
                    Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_SendCustomer", lines, show);
                }
                catch { }
        }

        /// <summary>
        /// Закриття чеку
        /// </summary>
        /// <param name="isLegalMode">Якщо true то чек є фіскальний</param>
        private void CloseCheque_profile(bool isLegalMode)//1_msg//lbl7
        {

            List<Hashtable> fullResult = new List<Hashtable>();
            Hashtable profileResult = new Hashtable();
            bool appIsLegal = isLegalMode;
            int pIdx = 0;
            object currentProfileKey = new object();
            object[] localData = new object[9];
            string chqNom = string.Empty;
            List<string> chqNumbers = new List<string>();
            List<string> chqNumbersFull = new List<string>();
            bool generalError = false;
            uiWndPayment pMethod = new uiWndPayment(realSUMA, true);
            pMethod.ShowDialog();
            pMethod.Dispose();



            if (pMethod.DialogResult != DialogResult.OK)
                return;


            if (UserConfig.Properties[4] &&
                DialogResult.Yes == MMessageBoxEx.Show(this.chequeDGV, "Видати накладну згідно цього чеку ?", Application.ProductName,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                nakladna = true;
                CashLbl.Image = Properties.Resources.naklad;
            }

            switch (pMethod.Type[0])
            {
                case 0: { digitalPanel.BackgroundImage = Properties.Resources.payment_card; break; }
                case 1: { digitalPanel.BackgroundImage = Properties.Resources.payment_credit; break; }
                case 2: { digitalPanel.BackgroundImage = Properties.Resources.payment_cheque; break; }
                case 3: { digitalPanel.BackgroundImage = Properties.Resources.payment_cash; break; }
            }

            // check if there are any profile as legal marked
            // if it exists - use it at first and then all other
            // otherwise peroform profiles as they are

            List<string> _allProfiles = new List<string>();
            bool hasLegalProfile = ConfigManager.Instance.CommonConfiguration.PROFILES_Items.ContainsKey(ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID);
            foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                _allProfiles.Add(de.Key.ToString());
            if (hasLegalProfile)
            {
                _allProfiles.Remove(ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID.ToString());
                _allProfiles.Insert(0, ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID.ToString());
            }


            Dictionary<string, object> _orgPaymanet = pMethod.PaymentInfo;
            DataTable dtCopy = this.PD_Order.Copy();
            int skippedProfiles = 0;

            //foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
            foreach (string profileKey in _allProfiles.ToArray())
            {
                try
                {
                    // profile id
                    //profileKey = de.Key;
                    // profile main index
                    currentProfileKey = profileKey;

                    pIdx++;

                    Hashtable _suma = (Hashtable)this.Summa[profileKey];
                    Hashtable _discount = (Hashtable)this.Discount[profileKey];
                    DataTable _cheque = this.Cheques.Tables[profileKey.ToString()];


                    if (_cheque.Rows.Count == 0)
                    {
                        skippedProfiles++;
                        chqNumbers.Add(" ");
                        chqNumbersFull.Add(" ");
                        continue;
                    }

                    /* initializing discount values */
                    double[] _discArrPercent = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_PERCENT);
                    double[] _discArrCash = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_CASH);
                    double _discConstPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_CONST_PERCENT);
                    double _discOnlyPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_PERCENT);
                    double _discOnlyCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_CASH);
                    double _discCommonPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_PERCENT);
                    double _discCommonCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_CASH);
                    /* calculation items */
                    double _realSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_REAL_SUMA);
                    double _chqSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_CHEQUE_SUMA);
                    double _taxSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_TAX_SUMA);


                    // it's checking if the running profile allows to save order as legal
                    appIsLegal = isLegalMode && ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID.Equals(profileKey);
                    //closeResult = CloseCheque_profile(de.Key.ToString(), isLegalMode);

                    profileResult = new Hashtable();

                    profileResult["PROFILE_ID"] = profileKey;
                    profileResult["PROFILE_LAST"] = pIdx == ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Count;



                    /* close logic implementation */

                    // if this is a legal or in-between profile and there are other profiles
                    // it requires to change payment info and set it without change
                    // but las profile should has the rest and chnaged payment

                    Dictionary<string, object> _tmpPaymanet = new Dictionary<string, object>(_orgPaymanet);
                    _tmpPaymanet["CASHLIST"] = new List<double>((List<double>)_tmpPaymanet["CASHLIST"]);
                    _tmpPaymanet["TYPE"] = new List<byte>((List<byte>)_tmpPaymanet["TYPE"]);

                    bool overrideBuyerCash = !(_allProfiles.Count == 1 && appIsLegal && !pMethod.Autoclose);

                    if (overrideBuyerCash && _allProfiles.IndexOf(profileKey) + 1 < _allProfiles.Count)
                    {
                        _tmpPaymanet["SUMA"] = _realSUMA;
                        ((List<double>)_tmpPaymanet["CASHLIST"])[0] = _realSUMA;
                        _tmpPaymanet["REST"] = 0.0;

                        // removing these values from original payment
                        _orgPaymanet["SUMA"] = MathLib.GetRoundedMoney(MathLib.GetDouble(_orgPaymanet["SUMA"]) - _realSUMA);
                        ((List<double>)_orgPaymanet["CASHLIST"])[0] = MathLib.GetRoundedMoney(MathLib.GetDouble(((List<double>)_orgPaymanet["CASHLIST"])[0]) - _realSUMA);
                    }

                    profileResult[CoreConst.PAYMENT] = _tmpPaymanet;
                    profileResult[CoreConst.IS_LEGAL] = isLegalMode;


                    localData = new object[8];
                    chqNom = string.Empty;


                    localData[0] = clientID == string.Empty ? ConfigManager.Instance.CommonConfiguration.APP_ClientID : clientID;
                    localData[1] = _discCommonPercent;
                    localData[2] = _realSUMA;
                    localData[3] = _taxSUMA;
                    localData[4] = nakladna;
                    localData[5] = retriveChq;
                    localData[6] = _fl_useTotDisc;
                    
                    //winapi.Funcs.OutputDebugString("B");
                    if (appIsLegal)
                    {
                        global::components.Components.WinApi.Com_WinApi.OutputDebugString("is legal cheque for profile " + currentProfileKey + " and legal profile is " + ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID);
                        try
                        {
                            if (retriveChq)
                                Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_PayMoney", _cheque, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, _fl_useTotDisc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);
                            else
                                Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_Sale", _cheque, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, _fl_useTotDisc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);

                            if (_fl_useTotDisc && _discCommonPercent != 0.0)
                            {
                                Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction(
                                    "FP_Discount",
                                     new object[] {
                                     (byte)2,
                                     _discCommonPercent, 
                                     ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, 
                                     string.Empty
                                 }
                                );
                            }

                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_ResetOrder");
                            }
                            catch { }

                            CoreLib.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            MMessageBoxEx.Show(this.chequeDGV, "Помилка під час продажу товарів" + "\r\n" + ex.Message,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // try to close cheque
                        try
                        {
                            if (lastPayment >= pMethod.Type.Count)
                                lastPayment = 0;

                            for (int i = lastPayment; i < pMethod.Type.Count; i++)
                            {
                                Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_Payment", pMethod.Type[i], ((List<double>)_tmpPaymanet["CASHLIST"])[i], overrideBuyerCash, retriveChq);
                                lastPayment++;
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_ResetOrder");
                            }
                            catch { }

                            CoreLib.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            MMessageBoxEx.Show(this.chequeDGV, "Помилка під час закриття чеку" + "\r\n" + ex.Message,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }


                        // try to get last colsed order number
                        try
                        {
                            chqNom = Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_LastChqNo", retriveChq).ToString();
                        }
                        catch (Exception ex)
                        {
                            CoreLib.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            uiWndChqNomRequest chqR = new uiWndChqNomRequest();

                            bool customOrderNo = false;
                            if (chqR.ShowDialog(this.chequeDGV) == System.Windows.Forms.DialogResult.Yes)
                            {
                                customOrderNo = true;
                                chqNom = chqR.ChequeNumber.ToString();
                            }

                            chqR.Dispose();

                            if (!customOrderNo)
                                return;
                        }

                        // try to get Z-report number
                        try
                        {
                            localData[7] = Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_LastZRepNo", retriveChq);
                        }
                        catch (Exception ex)
                        {
                            MMessageBoxEx.Show(this.chequeDGV, "Не вдається отримати номер Z-звіту" + "\r\n" + ex.Message,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            CoreLib.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                        }
                       
                        try
                        {
                            Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_OpenBox");
                        }
                        catch (Exception ex)
                        {
                            MMessageBoxEx.Show(this.chequeDGV, "Помилка відкриття грошової скриньки" + "\r\n" + ex.Message,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            CoreLib.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                        }
                    }

                    lastPayment = 0;
                    chqNom = DataWorkCheque.SaveCheque(_cheque, localData, pMethod.Type[0], chqNom);
                    profileResult[CoreConst.ORDER_NO] = chqNom;

                    DataWorkShared.SetOrderProperty(_cheque, CoreConst.PAYMENT, _tmpPaymanet);
                    DataWorkShared.SetOrderProperty(_cheque, CoreConst.ORDER_NO, chqNom);

                    if (appIsLegal && UserConfig.Properties[10])
                    {
                        DataWorkOutput.Print(Enums.PrinterType.OrderLegal, this.GetProfileOrder(profileKey));
                    }

                    if (!appIsLegal && UserConfig.Properties[11])
                    {
                        DataWorkOutput.Print(Enums.PrinterType.OrderNormal, this.GetProfileOrder(profileKey));
                    }

                    fullResult.Add(profileResult);
                    chqNumbers.Add(chqNom);
                    chqNumbersFull.Add(appIsLegal ? chqNom : 'N' + chqNom);

                    RowsRemoved_MyEvent_profile(profileKey);
                }
                catch { generalError = true; }

                if (generalError)
                    break;
            }

            if (skippedProfiles == ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Count)
            {
                MMessageBox.Show(this.chequeDGV, "Немає товарів для усіх профілів. Обновіть фільтри профілів.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //UpdateSumInfo_profile(currentProfileKey, false);
                return;
            }


            if (generalError)
            {
                MMessageBox.Show(this.chequeDGV, "Виникла помилка під час збереження частини чеку.\r\nСпробуйте ще раз закрити чек", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateSumInfo_profile(currentProfileKey, false);
                return;
            }
            if (DataWorkShared.ExtractBillProperty(dtCopy, CoreConst.OID, string.Empty) != string.Empty)
            {
                //CoreLib.LockBill(this.PD_Order.Copy(), chqNom);
                if (ConfigManager.Instance.CommonConfiguration.Content_Bills_KeepAliveAfterCheque)
                {
                    //string billChqNumFmt = string.Empty;
                    //for (int cqnc = 0; cqnc < chqNumbers.Count; cqnc++)
                    //    billChqNumFmt += "{" + cqnc + "},";
                    //billChqNumFmt = billChqNumFmt.TrimEnd(',');
                    //DataWorkBill.LockBill(dtCopy.Copy(), string.Format("[" + billChqNumFmt + "]", chqNumbers.ToArray()));
                    DataTable billData = dtCopy.Copy();
                    DataWorkShared.SetOrderProperty(billData, CoreConst.PAYMENT, _orgPaymanet);
                    DataWorkBill.LockBill(billData, string.Join(" | ", chqNumbersFull.ToArray()));
                }
                else
                    DataWorkBill.BillDelete(dtCopy);

                //Cheque.ExtendedProperties["FXNO"] = chqNom;

                //File.Delete(Cheque.ExtendedProperties["PATH"].ToString());
            }
            
            string closedInfo = string.Empty;
            if (chqNumbers.Count > 1)
                closedInfo = string.Format("{0} {1} ", "Закриті чеки №", string.Join(",", chqNumbers.ToArray()));
            else
                closedInfo = string.Format("{0} {1} ", "Закритий чек №", string.Join(string.Empty, chqNumbers.ToArray()));
            if (!string.IsNullOrEmpty(DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.BILL_NO, string.Empty).ToString()))
                closedInfo += string.Format("{0} {1} ", "з рахунку №", DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.BILL_NO, string.Empty));

            RowsRemoved_MyEvent(false, true, true);
            CashLbl.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_RestFontColor;
            CashLbl.Font = ConfigManager.Instance.CommonConfiguration.STYLE_RestFont;
            CashLbl.Text = string.Format("{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", pMethod.Rest);
            chequeInfoLabel.Text = string.Format("{0} {1:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", "залишок з суми", pMethod.CashSum);
            this.addBillInfo.Text = string.Empty;
            addChequeInfo.Text = closedInfo;
            
            if (isLegalMode && ConfigManager.Instance.CommonConfiguration.APP_ShowInfoOnIndicator)
                try
                {
                    string[] lines = new string[2];
                    lines[0] = string.Format("{0} : {1:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", "Гроші", pMethod.CashSum);
                    lines[1] = string.Format("{0} : {1}", "Здача", CashLbl.Text);
                    bool[] show = new bool[] { true, true };
                    Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_SendCustomer", lines, show);
                }
                catch { }




            //winapi.Funcs.OutputDebugString("E");
        }//Make exceptions

        private void CloseCheque_single(bool isLegalMode)//1_msg//lbl7
        {
            uiWndPayment pMethod = new uiWndPayment(realSUMA);
            pMethod.ShowDialog();
            pMethod.Dispose();
            //winapi.Funcs.OutputDebugString("A");

            if (pMethod.DialogResult != DialogResult.OK)
                return;

            object[] localData = new object[8];
            string chqNom = string.Empty;

            switch (pMethod.Type[0])
            {
                case 0: { digitalPanel.BackgroundImage = Properties.Resources.payment_card; break; }
                case 1: { digitalPanel.BackgroundImage = Properties.Resources.payment_credit; break; }
                case 2: { digitalPanel.BackgroundImage = Properties.Resources.payment_cheque; break; }
                case 3: { digitalPanel.BackgroundImage = Properties.Resources.payment_cash; break; }
            }

            if (UserConfig.Properties[4] &&
                DialogResult.Yes == MMessageBoxEx.Show(this.chequeDGV, "Видати накладну згідно цього чеку ?", Application.ProductName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                nakladna = true;
                CashLbl.Image = Properties.Resources.naklad;
            }

            localData[0] = clientID == string.Empty ? ConfigManager.Instance.CommonConfiguration.APP_ClientID : clientID;
            localData[1] = discCommonPercent;
            localData[2] = realSUMA;
            localData[3] = taxSUMA;
            localData[4] = nakladna;
            localData[5] = retriveChq;
            localData[6] = _fl_useTotDisc;

            //winapi.Funcs.OutputDebugString("B");
            if (isLegalMode)
            {
                try
                {
                    if (retriveChq)
                        Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_PayMoney", Cheque, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, _fl_useTotDisc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);
                    else
                        Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_Sale", Cheque, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, _fl_useTotDisc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);

                    if (_fl_useTotDisc && discCommonPercent != 0.0)
                    {
                        //if ((discount[0] + discount[1]) != 0 || constDiscount != 0)
                        //OnDeactivate(EventArgs.Empty);
                        //double[] valueDISC = new double[] {
                        //    discount[0],
                        //    cash_discount[0],
                        //    discount[1],
                        //    cash_discount[1],
                        //    constDiscount};
                        //byte[] types = new byte[] { 2, 3, 2, 3, 2 };

                        //for (int i = 0; i < valueDISC.Length; i++)
                        //    if (valueDISC[i] != 0.0)
                        ; Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction(
                              "FP_Discount",
                             new object[] { 
                                    (byte)2/*types[i]*/, 
                                    /*valueDISC[i]*/discCommonPercent/*(discount[0] + discount[1]) == 0 ? constDiscount : (discount[0] + discount[1])*/,
                                    ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, "" });
                    }

                    if (lastPayment >= pMethod.Type.Count)
                        lastPayment = 0;

                    for (int i = lastPayment; i < pMethod.Type.Count; i++)
                    {
                        Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_Payment", pMethod.Type[i], pMethod.ItemsCash[i], pMethod.Autoclose);
                        lastPayment++;
                    }


                    chqNom = Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_LastChqNo", retriveChq).ToString();
                    localData[7] = Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_LastZRepNo", retriveChq);
                }
                catch (Exception ex)
                {
                    CoreLib.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);

                    MMessageBoxEx.Show(this.chequeDGV, "Помилка під час закриття чеку" + "\r\n" + ex.Message,
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    bool customOrderNo = false;
                    uiWndChqNomRequest chqR = new uiWndChqNomRequest();
                    if (chqR.ShowDialog(this.chequeDGV) == System.Windows.Forms.DialogResult.Yes)
                    {
                        customOrderNo = true;
                        chqNom = chqR.ChequeNumber.ToString();
                    }
                    chqR.Dispose();

                    if (!customOrderNo)
                        return;

                    /*
                    MMessageBox.Show(this, "Натисніть ОК та повторно закрийте чек." + "\r\n",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    */

                    //Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("ResetOrder");

                    //ChqNomRequest cnr = new ChqNomRequest();
                    //cnr.ShowDialog();
                    //cnr.Dispose();
                    //if (cnr.DialogResult != DialogResult.Yes)

                    //chqNom = cnr.ChequeNumber.ToString();
                }

                try
                {
                    Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_OpenBox");
                }
                catch (Exception ex)
                {
                    MMessageBoxEx.Show(this.chequeDGV, "Помилка відкриття грошової скриньки" + "\r\n" + ex.Message,
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CoreLib.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                }
            }

            lastPayment = 0;

            //winapi.Funcs.OutputDebugString("C");
            chqNom = DataWorkCheque.SaveCheque(Cheque, localData, pMethod.Type[0], chqNom);

            //object[] printerData = this.CreatePrinterData(fix, pMethod.PaymentInfo);
            //Dictionary<string, object> printerData = this.PD_Order;
            // add additional information
            //DataWorkShared.SetOrderProperty(this.PD_Order, CoreConst.ORDER_NO, chqNom);
            DataWorkShared.SetOrderProperty(this.Cheque, CoreConst.PAYMENT, pMethod.PaymentInfo);
            DataWorkShared.SetOrderProperty(this.Cheque, CoreConst.ORDER_NO, isLegalMode ? chqNom : 'N' + chqNom);
            // 1 this.PD_EmptyOrder.ExtendedProperties["ORDER_NO"] = chqNom;
            // 1 this.PD_EmptyOrder.ExtendedProperties["PAYMENT"] = pMethod.PaymentInfo;
            //printerData["IS_LEGAL"] = isLegalMode;
            //Printing
            //winapi.Funcs.OutputDebugString("D");
            if (isLegalMode && UserConfig.Properties[10])
            {
                DataWorkOutput.Print(Enums.PrinterType.OrderLegal, this.PD_Order);
                //CoreLib.Print(printerData, "fix", 0);
            }
            if (!isLegalMode && UserConfig.Properties[11])
            {
                DataWorkOutput.Print(Enums.PrinterType.OrderNormal, this.PD_Order);
                //CoreLib.Print(printerData, "none", 0);
            }
            if (DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.OID, string.Empty) != string.Empty)
            {
                //CoreLib.LockBill(this.PD_Order.Copy(), chqNom);
                if (ConfigManager.Instance.CommonConfiguration.Content_Bills_KeepAliveAfterCheque)
                    DataWorkBill.LockBill(this.PD_Order.Copy(), isLegalMode ? chqNom : 'N' + chqNom);
                else
                   DataWorkBill.BillDelete(this.PD_Order);

                //Cheque.ExtendedProperties["FXNO"] = chqNom;

                //File.Delete(Cheque.ExtendedProperties["PATH"].ToString());
            }

            RowsRemoved_MyEvent(false, true, true);


            CashLbl.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_RestFontColor;
            CashLbl.Font = ConfigManager.Instance.CommonConfiguration.STYLE_RestFont;
            CashLbl.Text = string.Format("{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", pMethod.Rest);
            chequeInfoLabel.Text = string.Format("{0} {1:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", "залишок з суми", pMethod.CashSum);
            addChequeInfo.Text = string.Format("{0} {1} ", "Закритий чек №", chqNom);
            if (DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.BILL_NO, string.Empty) != string.Empty)
                addChequeInfo.Text += string.Format("{0} {1} ", "з рахунку №", DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.BILL_NO, string.Empty));
            this.addBillInfo.Text = string.Empty;


            if (isLegalMode && ConfigManager.Instance.CommonConfiguration.APP_ShowInfoOnIndicator)
                try
                {
                    string[] lines = new string[2];
                    lines[0] = string.Format("{0} : {1:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", "Гроші", pMethod.CashSum);
                    lines[1] = string.Format("{0} : {1}", "Здача", CashLbl.Text);
                    bool[] show = new bool[] { true, true };
                    Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_SendCustomer", lines, show);
                }
                catch { }

            //winapi.Funcs.OutputDebugString("E");
        }//Make exceptions
        /*
        private void Hook_OnChequeClose(Hashtable profileParam)
        {
            // param struct
            // ORDER_NO
            // IS_LEGAL
            // PAYMENT
            //     TYPE
            //     SUMA
            //     CASHLIST
            //     REST

            Dictionary<string, object> pMethod = (Dictionary<string, object>)profileParam[CoreConst.PAYMENT];
            Hashtable[] profileItems = (Hashtable[])profileParam["ITEMS"];
            List<string> chqNumbers = new List<string>();
            bool wasLegal = false;

            foreach (Hashtable param in profileItems)
            {
                chqNumbers.Add(param[CoreConst.ORDER_NO].ToString());
                RowsRemoved_MyEvent_profile(param["PROFILE_ID"]);
                if ((bool)param[CoreConst.IS_LEGAL])
                    wasLegal = true;
            }

            // foreach (Hashtable param in profileParam)
            //{


            CashLbl.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_RestFontColor;
            CashLbl.Font = ConfigManager.Instance.CommonConfiguration.STYLE_RestFont;
            CashLbl.Text = string.Format("{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", pMethod["REST"]);

            chequeInfoLabel.Text = string.Format("{0} {1:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", "залишок з суми", pMethod["SUMA"]);

            addChequeInfo.Text = string.Format("{0} {1} ", "Закриті чеки №", string.Join(",", chqNumbers.ToArray()));
            if (DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.BILL_NO, string.Empty) != string.Empty)
                addChequeInfo.Text += string.Format("{0} {1} ", "з рахунку №", DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.BILL_NO, string.Empty));

            this.addBillInfo.Text = string.Empty;

            RowsRemoved_MyEvent(false, true, true);

            if (wasLegal && ConfigManager.Instance.CommonConfiguration.APP_ShowInfoOnIndicator)
                try
                {
                    string[] lines = new string[2];
                    lines[0] = string.Format("{0} : {1:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", "Гроші", pMethod["SUMA"]);
                    lines[1] = string.Format("{0} : {1}", "Здача", CashLbl.Text);
                    bool[] show = new bool[] { true, true };
                    Program.AppPlugins.GetActive<ILegalPrinterDriver>().CallFunction("FP_SendCustomer", lines, show);
                }
                catch { }
            //}
        }
        */
        private void CloseCheque(bool isLegalMode)
        {
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
            {
                CloseCheque_profile(isLegalMode);
            }
            else
            {
                CloseCheque_single(isLegalMode);
                //this.Hook_OnChequeClose(closeResult);
            }
        }

        /// <summary>
        /// Custom method. Perform reset of discount variables
        /// </summary>
        private void ResetDiscount()
        {
            //discConstPercent = 0.0;
            discArrPercent[0] = 0.0;
            discArrPercent[1] = 0.0;
            discArrCash[0] = 0.0;
            discArrCash[1] = 0.0;
            discOnlyPercent = 0.0;
            discOnlyCash = 0.0;
            discCommonPercent = 0.0;
            discCommonCash = 0.0;
            clientPriceNo = 0;
            //discApplied = false;
            
            //відмінитиЗнижкунадбавкуToolStripMenuItem.Enabled = false;
            відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Без знижки/надбавки";
            
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                {
                    this.Discount[de.Key] = DataWorkShared.GetStandartDiscountInfoStructure2();
                    this.Summa[de.Key] = DataWorkShared.GetStandartCalculationInfoStructure2();
                }
            
            //addChequeInfo.Text = string.Empty;
            //UpdateSumDisplay(true, true);
        }//ok
        /// <summary>
        /// Custom method. Perform serching in data by barcode.
        /// </summary>
        /// <param name="barcode">Article barcode</param>
        /// <param name="showMsg">It true then after searching would shown message otherwise false</param>
        /// <returns>Return true when searching was succesful and data was found otherwise flase</returns>
        private bool BCSearcher(string barcode, bool showMsg)//3_msg
        {
            barcode = barcode.Trim();
            //winapi.WinAPI.OutputDebugString("BC=" + barcode + "____Ln=" + barcode.Length.ToString());
            bool allowToShow = false;//returned value
            bool allowBBC = true;

            DataTable sTable = Articles.Clone();
            DataRow[] dr = new DataRow[1];
            double weightOfArticle = ConfigManager.Instance.CommonConfiguration.APP_StartTotal;

            //search by weight-barcodes
            if (ConfigManager.Instance.CommonConfiguration.APP_WeightType == 1)
            {
                if (barcode.Length >= 12 && barcode.Substring(0, 2) == "20")
                {
                    weightOfArticle = MathLib.GetDouble(barcode.Substring(7, 5)) / 1000;
                    barcode = barcode.Substring(2, 5);
                }
            }

            // buyer BC resolver
            if (ConfigManager.Instance.CommonConfiguration.APP_BuyerBarCodeSource != 0)
            {
                allowBBC = false;
                if (barcode != null && barcode.Length > 0 && barcode[0] == 'C')
                {
                    barcode = barcode.Substring(1);
                    allowBBC = true;
                }
            }

            //search by cards of clients
            #region Using Discount for client 998
            if (UserConfig.Properties[15])
            {
                // Get custom client's card prefix
                string cliCardPrefix = ConfigManager.Instance.CommonConfiguration.Content_Cheques_CustomClientCardBC;
                bool customClientCard = false;
                if (cliCardPrefix.Length != 0 && cliCardPrefix.Length < barcode.Length &&
                    barcode.Substring(0, cliCardPrefix.Length) == cliCardPrefix)
                    customClientCard = true;

                if (allowBBC && barcode.Length > 5 && (barcode.Substring(0, 3) == "998" || customClientCard))
                {
                    //if (UserConfig.Properties[15] && barcode.Length > 5 && barcode.Substring(0, 3) == "998")
                    //{
                    dr = Cards.Select("CBC =\'" + barcode + "\'");

                    if (dr.Length != 0 && dr[0] != null)
                    {
                        if (discArrPercent[0] < (double)dr[0]["CDISC"] && discConstPercent < (double)dr[0]["CDISC"])
                        {
                            if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                                discArrPercent[0] = discArrPercent[1] = 0.0;
                            discArrPercent[0] = (double)dr[0]["CDISC"];
                        }
                        this.clientPriceNo = (int)dr[0]["CPRICENO"];
                        UpdateSumInfo(true);
                        clientID = (string)dr[0]["CID"];
                        SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, false);
                    }
                    else
                        MMessageBoxEx.Show(this.chequeDGV, "Немає клієнта з кодом" + " " + barcode, "Результат пошуку",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return allowToShow;
                }
            }
            #endregion
            //search by cards of buyers - not finish
            #region Using Discount for buyer 999
            if (UserConfig.Properties[13] && barcode.Length > 5 && barcode.Substring(0, 3) == "999")
            {
                double dec = (double)Convert.ToInt32(barcode.Substring(3, 5), 8);
                dec /= 100;
                dec = Math.Round(dec, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, MidpointRounding.AwayFromZero);

                if (dec <= 100)
                    if (discArrPercent[0] < dec && discConstPercent < dec)
                    {
                        if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                            discArrPercent[0] = discArrPercent[1] = 0.0;
                        discArrPercent[0] = dec;
                    }

                if (dec > 100 && (discArrPercent[0] == 0 || discConstPercent >= 0))
                {
                    dec -= 100;
                    dec = -Math.Round(dec, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, MidpointRounding.AwayFromZero);
                    if (discArrPercent[1] < dec || discConstPercent < dec)
                    {
                        if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                            discArrPercent[0] = discArrPercent[1] = 0.0;
                        discArrPercent[1] = dec;
                    }
                }

                UpdateSumInfo(true);
                SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, false);

                return allowToShow;
            }
            #endregion
            //search by barcodes of articles

            dr = Articles.Select("BC = \'" + barcode.Trim() + "\'");
            if (dr.Length == 0 && UserConfig.Properties[16])
            {
                dr = AltBC.Select("ABC = \'" + barcode + "\'");
                if (dr.Length != 0)
                {
                    string cmd = string.Empty;

                    for (int i = 0; i < dr.Length; i++)
                    {
                        cmd += "ID='" + dr[i]["AID"] + "'";
                        if (i + 1 < dr.Length)
                            cmd += " OR ";
                    }

                    DataRow[] rows = new DataRow[dr.Length];
                    try
                    {
                        rows = Articles.Select(cmd);
                    }
                    catch { }

                    dr = (DataRow[])rows.Clone();
                }
            }

            if (dr.Length == 0)
            {
                //MMessageBoxEx.Show(this.chequeDGV, "Немає товару з таким штрих-кодом");
                if (showMsg)
                    MMessageBoxEx.Show(this.chequeDGV, "Немає товару з таким штрих-кодом", Application.ProductName,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (currSrchType != ConfigManager.Instance.CommonConfiguration.APP_SearchType)
                    SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                else
                    SearchFilter(true, currSrchType, false);
                return allowToShow;
            }

            if (dr.Length == 1)
            {
                CoreLib.AddArticleToCheque(chequeDGV, articleDGV, dr[0], weightOfArticle, Articles);
                if (!UserConfig.Properties[22])
                    SearchFilter(true, currSrchType, true);
                allowToShow = false;
            }
            else
            {
                sTable.Clear();
                sTable.BeginLoadData();

                for (int i = 0; i < dr.Length; i++)
                    sTable.Rows.Add(dr[i].ItemArray);
                sTable.EndLoadData();

                articleDGV.DataSource = sTable;
                articleDGV.Select();
                allowToShow = true;
            }

            return allowToShow;
        }
        /// <summary>
        /// Custom method. Perform chnaging searching type
        /// </summary>
        /// <param name="saveSearchText">If true then text in field wouldn't cleared else it will cleared</param>
        /// <param name="SrchType">Type of search: 0- by name; 1- by code; 2- by barcode; Others type are not premitted.</param>
        /// <param name="close">Approve to close filetred data after searching. If false then filtered data will showing again</param>

        private delegate void CreateFunc();

        private void SearchFilter(bool saveSearchText, int SrchType, bool close)
        {
            if (close)
            {
                Com_WinApi.OutputDebugString("MainWnd --- SearchFilter - reseting data begin");
                articleDGV.DataSource = Articles;
                Com_WinApi.OutputDebugString("MainWnd --- SearchFilter - reseting data end");
            }

            if (!saveSearchText)
                SrchTbox.Text = string.Empty;

            // hide product panel when it was displayed automatically
            if (splitContainer1.Panel2.Tag != null)
            {
                вікноТоварівToolStripMenuItem.PerformClick();
                splitContainer1.Panel2.Tag = null;
                // skin sensor integration (show panel when some arts are hidden)
                if (this.сенсорToolStripMenuItem.Checked)
                    this.chequeContainer.Panel2Collapsed = false;
            }

            switch (SrchType)
            {
                case 0:
                    {
                        SrchTbox.BackColor = Color.FromArgb(255, 255, 192);
                        searchImage.BackColor = Color.FromArgb(255, 255, 192);
                        searchImage.BackgroundImage = Properties.Resources.by_name;
                        break;
                    }
                case 1:
                    {
                        SrchTbox.BackColor = Color.FromArgb(192, 255, 192);
                        searchImage.BackColor = Color.FromArgb(192, 255, 192);
                        searchImage.BackgroundImage = Properties.Resources.by_c;
                        break;
                    }
                case 2:
                    {
                        SrchTbox.BackColor = Color.FromArgb(255, 192, 192);
                        searchImage.BackColor = Color.FromArgb(255, 192, 192);
                        searchImage.BackgroundImage = Properties.Resources.by_bc;
                        break;
                    }
            }

            SrchTbox.Focus();
            SrchTbox.Select();
            SrchTbox.SelectAll();

            currSrchType = SrchType;
        }


        // shold be removed after dataContainer2

        /* general properties */

        public void SaveGUI()
        {
            ConfigManager.Instance.CommonConfiguration.STYLE_SplitterDistance = splitContainer1.SplitterDistance;
            ConfigManager.Instance.CommonConfiguration.STYLE_MainWndState = this.WindowState;
            ConfigManager.Instance.CommonConfiguration.STYLE_SplitOrient = this.splitContainer1.Orientation;
            ConfigManager.Instance.CommonConfiguration.STYLE_MainWndSize = this.Size;
            ConfigManager.Instance.CommonConfiguration.STYLE_MainWndPosition = this.Location;

            // sensor style
            ConfigManager.Instance.CommonConfiguration.skin_sensor_active = this.сенсорToolStripMenuItem.Checked;
            if (ConfigManager.Instance.CommonConfiguration.skin_sensor_active)
            {
                ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqnav = this.sensorPanel1.GetComponentVisiblity(SensorUgcPanel.SensorComponents.Scrolling);
                ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqopr = this.sensorPanel1.GetComponentVisiblity(SensorUgcPanel.SensorComponents.Operations);
                ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqsrch = this.sensorPanel1.GetComponentVisiblity(SensorUgcPanel.SensorComponents.Search);
                ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqbills = this.sensorPanel1.GetComponentVisiblity(SensorUgcPanel.SensorComponents.Additional);
                /*
                ConfigManager.Instance.CommonConfiguration.skin_sensor_com_artnav = !this.sensorDataPanel1.Container.Panel1Collapsed;
                ConfigManager.Instance.CommonConfiguration.skin_sensor_com_artscroll = this.sensorDataPanel1.Scroller.Visible;
                */

                ConfigManager.Instance.CommonConfiguration.skin_sensor_com_artnav = навігаціяТоварівToolStripMenuItem.Checked;
                ConfigManager.Instance.CommonConfiguration.skin_sensor_com_artscroll = переміщенняПоТоварахToolStripMenuItem.Checked;

                //ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chqmain = this.splitContainer_chequeControls.SplitterDistance;
                //ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chqcontrols = this.splitContainer_chequeControlContainer.SplitterDistance;

                ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_orient = (int)this.chequeContainer.Orientation;
                ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_artnav = (int)this.sensorDataPanel1.Container.SplitterDistance;

                switch (this.sensorPanel1.SensorType)
                {
                    case 50:
                        {
                            ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_h_50 = this.sensorPanel1.GetSplitterControl("h_50").SplitterDistance;
                            ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_v_50 = this.sensorPanel1.GetSplitterControl("v_50").SplitterDistance;
                            ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_50 = this.chequeContainer.SplitterDistance;
                            break;
                        }
                    default:
                    case 100:
                        {
                            ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_h_100 = this.sensorPanel1.GetSplitterControl("h_100").SplitterDistance;
                            ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_v_100 = this.sensorPanel1.GetSplitterControl("v_100").SplitterDistance;
                            ConfigManager.Instance.CommonConfiguration.skin_sensor_splitter_chq_100 = this.chequeContainer.SplitterDistance;
                            break;
                        }
                }
            }
        }

        public void UpdateDiscountValues(DataTable order)
        {
            this.currentSubUnit = (byte)order.ExtendedProperties[CoreConst.STORE_NO];
            try
            {
                this.clientID = order.ExtendedProperties[CoreConst.CLIENT_ID].ToString();
            }
            catch { }
            this.retriveChq = (bool)order.ExtendedProperties[CoreConst.IS_RET];

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
                 */
            }
        }

        public Hashtable PD_DiscountInfo
        {
            set { }
            get
            {
                Hashtable chqInfo = DataWorkShared.GetStandartDiscountInfoStructure2();
                //Якщо true то знижка чи надбавка діє на всі позиції(товари) чеку
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
                //Загальний коефіціент знижки в грошовому значенні
                chqInfo["DISC_APPLIED"] = this.discApplied;
                return chqInfo;
            }
        }

        public bool[] PD_Statements
        {
            set { }
            get { return new bool[3]; }
        }

        public DataTable PD_DEMO_Order
        {
            get
            {
                Dictionary<string, object> chqInfo = DataWorkShared.GetStandartOrderInfoStructure();
                // fill cheque structure
                //chqInfo["DATA"] = this.Cheque.Copy();
                chqInfo["STORE_NO"] = this.currentSubUnit;
                chqInfo["CLIENT_ID"] = this.clientID;
                chqInfo["IS_RET"] = this.retriveChq;
                chqInfo["IS_LEGAL"] = false;
                chqInfo["ORDER_NO"] = string.Empty;
                chqInfo["ORDER_SUMA"] = this.chqSUMA;
                chqInfo["ORDER_REAL_SUMA"] = this.realSUMA;
                chqInfo["TAX_SUMA"] = this.realSUMA;
                chqInfo["TAX_BILL"] = this.nakladna;
                chqInfo["DISCOUNT"] = this.PD_DiscountInfo;

                object bill = this.Cheque.ExtendedProperties["BILL"];
                DataWorkShared.UpdateExtendedProperties(this.Cheque, chqInfo);
                this.Cheque.ExtendedProperties["BILL"] = bill;
                /*
                chqInfo["BILL_NO"] = string.Empty;
                chqInfo["BILL_COMMENT"] = string.Empty;
                if (this.Cheque.ExtendedProperties.Contains("BILL")) {
                    //Номер рахунку
                    chqInfo["BILL_NO"] = Cheque.ExtendedProperties["NOM"];
                    //Коментр рахунку
                    chqInfo["BILL_COMMENT"] = Cheque.ExtendedProperties["CMT"];
                }*/
                return this.Cheque;
            }
        }

        public DataTable PD_Order
        {
            get
            {
                Dictionary<string, object> chqInfo = DataWorkShared.GetStandartOrderInfoStructure(this.Cheque);
                // fill cheque structure
                //chqInfo["DATA"] = this.Cheque.Copy();
                chqInfo[CoreConst.PAYDESK_NO] = ConfigManager.Instance.CommonConfiguration.APP_PayDesk;
                chqInfo["STORE_NO"] = this.currentSubUnit;
                chqInfo["CLIENT_ID"] = this.clientID;
                chqInfo["IS_RET"] = this.retriveChq;
                chqInfo["IS_LEGAL"] = false;
                //chqInfo["ORDER_NO"] = string.Empty;
                chqInfo["ORDER_SUMA"] = this.chqSUMA;
                chqInfo["ORDER_REAL_SUMA"] = this.realSUMA;
                chqInfo["TAX_SUMA"] = this.realSUMA;
                chqInfo["TAX_BILL"] = this.nakladna;
                chqInfo["DISCOUNT"] = this.PD_DiscountInfo;

                DataWorkShared.UpdateExtendedProperties(this.Cheque, chqInfo);
                /*
                chqInfo["BILL_NO"] = string.Empty;
                chqInfo["BILL_COMMENT"] = string.Empty;
                if (this.Cheque.ExtendedProperties.Contains("BILL")) {
                    //Номер рахунку
                    chqInfo["BILL_NO"] = Cheque.ExtendedProperties["NOM"];
                    //Коментр рахунку
                    chqInfo["BILL_COMMENT"] = Cheque.ExtendedProperties["CMT"];
                }*/
                return this.Cheque;
            }
        }

        public DataTable GetProfileOrder(object profileKey)
        {
            Hashtable _suma = (Hashtable)this.Summa[profileKey];
            Hashtable _discount = (Hashtable)this.Discount[profileKey];
            DataTable _cheque = this.Cheques.Tables[profileKey.ToString()];


            Dictionary<string, object> chqInfo = DataWorkShared.GetStandartOrderInfoStructure(_cheque);
            Hashtable discInfo = DataWorkShared.GetStandartDiscountInfoStructure2();


            /* initializing discount values */
            bool _discApplied = CoreLib.GetValue<bool>(_discount, CoreConst.DISC_APPLIED);
            double[] _discArrPercent = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_PERCENT);
            double[] _discArrCash = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_CASH);
            double _discConstPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_CONST_PERCENT);
            double _discOnlyPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_PERCENT);
            double _discOnlyCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_CASH);
            double _discCommonPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_PERCENT);
            double _discCommonCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_CASH);
            /* calculation items */
            double _realSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_REAL_SUMA);
            double _chqSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_CHEQUE_SUMA);
            double _taxSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CALC_TAX_SUMA);

            discInfo["DISC_ALL_ITEMS"] = this._fl_useTotDisc;
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
            chqInfo["IS_RET"] = this.retriveChq;
            chqInfo["IS_LEGAL"] = false;
            chqInfo["ORDER_SUMA"] = _chqSUMA;
            chqInfo["ORDER_REAL_SUMA"] = _realSUMA;
            chqInfo["TAX_SUMA"] = _realSUMA;
            chqInfo["TAX_BILL"] = this.nakladna;
            chqInfo["DISCOUNT"] = discInfo;

            DataWorkShared.UpdateExtendedProperties(_cheque, chqInfo);

            return _cheque;
        }

        private void CreateOrderStructure(DataTable dtOrder)
        {
            Dictionary<string, object> chqInfo = DataWorkShared.GetStandartOrderInfoStructure();
            DataWorkShared.AppendExtendedProperties(dtOrder, chqInfo, true);
        }

        private void splitContainer1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.splitContainer1.Orientation == Orientation.Vertical)
                this.splitContainer1.SplitterDistance = this.splitContainer1.Width / 2;
            else
                this.splitContainer1.SplitterDistance = this.splitContainer1.Height / 2;
        }

        private void Navigator_OnFilterChanged(string filter, EventArgs e)
        {
            if (filter.Length == 0)
                this.articleDGV.DataSource = this.Articles;
            else
            {
                DataRow[] dr = this.Articles.Select("ID Like '" + filter + "%'");
                DataTable sTable = this.Articles.Clone();
                sTable.Clear();
                sTable.BeginLoadData();
                for (int i = 0; i < dr.Length; i++)
                    sTable.Rows.Add(dr[i].ItemArray);
                sTable.EndLoadData();
                this.articleDGV.DataSource = sTable;
            }
            this.articleDGV.Select();
        }

        private void sensorPanel1_OnSensorButtonClicked(string buttonName, EventArgs e)
        {
            switch (buttonName)
            {
                case "up":
                    {
                        this.chequeDGV.Select();
                        SendKeys.SendWait("{UP}");
                        break;
                    }
                case "dn":
                    {
                        this.chequeDGV.Select();
                        SendKeys.SendWait("{DOWN}");
                        break;
                    }
                case "s_name":
                    {
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_F5), new IntPtr(0));
                        break;
                    }
                case "s_code":
                    {
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_F6), new IntPtr(0));
                        break;
                    }
                case "s_bcode":
                    {
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_F7), new IntPtr(0));
                        break;
                    }
                case "sub":
                    {
                        if (this.articleDGV.RowCount == 0)
                            break;
                        this.chequeDGV.Select();
                        if (chequeDGV.CurrentRow != null)
                        {
                            //DataRow[] article = Articles.Select("ID =" + chequeDGV.CurrentRow.Cells["ID"].Value.ToString());
                            DataRow[] article = Articles.Select("ID =\'" + chequeDGV.CurrentRow.Cells["ID"].Value.ToString() + "\'");
                            if (article != null && article.Length == 1)
                            {
                                CoreLib.AddArticleToCheque(chequeDGV, articleDGV, article[0], -ConfigManager.Instance.CommonConfiguration.APP_StartTotal, Articles);
                                SearchFilter(true, this.currSrchType, false);
                            }
                        }
                        break;
                    }
                case "add":
                    {
                        if (this.articleDGV.RowCount == 0)
                            break;
                        /*
                        if (this.articleDGV.Visible)
                        {
                            Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_Enter), new IntPtr(0));
                            break;
                        }
                        
                        if (this.currSrchType == 2 && this.SrchTbox.Text != string.Empty)
                        {
                            Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_F7), new IntPtr(0));
                            Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_Enter), new IntPtr(0));
                            break;
                        }

                        if (this.chequeDGV.Visible)
                        {
                            this.chequeDGV.Select();
                            Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_Enter), new IntPtr(0));
                            break;
                        }*/

                        this.chequeDGV.Select();
                        if (chequeDGV.CurrentRow != null)
                        {
                            DataRow[] article = Articles.Select("ID =\'" + chequeDGV.CurrentRow.Cells["ID"].Value.ToString() + "\'");
                            if (article != null && article.Length == 1)
                            {
                                CoreLib.AddArticleToCheque(chequeDGV, articleDGV, article[0], ConfigManager.Instance.CommonConfiguration.APP_StartTotal, Articles, false, false);
                                SearchFilter(true, this.currSrchType, false);
                            }
                        }
                        break;
                    }
                case "edit":
                    {
                        this.chequeDGV.Select();
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_Enter), new IntPtr(0));
                        break;
                    }
                case "dell":
                    {
                        this.chequeDGV.Select();
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_CtrlDel), new IntPtr(0x100));
                        break;
                    }
                case "dellall":
                    {
                        this.chequeDGV.Select();
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_CtrlShiftDel), new IntPtr(0x100));
                        break;
                    }

                case "sale":
                    {
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_CtrlEnter), new IntPtr(0));
                        break;
                    }
                case "nsale":
                    {
                        Com_WinApi.SendMessage(this.Handle, (uint)CoreLib.MyMsgs.WM_HOTKEY, new IntPtr((int)CoreLib.MyHotKeys.HK_CtrlShiftEnter), new IntPtr(0));
                        break;
                    }

                case "bills":
                    {
                        this.всіРахункиToolStripMenuItem.PerformClick();
                        break;
                    }
                case "billsave":
                    {
                        this.зберегтиРахунокToolStripMenuItem.PerformClick();
                        break;
                    }
                case "billsaveprint":
                    {
                        this.зберегтиІДрукуватиToolStripMenuItem.PerformClick();
                        break;
                    }
                case "billsaveprintclose":
                    {
                        this.ToolStripMenu_Bills_SavePrintAndClose.PerformClick();
                        break;
                    }
                case "billsaveclose":
                    {
                        this.зберегтиІЗакритиToolStripMenuItem.PerformClick();
                        break;
                    }
                case "billchangecomment":
                    {
                        this.змінитиКоментарToolStripMenuItem.PerformClick();
                        break;
                    }
            }
        }
        
        private string readedBuyerBarCode = string.Empty;
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (serialPort1.BytesToRead >= ConfigManager.Instance.CommonConfiguration.APP_BuyerBarCodeMinLen)
            {
                readedBuyerBarCode = serialPort1.ReadExisting().Trim('%', '?', '\r', '\n');
            }
        }

        private void timer_buyer_ready_Tick(object sender, EventArgs e)
        {
            if (readedBuyerBarCode.Length != 0)
            {
                string dd = "C" + readedBuyerBarCode;
                readedBuyerBarCode = "";
                global::components.Components.WinApi.Com_WinApi.OutputDebugString("Received data = " + dd);
                BCSearcher(dd, true);
                try
                {
                    відмінитиЗнижкунадбавкуToolStripMenuItem.Enabled = true;
                    відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку";
                }
                catch { }
            }
        }

        private bool getAdminAccess(params int[] userAccessFnIndex)
        {
            bool generalResult = true;
            for (int i = 0, len = userAccessFnIndex.Length; i < len; i++)
                generalResult &= getAdminAccess(userAccessFnIndex[i], false);
            return generalResult;
        }

        private bool getAdminAccess(int userAccessFnIndex)
        {
            return getAdminAccess(userAccessFnIndex, true);
        }

        private bool getAdminAccess(int userAccessFnIndex, bool requestAdminWindow)
        {
            // admin has access to all settings
            if (requestAdminWindow && ADMIN_STATE)
                return true;

            // check whether user has access
            if (userAccessFnIndex >= 0)
            {
                bool userHasAccess = UserConfig.Properties[userAccessFnIndex];
                if (userHasAccess)
                    return true;
            }

            // show admin window to unlock requested function
            if (requestAdminWindow)
                return admin.ShowDialog() == DialogResult.OK;

            return false;
        }

    }
}