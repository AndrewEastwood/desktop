using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace driver.Config
{
    [Serializable]
    public class AppConfig
    {
        public string p_name = "Default";
        public string p_key = "1";

        // logical groups
        // taxes
        // system
        // profiles
        public bool PROFILES_UseProfiles = false;
        public Hashtable PROFILES_Items = new Hashtable();
        public Hashtable PROFILES_updateDateTime = new Hashtable();
        public object PROFILES_LegalProgileID = null;
        // view
        // content

        #region TAX Settings
        public double[] TAX_AppTaxRates;
        public char[] TAX_AppTaxChar;
        public bool[] TAX_AppTaxDisc;

        public double[] TAX_FxRates;
        public char[] TAX_AppColumn = new char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        public char[] TAX_MarketColumn = new char[] { ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };


        public Hashtable TAX_DefinedRates = new Hashtable();
        public Hashtable TAX_Compatibility = new Hashtable();
        public Hashtable TAX_Compatibility_Template = new Hashtable()
        { 
            { ' ', ' ' },
            { 'A', ' ' },
            { 'B', ' ' },
            { 'C', ' ' },
            { 'D', ' ' },
            { 'E', ' ' },
            { 'F', ' ' },
            { 'G', ' ' },
            { 'H', ' ' },
            { 'I', ' ' },
            { 'J', ' ' },
            { 'K', ' ' },
            { 'L', ' ' },
            { 'M', ' ' },
            { 'N', ' ' },
            { 'O', ' ' },
            { 'P', ' ' },
            { 'Q', ' ' },
            { 'R', ' ' },
            { 'S', ' ' },
            { 'T', ' ' },
            { 'U', ' ' },
            { 'V', ' ' },
            { 'W', ' ' },
            { 'X', ' ' },
            { 'Y', ' ' },
            { 'Z', ' ' }
        };
        
        #endregion
        const byte vars_tax = 6;

        #region System Settings
        public int APP_MoneyDecimals = 2;
        public object[] APP_UnitFilter = new object[3] { new string[0], new bool[0], new bool[0] };
        public string APP_Admin = "0";
        public byte APP_SubUnit = 2;
        public byte APP_PayDesk = 1;
        public string APP_ClientID = "";
        public string APP_SubUnitName = "";
        public int APP_RefreshRate = 15000;
        public double APP_StartTotal = 1;
        public string APP_Language = "";
        public int APP_Printer = 1;
        public string APP_DOSCommand = "";
        public string APP_OutPrnStyle = "File";
        public bool APP_UseStaticDiscount = false;
        public bool APP_AllowOneCopy = true;
        public bool APP_UseStaticRules = false;
        public bool APP_OnlyDiscount = true;
        public double APP_StaticDiscountValue = 0.0;
        public bool APP_ShowInventWindow = false;
        public int APP_WeightType = 0;
        public int APP_SearchType = 2;
        public string APP_ChequeName = "%9%0%1_%2_%3_%4_%5_%6_%7";
        public int APP_DoseDecimals = 3;
        public byte APP_InvAutoSave = 10;
        public string[] APP_DiscountRules = new string[0];
        public bool[] APP_SrchTypesAccess = new bool[3] { false, false, true };
        //public byte APP_StaticDiscountType = 0;
        public bool APP_ClearTEMPonExit = true;
        public bool APP_ShowInfoOnIndicator = false;
        public int APP_RefreshTimeout = 100;
        public bool APP_UsePercentTypeDisc = true;
        public bool APP_UseAbsoluteTypeDisc = false;
        public int APP_DefaultTypeDisc = 0;
        public string[][] APP_PrintersLinks = new string[3][];
        public bool APP_UseCommonPrinting = false;
        public bool APP_ScannerUseProgamMode = true;
        public int APP_ScannerCharReadFrequency = 130;
        public int APP_BuyerBarCodeSource = 0;
        public int APP_BuyerBarCodeMinLen = 8;

        #endregion
        const byte vars_sys = 37;

        #region Path_Settings
        public string Path_Config = Application.StartupPath + @"\config";
        public string Path_Articles = Application.StartupPath + @"\articles";
        public string Path_Cheques = Application.StartupPath + @"\cheques";
        public string Path_Temp = Application.StartupPath + @"\temp";
        public string Path_Bills = Application.StartupPath + @"\bills";
        public string Path_Schemes = Application.StartupPath + @"\schemes";
        public string Path_Exchnage = @"";
        public string Path_Users = Application.StartupPath + @"\users";
        public string Path_Templates = Application.StartupPath + @"\templates";
        public string Path_Tpl_1 = @"";
        public string Path_Tpl_2 = @"";
        public string Path_Tpl_3 = @"";
        public Dictionary<string, string> Path_PrinterTempleates = new Dictionary<string, string>();
        public string Path_Rules = Application.StartupPath + @"\Rules.ini";
        public string Path_Units = Application.StartupPath + @"\Units.ini";
        public string Path_Plugins = Application.StartupPath + @"\plugins";
        public Dictionary<string, Dictionary<string, string>> Path_Printers = new Dictionary<string, Dictionary<string, string>>();
        public string Path_Reports = Application.StartupPath + @"\reports";
        public string Path_FeedInbox = Application.StartupPath + @"\feed_inbox";
        #endregion Path
        const byte vars_path = 18;

        #region Style
        //--- Colors 7
        public Color STYLE_BackgroundStatPan = Color.FromKnownColor(KnownColor.GradientInactiveCaption);
        public Color STYLE_BackgroundArtTbl = Color.FromArgb(216, 228, 248);
        public Color STYLE_BackgroundAChqTbl = Color.FromArgb(243, 244, 218);
        public Color STYLE_BackgroundNAChqTbl = System.Drawing.Color.White;
        public Color STYLE_BackgroundAddPan = System.Drawing.Color.White;
        public Color STYLE_BackgroundInfPan = System.Drawing.Color.White;
        public Color STYLE_BackgroundSumRest = System.Drawing.Color.White;

        // ---- Fonts 16
        public Font STYLE_SumFont = new Font("Tahoma", 36.0F, FontStyle.Bold);
        public Color STYLE_SumFontColor = System.Drawing.Color.Green;
        public Font STYLE_RestFont = new Font("Tahoma", 36.0F, FontStyle.Bold);
        public Color STYLE_RestFontColor = System.Drawing.Color.Red;
        public Font STYLE_StatusFont = new Font("Tahoma", 8.25F, FontStyle.Regular);
        public Color STYLE_StatusFontColor = System.Drawing.Color.Black;
        public Font STYLE_AppInformerFont = new Font("Tahoma", 8.25F, FontStyle.Regular);
        public Color STYLE_AppInformerFontColor = System.Drawing.Color.Black;
        public Font STYLE_AddInformerFont = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular);
        public Color STYLE_AddInformerFontColor = System.Drawing.Color.Black;
        public Font STYLE_ChqInformerFont = new Font("Tahoma", 9.75F, FontStyle.Bold);
        public Color STYLE_ChqInformerFontColor = System.Drawing.Color.Black;
        public Font STYLE_ChequeFont = new Font("Microsoft Sans Serif", 12.0F, FontStyle.Regular);
        public Color STYLE_ChequeFontColor = System.Drawing.Color.Black;
        public Font STYLE_ArticlesFont = new Font("Microsoft Sans Serif", 12.0F, FontStyle.Regular);
        public Color STYLE_ArticlesFontColor = System.Drawing.Color.Black;

        // ---- DGV 7
        public bool STYLE_ChqColumnLock = false;
        public bool STYLE_ArtColumnLock = false;
        public object[] STYLE_GridsView = new object[2] { new object[2], new object[2] };
        public string[] STYLE_ARTColumnName = new string[] { "ID", "BC", "NAME", "DESC", "UNIT", "VG", "TID", "TQ", "PACK", "WEIGHT", "PRICE", "PR1", "PR2", "PR3", "Q2", "Q3" };
        public string[] STYLE_ColumnCaption = new string[] { "Код", "Штрих-код", "Коротка назва", "Повна назва", "Одиниця", "ПДВ", "TID", "TQ", "PACK", "Вага", "Ціна", "PR1", "PR2", "PR3", "Q2", "Q3", "К-сть", "ПДВ%", "Знижка", "Знижка %", "Власна сума", "Сума", "Значення ПДВ", "К-сть", "Стартова ціна", "Надрукована к-сть" };
        public string[] STYLE_ALTColumnName = new string[] { "ABC", "AID" };
        public string[] STYLE_CARDColumnName = new string[] { "CBC", "CID", "CDISC", "CPRICENO" };

        // ---- Window 4
        public Orientation STYLE_SplitOrient = Orientation.Horizontal;
        public Point STYLE_MainWndPosition = new Point(120, 120);
        public Size STYLE_MainWndSize = new Size(640, 480);
        public bool STYLE_ArtSideCollapsed = false;

        public FormWindowState STYLE_MainWndState = FormWindowState.Normal;
        public int STYLE_SplitterDistance = 200;

        public Font STYLE_BillWindow = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);
        public Font STYLE_BillWindowEntry = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);
        public Font STYLE_BillWindowEntryItems = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);

        public int STYLE_Misc_ChequeRowHeight = 21;
        public int STYLE_Misc_ArticleRowHeight = 21;
        public int STYLE_Misc_BillItemsRowHeight = 21;
        public int STYLE_Misc_BillItemProductsRowHeight = 21;

        #endregion View
        const byte vars_style = 43;

        #region DataGridPrinter
        public Font DGP_HeaderFont = new Font("Arial", 12.8F, FontStyle.Bold);
        public Font DGP_TableFont = new Font("Tahoma", 11.1F, FontStyle.Regular);
        public Color DGP_Background = Color.White;
        public bool DGP_Interchange = true;
        public Color DGP_Color1 = Color.FromArgb(125, 125, 125);
        public Color DGP_Color2 = Color.White;
        public object[][] DGP_Columns = new object[4][];
        #endregion
        const byte vars_dgp = 7;

        #region Additional
        public DateTime[] ADD_updateDateTime = new DateTime[3] { 
                    new DateTime(),
                    new DateTime(),
                    new DateTime()
            };
        #endregion
        const byte vars_add = 1;

        #region Content
        #region Common [1]
        public bool Content_Common_PromptMsgOnIllegal = true;
        public int Content_Common_PrinterDelaySec = 60;
        #endregion
        #region Cheques [6]
        public string Content_Cheques_AddTotal = "none";
        public bool Content_Cheques_UseAddTotal = false;
        public bool Content_Cheques_UseSeparateCheque = false;
        public string Content_Cheques_SeparatedArticleMaskById = string.Empty;
        public bool Content_Cheques_UseCustomClientCardBC = false;
        public string Content_Cheques_CustomClientCardBC = string.Empty;
        public bool Content_Cheques_AddCopyToArchive = false;
        #endregion
        #region Bills [4]
        public bool Content_Bills_KeepAliveAfterCheque = true;
        public bool Content_Bills_AddCopyToArchive = false;
        public bool Content_Bills_ShowBillSumColumn = false;
        public bool Content_Bills_ShowBillTotalSum = false;
        #endregion
        #region Articles [1]
        public bool Content_Articles_KeepDataAfterImport = true;

        #endregion
        #endregion
        const byte vars_content = 12;

        #region Skin
        #region Skin Sensor
        public bool skin_sensor_active = false;
        public bool skin_sensor_com_chqnav = true;
        public bool skin_sensor_com_chqopr = true;
        public bool skin_sensor_com_chqsrch = true;
        public bool skin_sensor_com_chqbills = true;
        public bool skin_sensor_com_artnav = true;
        public bool skin_sensor_com_artscroll = true;
        public Font skin_sensor_fontsize = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);
        public int skin_sensor_com_size_cheque = 100;
        public int skin_sensor_com_size_art = 50;

        public int skin_sensor_splitter_chq_v_100 = 130;
        public int skin_sensor_splitter_chq_h_100 = 600;
        public int skin_sensor_splitter_chq_v_50 = 100;
        public int skin_sensor_splitter_chq_h_50 = 450;

        public int skin_sensor_splitter_chq_100 = 50;
        public int skin_sensor_splitter_chq_50 = 50;

        public int skin_sensor_splitter_chq_orient = 0;

        public int skin_sensor_splitter_artnav = 150;
        #endregion
        private const byte vars_skin_sensor = 12;

        #endregion
        const byte vars_skin = 1;

        // window posiotns
        public Hashtable WP_ALL = new Hashtable()
        {
            {"BILL_VIEW", new Point()},
            {"BILL_MGR", new Point()},
            {"BILL_CMT", new Point()},
            {"BILL_PRN", new Point()},
            {"PAYMENT", new Point()}
        };

        // total sections
        const byte cfgRange = 8;

    }
}