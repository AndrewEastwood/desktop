using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
/* CUSTOM REFERENCES */
//0using mdcore;
//0using winapi;
//0using PluginModule;
//0using ;
//0using mdcore.Config;
//0using ;
//0using mdcore.Components.Objects;
//0using mdcore.Components.UI;
using PayDesk.Components.UI.wndBills;
//0using mdcore.Components;
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
using components.UI.Controls;
using components.Shared.Enums;
using System.Threading;
using driver.Components.Profiles;
//using comport;
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
        private uiWndAdmin admin = new uiWndAdmin();
        // Main Data
        // *** private DataTable this.profileCnt.Default.Order;
        // *** private DataTable this.profileCnt.Default.Products;
        // *** private DataTable this.profileCnt.Default.Alternative;
        // *** private DataTable this.profileCnt.Default.DiscountCards;
        private DataSet ImportedData = new DataSet();
        // Scanner Data
        private string chararray;
        private DateTime lastInputChar;
        // Order Data
        // *** private double chqSUMA;
        // *** private double realSUMA;
        // *** private double taxSUMA;

        private string clientID;
        private byte currentSubUnit;
        private int currSrchType;
        private int lastPayment;

        // Discount Data
        // *** private double this.profileCnt.Default.getPropertyValue<double>(CoreConst.DISC_CONST_PERCENT);
        private int clientPriceNo;
        /*
         * private double[] this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT) = new double[2] { 0.0, 0.0 };
        private double[] this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH) = new double[2] { 0.0, 0.0 };
        private double discOnlyPercent = 0.0;
        private double discOnlyCash = 0.0;
        private double discCommonPercent = 0.0;
        private double discCommonCash = 0.0;
        private bool discApplied = false;

        private DataSet Data = new DataSet();
        private Hashtable Discount = new Hashtable();
        private Hashtable Summa = new Hashtable();
        private DataSet Cheques = new DataSet();
        */
        private Dictionary<string, bool> State = new Dictionary<string, bool>();

        // Statements
        //private bool _fl_taxDocRequired = false;
        private bool _fl_singleMode = false;
        //private bool _fl_isReturnCheque = false;
        //private bool _fl_isInvenCheque = false;
        //private bool _fl_artUpdated = false;
        private bool _fl_canUpdate = false;
        //private bool _fl_onlyUpdate = false;
        private bool _fl_adminMode = false;
        private bool _fl_menuIsActive = false;
        //private bool _fl_subUnitChanged = false;
        //private bool _fl_useTotDisc = true;
        private bool _fl_isOk = false;
        //private bool _fl_modeChanged = false;
        private bool _fl_importIsRunning = false;
        // inner data
        private string readedBuyerBarCode = string.Empty;
        /* new data */
        //components.Components.DataContainer.DataContainer dataContainer2;

        /* profiles 2.0  */
        private ProfilesContainer profileCnt;

        private System.Windows.Forms.Timer timerDataImportSynchronizer;

        /// <summary>
        /// Application's Constructor
        /// </summary>
        public uiWndMain()
        {
            InitializeComponent();

            timerDataImportSynchronizer = new System.Windows.Forms.Timer();
            timerDataImportSynchronizer.Interval = 50000;
            timerDataImportSynchronizer.Tick += new EventHandler(timerDataImportSynchronizer_Tick);
            
            profileCnt = new ProfilesContainer();
            profileCnt.onSubUnitChanged += new SuibUnitChangedEventHandler(profileCnt_onSubUnitChanged);
            profileCnt.onUpdateRequired += new UpdateRequiredEventHandler(profileCnt_onUpdateRequired);
            profileCnt.onDataUpdated += new DataUpdatedEventHandler(profileCnt_onDataUpdated);
            profileCnt.onDataUnchanged += new DataUnchangedEventHandler(profileCnt_onDataUnchanged);
            //profileCnt.onCashChanged += new CashChangedEventHandler(profileCnt_onCashChanged);
            profileCnt.onProfileCommandReceived += new ProfileCommandReceivedEventHandler(profileCnt_onProfileCommandReceived);

            // setup order data row handlers
            //profileCnt.Default.DataOrder.RowDeleted += new DataRowChangeEventHandler(Order_RowDeleted);
            //profileCnt.Default.DataOrder.RowChanged += new DataRowChangeEventHandler(Order_RowChanged);
        }

        private void profileCnt_onProfileCommandReceived(AppProfile sender, string command, EventArgs e)
        {
            switch (command)
            {
                case "pu_reset_discount":
                    {
                        // move it into UpdateGUI
                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Без знижки/надбавки";
                        // 
                        UpdateGUI(uiComponents.ControlsType2 | uiComponents.InformersAll);
                        break;
                    }
                case "pu_reset_cash":
                    {
                        // *** UpdateSumDisplay();
                        UpdateGUI(uiComponents.ControlsType2 | uiComponents.InformersAll, new Hashtable() {
                            {"RESET_CASH_INDICATOR", true}
                        });
                        break;
                    }
                case "pu_refresh_cash":
                    {
                        // *** UpdateSumDisplay();
                        UpdateGUI(uiComponents.ControlsType2 | uiComponents.InformersType2);
                        break;
                    }
                case "pu_order_item_changed":
                    {
                        // *** UpdateSumDisplay();
                        UpdateGUI(uiComponents.ControlsType2);
                        break;
                    }
                case "pu_order_item_removed":
                    {
                        // **** UpdateSumDisplay();
                        UpdateGUI(uiComponents.ControlsType2 | uiComponents.InformersType2);
                        break;
                    }
                case "pu_order_cleared":
                    {
                        // *** RefershMenus();
                        if (this.profileCnt.triggerReturnCheque)
                            this.profileCnt.triggerReturnCheque = false; // /**** чекПоверненняToolStripMenuItem.PerformClick();
                        //winapi.Funcs.OutputDebugString("3");
                        // ??? if (resetSrchFilter)
                        /*****if (true)
                            SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                        else
                            SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, false);
                        */
                        // --- this.profileCnt.refresh(true);
                        // handle reset search param
                        UpdateGUI(uiComponents.ControlsType1 | uiComponents.InformersType2 | uiComponents.MenuAll, new Hashtable() {
                            {"CLOSE",true},
                            {"STYPE", ConfigManager.Instance.CommonConfiguration.APP_SearchType},
                            {"SAVESRCH", false},
                            {"RESET_CASH_INDICATOR", true}
                        });

                        // *** RefreshChequeInformer(false);
                        // *** UpdateSumDisplay(/*false*/);
                        // *********** latest and from the top : UpdateGUI(uiComponents.ControlsType2 | uiComponents.InformersType2);

                        break;
                    }
            }
        }

        private void profileCnt_onDataUnchanged(object sender, EventArgs e)
        {

        }

        private void profileCnt_onDataUpdated(object sender, EventArgs e)
        {
            // *** articleDGV.DataSource = profileCnt.Default.DataProducts;
            // if (this.profileCnt.triggerRunUpdateOnly)
            {
                MMessageBoxEx.Show(this.chequeDGV, "Були внесені зміни в базу товарів", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            /* device status */
            if (Program.AppPlugins.IsActive(PluginType.FPDriver))
            {
                try
                {
                    bool status = (bool)Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_SetCashier", ConfigManager.Instance.CommonConfiguration.APP_PayDesk, UserConfig.UserFpLogin, UserConfig.UserFpPassword, UserConfig.UserID);
                    if (status)
                        DDM_FPStatus.Image = Properties.Resources.ok;
                    else
                        DDM_FPStatus.Image = Properties.Resources.FpNotOk;
                }
                catch { DDM_FPStatus.Image = Properties.Resources.FpNotOk; }
            }
            else
                DDM_FPStatus.Image = Properties.Resources.FpNotOk;
            SrchTbox.Select();
            GC.Collect();
        }

        private void profileCnt_onUpdateRequired(object sender, EventArgs e)
        {
            FetchProductData(true, true, false);
        }

        private void profileCnt_onSubUnitChanged(object sender, EventArgs e)
        {

        }

        ~uiWndMain()
        {
            global::components.Components.SerialPort.Com_SerialPort.CloseAllPorts(true);
        }

        #region General Event Handlers
        /// <summary>
        /// Перевизначений метод для виконання операцій відновлення
        /// інтерфейсу та інших параметрів програми під час її завантаження
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // set parent
            admin.OwnerControlEx = this.chequeDGV;

            // set triggers
            this._fl_isOk = new Com_SecureRuntime().FullLoader();
            this._fl_adminMode = UserConfig.AdminState;

            // link data
            articleDGV.DataSource = profileCnt.Default.DataProducts;
            chequeDGV.DataSource = profileCnt.Default.DataOrder;

            // configure grids
            DataGridView[] grids = new DataGridView[] { chequeDGV, articleDGV };
            ViewLib.LoadGridsView(ref grids, splitContainer1.Orientation);

            // setup exchange folder
            if (ConfigManager.Instance.CommonConfiguration.Path_Exchnage == string.Empty)
            {
                MMessageBoxEx.Show("Вкажіть шлях до папки обміну", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
                folderBrowserDialog1.ShowDialog();
                ConfigManager.Instance.CommonConfiguration.Path_Exchnage = folderBrowserDialog1.SelectedPath;
                ConfigManager.SaveConfiguration();
            }

            //Set default type of search
            // SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, false);
            
            
            // * UpdateSumInfo(true);

            // setup additional devices
            configureAdditionalDevices();

            // UpdateMyControls();
            // UpdateGUI(uiComponents.All);
            UpdateGUI(uiComponents.All, new Hashtable() {
                {"CLOSE",true},
                {"STYPE", ConfigManager.Instance.CommonConfiguration.APP_SearchType},
                {"SAVESRCH", false}
            });


            profileCnt.refresh(false);

            // temporary refresh skins
            /*Com_WinApi.OutputDebugString("RefershStyles_Start");
            if (ConfigManager.Instance.CommonConfiguration.skin_sensor_active)
            {
                this.сенсорToolStripMenuItem.PerformClick();
            }*/

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
            // if (_fl_isInvenCheque)
            if (this.profileCnt.triggerInventCheque)
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
                        if (Program.AppPlugins.IsActive(PluginType.FPDriver))
                        {
                            try
                            {
                                Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_SetCashier", ConfigManager.Instance.CommonConfiguration.APP_PayDesk, UserConfig.UserLogin, UserConfig.UserPassword, string.Empty);
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

            //Com_WinApi.OutputDebugString("regHotKeys");
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
            //Com_WinApi.OutputDebugString("un_RegHotKeys");
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
                    // remove active order row
                    case 0x10:
                        #region CONTROL + DELETE
                        {
                            //if (this.profileCnt.Default.Order.ExtendedProperties.ContainsKey("BILL") && this.profileCnt.Default.Order.ExtendedProperties["BILL"] != null && bool.Parse(this.profileCnt.Default.Order.ExtendedProperties["LOCK"].ToString()))
                            if ((bool)DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }
                            
                            if (this.profileCnt.Default.DataOrder.Rows.Count == 0)
                                break;//r

                            if (!(_fl_adminMode || UserConfig.Properties[24]) && (DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_OWNER_NO, string.Empty).ToString() == string.Empty))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    break;//r

                            try
                            {
                                int index = chequeDGV.CurrentRow.Index;

                                if (DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_OID, string.Empty).ToString() != string.Empty)
                                {
                                    try
                                    {
                                        Dictionary<string, object[]> deletedRows = new Dictionary<string, object[]>();
                                        deletedRows = (Dictionary<string, object[]>)DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_DELETED_ROWS, deletedRows);
                                        //DataRow[] dRow = new DataRow[1] { };
                                        //this.profileCnt.Default.Order.Rows.CopyTo(dRow, index);
                                        if (!deletedRows.ContainsKey(this.profileCnt.Default.DataOrder.Rows[index]["C"].ToString()))
                                        {
                                            deletedRows.Add(this.profileCnt.Default.DataOrder.Rows[index]["C"].ToString(), this.profileCnt.Default.DataOrder.Rows[index].ItemArray);
                                            DataWorkShared.SetBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_DELETED_ROWS, deletedRows);
                                        }
                                    }
                                    catch { }
                                }


                                object profileKey = this.profileCnt.Default.DataOrder.Rows[index]["F"];
                                object productId = this.profileCnt.Default.DataOrder.Rows[index]["ID"];
                                object productOID = this.profileCnt.Default.DataOrder.Rows[index]["C"];

                                this.profileCnt.Default.DataOrder.Rows.RemoveAt(index);

                                try
                                {
                                    DataRow dProfileRow = this.profileCnt[profileKey].DataOrder.Rows.Find(productOID);
                                    int idxPR = this.profileCnt[profileKey].DataOrder.Rows.IndexOf(dProfileRow);
                                    this.profileCnt[profileKey].DataOrder.Rows.RemoveAt(idxPR);
                                }
                                catch { }

                                //AppFunc.OutputDebugString("k");
                                
                                /* !!!!!!!!!!!!!!!!!!!!!!!
                                if (m.LParam.ToInt32() == 0x100)
                                    RowsRemoved_MyEvent(true, false);
                                else
                                    RowsRemoved_MyEvent(true);

                                */

                                if (m.LParam.ToInt32() == 0x100)
                                {
                                    UpdateGUI(uiComponents.ControlsType2 | uiComponents.InformersType2, new Hashtable() { 
                                        {"UPDATE_CUSTOMER", true}
                                    });
                                }
                                else
                                {
                                    UpdateGUI(uiComponents.ControlsType2 | uiComponents.InformersType2, new Hashtable() { 
                                        {"UPDATE_CUSTOMER", true},
                                        {"CLOSE", true}
                                    });
                                }


                                index--;
                                if (index < 0)
                                    if (this.profileCnt.Default.DataOrder.Rows.Count != 0)
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
                    // remove all order rows
                    case 0x11:
                        #region CONTROL + SHIFT + DELETE
                        {
                            //if (this.profileCnt.Default.Order.ExtendedProperties.ContainsKey("BILL") && this.profileCnt.Default.Order.ExtendedProperties["BILL"] != null && bool.Parse(this.profileCnt.Default.Order.ExtendedProperties["LOCK"].ToString()))
                            if ((bool)DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }

                            if (this.profileCnt.Default.DataOrder.Rows.Count == 0)
                                break;//r

                            if (!(_fl_adminMode || UserConfig.Properties[24]))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    break;//r

                            if (DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_OID, string.Empty).ToString() != string.Empty)
                            {
                                try
                                {
                                    Dictionary<string, object[]> deletedRows = new Dictionary<string, object[]>();
                                    deletedRows = (Dictionary<string, object[]>)DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_DELETED_ROWS, deletedRows);
                                    for (int index = 0; index < this.profileCnt.Default.DataOrder.Rows.Count; index++)
                                    {
                                        if (!deletedRows.ContainsKey(this.profileCnt.Default.DataOrder.Rows[index]["C"].ToString()))
                                            deletedRows.Add(this.profileCnt.Default.DataOrder.Rows[index]["C"].ToString(), this.profileCnt.Default.DataOrder.Rows[index].ItemArray);
                                    }
                                    DataWorkShared.SetBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_DELETED_ROWS, deletedRows);

                                }
                                catch { }
                            }


                            this.profileCnt.Default.DataOrder.Rows.Clear();
                            /* !!!!!!!!!!!!!!!!!!!!!!!!!
                            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                                RowsRemoved_MyEvent_profile(de.Key);
                            */

                            /* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                            if (m.LParam.ToInt32() == 0x100)
                                RowsRemoved_MyEvent(true, false, false);
                            else
                                RowsRemoved_MyEvent(true, true, true);
                            
                             * 
                             * 
                             */ 
                            
                            break;
                        }
                        #endregion
                    // setup discout
                    case 0x12:
                        #region CONTROL + PageDown
                        {
                            if ((bool)DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }

                            if (this.profileCnt.triggerInventCheque)
                                return;

                            if (!(_fl_adminMode || UserConfig.Properties[3]))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    return;

                            double discSUMA = 0.0;
                            try
                            {
                                discSUMA = (double)this.profileCnt.Default.DataOrder.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                            }
                            catch { }
                            uiWndDiscountRequest d = new uiWndDiscountRequest(discSUMA, true);
                            // *** d.SetDiscount(ref this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT), ref this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH));
                            d.SetupDiscount(this.profileCnt.Default);
                            d.Dispose();

                            //if (this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[0] == 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[1] == 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0] == 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[1] == 0.0)
                            if (this.profileCnt.Default.customCashDiscountManualIsEmpty)
                                this.profileCnt.Default.customResetBlockDiscountAll(); // *** ResetDiscount();
                            else
                            {
                                відмінитиЗнижкунадбавкуToolStripMenuItem.Enabled = true;
                                if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                                    відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку";
                                else
                                {
                                    //if ((this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[0] != 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[1] != 0.0) || (this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0] != 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[1] != 0.0))
                                    if (this.profileCnt.Default.customCashDiscountManualOnlyEnabled)
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку і націнку";
                                    //if ((this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[0] == 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[1] != 0.0) || (this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0] == 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[1] != 0.0))
                                    if (this.profileCnt.Default.customCashDiscountManualExtraOnlyEnabled)
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати націнку";
                                    //if ((this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[0] != 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[1] == 0.0) || (this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0] != 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[1] == 0.0))
                                    if (this.profileCnt.Default.customCashDiscountManualSavingsOnlyEnabled)
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку";
                                }
                            }

                            // * UpdateSumInfo(true);
                            profileCnt.Default.refresh();
                            break;
                        }
                        #endregion
                    // setup extra %
                    case 0x13:
                        #region CONTROL + PageUp
                        {
                            if ((bool)DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }

                            if (this.profileCnt.triggerInventCheque)
                                return;

                            if (!(_fl_adminMode || UserConfig.Properties[3]))
                                if (admin.ShowDialog() != DialogResult.OK)
                                    return;

                            double discSUMA = 0;
                            try
                            {
                                discSUMA = (double)this.profileCnt.Default.DataOrder.Compute("Sum(SUM)", "USEDDISC = " + Boolean.TrueString);
                            }
                            catch { }
                            uiWndDiscountRequest d = new uiWndDiscountRequest(discSUMA, false);
                            //d.SetDiscount(ref this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT), ref this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH));

                            d.SetupDiscount(this.profileCnt.Default);
                            d.Dispose();

                            // ** if (this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[0] == 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[1] == 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0] == 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[1] == 0.0)
                            if (this.profileCnt.Default.customCashDiscountManualIsEmpty)
                                this.profileCnt.Default.customResetBlockDiscountAll();  // ** ResetDiscount();
                            else
                            {
                                відмінитиЗнижкунадбавкуToolStripMenuItem.Enabled = true;
                                if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                                    відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати націнку";
                                else
                                {
                                    if (this.profileCnt.Default.customCashDiscountSomeEnabled)
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку і націнку";
                                    if (this.profileCnt.Default.customCashDiscountManualExtraOnlyEnabled)
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати націнку";
                                    if (this.profileCnt.Default.customCashDiscountManualSavingsOnlyEnabled)
                                        відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Скасувати знижку";
                                }
                            }

                            // * UpdateSumInfo(true);
                            profileCnt.Default.refresh();
                            break;
                        }
                        #endregion
                    // discard discount
                    case 0x14:
                        #region SHIFT + DELETE
                        {
                            if ((bool)DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }

                            if (this.profileCnt.triggerInventCheque)
                                return;

                            // ***** ResetDiscount();
                            
                        this.profileCnt.Default.customResetBlockDiscountAll();

                            // * UpdateSumInfo(true);
                            profileCnt.Default.refresh();
                            addChequeInfo.Text = string.Empty;
                            break;
                        }
                        #endregion
                    // add product into the order
                    // search
                    case 0x15:
                        #region ENTER
                        {

                            if (global::components.Components.WinApi.Com_WinApi.InSendMessage())
                            {
                                if (ConfigManager.Instance.CommonConfiguration.APP_ScannerUseProgamMode)
                                    System.Threading.Thread.Sleep(ConfigManager.Instance.CommonConfiguration.APP_ScannerCharReadFrequency + 1);
                                global::components.Components.WinApi.Com_WinApi.ReplyMessage(new IntPtr(0x15));
                            }

                            //if (   this.profileCnt.Default.Order.ExtendedProperties.ContainsKey("BILL") && this.profileCnt.Default.Order.ExtendedProperties["BILL"] != null && bool.Parse(((Dictionary<string, object>)this.profileCnt.Default.Order.ExtendedProperties["BILL"])["IS_LOCKED"].ToString()))
                            if ((bool)DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_IS_LOCKED, false))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_NO) + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;//r
                            }
                            //for (int i = 0x10; i < 0x20; i++)
                            //AppFunc.UnregisterHotKey(this, i);

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
                                        // **** SearchFilter(false, 2, true);
                                        UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                            {"CLOSE",true},
                                            {"STYPE", 2},
                                            {"SAVESRCH", false}
                                        });
                                        SrchTbox.Text = chararray;
                                    }
                                    else
                                    {
                                        Com_WinApi.OutputDebugString("SEARCH: no ok. Using textbox.");
                                        string _sameText = SrchTbox.Text;
                                        // **** SearchFilter(false, 2, true);
                                        UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                            {"CLOSE",true},
                                            {"STYPE", 2},
                                            {"SAVESRCH", false}
                                        });
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
                                if (!(_fl_adminMode || UserConfig.Properties[24]) && (DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_OWNER_NO, string.Empty).ToString() == string.Empty))
                                    if (admin.ShowDialog(this) != DialogResult.OK)
                                        return;

                                DataRow dRow = this.profileCnt.Default.DataOrder.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                                Request req = new Request(dRow, MathLib.GetDouble(dRow["TOT"]));
                                req.UpdateRowSource(this.chequeDGV, this);
                                req.Dispose();
                                // * UpdateSumInfo(true);
                                profileCnt.Default.refresh();
                                break;//r
                            }

                            //Adding article to this.profileCnt.Default.Order
                            if (articleDGV.Focused && articleDGV.RowCount != 0)
                            {
                                DataRow article = this.profileCnt.Default.DataProducts.Rows.Find(articleDGV.CurrentRow.Cells["C"].Value);

                                if (!this._fl_isOk && this.profileCnt.Default.DataOrder.Rows.Count >= 3 && this.profileCnt.Default.DataOrder.Rows.Find(article["C"].ToString()) == null)
                                {
                                    MMessageBoxEx.Show(this.chequeDGV, "Ви не можете продавати більше позицій в демо-режимі", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    break;
                                }

                                if (article != null)
                                {
                                    CoreLib.AddArticleToCheque(chequeDGV, articleDGV, article, ConfigManager.Instance.CommonConfiguration.APP_StartTotal, this.profileCnt.Default.DataProducts);
                                    // *** SearchFilter(true, this.currSrchType, false);
                                    UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                        {"CLOSE",false},
                                        {"STYPE", this.currSrchType},
                                        {"SAVESRCH", true}
                                    });
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
                                DataTable sTable = this.profileCnt.Default.DataProducts.Clone();
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
                                                    // *** SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                                                    UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                                        {"CLOSE",true},
                                                        {"STYPE", ConfigManager.Instance.CommonConfiguration.APP_SearchType},
                                                        {"SAVESRCH", false}
                                                    });
                                                }

                                                #endregion
                                                break;
                                            }
                                        case 1:
                                            {
                                                #region by id
                                                try
                                                {
                                                    DataRow[] dr = this.profileCnt.Default.DataProducts.Select("ID Like \'" + SrchTbox.Text + "%\'");

                                                    if (dr.Length == 0)
                                                    {
                                                        MMessageBoxEx.Show(this.chequeDGV, "Нажаль нічого не вдалось знайти", "Результат пошуку", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                        // *** SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                                                        UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                                            {"CLOSE",true},
                                                            {"STYPE", ConfigManager.Instance.CommonConfiguration.APP_SearchType},
                                                            {"SAVESRCH", false}
                                                        });
                                                        break;
                                                    }
                                                    if (dr.Length == 1)
                                                    {
                                                        // **** SearchFilter(false, currSrchType, true);
                                                        UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                                            {"CLOSE",true},
                                                            {"STYPE", currSrchType},
                                                            {"SAVESRCH", false}
                                                        });
                                                        CoreLib.AddArticleToCheque(chequeDGV, articleDGV, dr[0], ConfigManager.Instance.CommonConfiguration.APP_StartTotal, this.profileCnt.Default.DataProducts);
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
                    // close normal order
                    case 0x16:
                        #region CONTROL + ENTER
                        {
                            if (this.profileCnt.triggerInventCheque || this.profileCnt.Default.DataOrder.Rows.Count == 0)
                                break;//r

                            if (!(_fl_adminMode || UserConfig.Properties[23]))
                            {
                                //if (admin.ShowDialog() != DialogResult.OK)
                                MMessageBoxEx.Show(this.chequeDGV, "Закриття чеку заблоковано", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;//r
                            }

                            // if we use legal printer
                            if (Program.AppPlugins.IsActive(PluginType.FPDriver))
                            {
                                // we close legal cheque
                                // ******* CloseCheque(true);
                                OrderClose(true);
                            }
                            else
                            {
                                // if we don't use legal printer and
                                // if we allow to close normal cheque or admin mode is active
                                if (UserConfig.Properties[6])
                                {
                                    // we close normal cheque
                                    // ****** CloseCheque(false);
                                    OrderClose(false);
                                }
                                else
                                    MMessageBoxEx.Show(this.chequeDGV, "Закриття чеку заблоковано", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            break;
                        }
                        #endregion
                    // close simple order
                    case 0x17:
                        #region CONTROL + SHIFT + ENTER
                        {
                            if (this.profileCnt.triggerInventCheque)
                                break;//r

                            if (this.profileCnt.Default.DataOrder.Rows.Count == 0 && UserConfig.Properties[12])
                            {
                                string nextChqNom = string.Empty;
                                object[] localData = new object[0];
                                if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                                {
                                    string info = "";
                                    foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                                    {
                                        localData = DataWorkCheque.NonFxChqsInfo(0, ref nextChqNom, int.Parse(de.Key.ToString()));
                                        info += "| " + string.Format("{3}: за {1} продано {0} чек(ів) на суму {2:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "} ", localData[0], localData[1], MathLib.GetDouble(localData[2].ToString()), ((Hashtable)de.Value)["NAME"]);
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

                            if (this.profileCnt.Default.DataOrder.Rows.Count == 0)// || !Program.Service.UseEKKR)
                                break;//r

                            if (!(_fl_adminMode || (UserConfig.Properties[23] && UserConfig.Properties[6])))
                            {
                                MMessageBoxEx.Show(this.chequeDGV, "Закриття чеку заблоковано", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }

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

                            // ****** CloseCheque(false);

                            OrderClose(false);
                            
                            break;
                        }
                        #endregion
                    // set search type (by name)
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
                            {
                                // *** SearchFilter(false, 0, true);
                                UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                    {"CLOSE",true},
                                    {"STYPE", 0},
                                    {"SAVESRCH", false}
                                });
                            }
                            else
                            {
                                SrchTbox.Focus();
                                SrchTbox.Select(SrchTbox.Text.Length, 0);
                            }
                            break;
                        }
                        #endregion
                    // set search type (by id)
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
                            {
                                // *** SearchFilter(false, 1, true);
                                UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                    {"CLOSE",true},
                                    {"STYPE", 1},
                                    {"SAVESRCH", false}
                                });
                            } 
                            else
                            {
                                SrchTbox.Focus();
                                SrchTbox.Select(SrchTbox.Text.Length, 0);
                            }
                            break;
                        }
                        #endregion
                    // set search type (by bc)
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
                            {
                                // *** SearchFilter(false, 2, true);
                                UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                    {"CLOSE",true},
                                    {"STYPE", 2},
                                    {"SAVESRCH", false}
                                });
                            } 
                            else
                            {
                                SrchTbox.Focus();
                                SrchTbox.Select(0, SrchTbox.Text.Length);
                            }
                            break;
                        }
                        #endregion
                    // bill number
                    case 0x1B:
                        #region F8
                        {
                            if (this.profileCnt.Default.Properties[CoreConst.BILL_NO] != null)
                                MMessageBoxEx.Show(this.chequeDGV, "Відкритий рахунок №" + " " + this.profileCnt.Default.Properties[CoreConst.BILL_NO], Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                        #endregion
                    // user info
                    case 0x1C:
                        #region F9
                        {
                            string infoText = string.Empty;
                            UserSchema us = new UserSchema();
                            infoText += UserConfig.UserID;
                            infoText += "\r\n\r\n--------------------------------------------------\r\n\r\n";
                            for (int i = 0; i < UserSchema.ITEMS_COUNT; i++)
                                infoText += us.SchemaItems[i] + " : " + (UserConfig.Properties[i] ? "Так" : "Ні") + "\r\n";
                            MMessageBoxEx.Show(infoText, UserConfig.UserID);
                            break;
                        }
                        #endregion
                    // reset search
                    // reset filter
                    // exit
                    case 0x1D:
                        #region ESCAPE
                        {
                            if (this.currSrchType != ConfigManager.Instance.CommonConfiguration.APP_SearchType ||
                                this.SrchTbox.Text.Length != 0 ||
                                this.profileCnt.Default.DataProducts.Rows.Count != this.articleDGV.RowCount)
                            {
                                // ** SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                                UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                    {"CLOSE",true},
                                    {"STYPE", ConfigManager.Instance.CommonConfiguration.APP_SearchType},
                                    {"SAVESRCH", false}
                                });
                                //this.sensorDataPanel1.Navigator.DisplayedCategoryFilter = "";
                                //this.Navigator_OnFilterChanged("", EventArgs.Empty);
                            }
                            else
                                this.Close();
                            break;
                        }
                        #endregion
                    // on/off tax doc
                    case 0x1E:
                        #region CONTROL + Q
                        {
                            if (this.profileCnt.triggerInventCheque)
                                break;//r

                            this.profileCnt.triggerTaxDocRequired = !this.profileCnt.triggerTaxDocRequired;

                            if (this.profileCnt.triggerTaxDocRequired)
                                CashLbl.Image = Properties.Resources.naklad;
                            else
                                CashLbl.Image = null;
                            break;
                        }
                        #endregion
                    // stop editing
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
                    this.timerExchangeImport_Tick(this.timerExchangeImport, EventArgs.Empty);
            }
        }
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
                        uiWndFiscalFunctions ff = new uiWndFiscalFunctions(Program.AppPlugins.GetActive<IFPDriver>().Name, Program.AppPlugins.GetActive<IFPDriver>().AllowedMethods);
                        try
                        {
                            if (ff.ShowDialog(this) == DialogResult.OK)
                                Program.AppPlugins.GetActive<IFPDriver>().CallFunction(ff.Function);
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
                        // *** this.timerExchangeImport_Tick(this.timerExchangeImport, EventArgs.Empty);
                        /*uiWndBaseChanges DBChanges = new uiWndBaseChanges();
                        if (DBChanges.ShowDialog() == DialogResult.OK)
                            timer1_Tick(timer1, EventArgs.Empty);
                        DBChanges.Dispose();
                        */
                        FetchProductData(true, true, false);
                        break;
                    }
                case "Administrator":
                    {
                        DialogResult rez = DialogResult.None;
                        if (_fl_adminMode)
                            rez = MMessageBoxEx.Show(this.chequeDGV, "Вийти з режиму адміністратора", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        else
                            rez = admin.ShowDialog();

                        switch (rez)
                        {
                            case DialogResult.OK:
                                _fl_adminMode = true;
                                break;
                            case DialogResult.Yes:
                                _fl_adminMode = false;
                                break;
                        }

                        UpdateGUI(uiComponents.MenuEnable | uiComponents.MenuTicks);

                        // **** RefershMenus();
                        break;
                    }
                case "UnitFilter":
                    {
                        if (_fl_adminMode || admin.ShowDialog() == DialogResult.OK)
                        {
                            uiWndUnitFilter fl = new uiWndUnitFilter(this.profileCnt.Default.DataProducts);
                            fl.ShowDialog();
                            fl.Dispose();
                        }
                        break;
                    }
                case "ChequeFormat":
                    {
                        uiWndDiscountSettings billRul = new uiWndDiscountSettings();
                        billRul.ShowDialog();
                        billRul.Dispose();


                        // *** ResetDiscount();
                        this.profileCnt.Default.customResetBlockDiscountAll();
                        
                        // * UpdateSumInfo(true);
                        profileCnt.refresh(true);
                        break;
                    }
                case "Invent":
                    {
                        if (this.profileCnt.Default.DataOrder.Rows.Count != 0 && !this.profileCnt.triggerInventCheque)
                            return;
                        this.profileCnt.triggerInventCheque = !this.profileCnt.triggerInventCheque;
                        if (this.profileCnt.triggerInventCheque)
                        {
                            DataTable dTable = new DataTable();
                            DataSet dSet = new DataSet();
                            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                            {
                                dSet = DataWorkCheque.OpenInvent(this.profileCnt.getDataAllProfiles(DataType.ORDER));


                                // ******* this.profileCnt.Default.Orders.Tables.Clear();
                                
                                
                                foreach (DataTable dt in dSet.Tables)
                                {
                                    // ****** this.profileCnt.Default.Orders.Tables.Add(dt.Copy());
                                    dTable.Merge(dt.Copy());
                                }
                            }
                            else
                                dTable = DataWorkCheque.OpenInvent();


                            if (dTable != null)
                            {
                                dTable.ExtendedProperties.Add("loading", true);
                                this.profileCnt.Default.DataOrder.Merge(dTable);
                                this.profileCnt.Default.DataOrder.ExtendedProperties.Remove("loading");
                                dTable.ExtendedProperties.Remove("loading");
                            }
                            else
                                this.profileCnt.triggerInventCheque = false;
                        }
                        else
                        {
                            DataWorkCheque.SaveInvent(this.profileCnt.Default.DataOrder, false, this.profileCnt.getDataAllProfiles(DataType.ORDER));
                            //this.profileCnt.Default.Order.Rows.Clear();
                            //UpdateSumDisplay(true, true);
                            // *** RowsRemoved_MyEvent(true, true, true);
                            this.profileCnt.Default.resetOrder();
                        }

                        // *** RefershMenus();
                        // *** RefreshChequeInformer(true);

                        UpdateGUI(uiComponents.MenuEnable | uiComponents.InformersType2 | uiComponents.InformersType3 | uiComponents.MenuTicks);
                        // ***** інвентаризаціяToolStripMenuItem.Checked = _fl_isInvenCheque;
                        break;
                    }
                case "RetriveCheque":
                    {
                        // *** _fl_isReturnCheque = !_fl_isReturnCheque;
                        // **** чекПоверненняToolStripMenuItem.Checked = _fl_isReturnCheque;
                        // *** RefreshChequeInformer(true);
                        this.profileCnt.triggerReturnCheque = !this.profileCnt.triggerReturnCheque;
                        UpdateGUI(uiComponents.InformersType2 | uiComponents.InformersType3 | uiComponents.MenuTicks);
                        break;
                    }
                case "Settings":
                    {
                        uiWndSettings set = new uiWndSettings();
                        if (set.ShowDialog() == DialogResult.OK)
                        {
                            // *** RefreshComponents(false);
                            // *** UpdateMyControls();
                            // ******* UpdateGUI(uiComponents.All);
                            // profile 2.0
                            this.profileCnt.refresh(true);

                            // *** SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                            UpdateGUI(uiComponents.All, new Hashtable() {
                                {"CLOSE",true},
                                {"STYPE", ConfigManager.Instance.CommonConfiguration.APP_SearchType},
                                {"SAVESRCH", false}
                            });
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
                        // **** RefreshWindowMenu();
                        UpdateGUI(uiComponents.MenuEnable);
                        break;
                    }
                case "ArticleWindow":
                    {
                        splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
                        ConfigManager.Instance.CommonConfiguration.STYLE_ArtSideCollapsed = !ConfigManager.Instance.CommonConfiguration.STYLE_ArtSideCollapsed;
                        // **** RefreshWindowMenu();
                        UpdateGUI(uiComponents.MenuEnable);
                        break;
                    }
                case "SensorType":
                    {

                        UpdateGUI(uiComponents.Widgets);
                        break;

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
                            this.sensorDataPanel1.setupDataContainer(this.articleDGV);
                            //this.articleDGV.BringToFront();

                            // RefreshComponents(true);
                            UpdateGUI(uiComponents.Widgets);
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
                            System.Diagnostics.Process.Start("vk.bat");
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
                        if (new uiWndAdmin().ShowDialog(this) != DialogResult.OK)
                            break;
                        uiWndBillManagercs bm = new uiWndBillManagercs();
                        bm.ShowDialog(this);
                        bm.Dispose();
                        break;
                    }
                case "ResetBill": // lock bill forever
                    {
                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.profileCnt.Default);
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

                        if (this.profileCnt.getDefaultProfileValue<bool>(CoreConst.BILL_IS_LOCKED))
                        {
                            MMessageBoxEx.Show(this.chequeDGV, "Поточний рахунок № " + this.profileCnt.Default.Properties[CoreConst.BILL_NO] + " надрукований клієнту.\r\nЗробіть з нього чек.",
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;//r
                        }
                        /*
                        if (new uiWndAdmin().ShowDialog(this) != DialogResult.OK)
                            break;
                        */

                        if (DialogResult.Yes != MMessageBoxEx.Show(this.chequeDGV, "Анулювати поточний рахунок № " + DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_NO),
                            Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            break;
                        //CoreLib.LockBill(this.profileCnt.Default.Order, "null");
                        string billNo = this.profileCnt.Default.Properties[CoreConst.BILL_NO].ToString();
                        DataWorkBill.LockBill(this.profileCnt.Default, "null");
                        // *** RowsRemoved_MyEvent(true, true, true);
                        // *** this.RefershMenus();
                        
                        this.profileCnt.Default.resetOrder();
                        this.UpdateGUI(uiComponents.MenuEnable);

                        //this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", billNo);
                        break;
                    }
                case "SaveAndPrintAndClose": // save, print and close
                    {
                        if (this.profileCnt.triggerInventCheque)
                            break;

                        // *** int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.PD_Order);
                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.profileCnt.Default);
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

                        uiWndBillSave bs = new uiWndBillSave(this.profileCnt.Default);
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
                                // *** this.RefershMenus();
                                UpdateGUI(uiComponents.MenuEnable);
                            }
                            bPrn.Dispose();
                            // *** addChequeInfo.Text = string.Empty;
                            // *** RowsRemoved_MyEvent(true, true, true);
                            this.profileCnt.Default.resetOrder();
                            //this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", bs.GetNewBillNo);
                        }
                        bs.Dispose();
                        break;
                    }
                case "SaveAndPrint": // save, print but leave opened
                    {
                        if (this.profileCnt.triggerInventCheque)
                            break;

                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.profileCnt.Default);
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

                        uiWndBillSave bs = new uiWndBillSave(this.profileCnt.Default);
                        if (bs.ShowDialog() == DialogResult.OK)
                        {
                            this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", bs.GetNewBillNo);
                            uiWndBillPrint bPrn = new uiWndBillPrint(bs.SavedBill);
                            if (bPrn.ShowDialog(this) == DialogResult.OK)
                            {
                                // *** this.RefershMenus();
                            }
                            bPrn.Dispose();
                            //DataWorkShared.MergeDataTableProperties(ref this.profileCnt.Default.Order, bs.SavedBill);
                            // this.profileCnt.Default.DataOrder.Merge(bs.SavedBill);
                            // **** this.RefershMenus();
                            UpdateGUI(uiComponents.MenuEnable);
                        }
                        bs.Dispose();
                        break;
                    }
                case "SaveAndClose": // save and close
                    {
                        if (this.profileCnt.triggerInventCheque)
                            break;

                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.profileCnt.Default);
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
                        uiWndBillSave bs = new uiWndBillSave(this.profileCnt.Default);
                        if (bs.ShowDialog() == DialogResult.OK)
                        {
                            // *** addChequeInfo.Text = string.Empty;
                            // **** RowsRemoved_MyEvent(true, true, true);

                            this.profileCnt.Default.resetOrder();

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
                        string currentBillNumber = DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_NO).ToString();
                        /*
                            if (this.profileCnt.Default.Order.ExtendedProperties["NOM"] != null)
                                currentBillNumber = this.profileCnt.Default.Order.ExtendedProperties["NOM"].ToString();
                        */
                        //if ()
                        uiWndBillList bl = new uiWndBillList(currentBillNumber);
                        if (bl.ShowDialog() == DialogResult.OK)
                            if (this.profileCnt.Default.DataOrder.Rows.Count == 0)
                            {
                                this.profileCnt.Default.Merge(bl.LoadedBill);
                                // **** this.UpdateDiscountValues(this.profileCnt.Default.Order);
                                this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_NO));
                                // * UpdateSumInfo(true);
                                profileCnt.Default.refresh();
                            }
                            else MMessageBoxEx.Show(this.chequeDGV, "Неможливо відкрити рахунок\nТаблиця чеку не є порожня", Application.ProductName,
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                        bl.Dispose();
                        break;
                    }
                case "PrintBill": // removed
                    {
                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.profileCnt.Default);
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

                        uiWndBillPrint bPrn = new uiWndBillPrint(this.profileCnt.Default);
                        bPrn.ShowDialog(this);
                        bPrn.Dispose();
                        break;
                    }
                case "SaveChangeComment":
                    {
                        if (this.profileCnt.triggerInventCheque)
                            break;

                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.profileCnt.Default);
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


                        uiWndBillSave bs = new uiWndBillSave(this.profileCnt.Default);
                        bs.UpdateComment = true;
                        if (bs.ShowDialog() == DialogResult.OK)
                        {
                            //if (bs.IsNewBill)
                            this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", bs.GetNewBillNo);
                            //this.PD_Order = bs.SavedBill;
                            // else
                            //    this.addBillInfo.Text = "";

                            // ????? 
                            // ****** DataWorkShared.MergeDataTableProperties(ref this.profileCnt.Default.DataOrder, bs.SavedBill);

                            // **** this.RefershMenus();
                            // PUT REFRESH FUNCTION CALL HERE

                        }
                        bs.Dispose();
                        break;
                    }
                case "SaveBill": // just save and leave opened
                    {
                        if (this.profileCnt.triggerInventCheque)
                            break;

                        int changeState = DataWorkBill.BillWasChanged(ConfigManager.Instance.CommonConfiguration.Path_Bills, this.profileCnt.Default);
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

                        uiWndBillSave bs = new uiWndBillSave(this.profileCnt.Default);
                        if (bs.ShowDialog() == DialogResult.OK)
                        {
                            //if (bs.IsNewBill)
                            this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", bs.GetNewBillNo);
                            //this.PD_Order = bs.SavedBill;
                            // else
                            //    this.addBillInfo.Text = "";

                            // *** DataWorkShared.MergeDataTableProperties(ref this.profileCnt.Default.DataOrder, bs.SavedBill);

                            // *** this.RefershMenus();
                            UpdateGUI(uiComponents.MenuEnable);

                        }
                        bs.Dispose();
                        break;
                    }
                case "ReloadBill": // ReloadBill
                    {
                        object billName = this.profileCnt.Default.Properties[CoreConst.BILL_PATH];
                        AppProfile LoadedBill = DataWorkBill.LoadCombinedBill(ConfigManager.Instance.CommonConfiguration.Path_Bills + "\\" + billName.ToString());
                        // **** RowsRemoved_MyEvent(true, true, true);
                        // ****** ADD/REPLACE DATA MEREGE FUNCTION INTO PROFILE
                        this.profileCnt.Default.Merge(LoadedBill);
                        this.addBillInfo.Text = string.Format("{0} {1}", "Рахунок №", DataWorkShared.ExtractBillProperty(this.profileCnt.Default.DataOrder, CoreConst.BILL_NO));
                        // * UpdateSumInfo(true);
                        profileCnt.Default.refresh();
                        break;
                    }
                case "CloseBillWithoutChanges": // CloseBill
                    {
                        addChequeInfo.Text = string.Empty;
                        // *** RowsRemoved_MyEvent(true, true, true);
                        this.profileCnt.Default.resetOrder();
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
            if (this.profileCnt.Default.DataOrder.ExtendedProperties.Contains("loading"))
                return;

            if (this.profileCnt.Default.DataOrder.Rows.Count % ConfigManager.Instance.CommonConfiguration.APP_InvAutoSave == 0 && this.profileCnt.triggerInventCheque)
                DataWorkCheque.SaveInvent(this.profileCnt.Default.DataOrder, true, this.profileCnt.getDataAllProfiles(DataType.ORDER));

            if (chequeDGV.Rows.Count == 1)
                UpdateGUI(uiComponents.MenuEnable); // *** RefershMenus();

            if (!this.profileCnt.triggerInventCheque)
            {
                Hashtable guiParams = new Hashtable();
                // *** RefreshChequeInformer(this.profileCnt.Default.DataOrder.Rows.Count == 1);
                if (this.profileCnt.Default.DataOrder.Rows.Count == 1)
                    guiParams.Add("RESET_CASH_INDICATOR", true);
                UpdateGUI(uiComponents.InformersType2, guiParams);
            }
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
                    DataRow dRow = this.profileCnt.Default.DataOrder.Rows.Find(chequeDGV.CurrentRow.Cells["C"].Value);
                    price = CoreLib.AutomaticPrice(thisTot, dRow);
                }
                double sum = MathLib.GetRoundedMoney(thisTot * price);

                chequeDGV["TOT", e.RowIndex].Value = thisTot;
                chequeDGV["TMPTOT", e.RowIndex].Value = thisTot;
                chequeDGV["PRICE", e.RowIndex].Value = price;
                chequeDGV["SUM", e.RowIndex].Value = sum;
                chequeDGV["ASUM", e.RowIndex].Value = sum;
                chequeDGV.Update();

                // * UpdateSumInfo(true);
                this.profileCnt.Default.refresh();
                SrchTbox.Select();
                SrchTbox.SelectAll();

                try
                {
                    DataRow[] dr = this.profileCnt.Default.DataProducts.Select("ID like '" + chequeDGV.CurrentRow.Cells["TID"].Value + "'");
                    if (dr != null && dr.Length != 0 && dr[0] != null)
                    {
                        thisTot = MathLib.GetDouble(chequeDGV["TQ", e.RowIndex].Value);
                        if (thisTot != 0)
                            addedTot *= MathLib.GetDouble(chequeDGV["TQ", e.RowIndex].Value);
                        CoreLib.AddArticleToCheque(chequeDGV, articleDGV, dr[0], addedTot, this.profileCnt.Default.DataProducts);
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

        #endregion

        #region Widget Event Handlers
        private void Navigator_OnFilterChanged(string filter, EventArgs e)
        {
            if (filter.Length == 0)
                this.articleDGV.DataSource = this.profileCnt.Default.DataProducts;
            else
            {
                DataRow[] dr = this.profileCnt.Default.DataProducts.Select("ID Like '" + filter + "%'");
                DataTable sTable = this.profileCnt.Default.DataProducts.Clone();
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
                            //DataRow[] article = this.profileCnt.Default.Products.Select("ID =" + chequeDGV.CurrentRow.Cells["ID"].Value.ToString());
                            DataRow[] article = this.profileCnt.Default.DataProducts.Select("ID =\'" + chequeDGV.CurrentRow.Cells["ID"].Value.ToString() + "\'");
                            if (article != null && article.Length == 1)
                            {
                                CoreLib.AddArticleToCheque(chequeDGV, articleDGV, article[0], -ConfigManager.Instance.CommonConfiguration.APP_StartTotal, this.profileCnt.Default.DataProducts);
                                // **** SearchFilter(true, this.currSrchType, false);
                                UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                    {"CLOSE", false},
                                    {"STYPE", this.currSrchType},
                                    {"SAVESRCH", true}
                                });
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
                            DataRow[] article = this.profileCnt.Default.DataProducts.Select("ID =\'" + chequeDGV.CurrentRow.Cells["ID"].Value.ToString() + "\'");
                            if (article != null && article.Length == 1)
                            {
                                CoreLib.AddArticleToCheque(chequeDGV, articleDGV, article[0], ConfigManager.Instance.CommonConfiguration.APP_StartTotal, this.profileCnt.Default.DataProducts, false, false);
                                // *** SearchFilter(true, this.currSrchType, false);
                                UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                                    {"CLOSE", false},
                                    {"STYPE", this.currSrchType},
                                    {"SAVESRCH", true}
                                });
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
        #endregion

        #region Custom Event Handlers
        private void splitContainer1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.splitContainer1.Orientation == Orientation.Vertical)
                this.splitContainer1.SplitterDistance = this.splitContainer1.Width / 2;
            else
                this.splitContainer1.SplitterDistance = this.splitContainer1.Height / 2;
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (serialPort1.BytesToRead >= ConfigManager.Instance.CommonConfiguration.APP_BuyerBarCodeMinLen)
            {
                readedBuyerBarCode = serialPort1.ReadExisting().Trim('%', '?', '\r', '\n');
            }
        }

        #endregion

        #region UI
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

        public void UpdateGUI(components.Shared.Enums.uiComponents blockToUpdate)
        {
            UpdateGUI(blockToUpdate, null);
        }
        public void UpdateGUI(components.Shared.Enums.uiComponents blockToUpdate, Hashtable p)
        {
            // *** components
            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.Components) != 0)
            {
                // ---- moved to profile container
                /*
                 * 
                 * this section is handled by profile container
                 * use profilecnt.refresh()
                 * 
                 * bool needUpdate = false;
                if (_fl_singleMode != ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                {
                    _fl_singleMode = ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles;
                    _fl_onlyUpdate = false;
                    needUpdate = true;
                }

                if (currentSubUnit != ConfigManager.Instance.CommonConfiguration.APP_SubUnit)
                {
                    _fl_onlyUpdate = false;
                    _fl_subUnitChanged = true;
                    needUpdate = true;
                    currentSubUnit = ConfigManager.Instance.CommonConfiguration.APP_SubUnit;
                }

                // trigger update function
                if (needUpdate)
                    FetchProductData(true, true, false);


                */

                //timer1_Tick(timer1, EventArgs.Empty);
                //winapi.Funcs.OutputDebugString("UpdateMyControls_end");

                //if (!timerExchangeImport.Enabled && !timerExchangeScanner.Enabled)

                timerExchangeScanner.Stop();
                timerExchangeImport.Stop();
                timerDataImportSynchronizer.Stop();

                // update import timer
                timerExchangeImport.Interval = ConfigManager.Instance.CommonConfiguration.APP_RefreshRate;
                // update exchange scanner timer
                timerExchangeScanner.Interval = ConfigManager.Instance.CommonConfiguration.APP_RefreshRate;
                // will setup exchnage scanner in 10 sec.
                // will run only once and launch timerScanner
                timerDataImportSynchronizer.Interval = ConfigManager.Instance.CommonConfiguration.APP_RefreshRate / 2;

                //if (!timerDataImportSynchronizer.Enabled || !timerExchangeScanner.Enabled)
                //{
                    timerExchangeScanner.Start();
                    timerDataImportSynchronizer.Start();
                    //timerExchangeImport.Start();
                    // trigger import timer with delay of 2 sec.
                    //Thread.Sleep(2000);
                    //timerExchangeImport.Start();
                //}
            }

            // *** Labels
            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.Labels) != 0)
            {
                this.label_uiWndmain_DemoShowArt.Visible = this.label_uiWndmain_DemoShowChq.Visible = !this._fl_isOk;
                this.Text = string.Format("{2}{1}{0}", Application.ProductName, " - ", ConfigManager.Instance.CommonConfiguration.APP_SubUnitName);
            }

            // *** App Window
            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.AppWindow) != 0)
            {
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

                this.Activate();
                this.BringToFront();
                this.UpdateZOrder();
            }

            // *** APP Informer
            // informer type 1
            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.InformersType1) != 0)
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
            }

            // *** Order Informer
            // if (_fl_isInvenCheque)
            // informer type 2
            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.InformersType2) != 0)
            {

                bool _local_resetDigitalPanel = (p != null && p.ContainsKey("RESET_CASH_INDICATOR") && (bool)p["RESET_CASH_INDICATOR"]);
                if (this.profileCnt.triggerInventCheque)
                {
                    CashLbl.Text = string.Format("{0}", "ІНВЕНТАРИЗАЦІЯ"); ;
                    chequeInfoLabel.Text = string.Format("{0}", this.profileCnt.Default.Properties["Date"]);
                }
                else
                {
                    string ctrlWord = "чеку";
                    // *** if (this.profileCnt.Default.Order.ExtendedProperties["BILL"] != null)
                    if (this.profileCnt.Default.Properties["BILL"] != null)
                        ctrlWord = "рахунку";
                    string totalWord = "позиці";
                    int numValue = this.profileCnt.Default.DataOrder.Rows.Count;

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

                    if (this.profileCnt.triggerReturnCheque)
                        chequeInfoLabel.Text = string.Format("{0} {1} {2} {3} {4}", "В", ctrlWord, this.profileCnt.Default.DataOrder.Rows.Count, totalWord, "повертається на суму");
                    else
                        chequeInfoLabel.Text = string.Format("{0} {1} {2} {3} {4}", "В", ctrlWord, this.profileCnt.Default.DataOrder.Rows.Count, totalWord, "продається на суму");
                    CashLbl.Text = string.Format("{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", this.profileCnt.Default.Properties[CoreConst.CASH_REAL_SUMA]);
                    //if (DataWorkShared.ExtractOrderProperty(this.profileCnt.Default.Order, CoreConst.BILL, null, true) == null)
                    if (this.profileCnt.Default.Properties["BILL"] == null)
                        this.addBillInfo.Text = string.Empty;
                }

                if (_local_resetDigitalPanel)
                {
                    CashLbl.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_SumFontColor;
                    CashLbl.Font = ConfigManager.Instance.CommonConfiguration.STYLE_SumFont;

                    CashLbl.Image = null;
                    digitalPanel.BackgroundImage = null;

                    this.profileCnt.triggerTaxDocRequired = false;
                }
            }

            // Digital Block
            // informer type 3
            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.InformersType3) != 0) // ??? resetDigitalPanel
            {


                // _fl_taxDocRequired = false;
            }

            // *** Styling
            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.Colors) != 0)
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
            }

            // *** Menu Items

            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.MenuEnable) != 0)
            {
                if (Program.AppPlugins.IsActive(PluginType.FPDriver))
                    try
                    {
                        fxFunc_toolStripMenuItem.Enabled = Program.AppPlugins.GetActive<IFPDriver>().AllowedMethods.Count != 0;
                    }
                    catch { }
                else
                    fxFunc_toolStripMenuItem.Enabled = false;
                фільтрОдиницьToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0 && (_fl_adminMode || UserConfig.Properties[9]);
                формуванняЧекуToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0 && _fl_adminMode;
                інвентаризаціяToolStripMenuItem.Enabled = (this.profileCnt.triggerInventCheque || profileCnt.Default.DataOrder.Rows.Count == 0) && _fl_adminMode;
                чекПоверненняToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0 && (_fl_adminMode || UserConfig.Properties[5]);
                налаштуванняToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0 && _fl_adminMode;
                параметриДрукуToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0 && _fl_adminMode;
                змінитиКористувачаToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0;
                вихідToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0;

                //друкуватиРахунокToolStripMenuItem.Enabled = this.profileCnt.Default.Order.ExtendedProperties.Contains("BILL");
                /*bool isLocked = (bool)DataWorkShared.ExtractBillProperty(this.profileCnt.Default.Order, CoreConst.IS_LOCKED, false);
                bool isBill = DataWorkShared.ExtractOrderProperty(this.profileCnt.Default.Order, CoreConst.BILL, null, true) != null;
                */
                bool isLocked = profileCnt.Default.Properties[CoreConst.BILL_IS_LOCKED] != null && (bool)profileCnt.Default.Properties[CoreConst.BILL_IS_LOCKED] == false;
                bool isBill = profileCnt.Default.Properties[CoreConst.ORDER_BILL] != null;

                анулюватиРахунокToolStripMenuItem.Enabled = isBill && !isLocked;
                зберегтиРахунокToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && !this.profileCnt.triggerInventCheque && !isLocked;
                всіРахункиToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque;
                зберегтиІЗакритиToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && !this.profileCnt.triggerInventCheque;
                зберегтиІДрукуватиToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && !this.profileCnt.triggerInventCheque;// && !isLocked;
                ToolStripMenu_Bills_SavePrintAndClose.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && !this.profileCnt.triggerInventCheque;// && !isLocked;
                закритиБезЗмінToolStripMenuItem.Enabled = isBill;
                перезавантажитиРахунокToolStripMenuItem.Enabled = isBill;
                змінитиКоментарToolStripMenuItem.Enabled = isBill;

                змінитиКстьТоваруToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[24]);
                видалитиВибранийТоварToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[24]);
                видалитиВсіТовариToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[24]);
                здійснитиОплатуToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[23]);
                задатиЗнижкаToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[3]);
                задатиНадбавкуToolStripMenuItem1.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[3]);
            }

            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.MenuTicks) != 0)
            {
                інвентаризаціяToolStripMenuItem.Checked = this.profileCnt.triggerInventCheque;
                чекПоверненняToolStripMenuItem.Checked = this.profileCnt.triggerReturnCheque;
                адміністраторToolStripMenuItem.Checked = _fl_adminMode;
                вертикальноToolStripMenuItem.Checked = (splitContainer1.Orientation == Orientation.Vertical);
                вікноТоварівToolStripMenuItem.Checked = !splitContainer1.Panel2Collapsed;
            }

            // *** Sensor Mode
            // widgets
            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.Widgets) != 0 /*&& this.сенсорToolStripMenuItem.Checked/* ???? || force*/)
            {
                //Com_WinApi.OutputDebugString("RefershStyles_Sensor_Activated");

                сенсорToolStripMenuItem.Checked = ConfigManager.Instance.CommonConfiguration.skin_sensor_active;
                // cheque
                переміщенняПоЧекуToolStripMenuItem1.Checked = this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Scrolling, ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqnav);
                операціїЧекуToolStripMenuItem1.Checked = this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Operations, ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqopr);
                режимиПошукуToolStripMenuItem1.Checked = this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Search, ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqsrch);
                рахункиToolStripMenuItem1.Checked = this.sensorPanel1.ShowComponent(SensorUgcPanel.SensorComponents.Additional, ConfigManager.Instance.CommonConfiguration.skin_sensor_com_chqbills);

                // art
                навігаціяТоварівToolStripMenuItem.Checked = ConfigManager.Instance.CommonConfiguration.skin_sensor_com_artnav;
                переміщенняПоТоварахToolStripMenuItem.Checked = this.sensorDataPanel1.Scroller.Visible = ConfigManager.Instance.CommonConfiguration.skin_sensor_com_artscroll;


                // = sensor widget
                if (ConfigManager.Instance.CommonConfiguration.skin_sensor_active)
                {

                    this.sensorPanel1.SensorType = ConfigManager.Instance.CommonConfiguration.skin_sensor_com_size_cheque;

                    this.sensorDataPanel1.Navigator.SetAndShowNavigator(ApplicationConfiguration.Instance.GetValueByKey<Hashtable>("productFiltering"));
                    this.sensorDataPanel1.Visible = true;
                    this.chequeContainer.Panel2Collapsed = false;
                    this.sensorDataPanel1.setupDataContainer(this.articleDGV);

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
                else
                {
                    this.sensorDataPanel1.Visible = false;
                    this.chequeContainer.Panel2Collapsed = true;
                    // push 
                    this.articleDGV.Parent = this.splitContainer1.Panel2;

                    //this.TopMost = false;
                    управліннToolStripMenuItem.Enabled = false;
                } // sensor widget
            }

            // = search controls
            // control type 1
            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.ControlsType1) != 0)
            {
                if (p.ContainsKey("CLOSE") && (bool)p["CLOSE"] /*close*/)
                {
                    Com_WinApi.OutputDebugString("MainWnd --- SearchFilter - reseting data begin");
                    // *** articleDGV.DataSource = this.profileCnt.Default.Products;
                    articleDGV.DataSource = profileCnt.Default.DataProducts;
                    Com_WinApi.OutputDebugString("MainWnd --- SearchFilter - reseting data end");
                }

                if (p.ContainsKey("SAVESRCH") && (bool)p["SAVESRCH"]  /*!saveSearchText*/)
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

                if (p.ContainsKey("STYPE"))
                {
                    currSrchType = int.Parse(p["STYPE"].ToString());
                    switch (currSrchType/*SrchType*/)
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
                    
                }
                SrchTbox.Focus();
                SrchTbox.Select();
                SrchTbox.SelectAll();
            } // control type 1


            // = update sum display
            // control type 2
            if (((int)blockToUpdate & (int)global::components.Shared.Enums.uiComponents.ControlsType2) != 0)
            {
                // ***** if (updateAddChequeInfo)
                addChequeInfo.Text = string.Empty;
                // if (updateAddChequeInfo && discCommonPercent != 0.0)

                /*if (this.profileCnt.Default.getPropertyValue<double>(CoreConst.DISC_CONST_PERCENT) != 0.0 || this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[0] != 0.0 || this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[1] != 0.0 ||
                    this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0] != 0.0 || this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[1] != 0.0)*/
                if (this.profileCnt.Default.customCashDiscountSomeEnabled)
                {
                    object[] discInfo = new object[6];
                    string valueMask = "{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}{1}";
                    /*bool useConstDisc = this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[0] == 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[1] == 0.0 &&
                        this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0] == 0.0 && this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[1] == 0.0;
                    */
                    discInfo[0] = "";
                    if (this.profileCnt.Default.DataOrder.Rows.Count != 0)
                        if (this.profileCnt.triggerUseTotDisc)
                            discInfo[0] = " загальна";
                        else
                            discInfo[0] = " позиційна";

                    // *** if (useConstDisc)
                    if (this.profileCnt.Default.customCashDiscountPropgramOnlyEnable)
                    {
                        discInfo[0] = "постійна" + discInfo[0].ToString();
                        discInfo[1] = this.profileCnt.Default.getPropertyValue<double>(CoreConst.DISCOUNT_CONST_PERCENT) > 0 ? "знижка" : "націнка";
                        discInfo[2] = string.Format(valueMask, Math.Abs(this.profileCnt.Default.getPropertyValue<double>(CoreConst.DISCOUNT_CONST_PERCENT)), "%");
                    }
                    else
                        if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                        {
                            // ** if (this.profileCnt.Default.customCashDiscountItems.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[0] != 0.0 || this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0] != 0.0)
                            if (this.profileCnt.Default.customCashDiscountManualSavingsEnabled)
                            {
                                // !!!! NEED TO BE REFACTORED
                                discInfo[1] = "знижка";
                                if (ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc == 0)
                                    if (this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_SUB] == 0.0)
                                        discInfo[2] = string.Format(valueMask, this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB], "%");
                                    else
                                        discInfo[2] = string.Format(valueMask, this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_SUB], "грн.");
                                else
                                    if (this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] == 0.0)
                                        discInfo[2] = string.Format(valueMask, this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_SUB], "грн.");
                                    else
                                        discInfo[2] = string.Format(valueMask, this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB], "%");
                            }
                            if (this.profileCnt.Default.customCashDiscountManualExtraEnabled)
                            {
                                discInfo[1] = "націнка";
                                if (ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc == 0)
                                    if (this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_ADD] == 0.0)
                                        discInfo[2] = string.Format(valueMask, Math.Abs(this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD]), "%");
                                    else
                                        discInfo[2] = string.Format(valueMask, Math.Abs(this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_ADD]), "грн.");
                                else
                                    if (this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISCOUNT_MANUAL_PERCENT_ADD)[1] == 0.0)
                                        discInfo[2] = string.Format(valueMask, Math.Abs(this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_ADD]), "грн.");
                                    else
                                        discInfo[2] = string.Format(valueMask, Math.Abs(this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD]), "%");
                            }
                        }
                        else
                        {
                            discInfo[1] = "знижка";

                            if (ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc == 0)
                                if (this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_SUB] == 0.0)
                                    discInfo[2] = string.Format(valueMask, this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB], "%");
                                else
                                    discInfo[2] = string.Format(valueMask, this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_SUB], "грн.");
                            else
                                if (this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISCOUNT_MANUAL_PERCENT_SUB)[0] == 0.0)
                                    discInfo[2] = string.Format(valueMask, this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_SUB], "грн.");
                                else
                                    discInfo[2] = string.Format(valueMask, this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB], "%");

                            discInfo[3] = "i";
                            discInfo[4] = "націнка";

                            if (ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc == 0)
                                if (this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_ADD] == 0.0)
                                    discInfo[5] = string.Format(valueMask, Math.Abs(this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD]), "%");
                                else
                                    discInfo[5] = string.Format(valueMask, Math.Abs(this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_ADD]), "грн.");
                            else
                                if (this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD] == 0.0)
                                    discInfo[5] = string.Format(valueMask, Math.Abs(this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_CASH_ADD]), "грн.");
                                else
                                    discInfo[5] = string.Format(valueMask, Math.Abs(this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD]), "%");
                        }

                    addChequeInfo.Text = valueMask = string.Empty;
                    for (byte i = 0; i < discInfo.Length && discInfo[i] != null; i++)
                        valueMask += (discInfo[i] + " ");
                    addChequeInfo.Text = valueMask.Remove(valueMask.Length - 1, 1);
                }

                //Show cheque Suma on display
                // *** CashLbl.Text = string.Format("{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", realSUMA);
                CashLbl.Text = string.Format("{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", this.profileCnt.getDefaultProfileValue<double>(CoreConst.CASH_REAL_SUMA));

                if (ConfigManager.Instance.CommonConfiguration.APP_ShowInfoOnIndicator && Program.AppPlugins.IsActive(PluginType.FPDriver) && p.ContainsKey("UPDATE_CUSTOMER") && (bool)p["UPDATE_CUSTOMER"]/* && updateCustomer*/)
                    try
                    {
                        string _topLabel = "СУМА:" + CashLbl.Text;
                        // if (discCommonPercent != 0)
                        if (profileCnt.getDefaultProfileValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT) != 0.0)
                        {
                            if (profileCnt.getDefaultProfileValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT) > 0)
                                _topLabel += " Зн:";
                            else
                                _topLabel += " Нб:";
                            _topLabel += Math.Abs(profileCnt.getDefaultProfileValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT)) + "%";
                        }

                        string[] lines = new string[] { string.Empty, string.Empty };
                        bool[] show = new bool[] { true, true };
                        if (this.profileCnt.Default.DataOrder.Rows.Count != 0)
                            lines = new string[] { _topLabel, chequeDGV.CurrentRow.Cells["DESC"].Value.ToString() };
                        Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_SendCustomer", lines, show);
                    }
                    catch { }
            }
        }
        
        /// <summary>
        /// Custom method. Used for updating data of elements.
        ///  !!! Must be replaced with UpdateGUI
        /// </summary>
        private void _UpdateMyControls()
        {
            //winapi.Funcs.OutputDebugString("UpdateMyControls_begin");
            _RefreshAppInformer();
            _RefreshChequeInformer(true);
            _RefershStyles();
            _RefershMenus();
            _RefreshWindowMenu();


            // ---- moved to profile container
            bool needUpdate = false;
            if (_fl_singleMode != ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
            {
                _fl_singleMode = ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles;
                // ** _fl_onlyUpdate = false;
                needUpdate = true;
            }

            if (currentSubUnit != ConfigManager.Instance.CommonConfiguration.APP_SubUnit)
            {
                // ** _fl_onlyUpdate = false;
                this.profileCnt.triggerInventCheque = true;
                needUpdate = true;
                currentSubUnit = ConfigManager.Instance.CommonConfiguration.APP_SubUnit;
            }

            // trigger update function
            if (needUpdate)
                FetchProductData(true, true, false);
            //timer1_Tick(timer1, EventArgs.Empty);
            //winapi.Funcs.OutputDebugString("UpdateMyControls_end");

            //if (!timerExchangeImport.Enabled && !timerExchangeScanner.Enabled)

            timerExchangeScanner.Stop();
            timerExchangeImport.Stop();
            timerDataImportSynchronizer.Stop();

            // update import timer
            timerExchangeImport.Interval = ConfigManager.Instance.CommonConfiguration.APP_RefreshRate;
            // update exchange scanner timer
            timerExchangeScanner.Interval = ConfigManager.Instance.CommonConfiguration.APP_RefreshRate;
            // will setup exchnage scanner in 10 sec.
            // will run only once and launch timerScanner
            timerDataImportSynchronizer.Interval = ConfigManager.Instance.CommonConfiguration.APP_RefreshRate / 2;

            if (!timerDataImportSynchronizer.Enabled || !timerExchangeScanner.Enabled)
            {
                timerExchangeScanner.Start();
                timerDataImportSynchronizer.Start();
                //timerExchangeImport.Start();
                // trigger import timer with delay of 2 sec.
                //Thread.Sleep(2000);
                //timerExchangeImport.Start();
            }

        }
        private void _RefreshAppInformer()
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
        private void _RefreshChequeInformer(bool resetDigitalPanel)
        {
            if (this.profileCnt.triggerInventCheque)
            {
                CashLbl.Text = string.Format("{0}", "ІНВЕНТАРИЗАЦІЯ"); ;
                chequeInfoLabel.Text = string.Format("{0}", profileCnt.Default.Properties["Date"]);
            }
            else
            {
                string ctrlWord = "чеку";
                // *** if (this.profileCnt.Default.Order.ExtendedProperties["BILL"] != null)
                if (profileCnt.Default.Properties["BILL"] != null)
                    ctrlWord = "рахунку";
                string totalWord = "позиці";
                int numValue = profileCnt.Default.DataOrder.Rows.Count;

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

                if (this.profileCnt.triggerReturnCheque)
                    chequeInfoLabel.Text = string.Format("{0} {1} {2} {3} {4}", "В", ctrlWord, profileCnt.Default.DataOrder.Rows.Count, totalWord, "повертається на суму");
                else
                    chequeInfoLabel.Text = string.Format("{0} {1} {2} {3} {4}", "В", ctrlWord, profileCnt.Default.DataOrder.Rows.Count, totalWord, "продається на суму");
                CashLbl.Text = string.Format("{0:F" + ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals + "}", profileCnt.Default.Properties[CoreConst.CASH_REAL_SUMA]);
                //if (DataWorkShared.ExtractOrderProperty(this.profileCnt.Default.Order, CoreConst.BILL, null, true) == null)
                if (profileCnt.Default.Properties["BILL"] == null)
                    this.addBillInfo.Text = string.Empty;
            }

            if (resetDigitalPanel)
            {
                CashLbl.ForeColor = ConfigManager.Instance.CommonConfiguration.STYLE_SumFontColor;
                CashLbl.Font = ConfigManager.Instance.CommonConfiguration.STYLE_SumFont;

                CashLbl.Image = null;
                digitalPanel.BackgroundImage = null;
                // ** _fl_taxDocRequired = false;
            }
        }
        private void _RefershStyles()
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
        private void _RefershMenus()
        {
            if (Program.AppPlugins.IsActive(PluginType.FPDriver))
                try
                {
                    fxFunc_toolStripMenuItem.Enabled = Program.AppPlugins.GetActive<IFPDriver>().AllowedMethods.Count != 0;
                }
                catch { }
            else
                fxFunc_toolStripMenuItem.Enabled = false;
            адміністраторToolStripMenuItem.Checked = _fl_adminMode;
            фільтрОдиницьToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0 && (_fl_adminMode || UserConfig.Properties[9]);
            формуванняЧекуToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0 && _fl_adminMode;
            інвентаризаціяToolStripMenuItem.Enabled = (this.profileCnt.triggerInventCheque || profileCnt.Default.DataOrder.Rows.Count == 0) && _fl_adminMode;
            чекПоверненняToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0 && (_fl_adminMode || UserConfig.Properties[5]);
            налаштуванняToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0 && _fl_adminMode;
            параметриДрукуToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0 && _fl_adminMode;
            змінитиКористувачаToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0;
            вихідToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count == 0;

            //друкуватиРахунокToolStripMenuItem.Enabled = this.profileCnt.Default.Order.ExtendedProperties.Contains("BILL");
            /*bool isLocked = (bool)DataWorkShared.ExtractBillProperty(this.profileCnt.Default.Order, CoreConst.IS_LOCKED, false);
            bool isBill = DataWorkShared.ExtractOrderProperty(this.profileCnt.Default.Order, CoreConst.BILL, null, true) != null;
            */
            bool isLocked = profileCnt.Default.Properties[CoreConst.BILL_IS_LOCKED] != null && (bool)profileCnt.Default.Properties[CoreConst.BILL_IS_LOCKED] == false;
            bool isBill = profileCnt.Default.Properties[CoreConst.ORDER_BILL] != null;

            анулюватиРахунокToolStripMenuItem.Enabled = isBill && !isLocked;
            зберегтиРахунокToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && !this.profileCnt.triggerInventCheque && !isLocked;
            всіРахункиToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque;
            зберегтиІЗакритиToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && !this.profileCnt.triggerInventCheque;
            зберегтиІДрукуватиToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && !this.profileCnt.triggerInventCheque;// && !isLocked;
            ToolStripMenu_Bills_SavePrintAndClose.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && !this.profileCnt.triggerInventCheque;// && !isLocked;
            закритиБезЗмінToolStripMenuItem.Enabled = isBill;
            перезавантажитиРахунокToolStripMenuItem.Enabled = isBill;
            змінитиКоментарToolStripMenuItem.Enabled = isBill;

            змінитиКстьТоваруToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[24]);
            видалитиВибранийТоварToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[24]);
            видалитиВсіТовариToolStripMenuItem.Enabled = profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[24]);
            здійснитиОплатуToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[23]);
            задатиЗнижкаToolStripMenuItem.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[3]);
            задатиНадбавкуToolStripMenuItem1.Enabled = !this.profileCnt.triggerInventCheque && profileCnt.Default.DataOrder.Rows.Count != 0 && (_fl_adminMode || UserConfig.Properties[3]);

        }//ok
        private void _RefreshWindowMenu()
        {
            вертикальноToolStripMenuItem.Checked = (splitContainer1.Orientation == Orientation.Vertical);
            вікноТоварівToolStripMenuItem.Checked = !splitContainer1.Panel2Collapsed;
        }
        private void _RefreshComponents(bool force)
        {
            if (this.сенсорToolStripMenuItem.Checked || force)
            {
                //Com_WinApi.OutputDebugString("RefershStyles_Sensor_Activated");

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

        #region Application Timers
        /// <summary>
        /// Timer's event indicate when timer is up and perform application data updating
        /// </summary>
        /// <param name="sender">Timer object</param>
        /// <param name="e">Timer event arguments</param>

        private void timerDataImportSynchronizer_Tick(object sender, EventArgs e)
        {
            Com_WinApi.OutputDebugString("timerDataImportSynchronizer: started");
            timerDataImportSynchronizer.Stop();
            timerExchangeImport.Start();
            Com_WinApi.OutputDebugString("timerDataImportSynchronizer: end");

            //if (!timerExchangeImport.Enabled && !timerExchangeScanner.Enabled)
            //    ;
        }
        private void timerExchangeScanner_Tick(object sender, EventArgs e)
        {
            Com_WinApi.OutputDebugString("TimerExchangeGrabbermer: started");
            if (_fl_importIsRunning || ImportedData.Tables.Count != 0)
            {
                Com_WinApi.OutputDebugString("TimerExchangeGrabbermer: import is running");
                return;
            }
            FetchProductData(true, false, true);
            Com_WinApi.OutputDebugString("TimerExchangeGrabbermer: end");
        }
        private void timerExchangeImport_Tick(object sender, EventArgs e)//lbl
        {
            //timerExchangeImport.Start();
            Com_WinApi.OutputDebugString("timerExchangeImport: started");
            
            if (_fl_importIsRunning)
            {
                Com_WinApi.OutputDebugString("timerExchangeImport: is running");
                return;
            }

            //if (this.profileCnt.Default.Order.Rows.Count != 0)
            if (profileCnt.Default.DataOrder.Rows.Count != 0)
            {
                _fl_canUpdate = true;
                Com_WinApi.OutputDebugString("timerExchangeImport: waiting for empty checkque");
                return;
            }

            _fl_canUpdate = false;

            /*if (_fl_subUnitChanged)
            {
                this.profileCnt.Default.Products.Rows.Clear();
                this.profileCnt.Default.Alternative.Rows.Clear();
            }*/

            /*
             * bool hasUpdates = scanExchangeFolder(); 
             * if (hasUpdates)
             *      processImportedData();
             * 
             */

            //Thread th = new Thread(new ThreadStart(BgWorker));
            //th.Start();
            //this.BeginInvoke(new ExchangeScanner(ScanExchangeFolder), new object[] { null, EventArgs.Empty });
            /*lock (this)
            {
                Thread th = new Thread(new ThreadStart(startCheckExchange));
                th.Start();
            }*/

            _fl_importIsRunning = true;
            try
            {
                FetchProductData(false, true, false);
            }
            catch (Exception ex)
            {
                CoreLib.WriteLog(ex, "PayDesk.Components.UI.uiWndMain@timerDataLoader_Tick");
            }
            _fl_importIsRunning = false;

            Com_WinApi.OutputDebugString("timerExchangeImport: end");
        }
        private void timerBuyerReady_Tick(object sender, EventArgs e)
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
        #endregion

        #region Application Properties


        // shold be removed after dataContainer2
        // **************************************************************************
        // public Hashtable _PD_DiscountInfo
        // {
            // set { }
            // get
            // {
                // *** Hashtable chqInfo = DataWorkShared.GetStandartDiscountInfoStructure2();
                /*
                //Якщо true то знижка чи надбавка діє на всі позиції(товари) чеку
                chqInfo["DISC_ALL_ITEMS"] = this._fl_useTotDisc;
                //Масив з значеннями знижки та надбавки в процентних значеннях
                chqInfo["DISC_ARRAY_PERCENT"] = new double[2] { this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[0], this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[1] };
                //Масив з значеннями знижки та надбавки в грошових значеннях
                chqInfo["DISC_ARRAY_CASH"] = new double[2] { this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0], this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[1] };
                //Значення постійної знижки в процентному значенні
                chqInfo["DISC_CONST_PERCENT"] = this.profileCnt.Default.getPropertyValue<double>(CoreConst.DISC_CONST_PERCENT);
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
                */
                // return chqInfo;
            // }
        // }

        public bool[] _PD_Statements
        {
            set { }
            get { return new bool[3]; }
        }

        public DataTable _PD_DEMO_Order
        {
            get
            {
                Dictionary<string, object> chqInfo = DataWorkShared.GetStandartOrderInfoStructure();
                // fill cheque structure
                //chqInfo["DATA"] = this.profileCnt.Default.Order.Copy();
                /*****chqInfo["STORE_NO"] = this.currentSubUnit;
                chqInfo["CLIENT_ID"] = this.clientID;
                chqInfo["IS_RET"] = this._fl_isReturnCheque;
                chqInfo["IS_LEGAL"] = false;
                chqInfo["ORDER_NO"] = string.Empty;
                chqInfo["ORDER_SUMA"] = this.chqSUMA;
                chqInfo["ORDER_REAL_SUMA"] = this.realSUMA;
                chqInfo["TAX_SUMA"] = this.realSUMA;
                chqInfo["TAX_BILL"] = this._fl_taxDocRequired;
                chqInfo["DISCOUNT"] = this.PD_DiscountInfo;
                */
                object bill = this.profileCnt.Default.DataOrder.ExtendedProperties["BILL"];
                DataWorkShared.UpdateExtendedProperties(this.profileCnt.Default.DataOrder, chqInfo);
                this.profileCnt.Default.DataOrder.ExtendedProperties["BILL"] = bill;
                /*
                chqInfo["BILL_NO"] = string.Empty;
                chqInfo["BILL_COMMENT"] = string.Empty;
                if (this.profileCnt.Default.Order.ExtendedProperties.Contains("BILL")) {
                    //Номер рахунку
                    chqInfo["BILL_NO"] = this.profileCnt.Default.Order.ExtendedProperties["NOM"];
                    //Коментр рахунку
                    chqInfo["BILL_COMMENT"] = this.profileCnt.Default.Order.ExtendedProperties["CMT"];
                }*/
                return this.profileCnt.Default.DataOrder;
            }
        }

        public DataTable _PD_Order
        {
            get
            {
                Dictionary<string, object> chqInfo = DataWorkShared.GetStandartOrderInfoStructure(this.profileCnt.Default.DataOrder);
                // fill cheque structure
                //chqInfo["DATA"] = this.profileCnt.Default.Order.Copy();
                /******chqInfo["STORE_NO"] = this.currentSubUnit;
                chqInfo["CLIENT_ID"] = this.clientID;
                chqInfo["IS_RET"] = this._fl_isReturnCheque;
                chqInfo["IS_LEGAL"] = false;
                //chqInfo["ORDER_NO"] = string.Empty;
                chqInfo["ORDER_SUMA"] = this.chqSUMA;
                chqInfo["ORDER_REAL_SUMA"] = this.realSUMA;
                chqInfo["TAX_SUMA"] = this.realSUMA;
                chqInfo["TAX_BILL"] = this._fl_taxDocRequired;
                chqInfo["DISCOUNT"] = this.PD_DiscountInfo;
                */
                DataWorkShared.UpdateExtendedProperties(this.profileCnt.Default.DataOrder, chqInfo);
                /*
                chqInfo["BILL_NO"] = string.Empty;
                chqInfo["BILL_COMMENT"] = string.Empty;
                if (this.profileCnt.Default.Order.ExtendedProperties.Contains("BILL")) {
                    //Номер рахунку
                    chqInfo["BILL_NO"] = this.profileCnt.Default.Order.ExtendedProperties["NOM"];
                    //Коментр рахунку
                    chqInfo["BILL_COMMENT"] = this.profileCnt.Default.Order.ExtendedProperties["CMT"];
                }*/
                return this.profileCnt.Default.DataOrder;
            }
        }

        #endregion

        #region Helpful Methods
        /*private Hashtable GetAppStatements()
        {
            Hashtable currentStates = new Hashtable();
            currentStates[CoreConst.STATE_DATA_UPDATED] = false;

            return currentStates;
        }*/
        private void _SearchFilter(bool saveSearchText, int SrchType, bool close)
        {

        }

        // will be removed after Profile2
        // used in main menu bill section
        // setup app values with loaded bill
        /*
        public void UpdateDiscountValues(DataTable order)
        {
            this.currentSubUnit = (byte)order.ExtendedProperties[CoreConst.ORDER_STORE_NO];
            try
            {
                this.clientID = order.ExtendedProperties[CoreConst.ORDER_CLIENT_ID].ToString();
            }
            catch { }
            this._fl_isReturnCheque = (bool)order.ExtendedProperties[CoreConst.ORDER_IS_RET];

            if (order.ExtendedProperties.ContainsKey(CoreConst.ORDER_DISCOUNT))
            {

                Hashtable discount = (Hashtable)order.ExtendedProperties[CoreConst.ORDER_DISCOUNT];

                try
                {
                    this._fl_useTotDisc = (bool)discount[CoreConst.DISC_ALL_ITEMS];
                    this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT) = (double[])discount[CoreConst.DISC_ARRAY_PERCENT];
                    this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH) = (double[])discount[CoreConst.DISC_ARRAY_CASH];
                    this.profileCnt.Default.getPropertyValue<double>(CoreConst.DISC_CONST_PERCENT) = (double)discount[CoreConst.DISC_CONST_PERCENT];
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
                chqInfo["DISC_ARRAY_PERCENT"] = new double[2] { this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[0], this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[1] };
                //Масив з значеннями знижки та надбавки в грошових значеннях
                chqInfo["DISC_ARRAY_CASH"] = new double[2] { this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0], this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[1] };
                //Значення постійної знижки в процентному значенні
                chqInfo["DISC_CONST_PERCENT"] = this.profileCnt.Default.getPropertyValue<double>(CoreConst.DISC_CONST_PERCENT);
                //Сума знижки і надбавки з процентними значеннями
                chqInfo["DISC_ONLY_PERCENT"] = this.discOnlyPercent;
                //Сума знижки і надбавки з грошовими значеннями
                chqInfo["DISC_ONLY_CASH"] = this.discOnlyCash;
                //Загальний коефіціент знижки в процентному значенні
                chqInfo["DISC_FINAL_PERCENT"] = this.discCommonPercent;
                //Загальний коефіціент знижки в грошовому значенні
                chqInfo["DISC_FINAL_CASH"] = this.discCommonCash;
                 *--------/
            }
        }
        public DataTable GetProfileOrder(object profileKey)
        {
            Hashtable _suma = (Hashtable)this.Summa[profileKey];
            Hashtable _discount = (Hashtable)this.Discount[profileKey];
            DataTable _cheque = this.profileCnt.Default.Orders.Tables[profileKey.ToString()];

            Dictionary<string, object> chqInfo = DataWorkShared.GetStandartOrderInfoStructure(_cheque);
            Hashtable discInfo = DataWorkShared.GetStandartDiscountInfoStructure2();

            /* initializing discount values *------/
            bool _discApplied = CoreLib.GetValue<bool>(_discount, CoreConst.DISC_APPLIED);
            double[] _discArrPercent = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_PERCENT);
            double[] _discArrCash = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_CASH);
            double _discConstPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_CONST_PERCENT);
            double _discOnlyPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_PERCENT);
            double _discOnlyCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_CASH);
            double _discCommonPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_PERCENT);
            double _discCommonCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_CASH);
            /* calculation items *----/
            double _realSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CASH_REAL_SUMA);
            double _chqSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CASH_CHEQUE_SUMA);
            double _taxSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CASH_TAX_SUMA);

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
            chqInfo["IS_RET"] = this._fl_isReturnCheque;
            chqInfo["IS_LEGAL"] = false;
            chqInfo["ORDER_SUMA"] = _chqSUMA;
            chqInfo["ORDER_REAL_SUMA"] = _realSUMA;
            chqInfo["TAX_SUMA"] = _realSUMA;
            chqInfo["TAX_BILL"] = this._fl_taxDocRequired;
            chqInfo["DISCOUNT"] = discInfo;

            DataWorkShared.UpdateExtendedProperties(_cheque, chqInfo);

            return _cheque;
        }
        private void CreateOrderStructure(DataTable dtOrder)
        {
            Dictionary<string, object> chqInfo = DataWorkShared.GetStandartOrderInfoStructure();
            DataWorkShared.AppendExtendedProperties(dtOrder, chqInfo, true);
        }
        */
        // ---
        
        // discount
        /// <summary>
        /// Виконує обрахунок суми товарів, які знаходяться в таблиці чеку
        /// а також вираховує коефіціенти знижок чи надбавок
        /// </summary>
        /// <param name="updateCustomer">Якщо true то результати обчислення будуть ще виведені на дисплей ФП</param>
        /// 
        // will be removed after Profile2
        private void _UpdateSumInfo(bool updateCustomer)
        {
            /*UpdateSumInfo_single(updateCustomer);

            /* perform update for all profiles if it necessary *-/
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

                    profileDataRows = this.profileCnt.Default.Order.Select(selectCommandForLegalProducts);
                    this.profileCnt.Default.Orders.Tables[de.Key.ToString()].Rows.Clear();
                    foreach (DataRow dr in profileDataRows)
                        this.profileCnt.Default.Orders.Tables[de.Key.ToString()].Rows.Add(dr.ItemArray);


                    /* set discounts *-/



                    Hashtable _discount = (Hashtable)this.Discount[de.Key.ToString()];
                    _discount[CoreConst.DISC_ARRAY_PERCENT] = this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT);
                    _discount[CoreConst.DISC_ARRAY_CASH] = this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH);
                    //_discount[CoreConst.DISC_CONST_PERCENT] = this.profileCnt.Default.getPropertyValue<double>(CoreConst.DISC_CONST_PERCENT);
                    //_discount[CoreConst.DISC_ONLY_CASH] = this.discOnlyCash;
                    //_discount[CoreConst.DISC_ONLY_PERCENT] = this.discOnlyPercent;
                    this.Discount[de.Key.ToString()] = _discount;


                    UpdateSumInfo_profile(de.Key.ToString(), updateCustomer);

                    if (this.profileCnt.Default.Order.Rows.Count == 0)
                    {
                        realSUMA = chqSUMA = taxSUMA = 0.0;
                        UpdateSumDisplay(false, updateCustomer);
                        // this.PD_EmptyOrder;
                        return;
                    }
                }
            }*/
        }

        /// <summary>
        /// Custom method. Perform updating of information labels. Also update device displai
        /// </summary>
        /// <param name="updateAddChequeInfo"></param>
        /// <param name="updateCustomer">If true methid will update device display otherwise false</param>
        private void _UpdateSumDisplay(/*bool updateAddChequeInfo, bool updateCustomer*/ )
        {

        }

        private void OrderClose(bool isLegalMode)
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
            uiWndPayment pMethod = new uiWndPayment(this.profileCnt.getDefaultProfileValue<double>(CoreConst.CASH_REAL_SUMA), true);
            pMethod.ShowDialog();
            pMethod.Dispose();

            if (pMethod.DialogResult != DialogResult.OK)
                return;

            if (UserConfig.Properties[4] &&
                DialogResult.Yes == MMessageBoxEx.Show(this.chequeDGV, "Видати накладну згідно цього чеку ?", Application.ProductName,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
            {
                // *** _fl_taxDocRequired = true;
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


            /*List<string> _allProfiles = new List<string>();
            bool hasLegalProfile = ConfigManager.Instance.CommonConfiguration.PROFILES_Items.ContainsKey(ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID);
            foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                _allProfiles.Add(de.Key.ToString());
            if (hasLegalProfile)
            {
                _allProfiles.Remove(ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID.ToString());
                _allProfiles.Insert(0, ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID.ToString());
            }*/

            List<string> _allProfiles = this.profileCnt.getProfileList();


            Dictionary<string, object> _orgPaymanet = pMethod.PaymentInfo;
            // *** DataTable dtCopy = this.PD_Order.Copy();
            AppProfile dtCopy = (AppProfile)this.profileCnt.Default.Clone();
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
                    /*
                    Hashtable _suma = (Hashtable)this.Summa[profileKey];
                    Hashtable _discount = (Hashtable)this.Discount[profileKey];
                    DataTable _cheque = this.profileCnt.Default.Orders.Tables[profileKey.ToString()];
                    */



                    if (this.profileCnt[profileKey].DataOrder.Rows.Count == 0)
                    {
                        skippedProfiles++;
                        continue;
                    }

                    /* initializing discount values */
                    double[] _discArrPercent = {this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_SUB), 
                                               this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_PERCENT_ADD)};
                    double[] _discArrCash = { this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_SUB), 
                                                this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.DISCOUNT_MANUAL_CASH_ADD) };
                    double _discConstPercent = this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.DISCOUNT_CONST_PERCENT);
                    double _discOnlyPercent = this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.DISCOUNT_ONLY_PERCENT);
                    double _discOnlyCash = this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.DISCOUNT_ONLY_CASH);
                    double _discCommonPercent = this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_PERCENT);
                    double _discCommonCash = this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.DISCOUNT_FINAL_CASH);
                    /* calculation items */
                    double _realSUMA = this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.CASH_REAL_SUMA);
                    double _chqSUMA = this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.CASH_CHEQUE_SUMA);
                    double _taxSUMA = this.profileCnt[profileKey].getPropertyValue<double>(CoreConst.CASH_TAX_SUMA);


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

                    if (_allProfiles.IndexOf(profileKey) + 1 < _allProfiles.Count)
                    {
                        _tmpPaymanet["SUMA"] = _realSUMA;
                        ((List<double>)_tmpPaymanet["CASHLIST"])[0] = _realSUMA;
                        _tmpPaymanet["REST"] = 0.0;

                        // removing these values from original payment
                        _orgPaymanet["SUMA"] = MathLib.GetRoundedMoney(MathLib.GetDouble(_orgPaymanet["SUMA"]) - _realSUMA);
                        ((List<double>)_orgPaymanet["CASHLIST"])[0] = MathLib.GetRoundedMoney(MathLib.GetDouble(((List<double>)_orgPaymanet["CASHLIST"])[0]) - _realSUMA);
                    }

                    profileResult[CoreConst.ORDER_PAYMENT] = _tmpPaymanet;
                    profileResult[CoreConst.ORDER_IS_LEGAL] = isLegalMode;


                    localData = new object[8];
                    chqNom = string.Empty;


                    localData[0] = clientID == string.Empty ? ConfigManager.Instance.CommonConfiguration.APP_ClientID : clientID;
                    localData[1] = _discCommonPercent;
                    localData[2] = _realSUMA;
                    localData[3] = _taxSUMA;
                    localData[4] = this.profileCnt.triggerTaxDocRequired;
                    localData[5] = this.profileCnt.triggerReturnCheque;
                    localData[6] = this.profileCnt.triggerUseTotDisc;

                    //winapi.Funcs.OutputDebugString("B");
                    if (appIsLegal)
                    {
                        global::components.Components.WinApi.Com_WinApi.OutputDebugString("is legal cheque for profile " + currentProfileKey + " and legal profile is " + ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID);
                        try
                        {
                            if (this.profileCnt.triggerReturnCheque)
                                Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_PayMoney", this.profileCnt[profileKey].DataOrder, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, this.profileCnt.triggerUseTotDisc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);
                            else
                                Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_Sale", this.profileCnt[profileKey].DataOrder, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, this.profileCnt.triggerUseTotDisc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);

                            if (this.profileCnt.triggerUseTotDisc && _discCommonPercent != 0.0)
                            {
                                Program.AppPlugins.GetActive<IFPDriver>().CallFunction(
                                    "FP_Discount",
                                     new object[] {
                                     (byte)2,
                                     _discCommonPercent, 
                                     ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, 
                                     string.Empty
                                 }
                                );
                            }

                            if (lastPayment >= pMethod.Type.Count)
                                lastPayment = 0;

                            for (int i = lastPayment; i < pMethod.Type.Count; i++)
                            {
                                Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_Payment", pMethod.Type[i], ((List<double>)_tmpPaymanet["CASHLIST"])[i], true);
                                lastPayment++;
                            }

                            chqNom = Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_LastChqNo", this.profileCnt.triggerReturnCheque).ToString();
                            localData[7] = Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_LastZRepNo", this.profileCnt.triggerReturnCheque);
                        }
                        catch (Exception ex)
                        {
                            CoreLib.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);

                            MMessageBoxEx.Show(this.chequeDGV, "Помилка під час закриття чеку" + "\r\n" + ex.Message,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

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

                        try
                        {
                            Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_OpenBox");
                        }
                        catch (Exception ex)
                        {
                            MMessageBoxEx.Show(this.chequeDGV, "Помилка відкриття грошової скриньки" + "\r\n" + ex.Message,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            CoreLib.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                        }
                    }

                    lastPayment = 0;
                    chqNom = DataWorkCheque.SaveCheque(this.profileCnt[profileKey].DataOrder, localData, pMethod.Type[0], chqNom);
                    profileResult[CoreConst.ORDER_NO] = chqNom;

                    // *** DataWorkShared.SetOrderProperty(this.profileCnt[profileKey].DataOrder, CoreConst.ORDER_PAYMENT, _tmpPaymanet);
                    // ***DataWorkShared.SetOrderProperty(this.profileCnt[profileKey].DataOrder, CoreConst.ORDER_NO, chqNom);


                    this.profileCnt[profileKey].Properties[CoreConst.ORDER_PAYMENT] = _tmpPaymanet;
                    this.profileCnt[profileKey].Properties[CoreConst.ORDER_NO] = chqNom;


                    if (appIsLegal && UserConfig.Properties[10])
                    {
                        // *** must be migrated
                        // *** DataWorkOutput.Print(Enums.PrinterType.OrderLegal, this.GetProfileOrder(profileKey));
                    }

                    if (!appIsLegal && UserConfig.Properties[11])
                    {
                        // *** must be migrated
                        // *** DataWorkOutput.Print(Enums.PrinterType.OrderNormal, this.GetProfileOrder(profileKey));
                    }

                    fullResult.Add(profileResult);
                    chqNumbers.Add(chqNom);
                    chqNumbersFull.Add(appIsLegal ? chqNom : 'N' + chqNom);

                    // *** RowsRemoved_MyEvent_profile(profileKey);
                    this.profileCnt[profileKey].resetOrder();
                }
                catch { generalError = true; }

                if (generalError)
                    break;
            }

            if (skippedProfiles == this.profileCnt.Profiles.Count/* ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Count*/)
            {
                MMessageBox.Show(this.chequeDGV, "Немає товарів для усіх профілів. Обновіть фільтри профілів.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //UpdateSumInfo_profile(currentProfileKey, false);
                profileCnt[currentProfileKey].refresh();
                return;
            }


            if (generalError)
            {
                MMessageBox.Show(this.chequeDGV, "Виникла помилка під час збереження частини чеку.\r\nСпробуйте ще раз закрити чек", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                // - UpdateSumInfo_profile(currentProfileKey, false);
                profileCnt[currentProfileKey].refresh();
                return;
            }
            //if (DataWorkShared.ExtractBillProperty(dtCopy, CoreConst.BILL_OID, string.Empty) != string.Empty)

            if ( this.profileCnt.Default.Properties[CoreConst.BILL_OID] != null)
            {
                //CoreLib.LockBill(this.PD_Order.Copy(), chqNom);
                if (ConfigManager.Instance.CommonConfiguration.Content_Bills_KeepAliveAfterCheque)
                {
                    //string billChqNumFmt = string.Empty;
                    //for (int cqnc = 0; cqnc < chqNumbers.Count; cqnc++)
                    //    billChqNumFmt += "{" + cqnc + "},";
                    //billChqNumFmt = billChqNumFmt.TrimEnd(',');
                    //DataWorkBill.LockBill(dtCopy.Copy(), string.Format("[" + billChqNumFmt + "]", chqNumbers.ToArray()));
                    // ***** DataWorkBill.LockBill(dtCopy.Copy(), string.Join(" | ", chqNumbersFull.ToArray()));
                    
                    // *** migrate to profile
                    // *** DataWorkBill.LockBill(dtCopy.Copy(), string.Join(" | ", chqNumbersFull.ToArray()));

                }
                else
                {
                    // *** must be migrated
                    // *** DataWorkBill.BillDelete(dtCopy);
                }
                //this.profileCnt.Default.Order.ExtendedProperties["FXNO"] = chqNom;

                //File.Delete(this.profileCnt.Default.Order.ExtendedProperties["PATH"].ToString());
            }

            string closedInfo = string.Empty;
            if (chqNumbers.Count > 1)
                closedInfo = string.Format("{0} {1} ", "Закриті чеки №", string.Join(",", chqNumbers.ToArray()));
            else
                closedInfo = string.Format("{0} {1} ", "Закритий чек №", string.Join(string.Empty, chqNumbers.ToArray()));
            // *** if (!string.IsNullOrEmpty(DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.BILL_NO, string.Empty).ToString()))
            // ***    closedInfo += string.Format("{0} {1} ", "з рахунку №", DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.BILL_NO, string.Empty));

            if (this.profileCnt.Default.Properties[CoreConst.BILL_NO] != null )
                closedInfo += string.Format("{0} {1} ", "з рахунку №", this.profileCnt.Default.Properties[CoreConst.BILL_NO]);

            // ****** RowsRemoved_MyEvent(false, true, true);


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
                    Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_SendCustomer", lines, show);
                }
                catch { }



        }

        /// <summary>
        /// Закриття чеку
        /// </summary>
        /// <param name="isLegalMode">Якщо true то чек є фіскальний</param>
        /*private void CloseCheque_profile(bool isLegalMode)//1_msg//lbl7
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
                _fl_taxDocRequired = true;
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
                    DataTable _cheque = this.profileCnt.Default.Orders.Tables[profileKey.ToString()];


                    if (_cheque.Rows.Count == 0)
                    {
                        skippedProfiles++;
                        continue;
                    }

                    /* initializing discount values *-----/
                    double[] _discArrPercent = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_PERCENT);
                    double[] _discArrCash = CoreLib.GetValue<double[]>(_discount, CoreConst.DISC_ARRAY_CASH);
                    double _discConstPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_CONST_PERCENT);
                    double _discOnlyPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_PERCENT);
                    double _discOnlyCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_ONLY_CASH);
                    double _discCommonPercent = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_PERCENT);
                    double _discCommonCash = CoreLib.GetValue<double>(_discount, CoreConst.DISC_FINAL_CASH);
                    /* calculation items *----/
                    double _realSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CASH_REAL_SUMA);
                    double _chqSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CASH_CHEQUE_SUMA);
                    double _taxSUMA = CoreLib.GetValue<double>(_suma, CoreConst.CASH_TAX_SUMA);


                    // it's checking if the running profile allows to save order as legal
                    appIsLegal = isLegalMode && ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID.Equals(profileKey);
                    //closeResult = CloseCheque_profile(de.Key.ToString(), isLegalMode);

                    profileResult = new Hashtable();

                    profileResult["PROFILE_ID"] = profileKey;
                    profileResult["PROFILE_LAST"] = pIdx == ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Count;



                    /* close logic implementation *---/

                    // if this is a legal or in-between profile and there are other profiles
                    // it requires to change payment info and set it without change
                    // but las profile should has the rest and chnaged payment

                    Dictionary<string, object> _tmpPaymanet = new Dictionary<string, object>(_orgPaymanet);
                    _tmpPaymanet["CASHLIST"] = new List<double>((List<double>)_tmpPaymanet["CASHLIST"]);
                    _tmpPaymanet["TYPE"] = new List<byte>((List<byte>)_tmpPaymanet["TYPE"]);

                    if (_allProfiles.IndexOf(profileKey) + 1 < _allProfiles.Count)
                    {
                        _tmpPaymanet["SUMA"] = _realSUMA;
                        ((List<double>)_tmpPaymanet["CASHLIST"])[0] = _realSUMA;
                        _tmpPaymanet["REST"] = 0.0;

                        // removing these values from original payment
                        _orgPaymanet["SUMA"] = MathLib.GetRoundedMoney(MathLib.GetDouble(_orgPaymanet["SUMA"]) - _realSUMA);
                        ((List<double>)_orgPaymanet["CASHLIST"])[0] = MathLib.GetRoundedMoney(MathLib.GetDouble(((List<double>)_orgPaymanet["CASHLIST"])[0]) - _realSUMA);
                    }

                    profileResult[CoreConst.ORDER_PAYMENT] = _tmpPaymanet;
                    profileResult[CoreConst.ORDER_IS_LEGAL] = isLegalMode;


                    localData = new object[8];
                    chqNom = string.Empty;


                    localData[0] = clientID == string.Empty ? ConfigManager.Instance.CommonConfiguration.APP_ClientID : clientID;
                    localData[1] = _discCommonPercent;
                    localData[2] = _realSUMA;
                    localData[3] = _taxSUMA;
                    localData[4] = _fl_taxDocRequired;
                    localData[5] = _fl_isReturnCheque;
                    localData[6] = _fl_useTotDisc;

                    //winapi.Funcs.OutputDebugString("B");
                    if (appIsLegal)
                    {
                        global::components.Components.WinApi.Com_WinApi.OutputDebugString("is legal cheque for profile " + currentProfileKey + " and legal profile is " + ConfigManager.Instance.CommonConfiguration.PROFILES_LegalProgileID);
                        try
                        {
                            if (_fl_isReturnCheque)
                                Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_PayMoney", _cheque, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, _fl_useTotDisc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);
                            else
                                Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_Sale", _cheque, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, _fl_useTotDisc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);

                            if (_fl_useTotDisc && _discCommonPercent != 0.0)
                            {
                                Program.AppPlugins.GetActive<IFPDriver>().CallFunction(
                                    "FP_Discount",
                                     new object[] {
                                     (byte)2,
                                     _discCommonPercent, 
                                     ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, 
                                     string.Empty
                                 }
                                );
                            }

                            if (lastPayment >= pMethod.Type.Count)
                                lastPayment = 0;

                            for (int i = lastPayment; i < pMethod.Type.Count; i++)
                            {
                                Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_Payment", pMethod.Type[i], ((List<double>)_tmpPaymanet["CASHLIST"])[i], true);
                                lastPayment++;
                            }

                            chqNom = Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_LastChqNo", _fl_isReturnCheque).ToString();
                            localData[7] = Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_LastZRepNo", _fl_isReturnCheque);
                        }
                        catch (Exception ex)
                        {
                            CoreLib.WriteLog(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);

                            MMessageBoxEx.Show(this.chequeDGV, "Помилка під час закриття чеку" + "\r\n" + ex.Message,
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

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

                        try
                        {
                            Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_OpenBox");
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

                    DataWorkShared.SetOrderProperty(_cheque, CoreConst.ORDER_PAYMENT, _tmpPaymanet);
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
                profileCnt[currentProfileKey].refresh();
                return;
            }


            if (generalError)
            {
                MMessageBox.Show(this.chequeDGV, "Виникла помилка під час збереження частини чеку.\r\nСпробуйте ще раз закрити чек", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                // - UpdateSumInfo_profile(currentProfileKey, false);
                profileCnt[currentProfileKey].refresh();
                return;
            }
            if (DataWorkShared.ExtractBillProperty(dtCopy, CoreConst.BILL_OID, string.Empty) != string.Empty)
            {
                //CoreLib.LockBill(this.PD_Order.Copy(), chqNom);
                if (ConfigManager.Instance.CommonConfiguration.Content_Bills_KeepAliveAfterCheque)
                {
                    //string billChqNumFmt = string.Empty;
                    //for (int cqnc = 0; cqnc < chqNumbers.Count; cqnc++)
                    //    billChqNumFmt += "{" + cqnc + "},";
                    //billChqNumFmt = billChqNumFmt.TrimEnd(',');
                    //DataWorkBill.LockBill(dtCopy.Copy(), string.Format("[" + billChqNumFmt + "]", chqNumbers.ToArray()));
                    DataWorkBill.LockBill(dtCopy.Copy(), string.Join(" | ", chqNumbersFull.ToArray()));

                }
                else
                    DataWorkBill.BillDelete(dtCopy);

                //this.profileCnt.Default.Order.ExtendedProperties["FXNO"] = chqNom;

                //File.Delete(this.profileCnt.Default.Order.ExtendedProperties["PATH"].ToString());
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
                    Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_SendCustomer", lines, show);
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
                _fl_taxDocRequired = true;
                CashLbl.Image = Properties.Resources.naklad;
            }

            localData[0] = clientID == string.Empty ? ConfigManager.Instance.CommonConfiguration.APP_ClientID : clientID;
            localData[1] = discCommonPercent;
            localData[2] = realSUMA;
            localData[3] = taxSUMA;
            localData[4] = _fl_taxDocRequired;
            localData[5] = _fl_isReturnCheque;
            localData[6] = _fl_useTotDisc;

            //winapi.Funcs.OutputDebugString("B");
            if (isLegalMode)
            {
                try
                {
                    if (_fl_isReturnCheque)
                        Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_PayMoney", this.profileCnt.Default.DataOrder, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, _fl_useTotDisc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);
                    else
                        Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_Sale", this.profileCnt.Default.DataOrder, ConfigManager.Instance.CommonConfiguration.APP_DoseDecimals, _fl_useTotDisc, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals);

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
                        ; Program.AppPlugins.GetActive<IFPDriver>().CallFunction(
                              "FP_Discount",
                             new object[] { 
                                    (byte)2/*types[i]* --- /, 
                                    /*valueDISC[i]* -- /discCommonPercent/*(discount[0] + discount[1]) == 0 ? constDiscount : (discount[0] + discount[1])* -- /,
                                    ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, "" });
                    }

                    if (lastPayment >= pMethod.Type.Count)
                        lastPayment = 0;

                    for (int i = lastPayment; i < pMethod.Type.Count; i++)
                    {
                        Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_Payment", pMethod.Type[i], pMethod.ItemsCash[i], pMethod.Autoclose);
                        lastPayment++;
                    }


                    chqNom = Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_LastChqNo", _fl_isReturnCheque).ToString();
                    localData[7] = Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_LastZRepNo", _fl_isReturnCheque);
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
                    * --- /

                    //Program.AppPlugins.GetActive<IFPDriver>().CallFunction("ResetOrder");

                    //ChqNomRequest cnr = new ChqNomRequest();
                    //cnr.ShowDialog();
                    //cnr.Dispose();
                    //if (cnr.DialogResult != DialogResult.Yes)

                    //chqNom = cnr.ChequeNumber.ToString();
                }

                try
                {
                    Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_OpenBox");
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
            chqNom = DataWorkCheque.SaveCheque(this.profileCnt.Default.DataOrder, localData, pMethod.Type[0], chqNom);

            //object[] printerData = this.CreatePrinterData(fix, pMethod.PaymentInfo);
            //Dictionary<string, object> printerData = this.PD_Order;
            // add additional information
            //DataWorkShared.SetOrderProperty(this.PD_Order, CoreConst.ORDER_NO, chqNom);
            DataWorkShared.SetOrderProperty(this.profileCnt.Default.DataOrder, CoreConst.ORDER_PAYMENT, pMethod.PaymentInfo);
            DataWorkShared.SetOrderProperty(this.profileCnt.Default.DataOrder, CoreConst.ORDER_NO, isLegalMode ? chqNom : 'N' + chqNom);
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
            if (DataWorkShared.ExtractBillProperty(this.PD_Order, CoreConst.BILL_OID, string.Empty) != string.Empty)
            {
                //CoreLib.LockBill(this.PD_Order.Copy(), chqNom);
                if (ConfigManager.Instance.CommonConfiguration.Content_Bills_KeepAliveAfterCheque)
                    DataWorkBill.LockBill(this.PD_Order.Copy(), isLegalMode ? chqNom : 'N' + chqNom);
                else
                    DataWorkBill.BillDelete(this.PD_Order);

                //this.profileCnt.Default.Order.ExtendedProperties["FXNO"] = chqNom;

                //File.Delete(this.profileCnt.Default.Order.ExtendedProperties["PATH"].ToString());
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
                    Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_SendCustomer", lines, show);
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
                    Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_SendCustomer", lines, show);
                }
                catch { }
            //}
        }
        * --- /
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
        */
        /// <summary>
        /// Custom method. Perform reset of discount variables
        /// </summary>
        private void _ResetDiscount()
        {
            //this.profileCnt.Default.getPropertyValue<double>(CoreConst.DISC_CONST_PERCENT) = 0.0;
            /*this.profileCnt.Default.customCashDiscountItems[CoreConst.DISC_ARRAY_PERCENT] = 0.0;
            this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_PERCENT)[1] = 0.0;
            this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[0] = 0.0;
            this.profileCnt.Default.getPropertyValue<double[]>(CoreConst.DISC_ARRAY_CASH)[1] = 0.0;
            discOnlyPercent = 0.0;
            discOnlyCash = 0.0;
            discCommonPercent = 0.0;
            discCommonCash = 0.0;
            clientPriceNo = 0;*/
            //discApplied = false;



            //відмінитиЗнижкунадбавкуToolStripMenuItem.Enabled = false;
            відмінитиЗнижкунадбавкуToolStripMenuItem.Text = "Без знижки/надбавки";

            /*if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                {
                    this.Discount[de.Key] = DataWorkShared.GetStandartDiscountInfoStructure2();
                    this.Summa[de.Key] = DataWorkShared.GetStandartCalculationInfoStructure2();
                }
            */
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

            DataTable sTable = this.profileCnt.Default.DataProducts.Clone();
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
                    dr = this.profileCnt.Default.DataDiscountCards.Select("CBC =\'" + barcode + "\'");

                    if (dr.Length != 0 && dr[0] != null)
                    {
                        if (this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] < (double)dr[0]["CDISC"] && this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_CONST_PERCENT] < (double)dr[0]["CDISC"])
                        {
                            if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                                this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] = this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD] = 0.0;
                            this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] = (double)dr[0]["CDISC"];
                        }
                        this.clientPriceNo = (int)dr[0]["CPRICENO"];
                        // - UpdateSumInfo(true);
                        profileCnt.Default.refresh();
                        clientID = (string)dr[0]["CID"];
                        // *** SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, false);
                        UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                            {"CLOSE", false},
                            {"STYPE", ConfigManager.Instance.CommonConfiguration.APP_SearchType},
                            {"SAVESRCH", false}
                        });
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
                    if (this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] < dec && this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_CONST_PERCENT] < dec)
                    {
                        if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                            this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] = this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD] = 0.0;
                        this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] = dec;
                    }

                if (dec > 100 && (this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] == 0 || this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_CONST_PERCENT] >= 0))
                {
                    dec -= 100;
                    dec = -Math.Round(dec, ConfigManager.Instance.CommonConfiguration.APP_MoneyDecimals, MidpointRounding.AwayFromZero);
                    if (this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD] < dec || this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_CONST_PERCENT] < dec)
                    {
                        if (ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount)
                            this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_SUB] = this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD] = 0.0;
                        this.profileCnt.Default.customCashDiscountItems[CoreConst.DISCOUNT_MANUAL_PERCENT_ADD] = dec;
                    }
                }

                // - UpdateSumInfo(true);
                profileCnt.Default.refresh();
                // *** SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, false);
                UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                    {"CLOSE", false},
                    {"STYPE", ConfigManager.Instance.CommonConfiguration.APP_SearchType},
                    {"SAVESRCH", false}
                });
                return allowToShow;
            }
            #endregion
            //search by barcodes of articles
            
            dr = this.profileCnt.Default.DataProducts.Select("BC = \'" + barcode.Trim() + "\'");
            if (dr.Length == 0 && UserConfig.Properties[16])
            {
                dr = this.profileCnt.Default.DataAlternative.Select("ABC = \'" + barcode + "\'");
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
                        rows = this.profileCnt.Default.DataProducts.Select(cmd);
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
                {
                    // *** SearchFilter(false, ConfigManager.Instance.CommonConfiguration.APP_SearchType, true);
                    UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                        {"CLOSE", true},
                        {"STYPE", ConfigManager.Instance.CommonConfiguration.APP_SearchType},
                        {"SAVESRCH", false}
                    });
                }
                else
                {
                    // *** SearchFilter(true, currSrchType, false);
                    UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                        {"CLOSE", false},
                        {"STYPE", this.currSrchType},
                        {"SAVESRCH", true}
                    });
                }
                return allowToShow;
            }

            if (dr.Length == 1)
            {
                CoreLib.AddArticleToCheque(chequeDGV, articleDGV, dr[0], weightOfArticle, this.profileCnt.Default.DataProducts);
                if (!UserConfig.Properties[22])
                {
                    // *** SearchFilter(true, currSrchType, true);
                    UpdateGUI(uiComponents.ControlsType1, new Hashtable() {
                        {"CLOSE", true},
                        {"STYPE", this.currSrchType},
                        {"SAVESRCH", true}
                    });
                }
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

        /// <summary>
        /// Обробляє чек після видалення одного або всіх товару(ів).
        /// (обраховує суму, відновлує фільтрацію таблиці товарів, оновлює повідомлення на панелях)
        /// </summary>
        /// <param name="updateCustomer">Якщо true то результати обчислення будуть ще виведені на дисплей ФП</param>
        
        /*********
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
                // *--- this.profileCnt.Default.Order.Rows.Clear();
                // ***** this.CreateOrderStructure(this.profileCnt.Default.Order);
                //this.profileCnt.Default.Order.ExtendedProperties.Clear();
                this.profileCnt.reset(true);

                if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
                    foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                        RowsRemoved_MyEvent_profile(de.Key);
            }

            //winapi.Funcs.OutputDebugString("t");
            if (this.profileCnt.Default.DataOrder.Rows.Count == 0)
            {
                RefershMenus();
                if (_fl_isReturnCheque)
                    чекПоверненняToolStripMenuItem.PerformClick();
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
            // * UpdateSumInfo(updateCustomer);
            profileCnt.Default.refresh();
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

            foreach (DataRow dRow in this.profileCnt.Default.Orders.Tables[profileKey.ToString()].Rows)
            {
                try
                {
                    this.profileCnt.Default.DataOrder.Rows.Find(dRow["C"]).Delete();
                }
                catch { }
            }

            // clear profile cheque
            this.profileCnt.Default.Orders.Tables[profileKey.ToString()].Rows.Clear();



        }
        */

        private void _InitChequeInformationStructure()
        {
            /* loop by all available profiles * -------- /
            this.Discount.Clear();
            this.Summa.Clear();
            if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles)
            {
                DataWorkSource.CreateTables(ref this.profileCnt.Default.Order, ref this.profileCnt.Default.Products, ref this.profileCnt.Default.Alternative, ref this.profileCnt.Default.DiscountCards, ref Cheques);
                this.CreateOrderStructure(this.profileCnt.Default.Order);
                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                {
                    this.CreateOrderStructure(this.profileCnt.Default.Orders.Tables[de.Key.ToString()]);
                    this.Discount.Add(de.Key, DataWorkShared.GetStandartDiscountInfoStructure2());
                    this.Summa.Add(de.Key, DataWorkShared.GetStandartCalculationInfoStructure2());
                }
            }
            else
            {
                this.Discount.Add(CoreConst.KEY_DEFAULT_PROFILE_ID, this.PD_DiscountInfo);
                this.Summa.Add(CoreConst.KEY_DEFAULT_PROFILE_ID, DataWorkShared.GetStandartCalculationInfoStructure2());
            }*/
        }
        #endregion

        #region Data
        private void FetchProductData(bool doCheck, bool doImport, bool runCheckInSeparateThread)
        {
            // will download and transform new data
            if (doCheck)
            {
                if (runCheckInSeparateThread)
                {
                    Thread scannerTh = new Thread(new ThreadStart(getNewDataAndDoLocalRawImport));
                    scannerTh.Start();
                }
                else
                    getNewDataAndDoLocalRawImport();
            }
            if (doImport)
            {
                try
                {
                    // *** PostDataImportAction();
                    if (ImportedData.Tables.Count > 0)
                        profileCnt.setupData(ImportedData);
                    // *** _fl_onlyUpdate = true;
                    this.profileCnt.triggerInventCheque = false;
                    _fl_isOk = new Com_SecureRuntime().FullLoader();
                    label_uiWndmain_DemoShowArt.Visible = label_uiWndmain_DemoShowChq.Visible = !_fl_isOk;
                }
                catch (Exception ex)
                {
                    CoreLib.WriteLog(ex, "PayDesk.Components.UI.uiWndMain@FetchProductData");
                }
                ImportedData.Tables.Clear();
            }
        }

        private void getNewDataAndDoLocalRawImport()
        {
            Hashtable filesToImport = new Hashtable();
            int currentProfileIndex = 0;
            int startupIndex = 0;
            string[] tableNames = { "PRODUCT", "ALTERNATEBC", "DCARDS" };
            bool _local_updateOnly = this.profileCnt.valueOfUpdateMode == UpdateMode.SERVERDATAONLY;

            // check new data existance
            DataWorkSource.CheckForUpdate(ref filesToImport);

            // clear temporaty exchange data
            this.ImportedData.Tables.Clear();

            // check exchange folders
            // check local folder

            int maxRowCount = 0;
            int totalImportedRows = 0;
            foreach (DictionaryEntry de in filesToImport)
            {
                maxRowCount = 0;
                string[] files = (string[])de.Value;

                string[] localFiles = DataWorkSource.LoadFilesOnLocalTempFolder(files, de.Key);


                if ((files[0] == CoreConst.STATE_LAN_ERROR || files[0] == "") && files[1] == "" && files[2] == "" && _local_updateOnly)
                {
                    // if only one profile
                    if (filesToImport.Count == 1)
                        continue;

                    // next turn
                    currentProfileIndex++;
                    continue;
                }

                if (currentProfileIndex == 0)
                    startupIndex = 0;
                else
                    startupIndex = totalImportedRows + 1;// profileCnt.Default.DataProducts.Rows.Count;// this.profileCnt.Default.Products.Rows.Count;
                // object[] loadResult = DataWorkSource.LoadData(localFiles, this.profileCnt.triggerRunUpdateOnly, de.Key, startupIndex);
                object[] loadResult = DataWorkSource.LoadData(localFiles, false, de.Key, startupIndex);

                DataTable[] tables = (DataTable[])loadResult[0];

                for (int i = 0; i < tables.Length; i++)
                    if (tables[i] != null)
                    {
                        DataTable dt = new DataTable(tableNames[i] + "=" + de.Key);
                        dt.Merge(tables[i]);
                        if (dt.Rows.Count > maxRowCount)
                            maxRowCount = dt.Rows.Count;
                        this.ImportedData.Tables.Add(dt);
                    }
                totalImportedRows += maxRowCount;
                currentProfileIndex++;
            }




            //{
                GC.Collect();
                //return;
            //}
            ConfigManager.SaveConfiguration();



            /*
            //bool wasUpdatedAtLeastOneSource = false;
            //_fl_artUpdated = false;
            foreach (DictionaryEntry de in filesToImport)
            {

                //this.DDM_Scanner.Value++;

                string[] files = (string[])de.Value;

                /* detectiong for updates */

                //this.DDM_Scanner.Value++;

                // server status
                /*if (files[0] == CoreConst.STATE_LAN_ERROR && filesToImport.Count == 1)
                    DDM_UpdateStatus.Image = Properties.Resources.ExNotOk;
                else
                    DDM_UpdateStatus.Image = Properties.Resources.ok;
                *---/
                //this.DDM_Scanner.Value++;

                if ((files[0] == CoreConst.STATE_LAN_ERROR || files[0] == "") && files[1] == "" && files[2] == "" && this.profileCnt.triggerRunUpdateOnly)
                {
                    /* if only one profile *---/
                    if (filesToImport.Count == 1)
                    //if (!_fl_artUpdated && (hfiles.Count == 1 || currentProfileIndex + 1 == hfiles.Count))
                    {
                        //timer1.Start();
                        GC.Collect();
                        /* close notification */
                        /*if (notificationIsActive)
                        {
                            uw.Close();
                            uw.Dispose();
                        }*----/
                        return;
                    }

                    /* next turn *---/
                    currentProfileIndex++;
                    continue;
                }
                //MessageBox.Show("done 2");

                //this.DDM_Scanner.Value++;

                /*if (!notificationIsActive)
                {
                    uw.ShowUpdate(this);
                    uw.Update();
                    uw.Refresh();
                    notificationIsActive = true;
                }*/


                //this.DDM_Scanner.Value++;

                /* loading *---/

                //MessageBox.Show("done 3");
                string[] localFiles = DataWorkSource.LoadFilesOnLocalTempFolder(files, de.Key);


                //this.DDM_Scanner.Value++;

                if (currentProfileIndex == 0)
                    startupIndex = 0;
                else
                    startupIndex = profileCnt.Default.DataProducts.Rows.Count;// this.profileCnt.Default.Products.Rows.Count;
                object[] loadResult = DataWorkSource.LoadData(localFiles, this.profileCnt.triggerRunUpdateOnly, de.Key, startupIndex);


                //this.DDM_Scanner.Value++;

                ConfigManager.SaveConfiguration();

                /* adding data *---/


                //this.DDM_Scanner.Value++;

                //MessageBox.Show("done 4");

                DataTable[] tables = (DataTable[])loadResult[0];
                //_fl_artUpdated = (bool)loadResult[1];

                //this.DDM_Scanner.Value++;

                for (int i = 0; i < tables.Length; i++)
                    if (tables[i] != null)
                    {
                        DataTable dt = new DataTable(tableNames[i] + "=" + de.Key);
                        dt.Merge(tables[i]);
                        this.ImportedData.Tables.Add(dt);
                    }
                
                /*
                if (tables[0] != null) {
                    //this.profileCnt.Default.Products = tables[0].Copy();
                    //DataRow[] dRows = this.profileCnt.Default.Products.Select("F = " + de.Key);
                    //foreach (DataRow dr in dRows)
                    //    dr.Delete();
                    //this.profileCnt.Default.Products.Merge(tables[0]);
                    //wasUpdatedAtLeastOneSource = true;
                }
                if (tables[1] != null)
                {

                    //this.profileCnt.Default.Alternative = tables[1].Copy();
                    //DataRow[] dRows = this.profileCnt.Default.Alternative.Select("F = " + de.Key);
                    //foreach (DataRow dr in dRows)
                    //    dr.Delete();
                    //this.profileCnt.Default.Alternative.Merge(tables[1]);
                    //wasUpdatedAtLeastOneSource = true;
                }
                if (tables[2] != null)
                {
                    //this.profileCnt.Default.DiscountCards = tables[2].Copy();
                    //if (currentProfileIndex == 0)
                    this.profileCnt.Default.DiscountCards.Rows.Clear();
                    this.profileCnt.Default.DiscountCards.Merge(tables[2]);
                    //wasUpdatedAtLeastOneSource = true;
                }

                *---/
                //this.DDM_Scanner.Value++;

                //MessageBox.Show("done 5");
                currentProfileIndex++;

            }*/
            Com_WinApi.OutputDebugString("MainWnd --- AddingData End");
        }

        /* Checker callback */

        // not ok// remove
        private void PostDataImportAction()
        {
            /*if (ConfigManager.Instance.CommonConfiguration.PROFILES_UseProfiles && this.profileCnt.Default.Orders.Tables.Count != ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Count)
            {
                DataWorkSource.CreateTables(ref this.profileCnt.Default.Order, ref this.profileCnt.Default.Products, ref this.profileCnt.Default.Alternative, ref this.profileCnt.Default.DiscountCards, ref Cheques);
                this.CreateOrderStructure(this.profileCnt.Default.Order);
                foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
                {
                    this.CreateOrderStructure(this.profileCnt.Default.Orders.Tables[de.Key.ToString()]);
                    this.Discount.Add(de.Key, DataWorkShared.GetStandartDiscountInfoStructure2());
                    this.Summa.Add(de.Key, DataWorkShared.GetStandartCalculationInfoStructure2());
                }
            }
            */
            //if (this.Summa.Count != ConfigManager.Instance.CommonConfiguration.PROFILES_Items.Count)
            // *** this.InitChequeInformationStructure();


            //bool notificationIsActive = false;
            // * bool _fl_updated = false;
            //uiWndUpdateWnd uw = new uiWndUpdateWnd(_fl_onlyUpdate);
            List<string> allProfiles = new List<string>();
            //foreach (DictionaryEntry de in ConfigManager.Instance.CommonConfiguration.PROFILES_Items)
            //    allProfiles.Add(de.Key.ToString());

            // add all data
            /*foreach (DataTable dt in this.ImportedData.Tables)
            {
                // get profile name and data
                // 0 - data name
                // 1 - profile name
                string[] prData = dt.TableName.Split(new[] { '=' });
                string key = prData[1];

                // add unique profile index
                if (!allProfiles.Contains(key))
                    allProfiles.Add(key);

                switch (prData[0].ToUpper())
                {
                    case "PROD":
                        {
                            //this.profileCnt.Default.Products = tables[0].Copy();
                            DataRow[] dRows = this.profileCnt.Default.Products.Select("F = " + key);
                            foreach (DataRow dr in dRows)
                                dr.Delete();
                            this.profileCnt.Default.Products.Merge(dt);
                            //wasUpdatedAtLeastOneSource = true;
                            _fl_updated = true;
                            break;
                        }
                    case "ALT":
                        {
                            DataRow[] dRows = this.profileCnt.Default.Alternative.Select("F = " + key);
                            foreach (DataRow dr in dRows)
                                dr.Delete();
                            this.profileCnt.Default.Alternative.Merge(dt);
                            _fl_updated = true;
                            break;
                        }
                    case "CARD":
                        {
                            this.profileCnt.Default.DiscountCards.Rows.Clear();
                            this.profileCnt.Default.DiscountCards.Merge(dt);
                            _fl_updated = true;
                            break;
                        }
                }

            }*/

            //_fl_onlyUpdate = true;
            //_fl_subUnitChanged = false;
            //_fl_isOk = new Com_SecureRuntime().FullLoader();
            //label_uiWndmain_DemoShowArt.Visible = label_uiWndmain_DemoShowChq.Visible = !_fl_isOk;

            /*if (_fl_updated)
            {
                /* Removing unused rows * /
                string cleanupQuery = string.Empty;
                foreach (string existedProfiles in allProfiles)
                {
                    cleanupQuery += " F <> " + existedProfiles + " AND ";
                }
                cleanupQuery = cleanupQuery.Trim(new char[] { ' ', 'A', 'N', 'D' });
                DataRow[] unusedRowsArt = this.profileCnt.Default.Products.Select(cleanupQuery);
                DataRow[] unusedRowsAlt = this.profileCnt.Default.Alternative.Select(cleanupQuery);
                foreach (DataRow dr in unusedRowsArt)
                    dr.Delete();
                foreach (DataRow dr in unusedRowsAlt)
                    dr.Delete();

                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Normal;
                //this.BringToFront();
                MMessageBoxEx.Show(this.chequeDGV, "Були внесені зміни в базу товарів", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }*/




            // * DataTable ddt = profileCnt.dataGetProducts();
            
        }

        #endregion

        #region Devices
        private void configureAdditionalDevices()
        {
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

            if (Program.AppPlugins.IsActive(PluginType.FPDriver))
            {
                try
                {
                    bool status = (bool)Program.AppPlugins.GetActive<IFPDriver>().CallFunction("FP_SetCashier", ConfigManager.Instance.CommonConfiguration.APP_PayDesk, UserConfig.UserFpLogin, UserConfig.UserFpPassword, UserConfig.UserID);
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
            }
            else
                DDM_FPStatus.Image = Properties.Resources.FpNotOk;
        }
        #endregion
    }
}
